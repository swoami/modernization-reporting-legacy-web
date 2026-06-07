# .NET Upgrade Plan: Reporting.Legacy.Web

## Overview

Upgrade the **Reporting.Legacy.Web** ASP.NET Web Application from **.NET Framework 4.7.2** to **.NET 10.0 (LTS)**.

The project currently uses the legacy non-SDK-style `.csproj` format targeting `v4.7.2`. Upgrading to .NET 10.0 requires converting the project file to the modern SDK-style format, updating all NuGet dependencies, migrating ASP.NET Web Forms/ASMX constructs where possible, and fixing any breaking API changes.

## Project Details

| Property | Value |
|----------|-------|
| **Project** | Reporting.Legacy.Web |
| **Project File** | Reporting.Legacy.Web.csproj |
| **Source Framework** | .NET Framework 4.7.2 |
| **Target Framework** | net10.0 |
| **SDK-Style Conversion** | Required |

## Projects in Solution

| Project | Current TFM | Target TFM |
|---------|-------------|------------|
| Reporting.Legacy.Web | .NET Framework 4.7.2 | net10.0 |

## Upgrade Scope

1. **SDK-style project file conversion** — Replace the legacy non-SDK `.csproj` with modern SDK-style format (`<Project Sdk="Microsoft.NET.Sdk.Web">`).
2. **Target framework update** — Change `<TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>` to `<TargetFramework>net10.0</TargetFramework>`.
3. **NuGet package updates** — Migrate from `packages.config` to `<PackageReference>` and update all packages to versions compatible with net10.0.
4. **API compatibility fixes** — Address breaking changes between .NET Framework and .NET 10.0 (e.g., `System.Web` removal, ASMX/Web Forms migration to ASP.NET Core equivalents).
5. **Configuration migration** — Migrate `Web.config` settings to `appsettings.json` / ASP.NET Core middleware configuration.
6. **Build validation** — Ensure the solution compiles and all unit tests pass on .NET 10.0.

## Reason for Upgrade

The user explicitly requested an upgrade to the latest LTS version. Additionally, .NET Framework 4.7.2 is a legacy platform that will not receive new feature updates; migrating to .NET 10.0 LTS provides long-term support, performance improvements, modern API access, and full compatibility with the Azure SDK (`Azure.*` packages).
