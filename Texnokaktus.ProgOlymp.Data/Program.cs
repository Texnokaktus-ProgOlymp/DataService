using Microsoft.EntityFrameworkCore;
using Texnokaktus.ProgOlymp.Data.DataAccess;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddDataAccess(optionsBuilder => optionsBuilder.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDb"))
                                                               .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")
   .WithStaticAssets();

app.Run();
