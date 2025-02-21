using Microsoft.IdentityModel.Logging;
using SqlSugar;
using BIApiServer.Common.DbContexts;
using AutoMapper;
using BIApiServer.Interfaces;
using BIApiServer.Models;

namespace BIApiServer.Services
{
    public class AddRefundService 
    {
        private readonly AppDbContext _db;
        private RefundService _refundServices { get; set; }

        public AddRefundService(RefundService refundServices, AppDbContext db)
        {
            _db = db;
            _refundServices = refundServices;
        }

        public async Task FlushRefundData()
        {
            try
            {
                var tableFlushTimes = await _db.BIDB.Queryable<T_TableFlushTimeData>()
                    .Where(s => s.TableName == "t_table_flush_time").ToListAsync();
                var tableFlushTime = tableFlushTimes.FirstOrDefault();
                if (tableFlushTime == null) return;
                var beginTime = tableFlushTime.LastFlushTime;

                List<T_RefundData> list = new List<T_RefundData>();

                //时间间隔14h

                var startTimeStr = beginTime.ToString("yyyy-MM-dd HH:mm:ss");
                var lastTime = Convert.ToDateTime(startTimeStr).AddHours(14);
                //结束时间 要小于当前时间5分钟
                if (lastTime >= DateTime.Now.AddMinutes(-5))
                {
                    lastTime = DateTime.Now.AddMinutes(-5);
                }

                var lastTimeStr = lastTime.ToString("yyyy-MM-dd HH:mm:ss");
                tableFlushTime.LastFlushTime = Convert.ToDateTime(lastTimeStr);

                var products = _db.BIDB.Queryable<T_RefundData>().GroupBy(s => s.Pk)
                    .ToList();


                var PurchaseOrdersResult = await _refundServices.GetRefundByTimeList(startTimeStr, lastTimeStr);
                if (PurchaseOrdersResult != null)
                {
                    //只取大于最初约定时间的数据
                    var items = PurchaseOrdersResult
                        .Where(s => Convert.ToDateTime(s.ApproveDate) >= tableFlushTime.FirstTime).ToList();

                    foreach (var item in items)
                    {
                        var product = products.Where(s => s.Pk == item.Pk).FirstOrDefault();
                        //筛选重复项

                        if (product == null)
                        {
                            T_RefundData genJournal = new T_RefundData()
                            {
                                Id = SnowFlakeSingle.instance.getID(),
                                Pk = item.Pk,
                                Sku = item.Sku,
                                ApproveDate = item.CreateTime,
                                CreateTime = DateTime.Now
                                //pk_flow = item.pk_flow,
                                //BoundTypeCode = item.logTypeCode,
                                //BoundTypeName = item.logTypeName,
                                //BoundTypeShortName = item.logTypeName.Contains("入库") ? "入库" : "出库",
                                //WareHouse = item.warehouseCode,
                                //WareHouseName = item.warehouseNode,
                                //ProCode = item.sku,
                                //Package = item.package,
                                //InQuantity = item.inQuantity,
                                //OutQuantity = item.outQuantity,
                                //CreateTime = DateTime.Now,
                                //LastFlushTime = lastTime,
                                //dr = item.dr,
                                //BusinessTime = Convert.ToDateTime(item.dbizdate),
                                //ProLine = product == null ? "" : product.Name,
                            };
                            list.Add(genJournal);
                            await _db.BIDB.Insertable(list).ExecuteCommandAsync();
                        }
                    }

                    // await SaveGenJournals(list);
                    await _db.BIDB.Storageable(tableFlushTime).ExecuteCommandAsync();
                }
            }
            catch (Exception ex)
            {
                LogHelper.LogExceptionMessage(ex);
            }
        }
    }
}