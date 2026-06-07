using System.Data.Odbc;
using System.Text;
using Reporting.Legacy.Web.Models;
using Reporting.Legacy.Web.Options;

namespace Reporting.Legacy.Web.Services;

public sealed class ReportDataService : IReportDataService
{
    private readonly ReportingOptions _options;

    public ReportDataService(Microsoft.Extensions.Options.IOptions<ReportingOptions> options)
    {
        _options = options.Value;
    }

    public (IReadOnlyList<MonthlySalesRow> Rows, string? Message) LoadMonthlySalesRows()
    {
        var rows = new List<MonthlySalesRow>();
        var sql = "SELECT CustomerId, COUNT(*) AS OrderCount, SUM(Total) AS TotalSales FROM dbo.Orders GROUP BY CustomerId";
        string? message = null;

        try
        {
            using var connection = new OdbcConnection(_options.ReportingOdbcConnection);
            using var command = new OdbcCommand(sql, connection);
            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                rows.Add(new MonthlySalesRow
                {
                    CustomerId = Convert.ToString(reader["CustomerId"]) ?? string.Empty,
                    OrderCount = Convert.ToInt32(reader["OrderCount"]),
                    TotalSales = Convert.ToDecimal(reader["TotalSales"])
                });
            }
        }
        catch (Exception ex)
        {
            message = $"Live ODBC reporting source is unavailable; showing local demo rows. {ex.Message}";
            rows.AddRange(CreateDemoRows());
        }

        if (rows.Count == 0)
        {
            message = "No live orders found; showing local demo rows.";
            rows.AddRange(CreateDemoRows());
        }

        return (rows, message);
    }

    public string BuildOrdersCsv()
    {
        var csv = new StringBuilder();
        csv.AppendLine("OrderId,CustomerId,Status,Total,CreatedAt");

        try
        {
            using var connection = new OdbcConnection(_options.ReportingOdbcConnection);
            using var command = new OdbcCommand("SELECT OrderId, CustomerId, Status, Total, CreatedAt FROM dbo.Orders", connection);
            connection.Open();
            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                csv.AppendLine(string.Join(",",
                    reader["OrderId"],
                    reader["CustomerId"],
                    reader["Status"],
                    reader["Total"],
                    reader["CreatedAt"]));
            }
        }
        catch (Exception ex)
        {
            csv.AppendLine($"DEMO-1001,CONTOSO-WAREHOUSE,DemoFallback,1299.45,{DateTimeOffset.UtcNow:O}");
            csv.AppendLine($"DEMO-1002,FABRIKAM-FULFILLMENT,DemoFallback,849.98,{DateTimeOffset.UtcNow:O}");
            csv.AppendLine($"DEMO-1003,NORTHWIND-OPS,DemoFallback,629.95,{DateTimeOffset.UtcNow:O}");
            csv.AppendLine($"ODBC_NOTE,{ex.GetType().Name},LiveSourceUnavailable,0,{DateTimeOffset.UtcNow:O}");
        }

        return csv.ToString();
    }

    private static IEnumerable<MonthlySalesRow> CreateDemoRows()
    {
        return
        [
            new MonthlySalesRow { CustomerId = "CONTOSO-WAREHOUSE", OrderCount = 6, TotalSales = 2489.44m },
            new MonthlySalesRow { CustomerId = "FABRIKAM-FULFILLMENT", OrderCount = 4, TotalSales = 1519.87m },
            new MonthlySalesRow { CustomerId = "NORTHWIND-OPS", OrderCount = 3, TotalSales = 789.92m }
        ];
    }
}
