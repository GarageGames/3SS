//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

//---------------------------------------------------------------------------------------------
// GuiDefaultProfile is a special profile that all other profiles inherit defaults from. It
// must exist.
//---------------------------------------------------------------------------------------------
if (!isObject(GuiDefaultProfile)) new GuiControlProfile (GuiDefaultProfile)
{
    tab = false;
    canKeyFocus = false;
    hasBitmapArray = false;
    mouseOverSelected = false;

    // fill color
    opaque = false;
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "244 244 244 52";

    // border color
    border = 1;
    borderColor    = "100 100 100 255";
    borderColorHL = "128 128 128 255";
    borderColorNA = "226 226 226 52";

    // font
    fontType = "Open Sans";
    fontSize = 19;

    fontColor = "27 59 95 255";
    fontColorHL = "232 240 248 255";
    fontColorNA = "0 0 0 52";
    fontColorSEL= "10 10 10 255";

    // bitmap information
    bitmap = "^{EditorAssets}/data/images/window";
    bitmapBase = "";
    textOffset = "0 0";

    // used by guiTextControl
    modal = true;
    justify = "left";
    autoSizeWidth = false;
    autoSizeHeight = false;
    returnTab = false;
    numbersOnly = false;
    cursorColor = "0 0 0 255";

    // sounds
    soundButtonDown = "";
    soundButtonOver = "";
};

if (!isObject(GuiSolidDefaultProfile)) new GuiControlProfile (GuiSolidDefaultProfile : GuiDefaultProfile)
{
    opaque = true;
    border = true;
};

if (!isObject(GuiTransparentProfile)) new GuiControlProfile (GuiTransparentProfile : GuiDefaultProfile)
{
    opaque = false;
    border = false;
};

if (!isObject(GuiToolTipProfile)) new GuiControlProfile (GuiToolTipProfile : GuiDefaultProfile)
{
    fillColor = "246 220 165 255";
    // font
    fontSize = 16;
};

if (!isObject(GuiModelessDialogProfile)) new GuiControlProfile(GuiModelessDialogProfile : GuiDefaultProfile)
{
    opaque = false;
    border = false;
    modal = false;
};

if (!isObject(GuiFrameSetProfile)) new GuiControlProfile (GuiFrameSetProfile : GuiDefaultProfile)
{
    fillColor = "232 240 248";
    borderColor    = "138 134 122";
    opaque = true;
    border = true;
};

if (!isObject(GuiWindowProfile)) new GuiControlProfile (GuiWindowProfile : GuiDefaultProfile)
{
    opaque = true;
    border = 0;
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "255 255 255 52";
    fontColor = "27 59 95 255";
    fontColorHL = "232 240 248 255";
    text = "untitled";
    bitmap = "^{EditorAssets}/data/images/window";
    textOffset = "5 5";
    hasBitmapArray = true;
    justify = "center";
    
    fontType = "Arial Bold";
    fontSize = 16;
};

// ----------------------------------------------------------------------------
// Button Profiles
// ----------------------------------------------------------------------------
if (!isObject(GuiButtonProfile)) new GuiControlProfile (GuiButtonProfile)
{
    opaque = true;
    border = -1;
    fontColor = "0 0 0";
    fontColorHL = "229 229 229 255";
    fixedExtent = true;
    justify = "center";
    canKeyFocus = false;
    fontType = "Open Sans";
    bitmap = "^{EditorAssets}/data/images/smallButtonContainer";
};

if (!isObject(GuiLargeButtonContainer)) new GuiControlProfile (GuiLargeButtonContainer : GuiButtonProfile)
{
    opaque = false;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/largeButtonContainer";
};

if (!isObject(GuiSelectedElementHighlight)) new GuiControlProfile (GuiSelectedElementHighlight : GuiButtonProfile)
{
    opaque = false;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/selectedImageContainer";
};

if (!isObject(BlueButtonProfile)) new GuiControlProfile (BlueButtonProfile : GuiButtonProfile)
{
    fontSize = 16;
    fontColor = "255 255 255 255";
    fontColorHL = "255 255 255 255";
    bitmap = "^{EditorAssets}/data/images/blueButton";
};

if (!isObject(RedButtonProfile)) new GuiControlProfile (RedButtonProfile : GuiButtonProfile)
{
    fontSize = 16;
    fontColor = "255 255 255 255";
    fontColorHL = "255 255 255 255";
    bitmap = "^{EditorAssets}/data/images/redButton";
};

