using System.Reflection;
using BIApiServer.Interfaces;

namespace BIApiServer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// 自动注册所有实现了 IScopedService、ITransientService 和 ISingletonService 接口的服务
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // 获取当前程序集中的所有类型
            var allTypes = assembly.GetTypes();

            // 注册 Scoped 服务
            var scopedTypes = allTypes.Where(t => typeof(IScopedService).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
            foreach (var type in scopedTypes)
            {
                services.AddScoped(type);
            }

            // 注册 Transient 服务
            var transientTypes = allTypes.Where(t => typeof(ITransientService).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
            foreach (var type in transientTypes)
            {
                services.AddTransient(type);
            }

            // 注册 Singleton 服务
            var singletonTypes = allTypes.Where(t => typeof(ISingletonService).IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
            foreach (var type in singletonTypes)
            {
                services.AddSingleton(type);
            }

            return services;
        }
    }
}