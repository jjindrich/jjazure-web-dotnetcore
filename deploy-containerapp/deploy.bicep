param location string = resourceGroup().location

// Required KeyVault Access Policy: Azure Resource Manager for template deployment
resource kv 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: 'jjazakskv'
}

module app 'deploy-app.bicep' = {
  name: 'jjweb-deployment'
  params:{
    location: location
    imageWeb: 'jjwebcore:c7d5f95a30fb4c7413551dd388ae309949a70b21'
    imageApi: 'jjwebapicore:b68ef92cd72cd20e21e32979a53530a32199491a'
    appConfigConnectionString: kv.getSecret('appConfig')
    appInsightsConnectionString: kv.getSecret('appInsightsConfig')    
    contactContextConnectionString: kv.getSecret('contactsDbConnection')
  }
}
