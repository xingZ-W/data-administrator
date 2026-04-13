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

namespace DataAdministrator.功能插件.停机弹窗
{
    public partial class StopWindows : Form
    {
        Form1 form;
        string DEVnumber = "";
        string beginTime = "";
        string endTime = "";
        string name;
        public StopWindows(Form1 form, string name, string DEVnumber, string beginTime = "", string endTime = "")
        {
            InitializeComponent();
            this.Text = "上传停机记录窗口(" + name + ")";
            this.name = name;
            this.form = form;
            this.DEVnumber = DEVnumber;
            this.beginTime = beginTime;//开始停机时间
            this.endTime = endTime;//结束停机时间
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (form.采集模式.Text != "离线")
            {
                string[] 停机原因 = comboBox4.Text.Split('：');
                string retJson = form.MySignl.mesClass.设备停机采集(DEVnumber, 停机原因[0], beginTime, endTime, form.MySignl.getData2Value(name + "操作人"));
                MessageBox.Show("MES返回结果：\r\n" + retJson);
                this.Dispose();
            }
            else
            {
                MessageBox.Show("上传异常！\r\n原因：\r\n1.程序的采集状态不能是离线，离线是屏蔽MES通讯的状态。\r\n2.程序没有登录MES账户。", "NG返修", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}
