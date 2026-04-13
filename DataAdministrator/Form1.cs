using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAdministraction.功能插件.PLC设置界面;
using DataAdministraction.公用函数类;
using DataAdministrator.功能插件.用户管理页;
using DataAdministrator.功能插件.用户登陆窗口;
using DataAdministrator.功能插件.右击菜单;
using DataAdministrator.功能插件.产品信息页;
using fileDelete;
using DataAdministrator.自动流程;
using 设置;
using System.Threading;
using DataAdministrator.功能插件.本地数据页;
using DataAdministrator.功能插件.斑马打印机连接;
using System.IO;
using DataAdministraction.MES_Coonect;
using Newtonsoft.Json.Linq;
using JP_DLL;

namespace DataAdministrator
{
    public partial class Form1 : Form
    {
        #region 全局变量
        public PLC_FatherClass PLC_ui;// PLC对象
        public PublicClass P_Class;// 公用对象
        public Right_Menu Right_menu;// 右击菜单对象

        public string 工站名 = "";

        public Father MySignl;// 自动流程对象

        public Shezhi Set_Ui;

        public MESClass Pms_Class;

        public BT3562 mBT3562;

        public L100B mL100B;
        //public BarTender_Windows barTender;
        #endregion

        public Form1()
        {
            InitializeComponent();
            SetTitleCenter("Native trace V10/08");

            工站名 = PublicClass.Get_ini_data("数据表主页", "选择的工站", System.Windows.Forms.Application.StartupPath + "/ini文件/主窗口配置参数.ini");

            Ini_Data();

            // 实例化子窗口
            Set_Ui = new Shezhi(this);

            Pms_Class = new MESClass(this);

            mBT3562 = new BT3562(this);

            mL100B = new L100B(this);
            //barTender = new BarTender_Windows(this);
            Ini();

            DataGridViewClass.DataGridCreateParams(this.dataGridView1);
            DataGridViewClass.DataGridCreateParams(this.dataGridView2);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Right_menu = new Right_Menu(this);
            button22.Visible = false;
            button23.Visible = false;
            button24.Visible = false;


        }
        /// <summary> 初始化所有form函数的全局变量对象
        /// </summary>
        public void Ini()
        {
            // 初始化公用类对象
            P_Class = new PublicClass(this);

            #region PLC连接
            PLC_ui = new PLC_Siemens
            (
                PublicClass.Get_ini_data("西门子", "ip地址", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/PLC窗口配置文件.ini"),// ip地址
                PublicClass.Get_ini_data("西门子", "PLC版本", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/PLC窗口配置文件.ini"),// 版本
                Convert.ToInt32(PublicClass.Get_ini_data("西门子", "超时", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/PLC窗口配置文件.ini"))// 超时
            );
            if (PLC_ui.connect.IsSuccess)
            {
                通讯状态.Text = "正常";
                通讯状态.ForeColor = Color.Green;
                P_Class.TxtLog("PLC通讯成功！", 2);
                label6.BackColor = Color.Green;
                label6.Text = "进站1日志";
                label7.BackColor = Color.Green;
                label7.Text = "进站2日志";

                采集模式.Text = "在线";
                采集模式.ForeColor = Color.Green;
            }
            else {
                通讯状态.Text = "连接失败";
                通讯状态.ForeColor = Color.Red;
                P_Class.TxtLog("PLC通讯失败！\r\n【PLC配置】\r\nIP：" + PLC_ui.siemens.ip + "\r\n型号：" + PLC_ui.siemens.versions + "\r\n超时：" + PLC_ui.siemens.overTime, 2);
                label6.BackColor = Color.Red;
                label6.Text = "PLC通讯异常";

                采集模式.Text = "离线";
                采集模式.ForeColor = Color.Pink;
                运行状态.Text = "未运行";
                运行状态.ForeColor = Color.Red;
            }
            #endregion
            工序名称.Text = 工站名;
            tabControl2.Controls.Clear();
            tabControl3.Controls.Clear();
            tabControl3.TabPages.Add(tabPage6);
            tabPage6.Text = 工站名;
            switch (工序名称.Text)
            {
                case "U壳体上料涂胶":
                    tabControl3.TabPages.Add(tabPage7); //电芯入壳界面
                    tabControl2.TabPages.Add(tabPage9);   // 称重界面
                    read20("U壳体上料涂胶",dataGridView2);
                    read20("电芯入壳",dataGridViewKIN);
                    MySignl = null;
                    MySignl = new U壳体上料涂胶(this, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名);
                    break;
                case "极耳折弯滚平":
                    read20("极耳折弯滚平",dataGridView2);
                    MySignl = null;
                    MySignl = new 极耳折弯滚平(this, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名);
                    break;
                case "极耳焊接":
                    read20("极耳焊接", dataGridView2);
                    MySignl = null;
                    MySignl = new 极耳焊接(this, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名);
                    break;
                case "极耳焊后":
                    read20("极耳焊后", dataGridView2);
                    MySignl = null;
                    MySignl = new 极耳焊后(this, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名);
                    break;
                case "外壳焊接":
                    read20("外壳焊接", dataGridView2);
                    MySignl = null;
                    MySignl = new 外壳焊接(this, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名);
                    break;
                case "外壳焊后":
                    read20("外壳焊后", dataGridView2);

                    string[] head7 = new string[0];
                    //添加列名
                    head7 = PublicClass.Get_ini_data("本地数据表头", "外壳焊后", System.Windows.Forms.Application.StartupPath + "/ini文件/本地数据表头.ini").Split('-');
                    for (int i = 0; i < head7.Length; i++)
                    {
                        comboBox14.Items.Add(head7[i]);
                    }

                    MySignl = null;
                    MySignl = new 外壳焊后(this, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名);
                    break;
                case "自动热铆":
                    read20("自动热铆", dataGridView2);
                    MySignl = null;
                    MySignl = new 自动热铆(this, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名);
                    break;
                case "绝缘耐压":
                    tabControl2.TabPages.Add(jueyuanceshi);   //绝缘耐压界面
                    tabControl2.TabPages.Add(tabPage8);   // Y电容ACR测试界面
                    read20("绝缘耐压", dataGridView2);              
                    read20("Y电容", dataGridView11);                 
                    read20("ACR测试", dataGridView10);
                    insulationResistance();
                    ///ACR初始化
                    ACRRest();
                    tabControl3.TabPages.Add(Ycell);
                    tabControl3.TabPages.Add(tabPage10);
                    MySignl = null;
                    MySignl = new 绝缘耐压(this, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名);
                    break;
            }
        }

        public void insulationResistance() {
            textBox1.Text = PublicClass.Get_ini_data("配置参数", "串口号", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
            textBox5.Text = PublicClass.Get_ini_data("配置参数", "端口号", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");

            textBox18.Text = PublicClass.Get_ini_data("配置参数", "绝缘电压", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
            textBox17.Text = PublicClass.Get_ini_data("配置参数", "绝缘电阻", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
            textBox16.Text = PublicClass.Get_ini_data("配置参数", "测试时间", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
            textBox15.Text = PublicClass.Get_ini_data("配置参数", "设定值", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");

            textBox22.Text = PublicClass.Get_ini_data("配置参数", "耐压电压",  System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
            textBox21.Text = PublicClass.Get_ini_data("配置参数", "耐压电流",  System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
            textBox20.Text = PublicClass.Get_ini_data("配置参数", "耐压时间",  System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
            textBox19.Text = PublicClass.Get_ini_data("配置参数", "延时时间", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
        }

        public async void ACRRest()
        {
            bool a = await mBT3562.SetModle("COM1", 9600);
        }
        /// <summary> 初始化界面数据
        /// </summary>
        public void Ini_Data()
        {


            // 删除所有数据表的列
            DataGridViewClass.RemoveAllColumns(dataGridView1);
            DataGridViewClass.RemoveAllColumns(dataGridView2);
            DataGridViewClass.RemoveAllColumns(dataGridView3);
            DataGridViewClass.RemoveAllColumns(dataGridView4);

            // 给 数据采集   添加表头
            DataGridViewClass.AddColumns(dataGridView1, PublicClass.Read_Txt(System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/表头.txt", 0).Split('~'));
            // 给 本地数据   添加表头
            DataGridViewClass.AddColumns(dataGridView2, PublicClass.Read_Txt(System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/表头.txt", 1).Split('~'));
            // 给 用户管理   添加表头
            DataGridViewClass.AddColumns(dataGridView3, PublicClass.Read_Txt(System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/表头.txt", 2).Split('~'));
            // 给 产品信息   添加表头
            DataGridViewClass.AddColumns(dataGridView4, PublicClass.Read_Txt(System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/表头.txt", 3).Split('~'));
            // 将表头铺满整个表的宽度
            DataGridViewClass.SetTitleWidth(dataGridView1);
            DataGridViewClass.SetTitleWidth(dataGridView3);
            DataGridViewClass.SetTitleWidth(dataGridView4);
            // 给用户管理页面的数据表添加内容
            for (int i = 0; i < 50; i++)
            {
                string tempUser = PublicClass.Get_ini_data("账户", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/用户账号.ini");

                if (tempUser != null && tempUser != "")
                {
                    DataGridViewClass.AddRows(dataGridView3, tempUser.Split('~'), Color.White);
                }
            }
            // 给产品信息页面的数据表添加内容
            for (int i = 0; i < 100; i++)
            {
                string tempStr = PublicClass.Get_ini_data("产品信息", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
                if (tempStr != null && tempStr != "")
                {
                    DataGridViewClass.AddRows(dataGridView4, tempStr.Split('~'), Color.White);
                }
            }

            设备标识.Text = PublicClass.Get_ini_data("配置参数", "设备标识", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");

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

        private void SetTitleCenter(string Title)// 居中窗口标题文字
        {
            this.Text = Title;
            //string titleMsg = Title;
            //Graphics g = this.CreateGraphics();
            //Double startingPoint = (this.Width / 2) - (g.MeasureString(titleMsg, this.Font).Width / 2);
            //Double widthOfASpace = g.MeasureString(" ", this.Font).Width;
            //String tmp = " ";
            //Double tmpWidth = 0;
     
            //while ((tmpWidth + widthOfASpace) < startingPoint)
            //{
            //    tmp += " ";
            //    tmpWidth += widthOfASpace;
            //}
            //this.Text = tmp + titleMsg;
        }

        private void button1_Click(object sender, EventArgs e)// 数据采集按钮
        {
            // 改变其他按钮的背景颜色
            button1.BackColor = Color.White;
            tabControl1.SelectedIndex = 0;

            //string[] tempStr = new string[10];
            //tempStr[0] = "电芯入壳";
            //tempStr[1] = "23";
            //tempStr[3] = "45";
            //tempStr[2] = "32";
            //tempStr[4] = "234";
            //tempStr[5] = "412541";
            //DataGridViewClass.AddRows(dataGridViewKIN, tempStr, Color.Red);
            //P_Class.Write_CSV(dataGridViewKIN, tempStr, "电芯入壳");
            ////P_Class.Write_CSVACR(tempStr, "ACR测试");
            ////P_Class.Write_CSVYCELL(tempStr, "Y电容");
            ////tempStr[6] = "U壳体上料涂胶";
            //Thread.Sleep(500);
            //DataGridViewClass.AddRows(dataGridView2, tempStr, Color.Red);
            //P_Class.Write_CSV(dataGridView2, tempStr, "U壳体上料涂胶");

        }

        private void button2_Click(object sender, EventArgs e)// 本地数据按钮
        {
            // 改变其他按钮的背景颜色
            button2.BackColor = Color.White;
            tabControl1.SelectedIndex = 1;
            
            button1.BackColor = Color.Gainsboro;
            button3.BackColor = Color.Gainsboro;
            button4.BackColor = Color.Gainsboro;
            button8.BackColor = Color.Gainsboro;
          
            button6.BackColor = Color.Gainsboro;

        }

        private void button3_Click(object sender, EventArgs e)// 用户管理按钮
        {
            // 改变其他按钮的背景颜色
            button3.BackColor = Color.White;
            tabControl1.SelectedIndex = 2;

            button1.BackColor = Color.Gainsboro;
            button2.BackColor = Color.Gainsboro;
            button4.BackColor = Color.Gainsboro;
            button8.BackColor = Color.Gainsboro;
            button6.BackColor = Color.Gainsboro;
        }

        private void button4_Click(object sender, EventArgs e)// 产品信息按钮
        {
            // 改变其他按钮的背景颜色
            button4.BackColor = Color.White;
            tabControl1.SelectedIndex = 3;

            button1.BackColor = Color.Gainsboro;
            button2.BackColor = Color.Gainsboro;
            button3.BackColor = Color.Gainsboro;
            button8.BackColor = Color.Gainsboro;
            button6.BackColor = Color.Gainsboro;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            button8.BackColor = Color.White;

            button1.BackColor = Color.Gainsboro;
            button2.BackColor = Color.Gainsboro;
            button3.BackColor = Color.Gainsboro;
            button4.BackColor = Color.Gainsboro;
            button6.BackColor = Color.Gainsboro;
            Set_Ui.Show();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            button6.BackColor = Color.White;

            tabControl1.SelectedIndex = 4;
            button1.BackColor = Color.Gainsboro;
            button2.BackColor = Color.Gainsboro;
            button3.BackColor = Color.Gainsboro;
            button4.BackColor = Color.Gainsboro;
            button8.BackColor = Color.Gainsboro;
        }

        #region 用户管理页面的操作
        private void button11_Click(object sender, EventArgs e)// 新增用户
        {
            Add_User add_user_ui = new Add_User(this);
            add_user_ui.Show();
        }

        private void button12_Click(object sender, EventArgs e)// 删除用户
        {
            if (this.dataGridView3.CurrentRow == null)
            {
                MessageBox.Show("请选择用户进行操作！");
                return;
            }
            if (MessageBox.Show("确认删除？", "此删除不可恢复", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // 移除ini表里面的信息
                //PublicClass.Write_DataGridView_ini(dataGridView3, "账户", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/用户账号.ini");
                PublicClass.Set_ini_data("账户", dataGridView3.CurrentRow.Index.ToString(), "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/用户账号.ini");

                DataGridViewClass.RemoveIndexRow(dataGridView3, dataGridView3.CurrentRow.Index);

                
                MessageBox.Show("删除成功！");
            }
        }

        private void button13_Click(object sender, EventArgs e)// 修改用户
        {
            if (this.dataGridView3.CurrentRow == null)
            {
                MessageBox.Show("请选择用户进行操作！");
                return;
            }

            string[] contont = DataGridViewClass.GetRowsData(dataGridView3, dataGridView3.CurrentRow.Index);
            Add_User add_user_ui = new Add_User(this, contont[0], contont[1], contont[2], dataGridView3.CurrentRow.Index);
            add_user_ui.Show();
        }
        #endregion
        
        private void button5_Click(object sender, EventArgs e)// 登陆用户按钮
        {
            if (button5.Text == "      登陆")
            {
                User_Log Log = new User_Log(this);
                Log.Show();
            }
            else {
                button5.Text = "      登陆";

                // 恢复界面的禁用
                button3.Enabled = false;
                button4.Enabled = false;
                button10.Enabled = false;
                button8.Enabled = false;
                button15.Enabled = false;
                button16.Enabled = false;
                button17.Enabled = false;
                button11.Enabled = false;
                button12.Enabled = false;
                button13.Enabled = false;
                button14.Enabled = false;
                button22.Visible = false;
                button23.Visible = false;
                button24.Visible = false;
                // 切换页面
                button1.BackColor = Color.White;
                tabControl1.SelectedIndex = 0;
                button2.BackColor = Color.Gainsboro;
                button3.BackColor = Color.Gainsboro;
                button4.BackColor = Color.Gainsboro;
            }
        }
        private void pictureBox1_Click(object sender, EventArgs e)// 登陆用户按钮
        {
            if (button5.Text == "      登陆")
            {
                User_Log Log = new User_Log(this);
                Log.Show();
            }
            else
            {
                button5.Text = "      登陆";
                
                // 恢复界面的禁用
                button3.Enabled = false;
                button4.Enabled = false;
                button10.Enabled = false;
                button8.Enabled = false;
                button15.Enabled = false;
                button16.Enabled = false;
                button17.Enabled = false;
                button11.Enabled = false;
                button12.Enabled = false;
                button13.Enabled = false;
                button14.Enabled = false;
                button22.Visible = false;
                button23.Visible = false;
                button24.Visible = false;
                // 切换页面
                button1.BackColor = Color.White;
                tabControl1.SelectedIndex = 0;
                button2.BackColor = Color.Gainsboro;
                button3.BackColor = Color.Gainsboro;
                button4.BackColor = Color.Gainsboro;
            }
        }
        private void dataGridView2_MouseDown(object sender, MouseEventArgs e)
        {
            // 判断我是否按下了右键
            if (button5.Text == "  立即退出" && e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                Right_menu.setPint(this.PointToClient(Control.MousePosition));
                Right_menu.Show();
            } else if (e.Button == System.Windows.Forms.MouseButtons.Left) {
                Right_menu.Hide();
            }
        }

        #region 本地数据页面的操作
        private void button10_Click(object sender, EventArgs e)// 点击了查询按钮
        {
            switch (textBox6.Text){
                case "":// 如果是空的就什么都不查询
                    MessageBox.Show("请在输入框输入想要查询的数据");
                    break;
                case "%":// 查询所有数据
                        new Thread(() => {// 用线程来显示，不影响主线程的速度和进程

                        // 获取日志路径下所有的CSV表
                        string[] files2 = Directory.GetFiles(PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini") + "/" + DateTime.Now.ToString("yyyy年MM月dd日") + " /", "*.CSV");

                         // 不等于null就是找到一些文件了
                        if (files2 != null) { 
                            // 清理掉原有的表行
                            DataGridViewClass.RemoveAllRow(dataGridView2);

                            // 打开每一个CSV文件
                            foreach (string file in files2) {
                                // 读取打开的CSV文件的数据
                                List<string> list = P_Class.Read_CSV(file);
                                // 将读取出来的数据进行拆分再显示
                                for (int i = 0; i < list.Count; i++)
                                {
                                    if (i == 0)
                                    {
                                    }
                                    else
                                    { // 添加行内容
                                        DataGridViewClass.AddRows(dataGridView2, list[i].Split(','), Color.White);
                                    }
                                }
                            }
                        }

                        MessageBox.Show("搜索完成！", "操作完成提示", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);

                    }).Start();
                    break;
                default:// 按照搜索内容进行查找
                    // 清理掉原有的表行
                    DataGridViewClass.RemoveAllRow(dataGridView2);

                    // 读取路径下所有的CSV文件名
                    string FilePath = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini") + "/" + DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss") + "/";
                    try {
                        var files = Directory.GetFiles(FilePath, "*.CSV");
                        foreach (string filename in files)
                        {
                            // 找到有关键字的文件
                            if (filename.Contains(textBox6.Text))
                            {
                                FilePath = filename;
                                break;
                            }
                        }
                        // 读取CSV表里面的所有数据读取出来
                        List<string> list2 = P_Class.Read_CSV(FilePath);

                        // 丢掉表头,开始导出数据
                        int index = 0;
                        foreach (string str in list2)
                        {
                            string[] tempStr2 = str.Split(',');

                            if (index != 0)
                            {
                                DataGridViewClass.AddRows(dataGridView2, tempStr2, Color.White);
                            }
                            index++;
                        }
                    } catch (Exception ex){ }
                    
                    MessageBox.Show("数据查询完成！");
                    break;
            }
            for (int i = 0; i < dataGridView2.ColumnCount; i++)
            {
                dataGridView2.Columns[i].SortMode = DataGridViewColumnSortMode.Automatic;
            }
        }
        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)// 禁止用户在输入框内输入回车
        {
            if (e.KeyChar == 13)
            {
                e.Handled = true;
            }
        }
        private void button14_Click(object sender, EventArgs e)// 新增一个模组数据
        {
            Add_Data data = new Add_Data(this);
            data.Show();
        }
        #endregion

        public void LogFileDelete()
        {
            int tempTime = Convert.ToInt32(PublicClass.Get_ini_data("数据表主页", "设置日期的时间", System.Windows.Forms.Application.StartupPath + "/ini文件/主窗口配置参数.ini"));
            int tempNum = Convert.ToInt32(PublicClass.Get_ini_data("数据表主页", "日志清理的天数", System.Windows.Forms.Application.StartupPath + "/ini文件/主窗口配置参数.ini"));

            // 计算时间是否到了
            if (Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd")) - tempTime >= tempNum)
            {
                //FileDelete.fileDelete(日志路径.Text + "/程序日志", "*.txt");
                //FileDelete.fileDelete(异常日志路径.Text + "/异常日志", "*.txt");
                //FileDelete.fileDelete(MES日志路径.Text + "/MES日志", "*.txt");
                //FileDelete.fileDelete(CSV表路径.Text, "*.csv");

                // 默认以上一次设定点时间为基础时间
                PublicClass.Set_ini_data("数据表主页", "设置日期的时间", DateTime.Now.ToString("yyyyMMdd"), System.Windows.Forms.Application.StartupPath + "/ini文件/主窗口配置参数.ini");
            }
        }
        
        private void button15_Click(object sender, EventArgs e)//新增产品信息
        {
            Add_Product add_product = new Add_Product(this);
            add_product.Show();
        }

        private void button16_Click(object sender, EventArgs e)//编辑产品信息
        {
            if (this.dataGridView4.CurrentRow == null)
            {
                MessageBox.Show("请选择用户进行操作！");
                return;
            }

            string[] contont = DataGridViewClass.GetRowsData(dataGridView4, dataGridView4.CurrentRow.Index);
            Add_Product add_product = new Add_Product(this, contont[0], contont[1], contont[2], contont[3]);
            add_product.Show();
        }

        private void button17_Click(object sender, EventArgs e)//删除产品信息

        {
            if (this.dataGridView4.CurrentRow == null)
            {
                MessageBox.Show("请选择用户进行操作！");
                return;
            }
            if (MessageBox.Show("确认删除？", "此删除不可恢复", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                // 移除ini表里面的信息
               PublicClass.Set_ini_data("产品信息", dataGridView4.CurrentRow.Index.ToString(), "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");

                DataGridViewClass.RemoveIndexRow(dataGridView4, dataGridView4.CurrentRow.Index);


                MessageBox.Show("删除成功！");
            }
        }
        
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否关闭程序", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (dr == DialogResult.Yes)
            {
                try
                {
                    //Save20();
                    System.Diagnostics.Process a = System.Diagnostics.Process.GetProcessById(System.Diagnostics.Process.GetCurrentProcess().Id);
                    a.Kill();
                    this.Close();
                    Application.Exit();
                    Application.ExitThread();
                    System.Environment.Exit(0);

                }catch
                {
                    Dispose(true);
                }
            }
            else
            {
                e.Cancel = true;
            }
        }

        private void button104_Click(object sender, EventArgs e)
        {
            button104.BackColor = Color.White;
            button8.BackColor = Color.Gainsboro;
            button1.BackColor = Color.Gainsboro;
            button2.BackColor = Color.Gainsboro;
            button3.BackColor = Color.Gainsboro;
            button4.BackColor = Color.Gainsboro;

            //barTender.Show();
        }

        private void 采集模式_Click(object sender, EventArgs e)
        {
            if (采集模式.Text.Contains("在线"))
            {
                采集模式.Text = "离线";
                采集模式.ForeColor = Color.Red;
            }
            else {
                采集模式.Text = "在线"+ MySignl.MES_operator;
                采集模式.ForeColor = Color.Green;
            }
        }

        #region MyRegion
        //连接绝缘仪器
        public void button7_Click(object sender, EventArgs e)
        {
            PublicClass.Set_ini_data("配置参数", "串口号", textBox1.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
            PublicClass.Set_ini_data("配置参数", "端口号", textBox5.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");

            MySignl.wT6582.openSeir(textBox1.Text, textBox5.Text);
        }

        //设置绝缘值
        public void button18_Click(object sender, EventArgs e)
        {
            PublicClass.Set_ini_data("配置参数", "绝缘电压", textBox18.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
            PublicClass.Set_ini_data("配置参数", "绝缘电阻", textBox17.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
            PublicClass.Set_ini_data("配置参数", "测试时间", textBox16.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
            PublicClass.Set_ini_data("配置参数", "设定值", textBox15.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");

            byte[] SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:IR:LEVel " + textBox18.Text + " \r\n");
            MySignl.wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
            Thread.Sleep(50);
            SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:IR:LIMIT " + textBox17.Text + " \r\n");
            MySignl.wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
            Thread.Sleep(50);
            SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:IR:TIME " + textBox16.Text + " \r\n");
            MySignl.wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
        }

        //绝缘测试
        public void button9_Click(object sender, EventArgs e)
        {
            MySignl.wT6582.insulationValueTwo = 1;
            MySignl.wT6582.strRecieve3 = "";
            MySignl.wT6582.strRecieve_All = "";
            MySignl.wT6582.pressureResistanceBool = false;
            byte[] SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STARt \r\n");
            MySignl.wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
            MySignl.wT6582.insulationBool = true;
            MySignl.wT6582.t1 = DateTime.Now;
        }

        //设置耐压值
        public void button20_Click(object sender, EventArgs e)
        {
            PublicClass.Set_ini_data("配置参数", "耐压电压", textBox22.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
            PublicClass.Set_ini_data("配置参数", "耐压电流", textBox21.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
            PublicClass.Set_ini_data("配置参数", "耐压时间", textBox20.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
            PublicClass.Set_ini_data("配置参数", "延时时间", textBox19.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");

            byte[] SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:DC:LEVel " + textBox22.Text + " \r\n");
            MySignl.wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
            Thread.Sleep(50);
            SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:DC:LIMIT " + textBox21.Text + " \r\n");
            MySignl.wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
            Thread.Sleep(50);
            SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STEP1:DC:TIME " + textBox20.Text + "	\r\n");
            MySignl.wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
        }

        //耐压测试
        public void button19_Click(object sender, EventArgs e)
        {
            MySignl.wT6582.strRecieve3 = "";
            MySignl.wT6582.strRecieve_All = "";
            MySignl.wT6582.insulationBool = false;
            byte[] SendBuf = new byte[1024 * 1024];//申请内存
            SendBuf = System.Text.Encoding.Default.GetBytes(":SOURce:SAFEty:STARt \r\n");
            MySignl.wT6582.sp.Write(SendBuf, 0, SendBuf.Length);//从0到最后（长度）
            MySignl.wT6582.pressureResistanceBool = true;
        }
        #endregion

        private void button21_Click(object sender, EventArgs e)
        {
            MySignl.wT6582.insulationTimeBool = true;
            byte[] SendBuf3 = new byte[1024 * 1024];//申请内存
            //SendBuf = System.Text.Encoding.Default.GetBytes(":SOURCE:SAFETY:RESULT:STEP<1/1>:MMETERAGE? \r\n");
            SendBuf3 = System.Text.Encoding.Default.GetBytes(":SAFETY:FETCH? STEP<1/1>,MODE,TELApsed? \r\n");
            MySignl.wT6582.sp.Write(SendBuf3, 0, SendBuf3.Length);//从0到最后（长度）

        }
        //P_Class = new PublicClass(this);

        //MESClass mesClass = new MESClass(form1);
     

        private void button22_Click(object sender, EventArgs e)
        {
            PLC_ui.write("DB7365.4.0", true);
        }

        private void button24_Click(object sender, EventArgs e)
        {
            PLC_ui.write("DB7365.4.0", false);
            PLC_ui.write("DB7365.12.0", false);
        }

        private void button23_Click(object sender, EventArgs e)
        {
          
            PLC_ui.write("DB7365.12.0", true);
        }

        public bool ocvTest1;
        public async void OCV(int index)
        {
            ocvTest1 = await mBT3562.GetVoltageAndResAsync("COM1", 9600, index);
        }
        private void btntestACR_Click(object sender, EventArgs e)
        {
            OCV(0);
            Thread.Sleep(100);
            if (ocvTest1)
            {
                string A = mBT3562.voltage.ToString("F2");
                string B = (mBT3562.res * 1000M).ToString("F3");
                //测试成功                             
                txtCell1.Text = A;
                txtCell2.Text = B;
                P_Class.TxtLog("手动测试，BT3562A获取数据成功！电压--" + A + "电阻--" + B, 2);
            }
            else
            {
                txtCell1.Text = "Error";
                txtCell2.Text = "Error";
                P_Class.TxtLog("手动测试，BT3562A获取数据失败！", 1);
            }
        }

        private void btnYcellTest_Click(object sender, EventArgs e)
        {
            try
            {
                double datab = DvcyRpcClient.DvApi.Measure("");
                txtYcell.Text = datab.ToString("F2");
            }
            catch (Exception ex)
            {
                txtYcell.Text = "Error";
                P_Class.TxtLog("手动测试，Y电容获取数据失败！" + ex, 1);
            }
        }

        //读取时拿最多200条
        public void read20(string fileName,DataGridView dataGridView)
        {
            string RedLog=  fileName == "" ? 工站名 : fileName;
            
            string currPath = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
            string dataPath = currPath + "/" + DateTime.Now.ToString("yyyy年MM月dd日");  //创建当天时间文件夹
            string databag = DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss");
            string subPath = "";
            int timea = int.Parse(databag.Substring(12, 2));
            if (timea<20&&timea>=8)        // 8-20点白班(8点是白班，20点是夜班)
            {
               subPath = dataPath + "/" + "白班"+ "/" + RedLog;
            }
            else 
            {
               subPath = dataPath + "/" + "夜班"+ "/" + RedLog;
            }        
            if (System.IO.Directory.Exists(subPath) == false)//如果不存在就创建file文件夹 
            {
                Directory.CreateDirectory(subPath);
            }
            string path = subPath + "/" + RedLog + ".CSV";
            // 判断这个路径下是否存在旧的CSV表头文件      
            if (File.Exists(path))// 如果CSV文件不存在了，就需要重新创建一个CSV文件，并生成标题
            {
                //File.Create(path);

                // 读取外部CSV文件表里面的内容只加载  ，只显示最新200条
                List<string> strTemp = P_Class.Read_CSV(path);
                if (strTemp.Count != 0)
                {
                    if (strTemp.Count >= 201)
                    {
                        for (int i = strTemp.Count - 200; i < strTemp.Count; i++)
                        {
                            // 将List转换成String[]
                            DataGridViewClass.AddRows(dataGridView, strTemp[i].Split(','), Color.White);
                        }
                    }

                    else
                    {
                        for (int i = 1; i < strTemp.Count; i++)
                        {
                            // 将List转换成String[]
                            DataGridViewClass.AddRows(dataGridView, strTemp[i].Split(','), Color.White);
                        }

                    }

                }
                else
                {

                    P_Class.TxtLog("本地表格数据为空！", 2);
                }
            }

            else
            {
                P_Class.TxtLog("未找到今日本地表格数据！", 2);
            }
        }
        //Close软件时候保存前40条数据
        public void Save20()
        {
            try
            {
                string currPath = PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini");
                string dataPath = currPath + "/" + DateTime.Now.ToString("yyyy年MM月dd日");  //创建当天时间文件夹
                string subPath = dataPath + 工站名;
                if (System.IO.Directory.Exists(subPath) == false)//如果不存在就创建file文件夹 
                {
                    Directory.CreateDirectory(subPath);
                }



                //string currPath = "D:\\Microsoft11";
                //string subPath = currPath + "/" + "缓存";  //创建当天时间文件夹

                if (System.IO.Directory.Exists(subPath) == false)//如果不存在就创建file文件夹 
                {
                    Directory.CreateDirectory(subPath);
                }
                string path = subPath + "/" + 工站名 + ".CSV";

                //string currPath = "D:\\Microsoft11";
                //string subPath = currPath + "/" + "缓存";  //创建文件夹

                //if (System.IO.Directory.Exists(subPath) == false)//如果不存在就创建file文件夹 
                //{
                //    Directory.CreateDirectory(subPath);
                //}
                //// 判断这个路径下是否存在旧的CSV表头文件
                //string path = subPath + "/记录.csv";
                //DataGridViewClass.RemoveAllRow(dataGridView2);// 移除当前窗口的表头数据表的所有行
                // 判断这个路径下是否存在旧的CSV表头文件       
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                // 生成CSV文件
                StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite), Encoding.UTF8);

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

            catch (Exception ex)
            {
                MessageBox.Show("选择CSV文件路径异常：" + ex.Message);
            }
        }
        public bool pressureTest1 =true;
        public async void Pressure()
        {
            pressureTest1 = await mL100B.GetVoltageAndResAsync("COM1", 19200);
        }
        private void btnpressure_Click(object sender, EventArgs e)
        {
            Pressure();
            Thread.Sleep(100);
            if (pressureTest1)
            {
                string data = mL100B.pressure.ToString("F3");

                //测试成功     
                txtpressureA.Text= data;
             
                P_Class.TxtLog("手动测试，称重获取数据成功！重量--" + data +"Kg", 2);
            }
            else
            {
                txtpressureA.Text = "Error";         
                P_Class.TxtLog("手动测试，称重获取数据失败！", 1);
            }
        }

        private void button21_Click_1(object sender, EventArgs e)
        {
            switch (textBox23.Text)
            {
                case "":// 如果是空的就什么都不查询
                    MessageBox.Show("请在输入框输入想要查询的数据");
                    break;
                case "%":// 查询所有数据
                    new Thread(() =>
                    {// 用线程来显示，不影响主线程的速度和进程
                        try
                        {
                            string[] files2 = new string[0];
                            // 获取日志路径下所有的CSV表
                            files2 = Directory.GetFiles(PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini") + "/" + dateTimePicker3.Text + "/" + comboBox15.Text + "/" + tabControl3.SelectedTab.Text + "/", tabControl3.SelectedTab.Text + ".CSV");

                            // 不等于null就是找到一些文件了
                            if (files2 != null && files2.Length > 0)
                            {
                                // 清理掉原有的表行
                                dataGridView2.Invoke(new MethodInvoker(delegate
                                {
                                    DataGridViewClass.RemoveAllRow(dataGridView2);
                                    //DataGridViewClass.RemoveAllColumns(dataGridView2);
                                }));

                                foreach (string file in files2)
                                {
                                    // 读取打开的CSV文件的数据
                                    List<string> list = PublicClass.Read_CSV1(file);
                                    // 将读取出来的数据进行拆分再显示
                                    for (int i = 0; i < list.Count; i++)
                                    {
                                        if (i != 0)// 重新载入表头
                                        {// 添加行内容
                                            dataGridView2.Invoke(new MethodInvoker(delegate
                                            {
                                                DataGridViewClass.AddRows(dataGridView2, list[i].Split(','), Color.White);
                                            }));
                                        }
                                    }
                                }
                            }

                            MessageBox.Show("搜索完成！", "操作完成提示", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "危险错误提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        }
                    }).Start();
                    break;
                default:// 按照搜索内容进行查找
                    // 清理掉原有的表行
                    dataGridView2.Invoke(new MethodInvoker(delegate
                    {
                        DataGridViewClass.RemoveAllRow(dataGridView2);
                    }));


                    try
                    {
                        string[] files3 = new string[0];
                        files3 = Directory.GetFiles(PublicClass.Get_ini_data("配置参数", "日志路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + 工站名 + "/窗口配置参数.ini") + "/" + dateTimePicker3.Text + "/" + comboBox15.Text + "/" + tabControl3.SelectedTab.Text + "/", tabControl3.SelectedTab.Text + ".CSV");
                        foreach (string FilePath in files3)
                        {
                            // 读取CSV表里面的所有数据读取出来
                            List<string> list2 = PublicClass.Read_CSV1(FilePath);

                            // 确定我们搜索的那一列是第几列
                            int celloIndex = 0;
                            foreach (string title in DataGridViewClass.GetTitle(dataGridView2))
                            {
                                if (title == comboBox14.Text) { break; }
                                celloIndex += 1;
                            }
                            // 丢掉表头,开始搜索数据并导出能匹配上的数据
                            int index = 0;
                            foreach (string str in list2)
                            {
                                string[] tempStr2 = str.Split(',');

                                try
                                {
                                    if (index != 0 && tempStr2[celloIndex].Contains(textBox23.Text))
                                    {
                                        dataGridView2.Invoke(new MethodInvoker(delegate { DataGridViewClass.AddRows(dataGridView2, tempStr2, Color.White); }));
                                    }
                                }
                                catch (Exception ex) { }
                                index++;
                            }
                        }
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message, "危险错误提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly); break; }
                    MessageBox.Show("数据查询完成！");
                    break;


            }
        }
    }
}
