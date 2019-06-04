# jjazure-func
Azure Function project running in Azure Managed Kubernetes (AKS). 
This function will process messages from Azure Storage Queue - taking message from orders queue and inserting into processed queue.

https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-function-linux-custom-image

## Create new Azure Function
Use commandline to create function with dockerfile

```
func init jjfunc --docker
cd jjfunc
func new
```

OR 

use Visual Studio Code https://code.visualstudio.com/tutorials/functions-extension/create-function

OR

use Visual Studio 2017/2019 https://docs.microsoft.com/en-us/azure/azure-functions/functions-create-your-first-function-visual-studio

For dotNetCore use this images
- for Linux image mcr.microsoft.com/azure-functions/dotnet:2.0
- for Windows image mcr.microsoft.com/azure-functions/dotnet:2.0-nanoserver-1809

## Build docker image

```
docker build jjfunc -t jjfunc
```

Try to run locally and watch process

```
docker run -it -e QueueStorageAccount='DefaultEndpointsProtocol=https;AccountName=jjfunctionastorage;AccountKey=KEY' jjfunc
```

## Push Docker image to Azure Container Repository
[Documentation how use Azure Container Repository](https://docs.microsoft.com/en-us/azure/container-registry/container-registry-get-started-docker-cli)

```bash
docker login jjcontainers.azurecr.io -u jjcontainers -p <PASSWORD>
docker tag jjfunc jjcontainers.azurecr.io/jjfunc
docker push jjcontainers.azurecr.io/jjfunc
```

## Run web on AKS and call service running on AKS

Added Helm Chart to project

```
cd jjfunc
helm create jjfunc-charts
```

Deploy from command line

!!! Replace connection strings in values.yaml file !!!

```bash
helm install --name jjfunc jjfunc-charts --set-string image.repository=jjcontainers.azurecr.io/jjfunc --set-string image.tag=latest
kubectl describe pods jjfunc
```