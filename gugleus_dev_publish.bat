
@echo off
REM variables
set pool="gugleus_dev"
set solutionFolder="C:\Projekty\_kw\Gugleus\"
set source="C:\Projekty\_kw\Gugleus\Gugleus.Api"
set dest="c:\inetpub\wwwroot\gugleus_dev"
set configSource="C:\inetpub\wwwroot\appsettings_dev.json"
set configDest= "%dest%\appsettings.json"
@echo on

@REM stop application pool in IIS
%SYSTEMROOT%\System32\inetsrv\appcmd stop apppool /apppool.name:%pool%

@REM pull from GIT
cd %solutionFolder%
git pull

@REM publish to inetpub
cd %source%
dotnet publish -o %dest%

@REM copy config
copy /y %configSource% %configDest%

@REM start application pool in IIS
%SYSTEMROOT%\System32\inetsrv\appcmd start apppool /apppool.name:%pool%
pause