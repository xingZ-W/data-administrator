using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImSignal;
using System.Threading;
using DataAdministraction.公用函数类;
using System.Drawing;
using System.Net;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using JP_DLL;
using DataAdministraction.MES_Coonect;

namespace DataAdministrator.自动流程
{
    class U壳体上料涂胶 : Father
    {
        public U壳体上料涂胶(Form1 form, string FilePath2)
        {
            this.form = form;
            FilePath = FilePath2;
            MES_operator = "user";

            mesClass = new MESClass(form); 

            Thread 心跳 = new Thread(xintiao);
            心跳.IsBackground = true;
            心跳.Start();

            MySignal signal11 = new MySignal(this.form);// 流程结构对象
            signal11.AddListSignal(new Signal(FindIniPath("进站触发")[0], 工位1进站, 工位1进站复位));
            signal11.AddListSignal(new Signal(FindIniPath("出站触发")[0], 工位1出站, 工位1出站复位));

            MySignal signal2 = new MySignal(this.form);// 流程结构对象
            signal2.AddListSignal(new Signal(FindIniPath("入壳进站触发")[0], 工位2进站, 工位2进站复位));
            signal2.AddListSignal(new Signal(FindIniPath("入壳出站触发")[0], 工位2出站, 工位2出站复位));

            MySignal signal3 = new MySignal(this.form);// 流程结构对象
            signal3.AddListSignal(new Signal(FindIniPath("称重触发")[0], 工位3进站, 工位3进站复位));
           
        }

        public override void xintiao()
        {
            while (true)
            {
                if (form.PLC_ui.connect == null || !form.PLC_ui.connect.IsSuccess)
                {
                    form.运行状态.Text = "未运行";
                    form.运行状态.ForeColor = Color.Red;

                    Thread.Sleep(500);
                    continue;
                }
                try
                {
                    string tempIp = FindIniPath("心跳")[0];
                    form.PLC_ui.write(tempIp, true);

                    Thread.Sleep(100);
                    form.PLC_ui.write(tempIp, false);

                    Thread.Sleep(100);
                    form.运行状态.Text = "正常运行";
                    form.运行状态.ForeColor = Color.Black;
                    GC.Collect();
                }
                catch
                {
                    form.P_Class.TxtLog("心跳发送异常！", 2, "异常日志");
                    form.运行状态.Text = "未运行";
                    form.运行状态.ForeColor = Color.Red;
                }
            }
        }
        
        public override string 工位1进站(string Code = "")
        {
            StationInTime = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
            // 信号的上升延
            form.P_Class.TxtLog("接收U壳体上料涂胶进站信号，开始进站！", 2);
            moduleCode1 = form.PLC_ui.Read_String(FindIniPath("WIP码")[0]);
            form.P_Class.TxtLog("WIP码：" + moduleCode1, 2);

            if (moduleCode1 == " ")
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "进站", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));

