using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAdministraction.MES_Coonect;
using DataAdministraction.公用函数类;
using JP_DLL;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

namespace DataAdministrator.自动流程
{
    public abstract class Father
    {
        public Form1 form;// 主窗口对象
        public static string FilePath;// 用来保存这个流程的ini文件路径
        public MESClass mesClass;

        public int gongZhanYiJinZhan =0, gongZhanErJinZhan = 0, gongZhanShanJinZhan = 0, gongZhanSiJinZhan = 0, gongZhanWuJinZhan = 0, gongZhanluJinZhan = 0, shoJianJinZhan =0;
        public int gongZhanYiChuZhan = 0, gongZhanErChuZhan = 0, gongZhanShanChuZhan = 0, gongZhanSiChuZhan = 0, gongZhanWuChuZhan = 0, gongZhanluChuZhan = 0, shoJianChuZhan = 0;
        public string moduleCode1, moduleCode2, moduleCode3, moduleCode4, moduleCode5;// 存储读取出来的模组条码
        public string kmnr;
        public string PN1, PN2, PN3, PN4;
        public BT3562 mBT3562;
        public WT6582 wT6582;
        public L100B mL100B; //称重测试
        public bool ocvTest;
        public bool pressureTest=true;
        public decimal voltage, ir;
        public keyence_scan keyence_Scan;
        public string code_上线;
        public string MES_operator, productCode;
        public string tuJiaoProductCode, ruKeProductCode, zheWanProductCode, hanJieProductCode, hanHoProductCode, waiKeHanHoProductCode, reMaoProductCode;
        public double positiveInsulationValue, positiveInsulationTestTime, positivePressureValue, positiveTestVoltage, positiveTestTime,ycellvalue;
        public double negativeInsulationValue, negativeInsulationTestTime, negativePressureValue, negativeTestVoltage, negativeTestTime;

        public string StationInTime= "";
        public abstract void xintiao();

        public abstract string[] Read_Data(JObject obj,string[] tempStr);
        public abstract string[] Read_Data2(JObject obj, string[] tempStr);
        public abstract string[] Read_Data3(JObject obj, string[] tempStr);
        public abstract string[] Read_Data4(JObject obj, string[] tempStr);
        public abstract string[] Read_Data5(JObject obj, string[] tempStr);
        public abstract string 工位1进站(string Code = "");
        public abstract string 工位1进站复位(string Code = "");
        public abstract string 工位1出站(string Code = "");
        public abstract string 工位1出站复位(string Code = "");

        public abstract string 工位2进站(string Code = "");
        public abstract string 工位2进站复位(string Code = "");
        public abstract string 工位2出站(string Code = "");
        public abstract string 工位2出站复位(string Code = "");

        public abstract string 工位3进站(string Code = "");
        public abstract string 工位3进站复位(string Code = "");
        public abstract string 工位3出站(string Code = "");
        public abstract string 工位3出站复位(string Code = "");

        public abstract string 工位4进站(string Code = "");
        public abstract string 工位4进站复位(string Code = "");
        public abstract string 工位4出站(string Code = "");
        public abstract string 工位4出站复位(string Code = "");

        public abstract string 工位5进站(string Code = "");
        public abstract string 工位5进站复位(string Code = "");
        public abstract string 工位5出站(string Code = "");
        public abstract string 工位5出站复位(string Code = "");
        
