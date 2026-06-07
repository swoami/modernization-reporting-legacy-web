namespace Reporting.Legacy.Web.Options;

public sealed class ReportingOptions
{
    public const string SectionName = "Reporting";

    public string ReportingOdbcConnection { get; set; } = string.Empty;
    public string LegacyReportingCredentialHint { get; set; } = string.Empty;
    public string ReportExportRoot { get; set; } = "../storage/legacy-report-exports";
    public string ReportArchiveRoot { get; set; } = "";
    public string ReportingMode { get; set; } = "ClassicBatch";
    public string LegacyOperatorGroup { get; set; } = "ECOM_REPORT_OPERATORS";
}
