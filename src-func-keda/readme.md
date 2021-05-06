#jjazure-func-keda
Azure Function project running in Azure Managed Kubernetes (AKS) using KEDA and virtual nodes.
Azure Function is HTTP trigger getting sample data. Is autoscaled by KEDA automatically.

https://docs.microsoft.com/en-us/azure/azure-functions/functions-kubernetes-keda

# Create Azure Function

Follow this tutorial https://docs.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-csharp

Created function GetData returning sample data.

Test locally 

```powershell
curl http://localhost:7071/api/GetData
```

# Create AKS using virtual nodes

This will create new AKS running in existing Azure virtual network, using existing monitoring Log analytics workspace and Azure Container Registry. Virtual Nodes (Azure Container Instances) requires running Azure CNI networking.

https://docs.microsoft.com/en-us/azure/aks/virtual-nodes-cli

```powershell
$rg='jjakstest-rg'
$rgmonitor='jjdevmanagement'
$rgnetwork='jjdevv2-infra'

az group create -n $rg -l westeurope
az aks create -n jjakstest -g $rg `
    -x -c 2 -z 1 2 3 `
    --node-vm-size Standard_B2s --enable-cluster-autoscaler --min-count 1 --max-count 3 `
    --network-plugin azure `
    --attach-acr $(az acr show -n jjakscontainers --query id -o tsv)
    --vnet-subnet-id $(az network vnet subnet show --vnet-name JJDevV2NetworkApp -g $rgnetwork -n DmzAks --query id -o tsv) `
    --enable-managed-identity `
    --enable-addons azure-policy,monitoring,virtual-node `
    --workspace-resource-id $(az monitor log-analytics workspace show -g $rgmonitor -n jjdev-analytics --query id -o tsv) `
    --aci-subnet-name DmzAci
```

# Deploy to AKS using KEDA

It creates docker file, push image into Azure Container Registry (using docker login), creates kubernetes kubemanifests and deploy to AKS (using kubectl current context).

```powershell
func init --docker-only
docker login jjakscontainers.azurecr.io -u jjakscontainers
kubectl create namespace jjfunckeda
func kubernetes deploy --name jjfunckeda --registry jjakscontainers.azurecr.io --namespace jjfunckeda
```

You will get this result

```powershell
secret/jjfunckeda created
secret/func-keys-kube-secret-jjfunckeda created
serviceaccount/jjfunckeda-function-keys-identity-svc-act created
role.rbac.authorization.k8s.io/functions-keys-manager-role created
rolebinding.rbac.authorization.k8s.io/jjfunckeda-function-keys-identity-svc-act-functions-keys-manager-rolebinding created
service/jjfunckeda-http created
deployment.apps/jjfunckeda-http created
Getting loadbalancer ip for the service: jjfunckeda-http
```

Now you can check your api is working

```powershell
kubectl get services jjfunckeda-http -n jjfunckeda
http://<your_ip>/api/GetData
```

# Configure autoscaling with KEDA of HTTP trigger

KEDA doesn't automatically manage HTTP trigger functions pods. But you can instrument KEDA to setup autoscaling.

https://docs.microsoft.com/en-us/azure/azure-functions/functions-kubernetes-keda#http-trigger-support
https://keda.sh/docs/2.2/scalers/azure-monitor/
