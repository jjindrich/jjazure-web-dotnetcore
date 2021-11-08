param envName string = 'jjweb-env'
param location string = 'North Europe'

param logName string = 'jjdev-analytics'
param logResourceGroupName string = 'jjdevmanagement'

resource log 'Microsoft.OperationalInsights/workspaces@2021-06-01' existing = {
  name: logName
  scope: resourceGroup(logResourceGroupName)
}

resource env 'Microsoft.Web/kubeEnvironments@2021-02-01' = {
  name: envName
  location: location
  properties: {
    type: 'managed'
    internalLoadBalancerEnabled: false
    appLogsConfiguration:{
      destination:'log-analytics'
      logAnalyticsConfiguration:{
        customerId: log.properties.customerId
        sharedKey: log.listKeys().primarySharedKey
      }
    }
  }
}
