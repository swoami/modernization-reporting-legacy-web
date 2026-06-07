using Microsoft.AspNetCore.Mvc;
using Reporting.Legacy.Web.Models;
using Reporting.Legacy.Web.Services;

namespace Reporting.Legacy.Web.Controllers;

[ApiController]
[Route("Services/ReportExport.asmx")]
public sealed class ReportExportController : ControllerBase
{
    private readonly IReportExportService _reportExportService;

    public ReportExportController(IReportExportService reportExportService)
    {
        _reportExportService = reportExportService;
    }

    [HttpPost("ExportOrders")]
    public ActionResult<ExportResult> ExportOrders([FromForm] int year, [FromForm] int month)
    {
        return _reportExportService.ExportOrders(year, month);
    }

    [HttpGet("ExportOrders")]
    public ActionResult<ExportResult> ExportOrdersGet([FromQuery] int year, [FromQuery] int month)
    {
        return _reportExportService.ExportOrders(year, month);
    }
}
