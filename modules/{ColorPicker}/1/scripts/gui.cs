//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
function ColorPickerGui::onWake(%this)
{
}


// This function pushes the color picker dialog and returns to a callback the selected value
function GetColorI( %currentColor, %callback )
{
    $ColorPickerCallback = %callback;
    $ColorCallbackType = 1; // ColorI
   
    // Set the RGBA displays accordingly
    %red = getWord(%currentColor, 0)/255;
    %green = getWord(%currentColor, 1)/255;
    %blue = getWord(%currentColor, 2)/255;
    %alpha = getWord(%currentColor, 3)/255;
   
    setColorInfo(%red, %green, %blue, %alpha);
   
    ColorPickerBlendSelect.pickColor = %red SPC %green SPC %blue SPC "1.0";
    ColorPickerBlendSelect.updateColor();
    ColorPickerAlphaSelect.setValue(%alpha);

    // Update preview profile
    updateColorPickerPreview();
    
    updateSelectorSprites(%red, %green, %blue, %alpha);
   
    Canvas.pushDialog(ColorPickerGui);
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

    ColorPickerBlendSelect.pickColor = %red SPC %green SPC %blue SPC "1.0";
    ColorPickerBlendSelect.updateColor();
    ColorPickerAlphaSelect.setValue(%alpha);

    // Update preview profile
    updateColorPickerPreview();
    
    updateSelectorSprites(%red, %green, %blue, %alpha);

    Canvas.pushDialog(ColorPickerGui);
}


function DoColorPickerCallback()
{
  eval( $ColorPickerCallback @ "(\"" @ constructNewColor(ColorPickerBlendSelect.getValue(), $ColorCallbackType) @"\");" );

  Canvas.popDialog(ColorPickerGui);
}   

// This function updates the base color on the blend control
function updatePickerBaseColor()
{
    %pickColor = ColorPickerBaseRangeSelect.getValue();
    %red = getWord(%pickColor, 0);
    %green = getWord(%pickColor, 1);
    %blue = getWord(%pickColor, 2);
    %alpha = getWord(%pickColor, 3);

    ColorPickerBlendSelect.baseColor = %red SPC %green SPC %blue SPC "1.0";
    ColorPickerBlendSelect.updateColor();
}

function ColorPickerAlphaSelect::onMouseDragged(%this)
{
    updateColorPickerPreview();
    ColorPickerGui.updateTextFromBlendSelect();
}

function ColorPickerGui::updateTextFromBlendSelect()
{
    %pickColorF = ColorPickerBlendSelect.getValue();
    %red = getWord(%pickColorF, 0);
    %green = getWord(%pickColorF, 1);
    %blue = getWord(%pickColorF, 2);
    %alpha = ColorPickerAlphaSelect.getValue();
    
    setColorInfo(%red, %green, %blue, %alpha);
}

function ColorPickerBlendSelect::onMouseDragged(%this)
{
    ColorPickerGui.updateTextFromBlendSelect();
}

function ColorPickerBlendSelect::onMouseDown(%this)
{
    ColorPickerGui.updateTextFromBlendSelect();
}

function ColorPickerBlendSelect::onAction(%this)
{ 
    updateColorPickerPreview();
    %this.updateSelector(%this.getSelectorPos());
}

function ColorPickerBaseRangeSelect::onMouseDragged(%this)
{
    ColorPickerGui.updateTextFromBlendSelect();
}

function ColorPickerBaseRangeSelect::onMouseDown(%this)
{
    ColorPickerGui.updateTextFromBlendSelect();
}

function ColorPickerBaseRangeSelect::onAction(%this)
{ 
    updatePickerBaseColor();
    updateColorPickerPreview(); 
    %this.updateSelector(%this.getSelectorPos());
}

function ColorPickerBlendSelect::updateSelector(%this, %position)
{
    %selectorPosition = %position; 
    %selectorPosition.x = %selectorPosition.x - ColorPickerBlendSelector.getExtent().x/2;
    %selectorPosition.y = %selectorPosition.y - ColorPickerBlendSelector.getExtent().y/2;       
        
    ColorPickerBlendSelector.setPosition(%selectorPosition.x, %selectorPosition.y);
}

function ColorPickerBaseRangeSelect::updateSelector(%this, %position)
{
    %selectorPosition = %position;
    %selectorPosition.x = %this.getExtent().x/2 - ColorPickerRangeSelector.getExtent().x/2;
    %selectorPosition.y = %selectorPosition.y - ColorPickerRangeSelector.getExtent().y/2;
    
    ColorPickerRangeSelector.setPosition(%selectorPosition.x, %selectorPosition.y);
}