                form.PLC_ui.write(FindIniPath("进站NG")[0], true);
                form.P_Class.TxtLog("进站NG，模组码为空", 2);
                return "";
            }



            if (!form.采集模式.Text.Contains("在线"))
            {
                form.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = { moduleCode1, "进站", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));

                form.PLC_ui.write(FindIniPath("进站OK")[0], true);
                form.P_Class.TxtLog("进站OK，当前屏蔽MES", 2);
                return "";
            }

            else
            {
                string url = getData2Value("U壳涂胶URL");
                string deviceCode = getData2Value("U壳涂胶设备编码");
                string processedMode = getData2Value("U壳涂胶加工模式");
                string userOperator = getData2Value("U壳涂胶操作人");
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (mesClass.inbound_MES(url, moduleCode1, deviceCode, processedMode, userOperator, timestamp, 2, "进站OK", "进站NG", ref tuJiaoProductCode))
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "进站", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                    }));
                }
                else {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "进站", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                    }));
                }
            }

            

            return ""; 
        }
        public override string 工位1进站复位(string Code = "")
        {
          
            form.PLC_ui.write(FindIniPath("进站NG")[0], false);
            form.PLC_ui.write(FindIniPath("进站OK")[0], false);    
            return "";
        }
        public override string 工位1出站(string Code = "")
        {
            //if (!form.PLC_ui.Read_Bool(FindIniPath("进站触发")[0]))
            //{
            //    form.P_Class.TxtLog("U壳体上料涂胶进站信号已复位", 1);
            //}
            //else
            //{
            //    form.P_Class.TxtLog("U壳体上料涂胶进站信号未复位，请PLC确认！", 1);
            //    return "";
            //}

            form.P_Class.TxtLog("接收U壳体上料涂胶出站信号，开始出站！", 2);
            moduleCode1 = form.PLC_ui.Read_String(FindIniPath("WIP码")[0]);
            form.P_Class.TxtLog("WIP码：" + moduleCode1, 2);

            if (moduleCode1 == " ")
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "出站", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));

                form.PLC_ui.write(FindIniPath("出站NG")[0], true);
                form.P_Class.TxtLog("出站NG，模组码为空", 2);
                return "";
            }

            string[] tempStr = new string[form.dataGridView2.Columns.Count];
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "WIP码")] = moduleCode1;
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "托盘码")] = form.PLC_ui.Read_String(FindIniPath("托盘码")[0]);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "涂胶开始时间")] = form.PLC_ui.Read_String(FindIniPath("涂胶开始时间")[0]);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "工位")] = "U壳体上料涂胶";
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "进站时间")] = StationInTime;


            JArray paramsArrayList = new JArray();
            JArray materialsArrayList = new JArray();
            string urlCheck = getData3Value("物料效验URL");
            string deviceCodeCheck = getData3Value("物料效验设备编码");
            string userOperatorCheck = getData3Value("物料效验操作人");
            string[] tempText = readPLC_CS(tempStr, "U壳体上料涂胶", urlCheck, tuJiaoProductCode, deviceCodeCheck, userOperatorCheck, 1,ref paramsArrayList, ref materialsArrayList);
            if (tempText == null)
            {
                form.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = { moduleCode1, "出站", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                    DataGridViewClass.AddRows(form.dataGridView2, tempStr, Color.Red);
                    form.P_Class.Write_CSV(form.dataGridView2,tempStr, "U壳体上料涂胶");
                    form.PLC_ui.write(FindIniPath("出站NG")[0], true);
                    form.P_Class.TxtLog("物料效验未通过", 2);
                }));
                return "";
            }

            if (!form.采集模式.Text.Contains("在线"))
            {
                form.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = { moduleCode1, "出站", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                    DataGridViewClass.AddRows(form.dataGridView2, tempStr, Color.White);
                    form.P_Class.Write_CSV(form.dataGridView2,tempStr, "U壳体上料涂胶");
                }));

                form.PLC_ui.write(FindIniPath("出站OK")[0], true);
                form.P_Class.TxtLog("出站OK，当前屏蔽MES", 2);
                return "";
            }
            else {
                string url = getData3Value("U壳涂胶URL");
                string deviceCode = getData3Value("U壳涂胶设备编码");
                string isOutBound = getData3Value("U壳涂胶是否出站");
                string userOperator = getData3Value("U壳涂胶操作人");
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "出站时间")] = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
            
                if (mesClass.outbound_MES(url, moduleCode1, deviceCode, isOutBound, userOperator, timestamp, "U壳体上料涂胶", paramsArrayList, materialsArrayList, 2, "出站OK", "出站NG"))
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "出站", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                        DataGridViewClass.AddRows(form.dataGridView2, tempStr, Color.White);
                        form.P_Class.Write_CSV(form.dataGridView2,tempStr, "U壳体上料涂胶");
                    }));
                }
                else {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "出站", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                        DataGridViewClass.AddRows(form.dataGridView2, tempStr, Color.Red);
                        form.P_Class.Write_CSV(form.dataGridView2, tempStr, "U壳体上料涂胶");
                    }));
                }
            }
            return "";
        }
        public override string 工位1出站复位(string Code = "")
        {
            form.PLC_ui.write(FindIniPath("出站NG")[0], false);
            form.PLC_ui.write(FindIniPath("出站OK")[0], false);
            return "";
        }

        public override string 工位2进站(string Code = "")
        {
            StationInTime = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
            // 信号的上升延
            form.P_Class.TxtLog("接收入壳进站信号，开始进站！", 2);
            moduleCode2 = form.PLC_ui.Read_String(FindIniPath("入壳WIP码")[0]);
            form.P_Class.TxtLog("入壳WIP码：" + moduleCode2, 2);
            if (moduleCode2 == " ")
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode2, "入壳进站", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));
                form.PLC_ui.write(FindIniPath("入壳进站NG")[0], true);
                form.P_Class.TxtLog("入壳进站NG，模组码为空", 2);
                return "";
            }

            if (!form.采集模式.Text.Contains("在线"))
            {
                form.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = { moduleCode2, "入壳进站", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));

                form.PLC_ui.write(FindIniPath("入壳进站OK")[0], true);
                form.P_Class.TxtLog("入壳进站OK，当前屏蔽MES", 2);
                return "";
            }
            else {
                string url = getData2Value("入壳URL");
                string deviceCode = getData2Value("入壳设备编码");
                string processedMode = getData2Value("入壳加工模式");
                string userOperator = getData2Value("入壳操作人");
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                if (mesClass.inbound_MES(url, moduleCode2, deviceCode, processedMode, userOperator, timestamp, 2, "入壳进站OK", "入壳进站NG", ref ruKeProductCode))
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode2, "入壳进站", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                    }));
                }
                else {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode2, "入壳进站", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                    }));
                }
            }
            return "";
        }
        public override string 工位2进站复位(string Code = "")
        {
            form.PLC_ui.write(FindIniPath("入壳进站NG")[0], false);
            form.PLC_ui.write(FindIniPath("入壳进站OK")[0], false);       
            return "";
        }

        public override string 工位2出站(string Code = "")
        {
            form.P_Class.TxtLog("接收入壳出站信号，开始出站！", 2);
            moduleCode2 = form.PLC_ui.Read_String(FindIniPath("入壳WIP码")[0]);
            form.P_Class.TxtLog("入壳WIP码：" + moduleCode2, 2);

            if (moduleCode2 == " ")
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode2, "入壳出站", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));

                form.PLC_ui.write(FindIniPath("入壳出站NG")[0], true);
                form.P_Class.TxtLog("入壳出站NG，模组码为空", 2);
                return "";
            }

            string[] tempStr = new string[form.dataGridViewKIN.Columns.Count];
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridViewKIN, "WIP码")] = moduleCode2;
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridViewKIN, "托盘码")] = form.PLC_ui.Read_String(FindIniPath("托盘码")[0]);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridViewKIN, "工位")] = "电芯入壳";
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridViewKIN, "进站时间")] = StationInTime;
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridViewKIN, "出站时间")] = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
            JArray paramsArrayList = new JArray();
            JArray materialsArrayList = new JArray();

            string urlCheck = getData3Value("入壳物料效验URL");
            string deviceCodeCheck = getData3Value("入壳物料效验设备编码");
            string userOperatorCheck = getData3Value("入壳物料效验操作人");

            string[] tempText = readPLC_CS1(tempStr, "电芯入壳", urlCheck, ruKeProductCode, deviceCodeCheck, userOperatorCheck,1, ref paramsArrayList, ref materialsArrayList);
            if (tempText == null)
           
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode2, "入壳出站", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));

                form.PLC_ui.write(FindIniPath("入壳出站NG")[0], true);
                form.P_Class.TxtLog("物料编码校验失败", 2);
                return "";
            }
            if (!form.采集模式.Text.Contains("在线"))
            {
                form.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = { moduleCode2, "入壳出站", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                    DataGridViewClass.AddRows(form.dataGridViewKIN, tempStr, Color.White);
                    form.P_Class.Write_CSV(form.dataGridViewKIN, tempStr, "电芯入壳");
                }));

                form.PLC_ui.write(FindIniPath("入壳出站OK")[0], true);
                form.P_Class.TxtLog("入壳出站OK，当前屏蔽MES", 2);
                return "";
            }
            else {
                string replaceTempProductSnUrl = getData3Value("替换接口URL");
                string tempProductSn = form.PLC_ui.Read_String(FindIniPath("模组虚拟码")[0]);
                string replaceTempProductSnDeviceCode = getData3Value("替换接口设备编码");
                string replaceTempProductSnUserOperator = getData3Value("替换接口操作人");
                string replaceTempProductSnTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridViewKIN, "WIP码")] = moduleCode2;
                tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridViewKIN, "托盘码")] = form.PLC_ui.Read_String(FindIniPath("托盘码")[0]);
                tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridViewKIN, "出站时间")] = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
                tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridViewKIN, "模组虚拟码")] = tempProductSn;
                //float a = float.Parse(tempStr[2])*1000;
                tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridViewKIN, "入壳压力(N)")] = tempStr[2];
                tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridViewKIN, "堆叠压力(N)")] = tempStr[3];
                if (!mesClass.barcodeReplacement(replaceTempProductSnUrl, tempProductSn, moduleCode2, replaceTempProductSnDeviceCode, replaceTempProductSnUserOperator, replaceTempProductSnTimestamp))
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode2, "入壳出站", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                        DataGridViewClass.AddRows(form.dataGridViewKIN, tempStr, Color.Red);
                        form.P_Class.Write_CSV(form.dataGridViewKIN, tempStr, "电芯入壳");
                    }));

                    form.PLC_ui.write(FindIniPath("入壳出站NG")[0], true);
                    form.P_Class.TxtLog("虚拟模组码替换失败", 2);
                    return "";
                }
                form.P_Class.TxtLog("虚拟模组码替换成功，准备出站", 2);

          
                string url = getData3Value("入壳URL");
                string deviceCode = getData3Value("入壳设备编码");
                string isOutBound = getData3Value("入壳是否出站");
                string userOperator = getData3Value("入壳操作人");
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (mesClass.outbound_MES(url, moduleCode2, deviceCode, isOutBound, userOperator, timestamp, "电芯入壳", paramsArrayList, materialsArrayList, 2, "入壳出站OK", "入壳出站NG"))
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode2, "入壳出站", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                        DataGridViewClass.AddRows(form.dataGridViewKIN, tempStr, Color.White);
                        form.P_Class.Write_CSV(form.dataGridViewKIN, tempStr, "电芯入壳");
                    }));
                }
                else {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode2, "入壳出站", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                        DataGridViewClass.AddRows(form.dataGridViewKIN, tempStr, Color.Red);
                        form.P_Class.Write_CSV(form.dataGridViewKIN, tempStr, "电芯入壳");
                    }));
                }
            }

            
            return "";
        }
        public override string 工位2出站复位(string Code = "")
        {
            form.PLC_ui.write(FindIniPath("入壳出站NG")[0], false);
            form.PLC_ui.write(FindIniPath("入壳出站OK")[0], false);        
            return "";
        }

        /// <summary>
        /// 称重获取数据
        /// </summary>
        public async void Pressure()
        {
            pressureTest = await mL100B.GetVoltageAndResAsync("COM1", 19200);
        }

        /// plc触发称重读取信号    
        public override string 工位3进站(string Code = "") 
        {
            // 信号的上升延
            float data = 0;
            form.P_Class.TxtLog("接收PLC开始称重信号！", 2);
            Pressure();
            Thread.Sleep(100);
            if (pressureTest)
            {
                data = mL100B.pressure;
                //测试成功             
                form.P_Class.TxtLog("称重获取数据成功！重量--" + data + "kg", 2);
                form.PLC_ui.write(FindIniPath("称重完成")[0], true);
                form.PLC_ui.write(FindIniPath("称重数据")[0], data);
                form.P_Class.TxtLog("称重数据写入PLC" + data + "kg", 2);
            }
            else
            {
                data = 0;
                form.P_Class.TxtLog("称重获取数据失败！"+data, 1);
            }

           
            return "";
        }
        public override string 工位3进站复位(string Code = "") 
        {
            float a = 0;
            form.PLC_ui.write(FindIniPath("称重完成")[0],false);
            form.PLC_ui.write(FindIniPath("称重数据")[0], a);
            return ""; 
        }
        public override string 工位3出站(string Code = "") { return ""; }
        public override string 工位3出站复位(string Code = "") { return ""; }

        public override string 工位4进站(string Code = "") { return ""; }
        public override string 工位4进站复位(string Code = "") { return ""; }
        public override string 工位4出站(string Code = "") { return ""; }
        public override string 工位4出站复位(string Code = "") { return ""; }

        public override string 工位5进站(string Code = "") { return ""; }
        public override string 工位5进站复位(string Code = "") { return ""; }
        public override string 工位5出站(string Code = "") { return ""; }
        public override string 工位5出站复位(string Code = "") { return ""; }

        public override string[] Read_Data(JObject obj, string[] tempStr) { return tempStr; }
        public override string[] Read_Data2(JObject obj, string[] tempStr) { return tempStr; }
        public override string[] Read_Data3(JObject obj, string[] tempStr) { return tempStr; }
        public override string[] Read_Data4(JObject obj, string[] tempStr) { return tempStr; }
        public override string[] Read_Data5(JObject obj, string[] tempStr) { return tempStr; }
    }
}
