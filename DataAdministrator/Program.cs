using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataAdministrator
{
    static class Program
    {
        private static System.Threading.Mutex mutex;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);// UnhandledException事件来处理非 UI 线程异常
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);//

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            HttpWebRequest.DefaultWebProxy = null;

            mutex = new System.Threading.Mutex(true, "Only");
            if (mutex.WaitOne(0, false))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false); 
                System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
                    Application.Run(new Form1());
            }
            else
            {
                MessageBox.Show("已经有一个MES程序在运行！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                Exception ex = (Exception)e.ExceptionObject;
                TxtWrite("Program," + MethodBase.GetCurrentMethod().Name + " 错误：" + ex.Message + "\n\nStack Trace:\n" + ex.StackTrace + "\r\n\r\n");
            }
            catch (Exception exc)
            {
                try
                {
                    MessageBox.Show(" Error",
                        " Could not write the error to the log. Reason: "
                        + exc.Message, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                }
                finally
                {
                    Application.Exit();
                }
            }
        }
        private static void TxtWrite(string value)
        {
            value = "[" + DateTime.Now.ToLongTimeString() + "]  " + value;
            string dataPath = System.Windows.Forms.Application.StartupPath + "/" + "Exception" + "/";  //创建当天时间文件夹
            string subPath = dataPath + "/" + DateTime.Now.ToLongDateString().ToString() + "/";
            //ImgName = DateTime.Now.ToFileTime().ToString(); //文件名称

            if (Directory.Exists(dataPath) == false)
            {
                Directory.CreateDirectory(dataPath);
            }
            if (System.IO.Directory.Exists(subPath) == false)//如果不存在就创建file文件夹 
            {
                Directory.CreateDirectory(subPath);
            }
            FileStream fs = new FileStream(subPath + "\\报错文件.txt", FileMode.Append);
            StreamWriter wr = new StreamWriter(fs);
            wr.WriteLine(value);
            wr.Close();

        }

    }
}
