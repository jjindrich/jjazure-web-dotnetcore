#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-alpine AS build
WORKDIR /src
COPY ["jjwebcore/jjwebcore.csproj", "jjwebcore/"]
RUN dotnet restore "jjwebcore/jjwebcore.csproj"
COPY . .
WORKDIR "/src/jjwebcore"
RUN dotnet build "jjwebcore.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "jjwebcore.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "jjwebcore.dll"]