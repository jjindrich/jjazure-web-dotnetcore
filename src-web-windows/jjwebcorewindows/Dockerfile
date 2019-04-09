#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

# Recommended images for dotNet Core on Windows
# https://azure.microsoft.com/en-us/blog/windows-server-2019-support-now-available-for-windows-containers-on-azure-app-service/
# images: https://hub.docker.com/_/microsoft-dotnet-core-sdk/
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
#FROM microsoft/dotnet:2.2-aspnetcore-runtime-nanoserver-1803 AS base
WORKDIR /app
EXPOSE 80
#EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 as build
#FROM microsoft/dotnet:2.2-sdk-nanoserver-1803 AS build
WORKDIR /src
COPY ["jjwebcorewindows/jjwebcorewindows.csproj", "jjwebcorewindows/"]
RUN dotnet restore "jjwebcorewindows/jjwebcorewindows.csproj"
COPY . .
WORKDIR "/src/jjwebcorewindows"
RUN dotnet build "jjwebcorewindows.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "jjwebcorewindows.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "jjwebcorewindows.dll"]