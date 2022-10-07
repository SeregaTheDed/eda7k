using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using NuGet.Protocol.Core.Types;
using Microsoft.EntityFrameworkCore;
using eda7k.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<DbContext, DBConnection>();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.AllowSynchronousIO = true;

});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options => //CookieAuthenticationOptions
        {
            options.LoginPath = new PathString("/Account/Login");
        });
var app = builder.Build();
app.UseAuthentication();    // аутентификация
app.UseAuthorization();     // авторизация

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");



app.Run();
