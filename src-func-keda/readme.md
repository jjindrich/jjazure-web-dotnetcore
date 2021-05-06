#jjazure-func-keda
Azure Function project running in Azure Managed Kubernetes (AKS) using KEDA and virtual nodes.

https://docs.microsoft.com/en-us/azure/azure-functions/functions-kubernetes-keda

# Create Azure Function

Follow this tutorial https://docs.microsoft.com/en-us/azure/azure-functions/create-first-function-vs-code-csharp

Created function GetData returning sample data.

Test locally 

```powershell
curl http://localhost:7071/api/GetData
```

# Create AKS using virtual nodes

https://docs.microsoft.com/en-us/azure/aks/virtual-nodes-cli