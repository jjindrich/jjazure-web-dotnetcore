FROM microsoft/aspnetcore:2.0 AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/aspnetcore-build:2.0 AS build
WORKDIR /src
COPY jjwebcore/jjwebcore.csproj jjwebcore/
RUN dotnet restore jjwebcore/jjwebcore.csproj
COPY . .
WORKDIR /src/jjwebcore
RUN dotnet build jjwebcore.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish jjwebcore.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "jjwebcore.dll"]
