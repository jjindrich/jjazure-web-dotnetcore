# fix \r issue -> dos2unix deploy-jjaks.sh
# load variables from keyvault jjakskv
serverApplicationId=$(az keyvault secret show --vault-name jjakskv --name aksserverApplicationId -o tsv --query value)
serverApplicationSecret=$(az keyvault secret show --vault-name jjakskv --name aksserverApplicationSecret -o tsv --query value)
clientApplicationId=$(az keyvault secret show --vault-name jjakskv --name aksclientApplicationId -o tsv --query value)
aksname=$(az keyvault secret show --vault-name jjakskv --name aksname -o tsv --query value)
winpassword=$(az keyvault secret show --vault-name jjakskv --name akswinpassword -o tsv --query value)

tenantId=$(az account show --query tenantId -o tsv)
vnetsubnetid=$(az network vnet subnet list --resource-group JJDevV2-Infra --vnet-name JJDevV2NetworkApp --query "[?name=='DmzAks'].id" --output tsv)
workspaceId=$(az resource show -n jjdev-analytics -g jjdevmanagement --resource-type microsoft.operationalinsights/workspaces --query id --output tsv)

az extension update --name aks-preview

az aks create \
    --resource-group jjmicroservices-rg \
    --name $aksname \
    --node-vm-size Standard_B2s \
    --node-count 1 \
    --min-count 1 \
    --max-count 3 \
    --enable-cluster-autoscaler \
    --zones 1 2 3 \
    --enable-addons monitoring \
    --workspace-resource-id $workspaceId \
    --generate-ssh-keys \
    --service-principal $serverApplicationId \
    --client-secret $serverApplicationSecret \
    --aad-server-app-id $serverApplicationId \
    --aad-server-app-secret $serverApplicationSecret \
    --aad-client-app-id $clientApplicationId \
    --aad-tenant-id $tenantId \
    --network-plugin azure \
    --vnet-subnet-id $vnetsubnetid \
    --windows-admin-username aksadmin \
    --windows-admin-password $winpassword \
    --node-resource-group jjmicroservices-aks-rg
# with limited vnet subnet size use: --network-plugin kubenet 

az aks get-credentials --resource-group jjmicroservices-rg --name $aksname --admin --overwrite-existing

vnetid=$(az network vnet show --resource-group JJDevV2-Infra --name JJDevV2NetworkApp --query id -o tsv)
az role assignment create --assignee $serverApplicationId --scope $vnetid --role Contributor
az aks enable-addons \
    --resource-group jjmicroservices-rg \
    --name $aksname \
    --addons virtual-node \
    --subnet-name DmzAci
kubectl get pods --all-namespaces

az aks nodepool add \
    --resource-group jjmicroservices-rg \
    --cluster-name $aksname \
    --os-type Windows \
    --name npwin \
    --node-vm-size Standard_B2ms \
    --node-taints os=windows:NoSchedule \
    --node-count 1 \
    --zones 1 2 3

# install helm2
#wget https://get.helm.sh/helm-v2.16.1-linux-amd64.tar.gz
#tar xvf helm-v2.16.1-linux-amd64.tar.gz
#export PATH="$HOME/bin:$PATH"

kubectl apply -f aks/rbac-aad-admin.yaml
kubectl apply -f aks/rbac-log-reader.yaml
kubectl create clusterrolebinding kubernetes-dashboard --clusterrole=cluster-admin --serviceaccount=kube-system:kubernetes-dashboard
kubectl apply -f aks/helm-account.yaml
helm init --service-account tiller

kubectl apply -f aks/corednsms.yaml
kubectl delete pod --namespace kube-system --selector k8s-app=kube-dns

kubectl create namespace ingress-basic
helm install stable/nginx-ingress --name nginx-ingress --namespace ingress-basic --set controller.replicaCount=2 --set controller.nodeSelector."kubernetes\.io/os"=linux
kubectl create namespace ingress-basic-internal
helm install stable/nginx-ingress --name nginx-ingress-internal --namespace ingress-basic-internal --set controller.replicaCount=2 -f aks/internal-ingress.yaml --set controller.ingressClass=nginx-internal

kubectl get pods --all-namespaces

# Dev Spaces install
#az aks use-dev-spaces -g jjmicroservices-rg -n $aksname --space dev --yes
#kubectl get pods --all-namespaces