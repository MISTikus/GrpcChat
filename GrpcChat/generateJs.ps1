Set-Location GrpcChat;

$jsFodler = "..\GrpcChat.Host\wwwroot\proto";
if (!(Test-Path $jsFodler)) {
    mkdir $jsFodler;
}

$files = (Get-ChildItem -Recurse -Filter "*.proto");
Write-Host "Generating protos: $(($files | Resolve-Path -Relative))";
protoc -I="." ($files | Resolve-Path -Relative) --js_out=import_style=commonjs:$jsFodler

Set-Location ..;