using DataAdministraction.公用函数类;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAdministrator.功能插件.手动出站窗口;

namespace DataAdministrator.功能插件.右击菜单
{
    public partial class Right_Menu : Form
    {
        Form1 form;

        public Right_Menu(Form1 form)
        {
            InitializeComponent();

            this.form = form;
        }
        public void setPint(Point pint) {
            this.Location = pint;
        }

        private void label1_MouseEnter(object sender, EventArgs e)
        {
            label1.BackColor = Color.White;

            label2.BackColor = Color.Gainsboro;
            label3.BackColor = Color.Gainsboro;
            label4.BackColor = Color.Gainsboro;
        }

        private void label2_MouseEnter(object sender, EventArgs e)
        {
            label2.BackColor = Color.White;

            label1.BackColor = Color.Gainsboro;
            label3.BackColor = Color.Gainsboro;
            label4.BackColor = Color.Gainsboro;
        }

        private void label3_MouseEnter(object sender, EventArgs e)
        {
            label3.BackColor = Color.White;

            label1.BackColor = Color.Gainsboro;
            label2.BackColor = Color.Gainsboro;
            label4.BackColor = Color.Gainsboro;
        }

        private void label4_MouseEnter(object sender, EventArgs e)
        {
            label4.BackColor = Color.White;

            label1.BackColor = Color.Gainsboro;
            label3.BackColor = Color.Gainsboro;
            label2.BackColor = Color.Gainsboro;
        }

        private void label1_Click(object sender, EventArgs e)
        {

            this.Hide();
        }

        private void label2_Click(object sender, EventArgs e)
        {


            this.Hide();
        }
    }
}
