using DataAdministraction.公用函数类;
using DataAdministrator;
using JP_DLL;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataAdministraction.MES_Coonect
{
    public partial class MESClass
    {
        public Form1 form;

        public MESClass(Form1 form){
            this.form = form;
        }

        public string UploadToMES(string jsonData, string url)
        {
            form.P_Class.TxtLog("上传MES数据： " + jsonData, 3);
            string returnJson = string.Empty;
            try
            {
                //创建web访问对象
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //把用户传过来的数据转换成utf-8的字节流
                byte[] buffer = Encoding.GetEncoding("UTF-8").GetBytes(jsonData);
                request.Method = "POST";
                request.ContentLength = buffer.Length;
                request.ContentType = "application/json;charset=utf-8 ";
                //request.MaximumAutomaticRedirections = 1;
                //request.AllowAutoRedirect = true;
                //发送请求
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Close();
                }
                Thread.Sleep(500);
                //获取接口返回值
                //通过Web访问对象获取响应内容
                using (HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse())
                {
                    //通过响应内容流创建StreamReader对象，因为StreamReader更高级更快
                    StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                    //string returnXml = HttpUtility.UrlDecode(reader.ReadToEnd());//如果有编码问题就用这个方法
                    returnJson = reader.ReadToEnd();//利用StreamReader就可以从响应内容从头读到尾
                    reader.Close();
                    myResponse.Close();
                }
            }

            
            catch (System.Net.WebException ex) //by txwtech
            {
                var strError = ex.Message;
                using (WebResponse res = ex.Response)
                {
                    var httpResponse = (HttpWebResponse)res;
                    if (res!=null)
                    {
                        using (Stream data = res.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(data))
                            {
                                strError = reader.ReadToEnd();
                            }
                        }
                        returnJson = strError;
                    }
                    else 
                    {
                                 
                        returnJson = strError;
                        form.P_Class.TxtLog("MES网络连接异常,请检查" + returnJson, 1);
                        return returnJson;
                    }

                   

                }
                //if (ex.Status != WebExceptionStatus.ProtocolError)
                //    returnJson = strError;
                //return returnJson;
               
            }
           
            return returnJson;
        }

        public string UploadToME1(string jsonData, string url)
        {
            //form.P_Class.TxtLog("上传MES数据： " + jsonData, 3);
            string returnJson = string.Empty;
            try
            {
                //创建web访问对象
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                //把用户传过来的数据转换成utf-8的字节流
                byte[] buffer = Encoding.GetEncoding("UTF-8").GetBytes(jsonData);
                request.Method = "POST";
                request.ContentLength = buffer.Length;
                request.ContentType = "application/json;charset=utf-8 ";
                //request.MaximumAutomaticRedirections = 1;
                //request.AllowAutoRedirect = true;
                //发送请求
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Close();
                }
                Thread.Sleep(500);
                //获取接口返回值
                //通过Web访问对象获取响应内容
                using (HttpWebResponse myResponse = (HttpWebResponse)request.GetResponse())
                {
                    //通过响应内容流创建StreamReader对象，因为StreamReader更高级更快
                    StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                    //string returnXml = HttpUtility.UrlDecode(reader.ReadToEnd());//如果有编码问题就用这个方法
                    returnJson = reader.ReadToEnd();//利用StreamReader就可以从响应内容从头读到尾
                    reader.Close();
                    myResponse.Close();
                }
            }


            catch (System.Net.WebException ex) //by txwtech
            {
                var strError = ex.Message;
                using (WebResponse res = ex.Response)
                {
                    var httpResponse = (HttpWebResponse)res;
                    if (res != null)
                    {
                        using (Stream data = res.GetResponseStream())
                        {
                            using (StreamReader reader = new StreamReader(data))
                            {
                                strError = reader.ReadToEnd();
                            }
                        }
                        returnJson = strError;
                    }
                    else
                    {

                        returnJson = strError;
                        form.P_Class.TxtLog("MES网络连接异常,请检查" + returnJson, 1);
                        return returnJson;
                    }



                }
                //if (ex.Status != WebExceptionStatus.ProtocolError)
                //    returnJson = strError;
                //return returnJson;

            }

            return returnJson;
        }

        //进站
        /**
         * url:接口地址
         * WipCode：WIP码
         * deviceCode：设备编码
         * processedMode：加工模式
         * Useroperator:操作员
         * timestamp：操作时间
         * workstation：工位
         * plc_Name_OK：OK信号
         *  plc_Name_NG：NG信号
         */
        public bool inbound_MES(string url,string WipCode,string deviceCode, string processedMode, string Useroperator, string timestamp, int workstation, string plc_Name_OK, string plc_Name_NG,ref string productCode) {
            JObject mes_Obj = new JObject();
            mes_Obj["productSn"] = WipCode;
            mes_Obj["deviceCode"] = deviceCode;
            mes_Obj["processedMode"] = processedMode;
            mes_Obj["operator"] = Useroperator;
            mes_Obj["timestamp"] = timestamp;
            string rtn = form.MySignl.mesClass.UploadToMES(mes_Obj.ToString(), url);
            try
            {
                mes_Obj = JsonHelper.GetJObject(rtn);
                if (mes_Obj["status"].ToString() == "200")
                {
                    productCode = mes_Obj["data"]["productCode"].ToString();
                    form.P_Class.TxtLog("MES反馈：" + mes_Obj.ToString(), 3);
                    form.PLC_ui.write(form.MySignl.FindIniPath(plc_Name_OK)[0], true);
                    form.P_Class.TxtLog(plc_Name_OK + "！", workstation);
                    return true;
                }
                else
                {
                    form.P_Class.TxtLog("MES反馈：" + mes_Obj.ToString(), 3);
                    form.PLC_ui.write(form.MySignl.FindIniPath(plc_Name_NG)[0], true);
                    form.P_Class.TxtLog(plc_Name_NG + "！", workstation);
                    return false;
                }
            }
            catch (Exception ex)
            {
                form.PLC_ui.write(form.MySignl.FindIniPath(plc_Name_NG)[0], true);
                form.P_Class.TxtLog(plc_Name_NG + "！", workstation);
                form.P_Class.TxtLog("MES数据反馈异常：" + ex.ToString(), 3);
                return false;
            }
        }

        //出站
        /*
         * url:接口地址
         * WipCode:WIP码
         * deviceCode:设备编码
         * isOutBound:是否出站
         * userOperator:操作人
         * timestamp:操作时间
         * deviceName:设备名称
         * workstation:工位
         * plc_Name_OK:OK信号
         * plc_Name_NG:NG信号
         * **/
        public bool outbound_MES(string url ,string WipCode,string deviceCode,string isOutBound , string  userOperator,string timestamp,string deviceName, JArray paramsArrayList, JArray materialsArrayList ,int workstation ,string plc_Name_OK,string plc_Name_NG, string forceOk = "0") {
            JObject mes_Obj = new JObject();
            mes_Obj = new JObject();
            mes_Obj["productSn"] = WipCode;
            mes_Obj["deviceCode"] = deviceCode;
            mes_Obj["isOutBound"] = isOutBound;
            mes_Obj["operator"] = userOperator;
            mes_Obj["timestamp"] = timestamp;
            mes_Obj["result"] = "1";
            mes_Obj["forceOk"] = forceOk;

            form.P_Class.TxtLog("MES数据获取完成", 2);
            mes_Obj["materials"] = materialsArrayList;
            mes_Obj["params"] = paramsArrayList;
            string rtn = form.MySignl.mesClass.UploadToMES(mes_Obj.ToString(), url);
           
            form.P_Class.TxtLog("数据已上传MES", 2);
            try
            {
                mes_Obj = JsonHelper.GetJObject(rtn);
                form.P_Class.TxtLog("MES出站反馈："+ mes_Obj.ToString(), 3);
                if (mes_Obj["status"].ToString() == "200")
                {
                    form.PLC_ui.write(form.MySignl.FindIniPath(plc_Name_OK)[0], true);
                    Thread.Sleep(50);
                    form.PLC_ui.write(form.MySignl.FindIniPath(plc_Name_OK)[0], true);
                    form.P_Class.TxtLog(plc_Name_OK + "！", 2);
                    return true;
                }
                else
                {
                    form.PLC_ui.write(form.MySignl.FindIniPath(plc_Name_NG)[0], true);
                    Thread.Sleep(50);
                    form.PLC_ui.write(form.MySignl.FindIniPath(plc_Name_NG)[0], true);
                    form.P_Class.TxtLog(plc_Name_NG + "！", 2);
                    return false;
                }
            }
            catch (Exception ex)
            {
                form.PLC_ui.write(form.MySignl.FindIniPath(plc_Name_NG)[0], true);
                form.P_Class.TxtLog("MES反馈数据异常" + ex.ToString(), 3);
                return false;
            }
        }

        public bool outbound_MES_TWO(string url, string WipCode, string deviceCode, string isOutBound, string userOperator, string timestamp, string deviceName, JArray paramsArrayList, JArray materialsArrayList, int workstation, string plc_Name_OK, string plc_Name_NG)
        {
            JObject mes_Obj = new JObject();
            mes_Obj = new JObject();
            mes_Obj["productSn"] = WipCode;
            mes_Obj["deviceCode"] = deviceCode;
            mes_Obj["isOutBound"] = isOutBound;
            mes_Obj["operator"] = userOperator;
            mes_Obj["timestamp"] = timestamp;
            mes_Obj["result"] = "1";
            mes_Obj["materials"] = materialsArrayList;
            mes_Obj["params"] = paramsArrayList;
            string rtn = form.MySignl.mesClass.UploadToMES(mes_Obj.ToString(), url);
         
            try
            {
                mes_Obj = JsonHelper.GetJObject(rtn);
                form.P_Class.TxtLog("MES出站反馈：" + mes_Obj.ToString(), 3);
                if (mes_Obj["status"].ToString() == "200")
                {
                    form.PLC_ui.write(form.MySignl.FindIniPath(plc_Name_OK)[0], true);
                    form.P_Class.TxtLog(plc_Name_OK + "！", 2);
                    return true;
                }
                else
                {
                    form.PLC_ui.write(form.MySignl.FindIniPath(plc_Name_NG)[0], true);
                    form.P_Class.TxtLog(plc_Name_NG + "！", 2);
                    return false;
                }
            }
            catch (Exception ex)
            {
                form.PLC_ui.write(form.MySignl.FindIniPath(plc_Name_NG)[0], true);
                form.P_Class.TxtLog("MES反馈数据异常"+ ex.ToString(), 3);
                return false;
            }
        }

        public string materialCheck(string url,string qrCode,string productCode,string deviceCode,string useroperator,string timestamp)
        {
            JObject mes_Obj = new JObject();
            mes_Obj = new JObject();
            mes_Obj["qrCode"] = qrCode;
            mes_Obj["productCode"] = productCode;
            mes_Obj["deviceCode"] = deviceCode;
            mes_Obj["operator"] = useroperator;
            mes_Obj["timestamp"] = timestamp;
            string rtn = form.MySignl.mesClass.UploadToMES(mes_Obj.ToString(), url);

         
            try
            {
                mes_Obj = JsonHelper.GetJObject(rtn);
                form.P_Class.TxtLog("物料校验MES反馈" + mes_Obj.ToString(), 3);
                if (mes_Obj["status"].ToString() == "200")
                {
                    return mes_Obj.ToString();
                }
                else
                {
                    return mes_Obj.ToString();
                }
            }
            catch (System.Net.WebException ex) //by txwtech
            {
                var strError = ex.Message;
                using (WebResponse res = ex.Response)
                {
                    var httpResponse = (HttpWebResponse)res;
                    using (Stream data = res.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(data))
                        {
                            strError = reader.ReadToEnd();
                        }
                    }
                    //  statusCode = httpResponse.StatusCode;
                }
                if (ex.Status != WebExceptionStatus.ProtocolError)
                    throw;

                // responseBody = strError;

                //erro = ex.ToString();
                // erro = "error";
                // return_message = "网络异常\r\n请求超时\r\n" + ex.StackTrace;
                //return_message = strError;
                return strError;
            } catch (Exception ex) {
                return "";
            }
        }

        public bool barcodeReplacement(string url ,string tempProductSn, string productSn, string deviceCode,string userOperator,string timestamp) {
            JObject obj = new JObject();
            obj = new JObject();
            obj["tempProductSn"] = tempProductSn;
            obj["productSn"] = productSn;
            obj["deviceCode"] = deviceCode;
            obj["operator"] = userOperator;
            obj["timestamp"] = timestamp;
            string rtn = form.MySignl.mesClass.UploadToMES(obj.ToString(), url);

            try
            {

                obj = JsonHelper.GetJObject(rtn);
                form.P_Class.TxtLog("虚拟码替换MES反馈" + obj.ToString(), 3);
                if (obj["status"].ToString() == "200")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (System.Net.WebException ex) //by txwtech
            {
                var strError = ex.Message;
                using (WebResponse res = ex.Response)
                {
                    var httpResponse = (HttpWebResponse)res;
                    using (Stream data = res.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(data))
                        {
                            strError = reader.ReadToEnd();
                        }
                    }
                    //  statusCode = httpResponse.StatusCode;
                }
                if (ex.Status != WebExceptionStatus.ProtocolError)
                    throw;

                // responseBody = strError;

                //erro = ex.ToString();
                // erro = "error";
                // return_message = "网络异常\r\n请求超时\r\n" + ex.StackTrace;
                //return_message = strError;
                return false;
            } catch (Exception ex) {
                return false;
            }
        }

        /// <summary>
        /// 设备心跳检测
        /// </summary>
        /// <param name="deviceCode">设备编号</param>
        /// <param name="newResult">操作人</param>
        /// <param name="style">状态</param>
        /// <returns></returns>
        public string 设备心跳检测(string deviceCode, string newResult, string style = "1")
        {
            JObject json = new JObject();
            json["deviceCode"] = deviceCode;
            json["isOnline"] = style;// 0 = 离线，1 = 在线
            json["operator"] = newResult;
            json["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");// 日期时间
            string rtn = form.MySignl.mesClass.UploadToME1(json.ToString(), form.MySignl.getData2Value("设备心跳URL"));


            try
            {
                json = JsonHelper.GetJObject(rtn);
                //form.P_Class.TxtLog("设备心跳上传MES反馈" + json.ToString(), 3);
                if (json["status"].ToString() == "200")
                {
                    return json.ToString();
                }
                else
                {
                    return json.ToString();
                }
            }
            catch (System.Net.WebException ex) //by txwtech
            {
                var strError = ex.Message;
                using (WebResponse res = ex.Response)
                {
                    var httpResponse = (HttpWebResponse)res;
                    using (Stream data = res.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(data))
                        {
                            strError = reader.ReadToEnd();
                        }
                    }
                    //  statusCode = httpResponse.StatusCode;
                }
                if (ex.Status != WebExceptionStatus.ProtocolError)
                    throw;

                // responseBody = strError;

                //erro = ex.ToString();
                // erro = "error";
                // return_message = "网络异常\r\n请求超时\r\n" + ex.StackTrace;
                //return_message = strError;
                return strError;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 设备实时状态
        /// </summary>
        /// <param name="deviceStatus">状态</param>
        /// <param name="newResult">操作人</param>
        /// <param name="deviceCode">设备编号</param>
        /// <returns></returns>
        public string 设备实时状态(string deviceStatus, string newResult, string deviceCode)
        {
            JObject json = new JObject();
            json["deviceCode"] = deviceCode;
            json["deviceStatus"] = deviceStatus;// 1:自动状态 2:手动状态3:待机状态 4:故障状态 5:维护状态 6:停机状态 
            json["operator"] = newResult;
            json["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");// 日期时间
            string rtn = form.MySignl.mesClass.UploadToMES(json.ToString(), form.MySignl.getData2Value("设备实时状态URL"));


            try
            {
                json = JsonHelper.GetJObject(rtn);
                form.P_Class.TxtLog("设备实时状态上传MES反馈" + json.ToString(), 3);
                if (json["status"].ToString() == "200")
                {
                    return json.ToString();
                }
                else
                {
                    return json.ToString();
                }
            }
            catch (System.Net.WebException ex) //by txwtech
            {
                var strError = ex.Message;
                using (WebResponse res = ex.Response)
                {
                    var httpResponse = (HttpWebResponse)res;
                    using (Stream data = res.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(data))
                        {
                            strError = reader.ReadToEnd();
                        }
                    }
                    //  statusCode = httpResponse.StatusCode;
                }
                if (ex.Status != WebExceptionStatus.ProtocolError)
                    throw;

                // responseBody = strError;

                //erro = ex.ToString();
                // erro = "error";
                // return_message = "网络异常\r\n请求超时\r\n" + ex.StackTrace;
                //return_message = strError;
                return strError;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 设备报警采集
        /// </summary>
        /// <param name="alarmStatus">0：解除报警，1：发生报警</param>
        /// <param name="alarmCode">报警编码</param>
        /// <param name="alarmMsg">报警信息</param>
        /// <param name="deviceCode">设备编号</param>
        /// <param name="newResult">操作人</param>
        /// <returns></returns>
        public string 设备报警采集(string alarmStatus, string alarmCode, string alarmMsg, string deviceCode, string newResult)
        {

            JObject json = new JObject();
            json["deviceCode"] = deviceCode;//设备编号
            json["alarmStatus"] = alarmStatus;// 0：解除报警，1：发生报警
            json["alarmCode"] = alarmCode;//报警编码
            json["alarmMsg"] = alarmMsg;//报警信息
            json["operator"] = newResult;//操作人
            json["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");// 日期时间
            string rtn = form.MySignl.mesClass.UploadToMES(json.ToString(), form.MySignl.getData2Value("设备报警采集URL"));


            try
            {
                json = JsonHelper.GetJObject(rtn);
                form.P_Class.TxtLog("设备报警采集上传MES反馈" + json.ToString(), 3);
                if (json["status"].ToString() == "200")
                {
                    return json.ToString();
                }
                else
                {
                    return json.ToString();
                }
            }
            catch (System.Net.WebException ex) //by txwtech
            {
                var strError = ex.Message;
                using (WebResponse res = ex.Response)
                {
                    var httpResponse = (HttpWebResponse)res;
                    using (Stream data = res.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(data))
                        {
                            strError = reader.ReadToEnd();
                        }
                    }
                    //  statusCode = httpResponse.StatusCode;
                }
                if (ex.Status != WebExceptionStatus.ProtocolError)
                    throw;

                // responseBody = strError;

                //erro = ex.ToString();
                // erro = "error";
                // return_message = "网络异常\r\n请求超时\r\n" + ex.StackTrace;
                //return_message = strError;
                return strError;
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        /// <summary>
        /// 设备停机采集
        /// </summary>
        /// <param name="deviceCode">设备编号</param>
        /// <param name="downReasonCode">停机原因</param>
        /// <param name="beginTime">开始停机时间</param>
        /// <param name="endTime">结束停机时间</param>
        /// <param name="newResult">操作人</param>
        /// <returns></returns>
        public string 设备停机采集(string deviceCode, string downReasonCode, string beginTime, string endTime, string newResult)
        {

            JObject json = new JObject();
            json["deviceCode"] = deviceCode;
            json["downReasonCode"] = downReasonCode;//01：维护保养;02：吃饭/休息;03：换型;04：设备改造;05：来料不良;06：设备校验;07：首件/点检;08：品质异常;09：维护保养&缺备件;10：环境异常;11：设备信息不完善
            json["beginTime"] = beginTime;
            json["endTime"] = endTime;
            json["operator"] = newResult;
            json["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");// 日期时间
            string rtn = form.MySignl.mesClass.UploadToMES(json.ToString(), form.MySignl.getData2Value("设备停机采集URL"));


            try
            {
                json = JsonHelper.GetJObject(rtn);
                form.P_Class.TxtLog("设备停机采集上传MES反馈" + json.ToString(), 3);
                if (json["status"].ToString() == "200")
                {
                    return json.ToString();
                }
                else
                {
                    return json.ToString();
                }
            }
            catch (System.Net.WebException ex) //by txwtech
            {
                var strError = ex.Message;
                using (WebResponse res = ex.Response)
                {
                    var httpResponse = (HttpWebResponse)res;
                    using (Stream data = res.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(data))
                        {
                            strError = reader.ReadToEnd();
                        }
                    }
                    //  statusCode = httpResponse.StatusCode;
                }
                if (ex.Status != WebExceptionStatus.ProtocolError)
                    throw;

                // responseBody = strError;

                //erro = ex.ToString();
                // erro = "error";
                // return_message = "网络异常\r\n请求超时\r\n" + ex.StackTrace;
                //return_message = strError;
                return strError;
            }
            catch (Exception ex)
            {
                return "";
            }
        }
    }
}
