namespace Reporting.Legacy.Web.Models;

public sealed class MonthlySalesRow
{
    public string CustomerId { get; set; } = string.Empty;
    public int OrderCount { get; set; }
    public decimal TotalSales { get; set; }
}
