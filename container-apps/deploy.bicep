// Startup Bicep script to get KeyVautl values
// Required KeyVault Access Policy: Azure Resource Manager for template deployment
 

resource kv 'Microsoft.KeyVault/vaults@2021-06-01-preview' existing = {
  name: 'jjakskv'
}

module app 'deploy-app.bicep' = {
  name: 'jjweb-deployment'
  params:{
    imageWeb: 'jjwebcore:eb36aa5bba1bf8504c38260c11a3a52962979e0c'
    imageApi: 'jjwebapicore:0e45305890f335a1b72203a1b40662d221209e9a'
    appConfigConnectionString: kv.getSecret('appConfig')
    appInsightsConnectionString: kv.getSecret('appInsightsConfig')    
    appInsightsInstrumentationKey: kv.getSecret('appInsightsKey')
    contactContextConnectionString: kv.getSecret('contactsDbConnection')
  }
}
