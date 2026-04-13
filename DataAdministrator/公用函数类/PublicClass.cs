using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using DataAdministrator;
using System.Windows.Forms;
using System.Threading;

//这里存放的是所有界面公用的功能

namespace DataAdministraction.公用函数类
{

    /// <summary> 这里存放的是所有界面公用的功能
    /// </summary>
    public class PublicClass
    {
        #region 全局变量
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        /// <summary>选择的工站的路径
        /// </summary>
        public string WorkestationPath = "";

        #endregion

        Form1 form;

        public PublicClass() { }
        public PublicClass(Form1 form) {
            this.form = form;
        }

        #region ini文件操作
        public static string Get_ini_data(string title, string name, string ini_path)
        {
            StringBuilder sb = new StringBuilder(255);
            GetPrivateProfileString(title, name, "", sb, 255, ini_path);
            return sb.ToString();
        }
        public static void Set_ini_data(string title, string name, string value, string ini_path)
        {
            WritePrivateProfileString(title, name, value, ini_path);
        }
        #endregion

        public void TxtLog(string value,int index, string Style = "程序日志") // 日志
        {
            // 选择在哪个日志里面显示
            switch(index){
                case 1:
                    this.form.textBox4.AppendText(DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") + ":" + DateTime.Now.Millisecond + " " + value + "\r\n"); // 主窗口日志控件                  
                    this.form.textBox4.SelectionStart = this.form.textBox4.Text.Length - 1;
                    this.form.textBox4.ScrollToCaret();                  
                    break;
                case 2:
                    this.form.textBox2.AppendText(DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") + ":" + DateTime.Now.Millisecond + " " + value + "\r\n"); // 主窗口日志控件
                    this.form.textBox2.SelectionStart = this.form.textBox2.Text.Length - 1;
                    this.form.textBox2.ScrollToCaret();
                    break;
                case 3:
                    this.form.textBox3.AppendText(DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss") + ":" + DateTime.Now.Millisecond + " " + value + "\r\n"); // 主窗口日志控件
                    this.form.textBox3.SelectionStart = this.form.textBox3.Text.Length - 1;
                    this.form.textBox3.ScrollToCaret();
                    break;
            }

            string currPath = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
            string dataPath = currPath + "/" + DateTime.Now.ToString("yyyy年MM月dd日");  //创建当天时间文件夹
            string subPath = dataPath + "/";
            if (Directory.Exists(dataPath) == false)
            {
                Directory.CreateDirectory(dataPath);
            }
            if (System.IO.Directory.Exists(subPath) == false)//如果不存在就创建file文件夹 
            {
                Directory.CreateDirectory(subPath);
            }
            try
            {
                FileStream fs = new FileStream(subPath + "\\" + DateTime.Now.ToString("yyyy年MM月dd日") + "日志文件.txt", FileMode.Append);
                StreamWriter wr = new StreamWriter(fs);
                wr.WriteLine(DateTime.Now.ToString() + ":" + DateTime.Now.Millisecond + " " + value);
                wr.Close();
            }
            catch (Exception) { }
        }

        /// <summary> 向指定的CSV文件路径写入数据
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <param name="content">要写入的内容，数组</param>
        /// <param name="dataGrid">要写入的表格，</param>
        public void Write_CSV(DataGridView dataGridView,object[] content, string fileName = "")// CSV表写入操作
        {
            StreamWriter writer = null;
            try
            {
                string currPath = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
                string dataPath = currPath + "/" + DateTime.Now.ToString("yyyy年MM月dd日");  //创建当天时间文件夹
                string subPath = dataPath + "/" + fileName;
                string str2 = "";
                string databag = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
                int timea = int.Parse(databag.Substring(12, 2));
                if (timea < 20 && timea >= 8)        // 8-20点白班(8点是白班，20点是夜班)
                {
                    subPath = dataPath + "/" + "白班" + "/" + form.工站名;
                    str2 = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + "/" + form.工站名 + "/窗口配置参数.ini") + "/" + DateTime.Now.ToString("yyyy年MM月dd日") + "/" + "白班" + "/" + fileName + "/" + (fileName == "" ? DateTime.Now.ToString("yyyy年MM月dd日") : fileName) + ".CSV";
                }
                else
                {
                    subPath = dataPath + "/" + "夜班" + "/" + form.工站名;
                    str2 = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/"+"/"  + form.工站名 + "/窗口配置参数.ini") + "/" + DateTime.Now.ToString("yyyy年MM月dd日") + "/" + "夜班"  + "/"+ fileName + "/" + (fileName == "" ? DateTime.Now.ToString("yyyy年MM月dd日") : fileName) + ".CSV";
                }
                if (System.IO.Directory.Exists(subPath) == false)//如果不存在就创建file文件夹 
                {
                    Directory.CreateDirectory(subPath);
                }               
                if (!File.Exists(str2))// 如果CSV文件不存在了，就需要重新创建一个CSV文件，并生成标题
                {
                    Write_CSV_Title(str2, DataGridViewClass.GetTitle(dataGridView));
                }
                writer = new StreamWriter(new FileStream(str2, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.Default);

                // 向文件里面写入数据
                string str3 = "";
                int index = 0;
                try
                {
                    while (true)
                    {
                        if (index >= content.Length)
                        {
                            writer.WriteLine(str3);
                            writer.Close();
                            break;
                        }
                        if (index > 0)
                        {
                            str3 = str3 + ",";
                        }
                        str3 = str3 + content[index];
                        index++;
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception("导出错误" + exception.Message);
                }
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                MessageBox.Show("CSV路径下的文件夹不存在，程序无法搜索到指定的路径！");
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show("模组数据录入失败！\r\n\r\n原因：当天的CSV数据文件在外部被打开，请关闭！\r\n注意：Except文档不能被两名用户同时打开！", "危险错误提示",MessageBoxButtons.OK,MessageBoxIcon.Exclamation,MessageBoxDefaultButton.Button1,MessageBoxOptions.DefaultDesktopOnly);
            }
        }
        /// <summary> 向指定的CSV文件路径写入数据
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <param name="content">要写入的内容，数组</param>
        public void Write_CSVKIN(object[] content, string fileName = "")// CSV表写入操作
        {
            StreamWriter writer = null;
            try
            {
                string currPath = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
                string dataPath = currPath + "/" + DateTime.Now.ToString("yyyy年MM月dd日");  //创建当天时间文件夹
                string subPath = dataPath + "/电芯入壳";         
                if (System.IO.Directory.Exists(subPath) == false)//如果不存在就创建file文件夹 
                {
                    Directory.CreateDirectory(subPath);
                }
                string str2 = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini") + "/" + DateTime.Now.ToString("yyyy年MM月dd日") + "/电芯入壳" + "/"+ (fileName == "" ? DateTime.Now.ToString("yyyy年MM月dd日") : fileName) + ".CSV";
                if (!File.Exists(str2))// 如果CSV文件不存在了，就需要重新创建一个CSV文件，并生成标题
                {
                    Write_CSV_Title(str2, DataGridViewClass.GetTitle(form.dataGridViewKIN));
                }
                writer = new StreamWriter(new FileStream(str2, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.Default);

                // 向文件里面写入数据
                string str3 = "";
                int index = 0;
                try
                {
                    while (true)
                    {
                        if (index >= content.Length)
                        {
                            writer.WriteLine(str3);
                            writer.Close();
                            break;
                        }
                        if (index > 0)
                        {
                            str3 = str3 + ",";
                        }
                        str3 = str3 + content[index];
                        index++;
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception("导出错误" + exception.Message);
                }
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                MessageBox.Show("CSV路径下的文件夹不存在，程序无法搜索到指定的路径！");
            }
            catch (System.IO.IOException)
            {
                MessageBox.Show("模组数据录入失败！\r\n\r\n原因：当天的CSV数据文件在外部被打开，请关闭！\r\n注意：Except文档不能被两名用户同时打开！", "危险错误提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        public static List<string> Read_CSV1(string FliePath)
        {
            try
            {
                string str2 = FliePath;
                //if (!Directory.Exists(str2))// 如果CSV文件不存在了，就需要提示
                if (!File.Exists(str2))
                {
                    return null;
                }
                StreamReader reader = new StreamReader(str2, System.Text.Encoding.Default);
                string line = "";
                List<string> listStrArr = new List<string>();
                while ((line = reader.ReadLine()) != null)
                {
                    listStrArr.Add(line);//将文件内容分割成数组
                }
                reader.Close();
                return listStrArr;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void Write_CSV1(object[] content, string fileName)// CSV表写入操作
        {
            StreamWriter writer = null;
            try
            {
                string str2 = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini") + "/" + DateTime.Now.ToString("yyyy年MM月dd日") + "/" + (fileName + "采集数据" + DateTime.Now.ToString("yyyy年MM月dd日")) + ".CSV";
                if (!File.Exists(str2))// 如果CSV文件不存在了，就需要重新创建一个CSV文件，并生成标题
                {
                    Write_CSV_Title(str2, DataGridViewClass.GetTitle(form.dataGridView1));
                }
                writer = new StreamWriter(new FileStream(str2, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.Default);

                // 向文件里面写入数据
                string str3 = "";
                int index = 0;
                try
                {
                    while (true)
                    {
                        if (index >= content.Length)
                        {
                            writer.WriteLine(str3);
                            writer.Close();
                            break;
                        }
                        if (index > 0)
                        {
                            str3 = str3 + ",";
                        }
                        str3 = str3 + content[index];
                        index++;
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception("导出错误" + exception.Message);
                }
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                MessageBox.Show("CSV路径下的文件夹不存在，程序无法搜索到指定的路径！");
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show("模组数据录入失败！\r\n\r\n原因：当天的CSV数据文件在外部被打开，请关闭！\r\n注意：Except文档不能被两名用户同时打开！", "危险错误提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }



        /// <summary> 向指定的CSV文件路径写入数据
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <param name="content">要写入的内容，数组</param>
        public void Write_CSVACR(object[] content, string fileName)// CSV表写入操作
        {
            StreamWriter writer = null;
            try
            {
                string currPath = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
                string dataPath = currPath + "/" + DateTime.Now.ToString("yyyy年MM月dd日");  //创建当天时间文件夹
                string subPath = dataPath + "/ACR测试";
                if (System.IO.Directory.Exists(subPath) == false)//如果不存在就创建file文件夹 
                {
                    Directory.CreateDirectory(subPath);
                }
                string str2 = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini") + "/" + DateTime.Now.ToString("yyyy年MM月dd日") + "/ACR测试" + "/" + (fileName == "" ? DateTime.Now.ToString("yyyy年MM月dd日") : fileName) + ".CSV";
                if (!File.Exists(str2))// 如果CSV文件不存在了，就需要重新创建一个CSV文件，并生成标题
                {
                    Write_CSV_Title(str2, DataGridViewClass.GetTitle(form.dataGridView10));
                }
                writer = new StreamWriter(new FileStream(str2, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.Default);

                // 向文件里面写入数据
                string str3 = "";
                int index = 0;
                try
                {
                    while (true)
                    {
                        if (index >= content.Length)
                        {
                            writer.WriteLine(str3);
                            writer.Close();
                            break;
                        }
                        if (index > 0)
                        {
                            str3 = str3 + ",";
                        }
                        str3 = str3 + content[index];
                        index++;
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception("导出错误" + exception.Message);
                }
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                MessageBox.Show("CSV路径下的文件夹不存在，程序无法搜索到指定的路径！");
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show("模组数据录入失败！\r\n\r\n原因：当天的CSV数据文件在外部被打开，请关闭！\r\n注意：Except文档不能被两名用户同时打开！", "危险错误提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        public void Write_CSVYCELL(object[] content, string fileName)// CSV表写入操作
        {
            StreamWriter writer = null;
            try
            {
                string currPath = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
                string dataPath = currPath + "/" + DateTime.Now.ToString("yyyy年MM月dd日");  //创建当天时间文件夹
                string subPath = dataPath + "/Y电容";
                if (System.IO.Directory.Exists(subPath) == false)//如果不存在就创建file文件夹 
                {
                    Directory.CreateDirectory(subPath);
                }
                string str2 = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini") + "/" + DateTime.Now.ToString("yyyy年MM月dd日") + "/Y电容" + "/" + (fileName == "" ? DateTime.Now.ToString("yyyy年MM月dd日") : fileName) + ".CSV";
                if (!File.Exists(str2))// 如果CSV文件不存在了，就需要重新创建一个CSV文件，并生成标题
                {
                    Write_CSV_Title(str2, DataGridViewClass.GetTitle(form.dataGridView11));
                }
                writer = new StreamWriter(new FileStream(str2, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.Default);

                // 向文件里面写入数据
                string str3 = "";
                int index = 0;
                try
                {
                    while (true)
                    {
                        if (index >= content.Length)
                        {
                            writer.WriteLine(str3);
                            writer.Close();
                            break;
                        }
                        if (index > 0)
                        {
                            str3 = str3 + ",";
                        }
                        str3 = str3 + content[index];
                        index++;
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception("导出错误" + exception.Message);
                }
            }
            catch (System.IO.DirectoryNotFoundException ex)
            {
                MessageBox.Show("CSV路径下的文件夹不存在，程序无法搜索到指定的路径！");
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show("模组数据录入失败！\r\n\r\n原因：当天的CSV数据文件在外部被打开，请关闭！\r\n注意：Except文档不能被两名用户同时打开！", "危险错误提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }


        /// <summary> 读取CSV表里面全部的内容
        /// </summary>
        public List<string> Read_CSV(string FliePath) {
            string str2 = FliePath;
            //if (!Directory.Exists(str2))// 如果CSV文件不存在了，就需要提示
            if (!File.Exists(str2))
            {
                return null;
            }
            StreamReader reader = new StreamReader(str2, System.Text.Encoding.Default);
            string line = "";
            List<string> listStrArr = new List<string>();
            while ((line = reader.ReadLine()) != null)
            {
                listStrArr.Add(line) ;//将文件内容分割成数组
            }
            reader.Close();
            return listStrArr;
        }

        private static void Write_CSV_Title(string fileName, object[] content) // 写入表头数据
        {
            //StreamWriter writer = new StreamWriter(new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8); System.Text.Encoding.Default
            StreamWriter writer = new StreamWriter(new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.Default); 
            // 向文件里面写入数据
            string str3 = "";
            int index = 0;
            try
            {
                while (true)
                {
                    if (index >= content.Length)
                    {
                        writer.WriteLine(str3);
                        writer.Close();
                        break;
                    }
                    if (index > 0)
                    {
                        str3 = str3 + ",";
                    }
                    str3 = str3 + content[index];
                    index++;
                }
            }
            catch (Exception exception)
            {
                throw new Exception("导出错误" + exception.Message);
            }
        }


        /// <summary> 向指定的CSV文件路径写入数据
        /// </summary>
        /// <param name="fileName">CSV文件路径</param>
        /// <param name="content">要写入的内容，数组</param>
        public void Write_MESLOG_CSV(object[] content, string filesName, string fileName)// CSV表写入操作
        {
            try
            {
                string Files = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini") + "/" + DateTime.Now.ToString("yyyy年MM月dd日") + "/" + filesName + "/";
                string str2 = Files + fileName + ".CSV";
                if (!File.Exists(Files))// 如果CSV文件不存在了，就需要重新创建一个CSV文件，并生成标题
                {
                    Directory.CreateDirectory(Files);
                }
                StreamWriter writer = new StreamWriter(new FileStream(str2, FileMode.Append, FileAccess.Write, FileShare.ReadWrite), System.Text.Encoding.Default);

                // 向文件里面写入数据
                string str3 = "";
                int index = 0;
                try
                {
                    while (true)
                    {
                        if (index >= content.Length)
                        {
                            writer.WriteLine(str3);
                            writer.Close();
                            break;
                        }
                        if (index > 0)
                        {
                            str3 = str3 + ",";
                        }
                        str3 = str3 + content[index];
                        index++;
                    }
                }
                catch (Exception exception)
                {
                    throw new Exception("导出错误" + exception.Message);
                }
            }
            catch (System.IO.DirectoryNotFoundException ex) { MessageBox.Show("CSV路径下的文件夹不存在，程序无法搜索到指定的路径！"); }
            catch (System.IO.IOException ex) { MessageBox.Show("MES数据录入失败！\r\n\r\n原因：当天的CSV数据文件在外部被打开，请关闭！\r\n注意：Except文档不能被两名用户同时打开！", "危险错误提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly); }
            catch (Exception ex) { }
        }

        /// <summary> 从ini文件里面将PLC的信号地址提取出来
        /// </summary>
        /// <returns>string[]素组</returns>
        public string[] FindIniPath(string PathName) 
        {
            for (int i = 0; i < form.Set_Ui.dataGridView20.Rows.Count; i++)
            {
                if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView20, i)[0].ToString() == PathName)
                {
                    string[] _PathName = { DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView20, i)[1].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView20, i)[2].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView20, i)[3].ToString() };
                    return _PathName;
                }
            }
            form.P_Class.TxtLog("检测不到PLC的【" + PathName + "】地址", 2);
            return new string[]{"","",""};
        }

        /// <summary> 读取txt文件某一行的数据
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <param name="index">读取的行号</param>
        /// <returns></returns>
        public static string Read_Txt(string FilePath, int index){
            string line = "";
            int tempIndex = 0;

            using (StreamReader sr = new StreamReader(FilePath))
            {
                while ((line = sr.ReadLine()) != null)
                {
                    // 读取txt的列内容
                    if (tempIndex == index)
                    {
                       sr.Close();
                       return line;
                    }
                    tempIndex++;
                }
            }
            return "";
        }

        /// <summary> 写入txt文件数据
        /// </summary>
        /// <param name="FilePath"></param>
        /// <param name="content"></param>
        public static void Write_Txt(string FilePath, string[] content) {
            using (StreamWriter sw = new StreamWriter(FilePath))
            {
                foreach (string s in content)
                {
                    sw.WriteLine(s);
                }
                sw.Close();
            }
        }

        public static void Write_DataGridView_ini(DataGridView dataGrid, string Title, string filePath)
        {
            // 将表里面的数据全部写入到ini表里去
            for (int i = 0; i < dataGrid.Rows.Count; i++)// 行循环
            {
                string tempstr = "";// 准备一个字符变量用来接收读取到的表行内容

                for (int Columns = 0; Columns < dataGrid.Columns.Count; Columns++)// 列循环
                {
                    if (Columns == dataGrid.Columns.Count - 1)
                    {// 不要后面的 - 符号了
                        tempstr += (dataGrid.Rows[i].Cells[Columns].Value == null ? "" : dataGrid.Rows[i].Cells[Columns].Value.ToString());
                        break;
                    }

                    // 由于进站和出站的user参数和我定义的 - 号规则有冲突，所以得特殊对待
                    if (Title == "进站" || Title == "出站" || Title == "首件")
                    {
                        tempstr += (dataGrid.Rows[i].Cells[Columns].Value == null ? "" : dataGrid.Rows[i].Cells[Columns].Value.ToString()) + "~";// 将读取到的表行内容交给准备好的变量
                    }
                    else {
                        tempstr += (dataGrid.Rows[i].Cells[Columns].Value == null ? "" : dataGrid.Rows[i].Cells[Columns].Value.ToString()) + "~";// 将读取到的表行内容交给准备好的变量
                    }
                }

                PublicClass.Set_ini_data(Title, i.ToString(), tempstr, filePath);
            }
        }
    }
}
