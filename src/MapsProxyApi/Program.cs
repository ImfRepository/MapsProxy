using MapsProxyApi.Data;
using MapsProxyApi.Interfaces;
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
        policy.WithOrigins("http://maps-stats-ui:8080");
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

services.AddSingleton<IBookingService, BookingService>();
services.AddSingleton<IRecordingService, RecordingService>();
services.AddSingleton<IProxyService, ProxyService>();

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

app.UseCors("allowFront");

app.UseResponseCompression();

app.MapGet("/api/testservices/{serviceName}/{*path}", 
    async ([FromServices] IProxyService proxy, 
    string serviceName, 
    string path,
    HttpContext context) =>
{
    var query = context.Request.QueryString.ToUriComponent();

    var response = await proxy.GetAsync(serviceName, path, query);

    context.Response.ContentType = "application/json";
    return response;
})
.WithOpenApi();

app.MapGet("/api/stats/{serviceName}",
    async([FromServices] IRecordingService service,
    string serviceName) =>
{   
    var response = await service.GetAvailableRequestAmountTo(serviceName);
    return response;  
})
.WithOpenApi();

app.Run();