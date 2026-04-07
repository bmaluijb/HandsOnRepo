@echo off
:: Change to the directory where this script lives
cd /d "%~dp0"

echo ============================================
echo   NN Pension Planner - Starting...
echo ============================================
echo.

:: Ensure .NET 10 SDK is available (downloads if needed)
call "%~dp0setup-dotnet.bat"
if %ERRORLEVEL% neq 0 exit /b 1

echo Building the application...
dotnet build NNPensionPlanner.csproj --nologo -verbosity:quiet
if %ERRORLEVEL% neq 0 (
    echo.
    echo ============================================
    echo   Build FAILED. Check the errors above.
    echo ============================================
    pause
    exit /b 1
)

echo.
echo ============================================
echo   Build succeeded!
echo   Opening browser to http://localhost:5000
echo   Press Ctrl+C to stop the application.
echo ============================================
echo.

:: Open browser after a short delay
start "" http://localhost:5000

:: Run the application
dotnet run --project NNPensionPlanner.csproj --no-build --urls http://localhost:5000

:: If we get here, the app exited — keep window open so user can see why
echo.
echo Application stopped.
pause
