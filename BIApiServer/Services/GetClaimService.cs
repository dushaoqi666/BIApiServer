using BIApiServer.Models;
using Microsoft.IdentityModel.Logging;
using NetTaste;
using System.Net.Http;
using System.Security.Policy;
using System.Text.Json;
using BIApiServer.Interfaces;

namespace BIApiServer.Services
{
    public class GetClaimService: IScopedService
    {
        private readonly IHttpClientFactory httpClientFactory;
        public GetClaimService(IHttpClientFactory _httpClientFactory)
        {
            httpClientFactory = _httpClientFactory;
        }

        private string url = Furion.App.Configuration["NCUrl"]!;
        private string token = Furion.App.Configuration["NCToken"]!;
        public async Task<List<T_NC_ClaimData>> GetNCClaimList(string starttime, string endtime)
        {

            try
            {
                var method = "AfterSales/GetClaimByTime";
                var fullurl = Path.Combine(url!, method) + $"?token={token}&startDate={starttime}&endDate={endtime}";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, fullurl)
                {
                };

                var httpClient = httpClientFactory.CreateClient();
                var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                    var result = await JsonSerializer.DeserializeAsync<List<T_NC_ClaimData>>(contentStream);
                    return result;
                }
                else
                {
                    return new List<T_NC_ClaimData>();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogExceptionMessage(ex);
                return new List<T_NC_ClaimData>();
            }
        }
    }
}
