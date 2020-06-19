#!/usr/bin/env bash

# install pwsh if missing
which pwsh > /dev/null 2>&1
if [[ $? -eq 1 ]] ; then
echo "Install pwsh"
bash <(curl -s https://raw.githubusercontent.com/PowerShell/PowerShell/master/tools/install-powershell.sh)
fi

# Verify pwsh install
which pwsh > /dev/null 2>&1
if [[ $? -eq 1 ]] ; then
echo "Unable to install pwsh"
exit $?
fi
