terraform {
  backend "azurerm" {
    resource_group_name  = "jjdevmanagement"
    storage_account_name = "jjtfstate"
    container_name       = "jmeter"
    key                  = "terraform.tfstate"
  }
  required_providers {
    azurerm    = "~> 2.26"
  }
}

provider "azurerm" {
  version = "=2.26.0"
  features {}
}

provider "random" {
  version = "=2.2.1"
}
