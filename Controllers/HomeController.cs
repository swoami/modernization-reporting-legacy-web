using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Reporting.Legacy.Web.Options;

namespace Reporting.Legacy.Web.Controllers;

[ApiController]
public sealed class HomeController : ControllerBase
{
    private readonly ReportingOptions _options;

    public HomeController(Microsoft.Extensions.Options.IOptions<ReportingOptions> options)
    {
        _options = options.Value;
    }

    [HttpGet("/")]
    public IActionResult Root()
    {
        return Redirect("/Default.aspx");
    }

    [HttpGet("/Default.aspx")]
    public ContentResult DefaultPage()
    {
        HttpContext.Session.SetString("LastReportingPage", "Default");
        var operatorName = string.IsNullOrWhiteSpace(HttpContext.User.Identity?.Name)
            ? "Local demo operator"
            : HttpContext.User.Identity!.Name;

        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html><head><title>Legacy Reporting Console</title>");
        html.AppendLine("<style>body { font-family: Segoe UI, Arial, sans-serif; margin: 32px; color: #1f2937; } .panel { border: 1px solid #cbd5e1; padding: 20px; max-width: 820px; } code { background: #f1f5f9; padding: 2px 4px; }</style>");
        html.AppendLine("</head><body><div class='panel'>");
        html.AppendLine("<h1>Legacy Reporting Console</h1>");
        html.AppendLine("<p>Back-office order and invoice reporting for the warehouse supply store.</p>");
        html.AppendLine($"<p><strong>Operator:</strong> {WebUtility.HtmlEncode(operatorName)}</p>");
        html.AppendLine($"<p><strong>Mode:</strong> {WebUtility.HtmlEncode(_options.ReportingMode)}</p>");
        html.AppendLine($"<p><strong>Archive root:</strong> <code>{WebUtility.HtmlEncode(_options.ReportArchiveRoot)}</code></p>");
        html.AppendLine("<p><a href='/Reports/MonthlySales.aspx'>Open monthly sales report</a></p>");
        html.AppendLine("</div></body></html>");

        return Content(html.ToString(), "text/html");
    }
}
