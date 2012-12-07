//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
/*$ColorPickerCallback = ""; // Control that we need to update
$ColorCallbackType   = 1;  // ColorI

// This function pushes the color picker dialog and returns to a callback the selected value
function GetColorI( %currentColor, %callback )
{
   $ColorPickerCallback = %callback;
   $ColorCallbackType = 1; // ColorI
   
   // Set the RGBA displays accordingly
   %red = getWord(%currentColor, 0);
   %green = getWord(%currentColor, 1);
   %blue = getWord(%currentColor, 2);
   %alpha = getWord(%currentColor, 3);
   
   setColorInfo(%red, %green, %blue, %alpha);
   
   ColorBlendSelect.pickColor = %red SPC %green SPC %blue SPC "1.0";
   ColorBlendSelect.updateColor();
   ColorAlphaSelect.setValue(%alpha);

   Canvas.pushDialog(ColorPickerDlg);
}

function GetColorF( %currentColor, %callback )
{
   $ColorPickerCallback = %callback;
   $ColorCallbackType = 2; // ColorF
   
   // Set the RGBA displays accordingly
   %red = getWord(%currentColor, 0);
   %green = getWord(%currentColor, 1);
   %blue = getWord(%currentColor, 2);
   %alpha = getWord(%currentColor, 3);

   setColorInfo(%red, %green, %blue, %alpha);

   ColorBlendSelect.pickColor = %red SPC %green SPC %blue SPC "1.0";
   ColorBlendSelect.updateColor();
   ColorAlphaSelect.setValue(%alpha);

   Canvas.pushDialog(ColorPickerDlg);
}


function DoColorPickerCallback()
{
  eval( $ColorPickerCallback @ "(\"" @ constructNewColor(ColorBlendSelect.getValue(), $ColorCallbackType) @"\");" );

  Canvas.popDialog(ColorPickerDlg);
}   

// This function updates the base color on the blend control
function updatePickerBaseColor()
{
   %pickColor = ColorRangeSelect.getValue();
   %red = getWord(%pickColor, 0);
   %green = getWord(%pickColor, 1);
   %blue = getWord(%pickColor, 2);

   ColorBlendSelect.baseColor = %red SPC %green SPC %blue SPC "1.0";
   ColorBlendSelect.updateColor();
}

// This function is used to update the text controls at the top
function setColorInfo(%red, %green, %blue, %alpha)
{
   Channel_R_Val.setValue("R :" SPC mCeil(%red * 255));
   Channel_G_Val.setValue("G :" SPC mCeil(%green * 255));
   Channel_B_Val.setValue("B :" SPC mCeil(%blue * 255));
   Channel_A_Val.setValue("A :" SPC mCeil(%alpha * 255));
}

// This function constructs a new color, and updates the text displays accordingly
function constructNewColor(%pickColor, %colorType )
{
   %red = getWord(%pickColor, 0);
   %green = getWord(%pickColor, 1);
   %blue = getWord(%pickColor, 2);
   %alpha = ColorAlphaSelect.getValue();
   
   // Update the text controls to reflect new color
   setColorInfo(%red, %green, %blue, %alpha);
   if ( %colorType == 1 ) // ColorI
      return mCeil( %red * 255 ) SPC mCeil( %green * 255 ) SPC mCeil( %blue * 255 ) SPC mCeil( %alpha * 255 );
   else // ColorF
      return %red SPC %green SPC %blue SPC %alpha;
}


// Functions to deal with the color dropper
function startColorDropper()
{
   //ColorPickerDlg.command = "ColorDropperSelect.baseColor = ColorPickerDlg.pickColor;";
   ColorPickerDlg.altCommand = $pickerUpdateControl@".setValue(constructNewColor(ColorPickerDlg.pickColor));endColorDropper();";
   ColorPickerDlg.setActive(true);
   $pickerActive = true;
}

function endColorDropper()
{
   ColorPickerDlg.command = "";
   ColorPickerDlg.altCommand = "";
   ColorPickerDlg.setActive(false);
   $pickerActive = false;
}

function toggleColorPicker()
{
   if ($pickerActive)
      endColorDropper();
   else
      startColorDropper();
}*/