namespace Reporting.Legacy.Web.Models;

public sealed class ExportResult
{
    public string Status { get; set; } = string.Empty;
    public string OutputPath { get; set; } = string.Empty;
    public string ArchiveRoot { get; set; } = string.Empty;
    public string Workstation { get; set; } = string.Empty;
}
