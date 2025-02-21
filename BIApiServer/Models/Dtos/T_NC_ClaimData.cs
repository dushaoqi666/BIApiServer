using BIApiServer.Infrastructure;
using SqlSugar;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Policy;
using Newtonsoft.Json;

namespace BIApiServer.Models
{
    public class T_NC_ClaimData: BaseTableEntity
    {
        [JsonConverter(typeof(LongConverter))]
        [SugarColumn(IsPrimaryKey = true, IsIdentity = false)]
        public long ID { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string Name { get; set; }

        [SugarColumn(ColumnDescription = "PK", IsNullable = false, ColumnDataType = "varchar(255)")]

        public string PK { get; set; }

        [SugarColumn(ColumnDescription = "pk_org", IsNullable = false, ColumnDataType = "varchar(255)")]
        public string pk_org { get; set; }

        /// <summary>
        /// Date
        /// </summary>
        [SugarColumn(ColumnDataType = "timestamp", DefaultValue = "'1900-01-01 00:00:00'::timestamp without time zone")]
        public DateTime Date { get; set; }

        /// <summary>
        /// SKU
        /// </summary>
        [SugarColumn(DefaultValue = "''::character varying", Length = 255, ColumnDescription = "SKU")]
        public string SKU { get; set; }

        /// <summary>
        /// 店铺
        /// </summary>
        [SugarColumn(DefaultValue = "''::character varying", Length = 255, ColumnDescription = "店铺")]
        public string ShopName { get; set; }

        /// <summary>
        ///
        /// </summary>
        [SugarColumn(DefaultValue = "''::character varying", Length = 255, ColumnDescription = "AccountUUId")]
        public string AccountUUId { get; set; }

        /// <summary>
        /// 国家编码
        /// </summary>
        [SugarColumn(DefaultValue = "''::character varying", Length = 255, ColumnDescription = "国家编码")]
        public string CuscountryCode { get; set; }

        /// <summary>
        /// 国家名称
        /// </summary>
        [SugarColumn(DefaultValue = "''::character varying", Length = 255, ColumnDescription = "国家名称")]
        public string CusCountryCnName { get; set; }

        /// <summary>
        /// eBay国家编码
        /// </summary>
        [SugarColumn(DefaultValue = "''::character varying", Length = 255, ColumnDescription = "eBay国家编码")]
        public string eBayCusCountryCode { get; set; }

        /// <summary>
        /// eBay国家名称
        /// </summary>
        [SugarColumn(DefaultValue = "''::character varying", Length = 255,ColumnDescription = "eBay国家名称")]
        public string eBayCusCountryName { get; set; }

        /// <summary>
        /// 仓库类型
        /// </summary>
        [SugarColumn(DefaultValue = "''::character varying", Length = 255, ColumnDescription = "仓库类型")]
        public string WareHouseType { get; set; }
        
        /// <summary>
        /// 仓库类型名称
        /// </summary>
        [SugarColumn(DefaultValue = "''::character varying", Length = 255, ColumnDescription = "仓库类型名称")]
        public string WareHouseTypeName { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        [SugarColumn(DefaultValue = "''::character varying", Length = 255, ColumnDescription = "仓库名称")]
        public string WareHouseName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        [SugarColumn(DefaultValue = "''::character varying", Length = 255, ColumnDescription = "币种")]
        public string CurrencyCode { get; set; }
    }
}