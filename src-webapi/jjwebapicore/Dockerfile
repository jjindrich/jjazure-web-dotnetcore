FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# fix on alpine image - https://www.gitmemory.com/issue/dotnet/SqlClient/220/535820849
RUN apk add icu-libs
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["jjwebapicore/jjwebapicore.csproj", "jjwebapicore/"]
RUN dotnet restore "jjwebapicore/jjwebapicore.csproj"
COPY . .
WORKDIR "/src/jjwebapicore"
RUN dotnet build "jjwebapicore.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "jjwebapicore.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "jjwebapicore.dll"]