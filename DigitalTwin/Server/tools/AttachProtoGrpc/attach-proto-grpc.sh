#!/bin/bash

# Argument values
Lang="$1"
ToolsDir=""
OutputDir=""
Protos=""

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case "$1" in
        --proto-tools)
            ToolsDir="$2"
            shift 2
            ;;
        --output)
            OutputDir="$2"
            shift 2
            ;;
        --protos)
            Protos="$2"
            shift 2
            ;;
        *)
            Lang="$1"
            shift
            ;;
    esac
done

# Check if all required parameters are provided
if [[ -z "$Lang" || -z "$ToolsDir" || -z "$OutputDir" || -z "$Protos" ]]; then
    echo "Usage: $0 <lang> --proto-tools <proto_tools_dir> --output <output_dir> --protos <comma_separated_proto_files>"
    exit 1
fi

create_output_dir() {
    if [ -d "$OutputDir" ]; then
        rm -rf "$OutputDir"
    fi
    mkdir "$OutputDir"
    mkdir "$OutputDir/Message.Generated/"
    mkdir "$OutputDir/GRPC.Generated/"
}

attach_proto_grpc() {
    create_output_dir
    IFS=',' read -r -a files <<< "$Protos"
    for file in "${files[@]}"; do
        protoc_exec="$ToolsDir/protoc"
        grpc_plugin="$ToolsDir/grpc_${Lang}_plugin"
        "$protoc_exec" --${Lang}_out="$OutputDir/Message.Generated/" --grpc_out="$OutputDir/GRPC.Generated/" --plugin=protoc-gen-grpc="$grpc_plugin" "$file"
    done
}

attach_proto_grpc