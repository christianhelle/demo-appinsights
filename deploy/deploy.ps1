param (
    [Parameter(Mandatory=$false)]
    [ValidateNotNullOrEmpty()]
    [ValidateSet('DevTest', 'Production')]
    [string]
    $environmentType = "DevTest",

    [Parameter(Mandatory=$true)]
    [ValidateNotNullOrEmpty()]
    [string]
    $environmentName
)



#############################################################################################
# Configure names and options
#############################################################################################
Write-Host "Initialize deployment" -ForegroundColor Blue

# import utility functions
. ".\deploy.utilities.ps1"
. ".\deploy.naming.ps1"

Write-Host "  Installing required extensions" -ForegroundColor DarkYellow
$output = az extension add `
  --name application-insights `
  --yes

Throw-WhenError -output $output

$output = az extension add `
  --name storage-preview `
  --yes

Throw-WhenError -output $output

# Location
$location = "westeurope"


#############################################################################################
# Resource naming section
#############################################################################################

# Environment Resource Names
$envResourceGroupName   = Get-ResourceGroupName -systemName "JyskebankDemo" -environmentName $environmentName

# Resource Names
$resourceGroupName      = Get-ResourceGroupName -serviceName "Api" -systemName "Weather" -environmentName $environmentName
$resourceName           = Get-ResourceName -serviceAbbreviation "demo" -companyAbbreviation "jb" -systemAbbreviation "api" -environmentName $environmentName
$appServicePlanName     = $resourceName
$gatewayApiName         = $resourceName
$weatherServiceApiName  = $resourceName + "weather"
$insightsName           = $resourceName

# Write setup
Write-Host "**********************************************************************" -ForegroundColor White
Write-Host "* Environment Resource Group            : $envResourceGroupName" -ForegroundColor White
Write-Host "* Environment name                      : $environmentName" -ForegroundColor White
Write-Host "* Resource group name                   : $resourceGroupName" -ForegroundColor White
Write-Host "* Application Insights Name             : $insightsName" -ForegroundColor White
Write-Host "* App service plan name                 : $appServicePlanName" -ForegroundColor White
Write-Host "* Gateway API Name                      : $gatewayApiName" -ForegroundColor White
Write-Host "* Weather Service API Name              : $weatherServiceApiName" -ForegroundColor White
Write-Host "**********************************************************************" -ForegroundColor White


#############################################################################################
# Provision resource group
#############################################################################################
Write-Host "Provision resource group" -ForegroundColor DarkGreen

Write-Host "  Creating resource group" -ForegroundColor DarkYellow
$output = az group create `
  --name $resourceGroupName `
  --location $location `
  --tags $resourceTags

Throw-WhenError -output $output


#############################################################################################
# Provision app service plan
#############################################################################################
Write-Host "Provision app service plan" -ForegroundColor DarkGreen

$sku = 'S1'
if ($environmentType -eq 'Production') {
  $sku = 'P1V2'
}
Write-Host "  Create app service plan" -ForegroundColor DarkYellow
$appServicePlanId = az appservice plan create `
  --name $appServicePlanName `
  --location $location `
  --resource-group $resourceGroupName `
  --sku $sku `
  --tags $resourceTags `
  --query id
  Throw-WhenError -output $appServicePlanId


#############################################################################################
# Provision application insights resource
#############################################################################################
Write-Host "Provision application insights" -ForegroundColor DarkGreen

Write-Host "  Creating application insights" -ForegroundColor DarkYellow
$output = az monitor app-insights component create `
  --app $insightsName `
  --location $location `
  --resource-group $resourceGroupName `
  --application-type web `
  --kind web `
  --tags $resourceTags

Throw-WhenError -output $output

Write-Host "Getting instrumentation key" -ForegroundColor DarkGreen
$instrumentationKey = az resource show `
  --name $insightsName `
  --resource-group $resourceGroupName `
  --resource-type "Microsoft.Insights/components" `
  --query properties.InstrumentationKey `
  --output tsv
  

#############################################################################################
# Provision Gateway App Service
#############################################################################################
Write-Host "Provision Gateway App Service" -ForegroundColor DarkGreen

Write-Host "  Create web app" -ForegroundColor DarkYellow
$output = az webapp create `
  --name $gatewayApiName `
  --resource-group $resourceGroupName `
  --plan $appServicePlanId `
  --tags $resourceTags

Throw-WhenError -output $output

Write-Host "  Configure web app" -ForegroundColor DarkYellow
$output = az webapp config set `
 --name $gatewayApiName `
 --resource-group $resourceGroupName `
 --min-tls-version '1.2' `
 --use-32bit-worker-process false

Throw-WhenError -output $output

Write-Host "  Allow cross-origin resource sharing (CORS)" -ForegroundColor DarkYellow

$output = az webapp cors remove `
  --name $gatewayApiName `
  --resource-group $resourceGroupName `
  --allowed-origins *

Throw-WhenError -output $output

$output = az webapp cors add `
  --name $gatewayApiName `
  --resource-group $resourceGroupName `
  --allowed-origins *

Throw-WhenError -output $output

Write-Host "  Apply web app settings" -ForegroundColor DarkYellow
$output = az webapp config appsettings set `
 --name $gatewayApiName `
 --resource-group $resourceGroupName `
 --settings `
   ApplicationInsights__InstrumentationKey=$instrumentationKey `
   APPINSIGHTS_INSTRUMENTATIONKEY=$instrumentationKey `
   ServiceOptions__EnvironmentName=$environmentName `
   ServiceOptions__EnvironmentType=$environmentType

  
#############################################################################################
# Provision Weather App Service
#############################################################################################
Write-Host "Weather Gateway App Service" -ForegroundColor DarkGreen

Write-Host "  Create web app" -ForegroundColor DarkYellow
$output = az webapp create `
  --name $weatherServiceApiName `
  --resource-group $resourceGroupName `
  --plan $appServicePlanId `
  --tags $resourceTags

Throw-WhenError -output $output

Write-Host "  Configure web app" -ForegroundColor DarkYellow
$output = az webapp config set `
 --name $weatherServiceApiName `
 --resource-group $resourceGroupName `
 --min-tls-version '1.2' `
 --use-32bit-worker-process false

Throw-WhenError -output $output

Write-Host "  Allow cross-origin resource sharing (CORS)" -ForegroundColor DarkYellow

$output = az webapp cors remove `
  --name $weatherServiceApiName `
  --resource-group $resourceGroupName `
  --allowed-origins *

Throw-WhenError -output $output

$output = az webapp cors add `
  --name $weatherServiceApiName `
  --resource-group $resourceGroupName `
  --allowed-origins *

Throw-WhenError -output $output

Write-Host "  Apply web app settings" -ForegroundColor DarkYellow
$output = az webapp config appsettings set `
 --name $weatherServiceApiName `
 --resource-group $resourceGroupName `
 --settings `
   ApplicationInsights__InstrumentationKey=$instrumentationKey `
   APPINSIGHTS_INSTRUMENTATIONKEY=$instrumentationKey `
   ServiceOptions__EnvironmentName=$environmentName `
   ServiceOptions__EnvironmentType=$environmentType