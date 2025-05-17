using Microsoft.EntityFrameworkCore;
using PatientTrackingSite.Models;
using System;

var builder = WebApplication.CreateBuilder(args);
 
builder.Services.AddSession(options =>      //Session ayarlar� i�in 
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Oturum s�resi
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();


// Database Context configuration

builder.Services.AddDbContext<PTSDBContext>(options => { options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon")); });


var app = builder.Build();





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

app.UseSession(); //  Buras� �nemli

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=PatientHome}/{action=Index}/{id?}");

app.Run();
