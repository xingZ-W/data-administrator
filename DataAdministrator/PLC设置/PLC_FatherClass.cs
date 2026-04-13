using HslCommunication;
using HslCommunication.Profinet.Omron;
using HslCommunication.Profinet.Siemens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// PLC信号的父类
namespace DataAdministraction.功能插件.PLC设置界面
{
    public abstract class PLC_FatherClass
    {
        /// <summary> plc连接结果
        /// </summary>
        public OperateResult connect;

        public Omron_dataVale omron;// 欧姆龙机构体对象
        public Siemens_dataVale siemens;// 西门子机构体对象

        /// <summary> 欧姆龙连接使用的参数
        /// </summary>
        public struct Omron_dataVale{
            /// <summary>欧姆龙对象
            /// </summary>
            public OmronFinsNet omronFinsNet;
            /// <summary>IP地址
            /// </summary>
            public string ip;
            /// <summary> PLC版本
            /// </summary>
            public int versions;
            /// <summary>PC号
            /// </summary>
            public byte PC_number;
            /// <summary>PLC号
            /// </summary>
            public byte PLC_number;
            /// <summary>PLC单元号
            /// </summary>
            public byte PLC_unit;
        }

        /// <summary> 西门子连接使用的参数
        /// </summary>
        public struct Siemens_dataVale
        {
            /// <summary>西门子对象
            /// </summary>
            public SiemensS7Net SiemensS7_Net;
            /// <summary>IP地址
            /// </summary>
            public string ip;
            /// <summary> PLC版本
            /// </summary>
            public SiemensPLCS versions;
            /// <summary>超时
            /// </summary>
            public int overTime;
        }

        /// <summary> 发送string类型的数据进字符串
        /// IP地址 - 数据内容（string）
        /// </summary>
        public abstract void write(string ip, string content);
        /// <summary> 发送bool类型的数据进字符串
        /// IP地址 - 数据内容（bool）
        /// </summary>
        public abstract void write(string ip, bool content);
        /// <summary> 发送float类型的数据进字符串
        /// IP地址 - 数据内容（float）
        /// </summary>
        public abstract void write(string ip, float content);
        /// <summary> 发送int类型的数据进字符串
        /// IP地址 - 数据内容（int）
        /// </summary>
        public abstract void write(string ip, int content);
        /// <summary> 发送ushort类型的数据进字符串
        /// IP地址 - 数据内容（ushort）
        /// </summary>
        public abstract void write(string ip, ushort content);


        /// <summary> 读取bool类型的数据
        /// IP地址
        /// </summary>
        public abstract bool Read_Bool(string ip);

        /// <summary> 读取string类型的数据
        /// IP地址
        /// </summary>
        public abstract string Read_String(string ip, ushort length = 0);

        /// <summary> 读取byte类型的数据
        /// IP地址
        /// </summary>
        public abstract byte Read_byte(string ip);

        /// <summary> 读取float类型的数据
        /// IP地址
        /// </summary>
        public abstract float Read_float(string ip);

        /// <summary> 读取int类型的数据
        /// IP地址
        /// </summary>
        public abstract int Read_int(string ip);

        /// <summary> 读取short类型的数据
        /// IP地址
        /// </summary>
        public abstract short Read_short(string ip);
    }
}
