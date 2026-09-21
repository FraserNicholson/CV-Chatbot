output "backend_url" {
  value = azurerm_container_app.backend.latest_revision_fqdn
}

output "frontend_url" {
  value = "https://${azurerm_static_web_app.frontend.default_host_name}"
}

output "frontend_deployment_token" {
  value     = azurerm_static_web_app.frontend.api_key
  sensitive = true
}
