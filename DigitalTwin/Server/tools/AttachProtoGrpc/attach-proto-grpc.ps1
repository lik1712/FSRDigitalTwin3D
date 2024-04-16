# Argument values
$Lang = $args[0]
$ToolsDir = ""
$OutputDir = ""
$Protos = ""

# Parse command line arguments
for ($i = 0; $i -lt $args.Length; $i++) {
    switch ($args[$i]) {
        "--proto-tools" {
            $ToolsDir = $args[$i + 1]
            $i += 1
            break
        }
        "--output" {
            $OutputDir = $args[$i + 1]
            $i += 1
            break
        }
        "--protos" {
            $Protos = $args[$i + 1]
            $i += 1
            break
        }
        default {
            $Lang = $args[$i]
            break
        }
    }
}

# Check if all required parameters are provided
if (-not $Lang -or -not $ToolsDir -or -not $OutputDir -or -not $Protos) {
    Write-Host "Usage: attach-proto-grpc <lang> --proto-tools <proto_tools_dir> --output <output_dir> --protos <comma_separated_proto_files>"
    exit 1
}

function create_output_dir {
    if (Test-Path $OutputDir) {
        Remove-Item $OutputDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
    New-Item -ItemType Directory -Path "$OutputDir\Message.Generated" | Out-Null
    New-Item -ItemType Directory -Path "$OutputDir\GRPC.Generated" | Out-Null
}

function attach_proto_grpc {
    create_output_dir
    $files = $Protos.Split(',')
    foreach ($file in $files) {
        $protoc_exec = "$ToolsDir\protoc.exe"
        $grpc_plugin = "$ToolsDir\grpc_${Lang}_plugin.exe"
        & $protoc_exec "--${Lang}_out=$OutputDir\Message.Generated" "--grpc_out=$OutputDir\GRPC.Generated" "--plugin=protoc-gen-grpc=$grpc_plugin" $file
    }
}

attach_proto_grpc
