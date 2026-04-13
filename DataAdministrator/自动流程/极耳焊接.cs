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
    class 极耳焊接 : Father
    {
        public 极耳焊接(Form1 form, string FilePath2)
        {
            this.form = form;
            FilePath = FilePath2;
            MES_operator = "user";

            mesClass = new MESClass(form);

            Thread 心跳 = new Thread(xintiao);
            心跳.IsBackground = true;
            心跳.Start();

            MySignal signal = new MySignal(this.form);// 流程结构对象
            signal.AddListSignal(new Signal(FindIniPath("进站1触发")[0], 工位1进站, 工位1进站复位));
            signal.AddListSignal(new Signal(FindIniPath("出站1触发")[0], 工位1出站, 工位1出站复位));
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
            form.P_Class.TxtLog("接收极耳焊接进站1信号，开始进站！", 2);
            moduleCode1 = form.PLC_ui.Read_String(FindIniPath("WIP码")[0]);
            form.P_Class.TxtLog("极耳焊接1进站WIP码：" + moduleCode1, 2);

            if (moduleCode1 == " ")
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "进站1", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));
                form.PLC_ui.write(FindIniPath("进站1NG")[0], true);
                form.P_Class.TxtLog("进站1NG，模组码为空", 2);
                return "";
            }

            if (!form.采集模式.Text.Contains("在线"))
            {
                form.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = { moduleCode1, "进站1", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));

                form.PLC_ui.write(FindIniPath("进站1OK")[0], true);
                form.P_Class.TxtLog("进站1OK，当前屏蔽MES", 2);
            }
            else {
                string url = getData2Value("极耳焊接URL");
                string deviceCode = getData2Value("极耳焊接设备编码");
                string processedMode = getData2Value("极耳焊接加工模式");
                if (form.PLC_ui.Read_Bool(FindIniPath("NG回流模式")[0]))
                {
                    //返修模式
                    processedMode = "1";
                    form.PLC_ui.write(FindIniPath("确认NG回流模式")[0], true);
                    form.P_Class.TxtLog("回流模式确认OK", 2);
                }
                string userOperator = getData2Value("极耳焊接操作人");
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (mesClass.inbound_MES(url, moduleCode1, deviceCode, processedMode, userOperator, timestamp, 2, "进站1OK", "进站1NG",ref hanJieProductCode))
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "进站1", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                    }));
                }
                else
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "进站1", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                    }));
                }
            }
            return ""; 
        }
        public override string 工位1进站复位(string Code = "")
        {
            form.PLC_ui.write(FindIniPath("进站1NG")[0], false);
            form.PLC_ui.write(FindIniPath("进站1OK")[0], false);
            form.PLC_ui.write(FindIniPath("确认NG回流模式")[0], false);
            return "";
        }

        public override string 工位1出站(string Code = "")
        {
            form.P_Class.TxtLog("接收极耳焊接出站信号，开始出站！", 2);
            moduleCode1 = form.PLC_ui.Read_String(FindIniPath("WIP码")[0]);
            form.P_Class.TxtLog("极耳焊接1出站WIP码：" + moduleCode1, 2);
            if (moduleCode1 == " ")
            {
                form.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = { moduleCode1, "出站1", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));

                form.PLC_ui.write(FindIniPath("出站1NG")[0], true);
                form.P_Class.TxtLog("出站1NG，模组码为空", 2);
                return "";
            }
            string[] tempStr = new string[form.dataGridView2.Columns.Count];
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "WIP码")] = moduleCode1;
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "托盘码")] = form.PLC_ui.Read_String(FindIniPath("托盘码")[0]);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "工位")] = "极耳焊接";
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "进站时间")] = StationInTime;
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "出站时间")] = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
            JArray paramsArrayList = new JArray();
            JArray materialsArrayList = new JArray();

            readPLC_CS(tempStr, "极耳焊接", "", "", "", "", 1,ref paramsArrayList, ref materialsArrayList);

            form.P_Class.TxtLog("本地数据获取完成", 2);
            if (!form.采集模式.Text.Contains("在线"))
            {
                form.P_Class.TxtLog("程序进入离线模式", 2);
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "出站1", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                    DataGridViewClass.AddRows(form.dataGridView2, tempStr, Color.White);
                    form.P_Class.Write_CSV(form.dataGridView2,tempStr, "极耳焊接");
                }));

                form.PLC_ui.write(FindIniPath("出站1OK")[0], true);
                form.P_Class.TxtLog("出站1OK，当前屏蔽MES", 2);
            }else {
                form.P_Class.TxtLog("程序进入在线模式", 2);
                string url = getData3Value("极耳焊接URL");
                string deviceCode = getData3Value("极耳焊接设备编码");
                string isOutBound = getData3Value("极耳焊接是否出站");
                string userOperator = getData3Value("极耳焊接操作人");
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                form.P_Class.TxtLog("MES配置完成", 2);
                if (mesClass.outbound_MES(url, moduleCode1, deviceCode, isOutBound, userOperator, timestamp, "极耳焊接", paramsArrayList, materialsArrayList, 2, "出站1OK", "出站1NG"))
                {
                    form.P_Class.TxtLog("出站完成", 2);
                    form.Invoke(new MethodInvoker(delegate {
                        string[] temp = { moduleCode1, "出站1", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                        DataGridViewClass.AddRows(form.dataGridView2, tempStr, Color.White);
                        form.P_Class.Write_CSV(form.dataGridView2,tempStr, "极耳焊接");
                    }));
                }else
                {
                    form.Invoke(new MethodInvoker(delegate {
                        string[] temp = { moduleCode1, "出站1", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.Red);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                        DataGridViewClass.AddRows(form.dataGridView2, tempStr, Color.Red);
                        form.P_Class.Write_CSV(form.dataGridView2,tempStr, "极耳焊接");
                    }));
                }
            }
            return "";
        }
        public override string 工位1出站复位(string Code = "")
        {
            form.PLC_ui.write(FindIniPath("出站1NG")[0], false);
            form.PLC_ui.write(FindIniPath("出站1OK")[0], false);
            return "";
        }

        public override string 工位2进站(string Code = "") { return ""; }
        public override string 工位2进站复位(string Code = "") { return ""; }

        public override string 工位2出站(string Code = "") { return ""; }
        public override string 工位2出站复位(string Code = "") { return ""; }

        public override string 工位3进站(string Code = "") { return ""; }
        public override string 工位3进站复位(string Code = "") { return ""; }
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
