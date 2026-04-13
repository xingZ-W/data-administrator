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

namespace DataAdministrator.功能插件.用户管理页
{
    public partial class Add_User : Form
    {
        Form1 form;
        int Rows = 0;
        /// <summary> 添加新用户
        /// </summary>
        public Add_User(Form1 form, string user = "", string pasw = "", string jurisdiction = "", int Rows = 0)
        {
            InitializeComponent();
            this.form = form;

            if (user == "" && pasw == "" && jurisdiction == "")
            {
                this.Text = "新增用户";

                textBox1_Leave((object)"", EventArgs.Empty);
            }
            else {
                this.Text = "修改用户";

                textBox1.Text = user;
                textBox2.Text = pasw;
                this.Rows = Rows;
                switch (jurisdiction) {
                    case "操作员":
                        checkBox2.Checked = true;
                        break;
                    case "管理员":
                        checkBox1.Checked = true;
                        break;
                }
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
        private void textBox1_Enter(object sender, EventArgs e)
        {
            if(textBox1.Text == "输入要创建的账号"){
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if(textBox1.Text == ""){
                textBox1.Text = "输入要创建的账号";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "输入新账号的密码")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "输入新账号的密码";
                textBox2.ForeColor = Color.Gray;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "") {
                MessageBox.Show("账户或密码不能为空！");
                return;
            }
            else if (checkBox1.Checked == false && checkBox2.Checked == false) {
                MessageBox.Show("权限必须选一种！");
                return;
            }
            else if (checkBox1.Checked == true && checkBox2.Checked == true)
            {
                MessageBox.Show("权限只能选一种！");
                return;
            }

            // 区分模式
            if (this.Text == "新增用户") {
                for (int i = 0; i < 50; i++)
                {
                    string tempUser = PublicClass.Get_ini_data("账户", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/用户账号.ini");

                    if (tempUser == "")
                    {
                        string tempStr = textBox1.Text + "-" + textBox2.Text + "-" + (checkBox1.Checked == true ? checkBox1.Text : "") + (checkBox2.Checked == true ? checkBox2.Text : "") + "-" + DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss");
                        PublicClass.Set_ini_data("账户", i + "", tempStr, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/用户账号.ini");
                        MessageBox.Show("账户创建成功！");
                        // 刷新主页数据表
                        DataGridViewClass.AddRows(form.dataGridView3, PublicClass.Get_ini_data("账户", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/用户账号.ini").Split('-'), Color.White);
                        return;
                    }
                }
            } else if (this.Text == "修改用户") {
                string tempStr = textBox1.Text + "-" + textBox2.Text + "-" + (checkBox1.Checked == true ? checkBox1.Text : "") + (checkBox2.Checked == true ? checkBox2.Text : "") + "-" + DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss");
                PublicClass.Set_ini_data("账户", Rows + "", tempStr, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/用户账号.ini");
                DataGridViewClass.RevampRows(form.dataGridView3, tempStr.Split('-'), Rows);
                MessageBox.Show("账户修改成功！");
                return;
            }
            
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox2.Checked = checkBox1.Checked == true ? false : true;
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            checkBox1.Checked = checkBox2.Checked == true ? false : true;
        }
    }
}
