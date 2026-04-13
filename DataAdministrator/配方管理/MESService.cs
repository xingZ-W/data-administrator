using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace DataAdministrator.配方管理
{
    /// <summary>
    /// MES 服务 - 配方和产品信息接口
    /// </summary>
    public class MESService
    {
        private readonly Form1 _form;
        private readonly string _baseUrl;
        private readonly string _logPrefix;
        
        public MESService(Form1 form)
        {
            _form = form;
            _logPrefix = $"[MES 服务-{DateTime.Now:HH:mm:ss}]";
            
            // 从配置获取 MES 地址
            _baseUrl = GetMESBaseUrl();
        }
        
        /// <summary>
        /// 获取产品基本信息
        /// </summary>
        public async Task<ProductInfo> GetProductInfo(string qrCode)
        {
            try
            {
                var url = $"{_baseUrl}/api/v1/product/info?qrCode={qrCode}";
                
                var response = await HttpGetAsync(url);
                var json = JObject.Parse(response);
                
                if (json["status"]?.ToString() == "200")
                {
                    var data = json["data"];
                    return new ProductInfo
                    {
                        QRCode = qrCode,
                        ModelId = data["modelId"]?.ToString(),
                        WorkOrderNo = data["workOrderNo"]?.ToString(),
                        BatchNo = data["batchNo"]?.ToString(),
                        CustomerCode = data["customerCode"]?.ToString()
                    };
                }
                else
                {
                    LogError($"MES 返回异常：{response}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogError($"获取产品信息失败：{ex.Message}", ex);
                return null;
            }
        }
        
        /// <summary>
        /// 下载配方
        /// </summary>
        public async Task<Recipe> DownloadRecipe(string modelId)
        {
            try
            {
                var url = $"{_baseUrl}/api/v1/recipe/download/{modelId}";
                
                var response = await HttpGetAsync(url);
                var json = JObject.Parse(response);
                
                if (json["status"]?.ToString() == "200")
                {
                    var data = json["data"];
                    
                    var recipe = new Recipe
                    {
                        RecipeId = data["recipeId"]?.ToString(),
                        ProductModel = data["productModel"]?.ToString(),
                        Version = data["version"]?.ToString(),
                        EffectiveDate = ParseDateTime(data["effectiveDate"]),
                        ApprovedBy = data["approvedBy"]?.ToString(),
                        Remark = data["remark"]?.ToString()
                    };
                    
                    // 解析参数
                    if (data["parameters"] != null)
                    {
                        recipe.Parameters = data["parameters"].ToObject<Dictionary<string, ProcessParameter>>();
                    }
                    
                    // 解析阈值
                    if (data["qualityThresholds"] != null)
                    {
                        recipe.QualityThresholds = data["qualityThresholds"].ToObject<QualityThresholds>();
                    }
                    
                    Log($"配方下载成功：{recipe.RecipeId}");
                    return recipe;
                }
                else
                {
                    LogError($"MES 返回异常：{response}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogError($"下载配方失败：{ex.Message}", ex);
                return null;
            }
        }
        
        /// <summary>
        /// 上传配方使用记录
        /// </summary>
        public async Task UploadUsage(string productSN, string recipeId)
        {
            try
            {
                var url = $"{_baseUrl}/api/v1/recipe/usage";
                
                var requestData = new
                {
                    productSN = productSN,
                    recipeId = recipeId,
                    useTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    operatorName = _form.MySignl.MES_operator
                };
                
                await HttpPostAsync(url, JObject.FromObject(requestData).ToString());
                
                Log($"配方使用记录已上传：{recipeId}");
            }
            catch (Exception ex)
            {
                LogError($"上传使用记录失败：{ex.Message}", ex);
                // 不抛异常，只记录日志
            }
        }
        
        #region HTTP 辅助方法
        
        /// <summary>
        /// HTTP GET
        /// </summary>
        private async Task<string> HttpGetAsync(string url)
        {
            return await Task.Run(() =>
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "application/json";
                request.Timeout = 5000;
                
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            });
        }
        
        /// <summary>
        /// HTTP POST
        /// </summary>
        private async Task<string> HttpPostAsync(string url, string jsonData)
        {
            return await Task.Run(() =>
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = 3000;
                
                var buffer = Encoding.UTF8.GetBytes(jsonData);
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(buffer, 0, buffer.Length);
                }
                
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var reader = new System.IO.StreamReader(response.GetResponseStream()))
                {
                    return reader.ReadToEnd();
                }
            });
        }
        
        #endregion
        
        #region 辅助方法
        
        /// <summary>
        /// 获取 MES 基础 URL
        /// </summary>
        private string GetMESBaseUrl()
        {
            try
            {
                // 从配置文件读取 MES 地址
                for (int i = 0; i < _form.Set_Ui.dataGridView3.Rows.Count; i++)
                {
                    var row = DataGridViewClass.GetRowsData(_form.Set_Ui.dataGridView3, i);
                    if (row[1]?.ToString() == "MES 基础 URL")
                    {
                        return row[2]?.ToString();
                    }
                }
                
                // 默认地址
                return "http://192.100.1.1:8001";
            }
            catch
            {
                return "http://192.100.1.1:8001";
            }
        }
        
        /// <summary>
        /// 解析 DateTime
        /// </summary>
        private DateTime ParseDateTime(object obj)
        {
            if (obj == null || string.IsNullOrEmpty(obj?.ToString()))
                return DateTime.MinValue;
            
            DateTime result;
            if (DateTime.TryParse(obj.ToString(), out result))
                return result;
            
            return DateTime.MinValue;
        }
        
        /// <summary>
        /// 日志
        /// </summary>
        private void Log(string message)
        {
            _form.P_Class.TxtLog($"{_logPrefix} {message}", 2);
        }
        
        /// <summary>
        /// 错误日志
        /// </summary>
        private void LogError(string message, Exception ex = null)
        {
            var errorMsg = $"{_logPrefix} [ERROR] {message}";
            if (ex != null)
                errorMsg += $"\n异常详情：{ex.Message}\n{ex.StackTrace}";
            
            _form.P_Class.TxtLog(errorMsg, 1);
        }
        
        #endregion
    }
    
    /// <summary>
    /// 产品基本信息
    /// </summary>
    public class ProductInfo
    {
        public string QRCode { get; set; }
        public string ModelId { get; set; }
        public string WorkOrderNo { get; set; }
        public string BatchNo { get; set; }
        public string CustomerCode { get; set; }
    }
}
