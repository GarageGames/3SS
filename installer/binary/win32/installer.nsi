;NSIS Modern User Interface version 1.70

;--------------------------------
;Include Modern UI

!include "MUI.nsh"

;--------------------------------
;General

!ifndef  PRODUCT_NAME
   !define  PRODUCT_NAME "Torque Game Builder"
!endif

!ifndef  VERSION
   !define  VERSION "1.7.5 Binary"
!endif

!define  REG_KEY  "SOFTWARE\Torque\${PRODUCT_NAME} ${VERSION}"

;Name and file
Name "${PRODUCT_NAME} ${VERSION}"
OutFile "..\${PRODUCT_NAME} ${VERSION}-Win.exe"

;Default installation folder
InstallDir "$PROGRAMFILES\Torque\$(^Name)"
InstallDirRegKey HKLM "${REG_KEY}" "InstallPath"
DirText "This will install the $(^Name) on your computer. Please choose a directory"
BrandingText "Torque"

;--------------------------------
;Interface Settings

!define MUI_ABORTWARNING
!define MUI_ICON "icon.ico"
!define MUI_UNICON "icon.ico"
!define MUI_COMPONENTSPAGE_SMALLDESC
!define MUI_HEADERIMAGE
!define MUI_WELCOMEFINISHPAGE_BITMAP "panel.bmp"
!define MUI_HEADERIMAGE_BITMAP "header.bmp"
!define MUI_CUSTOMFUNCTION_ABORT "customAbort"

;--------------------------------
;Pages

!insertmacro MUI_PAGE_WELCOME
!insertmacro MUI_PAGE_COMPONENTS
!insertmacro MUI_PAGE_DIRECTORY
!insertmacro MUI_PAGE_INSTFILES

!insertmacro MUI_UNPAGE_CONFIRM
!insertmacro MUI_UNPAGE_INSTFILES
!insertmacro MUI_PAGE_FINISH


;--------------------------------
;Languages

!insertmacro MUI_LANGUAGE "English"

;--------------------------------
; Main SDK section

RequestExecutionLevel admin

Section "!$(^Name) (required)" SecTorque
  SectionIn RO
  SetOutPath "$INSTDIR"
  File "..\..\oalinst.exe"
  File /r "..\staging\*.*"

  ; Start Menu
  SetOutPath "$SMPROGRAMS\$(^Name)"

  ; Editor exectuable
  SetOutPath "$INSTDIR\tgb"
  CreateShortCut "$SMPROGRAMS\$(^Name)\$(^Name).lnk" \
    "$INSTDIR\tgb\TorqueGameBuilder.exe"  
  SetOutPath "$SMPROGRAMS\$(^Name)"

  WriteINIStr "$SMPROGRAMS\$(^Name)\Documentation.url" \
    "InternetShortcut" "URL" "file://$INSTDIR/documentation/Official Documentation.html"
  WriteINIStr "$SMPROGRAMS\$(^Name)\$(^Name) Home Page.url" \
    "InternetShortcut" "URL" "http://www.torquepowered.com/products/torque-2d"
  WriteINIStr "$SMPROGRAMS\$(^Name)\Online Documentation.url" \
    "InternetShortcut" "URL" "http://www.torquepowered.com/documentation/tgb"
  WriteINIStr "$SMPROGRAMS\$(^Name)\$(^Name) Forums (online).url" \
    "InternetShortcut" "URL" "http://www.torquepowered.com/community/forums/28"

    
  SetOutPath "$INSTDIR"
  CreateShortCut "$SMPROGRAMS\$(^Name)\Uninstall $(^Name).lnk" \
    "$INSTDIR\uninst-tsdk.exe"

  ; Registry uninstall
  WriteRegStr HKLM "${REG_KEY}" "InstallPath" $INSTDIR
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\$(^Name)" \
                 "DisplayName" "$(^Name) (remove only)"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\$(^Name)" \
                 "UninstallString" '"$INSTDIR\uninst-tsdk.exe"'
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\$(^Name)" \
                 "DisplayIcon" "$INSTDIR\uninst-tsdk.exe"
  
  ; Silently install OpenAL
  ExecWait '"$INSTDIR\oalinst.exe" /S'
    
  ;Create uninstaller
  WriteUninstaller $INSTDIR\uninst-tsdk.exe
