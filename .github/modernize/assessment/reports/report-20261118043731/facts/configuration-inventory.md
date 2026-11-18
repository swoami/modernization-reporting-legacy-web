# Configuration & Externalized Settings Inventory

Application settings are primarily in one `Web.config`, with build settings in the project and solution and a Windows registry lookup. No environment-specific configuration files or external secret-store integration were found.

## Configuration Sources

| Source | Type | Path/Location | Notes |
|---|---|---|---|
| Application and ASP.NET settings | XML | `Web.config` | Six app settings and web/server settings; credential-style hint masked below. |
| Build settings | MSBuild XML and solution | `Reporting.Legacy.Web.csproj`, `Reporting.Legacy.Web.sln` | Debug and Release configurations, framework references and web targets. |
| Package manifest | XML | `packages.config` | Empty. |
| Workstation name | Windows registry | `HKLM\SOFTWARE\SoftwareOne\EcomReporting`, value `ReportWorkstation` | Read at startup and export, with different fallback values. |
| Assessment settings | YAML | `.github\modernize\assessment\reports\assessment-config.yaml` | Assessment tooling settings, not application runtime settings. |
| Agent setup | GitHub Actions YAML | `.github\workflows\copilot-setup-steps.yml` | Manually triggered Windows checkout; not application deployment. |
| Documentation | Markdown | `README.md` | Describes local IIS Express use and a demo script that is not present in the repository. |

No external configuration repository, Key Vault, Vault, or other secret store reference was found.

## Build Profiles

