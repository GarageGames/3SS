//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

if(!isObject(TerrainDialogProfile)) new GuiControlProfile (TerrainDialogProfile)
{
   opaque = false;
   border = false;
   modal = false;
};

if(!isObject(TerrainTabBookProfile)) new GuiControlProfile (TerrainTabBookProfile : GuiTabBookProfile)
{
   textOffset = "0 1";
};

if(!isObject(TerrainPathTitleProfile)) new GuiControlProfile (TerrainPathTitleProfile : GuiTextProfile)
{
   justify = "center";
};

if(!isObject(TerrainPathDisabledProfile)) new GuiControlProfile (TerrainPathDisabledProfile)
{
   border = false;
   opaque = true;
   fillColor = "192 192 192 128";
};

if(!isObject(TerrainNoBorderProfile)) new GuiControlProfile (TerrainNoBorderProfile)
{
   border = false;
   borderColor = "0 0 0 0";
};
if(!isObject(WaveEnemyPreviewProfile)) new GuiControlProfile (WaveEnemyPreviewProfile)
{
   modal = true;
};

if(!isObject(GuiDisabledProfile)) new GuiControlProfile (GuiDisabledProfile)
{
   // fill color
   opaque = true;
   fillColor = "58 58 58";
   fillColorHL = "64 64 64";
   fillColorNA = "37 37 37";
};

if(!isObject(GuiDarkLabelProfile))new GuiControlProfile (GuiDarkLabelProfile)
{
   fontColor = "0 0 0";
};

if(!isObject(GuiMLWhiteTextProfile)) new GuiControlProfile (GuiMLWhiteTextProfile : GuiMLTextProfile)
{
   fontColor = "255 255 255";
};

if(!isObject(GuiGeneralSettingsScrollProfile)) new GuiControlProfile (GuiGeneralSettingsScrollProfile : GuiScrollProfile)
{
   border = 0;
};