if (!isObject(GreenButtonProfile)) new GuiControlProfile (GreenButtonProfile : GuiButtonProfile)
{
    fontSize = 16;
    fontColor = "255 255 255 255";
    fontColorHL = "255 255 255 255";
    bitmap = "^{EditorAssets}/data/images/greenButton";
};

if (!isObject(GuiRadioProfile)) new GuiControlProfile (GuiRadioProfile : GuiDefaultProfile)
{
    fillColor = "232 232 232 255";
    fixedExtent = true;
    bitmap = "^{EditorAssets}/data/images/radioButton";
    hasBitmapArray = true;
};

// ----------------------------------------------------------------------------
// Container/Panel Profiles
// ----------------------------------------------------------------------------
if (!isObject(GuiSunkenContainerProfile)) new GuiControlProfile (GuiSunkenContainerProfile)
{
    opaque = false;
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "127 127 127 52";
    border = -2;
    bitmap = "^{EditorAssets}/data/images/sunkenContainer";
    borderColor = "40 40 40 10";
};

if (!isObject(GuiPictureContainerProfile)) new GuiControlProfile (GuiPictureContainerProfile)
{
    opaque = false;
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "127 127 127 255";
    border = -2;
    bitmap = "^{EditorAssets}/data/images/profilePicture";
    borderColor = "40 40 40 10";
};

if (!isObject(GuiScreenContainerProfile)) new GuiControlProfile (GuiScreenContainerProfile)
{
    opaque = false;
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "127 127 127 52";
    border = -2;
    bitmap = "^{EditorAssets}/data/images/containerForScreens";
    borderColor = "40 40 40 10";
};

if (!isObject(GuiThumbnailContainerProfile)) new GuiControlProfile (GuiThumbnailContainerProfile)
{
    opaque = false;
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "127 127 127 52";
    border = -2;
    bitmap = "^{EditorAssets}/data/images/thumbnailFrame";
    borderColor = "40 40 40 10";
};

if (!isObject(GuiMediumButton)) new GuiControlProfile (GuiMediumButton : GuiButtonProfile)
{
    bitmap = "^{EditorAssets}/data/images/mediumButtonContainer";
};

if (!isObject(GuiLargePanelContainer)) new GuiControlProfile (GuiLargePanelContainer : GuiButtonProfile)
{
    opaque = false;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/buttonStateContainer_atlas";
};

if (!isObject(GuiLargePanelContainerInactive)) new GuiControlProfile (GuiLargePanelContainerInactive : GuiButtonProfile)
{
    opaque = false;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/buttonStateContainer_inactive";
};

if (!isObject(GuiNarrowPanelContainer)) new GuiControlProfile (GuiNarrowPanelContainer : GuiButtonProfile)
{
    opaque = false;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/noStateContainer";
};

if (!isObject(GuiNarrowPanelContainerHighlight)) new GuiControlProfile (GuiNarrowPanelContainerHighlight : GuiButtonProfile)
{
    opaque = false;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/noStateContainer_highlight";
};

if (!isObject(GuiWorldPaneContainer)) new GuiControlProfile (GuiWorldPaneContainer : GuiButtonProfile)
{
    opaque = false;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/worldsLevelsContainer";
};

if (!isObject(GuiLargeButtonContainerInactive)) new GuiControlProfile (GuiLargeButtonContainerInactive : GuiButtonProfile)
{
    opaque = false;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/largeButtonContainer_inactive";
};

if (!isObject(GuiLargeButtonHighlight)) new GuiControlProfile (GuiLargeButtonHighlight : GuiButtonProfile)
{
    opaque = false;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/largeButtonContainer_highlightBar";
};

if (!isObject(GuiLargePanelContainerHighlight)) new GuiControlProfile (GuiLargePanelContainerHighlight : GuiButtonProfile)
{
    opaque = false;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/buttonStateContainer_highlight";
};

if (!isObject(PanelLightProfile)) new GuiControlProfile (PanelLightProfile) 
{
    opaque = false;
    bitmap = "^{EditorAssets}/data/images/tabBook";
    border = 1;
};

