# Azure Application Gateway for Containers

Steps
1. https://learn.microsoft.com/en-us/azure/application-gateway/for-containers/quickstart-create-application-gateway-for-containers-managed-by-alb-controller?tabs=new-subnet-aks-vnet
2. https://learn.microsoft.com/en-us/azure/application-gateway/for-containers/quickstart-deploy-application-gateway-for-containers-alb-controller?tabs=install-helm-windows

## Deploy

```powershell
# Register required resource providers on Azure.
az provider register --namespace Microsoft.ContainerService
az provider register --namespace Microsoft.Network
az provider register --namespace Microsoft.NetworkFunction
az provider register --namespace Microsoft.ServiceNetworking
az extension add --name alb
```

Install ALB controller in AKS
```powershell
$AKS_NAME='jjazaks'
$RESOURCE_GROUP='jjmicroservices-rg'
# az aks show -g $RESOURCE_GROUP -n $AKS_NAME
az aks update -g $RESOURCE_GROUP -n $AKS_NAME --enable-oidc-issuer --enable-workload-identity

$IDENTITY_RESOURCE_NAME='jjazaks-alb-identity'
az identity create --resource-group $RESOURCE_GROUP --name $IDENTITY_RESOURCE_NAME
$principalId="$(az identity show -g $RESOURCE_GROUP -n $IDENTITY_RESOURCE_NAME --query principalId -otsv)"
$mcResourceGroup=$(az aks show --resource-group $RESOURCE_GROUP --name $AKS_NAME --query "nodeResourceGroup" -o tsv)
$mcResourceGroupId=$(az group show --name $mcResourceGroup --query id -otsv)
az role assignment create --assignee-object-id $principalId --assignee-principal-type ServicePrincipal --scope $mcResourceGroupId --role "acdd72a7-3385-48ef-bd42-f606fba81ae7"

$AKS_OIDC_ISSUER="$(az aks show -n "$AKS_NAME" -g "$RESOURCE_GROUP" --query "oidcIssuerProfile.issuerUrl" -o tsv)"
az identity federated-credential create --name "$IDENTITY_RESOURCE_NAME" --identity-name "$IDENTITY_RESOURCE_NAME" --resource-group $RESOURCE_GROUP --issuer "$AKS_OIDC_ISSUER" --subject "system:serviceaccount:azure-alb-system:alb-controller-sa"

az aks get-credentials --resource-group $RESOURCE_GROUP --name $AKS_NAME
helm install alb-controller oci://mcr.microsoft.com/application-lb/charts/alb-controller --version 0.4.023971 --set albController.podIdentity.clientID=$(az identity show -g $RESOURCE_GROUP -n $IDENTITY_RESOURCE_NAME --query clientId -o tsv)

kubectl get pods -n azure-alb-system
```

Deploy Applicaton Gateway for Containers
```powershell
$VNET_RESOURCE_GROUP = 'jjnetwork-rg'
$VNET_NAME = 'jjazappvnet'
$ALB_SUBNET_NAME = 'appgw-snet'
$ALB_SUBNET_ID=$(az network vnet subnet show --name $ALB_SUBNET_NAME --resource-group $VNET_RESOURCE_GROUP --vnet-name $VNET_NAME --query '[id]' --output tsv)

az role assignment create --assignee-object-id $principalId --assignee-principal-type ServicePrincipal --scope $mcResourceGroupId --role "fbc52c3f-28ad-4303-a892-8a056630b8f1" 
az role assignment create --assignee-object-id $principalId --assignee-principal-type ServicePrincipal --scope $ALB_SUBNET_ID --role "4d97b98b-1d4f-4787-a291-c67834d212e7" 

kubectl create namespace alb-test-infra

(Get-Content "alb.yaml") -replace "ALB_SUBNET_ID", $ALB_SUBNET_ID | out-file "alb-deploy.yaml"
kubectl apply -f alb-deploy.yaml
kubectl get applicationloadbalancer alb-test -n alb-test-infra -o yaml

kubectl apply -f ingress.yaml
```