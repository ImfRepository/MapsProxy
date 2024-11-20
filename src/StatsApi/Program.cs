using Microsoft.EntityFrameworkCore;
using StatsApi.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var services = builder.Services;
var config = builder.Configuration;
// Add services to the container.

services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(config["POSTGRES_CONNECTION_STRING"]));

services.AddTransient<StatsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.MapGet("/api/limits", async (StatsService statsService) =>
{
    return await statsService.GetAllLimits();
})
.WithOpenApi();

app.MapPost("/api/limits/{serviceName}", async (StatsService statsService,
    string serviceName,
    int limit) =>
{
    if (await statsService.SetLimitFor(serviceName, limit))
        return Results.Ok();

    return Results.BadRequest();
})
.WithOpenApi();

app.MapPost("/api/limits/resetall", async (StatsService statsService) =>
{
    await statsService.ResetAllLimits();
})
.WithOpenApi();

app.Run();
