cd ~/NetCore/justine-core/src/JustineCore
dotnet publish -r win-arm -c Release
ssh Administrator@192.168.0.101 "kill.exe JustineCore"
scp -r ~/NetCore/justine-core/src/JustineCore/bin/Release/netcoreapp2.0/win-arm/publish/* Administrator@192.168.0.101:./../../../justine-core
ssh Administrator@192.168.0.101 "shutdown /r /t 0"