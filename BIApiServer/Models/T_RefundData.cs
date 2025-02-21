using BIApiServer.Infrastructure;
using SqlSugar;
using System.Text.Json.Serialization;

namespace BIApiServer.Models
{
    [SugarTable("t_refund", "退款数据表")]
    public class T_RefundData : BaseTableEntity
    {
        [JsonConverter(typeof(LongConverter))]
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public long Id { get; set; }
        [JsonPropertyName("pk")]
        public string Pk { get; set; }

        [JsonPropertyName("approvedate")]
        public DateTime ApproveDate { get; set; }

        [JsonPropertyName("sku")]
        public string Sku { get; set; }


      
    }
 
}
