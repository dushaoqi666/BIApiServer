# BI API 项目

## 项目简介

BI API 是一个基于 .NET 8 开发的 Web API MVC项目，提供数据处理等功能。

## 技术栈

- .NET 8
- SqlSugar ORM
- Redis 缓存
- AutoMapper 对象映射
- Swagger/OpenAPI 接口文档
- Refit HTTP 客户端
- 全局异常处理
- 请求日志中间件
- CORS 跨域支持

## 项目结构

BIApiServer/
├── Common/ # 通用组件
│ ├── DbContexts/ # 数据库上下文
│ ├── Mappings/ # AutoMapper 配置
│ └── Config/ # 配置类
├── Controllers/ # API 控制器
│ ├── FileController # 文件处理控制器
│ ├── FileClientController # 文件客户端控制器
│ └── TestController # 测试接口控制器
├── Models/ # 数据模型
│ └── Dtos/ # 数据传输对象
├── Services/ # 业务服务
│ └── BackgroundServices/ # 后台服务
├── Interfaces/ # 接口定义
├── Middlewares/ # 中间件
│ ├── GlobalExceptionMiddleware # 全局异常处理
│ └── RequestLoggingMiddleware # 请求日志
├── Extensions/ # 扩展方法
├── Handlers/ # 处理器
└── Config/ # 配置文件目录

## 主要功能

- HTTP 客户端请求
- Redis 缓存支持
- 全局异常处理
- 请求日志记录
- Swagger API 文档

## 配置说明

- `appsettings.json`: 主配置文件
- `appsettings.Development.json`: 开发环境配置

## API 文档

启动项目后访问 `/swagger` 查看完整的 API 文档。

## 开发环境要求

- .NET 8 SDK
- Redis Server (可选，根据配置决定是否启用)

## 服务自动注册

服务类实现 Interfaces 下面的三大接口则会自动注册，无需在 Program 里显式注册：

### 生命周期类型

1. **Transient（瞬态）**
  
  - 每次请求该服务时都会创建一个新的实例
  - 生命周期最短，适合轻量级、无状态的服务
2. **Scoped（作用域）**
  
  - 在同一个作用域内（如一个 HTTP 请求）共享同一个实例
  - 跨作用域时会创建新的实例
  - 常用于 Web 应用中，确保每个请求有独立的服务实例
3. **Singleton（单例）**
  
  - 整个应用程序生命周期内只创建一个实例
  - 实例在第一次被请求时创建，并在应用程序关闭时销毁
  - 共享全局状态，适合需要长期存在的服务

## 定时任务系统

### 配置说明

基础路径配置在 appsettings.json 中：
{
"ApiSettings": {
"BaseUrl": "http://localhost:5166"
}
}

### 任务管理

- 通过 `api/TaskManagement` 添加的定时任务被持久化在 `taskconfig.json` 中
  
- 系统自动去重，避免重复任务
  
  ### 任务配置示例
  
  {
  ` "name": "定时获取文件列表", // 任务名称标识
   "url": "/api/file/list", // API 接口路径（必须以/开头）
   "method": "GET", // HTTP 方法：GET, POST, PUT, DELETE 等
   "parameters": { // 请求参数（JSON 格式）
   "pageIndex": 1,
   "pageSize": 10
   },
   "intervalSeconds": 5, // 执行间隔（秒）
   "isEnabled": true // 是否启用`
  }
  

### 参数说明

- **name**: 任务的唯一标识名称
- **url**: API 接口地址，必须以 `/` 开头
- **method**: 支持标准 HTTP 方法
- **parameters**: JSON 格式的请求参数
  - 分页示例: `{ "pageIndex": 1, "pageSize": 10 }`
  - 日期范围: `{ "startDate": "2024-01-01", "endDate": "2024-12-31" }`
  - 无参数: `{}`
- **intervalSeconds**: 执行间隔（秒）
  - 5: 每 5 秒执行一次
  - 60: 每分钟执行一次
  - 300: 每 5 分钟执行一次
- **isEnabled**: 任务启用状态
  - true: 启用
  - false: 禁用
