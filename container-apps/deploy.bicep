param location string = resourceGroup().location

// Required KeyVault Access Policy: Azure Resource Manager for template deployment
resource kv 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: 'jjazakskv'
}

module app 'deploy-app.bicep' = {
  name: 'jjweb-deployment'
  params:{
    location: location
    imageWeb: 'jjwebcore:8655cb8f20c29af98e8c1e46d7d3342aa9647b13'
    imageApi: 'jjwebapicore:8655cb8f20c29af98e8c1e46d7d3342aa9647b13'
    appConfigConnectionString: kv.getSecret('appConfig')
    appInsightsConnectionString: kv.getSecret('appInsightsConfig')    
    appInsightsInstrumentationKey: kv.getSecret('appInsightsKey')
    contactContextConnectionString: kv.getSecret('contactsDbConnection')
  }
}
