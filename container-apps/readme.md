# Deploy to Azure Container Apps

This guide will deploy jjweb solution info Azure Container Apps

Check this new service https://azure.microsoft.com/en-us/services/container-apps/

Structure

- one container environment
- web container app for jjwebcore
- api container app for jjweapicore

ISSUE: Windows Containers not supproted now.

## Deploy it

It's using ARM template in Bicep format.

Referencing container image name (not using Helm manifests etc).

Security values from Azure KeyVaul - must be called as module.

Referencing to api service using https://docs.microsoft.com/en-us/azure/container-apps/connect-apps

```powershell
.\deploy.ps1
```

## Monitoring

Use Log Analytics workspace

```kql
ContainerAppConsoleLogs_CL
| sort by TimeGenerated desc
| project ContainerAppName_s, Log_s, TimeGenerated
```