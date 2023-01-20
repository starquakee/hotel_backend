using Essensoft.Paylink.Alipay;
using HotelManagement;
using HotelManagement.ErrorHandling;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Models;
using System.Configuration;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddAlipay();
builder.Configuration.GetSection("Alipay");

// 配置Server基本运行参数
builder.WebHost.UseKestrel(kestrelOptions =>
{
    kestrelOptions.ListenAnyIP(8080, portConfigure => portConfigure.Protocols = HttpProtocols.Http1);

    kestrelOptions.Limits.MinResponseDataRate = null;
    kestrelOptions.Limits.MaxRequestBodySize = 4L * 1048576;
});

// 配置接口监听，及序列化参数
builder.Services.AddCors(setupAction =>
{
    setupAction.AddPolicy("global", configue =>
    {
        configue.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

builder.Services.AddControllers(configure =>
{
    configure.Filters.Add(typeof(BackendExceptionFilter));
}).AddJsonOptions(configure =>
{
    configure.JsonSerializerOptions.AllowTrailingCommas = true;
    configure.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowReadingFromString;
    configure.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
    configure.JsonSerializerOptions.IgnoreReadOnlyFields = true;
    configure.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    configure.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

    if (builder.Environment.IsDevelopment())
        configure.JsonSerializerOptions.WriteIndented = true;
});

// 注入数据库连接池
builder.Services.AddDbContextPool<MyDbContext>(optionsAction =>
{
    optionsAction.UseMySql(builder.Configuration.GetConnectionString("MySQL"), ServerVersion.Parse("8.0"), mysqlAction =>
    {
        mysqlAction.MigrationsAssembly("HotelManagement");
        mysqlAction.UseMicrosoftJson();
    });

    if (builder.Environment.IsDevelopment())
    {
        optionsAction.EnableDetailedErrors();
        optionsAction.EnableSensitiveDataLogging();
    }
});


var app = builder.Build();
// 自动映射路由路径
app.UseCors("global");
app.MapControllers();

await app.RunAsync();
