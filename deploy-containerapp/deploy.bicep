param location string = resourceGroup().location

// Required KeyVault Access Policy: Azure Resource Manager for template deployment
resource kv 'Microsoft.KeyVault/vaults@2022-07-01' existing = {
  name: 'jjazgwakskv'
}

module app 'deploy-app.bicep' = {
  name: 'jjweb-deployment'
  params:{
    location: location
    imageRegistryName: 'jjazgwacr'
    imageWeb: 'jjwebcore:43f39f8847e996388df88b3066dc2af282a04c92'
    imageApi: 'jjwebapicore:f5910752b286b5d3b544b0f56c70ccacdd17be7e'    
    appConfigConnectionString: kv.getSecret('appConfig')
    appInsightsConnectionString: kv.getSecret('appInsightsConfig')    
    contactContextConnectionString: kv.getSecret('contactsDbConnection')
    logResourceGroupName: 'jjinfra-gw-rg'
    logName: 'jjazgwworkspace'
  }
}