function updateSelectorSprites(%redF, %greenF, %blueF, %alphaF)
{
    %hueSelectorPosition = ColorPickerBaseRangeSelect.getSelectorPosForColor(%redF, %greenF, %blueF, %alphaF);
    //ColorPickerBaseRangeSelect.updateSelector(%hueSelectorPosition);
    ColorPickerBaseRangeSelect.setSelectorPos(%hueSelectorPosition);
    
    %blendSelectorPosition = ColorPickerBlendSelect.getSelectorPosForColor(%redF, %greenF, %blueF, %alphaF);
    //ColorPickerBlendSelect.updateSelector(%blendSelectorPosition);
    ColorPickerBlendSelect.setSelectorPos(%blendSelectorPosition);
}

function updateColorPickerPreview()
{
    %pickColorF = ColorPickerBlendSelect.getValue();
    %pickColorI = constructNewColor(%pickColorF, 1);
    
    %red = getWord(%pickColorI, 0);
    %green = getWord(%pickColorI, 1);
    %blue = getWord(%pickColorI, 2);
    %alpha = getWord(%pickColorI, 3);

    GuiColorPickerPreviewProfile.fillColor = %red SPC %green SPC %blue SPC %alpha;
}

// This function is used to update the text controls at the top
// Returns true if the color was changed, false if all color components stayed the same
function setColorInfo(%redF, %greenF, %blueF, %alphaF)
{
    %redI = mRound(%redF * 255);
    %greenI = mRound(%greenF * 255);
    %blueI = mRound(%blueF * 255);
    %alphaI = mRound(%alphaF * 100);    
    
    if (%redI == $ColorPickerLastR 
        && %greenI == $ColorPickerLastG 
        && %blueI == $ColorPickerLastB 
        && %alphaI == $ColorPickerLastA)
        %colorChanged = false;
    else
        %colorChanged = true;
    
    $ColorPickerLastR = %redI;
    $ColorPickerLastG = %greenI;
    $ColorPickerLastB = %blueI;
    $ColorPickerLastA = %alphaI;
    
    ColorPickerChannelRText.setValue(%redI);
    ColorPickerChannelGText.setValue(%greenI);
    ColorPickerChannelBText.setValue(%blueI);
    ColorPickerOpacityText.setValue(%alphaI);

    %redHex = toHex(%redI);
    %greenHex = toHex(%greenI);
    %blueHex = toHex(%blueI);

    if (strlen(%redHex) == 1)
        %redHex = "0" @ %redHex;
    if (strlen(%greenHex) == 1)
        %greenHex = "0" @ %greenHex;
    if (strlen(%blueHex) == 1)
        %blueHex = "0" @ %blueHex;

    ColorPickerHexText.setValue(%redHex @ %greenHex @ %blueHex);
    
    return %colorChanged;
}

function ColorPickerOpacityText::onValidate(%this)
{
    ColorPickerAlphaSelect.setValue(%this.getValue()/100);
    updateColorPickerPreview();
}

function ColorPickerHexText::onValidate(%this)
{
    // Get the RGB values from the hex value
    %color = hexToRGB(%this.getValue()); 
    %red = getWord(%color, 0);
    %green = getWord(%color, 1);
    %blue = getWord(%color, 2);   
    %alpha = ColorPickerOpacityText.getValue();
    
    // Check that the entered hex is a valid color
    // If not revert it to the old value
    %valid = true;
    if (!(%red >= 0) || !(%red <= 255))
        %valid = false;
    if (!(%green >= 0) || !(%green <= 255))
        %valid = false;
    if (!(%blue >= 0) || !(%blue <= 255))
        %valid = false;
    
    if (%valid == false)
    {
        updateColorPickerPreview();
        return;   
    }
        
    %colorChanged = setColorInfo(%red/255, %green/255, %blue/255, %alpha/100);
    
    if (%colorChanged)
        updateSelectorSprites(%red/255, %green/255, %blue/255, %alpha/100);
}

function validateColorPickerChannelText(%editTextGui)
{
    %value = %editTextGui.getValue();    
    
    // Restrict to legal values
    if (%value < 0)
        %value = 0;
    if (%value > 255)
        %value = 255;
        
    %editTextGui.setValue(%value);
        
    // Update picker controls based on new RGB values
    %redI = ColorPickerChannelRText.getValue();
    %greenI = ColorPickerChannelGText.getValue();
    %blueI = ColorPickerChannelBText.getValue();
    %alphaI = ColorPickerOpacityText.getValue();
    
    // Refresh text info
    //updateColorPickerPreview();
    %colorChanged = setColorInfo(%redI/255, %greenI/255, %blueI/255, %alphaI/100);
    
    if (%colorChanged)
        updateSelectorSprites(%redI/255, %greenI/255, %blueI/255, %alphaI/100);
}

