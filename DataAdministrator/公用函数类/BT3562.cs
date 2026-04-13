using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JP_DLL;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.IO.Ports;
using HslCommunication.Profinet.Siemens;
using System.Globalization;
using DataAdministrator;

namespace DataAdministraction.公用函数类
{
    public class BT3562
    {
        public decimal voltage;
        public decimal res;
        public SerialPort mSerialPort;
        Form1 form;

        public BT3562(Form1 form)
        {
            this.voltage = 0M;
            this.res = 0M;
            this.form = form;
        }

        private decimal ChangeDataToDec(string strData)
        {
            decimal num = new decimal(0, 0, 0, false, 1);
            if (strData.Contains("E"))
            {
                num = Convert.ToDecimal(decimal.Parse(strData.ToString(), NumberStyles.Float));
            }
            return num;
        }
        public async Task<bool> GetVoltageAndResAsync(string portName, int baudRate, int index)
        {
            bool flag = true;
            try
            {
                mSerialPort = new SerialPort();
                mSerialPort.PortName = portName;
                mSerialPort.BaudRate = baudRate;
                mSerialPort.ReadTimeout = 2000;
                mSerialPort.Open();
            }
            catch (Exception)
            {
                form.P_Class.TxtLog("打开串口失败", 1);
                flag = false;
            }
            try
            {
                mSerialPort.DiscardInBuffer();
                mSerialPort.Write(":FETch?\r\n");
                await Task.Run(() =>
                {
                    string Voltag = mSerialPort.ReadLine().Replace("\r", "").Replace("\n", "");
                    form.P_Class.TxtLog("获取ACR数据:" + Voltag, 1);
                    string[] ocv_data = Voltag.Split(new char[] { ',' });
                    if (index == 0)
                    {
                        voltage = ChangeDataToDec(ocv_data[1]);
                    }
                    else
                    {
                        voltage = 0;
                    }
                    res = ChangeDataToDec(ocv_data[0]);
                    form.P_Class.TxtLog("电压:" + this.voltage.ToString() + ",内阻:" + this.res.ToString(), 2);
                });
            }
            catch (Exception ex)
            {
                flag = false;
                voltage = 0;
                res = 0;
                form.P_Class.TxtLog("OCV数据转换异常:" + ex.Message, 1);
            }
            try
            {
                if (mSerialPort.IsOpen)
                {
                    mSerialPort.Close();
                }
            }
            catch (Exception exception3)
            {
                form.P_Class.TxtLog("关闭串口异常:" + exception3.Message, 1);
            }
            return flag;
        }

        public async Task<bool> SetModle(string portName, int baudRate)
        {
            bool flag = true;

            try
            {
                mSerialPort = new SerialPort();
                mSerialPort.PortName = portName;
                mSerialPort.BaudRate = baudRate;
                mSerialPort.ReadTimeout = 2000;
                mSerialPort.Open();
            }
            catch (Exception)
            {
                form.P_Class.TxtLog("打开串口失败", 1);
                flag = false;
            }
            try
            {
                mSerialPort.DiscardInBuffer();
                mSerialPort.Write(":INITiate:CONTinuous ON\r\n");
                Thread.Sleep(500);
                mSerialPort.Write(":TRIGger:SOURce IMM\r\n");
                form.P_Class.TxtLog("ACR设置初始化成功", 2);
                Thread.Sleep(500);
                //mSerialPort.DiscardInBuffer();
                //mSerialPort.Write(":FUNC? RV\r\n");
                //string Voltag = "";
                await Task.Run(() =>
                {
                    //    Voltag = mSerialPort.ReadLine().Replace("\r", "").Replace("\n", "");
                    //    form.P_Class.TxtLog("获取ACR数据:" + Voltag, 2);

                });
                //if (Voltag.Contains("RV"))
                //{
                //    form.P_Class.TxtLog("ACR设置RV模式成功", 2);
                //}
            }
            catch (Exception ex)
            {
                flag = false;

                form.P_Class.TxtLog("仪器初始化异常:" + ex.Message, 1);
            }
            try
            {
                if (mSerialPort.IsOpen)
                {
                    mSerialPort.Close();
                }
            }
            catch (Exception exception3)
            {
                form.P_Class.TxtLog("关闭串口异常:" + exception3.Message, 1);
            }
            return flag;
        }
    }
}
