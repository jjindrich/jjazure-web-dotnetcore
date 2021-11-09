# Deploy to Azure Container Apps

This guide will deploy jjweb solution info Azure Container Apps

Check this new service https://azure.microsoft.com/en-us/services/container-apps/

Structure

- one container environment
- web container app for jjwebcore
- api container app for jjweapicore

ISSUE: Windows Containers not supproted now.

To use Azure CLI enable preview addon

```powershell
az extension add --source https://workerappscliextension.blob.core.windows.net/azure-cli-extension/containerapp-0.2.0-py2.py3-none-any.whl
```

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

## DevOps with GitHub Actions

This command generates pipeline https://docs.microsoft.com/en-us/azure/container-apps/github-actions-cli

There is no special Action task, it contains

```yaml
  deploy:
    runs-on: ubuntu-latest
    needs: build
    
    steps:
      - name: Azure Login
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}

      - name: Deploy to containerapp
        uses: azure/CLI@v1
        with:
          azcliversion: 2.20.0
          inlineScript: |
            echo "Installing containerapp extension"
            az provider register --namespace Microsoft.Web
            az extension add --source https://workerappscliextension.blob.core.windows.net/azure-cli-extension/containerapp-0.1.12-py2.py3-none-any.whl --yes
            echo "Starting Deploying"
            az containerapp update -n jjwebenv-jjweb -g jjmicroservices-rg -i http://jjakscontainers.azurecr.io/jjwebenv-jjweb:${{ github.sha }} --registry-login-server http://jjakscontainers.azurecr.io --registry-username  ${{ secrets.REGISTRY_USERNAME }} --registry-password ${{ secrets.REGISTRY_PASSWORD }} --debug

```