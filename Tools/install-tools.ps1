# Create tmp directory
if (-not (Test-Path -Path ./.tmp/)) {
    New-Item -ItemType Directory -Path ./.tmp/ | Out-Null
}

# =============== PROTOC ============== #
Write-Host "Installing protoc..."

$URL = "https://packages.grpc.io/archive/2019/11/6950e15882f28e43685e948a7e5227bfcef398cd-6d642d6c-a6fc-4897-a612-62b0a3c9026b/protoc/grpc-protoc_windows_x64-1.26.0-dev.zip"
$DESTINATION = "./protoc/"

# Create the destination directory if it doesn't exist
if (-not (Test-Path -Path $DESTINATION)) {
    New-Item -ItemType Directory -Path $DESTINATION | Out-Null
}

# Download the zip file using Invoke-WebRequest
Write-Host "Downloading $URL..."
Invoke-WebRequest -Uri $URL -OutFile "./.tmp/grpc-protoc_windows_x64-1.26.0-dev.zip"

# Extract the contents of the zip file using Expand-Archive
Write-Host "Extracting ./.tmp/grpc-protoc_windows_x64-1.26.0-dev.zip..."
Expand-Archive -Path "./.tmp/grpc-protoc_windows_x64-1.26.0-dev.zip" -DestinationPath $DESTINATION

# Check if the extraction was successful
if ($?) {
    Write-Host "DONE"
} else {
    Write-Host "Error: Failed to extract the contents of the zip file."
    exit 1
}

# =============== AAS CORE CODEGEN ============== #
Write-Host "Installing aas-core-codegen..."

$URL = "https://github.com/aas-core-works/aas-core-codegen/releases/download/v0.0.15/aas-core-codegen.0.0.15.win-x64.zip"
$DESTINATION = "./aas-core-codegen/"

# Create the destination directory if it doesn't exist
if (-not (Test-Path -Path $DESTINATION)) {
    New-Item -ItemType Directory -Path $DESTINATION | Out-Null
}

# Download the zip file using Invoke-WebRequest
Write-Host "Downloading $URL..."
Invoke-WebRequest -Uri $URL -OutFile "./.tmp/aas-core-codegen.0.0.15.win-x64.zip"

# Extract the contents of the zip file using Expand-Archive
Write-Host "Extracting ./.tmp/aas-core-codegen.0.0.15.win-x64.zip..."
Expand-Archive -Path "./.tmp/aas-core-codegen.0.0.15.win-x64.zip" -DestinationPath $DESTINATION/../

# Check if the extraction was successful
if ($?) {
    Write-Host "DONE"
} else {
    Write-Host "Error: Failed to extract the contents of the zip file."
    exit 1
}

# =============== AASX PACKAGE EXPLORER ============== #
Write-Host "Installing aasx-package-explorer..."

$URL = "https://github.com/admin-shell-io/aasx-package-explorer/releases/download/v2023-11-17.alpha/aasx-package-explorer-blazorexplorer.2023-11-17.alpha.zip"
$DESTINATION = "./aasx-package-explorer/"

# Create the destination directory if it doesn't exist
if (-not (Test-Path -Path $DESTINATION)) {
    New-Item -ItemType Directory -Path $DESTINATION | Out-Null
}

# Download the zip file using Invoke-WebRequest
Write-Host "Downloading $URL..."
Invoke-WebRequest -Uri $URL -OutFile "./.tmp/aasx-package-explorer-blazorexplorer.2023-11-17.alpha.zip"

# Extract the contents of the zip file using Expand-Archive
Write-Host "Extracting ./.tmp/aasx-package-explorer-blazorexplorer.2023-11-17.alpha.zip..."
Expand-Archive -Path "./.tmp/aasx-package-explorer-blazorexplorer.2023-11-17.alpha.zip" -DestinationPath $DESTINATION

# Check if the extraction was successful
if ($?) {
    Write-Host "DONE"
} else {
    Write-Host "Error: Failed to extract the contents of the zip file."
    exit 1
}

Write-Host "Deleting temp directory..."
rm -r .tmp/
