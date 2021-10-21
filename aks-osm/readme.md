# Using Open Service Mesh (OSM) with Azure Kubernetes Service (AKS)

Open Service Mesh (OSM) https://github.com/openservicemesh/osm

## Install OSM into AKS

### Install OSM as AKS addon

Follow this docs https://docs.microsoft.com/en-us/azure/aks/open-service-mesh-deploy-addon-az-cli

```powershell
az aks enable-addons --addons open-service-mesh -g jjmicroservices-rg  -n jjaks

kubectl get meshconfig osm-mesh-config -n kube-system -o yaml
```

Check Permissive traffic policy mode is set to true (traffic policy enforcement is bypassed).

NOT WORKING: addon is using old OSM version 0.9 which not working correctly

### Install OSM manually

Follow this docs https://docs.microsoft.com/en-us/azure/aks/open-service-mesh-binary

```powershell
# Specify the OSM version that will be leveraged throughout these instructions
$OSM_VERSION="v0.11.1"
[Net.ServicePointManager]::SecurityProtocol = "tls12"
$ProgressPreference = 'SilentlyContinue'; Invoke-WebRequest -URI "https://github.com/openservicemesh/osm/releases/download/$OSM_VERSION/osm-$OSM_VERSION-windows-amd64.zip" -OutFile "osm-$OSM_VERSION.zip"
Expand-Archive -Path "osm-$OSM_VERSION.zip" -DestinationPath .

osm version
osm install –set OpenServiceMesh.enablePermissiveTrafficPolicy=true
kubectl get meshconfig osm-mesh-config -n osm-system -o yaml
#osm mesh uninstall
```

## Enable OMS for existing application

Following this docs https://docs.microsoft.com/en-us/azure/aks/open-service-mesh-deploy-existing-application

Enable OSM on existing namespaces and restart deployments

```powershell
osm namespace add jjweb
osm namespace add jjapi

kubectl rollout restart deployment jjwebcore -n jjweb
kubectl rollout restart deployment jjwebapicore -n jjapi
kubectl rollout restart deployment jjwebcorewindows -n jjapi
```

Check that pod is running two containers (application container and envoy).

```powershell
kubectl get pods -n jjweb
kubectl get pods -n jjapi
```

Add service accounts (service accounts as part of authorizing service-to-service communications) and update deployments

```powershell
kubectl create serviceaccount jjweb -n jjweb
kubectl create serviceaccount jjapi -n jjapi

kubectl set serviceaccount deployment/jjwebcore -n jjweb jjweb
kubectl set serviceaccount deployment/jjwebapicore -n jjapi jjapi
kubectl set serviceaccount deployment/jjwebcorewindows -n jjapi jjapi
```

Change Permissive traffic policy mode is set to false

```bash
#kubectl patch meshconfig osm-mesh-config -n kube-system -p '{"spec":{"traffic":{"enablePermissiveTrafficPolicyMode":false}}}' --type=merge
kubectl patch meshconfig osm-mesh-config -n osm-system -p '{"spec":{"traffic":{"enablePermissiveTrafficPolicyMode":false}}}' --type=merge
```

NOT WORKING: Now you can see you cannot access api services - getting 404 

ISSUE: Windows containers not supported

## Configure with Ingress

Use with Ingress https://docs.microsoft.com/en-us/azure/aks/open-service-mesh-nginx-ingress

## Integrate with Azure Monitor

https://docs.microsoft.com/en-us/azure/aks/open-service-mesh-azure-monitor