# jjazure-web-dotnetcore-windows
Azure Web App and DotNet Core API website

This project is created with Visual Studio 2017 on Docker Windows base image.

## Check container running locally

Click Build Docker image on Dockerfile.

```cmd
docker run -p 80:80 -it jjwebcorewindows:latest
```

Test it on http://localhost/api/values

## Publish Docker image into Azure Container Registry

Click Publish in Visual Studio.

## Deploy to WebApp

Create Azure Web App for Containers service. Select Container from Azure Container Registry.

Test it on https://jjwebcore.azurewebsites.net/api/values

## Deploy to Azure Kubernetes Service (AKS)

How to deploy to AKS https://docs.microsoft.com/en-us/azure/aks/windows-container-cli

### Add Kubernetes manifests

Added Kubernetes support from Visual Studio 2017, docker file and helm chart is generated automatically.

Using Visual Studio tools:
- Visual Studio Kubernetes Tools - https://docs.microsoft.com/en-us/visualstudio/containers/tutorial-kubernetes-tools?view=vs-2017
- AKS Publishing Tools - https://aka.ms/get-vsk8spublish

### Deploy to Kubernetes

```
helm install --name jjwebcorewindows jjwebcorewindows/charts/jjwebcorewindows --set-string image.repository=jjcontainers.azurecr.io/jjwebcorewindows --set-string image.tag=20190529085235
```

How to validate final template ?
```
helm template
```

Now check public IP address for our service http://your_ip/api/values

```
kubectl get svc --all-namespaces
```


