#!/bin/bash

# =============== PROTOC ============== #
echo "Downloading Unity Plugins..."

URL="https://packages.grpc.io/archive/2019/11/6950e15882f28e43685e948a7e5227bfcef398cd-6d642d6c-a6fc-4897-a612-62b0a3c9026b/csharp/grpc_unity_package.2.26.0-dev.zip"
DESTINATION="../DigitalTwin/Client/FSR.DigitalTwin.Unity/Assets/"

# Create the destination directory if it doesn't exist
if [ ! -d "./.tmp/" ]; then
    mkdir -p "./.tmp/"
fi

# Download the zip file
echo "Downloading $URL..."
curl -o "./.tmp/grpc_unity_package.2.26.0-dev.zip" "$URL"

# Extract the contents of the zip file
echo "Extracting ./.tmp/grpc_unity_package.2.26.0-dev.zip..."
unzip -q "./.tmp/grpc_unity_package.2.26.0-dev.zip" -d "$DESTINATION"

# Check if the extraction was successful
if [ $? -eq 0 ]; then
    echo "DONE"
else
    echo "Error: Failed to extract the contents of the zip file."
    exit 1
fi

# =============== UniRx ============== #
URL="https://github.com/neuecc/UniRx.git"
DESTINATION="../DigitalTwin/Client/FSR.DigitalTwin.Unity/Assets/Plugins/UniRx/"

echo "Cloning repo at $URL..."
git clone $URL ./.tmp/unirx.7.1.0/
cd ./.tmp/unirx.7.1.0/
git checkout 66205df49631860dd8f7c3314cb518b54c944d30
cd ../..

echo "Copying UniRx into Plugin folder..."
cp -r "./.tmp/unirx.7.1.0/Assets/Plugins/UniRx/" "$DESTINATION"

echo "DONE"

# =============== Treeview ============== #
URL="https://github.com/neomasterhub/Unity-Treeview.git"
DESTINATION="../DigitalTwin/Client/FSR.DigitalTwin.Unity/Assets/Plugins/Treeview/"

echo "Cloning repo from $URL..."
git clone "$URL"  "./.tmp/treeview/"

echo "Copying TreeView into Plugin folder..."
cp -r "./.tmp/treeview/Assets/Treeview/" "$DESTINATION"

echo "DONE"

echo "Deleting temp directory..."
rm -rf .tmp/
