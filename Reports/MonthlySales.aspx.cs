using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Odbc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Reporting.Legacy.Web.Reports
{
    public partial class MonthlySales : Page
    {
        protected global::System.Web.UI.WebControls.Label Message;
        protected global::System.Web.UI.WebControls.GridView MonthlySalesGrid;
        protected global::System.Web.UI.WebControls.Button ExportButton;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["LastReportingPage"] = "MonthlySales";
                MonthlySalesGrid.DataSource = LoadRows();
                MonthlySalesGrid.DataBind();
            }
        }

        protected void ExportButton_Click(object sender, EventArgs e)
        {
            var service = new Services.ReportExport();
            var result = service.ExportOrders(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
            Message.Text = result.Status + ": " + result.OutputPath;
        }

        private IEnumerable<MonthlySalesRow> LoadRows()
        {
            var rows = new List<MonthlySalesRow>();
            var connectionString = ConfigurationManager.AppSettings["ReportingOdbcConnection"];
            var sql = "SELECT CustomerId, COUNT(*) AS OrderCount, SUM(Total) AS TotalSales FROM dbo.Orders GROUP BY CustomerId";

            try
            {
                using (var connection = new OdbcConnection(connectionString))
                using (var command = new OdbcCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            rows.Add(new MonthlySalesRow
                            {
                                CustomerId = reader["CustomerId"].ToString(),
                                OrderCount = Convert.ToInt32(reader["OrderCount"]),
                                TotalSales = Convert.ToDecimal(reader["TotalSales"])
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Message.Text = "Live ODBC reporting source is unavailable; showing local demo rows. " + ex.Message;
                rows.AddRange(CreateDemoRows());
            }

            if (rows.Count == 0)
            {
                Message.Text = "No live orders found; showing local demo rows.";
                rows.AddRange(CreateDemoRows());
            }

            return rows;
        }

        private static IEnumerable<MonthlySalesRow> CreateDemoRows()
        {
            return new[]
            {
                new MonthlySalesRow { CustomerId = "CONTOSO-WAREHOUSE", OrderCount = 6, TotalSales = 2489.44m },
                new MonthlySalesRow { CustomerId = "FABRIKAM-FULFILLMENT", OrderCount = 4, TotalSales = 1519.87m },
                new MonthlySalesRow { CustomerId = "NORTHWIND-OPS", OrderCount = 3, TotalSales = 789.92m }
            };
        }

        public sealed class MonthlySalesRow
        {
            public string CustomerId { get; set; }
            public int OrderCount { get; set; }
            public decimal TotalSales { get; set; }
        }
    }
}
