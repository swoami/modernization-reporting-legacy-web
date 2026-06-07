# Reporting.Legacy.Web

ASP.NET Web Forms / .NET Framework 4.7.2 back-office reporting module. This module is intentionally old-school so modernization assessment has a clear long-term-bet candidate.

## Run

Run the full local demo from the repository root:

```powershell
.\scripts\run-local.ps1
```

You can also open `Reporting.Legacy.Web.sln` in Visual Studio and run with IIS Express.

Expected local URL:

```text
http://localhost:44330
```

## Interfaces

- `GET /Default.aspx` opens the reporting landing page.
- `GET /Reports/MonthlySales.aspx` opens the monthly order summary report.
- `POST /Services/ReportExport.asmx/ExportOrders` exports an orders CSV file to the configured report drop path.

## Legacy Signals

- ASP.NET Web Forms and ASMX SOAP.
- Windows authentication mode and InProc session state. Anonymous users are allowed for local demo browsers that cannot negotiate Windows credentials.
- ODBC access through `System.Data.Odbc`.
- Local demo fallback rows when the live ODBC source is unavailable.
- Registry reads through `Microsoft.Win32.Registry`.
- Windows EventLog writes.
- Local and UNC file paths for report export/archive roots.
- Placeholder credential-style ODBC connection string in `Web.config`.

## Configuration

`Web.config` contains the assessment-facing keys:

- `ReportingOdbcConnection`
- `LegacyReportingCredentialHint`
- `ReportExportRoot`
- `ReportArchiveRoot`
- `ReportingMode`
- `LegacyOperatorGroup`

The default ODBC connection targets `localhost\SQLEXPRESS` / `EcomDemo` with integrated security. `LegacyReportingCredentialHint` keeps the old DSN and placeholder credential signal visible for assessment. The UNC archive path is also a placeholder. None of these resources are required for the project to compile, and the report page displays demo rows if the live source is unavailable.
