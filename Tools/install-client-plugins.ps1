# =============== PROTOC ============== #
Write-Host "Downloading Unity Plugins..."

$URL = "https://packages.grpc.io/archive/2019/11/6950e15882f28e43685e948a7e5227bfcef398cd-6d642d6c-a6fc-4897-a612-62b0a3c9026b/csharp/grpc_unity_package.2.26.0-dev.zip"
$DESTINATION = "../Client/Unity/DTClient/Assets/"

# Create the destination directory if it doesn't exist
if (-not (Test-Path -Path ./.tmp/)) {
    New-Item -ItemType Directory -Path ./.tmp/ | Out-Null
}

# Download the zip file using Invoke-WebRequest
Write-Host "Downloading $URL..."
Invoke-WebRequest -Uri $URL -OutFile "./.tmp/grpc_unity_package.2.26.0-dev.zip"

# Extract the contents of the zip file using Expand-Archive
Write-Host "Extracting ./.tmp/grpc_unity_package.2.26.0-dev.zip..."
Expand-Archive -Path "./.tmp/grpc_unity_package.2.26.0-dev.zip" -DestinationPath $DESTINATION

# Check if the extraction was successful
if ($?) {
    Write-Host "DONE"
} else {
    Write-Host "Error: Failed to extract the contents of the zip file."
    exit 1
}

# =============== UniRx ============== #

$URL = "https://github.com/neuecc/UniRx/archive/refs/tags/7.1.0.zip"
$DESTINATION = "../Client/Unity/DTClient/Assets/Plugins/UniRx/"

Write-Host "Downloading $URL..."
Invoke-WebRequest -Uri $URL -OutFile "./.tmp/unirx.7.1.0.zip"

Write-Host "Extracting ./.tmp/unirx.7.1.0.zip..."
if (Test-Path -Path "./.tmp/UniRx-7.1.0/") {
    Remove-Item -Path "./.tmp/UniRx-7.1.0/" -Recurse -Force
}
Expand-Archive -Path "./.tmp/unirx.7.1.0.zip" -DestinationPath "./.tmp/"

Write-Host "Copying UniRx into Plugin folder..."
Copy-Item -Path "./.tmp/UniRx-7.1.0/Assets/Plugins/UniRx/" -Destination $DESTINATION -Recurse

Write-Host "DONE"

# =============== Treeview ============== #

$URL = "https://github.com/neomasterhub/Unity-Treeview.git"
$DESTINATION = "../Client/Unity/DTClient/Assets/Plugins/Treeview/"

Write-Host "Cloning repo from $URL..."
git clone $URL  "./.tmp/treeview/"

Write-Host "Copying TreeView into Plugin folder..."
Copy-Item -Path "./.tmp/treeview/Assets/Treeview/" -Destination $DESTINATION -Recurse

Write-Host "DONE"

Write-Host "Deleting temp directory..."
rm -r .tmp/
