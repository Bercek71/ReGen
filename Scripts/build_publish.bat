@echo off
setlocal enabledelayedexpansion

:: Configuration - UPDATE THESE VALUES
set "REPO_URL=https://github.com/Bercek71/ReGen"
set "PROJECT_NAME=ReGen"
set "PACK_ID=VelopackCSharpWpf"
set "PROJECT_FILE=../ReGen.csproj"
set "PUBLISH_DIR=%~dp0publish"
set "RELEASES_DIR=%~dp0releases"
set "FRAMEWORK=net9.0-windows"

:: Get version from command line argument
if "%~1"=="" (
    echo Version number is required.
    echo Usage: %~nx0 [version]
    echo Example: %~nx0 1.0.0
    exit /b 1
)
set "VERSION=%~1"

echo ====================================
echo Velopack Build and GitHub Upload
echo ====================================
echo Repo URL: %REPO_URL%
echo Project: %PROJECT_NAME%
echo Pack ID: %PACK_ID%
echo Version: %VERSION%
echo Project File: %PROJECT_FILE%
echo Publish Dir: %PUBLISH_DIR%
echo Releases Dir: %RELEASES_DIR%
echo Framework: %FRAMEWORK%
echo ====================================
echo.

:: Build Step 1: Compile and Publish
echo Compiling %PROJECT_NAME% with dotnet...
dotnet publish "%PROJECT_FILE%" -c Release -o "%PUBLISH_DIR%"
if errorlevel 1 (
    echo Error: Failed to publish application
    exit /b 1
)
echo Successfully published to %PUBLISH_DIR%

:: Build Step 2: Create Velopack Release
echo.
echo Building Velopack Release v%VERSION%
vpk pack -u "%PACK_ID%" -v "%VERSION%" -o "%RELEASES_DIR%" -p "%PUBLISH_DIR%" -f "%FRAMEWORK%"
if errorlevel 1 (
    echo Error: Failed to create Velopack release
    exit /b 1
)
echo Successfully created Velopack release

:: Upload Step 1: Install vpk if not already installed
echo.
echo Installing/Updating vpk tool...
dotnet tool install -g vpk 2>nul || dotnet tool update -g vpk
if errorlevel 1 (
    echo Error: Failed to install/update vpk tool
    exit /b 1
)

:: Upload Step 2: Download existing releases (for delta updates)
echo.
echo Downloading existing releases...
vpk download github --repoUrl "%REPO_URL%"
if errorlevel 1 (
    echo Warning: Could not download existing releases (this is normal for first release)
)

:: Upload Step 3: Upload to GitHub
echo.
echo Uploading to GitHub releases...
vpk upload github --repoUrl "%REPO_URL%" --publish --releaseName "%PROJECT_NAME% %VERSION%" --tag "v%VERSION%"
if errorlevel 1 (
    echo Error: Failed to upload to GitHub releases
    echo Make sure you have the GITHUB_TOKEN environment variable set or are authenticated with GitHub CLI
    exit /b 1
)

echo.
echo ====================================
echo SUCCESS! Build and Upload Complete
echo ====================================
echo Release Name: %PROJECT_NAME% %VERSION%
echo Tag: v%VERSION%
echo URL: %REPO_URL%/releases/tag/v%VERSION%
echo.
echo Local releases folder: %RELEASES_DIR%
echo.
echo Your UpdateManager URL should be:
echo "%REPO_URL%/releases/latest/download"
echo ====================================

pause