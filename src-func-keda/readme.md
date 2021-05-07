# jjazure-func-keda

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
    --attach-acr $(az acr show -n jjakscontainers --query id -o tsv) `
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

az aks get-credentials --resource-group jjakstest-rg --name jjakstest
docker login jjakscontainers.azurecr.io -u jjakscontainers
kubectl create namespace jjfunckeda
func kubernetes install --namespace keda

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
Waiting for deployment "jjfunckeda-http" rollout to finish: 0 of 1 updated replicas are available...
deployment "jjfunckeda-http" successfully rolled out
        GetData - [httpTrigger]
        Invoke url: http://<your_ip>/api/getdata

        Master key: XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
```

Now you can check your api is working

```powershell
kubectl get services jjfunckeda-http -n jjfunckeda
curl http://<your_ip>/api/GetData
```

# Deploy to AKS virtual nodes using KEDA 

It's not possible to use Azure Functions CLI to configure using AKS virtual nodes. We have to create Kubernetes manifests and update manualy.
Check this sample https://github.com/kedacore/sample-hello-world-azure-functions

Run this command and update Kubernetes Deployment manifest [deployment-aksvirtualnodes.yaml](deployment-aksvirtualnodes.yaml).
Other manifests you can use as are generated automatically.

```powershell
func kubernetes deploy --name jjfunckeda --registry jjakscontainers.azurecr.io --namespace jjfunckeda --dry-run
```

There is limitation to access Azure Container Registry with AAD identity, use Kubenetes secret - https://docs.microsoft.com/en-us/azure/aks/virtual-nodes-cli#deploy-a-sample-app

Run this command to store ACR secrets and update Kubernetes deployment manifest [deployment-aksvirtualnodes.yaml](deployment-aksvirtualnodes.yaml) with imagePullSecrets.

```powershell
kubectl create secret docker-registry -n jjfunckeda jjakscontainerscred --docker-server=jjakscontainers.azurecr.io --docker-username=jjakscontainers --docker-password=<your-pword>
```

Deploy it and check node is virtual-node-aci-linux

```powershell
kubectl apply -f deployment-aksvirtualnodes.yaml
kubectl get pods -o wide -n jjfunckeda
```

Now you can check your api is working

```powershell
kubectl get services jjfunckeda-http -n jjfunckeda
curl http://<your_ip>/api/GetData
```

# Configure autoscaling with KEDA of HTTP trigger

KEDA doesn't automatically manage HTTP trigger functions pods. But you can instrument KEDA to setup autoscaling.

https://docs.microsoft.com/en-us/azure/azure-functions/functions-kubernetes-keda#http-trigger-support
https://keda.sh/docs/2.2/scalers/azure-monitor/

Deploy it

```powershell
kubectl apply -f scaleobject.yaml
kubectl get pods -o wide -n jjfunckeda
```

TODO: not working, submitted GH Issue https://github.com/MicrosoftDocs/azure-docs/issues/74902

Troubleshooting

```powershell
kubectl get pods -n keda
kubectl logs keda-68566445b8-wccjm -c keda -n keda
```