        /// <summary>用于提取ini文件里面的PLC的地址
        /// </summary>
        /// <param name="PathName">地址的名字</param>
        /// <returns></returns>
        public string[] FindIniPath(string PathName)
        {
            for (int i = 0; i < form.Set_Ui.dataGridView20.Rows.Count; i++)
            {
                if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView20, i)[0].ToString() == PathName)
                {
                    string[] _PathName = { DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView20, i)[1].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView20, i)[2].ToString() };
                    return _PathName;
                }
            }
            form.P_Class.TxtLog("检测不到PLC的【" + PathName + "】地址", 2);
            return new string[] { "", "" };
        }
        
        private string ReadDBArray(string address, int addressInterval, int length, string type)
        {
            // 拆分地址字符串，提取DB地址中间的数值
            string[] temp1 = address.Split('.');
            string tempAddress = address;
            string temp = "";

            for (int i = 0; i < length; i++)
            {
                // 将读取到的地址数据进行一个存储
                if (type.Equals("STRING"))
                {
                    if (i == 0)
                        temp += form.PLC_ui.Read_String(tempAddress);
                    else
                        temp += "," + form.PLC_ui.Read_String(tempAddress);
                }
                else if(type.Equals("NUMBER"))
                {
                    if (i == 0)
                        temp += form.PLC_ui.siemens.SiemensS7_Net.ReadFloat(tempAddress).Content;
                    else
                        temp += "#" + form.PLC_ui.siemens.SiemensS7_Net.ReadFloat(tempAddress).Content;
                }
                else
                {
                    if (i == 0)
                        temp += form.PLC_ui.siemens.SiemensS7_Net.ReadFloat(tempAddress);
                    else
                        temp += "#" + form.PLC_ui.siemens.SiemensS7_Net.ReadFloat(tempAddress);
                }
                // 将中间的数值转成int类型的数据
                // 并进行加法运算，得到下一个要读取的地址
                tempAddress = temp1[0] + "." + (Convert.ToInt32(temp1[1]) + addressInterval) + "." + temp1[2];
                temp1 = tempAddress.Split('.');
            }
            return temp;
        }

        //循环读取plc里的值  /单工位
        public string[] readPLC_CS(string[] tempStr,string str_Name, string url, string productCode, string deviceCode, string userOperator,int StationType, ref JArray paramsArrayList, ref JArray materialsArrayList)
        {
            #region 保存本地数据
            string address1 = form.P_Class.FindIniPath("外壳焊缝长度")[0];
            int addressInterval1 = Convert.ToInt32(form.P_Class.FindIniPath("外壳焊缝长度")[1]);
            int length1 = Convert.ToInt32(form.P_Class.FindIniPath("外壳焊缝长度")[2]);
            string type1 = "NUMBER";
            string paramValue1 = ReadDBArray(address1, addressInterval1, length1, type1);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "外壳焊缝长度")] = paramValue1;

            address1 = form.P_Class.FindIniPath("外壳焊缝宽度")[0];
            addressInterval1 = Convert.ToInt32(form.P_Class.FindIniPath("外壳焊缝宽度")[1]);
            length1 = Convert.ToInt32(form.P_Class.FindIniPath("外壳焊缝宽度")[2]);
            type1 = "NUMBER";
            paramValue1 = ReadDBArray(address1, addressInterval1, length1, type1);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "外壳焊缝宽度")] = paramValue1;

            address1 = form.P_Class.FindIniPath("外壳焊缝余高(最高高度)")[0];
            addressInterval1 = Convert.ToInt32(form.P_Class.FindIniPath("外壳焊缝余高(最高高度)")[1]);
            length1 = Convert.ToInt32(form.P_Class.FindIniPath("外壳焊缝余高(最高高度)")[2]);
            type1 = "NUMBER";
            paramValue1 = ReadDBArray(address1, addressInterval1, length1, type1);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "外壳焊缝余高(最高高度)")] = paramValue1;

            address1 = form.P_Class.FindIniPath("位置度X1")[0];
            addressInterval1 = Convert.ToInt32(form.P_Class.FindIniPath("位置度X1")[1]);
            length1 = Convert.ToInt32(form.P_Class.FindIniPath("位置度X1")[2]);
            type1 = "NUMBER";
            paramValue1 = ReadDBArray(address1, addressInterval1, length1, type1);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "位置度X1")] = paramValue1;

            address1 = form.P_Class.FindIniPath("位置度X2")[0];
            addressInterval1 = Convert.ToInt32(form.P_Class.FindIniPath("位置度X2")[1]);
            length1 = Convert.ToInt32(form.P_Class.FindIniPath("位置度X2")[2]);
            type1 = "NUMBER";
            paramValue1 = ReadDBArray(address1, addressInterval1, length1, type1);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "位置度X2")] = paramValue1;

            address1 = form.P_Class.FindIniPath("位置度Y1")[0];
            addressInterval1 = Convert.ToInt32(form.P_Class.FindIniPath("位置度Y1")[1]);
            length1 = Convert.ToInt32(form.P_Class.FindIniPath("位置度Y1")[2]);
            type1 = "NUMBER";
            paramValue1 = ReadDBArray(address1, addressInterval1, length1, type1);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "位置度Y1")] = paramValue1;

            address1 = form.P_Class.FindIniPath("位置度Y2")[0];
            addressInterval1 = Convert.ToInt32(form.P_Class.FindIniPath("位置度Y2")[1]);
            length1 = Convert.ToInt32(form.P_Class.FindIniPath("位置度Y2")[2]);
            type1 = "NUMBER";
            paramValue1 = ReadDBArray(address1, addressInterval1, length1, type1);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "位置度Y2")] = paramValue1;

            address1 = form.P_Class.FindIniPath("炸点数量")[0];
            addressInterval1 = Convert.ToInt32(form.P_Class.FindIniPath("炸点数量")[1]);
            length1 = Convert.ToInt32(form.P_Class.FindIniPath("炸点数量")[2]);
            type1 = "NUMBER";
            paramValue1 = ReadDBArray(address1, addressInterval1, length1, type1);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "炸点数量")] = paramValue1;

            address1 = form.P_Class.FindIniPath("最大炸点直径")[0];
            addressInterval1 = Convert.ToInt32(form.P_Class.FindIniPath("最大炸点直径")[1]);
            length1 = Convert.ToInt32(form.P_Class.FindIniPath("最大炸点直径")[2]);
            type1 = "NUMBER";
            paramValue1 = ReadDBArray(address1, addressInterval1, length1, type1);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "最大炸点直径")] = paramValue1;

            address1 = form.P_Class.FindIniPath("凹坑数量")[0];
            addressInterval1 = Convert.ToInt32(form.P_Class.FindIniPath("凹坑数量")[1]);
            length1 = Convert.ToInt32(form.P_Class.FindIniPath("凹坑数量")[2]);
            type1 = "NUMBER";
            paramValue1 = ReadDBArray(address1, addressInterval1, length1, type1);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "凹坑数量")] = paramValue1;

            address1 = form.P_Class.FindIniPath("最大凹坑深度")[0];
            addressInterval1 = Convert.ToInt32(form.P_Class.FindIniPath("最大凹坑深度")[1]);
            length1 = Convert.ToInt32(form.P_Class.FindIniPath("最大凹坑深度")[2]);
            type1 = "NUMBER";
            paramValue1 = ReadDBArray(address1, addressInterval1, length1, type1);
            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, "最大凹坑深度")] = paramValue1;

            #endregion
            int indexParamsArray = 1;
            int indexmaterialsArray = 1;
            for (int i = 0; i < form.Set_Ui.dataGridView5.Rows.Count; i++)
            {
                if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[7].ToString().Equals(str_Name))
                {
                    if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[5].ToString().Equals("params"))
                    {
                        if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[4].ToString().Equals("STRING"))
                        {
                            string action = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[2].ToString();
                            string address = form.P_Class.FindIniPath(action)[0];
                            int addressInterval = Convert.ToInt32(form.P_Class.FindIniPath(action)[1]);                      
                            int length = Convert.ToInt32(DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[5].ToString());
                            string type = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString();
                            string paramValue = ReadDBArray(address, addressInterval, length, type);
                            string paramResult = "1";                          
                            //tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, action)] = paramValue;
                            dataAcquisitionParams(ref paramsArrayList, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[1].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[0].ToString(), paramValue, paramResult, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[8].ToString() + "~" + DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[7].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[9].ToString(), indexParamsArray.ToString(), StationType);
                            indexParamsArray++;
                        }
                        else if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[4].ToString().Equals("NUMBER"))
                        {
                            string action = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[2].ToString();
                            string paramValue = form.PLC_ui.siemens.SiemensS7_Net.ReadFloat(action).Content.ToString();
                            string paramResult = "1";

                            //tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, action)] = paramValue;

                            dataAcquisitionParams(ref paramsArrayList, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[1].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[0].ToString(), paramValue, paramResult, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[9].ToString() + "~" + DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[8].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[10].ToString(), indexParamsArray.ToString(),StationType);
                            indexParamsArray++;
                        }
                    }
                    else
                    {
                        if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[4].ToString().Equals("STRING"))
                        {
                            string action = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[2].ToString();
                            string address = form.P_Class.FindIniPath(action)[0];
                            int addressInterval = Convert.ToInt32(form.P_Class.FindIniPath(action)[1]);
                            int length = Convert.ToInt32(DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[5].ToString());
                            string type = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString();
                            string paramValue = ReadDBArray(address, addressInterval, length, type);
                            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            string MESret = mesClass.materialCheck(url, paramValue, productCode, deviceCode, userOperator, time);

                            JObject obj = JsonHelper.GetJObject(MESret);
                            if (obj["status"].ToString().Equals("200"))
                            {
                                tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, action)] = paramValue;

                                dataAcquisitionMaterials(ref materialsArrayList, paramValue, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[0].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[1].ToString(), indexmaterialsArray.ToString());
                                indexmaterialsArray++;
                            }
                            else
                            {
                                return tempStr;
                            }
                        }
                        else if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString().Equals("NUMBER"))
                        {
                            string action = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[2].ToString();
                            string address = form.P_Class.FindIniPath(action)[0];
                            int addressInterval = Convert.ToInt32(form.P_Class.FindIniPath(action)[1]);
                            int length = Convert.ToInt32(DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[5].ToString());
                            string type = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString();
                            string paramValue = ReadDBArray(address, addressInterval, length, type);
                            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            string MESret = mesClass.materialCheck(url, paramValue, productCode, deviceCode, userOperator, time);
                            JObject obj = JsonHelper.GetJObject(MESret);
                            if (obj["status"].ToString().Equals("200"))
                            {
                                tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, action)] = paramValue;

                                dataAcquisitionMaterials(ref materialsArrayList, paramValue, obj["data"]["materialName"].ToString(), obj["data"]["materialCode"].ToString(), indexmaterialsArray.ToString());
                                indexmaterialsArray++;
                            }
                            else
                            {
                                return tempStr;
                            }
                        }
                    }
                }
            }
            return tempStr;
        }

        //循环读取plc里的值
        public string[] readPLC_CS1(string[] tempStr, string str_Name, string url, string productCode, string deviceCode, string userOperator,int StationType, ref JArray paramsArrayList, ref JArray materialsArrayList)
        {
            int indexParamsArray = 1;
            int indexmaterialsArray = 1;
            for (int i = 0; i < form.Set_Ui.dataGridView5.Rows.Count; i++)
            {
                if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[6].ToString().Equals(str_Name))
                {
                    if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[4].ToString().Equals("params"))
                    {
                        if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString().Equals("STRING"))
                        {
                            string action = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[2].ToString();
                            string address = form.P_Class.FindIniPath(action)[0];
                            int addressInterval = Convert.ToInt32(form.P_Class.FindIniPath(action)[1]);
                            int length = Convert.ToInt32(DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[5].ToString());
                            string type = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString();
                            string paramValue = ReadDBArray(address, addressInterval, length, type);
                            string paramResult = "1";

                            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridViewKIN, action)] = paramValue;

                            dataAcquisitionParams(ref paramsArrayList, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[1].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[0].ToString(), paramValue, paramResult, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[8].ToString() + "~" + DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[7].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[9].ToString(), indexParamsArray.ToString(), StationType);
                            indexParamsArray++;
                        }
                        else if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString().Equals("NUMBER"))
                        {
                            string action = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[2].ToString();
                            string address = form.P_Class.FindIniPath(action)[0];
                            int addressInterval = Convert.ToInt32(form.P_Class.FindIniPath(action)[1]);
                            int length = Convert.ToInt32(DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[5].ToString());
                            string type = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString();
                            string paramValue = ReadDBArray(address, addressInterval, length, type);
                            string paramResult = "1";

                            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridViewKIN, action)] = paramValue;

                            dataAcquisitionParams(ref paramsArrayList, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[1].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[0].ToString(), paramValue, paramResult, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[8].ToString() + "~" + DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[7].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[9].ToString(), indexParamsArray.ToString(), StationType);
                            indexParamsArray++;
                        }
                    }
                    else
                    {
                        if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString().Equals("STRING"))
                        {
                            string action = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[2].ToString();
                            string address = form.P_Class.FindIniPath(action)[0];
                            int addressInterval = Convert.ToInt32(form.P_Class.FindIniPath(action)[1]);
                            int length = Convert.ToInt32(DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[5].ToString());
                            string type = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString();
                            string paramValue = ReadDBArray(address, addressInterval, length, type);
                            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            string MESret = mesClass.materialCheck(url, paramValue, productCode, deviceCode, userOperator, time);
                            JObject obj = JsonHelper.GetJObject(MESret);
                            if (obj["status"].ToString().Equals("200"))
                            {
                                tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridViewKIN, action)] = paramValue;

                                dataAcquisitionMaterials(ref materialsArrayList, paramValue, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[0].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[1].ToString(), indexmaterialsArray.ToString());
                                indexmaterialsArray++;
                            }
                            else
                            {              //////更改  校验NG也读PLC值并返还，不return null;
                                return tempStr;
                            }
                        }
                        else if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString().Equals("NUMBER"))
                        {
                            string action = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[2].ToString();
                            string address = form.P_Class.FindIniPath(action)[0];
                            int addressInterval = Convert.ToInt32(form.P_Class.FindIniPath(action)[1]);
                            int length = Convert.ToInt32(DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[5].ToString());
                            string type = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString();
                            string paramValue = ReadDBArray(address, addressInterval, length, type);
                            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            string MESret = mesClass.materialCheck(url, paramValue, productCode, deviceCode, userOperator, time);
                            JObject obj = JsonHelper.GetJObject(MESret);
                            if (obj["status"].ToString().Equals("200"))
                            {
                                tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridViewKIN, action)] = paramValue;

                                dataAcquisitionMaterials(ref materialsArrayList, paramValue, obj["data"]["materialName"].ToString(), obj["data"]["materialCode"].ToString(), indexmaterialsArray.ToString());
                                indexmaterialsArray++;
                            }
                            else
                            {
                                return tempStr;
                            }
                        }
                    }
                }
            }
            return tempStr;
        }

        public string[] readPLC_CS(string[] tempStr, string str_Name,double [] testArray, string url, string productCode, string deviceCode, string userOperator,int StationType ,ref JArray paramsArrayList, ref JArray materialsArrayList)
        {
            int indexParamsArray = 1;
            int indexmaterialsArray = 1;
            for (int i = 0; i < form.Set_Ui.dataGridView5.Rows.Count; i++)
            {
                if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[6].ToString().Equals(str_Name))
                {
                    if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[4].ToString().Equals("params"))
                    {
                        string paramValue = testArray[i].ToString();
                        string paramResult = "1";

                        tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[2].ToString())] = paramValue;

                        dataAcquisitionParams(ref paramsArrayList, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[1].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[0].ToString(), paramValue, paramResult, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[8].ToString() + "~" + DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[7].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[9].ToString(), indexParamsArray.ToString(), StationType);
                    }
                    else
                    {
                        
                    }
                }
            }
            return tempStr;
        }


        /// <summary>
        /// 双工位循坏读数
        /// </summary>
        /// <param name="tempStr"></param>
        /// <param name="str_Name"></param>
        /// <param name="url"></param>
        /// <param name="productCode"></param>
        /// <param name="deviceCode"></param>
        /// <param name="userOperator"></param>
        /// <param name="index"></param>
        /// <param name="StationType"></param>
        /// <param name="paramsArrayList"></param>
        /// <param name="materialsArrayList"></param>
        /// <returns></returns>
        public string[] readPLC_CS2(string[] tempStr, string str_Name,string url,string productCode, string deviceCode,string userOperator, int index,int StationType, ref JArray paramsArrayList, ref JArray materialsArrayList)
        {
            int indexParamsArray = 1;
            int indexmaterialsArray = 1;
            for (int i = 0; i < form.Set_Ui.dataGridView5.Rows.Count; i++)
            {
                if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[6].ToString().Equals(str_Name))
                {
                    if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[4].ToString().Equals("params"))
                    {
                        if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString().Equals("STRING"))
                        {
                            string action = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[2].ToString();
                            string address = form.P_Class.FindIniPath(action + index)[0];
                            int addressInterval = Convert.ToInt32(form.P_Class.FindIniPath(action + index)[1]);
                            int length = Convert.ToInt32(DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[5].ToString());
                            string type = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString();
                            string paramValue = ReadDBArray(address, addressInterval, length, type);
                            string paramResult = "1";

                            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, action)] = paramValue;

                            dataAcquisitionParams(ref paramsArrayList, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[1].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[0].ToString(), paramValue, paramResult, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[8].ToString() + "~" + DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[7].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[9].ToString(), indexParamsArray.ToString(), StationType);
                            indexParamsArray++;
                        }
                        else if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString().Equals("NUMBER"))
                        {
                            string action = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[2].ToString();
                            string address = form.P_Class.FindIniPath(action + index)[0];
                            int addressInterval = Convert.ToInt32(form.P_Class.FindIniPath(action + index)[1]);
                            int length = Convert.ToInt32(DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[5].ToString());
                            string type = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString();
                            string paramValue = ReadDBArray(address, addressInterval, length, type);
                            string paramResult = "1";

                            tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, action)] = paramValue;

                            dataAcquisitionParams(ref paramsArrayList, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[1].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[0].ToString(), paramValue, paramResult, DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[8].ToString() + "~" + DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[7].ToString(), DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[9].ToString(), indexParamsArray.ToString(), StationType);
                            indexParamsArray++;
                        }
                    }
                    else
                    {
                        if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString().Equals("STRING"))
                        {
                            string action = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[2].ToString();
                            string address = form.P_Class.FindIniPath(action + index)[0];
                            int addressInterval = Convert.ToInt32(form.P_Class.FindIniPath(action + index)[1]);
                            int length = Convert.ToInt32(DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[5].ToString());
                            string type = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString();
                            string paramValue = ReadDBArray(address, addressInterval, length, type);
                            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            string MESret = mesClass.materialCheck(url, paramValue, productCode, deviceCode, userOperator, time);
                            JObject obj = JsonHelper.GetJObject(MESret);
                            if (obj["status"].ToString().Equals("200"))
                            {
                                tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, action)] = paramValue;

                                dataAcquisitionMaterials(ref materialsArrayList, paramValue, obj["data"]["materialName"].ToString(), obj["data"]["materialCode"].ToString(), indexmaterialsArray.ToString());
                                indexmaterialsArray++;
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString().Equals("NUMBER"))
                        {
                            string action = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[2].ToString();
                            string address = form.P_Class.FindIniPath(action + index)[0];
                            int addressInterval = Convert.ToInt32(form.P_Class.FindIniPath(action + index)[1]);
                            int length = Convert.ToInt32(DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[5].ToString());
                            string type = DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView5, i)[3].ToString();
                            string paramValue = ReadDBArray(address, addressInterval, length, type);
                            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            string MESret =  mesClass.materialCheck(url, paramValue, productCode, deviceCode, userOperator, time);
                            JObject obj = JsonHelper.GetJObject(MESret);
                            if (obj["status"].ToString().Equals("200"))
                            {
                                tempStr[DataGridViewClass.GetColumnsIndex(form.dataGridView2, action)] = paramValue;

                                dataAcquisitionMaterials(ref materialsArrayList, paramValue, obj["data"]["materialName"].ToString(), obj["data"]["materialCode"].ToString(), indexmaterialsArray.ToString());
                                indexmaterialsArray++;
                            }
                            else {
                                return null;
                            }
                        }
                    }
                }
            }
            return tempStr;
        }

        public void dataAcquisitionMaterials(ref JArray arrayList, string materialSn, string materialName, string materialCode, string scanNumber)
        {
            JObject obj = new JObject();
            obj["materialSn"] = materialSn;
            obj["materialName"] = materialName;
            obj["materialCode"] = materialCode;
            obj["scanNumber"] = scanNumber;
            arrayList.Add(obj);
        }

        public void dataAcquisitionParams(ref JArray arrayList, string paramCode, string paramName, string paramValue, string paramResult, string paramRange, string paramUnit, string paramNumber,int type)
        {
            ////JObject obj = new JObject();
            ////obj["paramCode"] = paramCode;
            ////obj["paramName"] = paramName;
            ////obj["paramValue"] = paramValue;
            ////obj["paramResult"] = paramResult;
            ////obj["paramRange"] = paramRange;
            ////obj["paramUnit"] = paramUnit;
            ////obj["paramNumber"] = paramNumber;
            ////arrayList.Add(obj);

          /////////////////// ////方法1       paramName="param{0}Name";
       
            string[] paramValues = paramValue.Split('#');
          
            if (type == 1)
            {
                int num = int.Parse(paramNumber);
                //JArray arrayList = new JArray { };

                for (int i = 0; i < paramValues.Length; i++)
                {
                    arrayList.Add
                        (
                         new JObject
                         {
                         { "paramCode",string.Format(paramCode,i+1) },
                         { "paramName", string.Format(paramName,i+1)},
                         { "paramValue", paramValues[i]},
                         { "paramResult", paramResult},
                         { "paramRange",  paramRange },
                         { "paramUnit", paramUnit },
                         { "paramNumber", num}
                         }
                        );

                    num++;
                }

            }

            /////////////////// ////方法2    
            if (type == 2)
            {
                //JArray arrayList = new JArray { };
                //string paramName = "weld{0}Length";
                //string paramCode = "焊缝{0}长度";
                //string paramValues = "111#2222#333#444#555";
                //string[] paramValue1s = paramValue.Split('#');
                //int num = 1;
                string _paramName = "";
                string _paramCode = "";
                for (int i = 0; i < paramValues.Length; i++)
                {
                    int num1 = int.Parse(paramNumber);
                    switch (i)
                    {
                        case 0:
                            _paramName = string.Format(paramName, "6-3");
                            _paramCode = string.Format(paramCode, "6-3");
                            break;
                        case 1:
                            _paramName = string.Format(paramName, "6-2");
                            _paramCode = string.Format(paramCode, "6-2");
                            break;
                        case 2:
                            _paramName = string.Format(paramName, "6-1");
                            _paramCode = string.Format(paramCode, "6-1");
                            break;
                        case 3:
                            _paramName = string.Format(paramName, "7-1");
                            _paramCode = string.Format(paramCode, "7-1");
                            break;
                        case 4:
                            _paramName = string.Format(paramName, "7-2");
                            _paramCode = string.Format(paramCode, "7-2");
                            break;
                        case 5:
                            _paramName = string.Format(paramName, "7-3");
                            _paramCode = string.Format(paramCode, "7-3");
                            break;
                        case 6:
                            _paramName = string.Format(paramName, "4-2");
                            _paramCode = string.Format(paramCode, "4-2");
                            break;
                        case 7:
                            _paramName = string.Format(paramName, "1-2");
                            _paramCode = string.Format(paramCode, "1-2");
                            break;
                        case 8:
                            _paramName = string.Format(paramName, "2-2");
                            _paramCode = string.Format(paramCode, "2-2");
                            break;
                        case 9:
                            _paramName = string.Format(paramName, "3-2");
                            _paramCode = string.Format(paramCode, "3-2");
                            break;
                        case 10:
                            _paramName = string.Format(paramName, "5-2");
                            _paramCode = string.Format(paramCode, "5-2");
                            break;
                        case 11:
                            _paramName = string.Format(paramName, "4-1");
                            _paramCode = string.Format(paramCode, "4-1");
                            break;
                        case 12:
                            _paramName = string.Format(paramName, "1-1");
                            _paramCode = string.Format(paramCode, "1-1");
                            break;
                        case 13:
                            _paramName = string.Format(paramName, "2-1");
                            _paramCode = string.Format(paramCode, "2-1");
                            break;
                        case 14:
                            _paramName = string.Format(paramName, "3-1");
                            _paramCode = string.Format(paramCode, "3-1");
                            break;
                        case 15:
                            _paramName = string.Format(paramName, "5-1");
                            _paramCode = string.Format(paramCode, "5-1");
                            break;
                        case 16:
                            _paramName = string.Format(paramName, "8-1");
                            _paramCode = string.Format(paramCode, "8-1");
                            break;
                        case 17:
                            _paramName = string.Format(paramName, "8-2");
                            _paramCode = string.Format(paramCode, "8-2");
                            break;
                          
                            break;
                            break;
                        default:
                            break;
                    }
                    arrayList.Add(
                         new JObject
                         {
                         { "paramCode", _paramCode},
                         { "paramName", _paramName},
                         { "paramValue", paramValues[i]},
                         { "paramResult", paramResult},
                         { "paramRange",paramRange},
                         { "paramUnit", paramUnit},
                         { "paramNumber", num1}
                         }
                        );

                    num1++;
                }
            } 
        }
        public string getData2Value(string str_Name)
        {
            for (int i = 0; i < form.Set_Ui.dataGridView2.Rows.Count; i++)
            {
                if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView2, i)[1].ToString().Equals(str_Name))
                {
                    return DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView2, i)[2].ToString();
                }
            }
            form.P_Class.TxtLog("MES配置异常：" + str_Name + "未读取到", 1);
            return "";
        }

        public string getData3Value(string str_Name)
        {
            for (int i = 0; i < form.Set_Ui.dataGridView3.Rows.Count; i++)
            {
                if (DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView3, i)[1].ToString().Equals(str_Name))
                {
                    return DataGridViewClass.GetRowsData(form.Set_Ui.dataGridView3, i)[2].ToString();
                }
            }
            form.P_Class.TxtLog("MES配置异常：" + str_Name + "未读取到", 1);
            return "";
        }
    }
}
