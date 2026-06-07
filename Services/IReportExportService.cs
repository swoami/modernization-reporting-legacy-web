using Reporting.Legacy.Web.Models;

namespace Reporting.Legacy.Web.Services;

public interface IReportExportService
{
    ExportResult ExportOrders(int year, int month);
}
