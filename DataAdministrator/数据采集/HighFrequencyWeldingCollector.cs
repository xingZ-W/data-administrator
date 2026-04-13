using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace DataAdministrator.数据采集
{
    /// <summary>
    /// 高频焊接参数采集器 - 1kHz 采样率
    /// 用于极耳激光焊接工站数字化系统
    /// </summary>
    public class HighFrequencyWeldingCollector : IDisposable
    {
        private readonly Form1 _form;
        private readonly CircularBuffer<WeldingSample> _buffer;
        private readonly CancellationTokenSource _cts;
        private readonly Task _collectionTask;
        private readonly string _logPrefix;
        
        // 配置参数
        private const int SAMPLE_RATE_HZ = 1000;  // 1kHz 采样率
        private const int BUFFER_SIZE_SECONDS = 10; // 缓存 10 秒数据
        private const int MAX_BUFFER_SIZE = SAMPLE_RATE_HZ * BUFFER_SIZE_SECONDS;
        
        public event Action<WeldingSample> OnSampleCollected; // 采样点事件
        public event Action<List<WeldingSample>> OnWeldingCompleted; // 焊接完成事件
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public HighFrequencyWeldingCollector(Form1 form)
        {
            _form = form;
            _logPrefix = $"[高频采集-{DateTime.Now:HH:mm:ss}]";
            _buffer = new CircularBuffer<WeldingSample>(MAX_BUFFER_SIZE);
            _cts = new CancellationTokenSource();
            
            Log("初始化高频采集器...");
            
            // 启动采集任务
            _collectionTask = Task.Run(() => CollectionLoop(_cts.Token));
            
            Log($"采集器已启动，采样率：{SAMPLE_RATE_HZ}Hz，缓冲区：{BUFFER_SIZE_SECONDS}秒");
        }
        
        /// <summary>
        /// 采集主循环
        /// </summary>
        private async Task CollectionLoop(CancellationToken token)
        {
            var sw = Stopwatch.StartNew();
            TimeSpan interval = TimeSpan.FromMilliseconds(1000.0 / SAMPLE_RATE_HZ);
            
            long sampleCount = 0;
            DateTime lastLogTime = DateTime.Now;
            
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var sampleStart = sw.Elapsed;
                    
                    // 读取所有参数（异步并行读取）
                    var sample = await ReadAllParametersAsync();
                    
                    // 写入环形缓冲区
                    _buffer.Write(sample);
                    sampleCount++;
                    
                    // 触发事件
                    OnSampleCollected?.Invoke(sample);
                    
                    // 每分钟记录一次统计
                    if ((DateTime.Now - lastLogTime).TotalMinutes >= 1)
                    {
                        Log($"采集统计：{sampleCount} 个样本/分钟，缓冲区使用：{_buffer.Count}/{_buffer.Capacity}");
                        sampleCount = 0;
                        lastLogTime = DateTime.Now;
                    }
                    
                    // 精确控制采样间隔
                    var elapsed = sw.Elapsed;
                    var nextSampleTime = (long)((elapsed.Ticks / interval.Ticks) + 1) * interval.Ticks;
                    var waitTicks = nextSampleTime - elapsed.Ticks;
                    
                    if (waitTicks > 0)
                    {
                        var waitTime = TimeSpan.FromTicks(waitTicks);
                        await Task.Delay(waitTime, token);
                    }
                }
                catch (OperationCanceledException)
                {
                    Log("采集任务已取消");
                    break;
                }
                catch (Exception ex)
                {
                    LogError($"采集异常：{ex.Message}", ex);
                    
                    // 发生异常时短暂等待，避免错误风暴
                    await Task.Delay(100, token);
                }
            }
        }
        
        /// <summary>
        /// 并行读取所有焊接参数
        /// </summary>
        private async Task<WeldingSample> ReadAllParametersAsync()
        {
            return await Task.Run(() =>
            {
                try
                {
                    // 并行读取多个参数（提高读取效率）
                    var tasks = new[]
                    {
                        Task.Run(() => ReadLaserPower()),
                        Task.Run(() => ReadWeldingSpeed()),
                        Task.Run(() => ReadFocusPosition()),
                        Task.Run(() => ReadBackReflection()),
                        Task.Run(() => ReadMeltPoolTemperature())
                    };
                    
                    Task.WaitAll(tasks);
                    
                    return new WeldingSample
                    {
                        Timestamp = DateTime.Now,
                        LaserPower = tasks[0].Result,
                        WeldingSpeed = tasks[1].Result,
                        FocusPosition = tasks[2].Result,
                        BackReflection = tasks[3].Result,
                        MeltPoolTemp = tasks[4].Result
                    };
                }
                catch (Exception ex)
                {
                    LogError($"参数读取失败：{ex.Message}", ex);
                    
                    // 返回默认值（避免数据中断）
                    return new WeldingSample
                    {
                        Timestamp = DateTime.Now,
                        LaserPower = 0,
                        WeldingSpeed = 0,
                        FocusPosition = 0,
                        BackReflection = 0,
                        MeltPoolTemp = 0
                    };
                }
            });
        }
        
        #region PLC 参数读取方法
        
        /// <summary>
        /// 读取激光功率
        /// </summary>
        private decimal ReadLaserPower()
        {
            try
            {
                var address = FindIniPath("激光功率反馈");
                if (string.IsNullOrEmpty(address[0]))
                    return 0;
                
                return (decimal)_form.PLC_ui.Read_float(address);
            }
            catch
            {
                return 0;
            }
        }
        
        /// <summary>
        /// 读取焊接速度
        /// </summary>
        private decimal ReadWeldingSpeed()
        {
            try
            {
                var address = FindIniPath("焊接速度反馈");
                if (string.IsNullOrEmpty(address[0]))
                    return 0;
                
                return (decimal)_form.PLC_ui.Read_float(address);
            }
            catch
            {
                return 0;
            }
        }
        
        /// <summary>
        /// 读取焦点位置
        /// </summary>
        private decimal ReadFocusPosition()
        {
            try
            {
                var address = FindIniPath("焦点位置反馈");
                if (string.IsNullOrEmpty(address[0]))
                    return 0;
                
                return (decimal)_form.PLC_ui.Read_float(address);
            }
            catch
            {
                return 0;
            }
        }
        
        /// <summary>
        /// 读取背向反射强度
        /// </summary>
        private decimal ReadBackReflection()
        {
            try
            {
                var address = FindIniPath("背向反射反馈");
                if (string.IsNullOrEmpty(address[0]))
                    return 0;
                
                return (decimal)_form.PLC_ui.Read_float(address);
            }
            catch
            {
                return 0;
            }
        }
        
        /// <summary>
        /// 读取熔池温度
        /// </summary>
        private decimal ReadMeltPoolTemperature()
        {
            try
            {
                var address = FindIniPath("熔池温度反馈");
                if (string.IsNullOrEmpty(address[0]))
                    return 0;
                
                return (decimal)_form.PLC_ui.Read_float(address);
            }
            catch
            {
                return 0;
            }
        }
        
        #endregion
        
        #region 辅助方法
        
        /// <summary>
        /// 查找 PLC 地址
        /// </summary>
        private string[] FindIniPath(string pathName)
        {
            for (int i = 0; i < _form.Set_Ui.dataGridView20.Rows.Count; i++)
            {
                if (DataGridViewClass.GetRowsData(_form.Set_Ui.dataGridView20, i)[0].ToString() == pathName)
                {
                    return new[]
                    {
                        DataGridViewClass.GetRowsData(_form.Set_Ui.dataGridView20, i)[1].ToString(),
                        DataGridViewClass.GetRowsData(_form.Set_Ui.dataGridView20, i)[2].ToString()
                    };
                }
            }
            _form.P_Class.TxtLog($"检测不到 PLC 的【{pathName}】地址", 2);
            return new[] { "", "" };
        }
        
        /// <summary>
        /// 日志记录
        /// </summary>
        private void Log(string message)
        {
            _form.P_Class.TxtLog($"{_logPrefix} {message}", 2);
        }
        
        /// <summary>
        /// 错误日志
        /// </summary>
        private void LogError(string message, Exception ex = null)
        {
            var errorMsg = $"{_logPrefix} [ERROR] {message}";
            if (ex != null)
                errorMsg += $"\n异常详情：{ex.Message}\n{ex.StackTrace}";
            
            _form.P_Class.TxtLog(errorMsg, 1);
        }
        
        #endregion
        
        #region 公共方法
        
        /// <summary>
        /// 获取最近的焊接数据（用于质量判定）
        /// </summary>
        public List<WeldingSample> GetRecentWeldingData(int milliseconds = 5000)
        {
            int count = (int)(_buffer.Count * milliseconds / (BUFFER_SIZE_SECONDS * 1000));
            return _buffer.ReadLast(count);
        }
        
        /// <summary>
        /// 清空缓冲区（新焊缝开始）
        /// </summary>
        public void ClearBuffer()
        {
            _buffer.Clear();
            Log("缓冲区已清空");
        }
        
        /// <summary>
        /// 停止采集
        /// </summary>
        public void Stop()
        {
            _cts.Cancel();
            _collectionTask.Wait(TimeSpan.FromSeconds(2));
            Log("采集器已停止");
        }
        
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Stop();
            _cts.Dispose();
            _buffer.Clear();
        }
        
        #endregion
    }
    
    /// <summary>
    /// 焊接参数采样点模型
    /// </summary>
    public class WeldingSample
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public DateTime Timestamp { get; set; }
        
        /// <summary>
        /// 激光功率 (W)
        /// </summary>
        public decimal LaserPower { get; set; }
        
        /// <summary>
        /// 焊接速度 (mm/s)
        /// </summary>
        public decimal WeldingSpeed { get; set; }
        
        /// <summary>
        /// 焦点位置 (mm)
        /// </summary>
        public decimal FocusPosition { get; set; }
        
        /// <summary>
        /// 背向反射强度
        /// </summary>
        public decimal BackReflection { get; set; }
        
        /// <summary>
        /// 熔池温度 (°C)
        /// </summary>
        public decimal MeltPoolTemp { get; set; }
        
        /// <summary>
        /// 等离子光谱数据 (JSON 格式)
        /// </summary>
        public string PlasmaSpectrum { get; set; }
        
        /// <summary>
        /// 转为字符串
        /// </summary>
        public override string ToString()
        {
            return $"[{Timestamp:HH:mm:ss.fff}] 功率:{LaserPower}W, 速度:{WeldingSpeed}mm/s, 焦点:{FocusPosition}mm";
        }
    }
}
