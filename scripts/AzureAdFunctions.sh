#!/bin/bash
###########################################################################
# Fuctions to create and udpdate Azure Active Directory
###########################################################################
set -euo pipefail
IFS=$'\n\t'

CreateAzureADApp(){
  local appName=$1
  local manifestFileName=$2
  local appPassword=$3
  local replyUrl=$4
  local appResourceAccessFileName=$5
  local appObjectId= 

  echo "Creating Azure AD Application "$appName
  az ad app create --display-name $appName \
  --identifier-uris "https://$appName" \
  --password $appPassword \
  --reply-urls $replyUrl > $manifestFileName
  
  appObjectId=$(grep -Po '"'"objectId"'"\s*:\s*"\K([^"]*)' $manifestFileName)

  if [ ! -z "$appResourceAccessFileName" ]; then
    az ad app update --id $appObjectId \
    --required-resource-accesses @$appResourceAccessFileName
  fi
  echo "Azure AD Application "$appName "created!"
  
  UpdateAccessTokenVersion $appObjectId $appName
}

##Property accessTokenAcceptedVersion in AD Graph, need to update api.requestedAccessTokenVersion in MS Graph that maps to it
UpdateAccessTokenVersion(){
  local appServerObjectId=$1  
  local tokenVersionUpdate=""
  local count=1
  set +e
  while [ -z "$tokenVersionUpdate" ]
  do  
    echo $count' - Trying to update '$2' Manifest to use AccessToken version 2'
    sleep 1
    az rest --method PATCH --uri https://graph.microsoft.com/v1.0/applications/$appServerObjectId --body '{"api":{"requestedAccessTokenVersion": 2}}' > /dev/null 2>&1
    tokenVersionUpdate=$(az rest --method GET --uri https://graph.microsoft.com/v1.0/applications/$appServerObjectId --query "api.requestedAccessTokenVersion" 2> /dev/null)  
    count=$(( $count + 1 ))
  done
  set -e
}

AddRedirectUrl(){
  local appObjectId=$1
  local redirectUrl1=$2
  local redirectUrl2=$3

  echo "Updating Azure AD Application "$appObjectId
  az ad app update --id $appObjectId \
    --reply-urls $redirectUrl1 $redirectUrl2 

  echo "Azure AD Application "$appObjectId "updated!"
  
}