if (!isObject(PanelMediumProfile)) new GuiControlProfile (PanelMediumProfile) 
{
    opaque = false;
    bitmap = "^{EditorAssets}/data/images/panelMedium";
    border = -2;
};

if (!isObject(PanelDarkProfile)) new GuiControlProfile (PanelDarkProfile) 
{
    opaque = false;
    bitmap = "^{EditorAssets}/data/images/sunkenContainer";
    border = -2;
};

if (!isObject(PanelTransparentProfile)) new GuiControlProfile(EditorPanelTransparent : PanelDarkProfile)
{
    bitmap = "^{EditorAssets}/data/images/panel_transparent";
};

if (!isObject(GuiTransparentProfileModeless)) new GuiControlProfile (GuiTransparentProfileModeless : GuiTransparentProfile) 
{
    modal = false;
};

if (!isObject(GuiLevelPanelContainer)) new GuiControlProfile (GuiLevelPanelContainer : GuiDefaultProfile)
{
    opaque = false;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/worldsLevelsContainer";
};

if (!isObject(GuiLevelPanelContainerHighlight)) new GuiControlProfile (GuiLevelPanelContainerHighlight : GuiDefaultProfile)
{
    opaque = false;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/worldsLevelsContainerHighlight";
};

// ----------------------------------------------------------------------------
// CheckBox Control Profiles
// ----------------------------------------------------------------------------
if (!isObject(GuiCheckBoxProfile)) new GuiControlProfile (GuiCheckBoxProfile)
{
    opaque = false;
    fillColor = "232 232 232 255";
    border = false;
    borderColor = "0 0 0 255";
    fontType = "Arial";
    fontSize = 16;
    fixedExtent = true;
    justify = "left";
    bitmap = "^{EditorAssets}/data/images/checkBox";
    hasBitmapArray = true;
};

// ----------------------------------------------------------------------------
// Slider Control Profiles
// ----------------------------------------------------------------------------
if (!isObject(GuiSliderProfile)) new GuiControlProfile (GuiSliderProfile)
{
    bitmap = "^{EditorAssets}/data/images/slider";
    fontType = "Arial";
    fontSize = 16;
    fontColor = "229 229 229 255";
};

if (!isObject(GuiSliderNoTextProfile)) new GuiControlProfile (GuiSliderNoTextProfile)
{
    bitmap = "^{EditorAssets}/data/images/slider";
    fontColor = "255 255 255 255";
    fontSize = 16;
};

// ----------------------------------------------------------------------------
// TreeView Control Profiles
// ----------------------------------------------------------------------------
if (!isObject(GuiTreeViewProfile)) new GuiControlProfile (GuiTreeViewProfile)
{
    fillColorHL = "0 60 150 255";
    fontType = "Arial";
    fontSize = 16;
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "127 127 127 52";
    fontColor = "27 59 95 255";
    fontColorHL = "232 240 248 255";
    fontColorNA = "0 0 0 255";
    fontColorSEL= "10 10 10 255";
    bitmap = "^{EditorAssets}/data/images/treeView";
    canKeyFocus = true;
    autoSizeHeight = true;
};

// ----------------------------------------------------------------------------
// ListBox Control Profiles
// ----------------------------------------------------------------------------
if (!isObject(GuiListBoxProfile)) new GuiControlProfile (GuiListBoxProfile)
{
    tab = true;
    canKeyFocus = true;
    
    fontType = "Arial";
    fontSize = 16;
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "127 127 127 52";
    fontColor = "27 59 95 255";
    fontColorHL = "232 240 248 255";
    fontColorNA = "0 0 0 52";
    fontColorSEL= "10 10 10 255";
};

// ----------------------------------------------------------------------------
// Text Control Profiles
// ----------------------------------------------------------------------------
if (!isObject(GuiTextProfile)) new GuiControlProfile (GuiTextProfile)
{
    border=false;

    // font
    fontType = "Open Sans";
    fontSize = 19;

    fontColor = "27 59 95 255";
    fontColorHL = "232 240 248 255";
    fontColorNA = "0 0 0 52";
    fontColorSEL= "10 10 10 255";

    modal = true;
    justify = "left";
    autoSizeWidth = false;
    autoSizeHeight = false;
    returnTab = false;
    numbersOnly = false;
    cursorColor = "0 0 0 255";
};

