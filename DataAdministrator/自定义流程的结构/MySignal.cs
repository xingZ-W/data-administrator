using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HslCommunication;
using HslCommunication.Instrument;
using HslCommunication.Profinet.Omron;
using DataAdministrator;
using System.Diagnostics;

namespace ImSignal
{
    class MySignal
    {
        List<Signal> listSignal;// 储存流程
        Form1 form;
        public MySignal(Form1 form)
        {
            this.form = form;
            listSignal = new List<Signal>();
            //Thread temp = new Thread(new ThreadStart(RunSignal));
            Thread temp = new Thread(new ThreadStart(RunSignal));
            temp.IsBackground = true;
            temp.Start();
        }
        // 执行的流程
        private void RunSignal() {
            while(true){
                if (form.PLC_ui.connect == null || !form.PLC_ui.connect.IsSuccess || form.运行状态.Text == "未运行")
                {
                    Thread.Sleep(500);
                    continue;
                }
                
                for (int i = 0; i < listSignal.Count; i++)
                {
                    // 判断条件是否被触发
                    if (form.PLC_ui.Read_Bool(listSignal[i].SignalValue))
                    {// 启动条件
                        if (!listSignal[i].delegateFlag)
                        {
                            // 对启动的委托进行开启标记
                            listSignal[i].delegateFlag = true;
                            listSignal[i].BeginFunction(listSignal[i].SignalValue);
                        }

                    }
                    else if (!form.PLC_ui.Read_Bool(listSignal[i].SignalValue))
                    {// 复位条件
                        try
                        {
                            // 复位委托标记
                            listSignal[i].delegateFlag = false;
                            listSignal[i].EndFunction(listSignal[i].SignalValue);// 这里需要套用异常辅助一下，因为有可能是执行为null的函数，所以可能会报错                            
                        }
                        catch { }
                    }
                    Thread.Sleep(100);// 缓解死线程带来的压力，给程序留点运行空间
                }

                form.LogFileDelete();// 清理日志文件
            }
        }

        /// <summary>添加新的流程进入list里面
        /// </summary>
        public void AddListSignal(Signal newSignal) {
            listSignal.Add(newSignal);
        }

    }
}
