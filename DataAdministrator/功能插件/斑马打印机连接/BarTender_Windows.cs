using DataAdministraction.公用函数类;
using DataAdministrator.自动流程;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataAdministrator.功能插件.斑马打印机连接
{
    public partial class BarTender_Windows : Form
    {
        Form1 form;
        BarTender_Connect BarTender;
        public string style = "";// 此变量用于判断当前窗口是否是自动打开的
        public BarTender_Windows(Form1 form)
        {
            InitializeComponent();

            this.form = form;
            string BTW_Path = "";
            this.ControlBox = false;// 取消窗口右上角退出按钮

            textBox1.Text = PublicClass.Get_ini_data("打印机数据设置", "二维码前缀固定值", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/斑马打印机数据设置.ini");
            textBox2.Text = PublicClass.Get_ini_data("打印机数据设置", "年份代码", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/斑马打印机数据设置.ini");
            label3.Text = PublicClass.Get_ini_data("打印机数据设置", "产品序号", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/斑马打印机数据设置.ini");
            label3.Text = label3.Text == "" ? "0000000" : label3.Text;
            BTW_Path = PublicClass.Get_ini_data("斑马打印机配置", "BTW标签文件路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
            this.textBox17.Text = BTW_Path;

            BarTender = new BarTender_Connect(form, BTW_Path, BarTender_DataGridView);

            string msg = BarTender.connect();
            if (msg != "")
            {
                BarTender.Close();
                BarTender = null;
                button1.Enabled = false;

                //form.label6.BackColor = Color.Red;
                //form.label6.Text = "斑马打印机通讯异常";
                //form.label7.BackColor = Color.Red;
                //form.label7.Text = "斑马打印机通讯异常";
                //form.label8.BackColor = Color.Red;
                //form.label8.Text = "斑马打印机通讯异常";
                //form.label9.BackColor = Color.Red;
                //form.label9.Text = "斑马打印机通讯异常";

                MessageBox.Show(msg, "打印机连接异常", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                return;
            }

            button1.Enabled = true;  
        }
        
        private void BarTender_Windows_Load(object sender, EventArgs e)
        {
            new System.Threading.Thread(() => {
                if (style != "") {
                    System.Threading.Thread.Sleep(3000);
                    button1_Click((object)"", EventArgs.Empty);
                }
            }).Start();
        }
        private void button11_Click(object sender, EventArgs e)
        {
            try
            {
                openLabelFileDialog.Filter = "模组标签文件(*.btw)|*.btw|所有文件(*.*)|*.*";
                DialogResult dialogResult = openLabelFileDialog.ShowDialog();
                if (dialogResult == DialogResult.OK)
                {
                    this.textBox17.Text = this.openLabelFileDialog.FileName;

                    if (BarTender != null)
                    {
                        BarTender.Close();
                        BarTender = null;
                    }

                    button1.Enabled = true;
                    PublicClass.Set_ini_data("斑马打印机配置", "BTW标签文件路径", this.textBox17.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
                    string BTW_Path = PublicClass.Get_ini_data("斑马打印机配置", "BTW标签文件路径", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
                    BarTender = new BarTender_Connect(form, BTW_Path, BarTender_DataGridView);
                    string msg = BarTender.connect();
                    if (msg != "")
                    {
                        button1.Enabled = false;
                        MessageBox.Show(msg, "打印机连接异常", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("选择模板标签异常：" + ex.Message, "危险错误提示", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (DataGridViewClass.GetTitle(BarTender_DataGridView).Length > 1 && BarTender_DataGridView.Rows.Count > 0)
            {
                BarTender_DataGridView.Rows[0].Cells[1].Value = 组合();
                button3_Click((object)"", EventArgs.Empty);
            }

            BarTender.BarTender_Print();
            style = "";
            this.Hide();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            PublicClass.Set_ini_data("打印机数据设置", "二维码前缀固定值", this.textBox1.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/斑马打印机数据设置.ini");
            
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            PublicClass.Set_ini_data("打印机数据设置", "年份代码", this.textBox2.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/斑马打印机数据设置.ini");
        }

        private void label3_TextChanged(object sender, EventArgs e)
        {
            PublicClass.Set_ini_data("打印机数据设置", "产品序号", this.label3.Text, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/斑马打印机数据设置.ini");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label3.Text = "0000";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Convert.ToInt32(label3.Text) <= 1) {
                label3.Text = "0000";
                return;
            }
            
            string num = (Convert.ToInt32(label3.Text) - 1).ToString();
            int forNum = 4 - num.Length;
            for (int i = 0; i < forNum; i++)
            {
                num = "0" + num;
            }
            label3.Text = num;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string num = (Convert.ToInt32(label3.Text) + 1).ToString();
            int forNum = 4 - num.Length;
            for(int i = 0; i < forNum; i++){
                num = "0" + num;
            }
            label3.Text = num;
        }

        private string 组合() {
            string 前缀固定值 = PublicClass.Get_ini_data("打印机数据设置", "二维码前缀固定值", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/斑马打印机数据设置.ini");
            string 年份代码 = PublicClass.Get_ini_data("打印机数据设置", "年份代码", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/斑马打印机数据设置.ini");
            string 产品序号 = PublicClass.Get_ini_data("打印机数据设置", "产品序号", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/斑马打印机数据设置.ini");
            return 前缀固定值 + 年份代码 + 产品序号;
        }

    }
}
