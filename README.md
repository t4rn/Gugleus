# Gugleus

### Tech stack:

* ASP.NET Core 2.0
* Autofac
* AutoMapper
* Npgsql
* Entity Framework Core
* Dapper
* xUnit
* Moq
* AutoFixture
* FluentAssertions
* Swashbuckle (Swagger)
* IMemoryCache
* NLog

### Hosting on IIS [https://docs.microsoft.com/en-us/aspnet/core/publishing/iis?tabs=aspnetcore2x]:
* Install .NET SDK https://download.microsoft.com/download/D/8/1/D8131218-F121-4E13-8C5F-39B09A36E406/dotnet-sdk-2.1.104-win-gs-x64.exe (from https://www.microsoft.com/net/learn/get-started/windows)
* Install .NET Core Windows Server Hosting bundle https://dotnet.microsoft.com/download/thank-you/dotnet-runtime-2.2.1-windows-hosting-bundle-installer [2.0.8: https://aka.ms/dotnetcore-2-windowshosting]
* CMD -> `net stop was /y`  ->  `net start w3svc`

### Web Deploy:
* Install WebDeploy_amd64_en-US.msi https://www.microsoft.com/en-us/download/details.aspx?id=43717 (Complete installation)
* IIS: Management Service -> Enable Remote Connections
* Check, if Server is listening (in CMD):  `netstat -aon | findstr :8172`
* Check, if Remote Agent Service and Web Management Service are running: `net start wmsvc` & `net start msdepsvc`

## Version check:
`dotnet --version`

`dotnet --info`
