# Architecture Diagram

This document summarizes the current legacy reporting application structure and its key component relationships.

## Application Architecture

```mermaid
flowchart TD
    subgraph Client["Client Layer"]
        Browser["Browser"]
    end

    subgraph App["Application Layer - ASP.NET Web Forms"]
        DefaultPage["Default.aspx"]
        MonthlyPage["Reports MonthlySales.aspx"]
        ExportService["ReportExport ASMX Service"]
    end

    subgraph Data["Data Layer"]
        OdbcClient["System.Data.Odbc"]
        SqlServer[("SQL Server via ODBC")]
        FileExport[("CSV Export Folder")]
        Registry["Windows Registry"]
        EventLog["Windows Event Log"]
    end

    subgraph External["External Systems"]
        ArchiveShare["UNC Archive Share"]
    end

    Browser -->|"HTTP request"| DefaultPage
    Browser -->|"HTTP request"| MonthlyPage
    MonthlyPage -->|"invoke export"| ExportService
    MonthlyPage -->|"query order summary"| OdbcClient
    ExportService -->|"query orders"| OdbcClient
    OdbcClient -->|"SQL over ODBC"| SqlServer
    ExportService -->|"write CSV"| FileExport
    ExportService -->|"returns archive root"| ArchiveShare
    ExportService -->|"read workstation"| Registry
    ExportService -->|"write telemetry event"| EventLog
```

### Technology Stack Summary

| Layer | Technology | Version | Purpose |
|---|---|---|---|
| Presentation | ASP.NET Web Forms | .NET Framework 4.7.2 | UI pages for reporting and export trigger |
| Service | ASMX SOAP Web Service | .NET Framework 4.7.2 | ExportOrders operation for CSV generation |
| Data Access | System.Data.Odbc | .NET Framework BCL | Reads order/reporting data |
| Runtime | IIS Express / ASP.NET | Legacy .NET runtime | Hosts Web Forms and ASMX endpoints |

### Data Storage & External Services

The app reads reporting data from SQL Server through an ODBC connection string and writes export output to a local filesystem path. It also references a UNC archive path in configuration and interacts with Windows Registry/EventLog for environment and audit signals.

### Key Architectural Decisions

- Uses classic Web Forms UI plus ASMX service endpoint rather than ASP.NET MVC/Web API.
- Keeps ODBC as the primary data access mechanism to a legacy SQL source.
- Provides resilient demo fallback rows when live ODBC connectivity is unavailable.

## Component Relationships

```mermaid
flowchart LR
    subgraph Presentation["Presentation"]
        DefaultCodeBehind["Default Page CodeBehind"]
        MonthlyCodeBehind["MonthlySales Page CodeBehind"]
    end

    subgraph Business["Business Logic"]
        ExportOrchestrator["ReportExport.ExportOrders"]
        CsvBuilder["ReportExport.BuildCsv"]
    end

    subgraph DataAccess["Data Access"]
        OdbcConn["OdbcConnection"]
        OdbcCmd["OdbcCommand"]
    end

    subgraph Infrastructure["Infrastructure"]
        ConfigMgr["ConfigurationManager"]
        FsOps["File IO"]
        RegOps["Registry Access"]
        EventOps["EventLog Access"]
        SessionOps["Session State"]
    end

    DefaultCodeBehind -->|"reads settings"| ConfigMgr
    DefaultCodeBehind -->|"stores page marker"| SessionOps
    MonthlyCodeBehind -->|"loads data"| OdbcConn
    MonthlyCodeBehind -->|"delegates export"| ExportOrchestrator
    ExportOrchestrator -->|"builds content"| CsvBuilder
    CsvBuilder -->|"executes query"| OdbcCmd
    OdbcCmd -->|"uses"| OdbcConn
    ExportOrchestrator -->|"writes file"| FsOps
    ExportOrchestrator -->|"reads keys"| ConfigMgr
    ExportOrchestrator -->|"reads workstation"| RegOps
    ExportOrchestrator -->|"writes audit event"| EventOps
```

### Component Inventory

| Component | Layer | Type | Responsibility |
|---|---|---|---|
| Default.aspx.cs | Presentation | Web Forms Page | Renders operator/reporting mode landing information |
| MonthlySales.aspx.cs | Presentation | Web Forms Page | Loads monthly sales rows and triggers export |
| ReportExport.asmx.cs | Business Logic | ASMX Web Service | Produces export metadata and writes report CSV |
| BuildCsv | Business Logic | Service Helper | Reads orders and converts to CSV rows with fallback |
| ConfigurationManager | Infrastructure | Config Provider | Supplies appSettings values from Web.config |
| OdbcConnection/OdbcCommand | Data Access | Data Access API | Executes SQL queries via ODBC |
| File/Directory APIs | Infrastructure | File System | Persists generated export files |
| Registry/EventLog APIs | Infrastructure | OS Integration | Reads machine setting and writes operational event |
```
