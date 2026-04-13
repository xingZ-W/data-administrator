using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

// 这里写的都是一些针对dataGridView控件的工具类

namespace DataAdministraction.公用函数类
{
    /// <summary> 这里写的都是一些针对dataGridView控件的公用函数
    /// </summary>
    static class DataGridViewClass
    {
        #region 增操作
        /// <summary> 增加列
        /// 表对象 - 内容
        /// </summary>
        public static void AddColumns(DataGridView dataGridView, string content) {
            DataGridViewTextBoxColumn acCode = new DataGridViewTextBoxColumn();
            acCode.Name = content;
            acCode.DataPropertyName = content;
            acCode.HeaderText = content;
            dataGridView.Columns.Add(acCode);
        }
        /// <summary> 增加列
        /// 表对象 - 内容(数组)
        /// </summary>
        public static void AddColumns(DataGridView dataGridView, string[] content)
        {
            for (int i = 0; i < content.Length; i++ )
            {
                DataGridViewTextBoxColumn acCode = new DataGridViewTextBoxColumn();
                acCode.Name = content[i];
                acCode.DataPropertyName = content[i];
                acCode.HeaderText = content[i];
                dataGridView.Columns.Add(acCode);
            }
        }

        /// <summary> 添加行
        /// 表对象 - 内容(数组)
        /// </summary>
        public static void AddRows(DataGridView dataGridView, string[] content, Color BackColor, DataGridView TargetDataGridView = null)
        {
            // 判断需要添加行的表控件是不是dataGridView1
            if (dataGridView.Name == "dataGridView1" && dataGridView.Rows.Count >= 14) { // 如果满足条件就移除第0,1行
                RemoveIndexRow(dataGridView, 0);
                RemoveIndexRow(dataGridView, 1);
            }

            // 添加一空行
            try {
                dataGridView.Rows.Add("");
            }
            catch (System.InvalidOperationException) {// 当使用的目标表的列为空的时候就会触发这里的异常，作用是将第三个参数的Data表的表头添加进来
                if (TargetDataGridView != null)
                {
                    for (int i = 0; i < TargetDataGridView.Columns.Count; i++)
                    {
                        AddColumns(dataGridView, TargetDataGridView.Columns[i].HeaderText);
                    }

                    dataGridView.Rows.Add("");
                }
            }

            try {
                // 寻找无数据的行
                for (int k = 0; k < dataGridView.Rows.Count; k++)
                {
                    // 判断当前行是否为空可以拿来使用
                    if (dataGridView.Rows[k].Cells[0].Value == null || dataGridView.Rows[k].Cells[0].Value == "")
                    {
                        // 将传来的数组数据导入到表里面
                        for (int i = 0; i < dataGridView.Columns.Count; i++)
                        {
                            string name = dataGridView.Columns[i].GetType().Name;
                            if (dataGridView.Columns[i].GetType().Name != "DataGridViewCheckBoxColumn")
                            {
                                dataGridView.Rows[k].Cells[i].Value = content[i];
                            } else
                            {
                                dataGridView.Rows[k].Cells[i].Value = content[i] == null || content[i] == "" ? "False" : content[i];
                            }
                            
                        }
                        dataGridView.Rows[k].DefaultCellStyle.BackColor = BackColor;
                        return;// 数据全部写完了后就可以不需要进行最外层的循环，直接跳出即可
                    }
                }
            }
            catch (System.IndexOutOfRangeException ex) { }
            
            dataGridView.CurrentCell = dataGridView.Rows[dataGridView.Rows.Count - 1].Cells[0];
        }
        #endregion

        #region 删操作
        /// <summary> 移除所有列
        /// 需要传入表对象
        /// </summary>
        public static void RemoveAllColumns(DataGridView dataGridView) {
            while (dataGridView.Columns.Count > 0)
            {
                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    dataGridView.Columns.RemoveAt(i);
                }
            }
        }

        /// <summary> 移除所有行
        /// 需要传入表对象
        /// </summary>
        public static void RemoveAllRow(DataGridView dataGridView)
        {
            try {
                while (dataGridView.Rows.Count > 0)
                {
                    for (int i = 0; i < dataGridView.Rows.Count; i++)
                    {
                        DataGridViewRow row2 = dataGridView.Rows[i];
                        dataGridView.Rows.Remove(row2);// 移除选中的行
                    }
                }
            } catch (System.InvalidOperationException ex) { MessageBox.Show("操作出了问题，请重试！"); }
            
        }

