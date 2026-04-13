using HslCommunication;
using HslCommunication.Profinet.Siemens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataAdministraction.功能插件.PLC设置界面
{
    class PLC_Siemens : PLC_FatherClass
    {
        /// <summary> 进行连接西门子PLC
        /// ip地址 - 版本 - 超时
        /// </summary>
        public PLC_Siemens(string ip, string versions, int PC_number)
        {
            this.siemens.ip = ip;// ip地址
            this.siemens.overTime = PC_number;// 超时

            // 判断是哪个版本的西门子
            switch(versions){
                case "S1500":
                    this.siemens.versions = SiemensPLCS.S1500;// 版本
                    break;
                case "S1200":
                    this.siemens.versions = SiemensPLCS.S1200;// 版本
                    break;
                case "S300":
                    this.siemens.versions = SiemensPLCS.S300;// 版本
                    break;
                case "S400":
                    this.siemens.versions = SiemensPLCS.S400;// 版本
                    break;
                default:
                    MessageBox.Show("程序内部并无当前输入的西门子版本！");
                    return;
            }
            
            // 进行连接PLC
            this.siemens.SiemensS7_Net = PLC_Connect();
        }

        /// <summary> 连接PLC
        /// </summary>
        private SiemensS7Net PLC_Connect() 
        {
            SiemensS7Net SiemensS7_Net = new SiemensS7Net(this.siemens.versions, this.siemens.ip);// 输入PLC的IP地址，然后将欧姆龙协议的版本写上，就是那个9600，欧姆龙有很多版本，不知道版本号的可以问PLC的人，他们肯定知道
            SiemensS7_Net.ConnectTimeOut = this.siemens.overTime;// 设置时间，再规定的多久时间内连接到PLC
            this.connect = SiemensS7_Net.ConnectServer();

            return SiemensS7_Net;
        }

        /// <summary> 发送string类型的数据进字符串
        /// IP地址 - 数据内容（string）
        /// </summary>
        public override void write(string ip, string content)
        {
            byte[] head = new byte[2];
            head[0] = (byte)97;
            head[1] = (byte)content.Length;
            byte[] data = System.Text.ASCIIEncoding.ASCII.GetBytes(content);
            byte[] sendData = new byte[content.Length + 2];
            head.CopyTo(sendData, 0);
            data.CopyTo(sendData, 2);
            this.siemens.SiemensS7_Net.Write(ip, sendData);
        }

        /// <summary> 发送bool类型的数据进字符串
        /// IP地址 - 数据内容（bool）
        /// </summary>
        public override void write(string ip, bool content)
        {
            this.siemens.SiemensS7_Net.Write(ip, content);
        }

        /// <summary> 发送float类型的数据进字符串
        /// IP地址 - 数据内容（float）
        /// </summary>
        public override void write(string ip, float content) 
        {
            this.siemens.SiemensS7_Net.Write(ip, content);
        }

        /// <summary> 发送int类型的数据进字符串
        /// IP地址 - 数据内容（int）
        /// </summary>
        public override void write(string ip, int content)
        {
            this.siemens.SiemensS7_Net.Write(ip, content);
        }
        /// <summary> 发送ushort类型的数据进字符串
        /// IP地址 - 数据内容（ushort）
        /// </summary>
        public override void write(string ip, ushort content)
        {
            this.siemens.SiemensS7_Net.Write(ip, content);
        }

        /// <summary> 读取bool类型的数据进字符串
        /// IP地址
        /// </summary>
        public override bool Read_Bool(string ip) 
        {
            return this.siemens.SiemensS7_Net.ReadBool(ip).Content;
        }

        /// <summary> 读取string类型的数据进字符串
        /// IP地址
        /// </summary>
        public override string Read_String(string ip, ushort length = 0)
        {
            var head = siemens.SiemensS7_Net.ReadUInt16(ip).Content;
            byte recID = (byte)(head >> 8 & 0xff);
            byte len = (byte)(head & 0xff);

            try {
                string value = siemens.SiemensS7_Net.ReadString(ip, (ushort)(len + 2)).Content.Substring(2, len);
                value = value == null ? "" : value;
                return value == "" ? " " : value;
            }
            catch { return "PLC异常，无法查询到地址里面的数据！"; }
            //return this.siemens.SiemensS7_Net.ReadString(ip, length).Content;
        }

        /// <summary> 读取byte类型的数据
        /// IP地址
        /// </summary>
        public override byte Read_byte(string ip) {
            return siemens.SiemensS7_Net.ReadByte(ip).Content;
        }

        /// <summary> 读取float类型的数据
        /// IP地址
        /// </summary>
        public override float Read_float(string ip) {
            return siemens.SiemensS7_Net.ReadFloat(ip).Content;
        }

        /// <summary> 读取int类型的数据
        /// IP地址
        /// </summary>
        public override int Read_int(string ip) {
            return siemens.SiemensS7_Net.ReadInt32(ip).Content;
        }

        /// <summary> 读取short类型的数据
        /// IP地址
        /// </summary>
        public override short Read_short(string ip) {
            return siemens.SiemensS7_Net.ReadInt16(ip).Content;
        }
    }
}
