# Using Open Service Mesh (OSM) with Azure Kubernetes Service (AKS)

Open Service Mesh (OSM) https://github.com/openservicemesh/osm

Key features https://github.com/openservicemesh/osm#features

We will focus on access control primary. There is required to have Kubernetes Service accounts deployed.

## Install OSM into AKS

### Install OSM as AKS addon

Follow this docs https://docs.microsoft.com/en-us/azure/aks/open-service-mesh-deploy-addon-az-cli

```powershell
az aks enable-addons --addons open-service-mesh -g jjmicroservices-rg  -n jjaks

kubectl get meshconfig osm-mesh-config -n kube-system -o yaml
```

Check Permissive traffic policy mode is set to true (traffic policy enforcement is bypassed).

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

Enable egress traffic (to platform services - monitoring, db etc)

```bash
kubectl patch meshconfig osm-mesh-config -n kube-system -p '{"spec":{"traffic":{"enableEgress":true}}}' --type=merge
#kubectl patch meshconfig osm-mesh-config -n osm-system -p '{"spec":{"traffic":{"enableEgress":true}}}' --type=merge
```

Change Permissive traffic policy mode is set to false

```bash
kubectl patch meshconfig osm-mesh-config -n kube-system -p '{"spec":{"traffic":{"enablePermissiveTrafficPolicyMode":false}}}' --type=merge
#kubectl patch meshconfig osm-mesh-config -n osm-system -p '{"spec":{"traffic":{"enablePermissiveTrafficPolicyMode":false}}}' --type=merge
```

Now you can run port forward and see you cannot access api services (getting 404) because OSM, see bellow policy configuration

```powershell
kubectl get pods -n jjweb
kubectl port-forward jjwebcore-76b4dfb478-b8hqm -n jjweb 8080:80
```

Web published by Ingress is not working because OSM, see bellow ingress configuration

ISSUE: Windows containers not supported (https://github.com/openservicemesh/osm/projects/3)

## Configure policy for api

Because OSM blocks communication btw jjweb and jjapi you need to configure policy to allow it.

Prepare policy based on this docs https://docs.microsoft.com/en-us/azure/aks/open-service-mesh-deploy-existing-application#deploy-the-necessary-service-mesh-interface-smi-policies 

```powershell
kubectl apply -f jjapi-allow.yaml
```

ISSUE: Windows containers not supported, so you cannot access API windows.

## Configure with Ingress

Because Nginx Ingress is outside of servicemesh (namespace is not enabled), we have to allow communication from Ingress service to web backend (jjwebcore). Thanks https://www.tomaskubica.cz/post/2021/kubernetes-prakticky-jak-napojit-ingress-na-open-service-mesh/

```powershell
kubectl apply -f jjweb-ingress-allow.yaml
```

ISSUE: This doc is not valid https://docs.microsoft.com/en-us/azure/aks/open-service-mesh-nginx-ingress

## Integrate with Azure Monitor

Now is possible to collect Prometheus metrics in Azure Monitor (Insights Metrics) - https://docs.microsoft.com/en-us/azure/aks/open-service-mesh-azure-monitor

```powershell
osm metrics enable --namespace jjweb
osm metrics enable --namespace jjapi
kubectl apply -f monitor.yaml
```

Now you can run Log analytics query 

```
InsightsMetrics
| where Name contains "envoy"
| extend t=parse_json(Tags)
| where t.app == "jjwebcore"
```

Other observability options, like Tracing https://docs.openservicemesh.io/docs/guides/observability/

## DevOps and Traffic splitting

OSM supports traffic shifting

GitHub Actions support for different deployment stragies https://github.com/Azure/k8s-deploy#action-capabilities