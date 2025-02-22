using SqlSugar;

namespace BIApiServer.Common.DbContexts
{
    public class AppDbContext
    {
        private readonly SqlSugarScope _db;

        public AppDbContext(SqlSugarScope db)
        {
            _db = db;
        }

        /// <summary>
        /// 默认数据库
        /// </summary>
        public SqlSugarProvider Default => _db.GetConnection("Default");

        /// <summary>
        /// 第二个数据库
        /// </summary>
        // public SqlSugarProvider DB2 => _db.GetConnection("DB2");
        
        
        /// <summary>
        /// 获取指定数据库连接
        /// </summary>
        public SqlSugarProvider GetConnection(string configId = "Default")
        {
            return _db.GetConnection(configId);
        }

        #region 事务管理
        /// <summary>
        /// 开启默认数据库事务
        /// </summary>
        public void BeginTran() => _db.BeginTran();

        /// <summary>
        /// 提交默认数据库事务
        /// </summary>
        public void CommitTran() => _db.CommitTran();

        /// <summary>
        /// 回滚默认数据库事务
        /// </summary>
        public void RollbackTran() => _db.RollbackTran();

        /// <summary>
        /// 开启多库事务
        /// </summary>
        public void BeginTranAll()
        {
            _db.BeginTran();
        }

        /// <summary>
        /// 提交多库事务
        /// </summary>
        public void CommitTranAll()
        {
            try
            {
                _db.CommitTran();
            }
            catch
            {
                RollbackTranAll();
                throw;
            }
        }

        /// <summary>
        /// 回滚多库事务
        /// </summary>
        public void RollbackTranAll()
        {
            _db.RollbackTran();
        }
        #endregion
    }
}