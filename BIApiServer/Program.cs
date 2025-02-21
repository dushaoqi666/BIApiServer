using System.Reflection;
using Microsoft.OpenApi.Models;
using BIApiServer.Common;
using BIApiServer.Extensions;
using SqlSugar;
using StackExchange.Redis;
using BIApiServer.Middlewares;
using BIApiServer.Interfaces;
using Refit;
using BIApiServer.Handlers;
using BIApiServer.Common.Mappings;
using Swashbuckle.AspNetCore.SwaggerUI;
using BIApiServer.Common.DbContexts;
using BIApiServer.Services;
var builder = WebApplication.CreateBuilder(args);

// 初始化 SqlSugar 配置
SqlSugarConfig.Initialize(builder.Configuration);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "BI API",
        Version = "v1",
        Description = "BI API 接口文档",
        Contact = new OpenApiContact
        {
            Name = "du",
            Email = "569909513@qq.com"
        }
    });
    
    // 添加XML注释
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
    
    // 添加通用参数
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme, 
                    Id = "Bearer" 
                }
            },
            Array.Empty<string>()
        }
    });

    // 对action的标签进行分组
    c.TagActionsBy(api =>
    {
        if (api.GroupName != null)
        {
            return new[] { api.GroupName };
        }
        return new[] { api.ActionDescriptor.RouteValues["controller"] };
    });
});

// 添加 SqlSugar 服务
builder.Services.AddScoped<SqlSugarScope>(sp =>
{
    return SqlSugarConfig.GetInstance();
});

// 添加统一的数据库上下文
builder.Services.AddScoped<AppDbContext>();

// Redis 服务注册
if (builder.Configuration.GetValue<bool>("Redis:Enabled"))
{
    builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    {
        return RedisConfig.GetInstance(builder.Configuration);
    });
    builder.Services.AddScoped<IRedisService, RedisService>();
}
else
{
    builder.Services.AddScoped<IRedisService, NullRedisService>();
}

// 添加 HTTP 客户端日志处理器
builder.Services.AddTransient<HttpClientLoggingHandler>();

// 添加 Refit 客户端
builder.Services.AddRefitClient<IFileApi>(RefitConfig.GetDefaultSettings())
    .ConfigureHttpClient(c => 
        c.BaseAddress = new Uri(builder.Configuration["ApiSettings:ApiBaseUrlOne"]))
    .AddHttpMessageHandler<HttpClientLoggingHandler>()
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

// 添加 AutoMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// 添加CORS服务
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// 添加全局过滤器
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ApiResponseFilter>();
});

// 自动注册其他服务（包括 TaskManagementService）
builder.Services.AddApplicationServices();

// 注册任务执行器服务
builder.Services.AddHostedService<TaskExecutorService>();

// 配置 HttpClient
builder.Services.AddHttpClient("TaskExecutor").ConfigurePrimaryHttpMessageHandler(() =>
{
    var handler = new HttpClientHandler();
    if (builder.Environment.IsDevelopment())
    {
        handler.ServerCertificateCustomValidationCallback = 
            HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
    }
    return handler;
});
// //更新表结构
// new AddTableService().AddTable();
var app = builder.Build();

// 初始化数据库表
// using (var scope = app.Services.CreateScope())
// {
//     var tableService = scope.ServiceProvider.GetRequiredService<AddTableService>();
//      tableService.AddTable();
// }

// 启用静态文件
app.UseStaticFiles();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "BI API V1");
        c.DocumentTitle = "BI API 文档";
        c.RoutePrefix = "swagger";
        c.DefaultModelsExpandDepth(-1);
        c.DocExpansion(DocExpansion.List);
        c.EnableFilter();
        c.EnableDeepLinking();
        c.InjectStylesheet("/swagger-ui/custom.css");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// 添加全局异常处理中间件
app.UseMiddleware<GlobalExceptionMiddleware>();

// 添加请求日志中间件
app.UseMiddleware<RequestLoggingMiddleware>();

// 使用CORS
app.UseCors("AllowAll");

app.Run();
