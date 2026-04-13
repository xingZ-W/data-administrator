using DataAdministraction.MES_Coonect;
using DataAdministraction.公用函数类;
using DataAdministrator;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace 设置
{
    public partial class Shezhi : Form
    {
        Form1 form;

        public Shezhi(Form1 form)
        {
            InitializeComponent();
            this.form = form;

            ini_data();

            tabControl1.Controls.Clear();
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Controls.Add(tabPage4);
            tabControl1.Controls.Add(tabPage5);
            tabControl1.Controls.Add(tabPage6);
        }

        /// <summary> 初始化界面上的参数
        /// </summary>
        private void ini_data() {
            comboBox1.Text = PublicClass.Get_ini_data("数据表主页", "选择的工站", System.Windows.Forms.Application.StartupPath + "/ini文件/主窗口配置参数.ini");

            textBox1.Text = PublicClass.Get_ini_data("配置参数", "设备标识", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
            textBox4.Text = PublicClass.Get_ini_data("配置参数", "操作者", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
            textBox3.Text = PublicClass.Get_ini_data("配置参数", "备注", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
            textBox5.Text = PublicClass.Get_ini_data("配置参数", "班次", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");

            for (int i = 0; i < comboBox2.Items.Count; i++)
            {
                try { comboBox2.Items.RemoveAt(i); } catch { }
            }
            for (int i = 0; i < form.dataGridView4.Rows.Count; i++ )
            {
                comboBox2.Items.Add(DataGridViewClass.GetRowsData(form.dataGridView4, i)[0]);
            }
            comboBox2.Text = DataGridViewClass.GetRowsData(form.dataGridView4, 0)[0];

            DataGridViewClass.RemoveAllColumns(dataGridView20);
            DataGridViewClass.AddColumns(dataGridView20,new string[]{"说明","地址","地址间隔","数据量"});

            // 将日志的路径显示出来
            textBox9.Text = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");

            #region 设置PLC信号地址表
            DataGridViewClass.RemoveAllRow(dataGridView20);
            for (int i = 0; i < 200; i++ )
            {
                string tempStr = PublicClass.Get_ini_data("信号地址", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/PLC窗口配置文件.ini");
                if (tempStr != null && tempStr != "")
                {
                    DataGridViewClass.AddRows(dataGridView20, tempStr.Split('~'), Color.White);             
                }
            }
            #endregion

            #region 设置进站表
            DataGridViewClass.RemoveAllRow(dataGridView2);
            for (int i = 0; i < 120; i++)
            {
                string tempStr = PublicClass.Get_ini_data("进站", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");
                if (tempStr != null && tempStr != "")
                {
                    DataGridViewClass.AddRows(dataGridView2, tempStr.Split('~'), Color.White);
                }
            }
            #endregion

            #region 设置出站表
            DataGridViewClass.RemoveAllRow(dataGridView3);
            for (int i = 0; i < 120; i++)
            {
                string tempStr = PublicClass.Get_ini_data("出站", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");
                if (tempStr != null && tempStr != "")
                {
                    DataGridViewClass.AddRows(dataGridView3, tempStr.Split('~'), Color.White);
                }
            }
            #endregion

            #region 设置首件表
            DataGridViewClass.RemoveAllRow(dataGridView4);
            for (int i = 0; i < 120; i++)
            {
                string tempStr = PublicClass.Get_ini_data("首件", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");
                if (tempStr != null && tempStr != "")
                {
                    DataGridViewClass.AddRows(dataGridView4, tempStr.Split('~'), Color.White);
                }
            }
            #endregion

            #region 设置参数表
            DataGridViewClass.RemoveAllRow(dataGridView5);
            for (int i = 0; i < 230; i++)
            {
                string tempStr = PublicClass.Get_ini_data("参数", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");
                if (tempStr != null && tempStr != "")
                {
                    DataGridViewClass.AddRows(dataGridView5, tempStr.Split('~'), Color.White);
                }
            }
            #endregion

            #region 设置表头数据表
            DataGridViewClass.RemoveAllRow(dataGridView6);
            string[] tempStr3 = PublicClass.Read_Txt(System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/表头.txt", 1).Split('~');
            for (int i = 0; i<tempStr3.Length; i++) {
                DataGridViewClass.AddRows(dataGridView6, new string[] { tempStr3[i] }, Color.White);
            }
            #endregion
        }

        private void Shezhi_Load(object sender, EventArgs e)   //表格顺序不可变
        {
            for (int i = 0; i < dataGridView2.Columns.Count; i++)
            {
                dataGridView2.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            for (int i = 0; i < dataGridView3.Columns.Count; i++)
            {
                dataGridView3.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            for (int i = 0; i < dataGridView4.Columns.Count; i++)
            {
                dataGridView4.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            for (int i = 0; i < dataGridView5.Columns.Count; i++)
            {
                dataGridView5.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            for (int i = 0; i < dataGridView6.Columns.Count; i++)
            {
                dataGridView6.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            for (int i = 0; i < dataGridView20.Columns.Count; i++)
            {
                dataGridView20.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }

        }
        
        /// <summary> 提升界面流畅度
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // 用双缓冲绘制窗口的所有子控件
                return cp;
            }
        }
        DataTable t_module = new DataTable();

        private void button6_Click(object sender, EventArgs e)// 点击了确认后要做的事
        {
            // 更改工站
            PublicClass.Set_ini_data("数据表主页", "选择的工站", comboBox1.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/主窗口配置参数.ini");
            if (comboBox1.Text != form.工站名)
            {
                form.工站名 = PublicClass.Get_ini_data("数据表主页", "选择的工站", System.Windows.Forms.Application.StartupPath + "/ini文件/主窗口配置参数.ini");
                MessageBox.Show("由于检测到切换了工站，所以撤销了本次其他修改，工站将进行切换！");
                form.Ini_Data();

                // 实例化子窗口
                form.Set_Ui = new Shezhi(form);

                form.Pms_Class = new MESClass(form);

                form.Ini();

                DataGridViewClass.DataGridCreateParams(form.dataGridView1);
                DataGridViewClass.DataGridCreateParams(form.dataGridView2);
            }
            else {
                PublicClass.Set_ini_data("配置参数", "设备标识", textBox1.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
                PublicClass.Set_ini_data("配置参数", "操作者", textBox4.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
                PublicClass.Set_ini_data("配置参数", "备注", textBox3.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
                PublicClass.Set_ini_data("配置参数", "日志路径", textBox9.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");

                PublicClass.Set_ini_data("信号地址", null,null, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/PLC窗口配置文件.ini");
                PublicClass.Set_ini_data("进站", null, null, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");
                PublicClass.Set_ini_data("参数", null, null, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");
                PublicClass.Set_ini_data("出站", null, null, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");
                PublicClass.Set_ini_data("首件", null, null, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");

                PublicClass.Write_DataGridView_ini(dataGridView20, "信号地址", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/PLC窗口配置文件.ini");
                PublicClass.Write_DataGridView_ini(dataGridView2, "进站", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");
                PublicClass.Write_DataGridView_ini(dataGridView5, "参数", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");
                PublicClass.Write_DataGridView_ini(dataGridView3, "出站", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");
                PublicClass.Write_DataGridView_ini(dataGridView4, "首件", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");

                #region 更改表头txt文件和ini表头标识符和当前窗口的表头数据表
                // 读取表头数据表的第一列所有数据
                string[] strTemp = new string[dataGridView6.Rows.Count];// 存储表头
                for (int i = 0; i < strTemp.Length; i++)
                {
                    strTemp[i] = DataGridViewClass.GetRowsData(dataGridView6, i)[0];
                }
                // 进行 - 组合
                string strTemp2 = "";
                for (int i = 0; i < strTemp.Length; i++)
                {
                    if (i == strTemp.Length - 1)
                    {
                        strTemp2 += strTemp[i];
                        break;
                    }
                    strTemp2 += strTemp[i] + "~";
                }
                // 读取txt表头文件里面的所有数据
                string[] tempList = new string[4];
                for (int i = 0; i < tempList.Length; i++)
                {
                    tempList[i] = PublicClass.Read_Txt(System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/表头.txt", i);
                }
                // 修改掉本地数据表头的那一行
                tempList[1] = strTemp2;
                // 将改好的表头写进表头txt文件里
                PublicClass.Write_Txt(System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/表头.txt", tempList);

                // 再读取表头txt文件里的表头出来
                DataGridViewClass.RemoveAllColumns(form.dataGridView2);// 移除主页的本地数据表的所有列
                DataGridViewClass.RemoveAllRow(dataGridView6);// 移除当前窗口的表头数据表的所有行
                string[] tempStr3 = PublicClass.Read_Txt(System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/表头.txt", 1).Split('~');
                for (int i = 0; i < tempStr3.Length; i++)
                {
                    // 写进主页的本地数据表里
                    DataGridViewClass.AddRows(dataGridView6, new string[] { tempStr3[i] }, Color.White);
                }
                //// 更换掉主页的本地数据表的表头
                DataGridViewClass.AddColumns(form.dataGridView2, tempStr3);
                #endregion
                ini_data();// 初始化set_ui窗口的所有数据

                form.Ini_Data();
                
            }

            GC.Collect();

            this.Hide();
        }

        private void shiShiBiaoTo(DataGridView dataGridView , DataGridView dataGridView2,int index) {
            // 读取表头数据表的第一列所有数据
            string[] str_Ary = new string[dataGridView.Rows.Count];// 存储表头
            for (int i = 0; i < str_Ary.Length; i++)
            {
                str_Ary[i] = DataGridViewClass.GetRowsData(dataGridView, i)[0];
            }
            // 进行 - 组合
            string str_Ary2 = "";
            for (int i = 0; i < str_Ary.Length; i++)
            {
                if (i == str_Ary.Length - 1)
                {
                    str_Ary2 += str_Ary[i];
                    break;
                }
                str_Ary2 += str_Ary[i] + "-";
            }
            // 读取txt表头文件里面的所有数据
            string[] str_List = new string[5];
            for (int i = 0; i < str_List.Length; i++)
            {
                str_List[i] = PublicClass.Read_Txt(System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/表头2.txt", i);
            }
            // 修改掉本地数据表头的那一行
            str_List[index] = str_Ary2;
            // 将改好的表头写进表头txt文件里
            PublicClass.Write_Txt(System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/表头2.txt", str_List);

            // 再读取表头txt文件里的表头出来
            DataGridViewClass.RemoveAllColumns(form.dataGridView2);// 移除主页的本地数据表的所有列
            DataGridViewClass.RemoveAllRow(dataGridView);// 移除当前窗口的表头数据表的所有行
            string[] str_Ary3 = PublicClass.Read_Txt(System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/表头2.txt", index).Split('-');
            for (int i = 0; i < str_Ary3.Length; i++)
            {
                // 写进主页的本地数据表里
                DataGridViewClass.AddRows(dataGridView, new string[] { str_Ary3[i] }, Color.White);
            }
            //// 更换掉主页的本地数据表的表头
            DataGridViewClass.AddColumns(form.dataGridView2, str_Ary3);
        }

        #region 表头修改
        private void button2_Click(object sender, EventArgs e)// 导入
        {
            // 弹出文件选择对话框
            try
            {
                // openLabelFileDialog是一个选择文件的一个对话窗口，这是一个控件
                openLabelFileDialog.Filter = "CSV文件(*.CSV)|*.CSV|所有文件(*.*)|*.*";
                //openLabelFileDialog.InitialDirectory = @"C:\Users\admin\Desktop";
                DialogResult dialogResult = openLabelFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    // 清空当前数据表里面的数据
                    DataGridViewClass.RemoveAllRow(dataGridView6);
                    // 读取外部CSV文件表里面的内容
                    List<string> strTemp = form.P_Class.Read_CSV(this.openLabelFileDialog.FileName);
                    for (int i = 0; i < strTemp.Count; i++) {
                        // 将List转换成String[]
                        DataGridViewClass.AddRows(dataGridView6, strTemp[i].Split(','), Color.White);
                    }
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show("选择CSV文件异常：" + ex.Message);
            }
        }

        private void button1_Click(object sender, EventArgs e)// 导出
        {
            // 弹出选择文件路径保存对话框
            try
            {
                // openLabelFileDialog是一个选择文件的一个对话窗口，这是一个控件
                saveFileDialog1.Filter = "CSV文件(*.CSV)|*.CSV";
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择文件路径";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 判断这个路径下是否存在旧的CSV表头文件
                    if (File.Exists(dialog.SelectedPath + "/表头.CSV")) {
                        File.Delete(dialog.SelectedPath + "/表头.CSV");
                    }

                    // 生成CSV文件
                    StreamWriter writer = new StreamWriter(new FileStream(dialog.SelectedPath + "/表头.CSV", FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);
                    // 向文件里面写入数据
                    for (int i = 0; i < dataGridView6.Rows.Count; i++) {
                        string[] temp = DataGridViewClass.GetRowsData(dataGridView6, i);

                        string str = "";
                        // 加入 , 号
                        for (int k = 0; k < temp.Length; k++) {
                            if (k == temp.Length - 1) {
                                str = str + temp[k];
                                break;
                            }
                            str = str + temp[k] + ",";
                        }

                        // 写入数据
                        writer.WriteLine(str);
                    }
                    writer.Close();
                }
                dialog.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("选择CSV文件路径异常：" + ex.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)// 增加
        {
            DataGridViewClass.AddRows(dataGridView6, new string[] { }, Color.White);
        }

        private void button5_Click(object sender, EventArgs e)// 删除
        {
            DataGridViewClass.RemoveIndexRow(dataGridView6, dataGridView6.CurrentRow.Index);
        }
        private void button4_Click(object sender, EventArgs e)// 刷新
        {
            // 移除所有行
            DataGridViewClass.RemoveAllRow(dataGridView6);
            // 重新导入表头
            string[] tempStr3 = PublicClass.Read_Txt(System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/表头.txt", 1).Split('-');
            for (int i = 0; i < tempStr3.Length; i++)
            {
                DataGridViewClass.AddRows(dataGridView6, new string[] { tempStr3[i] }, Color.White);
            }
        }
        
        #endregion

        #region PMS表更改

        #region 进站
        private void button37_Click(object sender, EventArgs e)
        {
            // 弹出文件选择对话框
            try
            {
                // openLabelFileDialog是一个选择文件的一个对话窗口，这是一个控件
                openLabelFileDialog.Filter = "CSV文件(*.CSV)|*.CSV|所有文件(*.*)|*.*";
                //openLabelFileDialog.InitialDirectory = @"C:\Users\admin\Desktop";
                DialogResult dialogResult = openLabelFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    // 清空当前数据表里面的数据
                    DataGridViewClass.RemoveAllRow(dataGridView2);
                    // 读取外部CSV文件表里面的内容
                    List<string> strTemp = form.P_Class.Read_CSV(this.openLabelFileDialog.FileName);
                    for (int i = 0; i < strTemp.Count; i++)
                    {
                        // 将List转换成String[]
                        DataGridViewClass.AddRows(dataGridView2, strTemp[i].Split(','), Color.White);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("选择CSV文件异常：" + ex.Message);
            }
        }

        private void button33_Click(object sender, EventArgs e)
        {
            // 弹出选择文件路径保存对话框
            try
            {
                // openLabelFileDialog是一个选择文件的一个对话窗口，这是一个控件
                saveFileDialog1.Filter = "CSV文件(*.CSV)|*.CSV";
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择文件路径";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 判断这个路径下是否存在旧的CSV表头文件
                    if (File.Exists(dialog.SelectedPath + "/进站.CSV"))
                    {
                        File.Delete(dialog.SelectedPath + "/进站.CSV");
                    }

                    // 生成CSV文件
                    StreamWriter writer = new StreamWriter(new FileStream(dialog.SelectedPath + "/进站.CSV", FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);
                    // 向文件里面写入数据
                    for (int i = 0; i < dataGridView2.Rows.Count; i++)
                    {
                        string[] temp = DataGridViewClass.GetRowsData(dataGridView2, i);

                        string str = "";
                        // 加入 , 号
                        for (int k = 0; k < temp.Length; k++)
                        {
                            if (k == temp.Length - 1)
                            {
                                str = str + temp[k];
                                break;
                            }
                            str = str + temp[k] + ",";
                        }

                        // 写入数据
                        writer.WriteLine(str);
                    }
                    writer.Close();
                }
                dialog.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("选择CSV文件路径异常：" + ex.Message);
            }
        }

        private void button36_Click(object sender, EventArgs e)
        {
            DataGridViewClass.AddRows(dataGridView2, new string[] { }, Color.White);
        }

        private void button35_Click(object sender, EventArgs e)
        {
            DataGridViewClass.RemoveIndexRow(dataGridView2, dataGridView2.CurrentRow.Index);
        }

        private void button34_Click(object sender, EventArgs e)
        {
            DataGridViewClass.RemoveAllRow(dataGridView2);
            for (int i = 0; i < 120; i++)
            {
                string tempStr = PublicClass.Get_ini_data("进站", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");
                if (tempStr != null && tempStr != "")
                {
                    DataGridViewClass.AddRows(dataGridView2, tempStr.Split('~'), Color.White);
                }
            }
        }
        #endregion

        #region 出站
        private void button32_Click(object sender, EventArgs e)
        {
            // 弹出文件选择对话框
            try
            {
                // openLabelFileDialog是一个选择文件的一个对话窗口，这是一个控件
                openLabelFileDialog.Filter = "CSV文件(*.CSV)|*.CSV|所有文件(*.*)|*.*";
                //openLabelFileDialog.InitialDirectory = @"C:\Users\admin\Desktop";
                DialogResult dialogResult = openLabelFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    // 清空当前数据表里面的数据
                    DataGridViewClass.RemoveAllRow(dataGridView3);
                    // 读取外部CSV文件表里面的内容
                    List<string> strTemp = form.P_Class.Read_CSV(this.openLabelFileDialog.FileName);
                    for (int i = 0; i < strTemp.Count; i++)
                    {
                        // 将List转换成String[]
                        DataGridViewClass.AddRows(dataGridView3, strTemp[i].Split(','), Color.White);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("选择CSV文件异常：" + ex.Message);
            }
        }

        private void button28_Click(object sender, EventArgs e)
        {
            // 弹出选择文件路径保存对话框
            try
            {
                // openLabelFileDialog是一个选择文件的一个对话窗口，这是一个控件
                saveFileDialog1.Filter = "CSV文件(*.CSV)|*.CSV";
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择文件路径";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 判断这个路径下是否存在旧的CSV表头文件
                    if (File.Exists(dialog.SelectedPath + "/出站.CSV"))
                    {
                        File.Delete(dialog.SelectedPath + "/出站.CSV");
                    }

                    // 生成CSV文件
                    StreamWriter writer = new StreamWriter(new FileStream(dialog.SelectedPath + "/出站.CSV", FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);
                    // 向文件里面写入数据
                    for (int i = 0; i < dataGridView3.Rows.Count; i++)
                    {
                        string[] temp = DataGridViewClass.GetRowsData(dataGridView3, i);

                        string str = "";
                        // 加入 , 号
                        for (int k = 0; k < temp.Length; k++)
                        {
                            if (k == temp.Length - 1)
                            {
                                str = str + temp[k];
                                break;
                            }
                            str = str + temp[k] + ",";
                        }

                        // 写入数据
                        writer.WriteLine(str);
                    }
                    writer.Close();
                }
                dialog.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("选择CSV文件路径异常：" + ex.Message);
            }
        }

        private void button31_Click(object sender, EventArgs e)
        {
            DataGridViewClass.AddRows(dataGridView3, new string[] { }, Color.White);
        }

        private void button30_Click(object sender, EventArgs e)
        {
            DataGridViewClass.RemoveIndexRow(dataGridView3, dataGridView3.CurrentRow.Index);
        }

        private void button29_Click(object sender, EventArgs e)
        {
            DataGridViewClass.RemoveAllRow(dataGridView3);
            for (int i = 0; i < 120; i++)
            {
                string tempStr = PublicClass.Get_ini_data("出站", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");
                if (tempStr != null && tempStr != "")
                {
                    DataGridViewClass.AddRows(dataGridView3, tempStr.Split('~'), Color.White);
                }
            }
        }
        
        #endregion

        #region 首件
        private void button27_Click(object sender, EventArgs e)
        {
            // 弹出文件选择对话框
            try
            {
                // openLabelFileDialog是一个选择文件的一个对话窗口，这是一个控件
                openLabelFileDialog.Filter = "CSV文件(*.CSV)|*.CSV|所有文件(*.*)|*.*";
                //openLabelFileDialog.InitialDirectory = @"C:\Users\admin\Desktop";
                DialogResult dialogResult = openLabelFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    // 清空当前数据表里面的数据
                    DataGridViewClass.RemoveAllRow(dataGridView4);
                    // 读取外部CSV文件表里面的内容
                    List<string> strTemp = form.P_Class.Read_CSV(this.openLabelFileDialog.FileName);
                    for (int i = 0; i < strTemp.Count; i++)
                    {
                        // 将List转换成String[]
                        DataGridViewClass.AddRows(dataGridView4, strTemp[i].Split(','), Color.White);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("选择CSV文件异常：" + ex.Message);
            }
        }

        private void button23_Click(object sender, EventArgs e)
        {
            // 弹出选择文件路径保存对话框
            try
            {
                // openLabelFileDialog是一个选择文件的一个对话窗口，这是一个控件
                saveFileDialog1.Filter = "CSV文件(*.CSV)|*.CSV";
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择文件路径";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 判断这个路径下是否存在旧的CSV表头文件
                    if (File.Exists(dialog.SelectedPath + "/首件.CSV"))
                    {
                        File.Delete(dialog.SelectedPath + "/首件.CSV");
                    }

                    // 生成CSV文件
                    StreamWriter writer = new StreamWriter(new FileStream(dialog.SelectedPath + "/首件.CSV", FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);
                    // 向文件里面写入数据
                    for (int i = 0; i < dataGridView4.Rows.Count; i++)
                    {
                        string[] temp = DataGridViewClass.GetRowsData(dataGridView4, i);

                        string str = "";
                        // 加入 , 号
                        for (int k = 0; k < temp.Length; k++)
                        {
                            if (k == temp.Length - 1)
                            {
                                str = str + temp[k];
                                break;
                            }
                            str = str + temp[k] + ",";
                        }

                        // 写入数据
                        writer.WriteLine(str);
                    }
                    writer.Close();
                }
                dialog.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("选择CSV文件路径异常：" + ex.Message);
            }
        }

        private void button26_Click(object sender, EventArgs e)
        {
            DataGridViewClass.AddRows(dataGridView4, new string[] { }, Color.White);
        }

        private void button25_Click(object sender, EventArgs e)
        {
            DataGridViewClass.RemoveIndexRow(dataGridView4, dataGridView4.CurrentRow.Index);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            DataGridViewClass.RemoveAllRow(dataGridView4);
            for (int i = 0; i < 120; i++)
            {
                string tempStr = PublicClass.Get_ini_data("首件", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");
                if (tempStr != null && tempStr != "")
                {
                    DataGridViewClass.AddRows(dataGridView4, tempStr.Split('~'), Color.White);
                }
            }
        }

        #endregion

        #region 上传参数
        private void button22_Click(object sender, EventArgs e)
        {
            // 弹出文件选择对话框
            try
            {
                // openLabelFileDialog是一个选择文件的一个对话窗口，这是一个控件
                openLabelFileDialog.Filter = "CSV文件(*.CSV)|*.CSV|所有文件(*.*)|*.*";
                //openLabelFileDialog.InitialDirectory = @"C:\Users\admin\Desktop";
                DialogResult dialogResult = openLabelFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    // 清空当前数据表里面的数据
                    DataGridViewClass.RemoveAllRow(dataGridView5);
                    // 读取外部CSV文件表里面的内容
                    List<string> strTemp = form.P_Class.Read_CSV(this.openLabelFileDialog.FileName);
                    for (int i = 0; i < strTemp.Count; i++)
                    {
                        // 将List转换成String[]
                        DataGridViewClass.AddRows(dataGridView5, strTemp[i].Split(','), Color.White);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("选择CSV文件异常：" + ex.Message);
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            // 弹出选择文件路径保存对话框
            try
            {
                // openLabelFileDialog是一个选择文件的一个对话窗口，这是一个控件
                saveFileDialog1.Filter = "CSV文件(*.CSV)|*.CSV";
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择文件路径";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 判断这个路径下是否存在旧的CSV表头文件
                    if (File.Exists(dialog.SelectedPath + "/参数.CSV"))
                    {
                        File.Delete(dialog.SelectedPath + "/参数.CSV");
                    }

                    // 生成CSV文件
                    StreamWriter writer = new StreamWriter(new FileStream(dialog.SelectedPath + "/参数.CSV", FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);
                    // 向文件里面写入数据
                    for (int i = 0; i < dataGridView5.Rows.Count; i++)
                    {
                        string[] temp = DataGridViewClass.GetRowsData(dataGridView5, i);

                        string str = "";
                        // 加入 , 号
                        for (int k = 0; k < temp.Length; k++)
                        {
                            if (k == temp.Length - 1)
                            {
                                str = str + temp[k];
                                break;
                            }
                            str = str + temp[k] + ",";
                        }

                        // 写入数据
                        writer.WriteLine(str);
                    }
                    writer.Close();
                }
                dialog.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("选择CSV文件路径异常：" + ex.Message);
            }
        }

        private void button21_Click(object sender, EventArgs e)
        {
            DataGridViewClass.AddRows(dataGridView5, new string[] { }, Color.White);
        }

        private void button20_Click(object sender, EventArgs e)
        {
            DataGridViewClass.RemoveIndexRow(dataGridView5, dataGridView5.CurrentRow.Index);
        }

        private void button19_Click(object sender, EventArgs e)
        {
            DataGridViewClass.RemoveAllRow(dataGridView5);
            for (int i = 0; i < 120; i++)
            {
                string tempStr = PublicClass.Get_ini_data("参数", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini");
                if (tempStr != null && tempStr != "")
                {
                    DataGridViewClass.AddRows(dataGridView5, tempStr.Split('-'), Color.White);
                }
            }
        }

        #endregion

        #region PLC设备信号配置
        private void button17_Click(object sender, EventArgs e)
        {
            // 弹出文件选择对话框
            try
            {
                // openLabelFileDialog是一个选择文件的一个对话窗口，这是一个控件
                openLabelFileDialog.Filter = "CSV文件(*.CSV)|*.CSV|所有文件(*.*)|*.*";
                //openLabelFileDialog.InitialDirectory = @"C:\Users\admin\Desktop";
                DialogResult dialogResult = openLabelFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    // 清空当前数据表里面的数据
                    DataGridViewClass.RemoveAllRow(dataGridView20);
                    // 读取外部CSV文件表里面的内容
                    List<string> strTemp = form.P_Class.Read_CSV(this.openLabelFileDialog.FileName);
                    for (int i = 0; i < strTemp.Count; i++)
                    {
                        // 将List转换成String[]
                        DataGridViewClass.AddRows(dataGridView20, strTemp[i].Split(','), Color.White);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("选择CSV文件异常：" + ex.Message);
            }
        }

        private void button13_Click(object sender, EventArgs e)
        {
            // 弹出选择文件路径保存对话框
            try
            {
                // openLabelFileDialog是一个选择文件的一个对话窗口，这是一个控件
                saveFileDialog1.Filter = "CSV文件(*.CSV)|*.CSV";
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                dialog.Description = "请选择文件路径";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 判断这个路径下是否存在旧的CSV表头文件
                    if (File.Exists(dialog.SelectedPath + "/PLC参数.CSV"))
                    {
                        File.Delete(dialog.SelectedPath + "/PLC参数.CSV");
                    }

                    // 生成CSV文件
                    StreamWriter writer = new StreamWriter(new FileStream(dialog.SelectedPath + "/PLC参数.CSV", FileMode.Append, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);
                    // 向文件里面写入数据
                    for (int i = 0; i < dataGridView20.Rows.Count; i++)
                    {
                        string[] temp = DataGridViewClass.GetRowsData(dataGridView20, i);

                        string str = "";
                        // 加入 , 号
                        for (int k = 0; k < temp.Length; k++)
                        {
                            if (k == temp.Length - 1)
                            {
                                str = str + temp[k];
                                break;
                            }
                            str = str + temp[k] + ",";
                        }

                        // 写入数据
                        writer.WriteLine(str);
                    }
                    writer.Close();
                }
                dialog.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("选择CSV文件路径异常：" + ex.Message);
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            DataGridViewClass.AddRows(dataGridView20, new string[] { }, Color.White);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            DataGridViewClass.RemoveIndexRow(dataGridView20, dataGridView20.CurrentRow.Index);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            DataGridViewClass.RemoveAllRow(dataGridView20);
            for (int i = 0; i < 120; i++)
            {
                string tempStr = PublicClass.Get_ini_data("信号地址", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/PLC窗口配置文件.ini");
                if (tempStr != null && tempStr != "")
                {
                    DataGridViewClass.AddRows(dataGridView20, tempStr.Split('~'), Color.White);
                }
            }
        }
        #endregion

        #endregion

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            PublicClass.Set_ini_data("配置参数", "模组码", textBox2.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            PublicClass.Set_ini_data("配置参数", "PN", textBox10.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            PublicClass.Set_ini_data("配置参数", "模组类型", textBox11.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
        }
    }
}
