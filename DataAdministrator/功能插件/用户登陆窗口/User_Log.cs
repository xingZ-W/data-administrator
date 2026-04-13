using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAdministraction.公用函数类;

namespace DataAdministrator.功能插件.用户登陆窗口
{
    public partial class User_Log : Form
    {
        Form1 form;
        bool user_Bol;

        public User_Log(Form1 form)
        {
            InitializeComponent();

            this.form = form;

            textBox1_Leave((object)"", EventArgs.Empty);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(textBox1.Text == "" || textBox2.Text == ""){
                MessageBox.Show("账户和密码输入框内不能为空！");
                return;
            }
            user_Bol = false;
            for (int i = 0; i < 50; i++)
            {
                string[] UserPas = PublicClass.Get_ini_data("账户", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/用户账号.ini").Split('~');

                if (UserPas != null && UserPas[0] != "" && (UserPas[0] == textBox1.Text && UserPas[1] == textBox2.Text))
                {
                    form.button5.Text = "  立即退出";
                    form.pictureBox1.BackgroundImage = Image.FromFile(System.Windows.Forms.Application.StartupPath + "/Image_ICon/退出.png");

                    // 恢复界面的使用
                    switch (UserPas[2])
                    {
                        case "操作员":
                            form.button3.Enabled = true;
                            form.button4.Enabled = true;
                            form.button10.Enabled = true;
                            form.button22.Visible = false;
                            form.button23.Visible = false;
                            form.button24.Visible = false;
                            DataGridViewClass.RemoveAllRow(form.dataGridView3, "密码");
                            user_Bol = true;
                            break;
                        case "管理员":
                            user_Bol = true;
                            form.button3.Enabled = true;
                            form.button4.Enabled = true;
                            form.button10.Enabled = true;

                            form.button8.Enabled = true;
                            form.button15.Enabled = true;
                            form.button16.Enabled = true;
                            form.button17.Enabled = true;
                            form.button11.Enabled = true;
                            form.button12.Enabled = true;
                            form.button13.Enabled = true;
                            form.button14.Enabled = true;
                            form.button22.Visible = true;
                            form.button23.Visible = true;
                            form.button24.Visible = true;
                            // 删除所有数据表的列
                            DataGridViewClass.RemoveAllColumns(form.dataGridView3);
                            // 给 用户管理   添加表头
                            DataGridViewClass.AddColumns(form.dataGridView3, PublicClass.Read_Txt(System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/表头.txt", 2).Split('~'));
                            // 将表头铺满整个表的宽度
                            DataGridViewClass.SetTitleWidth(form.dataGridView3);
                            // 关闭掉数据表的上下排序
                            DataGridViewClass.BanSort(form.dataGridView3);
                            // 给用户管理页面的数据表添加内容
                            for (int k = 0; k < 50; k++)
                            {
                                string tempUser = PublicClass.Get_ini_data("账户", k + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/用户账号.ini");

                                if (tempUser != null && tempUser != "")
                                {
                                    DataGridViewClass.AddRows(form.dataGridView3, tempUser.Split('~'), Color.White);
                                }
                            }
                            break;
                    }
                    this.Dispose();
                }
            }
            if (!user_Bol)
            {
                MessageBox.Show("账号或密码错误,请重试");
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "输入账号")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "输入账号";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "输入密码")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "输入密码";
                textBox2.ForeColor = Color.Gray;
            }
        }
    }
}
