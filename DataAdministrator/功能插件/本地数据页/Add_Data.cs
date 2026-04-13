using DataAdministraction.公用函数类;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataAdministrator.功能插件.本地数据页
{
    public partial class Add_Data : Form
    {
        Form1 form;
        
        public Add_Data(Form1 form)
        {
            InitializeComponent();

            this.form = form;

            switch (form.工站名) {
                case "BSB焊接工站":
                    label4.Text = "工位号";
                    label3.Text = "夹具号";
                    break;
                default:
                    break;
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text == "输入模组号")
            {
                textBox1.Text = "";
                textBox1.ForeColor = Color.Black;
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                textBox1.Text = "输入模组号";
                textBox1.ForeColor = Color.Gray;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (form.工站名 != "极柱拍照工站")
            {
                if (textBox1.Text == "" || textBox2.Text == "" || textBox3.Text == "" || (checkBox1.Checked && checkBox2.Checked || !checkBox1.Checked && !checkBox2.Checked))
                {
                    MessageBox.Show("请补全所有数据");
                    return;
                }
                switch (button1.Text)
                {
                    case "进站":
                        break;
                    case "出站":
                        break;
                }
            }
            else
            {
                if (textBox1.Text == "" || (checkBox1.Checked && checkBox2.Checked || !checkBox1.Checked && !checkBox2.Checked))
                {
                    MessageBox.Show("请补全所有数据");
                    return;
                }
            }
            
            
        }
        
        private void Add_Data_Load(object sender, EventArgs e)
        {
            if (form.工站名!="极柱拍照工站")
            {
                this.textBox2.Enabled = true;
                this.textBox3.Enabled = true;
            }
            else
            {
                this.textBox2.Enabled = false;
                this.textBox3.Enabled = false;
            }
        }
    }
}
