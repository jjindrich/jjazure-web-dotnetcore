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

Create new AAD group for admins.

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

##### Create AAD group for admins
![Active Directory blade Group - Client](media/aad-group.png)

### Deploy cluster in existing Virtual Network and with RBAC

Follow this instructions https://docs.microsoft.com/en-us/azure/aks/configure-azure-cni

I have existing virtual network (created by Azure Blueprint)
- resource group vnet-central-rg
- jjvnet-central with address space 10.10.0.0/16
- dmz-aks subnet with 10.10.10.0/24

#### 1. Create AKS cluster 

```bash
az extension add --name aks-preview
az extension update --name aks-preview
```

```bash
az group create --name jjmicroservices-rg --location WestEurope

tenantId=$(az account show --query tenantId -o tsv)
vnetid=$(az network vnet subnet list --resource-group vnet-central-rg --vnet-name jjvnet-central --query [].id --output tsv | grep dmz-aks)

az aks create \
    --resource-group jjmicroservices-rg \
    --name $aksname \
    --node-vm-size Standard_B2s \
    --node-count 1 \
    --min-count 1 \
    --max-count 3 \
    --enable-vmss \
    --enable-cluster-autoscaler \
    --generate-ssh-keys \
    --service-principal $serverApplicationId \
    --client-secret $serverApplicationSecret \
    --aad-server-app-id $serverApplicationId \
    --aad-server-app-secret $serverApplicationSecret \
    --aad-client-app-id $clientApplicationId \
    --aad-tenant-id $tenantId \
    --network-plugin azure \
    --vnet-subnet-id $vnetid \
    --node-resource-group jjmicroservices-aks-rg

az aks get-credentials --resource-group jjmicroservices-rg --name $aksname --admin
az aks browse --resource-group jjmicroservices-rg --name $aksname
```

#### 2. Assign admin to AKS cluster

I have AAD group with admins - AKS Admins. Update rbac-aad-admin.yaml with your AAD Group object Id.
Next login as admin (--admin) and run script.

```bash
az aks get-credentials --resource-group jjmicroservices-rg --name $aksname --admin
kubectl apply -f aks/rbac-aad-admin.yaml
```

#### 3. Try to access AKS cluster as AAD user

Try to login as AAD user to AKS cluster and get nodes.

```bash
az aks get-credentials --resource-group jjmicroservices-rg --name $aksname
kubectl get nodes
```

#### 4. Enable Dashboard for AKS with RBAC

By default Dashboard is created with minimal permissions, let's run this command to enable Dashboard for admins

```bash
kubectl create clusterrolebinding kubernetes-dashboard --clusterrole=cluster-admin --serviceaccount=kube-system:kubernetes-dashboard
```

Links:
- custom resource group https://docs.microsoft.com/en-us/azure/aks/faq#can-i-provide-my-own-name-for-the-aks-infrastructure-resource-group
- custom vnet https://docs.microsoft.com/en-us/azure/aks/configure-azure-cni
- connect to AAD and assign AAD admin https://docs.microsoft.com/en-us/azure/aks/azure-ad-integration-cli
- upgrade AKS pool https://docs.microsoft.com/en-us/azure/aks/use-multiple-node-pools#upgrade-a-node-pool
- enable Dashboard https://docs.microsoft.com/en-us/azure/aks/kubernetes-dashboard#for-rbac-enabled-clusters

### Add Http routing to allow public endpoints
TODO: http routing extension
TODO: add AppGw ingress

### Connect to PaaS services like SQL server
TODO: use service endpoint to access sql server

### Development
TODO: Use DevSpaces and Remote Development

### Setup security
Best practices
https://docs.microsoft.com/en-us/azure/aks/operator-best-practices-network#control-traffic-flow-with-network-policies

Setup addition RBAC roles to acccess cluster
https://docs.microsoft.com/en-us/azure/aks/azure-ad-rbac

Allow communication btw services
https://docs.microsoft.com/en-us/azure/aks/use-network-policies

Policy for Kubernetes - allow only defined ACR
https://docs.microsoft.com/en-us/azure/aks/use-pod-security-policies

API whitelisting
https://docs.microsoft.com/en-us/azure/aks/api-server-authorized-ip-ranges

Limit egress traffic - use Azure Firewall
https://docs.microsoft.com/en-us/azure/aks/limit-egress-traffic

Install Istio for advanced routing and networking features
https://docs.microsoft.com/en-us/azure/aks/istio-install

### Setup monitoring
Infra view - use Log Analytics
https://docs.microsoft.com/en-us/azure/azure-monitor/insights/container-insights-overview

Zero configuration for Azure Monitor for application monitoring (requires Istio)
https://docs.microsoft.com/en-us/azure/azure-monitor/app/kubernetes