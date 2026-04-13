using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using DataAdministrator;

namespace DataAdministraction.公用函数类
{
    public class WT6582
    {
        Form1 form;
        public SerialPort sp = new SerialPort();
        Thread thea1;
        Thread thea2;
        Thread thea3;

        public DateTime t1;
        public DateTime t2;
        public TimeSpan t3;

        public TimeSpan t4;
        public TimeSpan t5;

        public string strRecieve3 = "";
        public string strRecieve_All = "";
        public bool insulationBool = false;
        public bool pressureResistanceBool = false;
        public bool serialPortBool = false;
        public bool insulationTimeBool = false;

        public double insulationValueTwo;
        public double insulationValue;
        public double insulationTestTime;
        public double pressureValue;

        public bool zhengji = false;
        public bool fuji = false;

        public WT6582(Form1 form)
        {
            this.form = form;
            thea1 = new Thread(LOOB);
            thea1.IsBackground = true;
            thea1.Start();
            thea3 = new Thread(LOOB1);
            thea3.IsBackground = true;
            thea3.Start();
            thea2 = new Thread(sp_DataReceived);
            thea2.IsBackground = true;
        }

        public void openSeir(string serialPort, string port)
        {
            try
            {
                sp.PortName = serialPort;// strPortName赋给真正的串口号
                sp.BaudRate = int.Parse(port);//设置波特率，要强制转换
                sp.DataBits = int.Parse("8");//设置数据位
                sp.Parity = Parity.None;
                sp.StopBits = (StopBits)int.Parse("1");//设置停止位
                sp.ReadTimeout = 1000;//打开延时
                sp.Open();
                thea2.Start();
                serialPortBool = true;
                form.P_Class.TxtLog("串口开启成功", 2);
            }
            catch (Exception me)
            {
                form.P_Class.TxtLog("串口开启失败" + me.ToString(), 1);
            }
        }

        void sp_DataReceived()//读取函数
        {
            while (true)
            {
                try
                {
                    int count = sp.BytesToRead;
                    byte[] RecieveBuf = new byte[count];//申请内存
                    sp.Read(RecieveBuf, 0, count);//读取接收到的数据
                    strRecieve3 = System.Text.Encoding.Default.GetString(RecieveBuf);//得到RecieveBuf的数据
                    strRecieve_All = strRecieve3;
                    Thread.Sleep(10);
                }
                catch (Exception)
                {
                }
            }
        }

