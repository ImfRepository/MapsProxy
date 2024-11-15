using Microsoft.EntityFrameworkCore;
using WebUI.Data;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;
// Add services to the container.
builder.Services.AddRazorPages();

services.AddDbContextFactory<AppDbContext>(opt =>
    opt.UseNpgsql(config["POSTGRES_CONNECTION_STRING"]));
services.AddDbContext<AppDbContext>(opt =>
    opt.UseNpgsql(config["POSTGRES_CONNECTION_STRING"]));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
