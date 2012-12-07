//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$LocalPointEditor::dotMainColor =      0.0 SPC 0.5 SPC 1.0 SPC 1.0;
$LocalPointEditor::dotBorderColor =    0.0 SPC 0.0 SPC 1.0 SPC 1.0;
$LocalPointEditor::dotMainColor256 =   0 SPC 128 SPC 255 SPC 255;
$LocalPointEditor::dotBorderColor256 = 0 SPC 0 SPC 255 SPC 255;

$LocalPointEditor::dotErrorMainColor =      1.0 SPC 0.0 SPC 0.0 SPC 1.0;
$localPointEditor::dotErrorBorderColor =    0.0 SPC 0.0 SPC 1.0 SPC 1.0;
$LocalPointEditor::dotErrorMainColor256 =   255 SPC 0 SPC 0 SPC 255;
$LocalPointEditor::dotErrorBorderColor256 = 0 SPC 0 SPC 255 SPC 255;

$LocalPointEditor::precision = 3;
$LocalPointEditor::labelLineHeight = 0.029;
$LocalPointEditor::labelTextColor = "0 0 0 1";

$LocalPointEditor::selectGroup = 0;
$LocalPointEditor::ignoreGroup = 31;

$LocalPointEditor::gridLayer = 10;
$LocalPointEditor::polygonLayer = 8;
$LocalPointEditor::dotLayer  = 6;
$LocalPointEditor::labelLayer = 5;
$LocalPointEditor::editObjectLayer = 15;

$LocalPointEditor::dotSize = 0.05 SPC 0.05;

new GuiControlProfile(LocalPointEditorDotBGProfile)
{
   border = true;
   opaque = true;
   fillColor =     $LocalPointEditor::dotMainColor256;
   fillColorHL =   $LocalPointEditor::dotMainColor256;
   borderColor =   $LocalPointEditor::dotBorderColor256;
   borderColorHL = $LocalPointEditor::dotBorderColor256;
   modal = false;
};

new GuiControlProfile(LocalPointEditorDotBGErrorProfile)
{
   border = true;
   opaque = true;
   fillColor =     $LocalPointEditor::dotErrorMainColor256;
   fillColorHL =   $LocalPointEditor::dotErrorMainColor256;
   borderColor =   $LocalPointEditor::dotErrorBorderColor256;
   borderColorHL = $LocalPointEditor::dotErrorBorderColor256;
   modal = false;
};
   
new GuiControlProfile(LocalPointEditorDotTextProfile : EditorTextHLBoldRight)
{
   justify = center;
   modal = false;
};

new GuiControlProfile(LocalPointText : EditorTextHLBoldRight)
{
   modal = false;
};

new GuiControlProfile(LocalPointWindowProfile : EditorWindowProfile)
{
   canKeyFocus = true;
};
