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