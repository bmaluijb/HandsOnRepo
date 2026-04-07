@echo off
:: Change to the directory where this script lives
cd /d "%~dp0"

echo ============================================
echo   NN Pension Planner - Building...
echo ============================================
echo.

:: Ensure .NET 10 SDK is available (downloads if needed)
call "%~dp0setup-dotnet.bat"
if %ERRORLEVEL% neq 0 exit /b 1

dotnet build NNPensionPlanner.csproj
if %ERRORLEVEL% neq 0 (
    echo.
    echo Build FAILED.
    pause
    exit /b 1
)

echo.
echo ============================================
echo   Build succeeded!
echo ============================================
pause
