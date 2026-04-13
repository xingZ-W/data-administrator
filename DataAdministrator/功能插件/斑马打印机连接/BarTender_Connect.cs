using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Seagull.BarTender.Print;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;

namespace DataAdministrator.功能插件.斑马打印机连接
{
    public class BarTender_Connect
    {
        Engine engine;// 打印机线程控制对象
        BackgroundWorker formatLoader;// 打印机线程控制对象
        LabelFormatDocument format = null; // The currently open Format
        DataGridView dataGridView; // 将指定的BTW标签文件的数据信息显示到DataGrid表上显示，并实时更改
        string BTW_Path;// btw斑马标签文件的路径
        Form1 form;// 主窗口对象
        public BarTender_Connect(Form1 form, string BTW_Path, DataGridView dataGridView) {

            this.form = form;
            this.BTW_Path = BTW_Path;
            this.dataGridView = dataGridView;
        }
        /// <summary> 连接斑马打印机
        /// </summary>
        /// <returns></returns>
        public string connect() {

            #region 加载打印机驱动代码
            try
            {
                engine = new Engine(true);
                formatLoader = new BackgroundWorker();
                formatLoader.DoWork += new DoWorkEventHandler(formatLoader_DoWork);
                formatLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(formatLoader_RunWorkerCompleted);
                formatLoader.RunWorkerAsync(0);
                return "";
            }
            catch (Exception ex)
            {
                MessageBox.Show("程序没有检查到斑马打印机的工具程序，请确认好电脑是否安装了斑马工具软件。\r\n\r\n" + ex.ToString(), "打印机连接异常", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                #region 关闭程序
                Close();
                return ex.Message;
                #endregion
            }
            #endregion
        }

        void formatLoader_DoWork(object sender, DoWorkEventArgs e)
        {
            int index = (int)e.Argument;

            lock (engine)
            {
                try
                {
                    if (format != null)
                        format.Close(SaveOptions.DoNotSaveChanges);
                    format = engine.Documents.Open(BTW_Path);
                }
                catch
                {
                    format = null;
                }
            }

        }
        void formatLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            lock (engine)
            {
                if (format != null)
                {
                    if (format.SubStrings.Count > 0)
                    {
                        BindingSource bindingSource = new BindingSource();
                        bindingSource.DataSource = format.SubStrings;
                        dataGridView.DataSource = bindingSource;
                        dataGridView.AutoResizeColumns();
                    }

                    dataGridView.Invalidate();
                }

            }
        }

        /// <summary> 打印标签
        /// </summary>
        public void BarTender_Print() {
            lock (engine)
            {
                Cursor.Current = Cursors.WaitCursor;

                // 这个Messages是斑马dll库里面的一个函数，或是变量
                Messages messages;
                int waitForCompletionTimeout = 10000; // 10 seconds（秒）
                // 规定好chen是传输时随便定义的文件名，打印超时时间和加载的库对象
                Result result = format.Print("Tang", waitForCompletionTimeout, out messages);

                if (result == Result.Failure)
                {
                    //TxtWrite2("Label was successfully sent to printer.");
                    //TxtWrite2("指定条码已打印: " + substringGrid.Rows[4].Cells[1].Value);
                    //dataGridView2.Rows.Add(序号自增, null, null, null, null, null, null);
                    //dataGridView2.FirstDisplayedScrollingRowIndex = dataGridView2.Rows.Count - 1;
                    //MessageBox.Show("生成数据成功");
                }
                else
                {
                    //TxtWrite2("打印异常，请检查！");
                    //TxtWrite2("Print Failed ");
                    //MessageBox.Show("生成数据失败");
                }
            }
        }

        /// <summary> 清理掉这个对象的所有资源
        /// </summary>
        public void Close() {
            if (engine != null) {
                engine.Stop(SaveOptions.DoNotSaveChanges);
                engine.Dispose();
            }
            if (formatLoader != null) {
                formatLoader.DoWork -= new DoWorkEventHandler(formatLoader_DoWork); 
                formatLoader.RunWorkerCompleted += new RunWorkerCompletedEventHandler(formatLoader_RunWorkerCompleted);
                formatLoader.Dispose();
            }
            engine = null;
            format = null;
            formatLoader = null; 
        }
    }
}
