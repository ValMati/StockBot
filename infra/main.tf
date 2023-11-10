resource "azurerm_resource_group" "rg" {
  name     = "v-stockbot-rg-${var.environment}"
  location = var.resource_group_location

  tags = {
    environment = "${var.environment}"
  }
}

resource "azurerm_container_group" "container" {
  name                = "v-stockbot-acig-${var.environment}"
  location            = azurerm_resource_group.rg.location
  resource_group_name = azurerm_resource_group.rg.name
  ip_address_type     = "Public"
  os_type             = "Linux"
  restart_policy      = "OnFailure"

  image_registry_credential {
    username = var.imgreg_username
    password = var.imgreg_password
    server   = var.imgreg_server
  }

  container {
    name   = "v-stockbot-aci-${var.environment}"
    image  = "${var.imgreg_server}/stockbot/stockbot:${var.image_version}"
    cpu    = var.cpu_cores
    memory = var.memory_in_gb

    ports {
      port     = 443
      protocol = "TCP"
    }

    secure_environment_variables = {
      BotConfig__Token = var.bot_token
    }
  }

  tags = {
    environment = var.environment
  }
}