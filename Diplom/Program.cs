using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Diplom.Data;
using Diplom.SeedData;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DiplomContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DiplomContext") ?? throw new InvalidOperationException("Connection string 'DiplomContext' not found.")));

builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    SeedData.Initialize(services);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Devices}/{action=Index}/{id?}");

app.Run();
