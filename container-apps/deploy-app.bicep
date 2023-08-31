param envName string = 'jjazacanew'
param location string = resourceGroup().location

param imageRegistryName string = 'jjazacr'
param imageWeb string
param imageApi string

param logName string = 'jjazworkspace'
param logResourceGroupName string = 'jjinfra-rg'
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

// Create managed identity to access ACR and assign role
resource acrIdentity 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-01-31' = {
  name: '${envName}-acr-identity'
  location: location
}
var acrPullRole = resourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
resource acrRole 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, acrIdentity.id, acrPullRole)
  scope: acr
  properties: {
    principalId: acrIdentity.properties.principalId
    principalType: 'ServicePrincipal'
    roleDefinitionId: acrPullRole
  }
}

// resource vnet 'Microsoft.Network/virtualNetworks@2021-02-01' existing = {
//   name: 'jjazappvnet'
//   scope: resourceGroup('jjnetwork-rg')
// }
// resource subnet 'Microsoft.Network/virtualNetworks/subnets@2021-02-01' existing = {
//   parent: vnet
//   name: 'aca-snet'
// }

// Create Container App Environment
resource env 'Microsoft.App/managedEnvironments@2022-11-01-preview' = {
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
    workloadProfiles: [
      {
        name: 'wp-d4'
        workloadProfileType: 'D4'
        minimumCount: 3
        maximumCount: 5
      }
    ]
    // vnetConfiguration: {
    //   infrastructureSubnetId: subnet.id
    // }
    //zoneRedundant: true    
  }
}

// Create Container App: JJWeb
resource jjweb 'Microsoft.App/containerApps@2022-11-01-preview' = {
  name: '${envName}-jjweb'
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${acrIdentity.id}': {}
    }
  }
  properties: {
    environmentId: env.id
    // if not specified, will use Consumption workload profile
    //workloadProfileName: 'wp-d4'
    configuration: {
      ingress: {
        external: true
        targetPort: 80
        transport: 'auto'
      }
      secrets: []
      registries: [
        {
          server: acr.properties.loginServer
          identity: acrIdentity.id
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
resource jjapi 'Microsoft.App/containerApps@2022-11-01-preview' = {
  name: '${envName}-jjapi'
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${acrIdentity.id}': {}
    }
  }
  properties: {
    environmentId: env.id
    configuration: {
      ingress: {
        external: false
        targetPort: 80
        transport: 'auto'
      }
      secrets: []
      registries: [
        {
          server: acr.properties.loginServer
          identity: acrIdentity.id
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
