using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JP_DLL;
using System.Threading;
using Newtonsoft.Json.Linq;
using System.IO.Ports;
using HslCommunication.Profinet.Siemens;
using DataAdministrator;

namespace DataAdministraction.公用函数类
{  
    public  class keyence_scan
    {
        Form1 form;
        public keyence_scan(Form1 form)
        {
            this.form = form;
        }
        public async Task<string> GetTcpScannerCodeAsync(string strIP, int port, int timeOut)
        {
            JPTcpClient mJPTcpClient = new JPTcpClient();
            try
            {
                mJPTcpClient.HostIp = strIP;
                mJPTcpClient.HostPort = port;
                await Task.Run<bool>(() => mJPTcpClient.Open());
                mJPTcpClient.ReadTimeout = timeOut;
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开扫码枪失败:" + ex.Message);
                return "";
            }
            string scanCode = "";
            bool scanResult = true;
            try
            {
                scanCode = await Task.Run<string>(() => mJPTcpClient.SendRequest_j("LON\r"));
            }
            catch (Exception ex)
            {
                form.P_Class.TxtLog("当前扫码失败:" + ex.Message,3);
                scanResult = false;
            }
            if (!scanResult)
            {
                try
                {
                    mJPTcpClient.Send("LOFF\r");
                }
                catch (Exception ex)
                {
                    form.P_Class.TxtLog("取消扫码失败:" + ex.Message,1);
                }
            }
            if (mJPTcpClient != null)
            {
                mJPTcpClient.Close();
            }
            return scanCode;
        }

    }

}