if (!isObject(GuiText16Profile)) new GuiControlProfile (GuiText16Profile : GuiTextProfile)
{
    fontSize = 16;
};

if (!isObject(GuiMLTextCenteredLgPreviewProfile)) new GuiControlProfile (GuiMLTextCenteredLgPreviewProfile : GuiTextProfile)
{
    border = false;
    justify = "center";
};

if (!isObject(GuiMLTextCenteredSmPreviewProfile)) new GuiControlProfile (GuiMLTextCenteredSmPreviewProfile : GuiTextProfile)
{
    border = false;
    justify = "center";
};

if (!isObject(GuiTextCenteredLgPreviewProfile)) new GuiControlProfile (GuiTextCenteredLgPreviewProfile : GuiTextProfile)
{
    justify = "center";
};

if (!isObject(GuiTextCenteredSmPreviewProfile)) new GuiControlProfile (GuiTextCenteredSmPreviewProfile : GuiTextProfile)
{
    justify = "center";
};

if (!isObject(GuiModelessTextProfile)) new GuiControlProfile(GuiModelessTextProfile : GuiTextProfile)
{
    modal = false;
};

if (!isObject(GuiModelessInactiveTextProfile)) new GuiControlProfile(GuiModelessInactiveTextProfile : GuiTextProfile)
{
    modal = false;
    fontColor = "0 0 0 52";
};

if (!isObject(GuiDarkTextProfile)) new GuiControlProfile (GuiDarkTextProfile : GuiTextProfile)
{
    fontType = "Open Sans Bold";
    fontSize = 19;
    fontColorNA = "0 0 0 52";
    fontColor = "27 59 95 255";
    border=false;
};

if (!isObject(GuiPaneTextProfile)) new GuiControlProfile (GuiPaneTextProfile : GuiTextProfile)
{
    fontType = "Open Sans";
    fontSize = 19;
    fontColorNA = "0 0 0 52";
    Modal = false;
};

if (!isObject(GuiTextAddBtnProfile)) new GuiControlProfile (GuiTextAddBtnProfile : GuiTextProfile)
{
    fontType = "Open Sans";
    fontSize = 16;
    fontColorNA = "0 0 0 52";
    Modal = false;
};

if (!isObject(BlueTextProfile)) new GuiControlProfile (BlueTextProfile : GuiTextProfile)
{
    fontType = "Open Sans";
    fontSize = 14;
    fontColorNA = "27 95 59 255";
};

if (!isObject(GuiRedTextProfile)) new GuiControlProfile (GuiRedTextProfile : GuiTextProfile)
{
    fontType = "Open Sans";
    fontSize = 16;
    fontColor = "158 47 47 255";
};

if (!isObject(BlueTextCenteredProfile)) new GuiControlProfile (BlueTextCenteredProfile : GuiTextProfile)
{
    fontType = "Open Sans";
    fontSize = 14;
    fontColorNA = "27 95 59 255";
    justify = "center";
};

if (!isObject(GuiInactiveTextProfile)) new GuiControlProfile (GuiInactiveTextProfile : GuiTextProfile)
{
    fontType = "Open Sans";
    fontSize = 19;
    fontColor = "0 0 0 52";
};

if (!isObject(GuiFooterTextProfile)) new GuiControlProfile (GuiFooterTextProfile : GuiTextProfile)
{
    fontType = "Open Sans";
    fontSize = 14;
};

if (!isObject(GuiMediumTextProfile)) new GuiControlProfile (GuiMediumTextProfile : GuiTextProfile)
{
    fontSize = 24;
};

if (!isObject(GuiBigTextProfile)) new GuiControlProfile (GuiBigTextProfile : GuiTextProfile)
{
    fontSize = 36;
};

if (!isObject(GuiText24Profile)) new GuiControlProfile (GuiText24Profile : GuiTextProfile)
{
    fontSize = 24;
};

if (!isObject(GuiTextRightProfile)) new GuiControlProfile (GuiTextRightProfile : GuiTextProfile)
{
    justify = "right";
};

if (!isObject(GuiTextCenterProfile)) new GuiControlProfile (GuiTextCenterProfile : GuiTextProfile)
{
    justify = "center";
};

