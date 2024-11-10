using MapsProxyApi.Data;
using MapsProxyApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

var config = builder.Configuration;

services.AddCors(options =>
{
    options.AddPolicy("allowFront", policy =>
    {
        policy.WithOrigins("http://localhost:3000");
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});

services.AddResponseCompression(opt =>
{
    opt.EnableForHttps = true;
    opt.Providers.Add<GzipCompressionProvider>();
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

//services.AddDistributedMemoryCache();

services.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = config["REDIS_CONNECTION_STRING"];
    opt.InstanceName = "local";
});

services.AddTransient<ProxyTestsService>();

services.AddDbContext<DbContext, AppDbContext>(opt =>
    opt.UseNpgsql(config["POSTGRES_CONNECTION_STRING"])
    .EnableSensitiveDataLogging());

var app = builder.Build();

app.UseHttpsRedirection();

app.UseCors("allowFront");

app.UseResponseCompression();

app.MapGet("/api/proxy/testservices/{serviceName}/{*path}", 
    async ([FromServices] ProxyTestsService service, 
    string serviceName, 
    string path, 
    string token, // From query
    HttpContext context) =>
{
    var query = context.Request.QueryString.ToUriComponent();

    var response = await service.Proxy(token, serviceName, path, query);

    context.Response.ContentType = "application/json";
    return response;
});


app.Run();