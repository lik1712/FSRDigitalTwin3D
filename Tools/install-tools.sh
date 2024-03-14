#!/bin/bash

# Create tmp directory
if [ ! -d "./.tmp/" ]; then
    mkdir -p "./.tmp/"
fi

# =============== PROTOC ============== #
echo "Installing protoc..."

URL="https://packages.grpc.io/archive/2019/11/6950e15882f28e43685e948a7e5227bfcef398cd-6d642d6c-a6fc-4897-a612-62b0a3c9026b/protoc/grpc-protoc_linux_x64-1.26.0-dev.tar.gz"
DESTINATION="./protoc/"

# Create the destination directory if it doesn't exist
if [ ! -d "$DESTINATION" ]; then
    mkdir -p "$DESTINATION"
fi

# Download the zip file
echo "Downloading $URL..."
curl -o "./.tmp/grpc-protoc_linux_x64-1.26.0-dev.tar.gz" "$URL"

# Extract the contents of the zip file
echo "Extracting ./.tmp/grpc-protoc_linux_x64-1.26.0-dev.tar.gz..."
tar -xf "./.tmp/grpc-protoc_linux_x64-1.26.0-dev.tar.gz" -C "$DESTINATION"

# Check if the extraction was successful
if [ $? -eq 0 ]; then
    echo "DONE"
else
    echo "Error: Failed to extract the contents of the tar.gz file."
    exit 1
fi

echo "Deleting temp directory..."
rm -rf .tmp/