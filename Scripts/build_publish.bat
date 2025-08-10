@echo off
setlocal enabledelayedexpansion

:: Configuration - UPDATE THESE VALUES
set "REPO_URL=https://github.com/Bercek71/ReGen"
set "PROJECT_NAME=ReGen"
set "PACK_ID=ReGen"
set "PROJECT_FILE=../ReGen.csproj"
set "PUBLISH_DIR=%~dp0publish"
set "RELEASES_DIR=%~dp0releases"
set "CHANNEL=win"

:: Get version from command line argument
if "%~1"=="" (
    echo Version number is required.
    echo Usage: %~nx0 [version] [github_token]
    echo Example: %~nx0 1.0.0
    echo Example: %~nx0 1.0.0 ghp_your_token_here
    exit /b 1
)
set "VERSION=%~1"

:: Get GitHub token from parameter or environment or prompt
set "TOKEN=%~2"
if "!TOKEN!"=="" (
    set "TOKEN=%GITHUB_TOKEN%"
)
if "!TOKEN!"=="" (
    echo.
    echo GitHub token is required for uploading to releases.
    echo You can:
    echo 1. Pass it as second parameter: %~nx0 %VERSION% your_token_here  
    echo 2. Set GITHUB_TOKEN environment variable
    echo 3. Skip upload and do it manually
    echo.
    set /p "TOKEN=Enter your GitHub token (or press Enter to skip upload): "
)

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
echo Channel: %CHANNEL%
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

:: Build Step 2: Create Velopack Release (removed -f framework flag that was causing issues)
echo.
echo Building Velopack Release v%VERSION%
vpk pack -u "%PACK_ID%" -v "%VERSION%" -o "%RELEASES_DIR%" -p "%PUBLISH_DIR%" --channel "%CHANNEL%"
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
if "!TOKEN!"=="" (
    vpk download github --repoUrl "%REPO_URL%" --channel "%CHANNEL%"
) else (
    vpk download github --repoUrl "%REPO_URL%" --channel "%CHANNEL%" --token "!TOKEN!"
)
if errorlevel 1 (
    echo Warning: Could not download existing releases (this is normal for first release)
)

:: Upload Step 3: Upload to GitHub
if "!TOKEN!"=="" (
    echo.
    echo ==========================================
    echo SKIPPING UPLOAD - No GitHub token provided
    echo ==========================================
    echo Your release files are ready in: %RELEASES_DIR%
    echo.
    echo To upload manually:
    echo 1. Go to: %REPO_URL%/releases/new
    echo 2. Tag: v%VERSION%
    echo 3. Title: %PROJECT_NAME% %VERSION%
    echo 4. Upload these files:
    dir /b "%RELEASES_DIR%\*.exe" "%RELEASES_DIR%\*.nupkg" "%RELEASES_DIR%\RELEASES" 2>nul
    echo ==========================================
    goto :success
)

echo.
echo Uploading to GitHub releases with provided token...
vpk upload github --repoUrl "%REPO_URL%" --publish --releaseName "%PROJECT_NAME% %VERSION%" --tag "v%VERSION%" --channel "%CHANNEL%" --token "!TOKEN!"
if errorlevel 1 (
    echo.
    echo ==========================================
    echo UPLOAD FAILED
    echo ==========================================
    echo Possible issues:
    echo 1. Invalid GitHub token
    echo 2. No permission to create releases
    echo 3. Network connectivity issue
    echo 4. Token might need 'repo' scope permissions
    echo.
    echo Please verify your token has these permissions:
    echo - repo (full control of repositories)
    echo.
    echo Manual upload option:
    echo Go to: %REPO_URL%/releases/new
    echo Upload files from: %RELEASES_DIR%
    echo ==========================================
    exit /b 1
)

:success

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