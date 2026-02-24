using Serilog;
using Texnokaktus.ProgOlymp.Data.Infrastructure;
using Texnokaktus.ProgOlymp.Data.Middlewares;
using Texnokaktus.ProgOlymp.Data.Options;
using Texnokaktus.ProgOlymp.Data.Services;
using Texnokaktus.ProgOlymp.Data.Services.Abstractions;
using Texnokaktus.ProgOlymp.OpenTelemetry;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddGrpcClients(builder.Configuration);

builder.Services.AddTexnokaktusOpenTelemetry("DataService", null, null);

builder.Host.UseSerilog((context, configuration) => configuration.ReadFrom
                                                                 .Configuration(context.Configuration)
                                                                 .AddOpenTelemetrySupport("DataService"));

builder.Services.AddOptions<ContestRoutingOptions>().BindConfiguration(nameof(ContestRoutingOptions));

builder.Services.AddSingleton<IExcelService, ExcelService>();

var app = builder.Build();

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
