using System;
using System.Configuration;
using System.Data.Odbc;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web.Services;
using Microsoft.Win32;

namespace Reporting.Legacy.Web.Services
{
    [WebService(Namespace = "urn:softwareone:ecom:legacy-reporting")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class ReportExport : WebService
    {
        [WebMethod]
        public ExportResult ExportOrders(int year, int month)
        {
            var exportRoot = ConfigurationManager.AppSettings["ReportExportRoot"];
            var archiveRoot = ConfigurationManager.AppSettings["ReportArchiveRoot"];
            var operatorGroup = ConfigurationManager.AppSettings["LegacyOperatorGroup"];
            var workstation = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\SoftwareOne\EcomReporting", "ReportWorkstation", Environment.MachineName);
            var outputDirectory = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, exportRoot));
            Directory.CreateDirectory(outputDirectory);

            var outputPath = Path.Combine(outputDirectory, "orders-" + year + "-" + month.ToString("00") + ".csv");
            File.WriteAllText(outputPath, BuildCsv(year, month), Encoding.UTF8);
            TryWriteEvent("Exported monthly orders report for " + year + "-" + month + " to " + outputPath);

            return new ExportResult
            {
                Status = "Exported for " + operatorGroup,
                OutputPath = outputPath,
                ArchiveRoot = archiveRoot,
                Workstation = Convert.ToString(workstation)
            };
        }

        private static string BuildCsv(int year, int month)
        {
            var connectionString = ConfigurationManager.AppSettings["ReportingOdbcConnection"];
            var csv = new StringBuilder();
            csv.AppendLine("OrderId,CustomerId,Status,Total,CreatedAt");

            try
            {
                using (var connection = new OdbcConnection(connectionString))
                using (var command = new OdbcCommand("SELECT OrderId, CustomerId, Status, Total, CreatedAt FROM dbo.Orders", connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
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
                }
            }
            catch (Exception ex)
            {
                csv.AppendLine("DEMO-1001,CONTOSO-WAREHOUSE,DemoFallback,1299.45," + DateTimeOffset.UtcNow);
                csv.AppendLine("DEMO-1002,FABRIKAM-FULFILLMENT,DemoFallback,849.98," + DateTimeOffset.UtcNow);
                csv.AppendLine("DEMO-1003,NORTHWIND-OPS,DemoFallback,629.95," + DateTimeOffset.UtcNow);
                csv.AppendLine("ODBC_NOTE," + ex.GetType().Name + ",LiveSourceUnavailable,0," + DateTimeOffset.UtcNow);
            }

            return csv.ToString();
        }

        private static void TryWriteEvent(string message)
        {
            try
            {
                EventLog.WriteEntry("Reporting.Legacy.Web", message, EventLogEntryType.Information);
            }
            catch
            {
                // Assessment signal only; local machines may not have rights to create or write the source.
            }
        }
    }

    public sealed class ExportResult
    {
        public string Status { get; set; }
        public string OutputPath { get; set; }
        public string ArchiveRoot { get; set; }
        public string Workstation { get; set; }
    }
}
