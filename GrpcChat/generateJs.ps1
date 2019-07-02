Set-Location .;

$jsFolder = "..\GrpcChat.Host\wwwroot\src\proto";
if (!(Test-Path $jsFolder)) {
    mkdir $jsFolder;
}

$files = (Get-ChildItem -Recurse -Filter "*.proto");
Write-Host "Generating protos: $(($files | Resolve-Path -Relative))";
protoc -I="." ($files | Resolve-Path -Relative) --js_out=import_style=commonjs+dts:$jsFolder --grpc-web_out=import_style=commonjs+dts,mode=grpcweb:$jsFolder
protoc -I="." ($files | Resolve-Path -Relative) --js_out=import_style=commonjs:$jsFolder --grpc-web_out=import_style=commonjs+dts,mode=grpcweb:$jsFolder


Set-Location ..;