//
// R Channel
//
function ColorPickerChannelRUpButton::onClick()
{
    ColorPickerChannelRText.setValue(ColorPickerChannelRText.getValue() + 1);
    ColorPickerChannelRText.onValidate();
}

function ColorPickerChannelRDownButton::onClick()
{
    ColorPickerChannelRText.setValue(ColorPickerChannelRText.getValue() - 1);
    ColorPickerChannelRText.onValidate();
}

function ColorPickerChannelRText::onValidate()
{
    validateColorPickerChannelText(ColorPickerChannelRText);
}

//
// G Channel
//
function ColorPickerChannelGUpButton::onClick()
{
    ColorPickerChannelGText.setValue(ColorPickerChannelGText.getValue() + 1);
    ColorPickerChannelGText.onValidate();
}

function ColorPickerChannelGDownButton::onClick()
{
    ColorPickerChannelGText.setValue(ColorPickerChannelGText.getValue() - 1);
    ColorPickerChannelGText.onValidate();
}

function ColorPickerChannelGText::onValidate()
{
    validateColorPickerChannelText(ColorPickerChannelGText);
}

//
// B Channel
//
function ColorPickerChannelBUpButton::onClick()
{
    ColorPickerChannelBText.setValue(ColorPickerChannelBText.getValue() + 1);
    ColorPickerChannelBText.onValidate();
}

function ColorPickerChannelBDownButton::onClick()
{
    ColorPickerChannelBText.setValue(ColorPickerChannelBText.getValue() - 1);
    ColorPickerChannelBText.onValidate();
}

function ColorPickerChannelBText::onValidate()
{
    validateColorPickerChannelText(ColorPickerChannelBText);
}



// This function constructs a new color, and updates the text displays accordingly
function constructNewColor(%pickColor, %colorType )
{
    %red = getWord(%pickColor, 0);
    %green = getWord(%pickColor, 1);
    %blue = getWord(%pickColor, 2);
    %alpha = ColorPickerAlphaSelect.getValue();

    if ( %colorType == 1 ) // ColorI
        return mRound( %red * 255 ) SPC mRound( %green * 255 ) SPC mRound( %blue * 255 ) SPC mRound( %alpha * 255 );
    else // ColorF
        return %red SPC %green SPC %blue SPC %alpha;
}

function hexToRGB(%hexColor)
{
    %red = toDecimal(getSubStr(%hexColor, 0, 2));   
    %green = toDecimal(getSubStr(%hexColor, 2, 2));   
    %blue = toDecimal(getSubStr(%hexColor, 4, 2));  
    
    return %red SPC %green SPC %blue;
}

function toDecimal(%hex)
{
    %decimal = 0;
    %base = 1;
    for (%i = strlen(%hex) - 1; %i >= 0; %i--)
    {   
        %char = getSubStr(%hex, %i, 1);
        switch$(%char)
        {
            case "A":
                %char = 10;
            case "B":
                %char = 11;
            case "C":
                %char = 12;
            case "D":
                %char = 13;
            case "E":
                %char = 14;
            case "F":
                %char = 15;
        }
        
        %decimal += %base * %char;
        %base *= 16;
    }
    
    return %decimal;
}

function toHex(%decimal)
{
    %remainder = %decimal % 16;
    
    %digits = "0123456789ABCDEF";
    %char = getSubStr(%digits, %remainder, 1); 
    
    if (%decimal - %remainder == 0)
    {
        return %char;
    }
    else
    {
        %string =  toHex( (%decimal - %remainder) / 16 ) @ %char;
        return %string;
    }
}



// Functions to deal with the color dropper
function startColorDropper()
{
   //ColorPickerGui.command = "ColorDropperSelect.baseColor = ColorPickerGui.pickColor;";
   ColorPickerGui.altCommand = $pickerUpdateControl@".setValue(constructNewColor(ColorPickerGui.pickColor));endColorDropper();";
   ColorPickerGui.setActive(true);
   $pickerActive = true;
}

function endColorDropper()
{
   ColorPickerGui.command = "";
   ColorPickerGui.altCommand = "";
   ColorPickerGui.setActive(false);
   $pickerActive = false;
}

function toggleColorPicker()
{
   if ($pickerActive)
      endColorDropper();
   else
      startColorDropper();
}
