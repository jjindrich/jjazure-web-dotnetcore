# Using Open Service Mesh (OSM) with Azure Kubernetes Service (AKS)

## Install OSM as AKS addon

Follow this docs https://docs.microsoft.com/en-us/azure/aks/open-service-mesh-deploy-addon-az-cli

```powershell
az aks enable-addons --addons open-service-mesh -g jjmicroservices-rg  -n jjaks

kubectl get meshconfig osm-mesh-config -n kube-system -o yaml
```

Check Permissive traffic policy mode is set to true (traffic policy enforcement is bypassed).

## Configure with Ingress

Use with Ingress https://docs.microsoft.com/en-us/azure/aks/open-service-mesh-nginx-ingress

## Integrate with Azure Monitor

https://docs.microsoft.com/en-us/azure/aks/open-service-mesh-azure-monitor