| Profile | Activation | Purpose | Key Dependencies/Plugins |
|---|---|---|---|
| Debug, AnyCPU | Default project configuration or solution selection | Full debug symbols; optimization disabled; output `bin\` | Framework references, C# targets and conditional WebApplication targets. |
| Release, AnyCPU | Solution selection or MSBuild `Configuration=Release` | PDB-only debug information; optimization enabled; output `bin\` | Same references and targets. |

The project declares ToolsVersion 15.0, defaults `VisualStudioVersion` to 18.0, and conditionally locates Visual Studio web application targets. No profile-specific packages or compilation constants are declared.

## Runtime Profiles

| Profile | Activation Method | Config Files | Key Overrides |
|---|---|---|---|
| Shared/default | Standard ASP.NET configuration loading | `Web.config` | No environment-specific overrides or profile activation found. IIS/machine-level configuration outside the repository is unknown. |

## Properties Inventory

### Reporting.Legacy.Web

| Property Key | Default | Profiles | Source |
|---|---|---|---|
| `ReportingOdbcConnection` | `Driver={ODBC Driver 17 for SQL Server};Server=localhost\SQLEXPRESS;Database=EcomDemo;Trusted_Connection=Yes;TrustServerCertificate=Yes;` | Shared | `Web.config` appSettings; string. |
| `LegacyReportingCredentialHint` | `[MASKED]` | Shared | `Web.config` appSettings; credential-style placeholder; no code use found. |
| `ReportExportRoot` | `..\storage\legacy-report-exports` | Shared | `Web.config` appSettings; relative path resolved against application base directory. |
| `ReportArchiveRoot` | `\\legacy-fileserver\reports-archive` | Shared | `Web.config` appSettings; UNC path. |
| `ReportingMode` | `ClassicBatch` | Shared | `Web.config` appSettings; startup also uses `ClassicBatch` if absent. |
| `LegacyOperatorGroup` | `ECOM_REPORT_OPERATORS` | Shared | `Web.config` appSettings; string. |
| `ReportWorkstation` | `unregistered` at startup; machine name at export | Shared | Registry value; defaults supplied by `Global.asax.cs` and `Services\ReportExport.asmx.cs`. |
| `system.web/authentication@mode` | `Windows` | Shared | `Web.config`. |
| `system.web/authorization/allow@users` | `*` | Shared | `Web.config`; permissive authorization for demo. |
| `system.web/compilation@debug`, `@targetFramework` | `true`, `4.7.2` | Shared | `Web.config`. |
| `system.web/httpRuntime@targetFramework`, `@requestValidationMode` | `4.7.2`, `2.0` | Shared | `Web.config`. |
| `system.web/sessionState@mode`, `@timeout` | `InProc`, `60` minutes | Shared | `Web.config`. |
| `system.web/pages@controlRenderingCompatibilityVersion`, `@clientIDMode` | `3.5`, `AutoID` | Shared | `Web.config`. |
| `system.webServer/validation@validateIntegratedModeConfiguration` | `false` | Shared | `Web.config`. |
| `system.webServer/handlers/add` | `ReportExportAsmx`; verb `*`; `System.Web.Services.Protocols.WebServiceHandlerFactory, System.Web.Services`; resource type `Unspecified`; access `Script` | Shared | `Web.config`; handler mapping. |
| `analysisCoverage` | `full` | Assessment tooling | `.github\modernize\assessment\reports\assessment-config.yaml`. |
| `dotnet.assessmentDomains`, `dotnet.targetComputeServices` | `[cloud-readiness]`, `[Any]` | Assessment tooling | `.github\modernize\assessment\reports\assessment-config.yaml`. |

No environment-variable substitution or application-level property resolution chain is configured.

## Startup Parameters & Resource Requirements

| Service | JVM/Runtime Options | Memory | Instance Count |
|---|---|---|---|
| Reporting.Legacy.Web | .NET Framework 4.7.2; project enables IIS Express. No command-line startup options or environment overrides specified. | Not specified | Not specified |

The README gives an example IIS Express local address, but the repository contains no launch profile establishing the actual binding. No CPU limits or autoscaling settings were found.

## Startup Dependency Chain

1. IIS/IIS Express hosts the application; application startup reads the optional workstation registry value and attempts an Event Log write. Event Log write failures are ignored.
2. SQL Server and the ODBC driver are used on report/export requests, not explicitly waited for at startup; failed queries fall back to demo data. Export also needs a writable export directory at request time.

No startup ordering, readiness/health checks, retries, or explicit startup timeouts were found.

## Secrets & Sensitive Configuration

| Secret Reference | Type | Storage (masked) |
|---|---|---|
| `LegacyReportingCredentialHint` | Credential-style DSN hint; documented as placeholder | `Web.config`: `[MASKED]`; no application code consumption found. |
| `ReportingOdbcConnection` | Database connection configuration | `Web.config`; uses Windows integrated authentication, with no explicit password in the observed setting. |

No encryption or protected-configuration mechanism is configured in the repository. Paths and connection metadata are configuration, not proof of available resources.

### Secrets Provisioning Workflow

No secret provisioning pipeline, secret-store binding, managed identity, or RBAC configuration was found. The application reads `Web.config` directly; SQL Server access relies on the applicable Windows process identity, whose deployment setup and database permissions are not specified.

## Feature Flags

| Flag Name | Default | Controlled By |
|---|---|---|
| None found | — | No feature flag framework or conditional feature toggle found. `ReportingMode` is displayed and logged, but no behavioral branch on it was found. |

## Framework & Runtime Versions

| Component | Version | Source |
|---|---|---|
| .NET Framework / ASP.NET | 4.7.2 | `Reporting.Legacy.Web.csproj`, `Web.config` |
| ODBC Driver for SQL Server | 17 | `Web.config` |
| MSBuild project ToolsVersion | 15.0 | `Reporting.Legacy.Web.csproj` |
| Visual Studio solution | 17.0.31903.59; minimum 10.0.40219.1 | `Reporting.Legacy.Web.sln` |
| Web targets lookup | Defaults to Visual Studio 18.0; also searches a Visual Studio 2022 path | `Reporting.Legacy.Web.csproj` |
| NuGet packages | None declared | `packages.config` |
| Container image | None found | Repository |

