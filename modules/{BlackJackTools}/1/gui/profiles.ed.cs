//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// Darken row.
if(!isObject(GuiTabBookDarkProfile)) new GuiControlProfile (GuiTabBookDarkProfile)
{
   fillColor = "192 192 192";
   fillColorHL = "64 150 150";
   fillColorNA = "150 150 150";
   fontColor = "30 30 30";
   fontColorHL = "32 100 100";
   fontColorNA = "0 0 0";
   fontType = "Arial Bold";
   fontSize = 14;
   justify = "center";
   textOffset = "0 -2";
   tab = true;
   cankeyfocus = true;
};

// Blackjack "Hit" button profile.
if(!isObject(GuiBJHitProfile)) new GuiControlProfile (GuiBJHitProfile)
{
   fillColor = "170 24 24";
   fillColorHL = "64 150 150";
   fillColorNA = "150 150 150";
   fontColor = "255 255 255";
   fontColorHL = "32 100 100";
   fontColorNA = "0 0 0";
   fontType = "Arial Bold";
   fontSize = 14;
   justify = "center";
   textOffset = "0 -2";
   tab = true;
   cankeyfocus = true;
};

// Blackjack "Double" button profile.
if(!isObject(GuiBJDoubleProfile)) new GuiControlProfile (GuiBJDoubleProfile)
{
   fillColor = "169 170 24";
   fillColorHL = "64 150 150";
   fillColorNA = "150 150 150";
   fontColor = "255 255 255";
   fontColorHL = "32 100 100";
   fontColorNA = "0 0 0";
   fontType = "Arial Bold";
   fontSize = 14;
   justify = "center";
   textOffset = "0 -2";
   tab = true;
   cankeyfocus = true;
};

// Blackjack "Split" button profile.
if(!isObject(GuiBJSplitProfile)) new GuiControlProfile (GuiBJSplitProfile)
{
   fillColor = "45 157 157";
   fillColorHL = "64 150 150";
   fillColorNA = "150 150 150";
   fontColor = "255 255 255";
   fontColorHL = "32 100 100";
   fontColorNA = "0 0 0";
   fontType = "Arial Bold";
   fontSize = 14;
   justify = "center";
   textOffset = "0 -2";
   tab = true;
   cankeyfocus = true;
};

// Blackjack "Stand" button profile.
if(!isObject(GuiBJStandProfile)) new GuiControlProfile (GuiBJStandProfile)
{
   fillColor = "25 170 24";
   fillColorHL = "64 150 150";
   fillColorNA = "150 150 150";
   fontColor = "255 255 255";
   fontColorHL = "32 100 100";
   fontColorNA = "0 0 0";
   fontType = "Arial Bold";
   fontSize = 14;
   justify = "center";
   textOffset = "0 -2";
   tab = true;
   cankeyfocus = true;
};

// Is Active button profile.
if(!isObject(GuiBJActiveProfile)) new GuiControlProfile (GuiBJActiveProfile)
{
   fillColor = "0 255 0";
   fillColorHL = "64 150 150";
   fillColorNA = "150 150 150";
   fontColor = "30 30 30";
   fontColorHL = "32 100 100";
   fontColorNA = "0 0 0";
   fontType = "Arial Bold";
   fontSize = 14;
   justify = "center";
   textOffset = "0 -2";
   tab = true;
   cankeyfocus = true;
};

// Is InActive button profile.
if(!isObject(GuiBJInactiveProfile)) new GuiControlProfile (GuiBJInactiveProfile)
{
   fillColor = "255 0 0";
   fillColorHL = "64 150 150";
   fillColorNA = "150 150 150";
   fontColor = "30 30 30";
   fontColorHL = "32 100 100";
   fontColorNA = "0 0 0";
   fontType = "Arial Bold";
   fontSize = 14;
   justify = "center";
   textOffset = "0 -2";
   tab = true;
   cankeyfocus = true;
};

// Title Profile.
if(!isObject(GuiBJTitleProfile)) new GuiControlProfile (GuiBJTitleProfile)
{
   fontType = "Arial Bold";
   fontSize = 20;
   justify = "center";
};

// Title Profile.
if(!isObject(GuiBJTitle2Profile)) new GuiControlProfile (GuiBJTitle2Profile)
{
   fillColor = "0 255 0";
   fillColorHL = "64 150 150";
   fillColorNA = "150 150 150";
   fontColor = "30 30 30";
   fontColorHL = "32 100 100";
   fontColorNA = "0 0 0";
   fontType = "Arial Bold";
   fontSize = 14;
   justify = "center";
};

// List Title Profile.
if(!isObject(GuiBJListTitleProfile)) new GuiControlProfile (GuiBJListTitleProfile)
{
   fontType = "Arial Bold";
   fontSize = 16;
   fontColor = "229 229 229";
   justify = "Left";
};

// Image Title Profile.
if(!isObject(GuiBJListBoxProfile)) new GuiControlProfile (GuiBJListBoxProfile)
{
   borderColor   = "20 20 20"; 
   justify = "left";
   border = true;
   profileForChildren = GuiSolidDefaultProfile;
};

if(!isObject(GuiBJScrollProfile)) new GuiControlProfile (GuiBJScrollProfile : GuiScrollProfile)
{
   border = false;
   fillColor = "51 51 51";
};
