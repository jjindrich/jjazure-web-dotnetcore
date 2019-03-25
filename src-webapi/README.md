# jjazure-webapi-dotnetcore
.Net Core API web app running on Azure AKS using Visual Studio

- Using Linux image because of Kubernetes
- Using Visual Studio Tools to avouid Kubernetes hell

## Add Kubernetes manifests

Added Kubernetes support from Visual Studio 2017, docker file and helm chart is generated automatically.

Using Visual Studio tools:
- Visual Studio Kubernetes Tools - https://docs.microsoft.com/en-us/visualstudio/containers/tutorial-kubernetes-tools?view=vs-2017
- AKS Publishing Tools - https://aka.ms/get-vsk8spublish

Problems with Tiller for Helm
- run helm init tiller --upgrade
- reinstal https://helm.sh/docs/using_helm/#deleting-or-reinstalling-tiller

## Publish service to internet

If you want to publish server externally, you have to install Nginx as ingress controller and modify charts.

Changed values.yaml to ingress=true and hosts with Nginx public ip.

Install Nginx as new ingress controller
```
helm install --name nginx-ingress stable/nginx-ingress
```
![Nginx ingress](media/ingress.png)


## Deploy to Kubernetes

Click on Solution folder and select Publish to Azure AKS. There is created .akspub publishing profile.

![Publish to AKS](media/publish-to-aks.png)

or use commandline

```
helm install --name jjwebapicore "C:\Users\jajindri\source\repos\jjazure-web-dotnetcore\src-webapi\\jjwebapicore\charts\jjwebapicore" --set-string image.repository=jjcontainers.azurecr.io/jjwebapicore --set-string image.tag=2019032503 --kube-context jjaks
```

Now check public IP address fou our service
http://51.136.52.198.xip.io/api/values

## Add Application Insights telemetry
**TODO** add Application Insights and see telemetry

## Add reference to another API service running on cluster
**TODO** two different API services calling each other - reference in manifest