if (!isObject(GuiMLTextProfile)) new GuiControlProfile (GuiMLTextProfile)
{
    border=false;

    // font
    fontType = "Open Sans";
    fontSize = 19;

    fontColor = "27 59 95 255";
    fontColorHL = "232 240 248 255";
    fontColorNA = "0 0 0 52";
    fontColorSEL= "10 10 10 255";

    modal = true;
    justify = "left";
    returnTab = false;
    numbersOnly = false;
    cursorColor = "0 0 0 255";

    fontColorLink = "255 96 96 255";
    fontColorLinkHL = "0 0 255 255";
    autoSizeWidth = true;
    autoSizeHeight = true;  
};

if (!isObject(GuiMLTextCenterProfile)) new GuiControlProfile (GuiMLTextCenterProfile : GuiMLTextProfile)
{
    justify = "center";
    border = "0";
    fontSize = 19;
};

if (!isObject(GuiMLWhiteTextProfile)) new GuiControlProfile (GuiMLWhiteTextProfile : GuiMLTextProfile)
{
    fontColor = "229 229 229 255";
    fontType = "Arial";
    fontSize = 16;
    border = false;
};

if (!isObject(GuiConsoleProfile)) new GuiControlProfile (GuiConsoleProfile)
{
    fontType = ($platform $= "macos") ? "Monaco" : "Lucida Console";
    fontSize = ($platform $= "macos") ? 13 : 12;
    fontColor = "255 255 255 255";
    fontColorHL = "155 155 155 255";
    fontColorNA = "255 0 0 52";
    fontColors[6] = "100 100 100 255";
    fontColors[7] = "100 100 0 255";
    fontColors[8] = "0 0 100 255";
    fontColors[9] = "0 100 0 255";
};

// ----------------------------------------------------------------------------
// Text Edit Box Profiles
// ----------------------------------------------------------------------------
if (!isObject(GuiTextEditProfile)) new GuiControlProfile (GuiTextEditProfile)
{
    fontSize = 16;
    opaque = false;
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "127 127 127 52";
    border = -2;
    bitmap = "^{EditorAssets}/data/images/textEdit";
    borderColor = "40 40 40 10";
    fontColor = "27 59 95 255";
    fontColorHL = "232 240 248 255";
    fontColorNA = "0 0 0 127";
    fontColorSEL = "0 0 0 255";
    textOffset = "7 2";
    autoSizeWidth = false;
    autoSizeHeight = false;
    tab = false;
    canKeyFocus = true;
    returnTab = true;
};

if (!isObject(GuiSpinnerProfile)) new GuiControlProfile (GuiSpinnerProfile)
{
    fontSize = 16;
    opaque = false;
    justify = "center";
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "127 127 127 52";
    numbersOnly = true;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/textEdit_noSides";
    borderColor = "40 40 40 10";
    fontColor = "27 59 95 255";
    fontColorHL = "232 240 248 255";
    fontColorNA = "0 0 0 52";
    fontColorSEL = "0 0 0 255";
    textOffset = "4 2";
    autoSizeWidth = false;
    autoSizeHeight = false;
    tab = false;
    canKeyFocus = true;
    returnTab = true;
};

if (!isObject(GuiInactiveSpinnerProfile)) new GuiControlProfile (GuiInactiveSpinnerProfile)
{
    fontSize = 16;
    opaque = false;
    justify = "center";
    fillColor = "127 127 127 52";
    fillColorHL = "251 170 0 255";
    fillColorNA = "127 127 127 52";
    numbersOnly = true;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/textEdit_noSides_inactive";
    borderColor = "40 40 40 10";
    fontColor = "0 0 0 52";
    fontColorHL = "232 240 248 255";
    fontColorNA = "0 0 0 52";
    fontColorSEL = "0 0 0 255";
    textOffset = "4 2";
    autoSizeWidth = false;
    autoSizeHeight = false;
    tab = false;
    canKeyFocus = false;
    returnTab = false;
};

if (!isObject(GuiInvalidSpinnerProfile)) new GuiControlProfile (GuiInvalidSpinnerProfile)
{
    fontSize = 16;
    opaque = false;
    justify = "center";
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "127 127 127 52";
    numbersOnly = true;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/textEdit_noSides";
    borderColor = "40 40 40 10";
    fontColor = "255 0 0 255";
    fontColorHL = "232 240 248 255";
    fontColorNA = "0 0 0 52";
    fontColorSEL = "0 0 0 255";
    textOffset = "4 2";
    autoSizeWidth = false;
    autoSizeHeight = false;
    tab = false;
    canKeyFocus = true;
    returnTab = true;
};

