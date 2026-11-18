# Architecture Diagram

Reporting.Legacy.Web is a single ASP.NET Web Forms application with an ASMX export endpoint. Its reporting logic accesses SQL Server directly and falls back to demo data when the live source fails.

## Application Architecture

```mermaid
flowchart TD
    subgraph Client["Client Layer"]
        Browser["Web browser"]
        ServiceClient["ASMX service client"]
    end
    subgraph App["Application Layer - ASP.NET on .NET Framework 4.7.2"]
        Forms["Web Forms pages"]
        Export["ASMX report export"]
        Auth["Windows authentication and authorization"]
        Session["In-process session"]
    end
    subgraph Data["Data and File Access"]
        ODBC["System Data ODBC"]
        Demo["Demo fallback rows"]
        Files["CSV export files"]
    end
    subgraph External["External Resources"]
        SQL[("SQL Server EcomDemo")]
        Registry["Windows Registry"]
        EventLog["Windows Event Log"]
        Archive["Configured UNC archive path"]
    end

    Browser -->|"HTTP page requests"| Auth
    ServiceClient -->|"ASMX requests"| Auth
    Auth -->|"allows requests"| Forms
    Auth -->|"allows requests"| Export
    Forms -->|"tracks last page"| Session
    Forms -->|"invokes export in process"| Export
    Forms -->|"reads order summaries"| ODBC
    Export -->|"reads orders"| ODBC
    ODBC -->|"SQL queries via configured driver"| SQL
    Forms -->|"uses when unavailable or empty"| Demo
    Export -->|"uses on query failure"| Demo
    Export -->|"writes"| Files
    Export -->|"returns configured path"| Archive
    Export -->|"reads workstation"| Registry
    Export -->|"writes export event"| EventLog
```

### Technology Stack Summary

| Layer | Technology | Version | Purpose |
|---|---|---|---|
| Presentation | ASP.NET Web Forms | .NET Framework 4.7.2 | Landing page and sales report |
| Service | ASP.NET ASMX | .NET Framework 4.7.2 | Exposes order export as a web service |
| Data access | ADO.NET ODBC | .NET Framework 4.7.2; SQL Server ODBC Driver 17 configured | Queries order data |
| Storage | SQL Server Express | Version not specified | Configured `EcomDemo` reporting database |
| Infrastructure | ASP.NET session and Windows authentication | .NET Framework 4.7.2 | In-process session and identity handling |
| Infrastructure | Windows Registry and Event Log | Version not specified | Workstation lookup and best-effort event logging |
| Storage | Local file system and configured UNC path | Not specified | CSV output and archive path metadata |

### Data Storage & External Services

The configured ODBC connection points to SQL Server Express and queries `dbo.Orders`. CSV exports are written to the configured local export root; the UNC archive root is displayed or returned as metadata, not written to by the application. Windows Registry and Event Log provide workstation metadata and best-effort logging. Session state is in-process; no external cache, message broker, or outbound API is declared in the inspected application.

### Key Architectural Decisions

- Web Forms code-behind and the ASMX service contain SQL and reporting logic directly rather than using a separate repository or service layer.
- Both reporting paths fall back to demo data on ODBC errors; the report page also falls back when no rows are returned.
- Windows authentication is configured, but authorization explicitly allows all users for the local demo; sessions are stored in process.

## Component Relationships

```mermaid
flowchart LR
    subgraph Presentation["Presentation"]
        Default["Default page"]
        Monthly["MonthlySales page"]
    end
    subgraph Business["Reporting Logic"]
        Export["ReportExport ASMX service"]
        Result["ExportResult"]
        Row["MonthlySalesRow"]
    end
    subgraph DataAccess["Data Access"]
        PageODBC["Page ODBC query"]
        ExportODBC["Export ODBC query"]
        CSV["CSV file writer"]
    end
    subgraph Infra["Infrastructure"]
        Global["Global application startup"]
        Settings["Application settings"]
        Session["In-process session"]
        Security["Windows authentication and authorization"]
        Windows["Registry and event logging"]
    end

    Default -->|"links to"| Monthly
    Default -->|"reads"| Settings
    Default -->|"updates"| Session
    Monthly -->|"updates"| Session
    Monthly -->|"loads rows"| PageODBC
    Monthly -->|"binds"| Row
    Monthly -->|"invokes"| Export
    Export -->|"returns"| Result
    Export -->|"loads orders"| ExportODBC
    Export -->|"writes"| CSV
    Export -->|"reads"| Settings
    PageODBC -->|"reads connection setting"| Settings
    ExportODBC -->|"reads connection setting"| Settings
    Export -->|"uses"| Windows
    Global -->|"reads"| Settings
    Global -->|"uses"| Windows
    Security -.->|"guards requests"| Default
    Security -.->|"guards requests"| Monthly
    Security -.->|"guards requests"| Export
```

### Component Inventory

| Component | Layer | Type | Responsibility |
|---|---|---|---|
| Default | Presentation | Web Forms page | Shows operator, reporting mode and archive path; links to the report |
| MonthlySales | Presentation | Web Forms page | Loads and displays order summaries and triggers export |
| MonthlySalesRow | Reporting logic | Data object | Holds a customer summary row |
| ReportExport | Reporting logic | ASMX web service | Produces order CSV files and export metadata |
| ExportResult | Reporting logic | Service result | Holds export status, file path, archive root and workstation |
| Page ODBC query | Data access | Inline ADO.NET access | Queries order summaries with demo fallback |
| Export ODBC query and CSV file writer | Data access | Inline ADO.NET and file access | Queries orders and writes CSV, using demo rows on query failure |
| Global | Infrastructure | ASP.NET application lifecycle | Reads configuration and workstation information and logs startup |
| Application settings, session and security | Infrastructure | ASP.NET configuration | Supplies settings, maintains last-page session state and applies configured authentication and authorization |
| Registry and event logging | Infrastructure | Windows integration | Retrieves workstation information and writes best-effort events |

Sources: `D:\a\modernization-reporting-legacy-web\modernization-reporting-legacy-web\Reporting.Legacy.Web.csproj`, `D:\a\modernization-reporting-legacy-web\modernization-reporting-legacy-web\Web.config`, `D:\a\modernization-reporting-legacy-web\modernization-reporting-legacy-web\Default.aspx.cs`, `D:\a\modernization-reporting-legacy-web\modernization-reporting-legacy-web\Reports\MonthlySales.aspx.cs`, `D:\a\modernization-reporting-legacy-web\modernization-reporting-legacy-web\Services\ReportExport.asmx.cs`, and `D:\a\modernization-reporting-legacy-web\modernization-reporting-legacy-web\Global.asax.cs`.
