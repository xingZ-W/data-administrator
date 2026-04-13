using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImSignal
{
    public class Signal
    {
        public string SignalValue;
        public BeginFunctionDelegate BeginFunction;
        public EndFunctionDelegate EndFunction;
        public bool delegateFlag;

        public delegate string BeginFunctionDelegate(string temp = "");
        public delegate string EndFunctionDelegate(string temp = "");

        /// <summary>用于记录哪一段信号执行哪些函数
        /// </summary>
        /// <param name="SignalValue">信号地址</param>
        /// <param name="BeginFunction">信号启动时候需要执行的函数</param>
        /// <param name="EndFunction">信号复位时候需要执行的函数</param>
        public Signal(string SignalValue, BeginFunctionDelegate BeginFunction, EndFunctionDelegate EndFunction = null)
        {
            this.SignalValue = SignalValue;
            this.BeginFunction = BeginFunction;
            this.EndFunction = EndFunction;
            this.delegateFlag = false;
        }
    }
}
