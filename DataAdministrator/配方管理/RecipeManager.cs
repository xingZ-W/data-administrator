using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DataAdministrator.配方管理
{
    /// <summary>
    /// 配方管理器 - 支持云端同步和本地缓存
    /// </summary>
    public class RecipeManager
    {
        private readonly Form1 _form;
        private readonly string _dbPath;
        private readonly MESService _mesService;
        private readonly RecipeCache _localCache;
        private readonly string _logPrefix;
        
        /// <summary>
        /// 构造函数
        /// </summary>
        public RecipeManager(Form1 form)
        {
            _form = form;
            _logPrefix = $"[配方管理-{DateTime.Now:HH:mm:ss}]";
            
            // 数据库路径
            _dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "Recipes.db");
            
            // 初始化 MES 服务
            _mesService = new MESService(form);
            
            // 初始化本地缓存
            _localCache = new RecipeCache(_dbPath);
            
            Log("配方管理器初始化完成");
        }
        
        /// <summary>
        /// 扫码后自动加载配方
        /// </summary>
        public async Task<RecipeResult> AutoLoadRecipe(string productQRCode)
        {
            Log($"开始自动加载配方，产品二维码：{productQRCode}");
            
            try
            {
                // 1. 从 MES 获取产品信息
                var productInfo = await _mesService.GetProductInfo(productQRCode);
                if (productInfo == null)
                {
                    return RecipeResult.Fail("未找到产品信息");
                }
                
                Log($"产品信息：型号={productInfo.ModelId}, 工单={productInfo.WorkOrderNo}");
                
                // 2. 从云端下载最新配方
                var recipe = await _mesService.DownloadRecipe(productInfo.ModelId);
                
                // 3. 如果网络失败，使用本地缓存
                if (recipe == null)
                {
                    Log("MES 连接失败，尝试使用本地缓存...");
                    recipe = _localCache.GetLatestRecipe(productInfo.ModelId);
                    
                    if (recipe == null)
                    {
                        return RecipeResult.Fail($"未找到型号【{productInfo.ModelId}】的配方");
                    }
                    
                    Log($"使用本地缓存配方：{recipe.RecipeId}");
                }
                else
                {
                    Log($"从 MES 下载配方成功：{recipe.RecipeId}");
                    
                    // 4. 保存到本地缓存
                    _localCache.SaveRecipe(recipe);
                }
                
                // 5. 配方验证
                var validation = ValidateRecipe(recipe);
                if (!validation.IsValid)
                {
                    return RecipeResult.Fail($"配方验证失败：{validation.ErrorMessage}");
                }
                
                // 6. 下发至 PLC
                await SendToPLC(recipe);
                
                // 7. HMI 显示
                ShowRecipeOnHMI(recipe);
                
                // 8. 记录使用历史
                RecordUsage(productQRCode, recipe.RecipeId);
                
                Log($"配方加载成功：{recipe.RecipeId}");
                return RecipeResult.Success(recipe);
            }
            catch (Exception ex)
            {
                LogError($"配方加载失败：{ex.Message}", ex);
                return RecipeResult.Fail($"异常：{ex.Message}");
            }
        }
        
        /// <summary>
        /// 手动选择配方（兼容旧模式）
        /// </summary>
        public RecipeResult ManualSelectRecipe(string productModel)
        {
            Log($"手动选择配方，型号：{productModel}");
            
            try
            {
                var recipe = _localCache.GetLatestRecipe(productModel);
                
                if (recipe == null)
                {
                    return RecipeResult.Fail($"未找到型号【{productModel}】的配方");
                }
                
                // 验证
                var validation = ValidateRecipe(recipe);
                if (!validation.IsValid)
                {
                    return RecipeResult.Fail($"配方验证失败：{validation.ErrorMessage}");
                }
                
                // 下发 PLC
                SendToPLC(recipe).Wait();
                
                // 显示
                ShowRecipeOnHMI(recipe);
                
                Log($"手动配方选择成功：{recipe.RecipeId}");
                return RecipeResult.Success(recipe);
            }
            catch (Exception ex)
            {
                LogError($"手动配方选择失败：{ex.Message}", ex);
                return RecipeResult.Fail($"异常：{ex.Message}");
            }
        }
        
        /// <summary>
        /// 配方验证
        /// </summary>
        private ValidationMessage ValidateRecipe(Recipe recipe)
        {
            // 1. 检查必填字段
            if (string.IsNullOrEmpty(recipe.RecipeId))
                return ValidationMessage.Invalid("配方 ID 不能为空");
            
            if (string.IsNullOrEmpty(recipe.ProductModel))
                return ValidationMessage.Invalid("产品型号不能为空");
            
            // 2. 检查参数范围
            foreach (var param in recipe.Parameters.Values)
            {
                if (param.Value < param.MinLimit || param.Value > param.MaxLimit)
                {
                    return ValidationMessage.Invalid(
                        $"参数【{param.Name}】超出范围：{param.Value} ({param.MinLimit}~{param.MaxLimit})");
                }
            }
            
            // 3. 检查版本
            if (string.IsNullOrEmpty(recipe.Version))
                return ValidationMessage.Invalid("版本号不能为空");
            
            return ValidationMessage.Valid();
        }
        
        /// <summary>
        /// 下发配方到 PLC
        /// </summary>
        private async Task SendToPLC(Recipe recipe)
        {
            Log("开始下发配方到 PLC...");
            
            try
            {
                // 写入激光功率
                if (recipe.Parameters.TryGetValue("LaserPower", out var powerParam))
                {
                    var address = FindPLCAddress("激光功率设定");
                    if (!string.IsNullOrEmpty(address[0]))
                    {
                        await Task.Run(() => _form.PLC_ui.write(address, (float)powerParam.Value));
                    }
                }
                
                // 写入焊接速度
                if (recipe.Parameters.TryGetValue("WeldingSpeed", out var speedParam))
                {
                    var address = FindPLCAddress("焊接速度设定");
                    if (!string.IsNullOrEmpty(address[0]))
                    {
                        await Task.Run(() => _form.PLC_ui.write(address, (float)speedParam.Value));
                    }
                }
                
                // 写入焦点位置
                if (recipe.Parameters.TryGetValue("FocusPosition", out var focusParam))
                {
                    var address = FindPLCAddress("焦点位置设定");
                    if (!string.IsNullOrEmpty(address[0]))
                    {
                        await Task.Run(() => _form.PLC_ui.write(address, (float)focusParam.Value));
                    }
                }
                
                Log("配方参数下发完成");
            }
            catch (Exception ex)
            {
                LogError($"PLC 下发失败：{ex.Message}", ex);
                throw new Exception("PLC 下发失败，请检查通讯状态");
            }
        }
        
        /// <summary>
        /// HMI 显示配方信息
        /// </summary>
        private void ShowRecipeOnHMI(Recipe recipe)
        {
            try
            {
                _form.Invoke(new Action(() =>
                {
                    // 更新界面显示（假设主窗口有配方显示控件）
                    var label = _form.Controls.Find("label_配方信息", true);
                    if (label.Length > 0 && label[0] is Label lbl)
                    {
                        lbl.Text = $"当前配方：{recipe.RecipeId}\n" +
                                  $"型号：{recipe.ProductModel}\n" +
                                  $"版本：{recipe.Version}\n" +
                                  $"功率：{recipe.Parameters["LaserPower"]?.Value}W\n" +
                                  $"速度：{recipe.Parameters["WeldingSpeed"]?.Value}mm/s";
                    }
                    
                    _form.P_Class.TxtLog($"HMI 已显示配方：{recipe.RecipeId}", 2);
                }));
            }
            catch (Exception ex)
            {
                LogError($"HMI 显示失败：{ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// 记录配方使用历史
        /// </summary>
        private void RecordUsage(string productSN, string recipeId)
        {
            try
            {
                using (var conn = new SQLiteConnection($"Data Source={_dbPath}"))
                {
                    conn.Open();
                    var cmd = new SQLiteCommand(@"
                        INSERT INTO RecipeUsage (ProductSN, RecipeId, UseTime, Operator)
                        VALUES (@ProductSN, @RecipeId, @UseTime, @Operator)", conn);
                    
                    cmd.Parameters.AddWithValue("@ProductSN", productSN);
                    cmd.Parameters.AddWithValue("@RecipeId", recipeId);
                    cmd.Parameters.AddWithValue("@UseTime", DateTime.Now);
                    cmd.Parameters.AddWithValue("@Operator", _form.MySignl.MES_operator);
                    
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                LogError($"记录使用历史失败：{ex.Message}", ex);
            }
        }
        
        /// <summary>
        /// 查找 PLC 地址
        /// </summary>
        private string[] FindPLCAddress(string paramName)
        {
            for (int i = 0; i < _form.Set_Ui.dataGridView20.Rows.Count; i++)
            {
                if (DataGridViewClass.GetRowsData(_form.Set_Ui.dataGridView20, i)[0].ToString() == paramName)
                {
                    return new[]
                    {
                        DataGridViewClass.GetRowsData(_form.Set_Ui.dataGridView20, i)[1].ToString(),
                        DataGridViewClass.GetRowsData(_form.Set_Ui.dataGridView20, i)[2].ToString()
                    };
                }
            }
            return new[] { "", "" };
        }
        
        #region 日志方法
        
        private void Log(string message)
        {
            _form.P_Class.TxtLog($"{_logPrefix} {message}", 2);
        }
        
        private void LogError(string message, Exception ex)
        {
            var errorMsg = $"{_logPrefix} [ERROR] {message}";
            if (ex != null)
                errorMsg += $"\n异常详情：{ex.Message}\n{ex.StackTrace}";
            
            _form.P_Class.TxtLog(errorMsg, 1);
        }
        
        #endregion
    }
    
    /// <summary>
    /// 配方操作结果
    /// </summary>
    public class RecipeResult
    {
        public bool IsSuccess { get; set; }
        public Recipe Recipe { get; set; }
        public string ErrorMessage { get; set; }
        
        public static RecipeResult Success(Recipe recipe)
        {
            return new RecipeResult { IsSuccess = true, Recipe = recipe };
        }
        
        public static RecipeResult Fail(string error)
        {
            return new RecipeResult { IsSuccess = false, ErrorMessage = error };
        }
    }
    
    /// <summary>
    /// 验证消息
    /// </summary>
    public class ValidationMessage
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
        
        public static ValidationMessage Valid()
        {
            return new ValidationMessage { IsValid = true };
        }
        
        public static ValidationMessage Invalid(string error)
        {
            return new ValidationMessage { IsValid = false, ErrorMessage = error };
        }
    }
}