if (!isObject(GuiFileEditBoxProfile)) new GuiControlProfile (GuiFileEditBoxProfile)
{
    fontSize = 16;
    opaque = false;
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "127 127 127 52";
    border = -2;
    bitmap = "^{EditorAssets}/data/images/textEdit_noRightEdge";
    borderColor = "40 40 40 10";
    fontColor = "27 59 95 255";
    fontColorHL = "232 240 248 255";
    fontColorNA = "0 0 0 52";
    fontColorSEL = "0 0 0 255";
    textOffset = "7 2";
    autoSizeWidth = false;
    autoSizeHeight = false;
    tab = false;
    canKeyFocus = true;
    returnTab = true;
};

if (!isObject(GuiObjectEditBoxProfile)) new GuiControlProfile (GuiObjectEditBoxProfile)
{
    fontSize = 16;
    opaque = false;
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "127 127 127 52";
    border = -2;
    borderColor = "40 40 40 10";
    bitmap = "^{EditorAssets}/data/images/textEdit_transparent";
    textOffset = "4 2";
    fontColor = "27 59 95 255";
    fontColorHL = "232 240 248 255";
    fontColorNA = "0 0 0 52";
    fontColorSEL= "10 10 10 255";
    autoSizeWidth = false;
    autoSizeHeight = false;
    tab = false;
    canKeyFocus = false;
    returnTab = false;
    Modal=true;
    fontType = "Open Sans";
    fontSize = 16;
    justify="left";
};

if (!isObject(GuiObjectEditBoxCenteredProfile)) new GuiControlProfile (GuiObjectEditBoxCenteredProfile : GuiObjectEditBoxProfile)
{
    justify="center";
};

if (!isObject(GuiTextEditInactiveProfile)) new GuiControlProfile (GuiTextEditInactiveProfile : GuiTextEditProfile)
{
    bitmap = "^{EditorAssets}/data/images/textEdit_inactive";
};

if (!isObject(GuiFileEditInactiveProfile)) new GuiControlProfile (GuiFileEditInactiveProfile : GuiTextEditProfile)
{
    bitmap = "^{EditorAssets}/data/images/textEdit_noRightEdge_inactive";
};

if (!isObject(GuiSpinnerInactiveProfile)) new GuiControlProfile (GuiSpinnerInactiveProfile : GuiTextEditProfile)
{
    bitmap = "^{EditorAssets}/data/images/textEdit_noSides_inactive";
};

if (!isObject(GuiTextEditNumericProfile)) new GuiControlProfile (GuiTextEditNumericProfile : GuiTextEditProfile)
{
    numbersOnly = true;
};

if (!isObject(GuiTextEditNumericCenteredProfile)) new GuiControlProfile (GuiTextEditNumericCenteredProfile : GuiTextEditNumericProfile)
{
    justify = "center";
};

if (!isObject(GuiTextEditCenteredProfile)) new GuiControlProfile (GuiTextEditCenteredProfile : GuiTextEditProfile)
{
    justify = "center";
};

if (!isObject(GuiTextEditNumericInactiveCenteredProfile)) new GuiControlProfile (GuiTextEditNumericInactiveCenteredProfile : GuiTextEditInactiveProfile)
{
    justify = "center";
};

if (!isObject(GuiConsoleTextEditProfile)) new GuiControlProfile (GuiConsoleTextEditProfile : GuiTextEditProfile)
{
    fontType = ($platform $= "macos") ? "Monaco" : "Lucida Console";
    fontSize = ($platform $= "macos") ? 13 : 12;
};

// ----------------------------------------------------------------------------
// Scroll Control Profiles
// ----------------------------------------------------------------------------
if (!isObject(GuiScrollProfile)) new GuiControlProfile (GuiScrollProfile)
{
    tab = false;
    canKeyFocus = false;
    hasBitmapArray = true;
    mouseOverSelected = false;

    // fill color
    opaque = false;
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "244 244 244 52";

    // border color
    border = 0;
    borderColor    = "100 100 100 255";
    borderColorHL = "128 128 128 255";
    borderColorNA = "226 226 226 52";

    // font
    fontType = "Open Sans";
    fontSize = 19;

    fontColor = "27 59 95 255";
    fontColorHL = "232 240 248 255";
    fontColorNA = "0 0 0 52";
    fontColorSEL= "10 10 10 255";

    // bitmap information
    bitmap = "^{EditorAssets}/data/images/scroll";
    bitmapBase = "";
    textOffset = "0 0";

    // used by guiTextControl
    modal = true;
    justify = "left";
    autoSizeWidth = false;
    autoSizeHeight = false;
    returnTab = false;
    numbersOnly = false;
    cursorColor = "0 0 0 255";

    // sounds
    soundButtonDown = "";
    soundButtonOver = "";
};

