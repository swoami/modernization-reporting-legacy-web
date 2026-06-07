# Configuration & Externalized Settings Inventory

This application uses a compact, file-based configuration model centered on `Web.config` with no external config server or secret vault integration detected.

## Configuration Sources

| Source | Type | Path/Location | Notes |
|---|---|---|---|
| Web.config | XML application/runtime config | `/Web.config` | Primary app settings and ASP.NET runtime/auth config |
| Reporting.Legacy.Web.csproj | MSBuild project config | `/Reporting.Legacy.Web.csproj` | Build target framework and debug/release settings |
| README.md | Operational documentation | `/README.md` | Documents local URL and interface entry points |

## Build Profiles

| Profile | Activation | Purpose | Key Dependencies/Plugins |
|---|---|---|---|
| Debug | Default local build | Enables debug symbols and non-optimized output | .NET Framework 4.7.2 Web Application targets |
| Release | Explicit configuration selection | Optimized build output | .NET Framework 4.7.2 Web Application targets |

## Runtime Profiles

| Profile | Activation Method | Config Files | Key Overrides |
|---|---|---|---|
| Default ASP.NET runtime | IIS/IIS Express hosting | Web.config | Windows auth mode, permissive local authorization, InProc session |

## Properties Inventory

| Property Key | Default | Profiles | Source |
|---|---|---|---|
| ReportingOdbcConnection | ODBC SQL Server connection string | Default | Web.config appSettings |
| LegacyReportingCredentialHint | DSN/credential placeholder hint | Default | Web.config appSettings |
| ReportExportRoot | `..\storage\legacy-report-exports` | Default | Web.config appSettings |
| ReportArchiveRoot | `\\legacy-fileserver\reports-archive` | Default | Web.config appSettings |
| ReportingMode | `ClassicBatch` | Default | Web.config appSettings |
| LegacyOperatorGroup | `ECOM_REPORT_OPERATORS` | Default | Web.config appSettings |

## Startup Parameters & Resource Requirements

| Service | JVM/Runtime Options | Memory | Instance Count |
|---|---|---|---|
| Reporting.Legacy.Web | ASP.NET .NET Framework runtime; no explicit startup flags in repo | Not specified | Not specified |

## Startup Dependency Chain

1. Reporting.Legacy.Web starts with Web.config-loaded settings.
2. Runtime availability depends on configured ODBC driver/data source for live reporting queries.
3. Export path availability affects CSV write success during export operations.

## Secrets & Sensitive Configuration

| Secret Reference | Type | Storage (masked) |
|---|---|---|
| ReportingOdbcConnection | DB connection string (integrated security shown) | Web.config `[MASKED]` |
| LegacyReportingCredentialHint | Placeholder credential-style hint | Web.config `[MASKED]` |
| ReportArchiveRoot | Network path to file share | Web.config `[MASKED]` |

### Secrets Provisioning Workflow

Secrets are currently file-provisioned via `Web.config` values deployed with the application. No managed identity, external secret store, or automated secret rotation workflow is present in the repository.

## Feature Flags

| Flag Name | Default | Controlled By |
|---|---|---|
| ReportingMode | ClassicBatch | Web.config appSetting |

## Framework & Runtime Versions

| Component | Version | Source |
|---|---|---|
| .NET Framework target | 4.7.2 | Reporting.Legacy.Web.csproj |
| ASP.NET Web Forms | .NET Framework stack | Reporting.Legacy.Web.csproj/Web.config |
| ASMX Web Services | .NET Framework stack | Services/ReportExport.asmx.cs + Web.config handler |
| MSBuild ToolsVersion | 15.0 | Reporting.Legacy.Web.csproj |
| Visual Studio tooling reference | v18.0 preferred (fallback v17.0) | Reporting.Legacy.Web.csproj |
