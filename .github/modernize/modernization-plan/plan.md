# Infrastructure Plan: modernization-plan

## User Requirements

Create a modernization plan for this .NET legacy web application using the
assessment recommendations. Do not provision resources now; generate Infrastructure
as Code for each modernization solution so implementation can be executed later.

**Plan Configuration**:

| Parameter | Value | Description |
|-----------|-------|-------------|
| IaC Tool | bicep | Infrastructure as Code tool |
| Provision | false | Generate IaC only; do not provision resources |
| Subscription | TBD | Target Azure subscription |

---

## Proposed Architecture

Legacy ASP.NET Web Forms application is modernized to a cloud-ready architecture:

- Application runtime upgraded from .NET Framework 4.7.2 to .NET 10.
- Application hosted on Azure Container Apps.
- Authentication moved from Windows authentication to Microsoft Entra ID.
- Session state moved from InProc to Azure Cache for Redis.
- Sensitive configuration and connection details externalized to Azure Key Vault
  and managed identity-based access.
- Bicep templates prepared for all required Azure resources without provisioning.

---

## Azure Resource List

| Resource Type | Resource Name | SKU | Est. Monthly Cost | Purpose |
|---------------|---------------|-----|--------------------|---------|
| Azure Container Apps Environment | cae-reporting-legacy | Consumption | Usage-based | Host app containers |
| Azure Container App | ca-reporting-legacy-web | Consumption | Usage-based | Run web workload |
| Azure Container Registry | acrreportinglegacy | Basic | ~$5/mo | Store container images |
| Azure Cache for Redis | redis-reporting-legacy | Basic C0 | ~$16/mo | External session state |
| Azure Key Vault | kv-reporting-legacy | Standard | Usage-based | Secure secrets/config |
| Azure Log Analytics Workspace | law-reporting-legacy | PAYG | Usage-based | Centralized diagnostics |
| Azure Managed Identity | mi-reporting-legacy | N/A | $0 | Passwordless Azure auth |

> **Note**: The estimated costs shown above are based on Azure retail prices and serve only as a rough estimation. Actual costs may differ due to enterprise agreements, reservations, or other discounts. For consumption-based (pay-as-you-go) resources, costs depend on actual usage and cannot be accurately estimated upfront.

---

## Open Questions & Questionnaire

- [x] Q: Should the plan include environment/infrastructure provisioning? → A: No — generate IaC only and do not provision resources yet.
- [x] Q: Should the plan include integration testing? → A: No explicit integration-testing request was provided; excluded from this scoped plan.
- [x] Q: Should the plan include security/CVE remediation? → A: Yes — include default security/CVE remediation task.
- [x] Q: Which Azure deployment target should the plan use? → A: Azure Container Apps (default target) with IaC prepared via Bicep.