        private void LOOB()
        {
            while (true)
            {
                if (serialPortBool)
                {
                    if (insulationBool)
                    {
                        if (strRecieve_All.Length > 10)
                        {
                            string IR = strRecieve_All;
                            strRecieve_All = "";
                            double ac_double = (Convert.ToDouble(ChangeDataToDec(IR)) / 1000000);
                            form.Invoke(new MethodInvoker(delegate {
                                form.P_Class.TxtLog("绝缘数据" + ac_double + "MΩ\r\n", 2);
                            }));

                            insulationTestTime = t3.Seconds;
                            if (ac_double == insulationValueTwo)
                            {
                                //insulationValueTwo = ac_double;
                                t2 = DateTime.Now;
                                insulationTestTime = 10;
                                form.Invoke(new MethodInvoker(delegate
                                {
                                    form.P_Class.TxtLog("绝缘测试数据不合格 " + insulationValueTwo + "MΩ\r\n", 2);
                                    if (zhengji)
                                    {
                                        t4 = t2 - t1;
                                        form.MySignl.positiveInsulationValue = insulationValueTwo;
                                        form.MySignl.positiveInsulationTestTime = t4.Seconds;
                                        form.PLC_ui.write(form.MySignl.FindIniPath("正极绝缘NG")[0], true);
                                    }
                                    else if (fuji)
                                    {
                                        t5 = t2 - t1;
                                        form.MySignl.negativeInsulationValue = insulationValueTwo;
                                        form.MySignl.negativeInsulationTestTime = t5.Seconds;
                                        form.PLC_ui.write(form.MySignl.FindIniPath("负极绝缘NG")[0], true);
                                    }
                                }));

                                insulationValue = ac_double;
                                insulationBool = false;
                                byte[] SendBuf = new byte[1024 * 1024];//申请内存
                                SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STOP \r\n");
                                sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
                                strRecieve_All = "";
                            }
                            else
                            {
                                insulationValueTwo = ac_double;
                            }

                            if (ac_double >= Convert.ToDouble(form.textBox15.Text))
                            {
                                insulationValue = ac_double;
                                insulationBool = false;
                                byte[] SendBuf = new byte[1024 * 1024];//申请内存
                                SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STOP \r\n");
                                sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
                                form.Invoke(new MethodInvoker(delegate {
                                    form.P_Class.TxtLog("绝缘数据合格,已停止测试 \r\n", 2);
                                }));
                                strRecieve_All = "";
                                insulationTimeBool = true;
                                SendBuf = System.Text.Encoding.Default.GetBytes(":SOURCE:SAFETY:RESULT:STEP<1/1>:MMETERAGE? \r\n");
                                sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
                            }
                        }
                    }

                    if (insulationTimeBool)
                    {
                        if (strRecieve_All.Length > 10)
                        {
                            string IR = strRecieve_All;
                            strRecieve_All = "";
                            double ac_double = (Convert.ToDouble(ChangeDataToDec(IR)) / 1000000);

                            insulationTimeBool = false;
                            insulationValue = ac_double;
                            t2 = DateTime.Now;
                            t3 = t2 - t1;
                            insulationTestTime = t3.Seconds;
                            form.Invoke(new MethodInvoker(delegate {
                                form.P_Class.TxtLog("绝缘数据最终数据：" + insulationValue + "MΩ     测试时间：" + insulationTestTime + "S \r\n", 2);
                            }));

                            if (zhengji)
                            {
                                form.MySignl.positiveInsulationValue = insulationValue;
                                form.MySignl.positiveInsulationTestTime = insulationTestTime;
                                form.PLC_ui.write(form.MySignl.FindIniPath("正极绝缘OK")[0], true);
                                form.Invoke(new MethodInvoker(delegate
                                {
                                    form.P_Class.TxtLog("正极绝缘测试完成\r\n", 2);
                                }));
                            }
                            else if (fuji)
                            {
                                form.MySignl.negativeInsulationValue = insulationValue;
                                form.MySignl.negativeInsulationTestTime = insulationTestTime;
                                form.PLC_ui.write(form.MySignl.FindIniPath("负极绝缘OK")[0], true);
                                form.Invoke(new MethodInvoker(delegate
                                {
                                    form.P_Class.TxtLog("负极绝缘测试完成\r\n", 2);
                                }));
                            }
                        }
                    }

                    if (pressureResistanceBool)
                    {
                        if (strRecieve_All.Equals("STOPPED\r\n"))
                        {
                            strRecieve_All = "";

                            byte[] SendBuf = new byte[1024 * 1024];//申请内存
                            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STOP \r\n");
                            sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
                            form.Invoke(new MethodInvoker(delegate {
                                form.P_Class.TxtLog("耐压数据已停止测试 \r\n", 2);
                            }));
                            Thread.Sleep(Convert.ToInt32(form.textBox19.Text));
                            SendBuf = new byte[1024 * 1024];//申请内存
                            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURCE:SAFETY:RESULT:STEP<1/1>:MMETERAGE? \r\n");
                            sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
                        }

                        if (strRecieve_All.Length > 10)
                        {
                            pressureResistanceBool = false;
                            string DC = strRecieve_All;
                            strRecieve_All = "";
                            double ir_double = Convert.ToDouble(ChangeDataToDec(DC)) * 1000;
                            pressureValue = ir_double;
                            form.Invoke(new MethodInvoker(delegate
                            {
                                form.P_Class.TxtLog("耐压数据" + pressureValue + "mA\r\n", 2);
                            }));

                            if (zhengji)
                            {
                                form.MySignl.positivePressureValue = pressureValue;
                                form.PLC_ui.write(form.MySignl.FindIniPath("正极耐压OK")[0], true);
                                form.Invoke(new MethodInvoker(delegate
                                {
                                    form.P_Class.TxtLog("正极耐压测试完成\r\n", 2);
                                }));
                            }

                            else if (fuji)
                            {
                                form.MySignl.negativePressureValue = pressureValue;
                                form.PLC_ui.write(form.MySignl.FindIniPath("负极耐压OK")[0], true);
                                form.Invoke(new MethodInvoker(delegate
                                {
                                    form.P_Class.TxtLog("负极耐压测试完成\r\n", 2);
                                }));
                            }

                        }
                    }
                }
                Thread.Sleep(10);
            }
        }

        private void LOOB1()
        {
            while (true)
            {
                if (insulationBool)
                {
                    Thread.Sleep(1000);
                    byte[] SendBuf = new byte[1024 * 1024];//申请内存
                    SendBuf = System.Text.Encoding.Default.GetBytes(":SOURCE:SAFETY:RESULT:STEP<1/1>:MMETERAGE? \r\n");
                    sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
                }

                if (pressureResistanceBool)
                {
                    Thread.Sleep(1000);
                    byte[] SendBuf = new byte[1024 * 1024];//申请内存
                    SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STATus? \r\n");
                    sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
                }
            }
        }
        decimal num = new decimal(0);
        private decimal ChangeDataToDec(string strData)
        {

            if (strData.Contains("E"))
            {
                try
                {
                    num = Convert.ToDecimal(decimal.Parse(strData.ToString(), NumberStyles.Float));
                }
                catch (Exception)
                {
                    num = -999;
                }

            }
            return num;
        }

    }
}