        /// <summary> 移除指定的烈的表头
        /// 需要传入表对象和指定列里的内容
        /// </summary>
        public static void RemoveAllRow(DataGridView dataGridView, String TitleName)
        {
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                if (dataGridView.Columns[i].HeaderText == TitleName) {
                    dataGridView.Columns.RemoveAt(i);
                    return;
                }
            }
        }

        /// <summary> 根据传进来的索引位置进行删除指定的行
        /// </summary>
        public static void RemoveIndexRow(DataGridView dataGirdView, int index) {
            dataGirdView.Rows.RemoveAt(index);
        }
        #endregion

        #region 查操作
        /// <summary> 获取到数据表里面某一行里面的数据内容
        /// </summary>
        /// <returns>string[] 字符串数组</returns>
        public static string[] GetRowsData(DataGridView dataGridView,int index)
        {
            string[] temp = new string[dataGridView.Columns.Count];// 创建一个字符串素组

            try {
                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    temp[i] = (dataGridView.Rows[index].Cells[i].Value == null ? "" : dataGridView.Rows[index].Cells[i].Value.ToString());
                }
                return temp;
            }
            catch (System.ArgumentOutOfRangeException ex)
            {// 索引超出范围
                MessageBox.Show("当前搜索的行在列表中并不存在！");
            }
            
            return new string[]{"",""};
        }
        public static string[] GetRowsData(DataGridView dataGridView)
        {
            string[] temp = new string[dataGridView.Columns.Count];// 创建一个字符串素组

            try
            {
                for (int i = 0; i < dataGridView.Columns.Count; i++)
                {
                    temp[i] = (dataGridView.Rows[0].Cells[i].Value == null ? "" : dataGridView.Rows[0].Cells[i].Value.ToString());
                }
                return temp;
            }
            catch (System.ArgumentOutOfRangeException ex)
            {// 索引超出范围
                MessageBox.Show("当前搜索的行在列表中并不存在！");
            }

            return new string[] { "", "" };
        }
        /// <summary> 获取某行的索引位置
        /// 数据表对象 - 查询的数据内容
        /// </summary>
        /// <returns>返回int值</returns>
        public static int GetRowsIndex(DataGridView dataGridView, string content)
        {
            for (int i = 0; i < dataGridView.Rows.Count; i++)
            {
                for (int k = 0; k < dataGridView.Columns.Count; k++)
                {
                    if (dataGridView.Rows[i].Cells[k].Value != null && dataGridView.Rows[i].Cells[k].Value.ToString() == content)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary> 获取某列的索引位置
        /// </summary>
        /// <returns>返回int值</returns>
        public static int GetColumnsIndex(DataGridView dataGridView, string content)
        {
            for (int k = 0; k < dataGridView.Columns.Count; k++)// 循环列
            {
                if (dataGridView.Columns[k].HeaderText.Contains(content))
                {
                    return k;// 返回列索引
                }
            }
            return 0;
        }

        /// <summary> 获取指定的表的所有表标题
        /// </summary>
        /// <param name="dataGridView">指定的表对象</param>
        /// <returns>字符串数组</returns>
        public static string[] GetTitle(DataGridView dataGridView) {
            string[] tempStr = new string[dataGridView.Columns.Count];
            for (int i = 0; i < dataGridView.Columns.Count; i++ )
            {
                tempStr[i] = dataGridView.Columns[i].HeaderText;
            }
            return tempStr;
        }
        #endregion

        #region 改操作
        /// <summary> 修改指定的索引行内的内容
        /// </summary>
        /// <param name="dataGridView">数据表对象</param>
        /// <param name="contont">修改的内容</param>
        /// <param name="index">行索引</param>
        public static void RevampRows(DataGridView dataGridView, string[] contont, int index) {
            for (int i = 0; i < dataGridView.Rows.Count; i++) {
                dataGridView.Rows[index].Cells[i].Value = contont[i];
            }
        }
        #endregion

        /// <summary> 禁止点击了表标题后对内容进行排序
        /// </summary>
        /// <param name="dataGridView">表对象</param>
        public static void BanSort(DataGridView dataGridView) 
        {
            for (int i = 0; i < dataGridView.Columns.Count; i++)
            {
                dataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        /// <summary> 将表头平均占据整个表的宽度
        /// </summary>
        /// <param name="dataGridView"></param>
        public static void SetTitleWidth(DataGridView dataGridView) {
            if (dataGridView.Columns.Count <= 1) { return; }
            int width = dataGridView.Size.Width / dataGridView.Columns.Count;

            for (int i = 0; i < dataGridView.Columns.Count; i++ )
            {
                dataGridView.Columns[i].Width = width;
            }
        }

        /// <summary> 开启表控件的双缓冲器
        /// </summary>
        /// <param name="dataGridView">表对象</param>
        public static void DataGridCreateParams(DataGridView dataGridView) {
            Type dgvType = dataGridView.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dataGridView, true, null) ;
        }
    }
}
