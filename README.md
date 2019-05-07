# JJ Azure .Net Core samples
Azure dotNet Core samples

## Projects

### Project WebApp dotNet Core [jjazure-web-dotnetcore](src-web/README.md)
Web project running on Docker (Linux based) in Azure Container Instances(ACI) and Azure Kubernetes Service(AKS) with Visual Studio and Visual Studio Core.

### Project WebApp dotNet Core [jjazure-web-dotnetcore-windows](src-web-windows/README.md)
Web project running on Docker (Windows based) in Azure WebApp for Containers (Wnisual Studio and Visual Studio Core.

### Project ApiApp dotNet Core [jjazure-webapi-dotnetcore](src-webapi/README.md)
WebApi project running on Docker (Linux based) in Azure Kubernetes Service(AKS) with Visual Studio.

## Prepare Azure Kubernetes Service(AKS) with best practices

Link to best practices https://docs.microsoft.com/en-us/azure/aks/best-practices

### Setup RBAC for AKS

Follow this instructions https://docs.microsoft.com/en-us/azure/aks/azure-ad-integration

#### Prepare Azure Active Directory

I'm using my AAD jjdev.onmicrosoft.com with this configuration:
- App Registrations jjaksServer with Enterprise Application and permissions to AAD
- App Registrations jjaksClient with Enterprise Application and permissions to jjaksServer

```bash
aksname="jjaks"
```

Run this scripts https://docs.microsoft.com/en-us/azure/aks/azure-ad-integration-cli

Use new Active Directory blade App Registrations

##### Aks Server application
![Active Directory blade App Registrations - Server](media/aad-app-aksserver.png)
```bash
echo serverApplicationId=$serverApplicationId, serverApplicationSecret=$serverApplicationSecret
```
##### Aks Client application
![Active Directory blade App Registrations - Client](media/aad-app-aksclient.png)
```bash
echo clientApplicationId=$clientApplicationId
```

#### Deploy cluster in existing Virtual Network

Follow this instructions https://docs.microsoft.com/en-us/azure/aks/azure-ad-integration-cli#deploy-the-cluster

I have existing virtual network (created by Azure Blueprint)
- resource group vnet-central-rg
- jjvnet-central with address space 10.10.0.0/16
- dmz-aks subnet with 10.10.10.0/24

```bash
az group create --name jjmicroservices-rg --location WestEurope

tenantId=$(az account show --query tenantId -o tsv)

subscription=<YOUR_SUBSCRIPTION>

az aks create \
    --resource-group jjmicroservices-rg \
    --name $aksname \
    --node-count 1 \
    --generate-ssh-keys \
    --aad-server-app-id $serverApplicationId \
    --aad-server-app-secret $serverApplicationSecret \
    --aad-client-app-id $clientApplicationId \
    --aad-tenant-id $tenantId \
    --vnet-subnet-id /subscriptions/$subscription/resourceGroups/vnet-central-rg/providers/Microsoft.Network/virtualNetworks/jjvnet-central/subnets/dmz-aks

az aks get-credentials --resource-group jjkubernetes-rg --name $aksname --admin
```

TODO: Setup RBAC on cluster https://docs.microsoft.com/en-us/azure/aks/azure-ad-integration-cli#create-rbac-binding

#### Add Http routing to allow public endpoints
TODO: http routing extension

#### Setup Network policy
TODO: allow communitace to other service
https://docs.microsoft.com/en-us/azure/aks/operator-best-practices-network#control-traffic-flow-with-network-policies

TODO: install Istio 
https://docs.microsoft.com/en-us/azure/aks/istio-install

#### Connect to PaaS services like SQL server
TODO: use service endpoint to access sql server

### Add monitoring
TODO: Infra view - Log Analytics

TODO: Zero configuration for Azure Monitor
https://docs.microsoft.com/en-us/azure/azure-monitor/app/kubernetes

## Development
TODO: Use DevSpaces and Remote Development