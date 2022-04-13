#define MyAppName "TelegramBotForReddit"
#define MyAppVersion "1.0.0"
#define MyAppExeName "TelegramBotForReddit.exe"
#define MyAppAssocName MyAppName + " File"
#define MyAppAssocExt ".myp"
#define MyAppAssocKey StringChange(MyAppAssocName, " ", "") + MyAppAssocExt
#define AppDirectory "TelegramBotForReddit\bin\Release\net5.0\win-x64"

[Setup]
AppId={{BF3BA4D1-2215-4C55-9D96-9788ECC57207}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
DefaultDirName={autopf}\{#MyAppName}
ChangesAssociations=yes
DisableProgramGroupPage=yes
PrivilegesRequiredOverridesAllowed=commandline
OutputDir=.\
OutputBaseFilename=RedditBotSetup
Compression=lzma
SolidCompression=yes
WizardStyle=modern
DisableDirPage=no

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Dirs]
Name: "{app}\logs"; Permissions: users-full

[Files]
Source: "{#AppDirectory}\*"; DestDir: "{app}"; Flags: ignoreversion
Source: "{#AppDirectory}\ref\*"; DestDir: "{app}\refs"; Flags: ignoreversion
Source: {code:GetConfigPath}; DestDir: "{app}"; DestName: "appsettings.json"; Flags: external skipifsourcedoesntexist;
Source: "{#AppDirectory}\"; DestDir: "{app}"; DestName: "appsettings.default.json"; Flags: external skipifsourcedoesntexist;
Source: "{#AppDirectory}\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion

[Registry]
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocExt}\OpenWithProgids"; ValueType: string; ValueName: "{#MyAppAssocKey}"; ValueData: ""; Flags: uninsdeletevalue
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}"; ValueType: string; ValueName: ""; ValueData: "{#MyAppAssocName}"; Flags: uninsdeletekey
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\DefaultIcon"; ValueType: string; ValueName: ""; ValueData: "{app}\{#MyAppExeName},0"
Root: HKA; Subkey: "Software\Classes\{#MyAppAssocKey}\shell\open\command"; ValueType: string; ValueName: ""; ValueData: """{app}\{#MyAppExeName}"" ""%1"""
Root: HKA; Subkey: "Software\Classes\Applications\{#MyAppExeName}\SupportedTypes"; ValueType: string; ValueName: ".myp"; ValueData: ""

[Run]
Filename: {sys}\sc.exe; Parameters: "create {#MyAppName} start= auto binPath= ""{app}\{#MyAppExeName}""" ; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "start {#MyAppName}" ; Flags: runhidden

[UninstallRun]
Filename: {sys}\sc.exe; Parameters: "stop {#MyAppName}" ; Flags: runhidden
Filename: {sys}\sc.exe; Parameters: "delete {#MyAppName}" ; Flags: runhidden

[Code]
var 
  Page: TInputFileWizardPage;
  ConfigPath: String;
procedure InitializeWizard;
begin
  Page:= CreateInputFilePage(wpWelcome,
    'Select appsettings location', 'Select appsettings.json file.',
    'Select where appsettings.json is located, then click Next.');

  Page.Add('&Location of appsettings.json:',
    'JSON files|*.json|All files|*.*',
    '.json');
  ExpandConstant('{win}\appsettings.json');
end;
function GetConfigPath(Param : String): String;
begin
  if (Page is TInputFileWizardPage) then
  begin
    Result := Page.Values[0];
  end;
end;
