using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;

namespace DvcyRpcClient
{
    [XmlRpcUrl("http://localhost:5678/DvCy")]
    public interface IdvApi : IXmlRpcProxy
    {
        [XmlRpcMethod("Measure")]
        double Measure(string sn);
    }
    public class DvApi
    {
        private static IdvApi proxy = XmlRpcProxyGen.Create<IdvApi>();
        public static double Measure(string sn)
        {
            return proxy.Measure(sn);
        }
    }
}
