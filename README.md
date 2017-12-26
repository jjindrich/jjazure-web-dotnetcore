# jjazure-web-dotnetcore
Azure Web App and DotNet Core website

## Create new web site for Visual Studio Code
Use this article create new project - 
https://docs.microsoft.com/en-us/aspnet/core/tutorials/razor-pages-vsc/razor-pages-start

```
dotnet new razor -o jjwebcore
```

## Build and deploy web site
You can open project in Visual Studio Code or Visual Studio 2017. Project has Docker support files - Dockerbuild and Compose.

### Visual Studio 2017
Simply select Publish from context menu. Two options:
1. Select Azure Web App
2. Select Azure Container Registry

### Visual Studio Code
#### Docker for non-root user
Add user into docker group - [link](https://docs.docker.com/engine/installation/linux/linux-postinstall/#manage-docker-as-a-non-root-user)
```bash
sudo usermod -aG docker $USER
```
Run from command line - TODO

#### Build docker project
[Compile docker project](https://docs.microsoft.com/en-us/dotnet/core/docker/building-net-docker-images)
```bash
cd jjwebcore
docker build -t jjwebcore .
```