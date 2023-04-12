# Upgrade PrivatePreview extension
az extension remove --name containerapp
az extension add --upgrade --source https://containerappsprivatecli.blob.core.windows.net/cliextension/containerapp-private_preview_1.0.5-py2.py3-none-any.whl

# supported locations
az containerapp env workload-profile list-supported -l westeurope -o table
# wait for Dedicated-xyz

# Create Secure ACA
az group create -n aca-rg -l westeurope
$snet = $(az network vnet subnet list --resource-group jjnetwork-rg --vnet-name jjazappvnet --query "[?name=='aca-snet'].id" --output tsv)
az containerapp env create --enableWorkloadProfiles -g aca-rg -n jjaca -l westeurope -s $snet --internal-only