using Reporting.Legacy.Web.Options;
using Reporting.Legacy.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ReportingOptions>(builder.Configuration.GetSection(ReportingOptions.SectionName));
builder.Services.AddSingleton<IReportDataService, ReportDataService>();
builder.Services.AddSingleton<IReportExportService, ReportExportService>();
builder.Services.AddControllers();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
var mode = app.Configuration[$"{ReportingOptions.SectionName}:ReportingMode"] ?? "ClassicBatch";
logger.LogInformation("Reporting.Legacy.Web started in {Mode} mode on {MachineName}", mode, Environment.MachineName);

app.UseSession();
app.MapControllers();

app.Run();
