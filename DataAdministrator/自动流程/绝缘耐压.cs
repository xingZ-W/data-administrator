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
    class 绝缘耐压:Father
    {
        public 绝缘耐压(Form1 form, string FilePath2)
        {
            this.form = form;
            FilePath = FilePath2;

            positiveInsulationValue = 0;
            positiveInsulationTestTime = 0;
            positivePressureValue = 0;
            positiveTestVoltage = 0;
            positiveTestTime = 0;          
            negativeInsulationValue = 0;
            negativeInsulationTestTime = 0;
            negativePressureValue = 0;
            negativeTestVoltage = 0;
            negativeTestTime = 0;

            ycellvalue = 0;

            mesClass = new MESClass(form);

            wT6582 = new WT6582(form);
            wT6582.openSeir(form.textBox1.Text,form.textBox5.Text);
            mBT3562 = new BT3562(form);
            Thread 心跳 = new Thread(xintiao);
            心跳.IsBackground = true;
            心跳.Start();

            MySignal signal = new MySignal(this.form);// 流程结构对象
     

            signal.AddListSignal(new Signal(FindIniPath("进站触发")[0], 工位1进站, 工位1进站复位));
            signal.AddListSignal(new Signal(FindIniPath("出站触发")[0], 工位1出站, 工位1出站复位));

            MySignal signal2 = new MySignal(this.form);
            signal2.AddListSignal(new Signal(FindIniPath("正极绝缘触发")[0], 工位2进站, 工位2进站复位));
            signal2.AddListSignal(new Signal(FindIniPath("正极耐压触发")[0], 工位2出站, 工位2出站复位));

            MySignal signal3 = new MySignal(this.form);
            signal3.AddListSignal(new Signal(FindIniPath("负极绝缘触发")[0], 工位3进站, 工位3进站复位));
            signal3.AddListSignal(new Signal(FindIniPath("负极耐压触发")[0], 工位3出站, 工位3出站复位));

         
            signal.AddListSignal(new Signal(FindIniPath("ACR进站触发")[0], 工位4进站, 工位4进站复位));
            signal.AddListSignal(new Signal(FindIniPath("ACR出站触发")[0], 工位4出站, 工位4出站复位));

            //MySignal signal5 = new MySignal(this.form);
            signal.AddListSignal(new Signal(FindIniPath("Y电容进站触发")[0], 工位5进站, 工位5进站复位));
            signal.AddListSignal(new Signal(FindIniPath("Y电容出站触发")[0], 工位5出站, 工位5出站复位));

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
        public async void OCV(int index)
        {
            ocvTest = await mBT3562.GetVoltageAndResAsync("COM1", 9600, index);
        }
        public override string 工位1进站(string Code = "")
        {
            StationInTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // 信号的上升延
            form.P_Class.TxtLog("接收绝缘耐压进站信号，开始进站！", 2);
            moduleCode1 = form.PLC_ui.Read_String(FindIniPath("WIP码")[0]);
            form.P_Class.TxtLog("绝缘耐压进站WIP码：" + moduleCode1, 2);
          
            if (moduleCode1 == " ")
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "进站", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));
                form.PLC_ui.write(FindIniPath("进站NG")[0], true);
                form.P_Class.TxtLog("进站1NG，模组码为空", 2);
                return "";
            }

            if (!form.采集模式.Text.Contains("在线"))
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "进站", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));

                form.PLC_ui.write(FindIniPath("进站OK")[0], true);
                form.P_Class.TxtLog("进站OK，当前屏蔽MES", 2);
            }
            else
            {
                string url = getData2Value("绝缘耐压URL");
                string deviceCode = getData2Value("绝缘耐压设备编码");
                string processedMode = getData2Value("绝缘耐压加工模式");
                if (form.PLC_ui.Read_Bool(FindIniPath("NG回流模式")[0]))
                {
                    ////返修 模式  
                    processedMode = "1";
                    form.PLC_ui.write(FindIniPath("确认NG回流模式")[0], true);
                    form.P_Class.TxtLog("回流模式确认OK", 2);

                }
                string userOperator = getData2Value("绝缘耐压操作人");
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
              
                if (mesClass.inbound_MES(url, moduleCode1, deviceCode, processedMode, userOperator, timestamp, 2, "进站OK", "进站NG", ref hanHoProductCode))
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "进站", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                    }));
                }
                else
                {
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
            form.PLC_ui.write(FindIniPath("确认NG回流模式")[0], false);
            return "";
        }

        public override string 工位1出站(string Code = "")
        {
       
            form.P_Class.TxtLog("接收绝缘耐压出站信号，开始出站！", 2);
            moduleCode1 = form.PLC_ui.Read_String(FindIniPath("WIP码")[0]);
            form.P_Class.TxtLog("绝缘耐压出站WIP码：" + moduleCode1, 2);

            if (moduleCode1 == " ")
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "出站", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));

                form.PLC_ui.write(FindIniPath("出站NG")[0], true);
                form.P_Class.TxtLog("出站1NG，模组码为空", 2);
                return "";
            }

            string[] tempStr = new string[form.dataGridView2.Columns.Count];
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "WIP码")] = moduleCode1;
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "托盘码")] = form.PLC_ui.Read_String(FindIniPath("托盘码")[0]);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "工位")] = "绝缘耐压";
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "进站时间")] = StationInTime;
            JArray paramsArrayList = new JArray();
            JArray materialsArrayList = new JArray();

            positiveTestVoltage = Convert.ToDouble(form.textBox22.Text);
            positiveTestTime = Convert.ToDouble(form.textBox20.Text);

            negativeTestVoltage = Convert.ToDouble(form.textBox22.Text);
            negativeTestTime = Convert.ToDouble(form.textBox20.Text);

            double[] testArray = new double[10];
            testArray[0] = positiveInsulationValue;
            testArray[1] = positiveInsulationTestTime;
            testArray[2] = positivePressureValue;
            testArray[3] = positiveTestVoltage;
            testArray[4] = positiveTestTime;
            testArray[5] = negativeInsulationValue;
            testArray[6] = negativeInsulationTestTime;
            testArray[7] = negativePressureValue;
            testArray[8] = negativeTestVoltage;
            testArray[9] = negativeTestTime;

            readPLC_CS(tempStr, "绝缘耐压", testArray, "", "", "", "", 1,ref paramsArrayList, ref materialsArrayList);
          
            if (!form.采集模式.Text.Contains("在线"))
            {
              
                form.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = { moduleCode1, "出站", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                    DataGridViewClass.AddRows(form.dataGridView2, tempStr, Color.White);
                    form.P_Class.Write_CSV(form.dataGridView2, tempStr, form.工站名 );
                }));

                form.PLC_ui.write(FindIniPath("出站OK")[0], true);
                form.P_Class.TxtLog("出站OK，当前屏蔽MES", 2);
            }
            else
            {
                string url = getData3Value("绝缘耐压URL");
                string deviceCode = getData3Value("绝缘耐压设备编码");
                string isOutBound = getData3Value("绝缘耐压是否出站");
                string userOperator = getData3Value("绝缘耐压操作人");
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "WIP码")] = moduleCode1;
                tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "出站时间")] = timestamp;
                tempStr[4] = testArray[2].ToString("F3");
                if (mesClass.outbound_MES(url, moduleCode1, deviceCode, isOutBound, userOperator, timestamp, "绝缘耐压", paramsArrayList, materialsArrayList, 2, "出站OK", "出站NG"))
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "出站", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                        DataGridViewClass.AddRows(form.dataGridView2, tempStr, Color.White);
                        form.P_Class.Write_CSV(form.dataGridView2,tempStr, "绝缘耐压");
                    }));
                }
                else
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "出站", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                        DataGridViewClass.AddRows(form.dataGridView2, tempStr, Color.Red);
                       form.P_Class.Write_CSV(form.dataGridView2,tempStr, "绝缘耐压");
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

        //绝缘正极测试
        public override string 工位2进站(string Code = "")
        {
            wT6582.fuji = false;
            wT6582.zhengji = true;
            form.P_Class.TxtLog("接收绝缘正极测试信号，开始进站！", 2);
            wT6582.insulationValueTwo = 1;
            byte[] SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:IR:LEVel " + form.textBox18.Text + " \r\n");
            wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
            Thread.Sleep(50);
            SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:IR:LIMIT " + form.textBox17.Text + " \r\n");
            wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
            Thread.Sleep(50);
            SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:IR:TIME " + form.textBox16.Text + " \r\n");
            wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）

            Thread.Sleep(200);
            
            wT6582.strRecieve3 = "";
            wT6582.strRecieve_All = "";
            wT6582.pressureResistanceBool = false;
            byte[] SendBuf5 = new byte[1024 * 1024];//申请内存
            SendBuf5 = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STARt \r\n");
            wT6582.sp.Write(SendBuf5, 0, SendBuf5.Length);//从0到最后（长度）
            wT6582.insulationBool = true;
            wT6582.t1 = DateTime.Now;
            return "";
        }
        public override string 工位2进站复位(string Code = "")
        {
            form.PLC_ui.write(FindIniPath("正极绝缘NG")[0], false);
            form.PLC_ui.write(FindIniPath("正极绝缘OK")[0], false);
            return "";
        }

        //耐压正极测试
        public override string 工位2出站(string Code = "")
        {
            wT6582.fuji = false;
            wT6582.zhengji = true;
            form.P_Class.TxtLog("接收耐压正极测试信号，开始进站！", 2);
            byte[] SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:DC:LEVel " + form.textBox22.Text + " \r\n");
            wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
            Thread.Sleep(50);
            SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:DC:LIMIT " + form.textBox21.Text + " \r\n");
            wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
            Thread.Sleep(50);
            SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:DC:TIME " + form.textBox20.Text + "	\r\n");
            wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）

            Thread.Sleep(100);

            wT6582.strRecieve3 = "";
            wT6582.strRecieve_All = "";
            wT6582.insulationBool = false;
            byte[] SendBuf2 = new byte[1024 * 1024];//申请内存
            SendBuf2 = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STARt \r\n");
            wT6582.sp.Write(SendBuf2, 0, SendBuf2.Length);//从0到最后（长度）
            wT6582.pressureResistanceBool = true;

            return "";
        }
        public override string 工位2出站复位(string Code = "")
        {
            form.PLC_ui.write(FindIniPath("正极耐压NG")[0], false);
            form.PLC_ui.write(FindIniPath("正极耐压OK")[0], false);
            return "";
        }

        //绝缘负极测试
        public override string 工位3进站(string Code = "")
        {
            wT6582.zhengji = false;
            wT6582.fuji = true;
            form.P_Class.TxtLog("接收绝缘负极测试信号，开始进站！", 2);
            wT6582.insulationValueTwo = 1;
            byte[] SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:IR:LEVel " + form.textBox18.Text + " \r\n");
            wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
            Thread.Sleep(50);
            SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:IR:LIMIT " + form.textBox17.Text + " \r\n");
            wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
            Thread.Sleep(50);
            SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:IR:TIME " + form.textBox16.Text + " \r\n");
            wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）

            Thread.Sleep(200);

            wT6582.strRecieve3 = "";
            wT6582.strRecieve_All = "";
            wT6582.pressureResistanceBool = false;
            byte[] SendBuf5 = new byte[1024 * 1024];//申请内存
            SendBuf5 = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STARt \r\n");
            wT6582.sp.Write(SendBuf5, 0, SendBuf5.Length);//从0到最后（长度）
            wT6582.insulationBool = true;

            wT6582.t1 = DateTime.Now;
            return "";
        }
        public override string 工位3进站复位(string Code = "")
        {
            form.PLC_ui.write(FindIniPath("负极绝缘NG")[0], false);
            form.PLC_ui.write(FindIniPath("负极绝缘OK")[0], false);
            return "";
        }

        //耐压负极测试
        public override string 工位3出站(string Code = "")
        {
            wT6582.zhengji = false;
            wT6582.fuji = true;
            form.P_Class.TxtLog("接收耐压负极测试信号，开始进站！", 2);
            byte[] SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:DC:LEVel " + form.textBox22.Text + " \r\n");
            wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
            Thread.Sleep(50);
            SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:DC:LIMIT " + form.textBox21.Text + " \r\n");
            wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
            Thread.Sleep(50);
            SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:DC:TIME " + form.textBox20.Text + "	\r\n");
            wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）

            Thread.Sleep(100);

            wT6582.strRecieve3 = "";
            wT6582.strRecieve_All = "";
            wT6582.insulationBool = false;
            byte[] SendBuf2 = new byte[1024 * 1024];//申请内存
            SendBuf2 = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STARt \r\n");
            wT6582.sp.Write(SendBuf2, 0, SendBuf2.Length);//从0到最后（长度）
            wT6582.pressureResistanceBool = true;

            return "";
        }
        public override string 工位3出站复位(string Code = "")
        {
            form.PLC_ui.write(FindIniPath("负极耐压NG")[0], false);
            form.PLC_ui.write(FindIniPath("负极耐压OK")[0], false);
            return "";
        }

        //ACR测试
        public override string 工位4进站(string Code = "")
        {
            // 信号的上升延
            form.P_Class.TxtLog("接收ACR进站信号，开始进站！", 2);
            moduleCode1 = form.PLC_ui.Read_String(FindIniPath("WIP码2")[0]);
            form.P_Class.TxtLog("ACR进站进站WIP码：" + moduleCode1, 2);
            StationInTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (moduleCode1 == " ")
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "进站2", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                    form.P_Class.Write_CSV1(temp, "ACR测试");
                }));
                form.PLC_ui.write(FindIniPath("进站2NG")[0], true);
                form.P_Class.TxtLog("ACR进站NG，模组码为空", 2);
                return "";
            }

            if (!form.采集模式.Text.Contains("在线"))
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "进站2", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, "ACR测试");
                }));

                form.PLC_ui.write(FindIniPath("进站2OK")[0], true);
                form.P_Class.TxtLog("ACR进站OK，当前屏蔽MES", 2);
            }
            else
            {
                string url = getData2Value("ACRURL");
                string deviceCode = getData2Value("ACR设备编码");
                string processedMode = getData2Value("ACR加工模式");
                if (form.PLC_ui.Read_Bool(FindIniPath("NG回流模式")[0]))
                {
                    ////返修 模式  
                    processedMode = "1";
                    form.PLC_ui.write(FindIniPath("确认NG回流模式")[0], true);
                    form.P_Class.TxtLog("回流模式确认OK", 2);

                }
           
                string userOperator = getData2Value("ACR操作人");
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
                if (mesClass.inbound_MES(url, moduleCode1, deviceCode, processedMode, userOperator, timestamp, 2, "进站2OK", "进站2NG", ref hanHoProductCode))
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "进站2", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                        form.P_Class.Write_CSV1(temp, "ACR测试");
                    }));
                }
                else
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "进站2", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                        form.P_Class.Write_CSV1(temp, "ACR测试");
                    }));
                }
            }
            return "";
        }
        public override string 工位4进站复位(string Code = "")
        {
            form.PLC_ui.write(FindIniPath("进站2NG")[0], false);
            form.PLC_ui.write(FindIniPath("进站2OK")[0], false);
            form.PLC_ui.write(FindIniPath("确认NG回流模式")[0], false);
            return "";
        }
      
        public override string 工位4出站(string Code = "")
        {
            form.P_Class.TxtLog("接收ACR出站信号，开始出站！", 2);
            moduleCode1 = form.PLC_ui.Read_String(FindIniPath("WIP码2")[0]);
            form.P_Class.TxtLog("ACR出站WIP码：" + moduleCode1, 2);

            if (moduleCode1 == " ")
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "出站2", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, "ACR测试");
                }));

                form.PLC_ui.write(FindIniPath("出站NG")[0], true);
                form.P_Class.TxtLog("出站2NG，模组码为空", 2);
                return "";
            }
           string[] tempStr = new string[6];
            //string[] tempStr = new string[form.dataGridView2.Columns.Count];
            tempStr[0] = moduleCode1;
            tempStr[1] = form.PLC_ui.Read_String(FindIniPath("托盘码2")[0]);
            tempStr[2] = "error";
            tempStr[3] = "error";
            tempStr[4] = StationInTime;//进站时间
            tempStr[5] = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");//出站时间

            if (!form.采集模式.Text.Contains("在线"))
            {
                form.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = { moduleCode1, "出站2", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, "ACR测试");
                    DataGridViewClass.AddRows(form.dataGridView10, tempStr, Color.White);
                    form.P_Class.Write_CSV(form.dataGridView10, tempStr, "ACR测试");
                }));

                form.PLC_ui.write(FindIniPath("出站2OK")[0], true);
                form.P_Class.TxtLog("出站OK，当前屏蔽MES", 2);
            }
            else
            {
                //////ACR测试触发
                if (form.PLC_ui.Read_Bool(FindIniPath("ACR继电器切换")[0]) == true)
                {
                    if (form.PLC_ui.Read_Bool(FindIniPath("ACR触发测试")[0]) == true)
                    {
                        form.P_Class.TxtLog("收到ACR触发测试", 2);
                        ////BT3562A
                        OCV(0);
                        Thread.Sleep(100);
                        if (ocvTest)
                        {
                            voltage = mBT3562.voltage;
                            ir = mBT3562.res * 1000M;
                            //测试成功                             
                            form.PLC_ui.write(FindIniPath("ACR测试OK")[0], true);
                            form.P_Class.TxtLog("BT3562A获取数据成功！电压--" + voltage + "电阻--" + ir, 2);
                        }
                        else
                        {
                            form.PLC_ui.write(FindIniPath("出站2NG")[0], true);
                            form.PLC_ui.write(FindIniPath("ACR测试NG")[0], true);
                            form.P_Class.TxtLog("BT3562A获取数据失败！", 1);
                            return "";
                        }
                        double[] testArray = new double[15];
                        testArray[10] = double.Parse(voltage.ToString("F2"));
                        testArray[11] = double.Parse(ir.ToString("F3"));
                        JArray paramsArrayList = new JArray();
                        JArray materialsArrayList = new JArray();
                        readPLC_CS(tempStr, "ACR测试", testArray, "", "", "", "",1, ref paramsArrayList, ref materialsArrayList);

                        string url = getData3Value("ACRURL");
                        string deviceCode = getData3Value("ACR设备编码");
                        string isOutBound = getData3Value("ACR是否出站");
                        string userOperator = getData3Value("ACR操作人");
                        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        tempStr[0] = moduleCode1;
                        tempStr[1] = form.PLC_ui.Read_String(FindIniPath("托盘码2")[0]);
                        tempStr[2] = testArray[11].ToString("F3");//电阻
                        tempStr[3] = testArray[10].ToString("F2");//电压
                        tempStr[4] = StationInTime;//进站时间
                        tempStr[5] = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");//出站时间
                        if (mesClass.outbound_MES(url, moduleCode1, deviceCode, isOutBound, userOperator, timestamp, "ACR", paramsArrayList, materialsArrayList, 2, "出站2OK", "出站2NG"))
                        {
                            form.Invoke(new MethodInvoker(delegate
                            {
                                string[] temp = { moduleCode1, "出站2", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                                DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                                form.P_Class.Write_CSV1(temp, form.工站名);
                                DataGridViewClass.AddRows(form.dataGridView10, tempStr, Color.White);
                                form.P_Class.Write_CSV(form.dataGridView10, tempStr, "ACR测试");
                            }));
                        }
                        else
                        {
                            form.Invoke(new MethodInvoker(delegate
                            {
                                string[] temp = { moduleCode1, "出站2", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                                DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                                form.P_Class.Write_CSV1(temp, form.工站名);
                                DataGridViewClass.AddRows(form.dataGridView10, tempStr, Color.Red);
                                form.P_Class.Write_CSV(form.dataGridView10, tempStr, "ACR测试");
                            }));
                        }
                    }
                    else
                    {
                        form.PLC_ui.write(FindIniPath("出站2NG")[0], true);
                        form.PLC_ui.write(FindIniPath("ACR测试NG")[0], true);
                        form.P_Class.TxtLog("未收到ACR测试信号，出站NG", 2);
                    }
                }
                else
                {
                    form.PLC_ui.write(FindIniPath("出站2NG")[0], true);
                    form.PLC_ui.write(FindIniPath("ACR测试NG")[0], true);
                    form.P_Class.TxtLog("未收到PLC----ACR继电器信号，出站NG", 2);
                }
            }

            return ""; }
        public override string 工位4出站复位(string Code = "")
        {
            form.PLC_ui.write(FindIniPath("ACR测试OK")[0], false);
            form.PLC_ui.write(FindIniPath("ACR测试NG")[0], false);
            form.PLC_ui.write(FindIniPath("出站2OK")[0], false);
            form.PLC_ui.write(FindIniPath("出站2NG")[0], false);
            return ""; 
        }
        
        //Y电容测试
        public override string 工位5进站(string Code = "")
        {
            StationInTime = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
            // 信号的上升延
            form.P_Class.TxtLog("接收Y电容进站信号，开始进站！", 2);
            moduleCode1 = form.PLC_ui.Read_String(FindIniPath("WIP码3")[0]);
            form.P_Class.TxtLog("Y电容进站进站WIP码：" + moduleCode1, 2);

            if (moduleCode1 == " ")
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "进站3", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                    form.P_Class.Write_CSV1(temp, "Y电容测试");
                }));
                form.PLC_ui.write(FindIniPath("进站2NG")[0], true);
                form.P_Class.TxtLog("Y电容进站NG，模组码为空", 2);
                return "";
            }

            if (!form.采集模式.Text.Contains("在线"))
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "进站3", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, "Y电容测试");
                }));

                form.PLC_ui.write(FindIniPath("进站3OK")[0], true);
                form.P_Class.TxtLog("Y电容进站OK，当前屏蔽MES", 2);
            }
            else
            {
                string url = getData2Value("Y电容URL");
                string deviceCode = getData2Value("Y电容设备编码");
                string processedMode = getData2Value("Y电容加工模式");
                if (form.PLC_ui.Read_Bool(FindIniPath("NG回流模式")[0]))
                {
                    ////返修 模式  
                    processedMode = "1";
                    form.PLC_ui.write(FindIniPath("确认NG回流模式")[0], true);
                    form.P_Class.TxtLog("回流模式确认OK", 2);

                }
                string userOperator = getData2Value("Y电容操作人");
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (mesClass.inbound_MES(url, moduleCode1, deviceCode, processedMode, userOperator, timestamp, 2, "进站3OK", "进站3NG", ref hanHoProductCode))
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "进站3", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                        form.P_Class.Write_CSV1(temp, "Y电容测试");
                    }));
                }
                else
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "进站3", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                        form.P_Class.Write_CSV1(temp, "Y电容测试");
                    }));
                }
            }
            return "";
        }
        public override string 工位5进站复位(string Code = "")
        {
           
            form.PLC_ui.write(FindIniPath("进站3NG")[0], false);
            form.PLC_ui.write(FindIniPath("进站3OK")[0], false);
            form.PLC_ui.write(FindIniPath("确认NG回流模式")[0], false);
            return "";
        }

        public override string 工位5出站(string Code = "")
        {

            form.P_Class.TxtLog("接收Y电容出站信号，开始出站！", 2);
            moduleCode1 = form.PLC_ui.Read_String(FindIniPath("WIP码3")[0]);
            form.P_Class.TxtLog("Y电容出站WIP码：" + moduleCode1, 2);

            if (moduleCode1 == " ")
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "出站3", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, "Y电容测试");
                }));

                form.PLC_ui.write(FindIniPath("出站3NG")[0], true);
                form.P_Class.TxtLog("出站3NG，模组码为空", 2);
                return "";
            }
            string[] tempStr = new string[5];
            tempStr[0] = moduleCode1;
            tempStr[1] = form.PLC_ui.Read_String(FindIniPath("托盘码3")[0]);
            tempStr[2] = "0";
            tempStr[3] = StationInTime;
            tempStr[4] = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
     
            if (!form.采集模式.Text.Contains("在线"))
            {
                form.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = { moduleCode1, "出站3", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, "Y电容测试");
                    DataGridViewClass.AddRows(form.dataGridView11, tempStr, Color.White);
                    form.P_Class.Write_CSV(form.dataGridView11, tempStr, "Y电容");
                }));

                form.PLC_ui.write(FindIniPath("出站3OK")[0], true);
                form.P_Class.TxtLog("出站OK，当前屏蔽MES", 2);
            }
            else
            {
                ////Y电容触发
                if (form.PLC_ui.Read_Bool(FindIniPath("Y电容继电器切换")[0]) == true)
                {
                    if (form.PLC_ui.Read_Bool(FindIniPath("Y电容触发测试")[0]) == true)
                    {
                        form.P_Class.TxtLog("收到Y电容触发测试", 2);
                        ////Y电容触发                 
                        double datab = DvcyRpcClient.DvApi.Measure("YR");
                        Thread.Sleep(50);
                        if (!string.IsNullOrEmpty(datab.ToString()))
                        {
                            ycellvalue = double.Parse(datab.ToString("F2"));
                            //测试成功                             
                            form.PLC_ui.write(FindIniPath("Y电容测试OK")[0], true);
                            form.P_Class.TxtLog("Y电容测试OK获取数据成功！Y电容值--" + ycellvalue, 2);
                        }
                        else
                        {
                            form.PLC_ui.write(FindIniPath("出站3NG")[0], true);                            ;
                            form.PLC_ui.write(FindIniPath("Y电容测试NG")[0], true);
                            form.P_Class.TxtLog("Y电容测试OK获取数据失败！", 1);
                            return "";
                        }

                        JArray paramsArrayList = new JArray();
                        JArray materialsArrayList = new JArray();
                        double[] testArray = new double[15];
                        testArray[12] = ycellvalue;
                        readPLC_CS(tempStr, "Y电容", testArray, "", "", "", "",1, ref paramsArrayList, ref materialsArrayList);
                        string url = getData3Value("Y电容URL");
                        string deviceCode1 = "DEV0340";
                        string isOutBound = getData3Value("Y电容是否出站");
                        string userOperator = getData3Value("Y电容操作人");
                        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                        tempStr[0] = moduleCode1;
                        tempStr[1]= form.PLC_ui.Read_String(FindIniPath("托盘码3")[0]);
                        tempStr[2] = testArray[12].ToString("F2");
                        tempStr[3] = StationInTime;
                        tempStr[4] = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
                        if (mesClass.outbound_MES(url, moduleCode1, deviceCode1, isOutBound, userOperator, timestamp, "Y电容", paramsArrayList, materialsArrayList, 2, "出站3OK", "出站3NG"))
                        {
                            form.Invoke(new MethodInvoker(delegate
                            {
                                string[] temp = { moduleCode1, "出站3", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                                DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                                form.P_Class.Write_CSV1(temp, "Y电容测试");
                                DataGridViewClass.AddRows(form.dataGridView11, tempStr, Color.White);
                                form.P_Class.Write_CSV(form.dataGridView11, tempStr, "Y电容");
                            }));
                        }
                        else
                        {
                            form.Invoke(new MethodInvoker(delegate
                            {
                                string[] temp = { moduleCode1, "出站3", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                                DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                                form.P_Class.Write_CSV1(temp, "Y电容测试");
                                DataGridViewClass.AddRows(form.dataGridView11, tempStr, Color.Red);
                                form.P_Class.Write_CSV(form.dataGridView11, tempStr, "Y电容");

                            }));
                        }
                    }
                    else
                    {
                        form.PLC_ui.write(FindIniPath("出站3NG")[0], true); ;
                        form.PLC_ui.write(FindIniPath("Y电容测试NG")[0], true);
                        form.P_Class.TxtLog("未收到Y电容测试信号，出站NG", 2);
                    }
                }
                else 
                {
                    form.PLC_ui.write(FindIniPath("出站3NG")[0], true); ;
                    form.PLC_ui.write(FindIniPath("Y电容测试NG")[0], true);
                    form.P_Class.TxtLog("未收到PLC----Y电容继电器信号，出站NG", 2);
                }
            }

            return "";
        }
        public override string 工位5出站复位(string Code = "")
        {
            form.PLC_ui.write(FindIniPath("Y电容测试OK")[0], false);
            form.PLC_ui.write(FindIniPath("Y电容测试NG")[0], false);
            form.PLC_ui.write(FindIniPath("出站3OK")[0], false);
            form.PLC_ui.write(FindIniPath("出站3NG")[0], false);
            return "";
        }
        public override string[] Read_Data(JObject obj, string[] tempStr) { return tempStr; }
        public override string[] Read_Data2(JObject obj, string[] tempStr) { return tempStr; }
        public override string[] Read_Data3(JObject obj, string[] tempStr) { return tempStr; }
        public override string[] Read_Data4(JObject obj, string[] tempStr) { return tempStr; }
        public override string[] Read_Data5(JObject obj, string[] tempStr) { return tempStr; }
    }
}
