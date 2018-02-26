
@echo off
REM variables
set pool="gugleus_dev"
set solutionFolder="C:\Projekty\_kw\Gugleus\"
set source="C:\Projekty\_kw\Gugleus\Gugleus.Api"
set dest="c:\inetpub\wwwroot\gugleus_dev"
set configSource="C:\inetpub\wwwroot\appsettings_dev.json"
set configDest= "%dest%\appsettings.json"
set backup="%dest%_backup\"
@echo on

:: remove existing backup
RD %backup% /s/q

:: create backup
xcopy %dest% %backup% /s/h/e/k/f

:: stop application pool in IIS
%SYSTEMROOT%\System32\inetsrv\appcmd stop apppool /apppool.name:%pool%

:: pull from GIT
cd %solutionFolder%
git pull

:: publish to inetpub
cd %source%
dotnet publish -o %dest%

:: copy config
copy /y %configSource% %configDest%

:: start application pool in IIS
%SYSTEMROOT%\System32\inetsrv\appcmd start apppool /apppool.name:%pool%
pause