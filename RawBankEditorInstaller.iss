; Instalator RawBankEditora. Spolocny obsah instalatorov je v ToolsCore\Installer\AppSetup.iss.
; Pred kompilaciou treba zostavit Release: dotnet build RawBankEditor\RawBankEditor.csproj -c Release

#define MyAppName "RawBankEditor"
#define MyAppExeName "RawBankEditor.exe"
#define MyAppDir SourcePath + "RawBankEditor\bin\Release\net10.0-windows"
#define MyOutputBaseFilename "rawbankeditor-setup"

#include "..\ToolsCore\Installer\AppSetup.iss"
