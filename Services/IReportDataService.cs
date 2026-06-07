using Reporting.Legacy.Web.Models;

namespace Reporting.Legacy.Web.Services;

public interface IReportDataService
{
    (IReadOnlyList<MonthlySalesRow> Rows, string? Message) LoadMonthlySalesRows();
    string BuildOrdersCsv();
}
