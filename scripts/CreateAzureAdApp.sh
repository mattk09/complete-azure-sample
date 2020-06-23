#!/bin/bash
###########################################################################
# Creates an Azure Ad Application, using AccessTokenV2
# Creates app-manifest.json with the application's manifest
# Export the Apps configuration at the end of the process
###########################################################################
source ./AzureAdFunctions.sh
set -euo pipefail
IFS=$'\n\t'
appName=""
appPassword=$(uuidgen)
manifestFile="app-manifest.json"

# -e: immediately exit if any command has a non-zero exit status
# -o: prevents errors in a pipeline from being masked
# IFS new value is less likely to cause confusing bugs when looping arrays or arguments (e.g. $@)

usage() { echo "Usage: $0 --app-name=<app name>" 1>&2; exit 1; }

# Initialize parameters specified from command line
while [ $# -gt 0 ]; do
  param=$(echo $1 | cut -f1 -d=)
  paramValue=$(echo $1 | cut -f2 -d=)
  case "$param" in
        --ap-name) 
          appName=${paramValue} ;; 
        *)
  esac
  shift
done

if [ -z "$appName" ]; then
	echo "Please check the parameters and try again"
	usage
fi
echo "Using the current context for az command"

#Creating ADD App
CreateAzureADApp $appName $manifestFile $appPassword "https://localhost:44329/login" ""
appServerId=$(grep -Po '"'"appId"'"\s*:\s*"\K([^"]*)' $manifestFile)
appServerObjectId=$(grep -Po '"'"objectId"'"\s*:\s*"\K([^"]*)' $manifestFile)
appServerDomain=$(grep -Po '"'"publisherDomain"'"\s*:\s*"\K([^"]*)' $manifestFile)
tenantId=$(grep -Po '"'"odata.metadata"'"\s*:\s*"https://graph.windows.net/\K([^/]*)' $manifestFile)

AddRedirectUrl $appServerObjectId "https://localhost:44329/login" "https://*.azurewebsites.net/login"

echo '-------------APPLICATION-------------------'
echo 'MANIFEST_FILE     :'$manifestFile
echo 'TENANT_ID         :'$tenantId
echo 'DOMAIN            :'$appServerDomain
echo 'APP_NAME          :'$appName
echo 'OBJECT_ID         :'$appServerObjectId
echo 'CLIENT_ID         :'$appServerId
echo 'CLIENT_SECRET     :'$appPassword