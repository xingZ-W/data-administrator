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

namespace DataAdministrator.功能插件.产品信息页
{
    public partial class Add_Product : Form
    {
        Form1 form;
        int Rows = 0;
        public Add_Product(Form1 form, string a = "", string b = "", string c = "", string d = "", int Rows = 0)
        {
            InitializeComponent();

            this.form = form;

            if (a == "" && b == "" && c == "" && c == "")
            {
                this.Text = "新增产品";

                textBox1_Leave((object)"", EventArgs.Empty);
                textBox2_Leave((object)"", EventArgs.Empty);
                textBox3_Leave((object)"", EventArgs.Empty);
                textBox4_Leave((object)"", EventArgs.Empty);
            }
            else
            {
                this.Text = "修改产品";

                textBox1.Text = a;
                textBox2.Text = b;
                textBox3.Text = c;
                textBox4.Text = d;
                this.Rows = Rows;
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

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "输入要创建的模组PN号";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "输入要创建的模组PN号")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text == "")
            {
                textBox2.Text = "输入要创建的模组型号";
                textBox2.ForeColor = Color.Gray;
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text == "输入要创建的模组型号")
            {
                textBox2.Text = "";
                textBox2.ForeColor = Color.Black;
            }
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text == "")
            {
                textBox3.Text = "输入要创建的串";
                textBox3.ForeColor = Color.Gray;
            }
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            if (textBox3.Text == "输入要创建的串")
            {
                textBox3.Text = "";
                textBox3.ForeColor = Color.Black;
            }
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (textBox4.Text == "")
            {
                textBox4.Text = "输入要创建的并";
                textBox4.ForeColor = Color.Gray;
            }
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            if (textBox4.Text == "输入要创建的并")
            {
                textBox4.Text = "";
                textBox4.ForeColor = Color.Black;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || textBox4.Text == "")
            {
                MessageBox.Show("填写的信息不能为空！");
                return;
            }
            if (this.Text == "新增产品")
            {
                for (int i = 0; i < 100; i++)
                {
                    string tempUser = PublicClass.Get_ini_data("产品信息", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
                    if (tempUser == "")
                    {
                        string tempStr = textBox1.Text + "-" + textBox2.Text + "-" + textBox3.Text + "-" + textBox4.Text + "-"  + DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss");
                        PublicClass.Set_ini_data("产品信息", i + "", tempStr, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
                        MessageBox.Show("产品创建成功！");
                        // 刷新主页数据表
                        DataGridViewClass.AddRows(form.dataGridView4, PublicClass.Get_ini_data("产品信息", i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini").Split('-'), Color.White);
                        this.Close();
                        return;
                    }
                }
            }
            else if (this.Text == "修改产品")
            {
                string tempStr = textBox1.Text + "-" + textBox2.Text + "-" + textBox3.Text + "-" + textBox4.Text + "-"  + DateTime.Now.ToString("yyyy年MM月dd日 hh:mm:ss");
                PublicClass.Set_ini_data("产品信息", Rows + "", tempStr, System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
                DataGridViewClass.RevampRows(form.dataGridView4, tempStr.Split('-'), Rows);
                MessageBox.Show("产品修改成功！");

                return;
            }
        }
    }
}