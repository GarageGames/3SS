//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

$CurrencyEditorMaximumElements = 4;


/// <summary>
/// Gets the array index of the control in CurrencyEditorCurrencyArray.
/// Returns -1 if the control was not found in the array.
/// </summary>
/// <param name="control">The control whose index we are searching for.</param>
/// <return>Array index.</return>
function CurrencyEditorGUI::getControlIndex(%this, %control)
{
   for (%i = 0; %i < $CurrencyEditorMaximumElements; %i++)
   {
      if (CurrencyEditorCurrencyArray.getObject(%i) == %control.getParent().getParent())
         return %i;
   }
   
   return -1;
}

/// <summary>
/// AssectSelector callback.
/// </summary>
function CurrencyEditorGUI::setSelectedAsset(%this, %asset)
{
   // Update imageMap name and preview
   %currencyElement = CurrencyEditorCurrencyArray.getObject($CurrentCurrencyElementIndex);   

   %imagePath = %currencyElement.findObjectByInternalName("ImagePathTextEdit", true);
   %imagePath.setText(%asset);

   %imageFrame = %currencyElement.findObjectByInternalName("ImageFrameTextEdit", true);
   // todo (2012/03/21): use actual frame value when it is made available.
   %imageFrame.setValue(0);
   
   %imageFrameContainer = %currencyElement.findObjectByInternalName("ImageFrameContainer", true);
   %imageFrameContainer.Visible = (%imagePath.getText().getFrameCount() > 1);

   %imagePreview = %currencyElement.findObjectByInternalName("ImagePreview", true);
   %imagePreview.display(%imagePath.getText(), "t2dStaticSprite");
   %imagePreview.sprite.setFrame(%imageFrame.getValue());
}

/// <summary>
/// Button calling AssectSelector 
/// </summary>
function CurrencyEditorGUI::selectButtonPressed(%this)
{
   %index = %this.getControlIndex($ThisControl);
   
   if (%index == -1)
   {
      warn("Error: Could not get control's array index!");
      return;
   }
   
   // Load asset browser
   $CurrentCurrencyElementIndex = %index;
   AssetLibrary.open(CurrencyEditorGUI, $SpriteSheetPage, "Chips");
}

/// <summary>
/// Called when the control is woken up. 
/// </summary>
function CurrencyEditorGUI::onWake(%this)
{   
   %this.refresh();
}

/// <summary>
/// Updates the GUI state from the game data.
/// </summary>
function CurrencyEditorGUI::refresh(%this)
{
   %scene = ToolManager.getLastWindow().getScene();
   
   for (%i = 0; %i < $CurrencyEditorMaximumElements; %i++)
   { 
      %this.bankStack[%i] = null;
   }
   
   %index = 0;
   for (%i = 0; %i < %scene.getSceneObjectCount(); %i++)
   {
      %sceneObject = %scene.getSceneObject(%i);
      
      if (isObject(%sceneObject.getBehavior("BankStackBehavior")))
      {
         %this.bankStack[%index] = %sceneObject;     
         %index++;
      }
   }    
   
   %count = 0;    
   
   // Loop through each currency denomination
   for (%i = 0; %i < $CurrencyEditorMaximumElements; %i++)
   {   
      // Get the GUI array element
      %currencyElement = CurrencyEditorCurrencyArray.getObject(%i);      
      
      if (isObject(%this.bankStack[%i]))
      {        
         // Get the behavior of the bank stack object
         %bankStackBehavior = %this.bankStack[%i].getBehavior("BankStackBehavior");  
         
         // Update the currency element label
         %count++;
         %labelText = "Currency" SPC %count;
         %currencyLabel = %currencyElement.findObjectByInternalName("CurrencyLabel", true);
         if(isObject(%currencyLabel))
            %currencyLabel.setText(%labelText);
         
         
         // Update the isEnabled checkbox
         %isEnabledCheckBox = %currencyElement.findObjectByInternalName("IsEnabledBox", true);
         %isEnabledCheckBox.setStateOn(%this.bankStack[%i].isEnabled());
         
         // Update ValueTextEdit
         %value = %bankStackBehavior.denomination;
         %valueText = %currencyElement.findObjectByInternalName("ValueTextEdit", true);
         %valueText.setText(%value);
         
         // Update ImagePathTextEdit and ImagePreview
         %imageMap = %this.bankStack[%i].getImageMap();
         %imagePath = %currencyElement.findObjectByInternalName("ImagePathTextEdit", true);
         %imagePath.setText(%imageMap);
         
         %imageFrame = %currencyElement.findObjectByInternalName("ImageFrameTextEdit", true);
         %imageFrame.setValue(%this.bankStack[%i].getFrame());
   
         %imageFrameContainer = %currencyElement.findObjectByInternalName("ImageFrameContainer", true);
         %imageFrameContainer.Visible = (%imagePath.getText().getFrameCount() > 1);
             
         %imagePreview = %currencyElement.findObjectByInternalName("ImagePreview", true);
         %imagePreview.display(%imagePath.getText(), "t2dStaticSprite");
         %imagePreview.sprite.setFrame(%imageFrame.getValue());
            
         // Make sure control is visible
         %currencyElement.setVisible(true);
      }
      else
      {
         %currencyElement.setVisible(false);
      }
   }
}

