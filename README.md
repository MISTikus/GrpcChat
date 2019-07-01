# GrpcChat
Chat example using [gRPC](https://grpc.io)

Contains:
* Chat Server (Console)
* Chat Client (Console)
* Chat Client (Web) **(still in progress)**

## Install
* [.NetCore](dot.net)

* [PowerShell Core](https://docs.microsoft.com/ru-ru/powershell/scripting/install/installing-powershell?view=powershell-6)

After restore and build `GrpcChat.sln` run this script to add [grpc.tools](https://www.nuget.org/packages/Grpc.Tools/) into [PATH variable](https://en.wikipedia.org/wiki/PATH_(variable)):

**do not forget to restart Visual Studio (and other terminals) after script run**
```powershell
$path = [Environment]::GetEnvironmentVariable("Path", [System.EnvironmentVariableTarget]::Machine);

if ([Environment]::Is64BitOperatingSystem){ $dir = "$($env:USERPROFILE)\.nuget\packages\grpc.tools\1.21.0\tools\windows_x64"; } else { $dir = "$($env:USERPROFILE)\.nuget\packages\grpc.tools\1.21.0\tools\windows_x86"; }

if (!$path.Contains($dir)) { $path += ";$dir"; [Environment]::SetEnvironmentVariable("Path", $path, [System.EnvironmentVariableTarget]::Machine); }
```