## InstallUtil.exe

cd C:\Windows\Microsoft.NET\Framework\v4.0.30319

## Installing a Windows Service
 
Open the command prompt and fire the below command and press ENTER.
Syntax InstallUtil.exe + Your copied path + \your service name + .exe
Our path InstallUtil.exe D:\Github\C#\MyFirstService\MyFirstService\bin\Release\DaVcheztService.exe

## Uninstalling a Windows Service
 
If you want to uninstall your service, fire the below command.
Syntax InstallUtil.exe -u + Your copied path + \your service name + .exe
Our path InstallUtil.exe -u D:\Github\C#\MyFirstService\MyFirstService\bin\Release\DaVcheztService.exe