SectionEnd

;--------------------------------
; Desktop Shortcut

Section "Desktop Shortcut" SecDesktop
  ; Desktop
  SetOutPath "$INSTDIR\tgb"
  CreateShortCut "$DESKTOP\$(^Name).lnk" "$INSTDIR\tgb\TorqueGameBuilder.exe"
  SetOutPath "$INSTDIR"
SectionEnd

;--------------------------------
; Readme at the end

Section "View Documentation Overview" SecReadMe
  ExecShell open '$INSTDIR\documentation\Official Documentation.html'
SectionEnd

;--------------------------------
; Pingback installation success

Function .onInstSuccess
   ExecShell "open" "http://www.torquepowered.com/products/torque-2d/demo?type=binary&action=install&os=windows&name=${PRODUCT_NAME}&version=${VERSION}"
FunctionEnd

;--------------------------------
; Pingback installation cancel

Function customAbort
   ExecShell "open" "http://www.torquepowered.com/products/torque-2d/demo?type=binary&action=cancel&os=windows&name=${PRODUCT_NAME}&version=${VERSION}"
FunctionEnd

;--------------------------------
; Pingback uninstallation success

Function un.onUninstSuccess
   ExecShell "open" "http://www.torquepowered.com/products/torque-2d/demo?type=binary&action=uninstall&os=windows&name=${PRODUCT_NAME}&version=${VERSION}"
FunctionEnd

;--------------------------------
; Uninstaller

Section Uninstall
  MessageBox MB_YESNO|MB_ICONQUESTION \
           "Everything in the $(^Name) directory will be deleted. Are you sure you wish to uninstall $(^Name)?" \
           IDNO Removed

  ; Shortcuts
  Delete "$DESKTOP\$(^Name).lnk"
  Delete "$SMPROGRAMS\$(^Name)\$(^Name).lnk"
  Delete "$SMPROGRAMS\$(^Name)\Documentation.url.url"
  Delete "$SMPROGRAMS\$(^Name)\$(^Name) Home Page.url"
  Delete "$SMPROGRAMS\$(^Name)\Online Documentation.url"
  Delete "$SMPROGRAMS\$(^Name)\$(^Name) Forums.url"
  Delete "$SMPROGRAMS\$(^Name)\Uninstall $(^Name).lnk"
  RMDir /r "$SMPROGRAMS\$(^Name)"
  
  ; AppData
  RMDir /r "$APPDATA\GarageGames\TorqueGameBuilder"

  ; Registry
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\$(^Name)"
  DeleteRegKey HKLM "${REG_KEY}"

  ; SDK Source and Binaries
  RMDir /r $INSTDIR
  IfFileExists $INSTDIR 0 Removed
    MessageBox MB_OK|MB_ICONEXCLAMATION \
               "Note: $INSTDIR could not be removed."
  Removed:
SectionEnd

;--------------------------------
;Descriptions

!insertmacro MUI_FUNCTION_DESCRIPTION_BEGIN
!insertmacro MUI_DESCRIPTION_TEXT ${SecTorque} "Install the Torque Game Builder binaries, and editor. This component is required."
!insertmacro MUI_DESCRIPTION_TEXT ${SecDesktop} "Create a shortcut on your desktop for launching the Torque Game Builder"
!insertmacro MUI_DESCRIPTION_TEXT ${SecReadMe} "View the Documentation Overview at the end of the installation process"
!insertmacro MUI_FUNCTION_DESCRIPTION_END