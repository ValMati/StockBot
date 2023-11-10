variable "environment" {
  type        = string
  default     = "dev"
  description = "Suffix to distinguish different environments."
}

variable "resource_group_location" {
  type        = string
  default     = "France Central"
  description = "Location for all resources."
}

variable "image_version" {
  type        = string
  description = "Container image version to deploy."
  default     = "0.0.1"
}

variable "cpu_cores" {
  type        = number
  description = "The number of CPU cores to allocate to the container."
  default     = 1
}

variable "memory_in_gb" {
  type        = number
  description = "The amount of memory to allocate to the container in gigabytes."
  default     = 2
}

variable "imgreg_username" {
  type        = string
  description = "User name to access de ACR."
  sensitive   = true
}

variable "imgreg_password" {
  type        = string
  description = "Password to access de ACR."
  sensitive   = true
}

variable "imgreg_server" {
  type        = string
  description = "ACR URL."
  sensitive   = true
}

variable "bot_token" {
  type        = string
  description = "Bot's token"
  sensitive   = true
}