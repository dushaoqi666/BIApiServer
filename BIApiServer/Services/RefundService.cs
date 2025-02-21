using BIApiServer.Models;
using Microsoft.IdentityModel.Logging;
using NetTaste;
using System.Net.Http;
using System.Security.Policy;
using System.Text.Json;
using BIApiServer.Interfaces;

namespace BIApiServer.Services
{
    public class RefundService: IScopedService
    {
        private string url = Furion.App.Configuration["NCUrl"]!;
        private string token = Furion.App.Configuration["NCToken"]!;
      
        private readonly IHttpClientFactory httpClientFactory;
        public RefundService(IHttpClientFactory _httpClientFactory)
        {
            httpClientFactory = _httpClientFactory;
        }
        /// <summary>
        /// 获取退款利润
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public async Task<List<T_RefundData?>> GetRefundByTimeList(string starttime, string endtime)
        {
            try
            {
                var method = "AfterSales/GetRefundByTime";
                var fullurl = Path.Combine(url!, method) + $"?token={token}&startDate={starttime}&endDate={endtime}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, fullurl)
                {
                };

                var httpClient = httpClientFactory.CreateClient();
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    var result = await JsonSerializer.DeserializeAsync<ApiResponse<T_RefundData>>(contentStream);
                    return result.data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogExceptionMessage(ex);
                return null;
            }
        }

        /// <summary>
        /// 获取退货利润
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public async Task<List<T_RefundData?>> GetReturnByTimeList(string starttime, string endtime)
        {
            try
            {
                var method = "AfterSales/GetReturnByTime";
                var fullurl = Path.Combine(url!, method) + $"?token={token}&startDate={starttime}&endDate={endtime}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, fullurl)
                {
                };

                var httpClient = httpClientFactory.CreateClient();
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    var result = await JsonSerializer.DeserializeAsync<ApiResponse<T_RefundData>>(contentStream);
                    return result.data;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogExceptionMessage(ex);
                return null;
            }
        }


    }
}
