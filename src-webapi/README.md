# jjazure-webapi-dotnetcore
.Net Core API web app running on Azure AKS using Visual Studio 2017

- Using Linux image because of Kubernetes
- Using Visual Studio Tools to avoid Kubernetes hell

## Add Kubernetes manifests

Added Kubernetes support from Visual Studio 2017, docker file and helm chart is generated automatically.

Using Visual Studio tools:
- Visual Studio Kubernetes Tools - https://docs.microsoft.com/en-us/visualstudio/containers/tutorial-kubernetes-tools?view=vs-2017
- AKS Publishing Tools - https://aka.ms/get-vsk8spublish

## Deploy to Kubernetes

Visual Studio 2017 -> Click on Solution folder and select **Publish to Azure AKS**. There is created .akspub publishing profile.

![Publish to AKS](media/publish-to-aks.png)

or use command line

```
helm install --name jjwebapicore jjwebapicore/charts/jjwebapicore --set-string image.repository=jjcontainers.azurecr.io/jjwebapicore --set-string image.tag=2019051809
```

How to validate final template ?
```
helm template
```

Now check public IP address for our service http://your_ip/api/values

```
kubectl get svc --all-namespaces
```

## Configure DevOps

https://docs.microsoft.com/en-us/azure/devops/pipelines/tasks/deploy/helm-deploy?view=azure-devops#install-command

Configure Azure Pipeline https://docs.microsoft.com/en-us/azure/devops/pipelines/repos/github?view=azure-devops&tabs=yaml

### Build pipeline
![DevOps CI pipeline](media/devops-ci.png)

### Release pipeline


## Add Application Insights telemetry

Follow this instructions to add Application insights - https://github.com/Microsoft/ApplicationInsights-aspnetcore/wiki/Getting-Started-with-Application-Insights-for-ASP.NET-Core#option-2-environment-variable

- change application code to use Application Insights (using environment variable APPINSIGHTS_INSTRUMENTATIONKEY)

- added secret to values.yaml key and value of APPINSIGHTS_INSTRUMENTATIONKEY.

After new deployment you will get this report - e.g. Performance

![Application Insights](media/appinsights.png)
