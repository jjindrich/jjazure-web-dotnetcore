$rg ='jjmicroservices-gw-rg'

az deployment group create -g $rg --template-file deploy.bicep
