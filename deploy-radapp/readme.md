# Deployment using Radius

Deploy jjweb using Radius

Docs
- https://docs.radapp.io/
- https://docs.radapp.io/tutorials/new-app/
- https://docs.radapp.io/guides/author-apps/azure/azure-connection/

## Install and initialize environment

```powershell
iwr -useb "https://raw.githubusercontent.com/radius-project/radius/main/deploy/install.ps1" | iex
$Env:Path = [System.Environment]::GetEnvironmentVariable("Path","User")
```

Install Radius into AKS cluster
- environment: jjradapp
- namespace: jjradapp

```powershell
rad init -full
rad env list
```

## Deploy sample application

```powershell
rad deploy app-sample.bicep

rad app list

rad run app.bicep

rad env delete jjenv 
```