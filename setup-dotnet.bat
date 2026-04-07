@echo off
:: Checks if .NET 10 SDK is available locally or on the system.
:: If not, downloads it to .dotnet/ in the repo (no admin, no system install).

set DOTNET_SDK_VERSION=10.0.201

:: Check if the local .dotnet folder already has the SDK
if exist "%~dp0.dotnet\dotnet.exe" (
    set DOTNET_ROOT=%~dp0.dotnet
    set PATH=%~dp0.dotnet;%PATH%
    goto :done
)

:: Check if dotnet is already on the system with the right major version
where dotnet >nul 2>&1
if %ERRORLEVEL% equ 0 (
    for /f "tokens=1 delims=." %%a in ('dotnet --version 2^>nul') do (
        if "%%a"=="10" goto :done
    )
)

:: .NET 10 not found — download it
echo.
echo ============================================
echo   .NET 10 SDK not found on this machine.
echo   Downloading to .dotnet\ folder...
echo   (This only happens once, ~300 MB)
echo ============================================
echo.

:: Download the official dotnet-install script
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "Invoke-WebRequest -Uri 'https://dot.net/v1/dotnet-install.ps1' -OutFile '%TEMP%\dotnet-install.ps1' -UseBasicParsing"

if %ERRORLEVEL% neq 0 (
    echo ERROR: Failed to download the .NET install script. Check your internet connection.
    pause
    exit /b 1
)

:: Install .NET SDK locally (no admin, no system changes)
powershell -NoProfile -ExecutionPolicy Bypass -Command ^
    "& '%TEMP%\dotnet-install.ps1' -Version '%DOTNET_SDK_VERSION%' -InstallDir '%~dp0.dotnet' -NoPath"

if %ERRORLEVEL% neq 0 (
    echo ERROR: Failed to install .NET SDK. Check the errors above.
    pause
    exit /b 1
)

echo.
echo .NET 10 SDK installed to .dotnet\ folder.
echo.

set DOTNET_ROOT=%~dp0.dotnet
set PATH=%~dp0.dotnet;%PATH%

:done
:: Suppress telemetry and first-run experience
set DOTNET_CLI_TELEMETRY_OPTOUT=1
set DOTNET_NOLOGO=1
set DOTNET_SKIP_FIRST_TIME_EXPERIENCE=1