/// <summary>
/// Validates the value text fields of all containers in the currency editor
/// array.
/// </summary>
function CurrencyEditorGUI::validateAllValueTextFields(%this)
{
   for (%i = 0; %i < $CurrencyEditorMaximumElements; %i++)
   { 
      %currencyElement = CurrencyEditorCurrencyArray.getObject(%i);     
      
      if (isObject(%this.bankStack[%i]))
      {
         %textEdit = %currencyElement.findObjectByInternalName("ValueTextEdit", true);
         ValidateTextEditInteger(%textEdit, 1, "Inf");
         %textEdit.validatedText = %textEdit.getText();
      }
   }
}

/// <summary>
/// Serializes the state of the GUI into a space-delimited string.
/// </summary>
/// <return>A space-delimited string.</return>
function CurrencyEditorGUI::serialize(%this)
{
   %serialString = "";
   
   for (%i = 0; %i < $CurrencyEditorMaximumElements; %i++)
   { 
      %currencyElement = CurrencyEditorCurrencyArray.getObject(%i);     
      
      if (isObject(%this.bankStack[%i]))
      {
         %serialString = %currencyElement.findObjectByInternalName("IsEnabledBox", true).getValue() SPC %serialString;
         %serialString = %currencyElement.findObjectByInternalName("ImagePathTextEdit", true).getText() SPC %serialString;
         %serialString = %currencyElement.findObjectByInternalName("ImageFrameTextEdit", true).getValue() SPC %serialString;
         //Ben: %serialString = %currencyElement.findObjectByInternalName("ValueTextEdit", true).getText() SPC %serialString;
         %serialString = %currencyElement.findObjectByInternalName("ValueTextEdit", true).validatedText SPC %serialString;
      }
   }
   
   return %serialString;
}

/// <summary>
/// Handle click on the spin up button.
/// </summary>
function CurrencyEditorGUI::spinUpClicked(%this, %valueControl)
{
   %valueControl.setValue(%valueControl.getValue() + 1);
   CurrencyEditorGUI.validateAllValueTextFields();
}

/// <summary>
/// Handle click on the spin down button.
/// </summary>
function CurrencyEditorGUI::spinDownClicked(%this, %valueControl)
{
   %valueControl.setValue(%valueControl.getValue() - 1);
   CurrencyEditorGUI.validateAllValueTextFields();
}

/// <summary>
/// Validate the frame edit box.
/// </summary>
function CurrencyEditorGUI::validateFrameEditBox(%this, %valueControl, %imagePath, %imageWindow)
{
   if (!isObject(%imageWindow.sprite)) 
   {
      %valueControl.setValue(0);
      return;
   }
   
   if (%valueControl.getValue() < 0)
      %valueControl.setValue(0);
   else if (%valueControl.getValue() > (%imagePath.getText().getFrameCount() - 1))
      %valueControl.setValue(%imagePath.getText().getFrameCount() - 1);
      
   %imageWindow.sprite.setFrame(%valueControl.getValue());
}

/// <summary>
/// Handle click on left frame spinner.
/// </summary>
function CurrencyEditorGUI::frameSpinLeftClicked(%this, %valueControl, %imagePath, %imageWindow)
{
   %valueControl.setValue(%valueControl.getValue() - 1);
   CurrencyEditorGUI.validateFrameEditBox(%valueControl, %imagePath, %imageWindow);
}

/// <summary>
/// Handle click on right frame spinner.
/// </summary>
function CurrencyEditorGUI::frameSpinRightClicked(%this, %valueControl, %imagePath, %imageWindow)
{
   %valueControl.setValue(%valueControl.getValue() + 1);
   CurrencyEditorGUI.validateFrameEditBox(%valueControl, %imagePath, %imageWindow);
}
