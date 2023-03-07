param location string = resourceGroup().location

// Required KeyVault Access Policy: Azure Resource Manager for template deployment
resource kv 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: 'jjazakskv'
}

module app 'deploy-app.bicep' = {
  name: 'jjweb-deployment'
  params:{
    location: location
    imageWeb: 'jjwebcore:335f8aa8faeac5cd09e5d61a18c510f995975616'
    imageApi: 'jjwebapicore:122822df55b2d8fc63c2e8195187d4b858a85259'
    appConfigConnectionString: kv.getSecret('appConfig')
    appInsightsConnectionString: kv.getSecret('appInsightsConfig')    
    appInsightsInstrumentationKey: kv.getSecret('appInsightsKey')
    contactContextConnectionString: kv.getSecret('contactsDbConnection')
  }
}
