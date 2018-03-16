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
* Install .NET Core Windows Server Hosting bundle https://aka.ms/dotnetcore-2-windowshosting
* CMD -> net stop was /y  ->  net start w3svc

### Web Deploy:
* Install WebDeploy_amd64_en-US.msi https://www.microsoft.com/en-us/download/details.aspx?id=43717 (Complete)
* IIS: Management Service -> Enable Remote Connections
