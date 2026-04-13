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

namespace DataAdministrator.功能插件.手动出站窗口
{
    public partial class RweldDate : Form
    {
        Form1 form;
        int Row;// 选中的本地数据表哪一行
        string Code;
        public RweldDate(Form1 form, int Row, string Code)
        {
            InitializeComponent();

            // 得到主窗口对象
            this.form = form;

            this.Row = Row;

            this.Code = Code;

            if (form.工站名 == "BSB焊接工站")
            {
                label1.Text = Code;
                tabControl1.Controls.Remove(tabControl1.TabPages[1]); 

                string 首件标识 = PublicClass.Get_ini_data("配置参数", "首件识别符", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/窗口配置参数.ini");
                if (Code.Contains(首件标识))
                {
                    // 解开，这些是首件才有的东西
                    dataGridView6.Enabled = true;
                    textBox10.Enabled = true;
                    textBox12.Enabled = true;
                    dataGridView17.Enabled = true;
                    textBox33.Enabled = true;
                    textBox37.Enabled = true;
                    dataGridView35.Enabled = true;
                    textBox44.Enabled = true;
                    textBox48.Enabled = true;
                }
                else {
                    // 禁用，这些是首件才有的东西
                    dataGridView6.Enabled = false;
                    textBox10.Enabled = false;
                    textBox12.Enabled = false;
                    dataGridView17.Enabled = false;
                    textBox33.Enabled = false;
                    textBox37.Enabled = false;
                    dataGridView35.Enabled = false;
                    textBox44.Enabled = false;
                    textBox48.Enabled = false;
                }
            }
            else if(form.工站名 == "镍片工站")
            {
                label2.Text = Code;

                tabControl1.Controls.Remove(tabControl1.TabPages[0]);
            }

            ini();
        }

        private void ini() {
            禁用data上下排序();

            开启data双缓();

            if (form.工站名 == "BSB焊接工站") {
                set_DataGridRowNum(dataGridView1, "补偿后侧距值");
                set_DataGridRowNum(dataGridView2, "补偿值差值");
                set_DataGridRowNum(dataGridView3, "焊接速度");
                set_DataGridRowNum(dataGridView4, "离焦量");
                set_DataGridRowNum(dataGridView5, "保护气流量");
                set_DataGridRowNum(dataGridView13, "Z轴焊接位实际值");
                set_DataGridRowNum(dataGridView14, "理论补偿值");
                set_DataGridRowNum(dataGridView7, "实际补偿值");
                set_DataGridRowNum(dataGridView8, "测距基础值");
                set_DataGridRowNum(dataGridView9, "测距实际值");
                set_DataGridRowNum(dataGridView10, "测距差值MAX");
                set_DataGridRowNum(dataGridView11, "测距差值MIN");
                set_DataGridRowNum(dataGridView12, "测距极差值");
                set_DataGridRowNum(dataGridView15, "测距实际平均值");
                set_DataGridRowNum(dataGridView16, "实际离焦量");
                set_DataGridRowNum(dataGridView18, "焊接能量平均值");
                set_DataGridRowNum(dataGridView19, "焊接能量最大值");
                set_DataGridRowNum(dataGridView20, "焊接能量最小值");
                set_DataGridRowNum(dataGridView6, "参数");
                set_DataGridRowNum(dataGridView17, "首件输入功率");
                set_DataGridRowNum(dataGridView35, "首件输出功率");
                set_DataGridRowNum(dataGridView6, "测距仪点检值");
            } 
            else if (form.工站名 == "镍片工站")
            {
                set_DataGridRowNum(dataGridView34, "焊接能量最大值");
                set_DataGridRowNum(dataGridView33, "焊接能量最小值");
                set_DataGridRowNum(dataGridView31, "焊接能量平均值");
                set_DataGridRowNum(dataGridView29, "离焦量");
                set_DataGridRowNum(dataGridView27, "实际离焦量");
                set_DataGridRowNum(dataGridView24, "测距实际值");
                set_DataGridRowNum(dataGridView25, "测距极差值");
                set_DataGridRowNum(dataGridView23, "测距实际平均值");
                set_DataGridRowNum(dataGridView32, "理论补偿值");
                set_DataGridRowNum(dataGridView30, "实际补偿值");
                set_DataGridRowNum(dataGridView28, "补偿值差值");
                set_DataGridRowNum(dataGridView26, "Z轴焊接位实际值");
                set_DataGridRowNum(dataGridView22, "补偿后侧距值");
                set_DataGridRowNum(dataGridView21, "测距基础值");
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

        private void 禁用data上下排序() {
            // 关闭掉数据表的上下排序
            DataGridViewClass.BanSort(dataGridView1);
            DataGridViewClass.BanSort(dataGridView2);
            DataGridViewClass.BanSort(dataGridView3);
            DataGridViewClass.BanSort(dataGridView4);
            DataGridViewClass.BanSort(dataGridView5);
            DataGridViewClass.BanSort(dataGridView7);
            DataGridViewClass.BanSort(dataGridView8);
            DataGridViewClass.BanSort(dataGridView9);
            DataGridViewClass.BanSort(dataGridView10);
            DataGridViewClass.BanSort(dataGridView11);
            DataGridViewClass.BanSort(dataGridView12);
            DataGridViewClass.BanSort(dataGridView13);
            DataGridViewClass.BanSort(dataGridView14);
            DataGridViewClass.BanSort(dataGridView15);
            DataGridViewClass.BanSort(dataGridView16);
            DataGridViewClass.BanSort(dataGridView18);
            DataGridViewClass.BanSort(dataGridView19);
            DataGridViewClass.BanSort(dataGridView20);

            DataGridViewClass.BanSort(dataGridView21);
            DataGridViewClass.BanSort(dataGridView22);
            DataGridViewClass.BanSort(dataGridView23);
            DataGridViewClass.BanSort(dataGridView24);
            DataGridViewClass.BanSort(dataGridView25);
            DataGridViewClass.BanSort(dataGridView26);
            DataGridViewClass.BanSort(dataGridView27);
            DataGridViewClass.BanSort(dataGridView28);
            DataGridViewClass.BanSort(dataGridView29);
            DataGridViewClass.BanSort(dataGridView30);
            DataGridViewClass.BanSort(dataGridView31);
            DataGridViewClass.BanSort(dataGridView32);
            DataGridViewClass.BanSort(dataGridView33);
            DataGridViewClass.BanSort(dataGridView34);
        }

        private void 开启data双缓() {

            // 开启双缓冲器
            DataGridViewClass.DataGridCreateParams(dataGridView1);
            DataGridViewClass.DataGridCreateParams(dataGridView2);
            DataGridViewClass.DataGridCreateParams(dataGridView3);
            DataGridViewClass.DataGridCreateParams(dataGridView4);
            DataGridViewClass.DataGridCreateParams(dataGridView5);
            DataGridViewClass.DataGridCreateParams(dataGridView7);
            DataGridViewClass.DataGridCreateParams(dataGridView8);
            DataGridViewClass.DataGridCreateParams(dataGridView9);
            DataGridViewClass.DataGridCreateParams(dataGridView10);
            DataGridViewClass.DataGridCreateParams(dataGridView11);
            DataGridViewClass.DataGridCreateParams(dataGridView12);
            DataGridViewClass.DataGridCreateParams(dataGridView13);
            DataGridViewClass.DataGridCreateParams(dataGridView14);
            DataGridViewClass.DataGridCreateParams(dataGridView15);
            DataGridViewClass.DataGridCreateParams(dataGridView16);
            DataGridViewClass.DataGridCreateParams(dataGridView18);
            DataGridViewClass.DataGridCreateParams(dataGridView19);
            DataGridViewClass.DataGridCreateParams(dataGridView20);
            DataGridViewClass.DataGridCreateParams(dataGridView21);
            DataGridViewClass.DataGridCreateParams(dataGridView22);
            DataGridViewClass.DataGridCreateParams(dataGridView23);
            DataGridViewClass.DataGridCreateParams(dataGridView24);
            DataGridViewClass.DataGridCreateParams(dataGridView25);
            DataGridViewClass.DataGridCreateParams(dataGridView26);
            DataGridViewClass.DataGridCreateParams(dataGridView27);
            DataGridViewClass.DataGridCreateParams(dataGridView28);
            DataGridViewClass.DataGridCreateParams(dataGridView29);
            DataGridViewClass.DataGridCreateParams(dataGridView30);
            DataGridViewClass.DataGridCreateParams(dataGridView31);
            DataGridViewClass.DataGridCreateParams(dataGridView32);
            DataGridViewClass.DataGridCreateParams(dataGridView33);
            DataGridViewClass.DataGridCreateParams(dataGridView34);

        }

        /// <summary> 设置BSB页面所有表格的行数及内容
        /// </summary>
        private void set_DataGridRowNum(DataGridView dataGird ,string dataGridTitleName) {
            // 提取数据，然后进行拆分[]提取数值
            string tempStr = form.dataGridView2.Rows[Row].Cells[DataGridViewClass.GetColumnsIndex(form.dataGridView2, dataGridTitleName)].Value.ToString();
            string[] dataGirdTitleText = Analysis(tempStr);

            // 提取设置界面的参数数据表里面的表行数据
            for (int i = 0; i < dataGirdTitleText.Length; i++) {
                DataGridViewClass.AddRows(dataGird, new string[] { dataGirdTitleText[i] }, Color.White);
            }
        }

        /// <summary> 提取[]括号里面的数据
        /// </summary>
        /// <param name="tempStr">数据表要提取的列的文本</param>
        /// <returns></returns>
        private string[] Analysis(string tempStr) {
            tempStr = tempStr.Replace("[", "");
            string[] str = tempStr.Split(']');
            return str.Take(str.Count()-1).ToArray();
        }

        #region BSB索引textBox事件
        private void textBox1_TextChanged(object sender, EventArgs e)// 补偿后侧距值
        {
            索引(textBox1, textBox2, dataGridView1);
        }
        private void textBox3_TextChanged(object sender, EventArgs e)// 补偿值差值
        {
            索引(textBox3, textBox4, dataGridView2);
        }
        private void textBox5_TextChanged(object sender, EventArgs e)// 焊接速度
        {
            索引(textBox5, textBox7, dataGridView3);
        }
        private void textBox6_TextChanged(object sender, EventArgs e)// 离焦量
        {
            索引(textBox6, textBox8, dataGridView4);
        }
        private void textBox9_TextChanged(object sender, EventArgs e)// 保护气流量
        {
            索引(textBox9, textBox11, dataGridView5);
        }
        private void textBox13_TextChanged(object sender, EventArgs e)// Z轴焊接位实际值
        {
            索引(textBox13, textBox14, dataGridView13);
        }
        private void textBox15_TextChanged(object sender, EventArgs e)// 理论补偿值
        {
            索引(textBox15, textBox16, dataGridView14);
        }
        private void textBox17_TextChanged(object sender, EventArgs e)// 实际补偿值
        {
            索引(textBox17, textBox25, dataGridView7);
        }
        private void textBox18_TextChanged(object sender, EventArgs e)// 测距基础值
        {
            索引(textBox18, textBox26, dataGridView8);
        }
        private void textBox19_TextChanged(object sender, EventArgs e)// 测距实际值
        {
            索引(textBox19, textBox27, dataGridView9);
        }
        private void textBox21_TextChanged(object sender, EventArgs e)// 测距极差MAX
        {
            索引(textBox21, textBox28, dataGridView10);
        }
        private void textBox20_TextChanged(object sender, EventArgs e)// 测距极差MIN
        {
            索引(textBox20, textBox29, dataGridView11);
        }
        private void textBox22_TextChanged(object sender, EventArgs e)// 测距极差值
        {
            索引(textBox22, textBox30, dataGridView12);
        }
        private void textBox23_TextChanged(object sender, EventArgs e)// 测距实际平均值
        {
            索引(textBox23, textBox31, dataGridView15);
        }
        private void textBox24_TextChanged(object sender, EventArgs e)// 实际离焦量
        {
            索引(textBox24, textBox32, dataGridView16);
        }
        private void textBox34_TextChanged(object sender, EventArgs e)// 焊接能量平均值
        {
            索引(textBox34, textBox38, dataGridView18);
        }
        private void textBox35_TextChanged(object sender, EventArgs e)// 焊接能量最大值
        {
            索引(textBox35, textBox39, dataGridView19);
        }
        private void textBox36_TextChanged(object sender, EventArgs e)// 焊接能量最小值
        {
            索引(textBox36, textBox40, dataGridView20);
        }
        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            索引(textBox10, textBox12, dataGridView6);
        }
        private void textBox33_TextChanged(object sender, EventArgs e)
        {
            索引(textBox33, textBox37, dataGridView17);
        }
        private void textBox44_TextChanged(object sender, EventArgs e)
        {
            索引(textBox44, textBox48, dataGridView35);
        }
        #endregion

        #region BSB数值textBox事件
        private void textBox2_TextChanged(object sender, EventArgs e)// 补偿后侧距值
        {
            dataGridView1.Rows[Convert.ToInt32(textBox1.Text)].Cells[0].Value = textBox2.Text;
        }
        private void textBox4_TextChanged(object sender, EventArgs e)// 补偿值差值
        {
            dataGridView2.Rows[Convert.ToInt32(textBox3.Text)].Cells[0].Value = textBox4.Text;
        }
        private void textBox7_TextChanged(object sender, EventArgs e)// 焊接速度
        {
            dataGridView3.Rows[Convert.ToInt32(textBox5.Text)].Cells[0].Value = textBox7.Text;
        }
        private void textBox8_TextChanged(object sender, EventArgs e)// 离焦量
        {
            dataGridView4.Rows[Convert.ToInt32(textBox6.Text)].Cells[0].Value = textBox8.Text;
        }
        private void textBox11_TextChanged(object sender, EventArgs e)// 保护气流量
        {
            dataGridView5.Rows[Convert.ToInt32(textBox9.Text)].Cells[0].Value = textBox11.Text;
        }
        private void textBox14_TextChanged(object sender, EventArgs e)// Z轴焊接位实际值
        {
            dataGridView13.Rows[Convert.ToInt32(textBox13.Text)].Cells[0].Value = textBox14.Text;
        }
        private void textBox16_TextChanged(object sender, EventArgs e)// 理论补偿值
        {
            dataGridView14.Rows[Convert.ToInt32(textBox15.Text)].Cells[0].Value = textBox16.Text;
        }
        private void textBox25_TextChanged(object sender, EventArgs e)// 实际补偿值
        {
            dataGridView7.Rows[Convert.ToInt32(textBox17.Text)].Cells[0].Value = textBox25.Text;
        }
        private void textBox26_TextChanged(object sender, EventArgs e)// 测距基础值
        {
            dataGridView8.Rows[Convert.ToInt32(textBox18.Text)].Cells[0].Value = textBox26.Text;
        }
        private void textBox27_TextChanged(object sender, EventArgs e)// 测距实际值
        {
            dataGridView9.Rows[Convert.ToInt32(textBox19.Text)].Cells[0].Value = textBox27.Text;
        }
        private void textBox28_TextChanged(object sender, EventArgs e)// 测距极差MAX
        {
            dataGridView10.Rows[Convert.ToInt32(textBox21.Text)].Cells[0].Value = textBox28.Text;
        }
        private void textBox29_TextChanged(object sender, EventArgs e)// 测距极差MIN
        {
            dataGridView11.Rows[Convert.ToInt32(textBox20.Text)].Cells[0].Value = textBox29.Text;
        }
        private void textBox30_TextChanged(object sender, EventArgs e)// 测距极差值
        {
            dataGridView12.Rows[Convert.ToInt32(textBox22.Text)].Cells[0].Value = textBox30.Text;
        }
        private void textBox31_TextChanged(object sender, EventArgs e)// 测距实际平均值
        {
            dataGridView15.Rows[Convert.ToInt32(textBox23.Text)].Cells[0].Value = textBox31.Text;
        }
        private void textBox32_TextChanged(object sender, EventArgs e)// 实际离焦量
        {
            dataGridView16.Rows[Convert.ToInt32(textBox24.Text)].Cells[0].Value = textBox32.Text;
        }
        private void textBox38_TextChanged(object sender, EventArgs e)// 焊接能量平均值
        {
            dataGridView18.Rows[Convert.ToInt32(textBox34.Text)].Cells[0].Value = textBox38.Text;
        }
        private void textBox39_TextChanged(object sender, EventArgs e)// 焊接能量最大值
        {
            dataGridView19.Rows[Convert.ToInt32(textBox35.Text)].Cells[0].Value = textBox39.Text;
        }
        private void textBox40_TextChanged(object sender, EventArgs e)// 焊接能量最小值
        {
            dataGridView20.Rows[Convert.ToInt32(textBox36.Text)].Cells[0].Value = textBox40.Text;
        }
        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            dataGridView6.Rows[Convert.ToInt32(textBox10.Text)].Cells[0].Value = textBox12.Text;
        }
        private void textBox37_TextChanged(object sender, EventArgs e)
        {
            dataGridView17.Rows[Convert.ToInt32(textBox33.Text)].Cells[0].Value = textBox37.Text;
        }
        private void textBox48_TextChanged(object sender, EventArgs e)
        {
            dataGridView35.Rows[Convert.ToInt32(textBox44.Text)].Cells[0].Value = textBox48.Text;
        }
        #endregion

        #region 镍片索引textBox事件
        private void textBox80_TextChanged(object sender, EventArgs e)
        {
            索引(textBox80, textBox61, dataGridView34);
        }
        private void textBox79_TextChanged(object sender, EventArgs e)
        {
            索引(textBox79, textBox57, dataGridView33);
        }
        private void textBox77_TextChanged(object sender, EventArgs e)
        {
            索引(textBox59, textBox77, dataGridView31);
        }
        private void textBox70_TextChanged(object sender, EventArgs e)
        {
            索引(textBox70, textBox52, dataGridView29);
        }
        private void textBox72_TextChanged(object sender, EventArgs e)
        {
            索引(textBox72, textBox50, dataGridView27);
        }
        private void textBox67_TextChanged(object sender, EventArgs e)
        {
            索引(textBox67, textBox47, dataGridView24);
        }
        private void textBox65_TextChanged(object sender, EventArgs e)
        {
            索引(textBox65, textBox45, dataGridView25);
        }
        private void textBox62_TextChanged(object sender, EventArgs e)
        {
            索引(textBox62, textBox43, dataGridView23);
        }
        private void textBox78_TextChanged(object sender, EventArgs e)
        {
            索引(textBox78, textBox59, dataGridView32);
        }
        private void textBox75_TextChanged(object sender, EventArgs e)
        {
            索引(textBox75, textBox56, dataGridView30);
        }
        private void textBox73_TextChanged(object sender, EventArgs e)
        {
            索引(textBox73, textBox53, dataGridView28);
        }
        private void textBox68_TextChanged(object sender, EventArgs e)
        {
            索引(textBox68, textBox49, dataGridView26);
        }
        private void textBox69_TextChanged(object sender, EventArgs e)
        {
            索引(textBox69, textBox46, dataGridView22);
        }
        private void textBox41_TextChanged(object sender, EventArgs e)
        {
            索引(textBox41, textBox42, dataGridView21);
        }
        #endregion

        #region 镍片数值textBox事件
        private void textBox61_TextChanged(object sender, EventArgs e)
        {
            dataGridView34.Rows[Convert.ToInt32(textBox80.Text)].Cells[0].Value = textBox61.Text;
        }
        private void textBox54_TextChanged(object sender, EventArgs e)
        {
            dataGridView31.Rows[Convert.ToInt32(textBox77.Text)].Cells[0].Value = textBox59.Text;
        }
        private void textBox57_TextChanged(object sender, EventArgs e)
        {
            dataGridView33.Rows[Convert.ToInt32(textBox79.Text)].Cells[0].Value = textBox57.Text;
        }
        private void textBox52_TextChanged(object sender, EventArgs e)
        {
            dataGridView29.Rows[Convert.ToInt32(textBox70.Text)].Cells[0].Value = textBox52.Text;
        }
        private void textBox50_TextChanged(object sender, EventArgs e)
        {
            dataGridView27.Rows[Convert.ToInt32(textBox72.Text)].Cells[0].Value = textBox50.Text;
        }
        private void textBox47_TextChanged(object sender, EventArgs e)
        {
            dataGridView24.Rows[Convert.ToInt32(textBox67.Text)].Cells[0].Value = textBox47.Text;
        }
        private void textBox45_TextChanged(object sender, EventArgs e)
        {
            dataGridView25.Rows[Convert.ToInt32(textBox65.Text)].Cells[0].Value = textBox45.Text;
        }
        private void textBox43_TextChanged(object sender, EventArgs e)
        {
            dataGridView23.Rows[Convert.ToInt32(textBox62.Text)].Cells[0].Value = textBox43.Text;
        }
        private void textBox59_TextChanged(object sender, EventArgs e)
        {
            dataGridView32.Rows[Convert.ToInt32(textBox78.Text)].Cells[0].Value = textBox59.Text;
        }
        private void textBox56_TextChanged(object sender, EventArgs e)
        {
            dataGridView30.Rows[Convert.ToInt32(textBox75.Text)].Cells[0].Value = textBox56.Text;
        }
        private void textBox53_TextChanged(object sender, EventArgs e)
        {
            dataGridView28.Rows[Convert.ToInt32(textBox73.Text)].Cells[0].Value = textBox53.Text;
        }
        private void textBox49_TextChanged(object sender, EventArgs e)
        {
            dataGridView26.Rows[Convert.ToInt32(textBox68.Text)].Cells[0].Value = textBox49.Text;
        }
        private void textBox46_TextChanged(object sender, EventArgs e)
        {
            dataGridView22.Rows[Convert.ToInt32(textBox69.Text)].Cells[0].Value = textBox46.Text;
        }
        private void textBox42_TextChanged(object sender, EventArgs e)
        {
            dataGridView21.Rows[Convert.ToInt32(textBox41.Text)].Cells[0].Value = textBox42.Text;
        }
        #endregion

        private void 索引(TextBox label1, TextBox label2, DataGridView dataGriid) {
            int a;
            if (label1.Text == "")
            {
                label2.Text = "";
            } else
            if (int.TryParse(label1.Text, out a))
            {// 判断输入的是不是数字
                if (Convert.ToInt32(label1.Text) >= dataGriid.Rows.Count) { return; }// 判断输入的值有没有超过索引
                label2.Text = dataGriid.Rows[Convert.ToInt32(label1.Text)].Cells[0].Value.ToString();
            }
        }

        public string[] FindIniPath(string title, string str, int Colle)
        {
            for (int i = 0; i < 200; i++)
            {
                string[] _PathName = PublicClass.Get_ini_data(title, i + "", System.Windows.Forms.Application.StartupPath + "/ini文件/工站的配置文件/" + form.工站名 + "/MES配置文件.ini").Split('-');

                if (_PathName[0] == "") { break; }

                if (_PathName[Colle] == str) { 
                    return _PathName;
                }
            }
            return null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