if (!isObject(ConsoleScrollProfile)) new GuiControlProfile(ConsoleScrollProfile : GuiScrollProfile)
{
	opaque = true;
	fillColor = "0 0 0 120";
	border = 3;
	borderThickness = 0;
	borderColor = "0 0 0 255";
};

if (!isObject(GuiLightScrollProfile)) new GuiControlProfile (GuiLightScrollProfile : GuiScrollProfile)
{
    opaque = false;
    fillColor = "212 216 220";
    border = 0;
    bitmap = "^{EditorAssets}/data/images/scroll";
    hasBitmapArray = true;
};

if (!isObject(GuiTransparentScrollProfile)) new GuiControlProfile (GuiTransparentScrollProfile : GuiScrollProfile)
{
    opaque = false;
    fillColor = "0 0 0 0";
    border = false;
    borderThickness = 0;
    borderColor = "0 0 0 0";
    bitmap = "^{EditorAssets}/data/images/transparentScroll";
    hasBitmapArray = true;
};
 
// ----------------------------------------------------------------------------
// Popup Menu Profiles
// ----------------------------------------------------------------------------
if (!isObject(GuiPopupMenuItemBorder)) new GuiControlProfile (GuiPopupMenuItemBorder : GuiButtonProfile)
{
    fillColorHL = "251 170 0 255";
    borderColor = "51 51 53 200";
    borderColorHL = "51 51 53 200";

    fontType = "Open Sans";
    fontSize = 16;

    fontColor = "27 59 95 255";
    fontColorHL = "232 240 248 255";
    fontColorNA = "0 0 0 52";
    fontColorSEL= "10 10 10 255";
};

if (!isObject(GuiPopUpMenuDefault)) new GuiControlProfile (GuiPopUpMenuDefault)
{
    tab = false;
    canKeyFocus = false;
    hasBitmapArray = true;

    // fill color
    opaque = false;
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "244 244 244 52";

    // border color
    border = 1;
    borderColor    = "100 100 100 255";
    borderColorHL = "128 128 128 255";
    borderColorNA = "226 226 226 52";

    // font
    fontType = "Open Sans";
    fontSize = 16;

    fontColor = "27 59 95 255";
    fontColorHL = "232 240 248 255";
    fontColorNA = "0 0 0 52";
    fontColorSEL= "10 10 10 255";

    // bitmap information
    bitmap = "^{EditorAssets}/data/images/dropDown";

    // used by guiTextControl
    modal = true;
    justify = "left";
    autoSizeWidth = false;
    autoSizeHeight = false;
    returnTab = false;
    numbersOnly = false;
    cursorColor = "0 0 0 255";

    profileForChildren = "GuiPopupMenuItemBorder";
    // sounds
    soundButtonDown = "";
    soundButtonOver = "";
};

if (!isObject(GuiPopUpMenuProfile)) new GuiControlProfile (GuiPopUpMenuProfile : GuiPopUpMenuDefault)
{
    textOffset = "6 3";
    fontType = "Open Sans";
    justify = "center";
};

if (!isObject(GuiPopUpMenuEmptyProfile)) new GuiControlProfile (GuiPopUpMenuEmptyProfile : GuiPopUpMenuDefault)
{
    textOffset = "6 3";
    fontType = "Open Sans Italic";
    justify = "center";
};

// ----------------------------------------------------------------------------
// Separator Profiles
// ----------------------------------------------------------------------------
if (!isObject(GuiSeparatorInactiveProfile)) new GuiControlProfile (GuiSeparatorInactiveProfile : GuiDefaultProfile)
{
    borderMargin = -2;
    borderColor    = "100 100 100 52";
    borderColorHL = "128 128 128 52";
    borderColorNA = "226 226 226 52";
};

