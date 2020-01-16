
set version=%1
set key=%2

cd %~dp0
dotnet build magic.lambda.logging/magic.lambda.logging.csproj --configuration Release --source https://api.nuget.org/v3/index.json
dotnet nuget push magic.lambda.logging/bin/Release/magic.lambda.logging.%version%.nupkg -k %key% -s https://api.nuget.org/v3/index.json
