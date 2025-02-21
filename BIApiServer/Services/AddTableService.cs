using BIApiServer.Common.DbContexts;
using BIApiServer.Infrastructure;
using SqlSugar;
using System.Reflection;
using BIApiServer.Interfaces;

namespace BIApiServer.Services
{
    public class AddTableService : IScopedService
    {
        private static AppDbContext _db;

        public AddTableService(AppDbContext db)
        {
            _db = db;
        }

        public async void AddTable()
        {
            var ass = Assembly.GetAssembly(typeof(BaseTableEntity));

            var types = ass.GetTypes().Where(p => p.GetInterface(nameof(ITableEntity)) != null && !p.IsAbstract)
                .ToArray();

            foreach (var tp in types)
            {
                await InitTable(tp);
            }
        }

        public async Task InitTable(Type tp)
        {
            var attr = tp.GetCustomAttribute<SugarTable>();
            if (attr != null)
            {
                _db.BIDB.CodeFirst.InitTables(tp);
            }
        }
    }
}