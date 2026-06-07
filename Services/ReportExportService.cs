using System.Text;
using Reporting.Legacy.Web.Models;
using Reporting.Legacy.Web.Options;

namespace Reporting.Legacy.Web.Services;

public sealed class ReportExportService : IReportExportService
{
    private readonly ReportingOptions _options;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IReportDataService _reportDataService;
    private readonly ILogger<ReportExportService> _logger;

    public ReportExportService(
        Microsoft.Extensions.Options.IOptions<ReportingOptions> options,
        IHostEnvironment hostEnvironment,
        IReportDataService reportDataService,
        ILogger<ReportExportService> logger)
    {
        _options = options.Value;
        _hostEnvironment = hostEnvironment;
        _reportDataService = reportDataService;
        _logger = logger;
    }

    public ExportResult ExportOrders(int year, int month)
    {
        var exportRoot = _options.ReportExportRoot;
        var outputDirectory = Path.GetFullPath(Path.Combine(_hostEnvironment.ContentRootPath, exportRoot));
        Directory.CreateDirectory(outputDirectory);

        var outputPath = Path.Combine(outputDirectory, $"orders-{year}-{month:00}.csv");
        File.WriteAllText(outputPath, _reportDataService.BuildOrdersCsv(), Encoding.UTF8);

        _logger.LogInformation("Exported monthly orders report for {Year}-{Month} to {OutputPath}", year, month, outputPath);

        return new ExportResult
        {
            Status = $"Exported for {_options.LegacyOperatorGroup}",
            OutputPath = outputPath,
            ArchiveRoot = _options.ReportArchiveRoot,
            Workstation = Environment.MachineName
        };
    }
}