if (!isObject(GuiSeparatorProfile)) new GuiControlProfile (GuiSeparatorProfile : GuiDefaultProfile)
{
    opaque = false;
    border = false;
    borderMargin = -2;
};

// ----------------------------------------------------------------------------
// Tab Book/Page Profiles
// ----------------------------------------------------------------------------
if (!isObject(GuiTabBookProfile)) new GuiControlProfile (GuiTabBookProfile)
{
    fillColor = "232 240 248 255";
    fillColorHL = "251 170 0 255";
    fillColorNA = "150 150 150 52";
    fontColor = "27 59 95 255";
    fontColorHL = "232 240 248 255";
    fontColorNA = "0 0 0 52";
    fontType = "open sans";
    fontSize = 16;
    justify = "center";
    bitmap = "^{EditorAssets}/data/images/tab";
    tabWidth = 64;
    tabHeight = 24;
    tabPosition = "Top";
    tabRotation = "Horizontal";
    textOffset = "0 0";
    tab = true;
    cankeyfocus = true;
};

if (!isObject(GuiTabPageProfile)) new GuiControlProfile (GuiTabPageProfile : GuiTransparentProfile)
{
    fillColor = "232 240 248";
    opaque = false;
    border = -2;
    bitmap = "^{EditorAssets}/data/images/tabContainer";
};

// ----------------------------------------------------------------------------
// Utility Profiles
// ----------------------------------------------------------------------------
if (!isObject(GUIImageEditorGridBoxProfile)) new GuiControlProfile (GUIImageEditorGridBoxProfile)
{
    border = 1;
    borderColor = "100 100 255 255";
    borderColorHL = "100 100 255 255";
    borderColorNA = "100 100 255 255";
    modal = false;
};

if (!isObject(GUIImageEditorGridLinesProfile)) new GuiControlProfile (GUIImageEditorGridLinesProfile)
{
    border = 7; // draw border on right&bottom only
    borderColor  = "100 100 255 255";
    borderColorHL = "100 100 255 255";
    borderColorNA = "100 100 255 255";
    modal = false;
};

if (!isObject(GUIImageEditorGridGreyBoxProfile)) new GuiControlProfile (GUIImageEditorGridGreyBoxProfile)
{
    border = 0;
    opaque = true;
    fillColor = "100 100 100 150";
    fillColorHL = "100 100 100 150";
    fillColorNA = "100 100 100 150";
    modal = false;
};

if (!isObject(GUICollisionEditorSelectionBoxProfile)) new GuiControlProfile (GUICollisionEditorSelectionBoxProfile)
{
    border = 1;
    borderColor = "200 200 0 255";
    //modal = false;
};

// ----------------------------------------------------------------------------
// Used by GUI Editor
// ----------------------------------------------------------------------------
if (!isObject(GuiBackFillProfile)) new GuiControlProfile (GuiBackFillProfile)
{
   opaque = true;
   fillColor = "0 94 94 255";
   border = true;
   borderColor = "255 128 128 255";
   fontType = "Arial";
   fontSize = 16;
   fontColor = "0 0 0 255";
   fontColorHL = "32 100 100 255";
   fixedExtent = true;
   justify = "center";
};

if (!isObject(GuiInspectorFieldProfile)) new GuiControlProfile (GuiInspectorFieldProfile)
{
   // fill color
   opaque = false;
   fillColor = "255 255 255 255";
   fillColorHL = "128 128 128 255";
   fillColorNA = "244 244 244 52";

   // border color
   border = false;
   borderColor   = "190 190 190 255";
   borderColorHL = "156 156 156 255";
   borderColorNA = "64 64 64 52";
   
   bevelColorHL = "255 255 255 255";
   bevelColorLL = "0 0 0";
   
   bitmap = "^{EditorAssets}/data/images/rollout";

   // font
   fontType = "Arial";
   fontSize = 16;

   fontColor = "32 32 32 255";
   fontColorHL = "32 100 100 255";
   fontColorNA = "0 0 0 52";

   tab = true;
   canKeyFocus = true;
};

if (!isObject(GuiInspectorBackgroundProfile)) new GuiControlProfile (GuiInspectorBackgroundProfile : GuiInspectorFieldProfile)
{
   border = 5;
   cankeyfocus=true;
   tab = true;
};
