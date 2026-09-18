terraform {
  backend "azurerm" {
    resource_group_name  = "cvchatbot-tfstate-rg"
    storage_account_name = "cvchatbotstorageaccount"
    container_name       = "cvchatbot-tfstate"
    key                  = "backend.tfstate"
  }
}