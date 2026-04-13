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
    public class L100B
    {

        public float pressure = 0;
        public SerialPort LSerialPort;
        Form1 form;

        public L100B(Form1 form)
        {          
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
        static int buffersize = 100;   //十六进制数的大小（假设为9Byte,可调整数字大小）
      
        public async Task<bool> GetVoltageAndResAsync(string portName, int baudRate)
        {
            bool flag = true;
            try
            {
                LSerialPort = new SerialPort();
                LSerialPort.PortName = portName;
                LSerialPort.BaudRate = baudRate;
                LSerialPort.ReadTimeout = 2000;
                LSerialPort.Open();
            }
            catch (Exception)
            {
                form.P_Class.TxtLog("打开串口失败", 1);
                flag = false;
            }
            try
            {
                LSerialPort.DiscardInBuffer();
                //////发送指令"01 03 01 A4 00 02 84 14"
                Byte[] buffer = new Byte[8];
                buffer[0] = 0x01;
                buffer[1] = 0x03;
                buffer[2] = 0x01;
                buffer[3] = 0xA4;
                buffer[4] = 0x00;
                buffer[5] = 0x02;
                buffer[6] = 0x84;
                buffer[7] = 0x14;
                LSerialPort.Write(buffer,0,8);
                await Task.Run(() =>
                {
                    byte[] buffer2 = new Byte[buffersize];   //创建缓冲区
                    LSerialPort.Read(buffer2, 0, buffersize);
                    string ss;
                    ss = byteToHexStr(buffer); //用到函数byteToHexStr
                    string a2 = GetHexadecimalValue(ss).ToString();
                    pressure = ((float.Parse(a2)) / 1000);
                    //form.P_Class.TxtLog("压力计串口接收数据成功====:" + pressure, 2);

                });
            }
            catch (Exception ex)
            {
                form.P_Class.TxtLog("压力计串口接收数据失败====" , 1);
            }
            try
            {
                if ( LSerialPort.IsOpen)
                {
                    LSerialPort.Close();
                }
            }
            catch (Exception exception3)
            {
                form.P_Class.TxtLog("关闭串口异常:" + exception3.Message, 1);
            }
            return flag;
        }

        


        /// <summary>
        /// 十六进制换算为十进制
        /// </summary>
        /// <param name="strColorValue"></param>
        /// <returns></returns>
        public static int GetHexadecimalValue(String strColorValue)
        {
            char[] nums = strColorValue.ToCharArray();
            int total = 0;
            try
            {
                for (int i = 0; i < nums.Length; i++)
                {
                    String strNum = nums[i].ToString().ToUpper();
                    switch (strNum)
                    {
                        case "A":
                            strNum = "10";
                            break;
                        case "B":
                            strNum = "11";
                            break;
                        case "C":
                            strNum = "12";
                            break;
                        case "D":
                            strNum = "13";
                            break;
                        case "E":
                            strNum = "14";
                            break;
                        case "F":
                            strNum = "15";
                            break;
                        default:
                            break;
                    }
                    double power = Math.Pow(16, Convert.ToDouble(nums.Length - i - 1));
                    total += Convert.ToInt32(strNum) * Convert.ToInt32(power);
                }

            }
            catch (System.Exception ex)
            {
                String strErorr = ex.ToString();
                return 0;
            }
            return total;
        }


        //字节数组转16进制字符串
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
    }
}
