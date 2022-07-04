param envName string = 'jjwebenv'
param location string = resourceGroup().location

param imageRegistryName string = 'jjakscontainers'
param imageWeb string
param imageApi string

param logName string = 'jjdev-analytics'
param logResourceGroupName string = 'jjdevmanagement'
@secure()
param appConfigConnectionString string
@secure()
param appInsightsConnectionString string
@secure()
param appInsightsInstrumentationKey string
@secure()
param contactContextConnectionString string

// Reference existing resources
resource log 'Microsoft.OperationalInsights/workspaces@2021-06-01' existing = {
  name: logName
  scope: resourceGroup(logResourceGroupName)
}
resource acr 'Microsoft.ContainerRegistry/registries@2021-06-01-preview' existing = {
  name: imageRegistryName
}

// Create Container App Environment
resource env 'Microsoft.App/managedEnvironments@2022-03-01' = {
  name: envName
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: log.properties.customerId
        sharedKey: log.listKeys().primarySharedKey
      }
    }
  }
}

// Create Container App: JJWeb
resource jjweb 'Microsoft.App/containerApps@2022-03-01' = {
  name: '${envName}-jjweb'
  location: location
  properties: {
    managedEnvironmentId: env.id
    configuration: {
      ingress: {
        external: true
        targetPort: 80
        transport: 'auto'
      }
      secrets: [
        {
          name: 'registry-pwd'
          value: acr.listCredentials().passwords[0].value
        }
      ]
      registries: [
        {
          server: acr.properties.loginServer
          username: acr.listCredentials().username
          passwordSecretRef: 'registry-pwd'
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'jjwebcore'
          image: '${acr.properties.loginServer}/${imageWeb}'
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Development'
            }
            {
              name: 'SERVICEAPIROOT_URL'
              value: 'http://${jjapi.properties.configuration.ingress.fqdn}'
            }
            {
              name: 'SERVICEWINAPIROOT_URL'
              value: 'http://not-deployed'
            }
            {
              name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
              value: appInsightsConnectionString
            }
            {
              name: 'ConnectionStrings__AppConfig'
              value: appConfigConnectionString
            }
          ]
          resources: {
            cpu: json('.25')
            memory: '.5Gi'
          }
        }
      ]
    }
  }
}

// Create Container App: JJAPI
resource jjapi 'Microsoft.App/containerApps@2022-03-01' = {
  name: '${envName}-jjapi'
  location: location
  properties: {
    managedEnvironmentId: env.id
    configuration: {
      ingress: {
        external: false
        targetPort: 80
        transport: 'auto'
      }
      secrets: [
        {
          name: 'registry-pwd'
          value: acr.listCredentials().passwords[0].value
        }
      ]
      registries: [
        {
          server: acr.properties.loginServer
          username: acr.listCredentials().username
          passwordSecretRef: 'registry-pwd'
        }
      ]
    }
    template: {
      containers: [
        {
          name: 'jjwebapicore'
          image: '${acr.properties.loginServer}/${imageApi}'
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: 'Development'
            }
            {
              name: 'APPLICATIONINSIGHTS_INSTRUMENTATIONKEY'
              value: appInsightsInstrumentationKey
            }
            {
              name: 'ConnectionStrings_ContactsContext'
              value: contactContextConnectionString
            }
          ]
          resources: {
            cpu: json('.25')
            memory: '.5Gi'
          }
        }
      ]
    }
  }
}
