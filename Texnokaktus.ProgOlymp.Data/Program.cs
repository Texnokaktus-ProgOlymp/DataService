using Serilog;
using Texnokaktus.ProgOlymp.Data.Infrastructure;
using Texnokaktus.ProgOlymp.Data.Middlewares;
using Texnokaktus.ProgOlymp.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddGrpcClients(builder.Configuration);

builder.Services.AddTexnokaktusOpenTelemetry(builder.Configuration, "DataService", null, null);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

app.UseOpenTelemetryPrometheusScrapingEndpoint();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseRouting();

app.MapStaticAssets();

app.UseMiddleware<ClientIpLoggingMiddleware>();

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}")
   .WithStaticAssets();

await app.RunAsync();
