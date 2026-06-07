using System.Globalization;
using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Reporting.Legacy.Web.Services;

namespace Reporting.Legacy.Web.Controllers;

[ApiController]
public sealed class ReportsController : ControllerBase
{
    private readonly IReportDataService _reportDataService;
    private readonly IReportExportService _reportExportService;

    public ReportsController(IReportDataService reportDataService, IReportExportService reportExportService)
    {
        _reportDataService = reportDataService;
        _reportExportService = reportExportService;
    }

    [HttpGet("/Reports/MonthlySales.aspx")]
    public ContentResult MonthlySales([FromQuery] string? exportMessage = null)
    {
        HttpContext.Session.SetString("LastReportingPage", "MonthlySales");
        var (rows, loadMessage) = _reportDataService.LoadMonthlySalesRows();
        var message = exportMessage ?? loadMessage;

        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html><head><title>Monthly Sales Report</title>");
        html.AppendLine("<style>body { font-family: Segoe UI, Arial, sans-serif; margin: 32px; color: #1f2937; } table { border-collapse: collapse; min-width: 760px; } th, td { border: 1px solid #cbd5e1; padding: 8px 10px; text-align: left; } th { background: #e2e8f0; } .warning { color: #92400e; margin-bottom: 16px; }</style>");
        html.AppendLine("</head><body>");
        html.AppendLine("<h1>Monthly Sales Report</h1>");
        if (!string.IsNullOrWhiteSpace(message))
        {
            html.AppendLine($"<div class='warning'>{WebUtility.HtmlEncode(message)}</div>");
        }

        html.AppendLine("<table><thead><tr><th>CustomerId</th><th>OrderCount</th><th>TotalSales</th></tr></thead><tbody>");
        foreach (var row in rows)
        {
            html.AppendLine($"<tr><td>{WebUtility.HtmlEncode(row.CustomerId)}</td><td>{row.OrderCount}</td><td>{row.TotalSales.ToString("F2", CultureInfo.InvariantCulture)}</td></tr>");
        }

        html.AppendLine("</tbody></table>");
        html.AppendLine("<p><form method='post' action='/Reports/MonthlySales.aspx/export'><button type='submit'>Export Orders CSV</button></form></p>");
        html.AppendLine("</body></html>");

        return Content(html.ToString(), "text/html");
    }

    [HttpPost("/Reports/MonthlySales.aspx/export")]
    public IActionResult ExportFromPage()
    {
        var now = DateTime.UtcNow;
        var result = _reportExportService.ExportOrders(now.Year, now.Month);
        var message = $"{result.Status}: {result.OutputPath}";
        return Redirect($"/Reports/MonthlySales.aspx?exportMessage={Uri.EscapeDataString(message)}");
    }
}
