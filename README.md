# Reporting.Legacy.Web

ASP.NET Core / .NET 10 modernization of the legacy back-office reporting module.

## Run

```bash
dotnet run --project Reporting.Legacy.Web.csproj
```

Default local URL (Kestrel profile):

```text
http://localhost:5000
```

## Interfaces

- `GET /Default.aspx` opens the reporting landing page.
- `GET /Reports/MonthlySales.aspx` opens the monthly order summary report.
- `POST /Services/ReportExport.asmx/ExportOrders` exports an orders CSV file to the configured report drop path.

## Modernized implementation

- Migrated from ASP.NET Web Forms / ASMX to ASP.NET Core controllers with route-compatible endpoints.
- Migrated from `Web.config` / `ConfigurationManager` to `appsettings.json` + `IOptions<ReportingOptions>`.
- Preserved local demo fallback behavior when the ODBC source is unavailable.
- Replaced legacy EventLog startup/export signaling with ASP.NET Core logging.

## Configuration

`appsettings.json` contains the migrated settings:

- `Reporting:ReportingOdbcConnection`
- `Reporting:LegacyReportingCredentialHint`
- `Reporting:ReportExportRoot`
- `Reporting:ReportArchiveRoot`
- `Reporting:ReportingMode`
- `Reporting:LegacyOperatorGroup`
