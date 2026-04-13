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
using DataAdministrator.功能插件.停机弹窗;

namespace DataAdministrator.自动流程
{
    class 外壳焊后 : Father
    {
        public 外壳焊后(Form1 form, string FilePath2)
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

            MySignal signal2 = new MySignal(this.form);// 流程结构对象
            signal2.AddListSignal(new Signal(FindIniPath("外壳焊后设备报警触发")[0], 外壳焊后设备报警, 外壳焊后设备报警复位));

            new Thread(new ThreadStart(() =>
            {
                while (true)
                {

                    #region 发送工序设备状态(只有PLC里面的值发送改变后才会上传)
                    if (form.采集模式.Text != "离线")
                    {
                        外壳焊后设备实时状态();

                    }
                    #endregion

                    Thread.Sleep(1000);
                }
            })).Start();


            new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    #region 发送MES设备心跳 1000秒上传一次
                    string 外壳焊后设备Code = getData2Value("外壳焊后设备编码");
                    string 外壳焊后设备user = getData2Value("外壳焊后操作人");

                    if (form.采集模式.Text != "离线")
                    {
                        mesClass.设备心跳检测(外壳焊后设备Code, 外壳焊后设备user, "1");// 1在
                    }
                    else if (form.采集模式.Text == "离线")
                    {
                        mesClass.设备心跳检测(外壳焊后设备Code, 外壳焊后设备user, "0");// 0离线
                    }
                    #endregion
                    Thread.Sleep(10000);
                }
            })).Start();

            new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    int str = Convert.ToInt32(form.PLC_ui.Read_float(FindIniPath("外壳焊后设备状态")[0]));
                    if (str == 6)//6是停机状态
                    {
                        外壳焊后设备停机();
                        Thread.Sleep(2000);
                    }
                    else
                    {
                        外壳焊后设备停机复位();
                        Thread.Sleep(2000);
                    }
                }
            })).Start();
        }
        // 实时状态
        int 外壳焊后设备 = -1;
        public string 外壳焊后设备实时状态(string Code = "")
        {
            if (form.采集模式.Text != "离线")
            {
                int str = Convert.ToInt32(form.PLC_ui.Read_float(FindIniPath("外壳焊后设备状态")[0]));
                if (str == 外壳焊后设备)
                    return "";
                外壳焊后设备 = str;
                string retJson = mesClass.设备实时状态(str.ToString(), getData2Value("外壳焊后操作人"), getData2Value("外壳焊后设备编码"));
                JObject temp = new JObject();
                temp = JsonHelper.GetJObject(retJson);
                if (temp == null || temp["status"].ToString() != "200")
                {
                    form.P_Class.TxtLog("外壳焊后工序设备状态上传失败！", 2);
                    return "";
                }
                else
                {
                    form.P_Class.TxtLog("外壳焊后工序设备状态上传成功！", 2);
                }
            }
            else
            {
                //MessageBox.Show("外壳焊后设备状态上传失败！\r\n\r\n原因：\r\n1.请确认账户是否已登录！\r\n2.请确认采集模式是否是在线状态！", "MES上传失败", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            return "";
        }

        //设备报警
        bool ding1 = false;
        public string 外壳焊后设备报警(string Code = "")
        {
            if (form.采集模式.Text != "离线")
            {
                int str = Convert.ToInt32(form.PLC_ui.Read_float(FindIniPath("外壳焊后设备报警编码")[0]));
                PublicClass.Set_ini_data("外壳焊后设备报警编码", "当前报警编码", str.ToString(), System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/PLC设备报警表.ini");
                string message = PublicClass.Get_ini_data("外壳焊后设备报警编码", str.ToString(), System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/PLC设备报警表.ini");
                string retJson = mesClass.设备报警采集("1", str.ToString(), message, getData2Value("外壳焊后设备编码"), getData2Value("外壳焊后胶操作人"));
                JObject temp = new JObject();
                temp = JsonHelper.GetJObject(retJson);
                if (temp == null || temp["status"].ToString() != "200")
                {
                    form.P_Class.TxtLog("外壳焊后设备报警状态上传失败！", 2);
                    return "";
                }
                else
                {
                    form.P_Class.TxtLog("外壳焊后设备报警状态上传成功！", 2);
                }
                ding1 = true;
            }
            else
            {
                //MessageBox.Show("外壳焊后设备报警上传失败！\r\n\r\n原因：\r\n1.请确认账户是否已登录！\r\n2.请确认采集模式是否是在线状态！", "MES上传失败", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            return "";
        }

        public string 外壳焊后设备报警复位(string Code = "")
        {
            if (form.采集模式.Text != "离线")
            {
                if (ding1)
                {
                    string str = PublicClass.Get_ini_data("外壳焊后设备报警编码", "当前报警编码", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/PLC设备报警表.ini");
                    string message = PublicClass.Get_ini_data("外壳焊后设备报警编码", str.ToString(), System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/PLC设备报警表.ini");
                    string retJson = mesClass.设备报警采集("0", str.ToString(), message, getData2Value("外壳焊后设备编码"), getData2Value("外壳焊后操作人"));
                    JObject temp = new JObject();
                    temp = JsonHelper.GetJObject(retJson);
                    if (temp == null || temp["status"].ToString() != "200")
                    {
                        form.P_Class.TxtLog("外壳焊后设备解除报警状态上传失败！", 2);
                        return "";
                    }
                    else
                    {
                        form.P_Class.TxtLog("外壳焊后设备解除报警状态上传成功！", 2);
                    }
                    ding1 = false;
                }
            }
            else
            {
                //MessageBox.Show("外壳焊后设备解除报警上传失败！\r\n\r\n原因：\r\n1.请确认账户是否已登录！\r\n2.请确认采集模式是否是在线状态！", "MES上传失败", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
            return "";
        }

        // 停机
        bool ding3 = false;
        public string 外壳焊后设备停机(string Code = "")
        {
            if (form.采集模式.Text != "离线" && !ding3)
            {
                PublicClass.Set_ini_data("MES停机时间", "外壳焊后设备", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");
                form.P_Class.TxtLog("已保存当前外壳焊后设备停机时间！", 2);
                ding3 = true;
            }
            return "";
        }
        public string 外壳焊后设备停机复位(string Code = "")
        {
            if (form.采集模式.Text != "离线")
            {
                if (ding3)
                {
                    MESClass temp = new MESClass(form);

                    DateTime k1 = Convert.ToDateTime(PublicClass.Get_ini_data("MES停机时间", "外壳焊后设备", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini"));
                    DateTime k2 = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    TimeSpan ts3 = k1.Subtract(k2).Duration();
                    if (ts3.TotalMinutes >= 3)//大于等于3分钟
                    {
                        string 外壳焊后设备Code = getData2Value("外壳焊后设备编码");
                        new StopWindows(form, "外壳焊后", 外壳焊后设备Code, k1.ToString("yyyy-MM-dd HH:mm:ss"), k2.ToString("yyyy-MM-dd HH:mm:ss")).ShowDialog();
                    }
                    ding3 = false;
                }
            }
            else
            {
                //MessageBox.Show("外壳焊后设备停止上传失败！\r\n\r\n原因：\r\n1.请确认账户是否已登录！\r\n2.请确认采集模式是否是在线状态！", "MES上传失败【解除停止】", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }

            return "";
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
            form.P_Class.TxtLog("接收外壳焊后进站1信号，开始进站！", 2);
            moduleCode1 = form.PLC_ui.Read_String(FindIniPath("WIP码")[0]);
            form.P_Class.TxtLog("外壳焊后1进站WIP码：" + moduleCode1, 2);

            if (moduleCode1 == " ")
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "进站1", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));
                form.PLC_ui.write(FindIniPath("进站1NG")[0], true);
                form.P_Class.TxtLog("进站1NG，模组码为空", 2);
                return "";
            }

            

            if (!form.采集模式.Text.Contains("在线"))
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "进站1", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));

                form.PLC_ui.write(FindIniPath("进站1OK")[0], true);
                form.P_Class.TxtLog("进站1OK，当前屏蔽MES", 2);
            }else {
                string url = getData2Value("外壳焊后URL");
                string deviceCode = getData2Value("外壳焊后设备编码");
                string processedMode = getData2Value("外壳焊后加工模式");
                if (form.PLC_ui.Read_Bool(FindIniPath("NG回流模式")[0]))
                {
                    ////返修 模式  
                    processedMode = "1";
                    form.PLC_ui.write(FindIniPath("确认NG回流模式")[0], true);
                    form.P_Class.TxtLog("回流模式确认OK", 2);

                }

                string userOperator = getData2Value("外壳焊后操作人");
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (mesClass.inbound_MES(url, moduleCode1, deviceCode, processedMode, userOperator, timestamp, 2, "进站1OK", "进站1NG",ref waiKeHanHoProductCode))
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "进站1", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                    }));
                }
                else {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "进站1", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
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
            form.P_Class.TxtLog("接收外壳焊后出站信号，开始出站！", 2);
            moduleCode1 = form.PLC_ui.Read_String(FindIniPath("WIP码")[0]);
            form.P_Class.TxtLog("外壳焊后1出站WIP码：" + moduleCode1, 2);

            if (moduleCode1 == " ")
            {
                form.Invoke(new MethodInvoker(delegate {
                    string[] temp = { moduleCode1, "出站1", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                }));

                form.PLC_ui.write(FindIniPath("出站1NG")[0], true);
                form.P_Class.TxtLog("出站1NG，模组码为空", 2);
                return "";
            }

            string[] tempStr = new string[form.dataGridView2.Columns.Count];
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "托盘码")] = form.PLC_ui.Read_String(FindIniPath("托盘码")[0]);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "工位")] = "外壳焊后";
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "进站时间")] = StationInTime;
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "出站时间")] = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
            string 模式 = string.Empty;
            if (form.Set_Ui.textBox12.Text.Equals(moduleCode1))
            {
                模式 = "防错点检";
            }
            else
            {
                模式 = "正常";
            }
            if (!form.采集模式.Text.Contains("在线")) { 模式 = "离线"; }
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "模式")] = 模式;
            JArray paramsArrayList = new JArray();
            JArray materialsArrayList = new JArray();

            readPLC_CS(tempStr, "外壳焊后", "", "", "", "",1, ref paramsArrayList, ref materialsArrayList);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "WIP码")] = moduleCode1;
            if (!form.采集模式.Text.Contains("在线"))
            {
                form.Invoke(new MethodInvoker(delegate
                {
                    string[] temp = { moduleCode1, "出站1", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                    DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                    form.P_Class.Write_CSV1(temp, form.工站名);
                    DataGridViewClass.AddRows(form.dataGridView2, tempStr, Color.White);
                    form.P_Class.Write_CSV(form.dataGridView2,tempStr, "外壳焊后");
                }));

                form.PLC_ui.write(FindIniPath("出站1OK")[0], true);
                form.P_Class.TxtLog("出站1OK，当前屏蔽MES", 2);
            }
            else {
                string url = getData3Value("外壳焊后URL");
                string deviceCode = getData3Value("外壳焊后设备编码");
                string isOutBound = getData3Value("外壳焊后是否出站");
                string userOperator = getData3Value("外壳焊后操作人");
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string forceOk = form.PLC_ui.Read_Bool(FindIniPath("强制OK触发")[0]) == true ? "1" : "0";
                if(forceOk == "1")
                {
                    form.P_Class.TxtLog("开始执行强制出站OK，开始出站！", 2);
                }
                if (mesClass.outbound_MES(url, moduleCode1, deviceCode, isOutBound, userOperator, timestamp, "外壳焊后", paramsArrayList, materialsArrayList, 2, "出站1OK", "出站1NG", forceOk))
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "出站1", "OK", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                        DataGridViewClass.AddRows(form.dataGridView2, tempStr, Color.White);
                        form.P_Class.Write_CSV(form.dataGridView2,tempStr, "外壳焊后");
                    }));
                }else
                {
                    form.Invoke(new MethodInvoker(delegate
                    {
                        string[] temp = { moduleCode1, "出站1", "NG", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") };
                        DataGridViewClass.AddRows(form.dataGridView1, temp, Color.White);
                        form.P_Class.Write_CSV1(temp, form.工站名);
                        DataGridViewClass.AddRows(form.dataGridView2, tempStr, Color.White);
                        form.P_Class.Write_CSV(form.dataGridView2,tempStr, "外壳焊后");
                    }));
                }
                if (forceOk == "1")
                {
                    form.P_Class.TxtLog("执行强制出站OK完成，结束出站！", 2);
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
