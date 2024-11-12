using MapsProxyApi.Data;
using MapsProxyApi.Domain.Dtos;
using MapsProxyApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

services.AddResponseCompression(opt =>
{
    opt.EnableForHttps = true;
    opt.Providers.Add<GzipCompressionProvider>();
});
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Optimal;
});

services.AddMemoryCache();
services.AddStackExchangeRedisCache(opt =>
{
    opt.Configuration = config["REDIS_CONNECTION_STRING"];
    opt.InstanceName = "local";
});

services.AddSingleton<CachedLimitDtoFactory>();
services.AddSingleton<MemoryCacheService>();
services.AddSingleton<IProxyService, ProxyServiceV1>();

services.AddDbContextFactory<AppDbContext>(opt =>
    opt.UseNpgsql(config["POSTGRES_CONNECTION_STRING"]));
services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(config["POSTGRES_CONNECTION_STRING"]));


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("allowFront");

app.UseResponseCompression();

app.MapGet("/api/proxy/testservices/{serviceName}/{*path}", 
    async ([FromServices] IProxyService service, 
    string serviceName, 
    string path,
    HttpContext context) =>
{
    var query = context.Request.QueryString.ToUriComponent();

    var response = await service.Proxy(serviceName, path, query);

    context.Response.ContentType = "application/json";
    return response;
})
.WithName("Proxy")
.WithOpenApi();

app.MapGet("/api/stats",
    async ([FromServices] AppDbContext context,
    [FromServices] IMemoryCache cache,
    string service = "A06_ATE_TE_WGS84") =>
{
    var result = await context.UsageLimits
        .Include(x => x.Service)
        .Where(x => x.Service.Name == service)
        .FirstOrDefaultAsync();

    var key = $"{service}";
    if (cache.TryGetValue(key, out CachedLimitDto limit))
        result.UsedTimes = limit.UsedTimes;

    return result;
})
.WithName("stats")
.WithOpenApi();

app.Run();