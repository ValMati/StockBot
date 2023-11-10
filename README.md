# StockBot

[![Build](https://github.com/ValMati/StockBot/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/ValMati/StockBot/actions/workflows/build.yml)

A Telegram bot for accessing stock price data

# Deploy

In this repository, in the *infra* folder, the necessary files are provided to deploy the generated Docker image to an Azure Container Instance by following the steps below.

## Prerequisites

### OpenTofu

If you do not have OpenTofu installed, you can do so by following the instructions described in the tool's documentation: [Installing OpenTofu](https://opentofu.org/docs/intro/install)

### AZ Cli

1. If you do not have AZ Cli installed, you can do so by following the instructions described in the tool's documentation: [How to install the Azure CLI](https://learn.microsoft.com/en-gb/cli/azure/install-azure-cli)

1. Log in to your Azure account using one of the available authentication methods ([Sign in with Azure CLI](https://learn.microsoft.com/en-gb/cli/azure/authenticate-azure-cli)) and make sure you have selected the subscription where you want to deploy StockBot.

### Get a Telegram bot token

Follow the steps described in the tutorial to create a bot token and obtain its token. [Obtain Your Bot Token](https://core.telegram.org/bots/tutorial#obtain-your-bot-token)

## Create/Updae infrastructure and deploy the Docker image

Once OpenTofu is installed and AZ is logged in your Azure account, we will go to the infra folder and follow the next steps:

1. Use the files *secrests.tfvars.example* and *vars.tfvars.example* as templates to create your own *secrests.tfvars* and *vars.tfvars*.

1. *[This step is only necessary the first time]* Initialize your OpenTofu environment:
    ```
    $ tofu init
    ```

1. Create the deployment plan:
    ```
    $ tofu plan -out main.tfplan -var-file="secrets.tfvars" -var-file="vars.tfvars"
    ```

    If you are sure that the proposed plan is what you want to execute, continue to the next point.

1. Execute your deployment plan:
    ```
    $ tofu apply main.tfplan
    ```

