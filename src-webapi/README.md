# jjazure-webapi-dotnetcore
.Net Core API web app running on Azure AKS using Visual Studio

- Using Linux image because of Kubernetes
- Using Visual Studio Tools to avouid Kubernetes hell

## Add Kubernetes manifests

Added Kubernetes support from Visual Studio 2017, docker file and helm chart is generated automatically.

Using Visual Studio tools:
- Visual Studio Kubernetes Tools - https://docs.microsoft.com/en-us/visualstudio/containers/tutorial-kubernetes-tools?view=vs-2017
- AKS Publishing Tools - https://aka.ms/get-vsk8spublish

## Deploy to Kubernetes

Click on Solution folder and select Publish to Azure AKS. There is created .akspub publishing profile.

![Publish to AKS](media/publish-to-aks.png)

or use commandline

```
helm install --name jjwebapicore "C:\Users\jajindri\source\repos\jjazure-web-dotnetcore\src-webapi\\jjwebapicore\charts\jjwebapicore" --set-string image.repository=jjcontainers.azurecr.io/jjwebapicore --set-string image.tag=<TAG-ID> --kube-context jjaks
```

Now check public IP address fou our service http://your_ip/api/values

## Add Application Insights telemetry

Follow this instructions to add Application insights - https://github.com/Microsoft/ApplicationInsights-aspnetcore/wiki/Getting-Started-with-Application-Insights-for-ASP.NET-Core#option-2-environment-variable

- change application code to use Application Insights (using environment variable APPINSIGHTS_INSTRUMENTATIONKEY)

- added secret to values.yaml key and value of APPINSIGHTS_INSTRUMENTATIONKEY.

After new deployment you will get this report - e.g. Performance

![Application Insights](media/appinsights.png)
