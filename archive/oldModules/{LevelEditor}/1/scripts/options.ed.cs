//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// rdbnote: this wouldn't be necessary if everything was built into the
// options dialog like normal...
package OptionsHackPackage
{
   function OptionsDlg::onWake(%this)
   {
      Parent::onWake(%this);
      
      //
      // store the current values so we can revert them if necessary
      //
      
      T2DLVLShowGrid.oldValue = $levelEditor::ShowGrid;
      T2DLVLGridCheckX.oldValue = $levelEditor::SnapX;
      T2DLVLGridCheckY.oldValue = $levelEditor::SnapY;
      T2DLVLGridUnitX.oldValue = $levelEditor::GridSizeX;
      T2DLVLGridUnitY.oldValue = $levelEditor::GridSizeY;

      T2DBGColorControl.oldValue1 = $levelEditor::BackgroundColor;
      T2DBGColorControl.oldValue2 = $levelEditor::BackgroundColorPos;
      T2DGridColorControl.oldValue1 = $levelEditor::GridColor;
      T2DGridColorControl.oldValue2 = $levelEditor::GridColorPos;
      
      // just set these all the same, will get straightened out in the revert
      T2DObjectLibraryColorLight.oldValue = $levelEditor::ObjectLibraryBackgroundColor;
      //T2DObjectLibraryColorMedium.oldValue = $levelEditor::ObjectLibraryBackgroundColor;
      //T2DObjectLibraryColorDark.oldValue = $levelEditor::ObjectLibraryBackgroundColor;
      
      T2DSnapThresholdText.oldValue = $levelEditor::SnapThreshold;
      T2DFullContainToggle.oldValue = $levelEditor::FullContainSelection;
      T2DUndoSelectionsToggle.oldValue = $levelEditor::UndoSelections;
      T2DDesignResolutionXText.oldValue = $levelEditor::DesignResolutionX;
      T2DDesignResolutionYText.oldValue = $levelEditor::DesignResolutionY;
      T2DShowCameraToggle.oldValue = $levelEditor::ShowCamera;
      T2DShowGuidesToggle.oldValue = $levelEditor::ShowGuides;
                                          
      // rdbnote: it doesn't look like you can even change these values ?
      //$levelEditor::rotationSnap
      //$levelEditor::rotationSnapAngle
      //$levelEditor::rotationSnapThreshold
      
      T2DDisplayErrorsToggle.oldValue = $levelEditor::DisplayScriptErrors;
   }
};
activatePackage(OptionsHackPackage);

function updateLevelEdOptions()
{
   $levelEditor::ShowGrid = T2DLVLShowGrid.getValue();
   $levelEditor::SnapX = T2DLVLGridCheckX.getValue();
   $levelEditor::SnapY = T2DLVLGridCheckY.getValue();
   $levelEditor::GridSizeX = T2DLVLGridUnitX.getText();
   $levelEditor::GridSizeY = T2DLVLGridUnitY.getText();
   $levelEditor::BackgroundColor = T2DBGColorControl.getValue();
   $levelEditor::BackgroundColorPos = T2DBGColorControl.getSelectorPos();

   if(T2DObjectLibraryColorLight.getValue())
      $levelEditor::ObjectLibraryBackgroundColor = "Light";
   else if(T2DObjectLibraryColorMedium.getValue())
      $levelEditor::ObjectLibraryBackgroundColor = "Medium";
   else if(T2DObjectLibraryColorDark.getValue())
      $levelEditor::ObjectLibraryBackgroundColor = "Dark";

   $levelEditor::GridColor = T2DGridColorControl.getValue();
   $levelEditor::GridColorPos = T2DGridColorControl.getSelectorPos();
   $levelEditor::SnapThreshold = T2DSnapThresholdText.getText();
   $levelEditor::FullContainSelection = T2DFullContainToggle.getValue();
   $levelEditor::UndoSelections = T2DUndoSelectionsToggle.getValue();

   $levelEditor::DesignResolutionX = T2DDesignResolutionXText.getText();
   if ($levelEditor::DesignResolutionX $= "" || $levelEditor::DesignResolutionX < $levelEditor::MinDesignResolutionX)
      $levelEditor::DesignResolutionX = $levelEditor::MinDesignResolutionX;
   
   $levelEditor::DesignResolutionY = T2DDesignResolutionYText.getText();
   if ($levelEditor::DesignResolutionY $= "" || $levelEditor::DesignResolutionY < $levelEditor::MinDesignResolutionY)
      $levelEditor::DesignResolutionY = $levelEditor::MinDesignResolutionY;
      
   $levelEditor::ShowCamera = T2DShowCameraToggle.getValue();
   $levelEditor::ShowGuides = T2DShowGuidesToggle.getValue();
   $levelEditor::DisplayScriptErrors = T2DDisplayErrorsToggle.getValue();
   
   applyLevelEdOptions();
}

function applyLevelEdOptions()
{
   T2DLVLShowGrid.setValue($levelEditor::ShowGrid);
   T2DLVLGridCheckX.setValue($levelEditor::SnapX);
   T2DLVLGridCheckY.setValue($levelEditor::SnapY);
   T2DLVLGridUnitX.setText($levelEditor::GridSizeX);
   T2DLVLGridUnitY.setText($levelEditor::GridSizeY);
   T2DBGColorControl.setSelectorPos($levelEditor::BackgroundColorPos);
   T2DGridColorControl.setSelectorPos($levelEditor::GridColorPos);
   T2DBGColorControl.updateColor();
   T2DGridColorControl.updateColor();
   
   T2DObjectLibraryColorLight.setValue(false);
   T2DObjectLibraryColorMedium.setValue(false);
   T2DObjectLibraryColorDark.setValue(false);
   switch$ ($levelEditor::ObjectLibraryBackgroundColor)
   {
      case "Light":
         T2DObjectLibraryColorLight.setValue(true);
      
      case "Medium":
         T2DObjectLibraryColorMedium.setValue(true);
      
      case "Dark":
         T2DObjectLibraryColorDark.setValue(true);
   }
   
   T2DSnapThresholdText.setText($levelEditor::SnapThreshold);
   T2DFullContainToggle.setValue($levelEditor::FullContainSelection);
   T2DUndoSelectionsToggle.setValue($levelEditor::UndoSelections);
   T2DDesignResolutionXText.setText($levelEditor::DesignResolutionX);
   T2DDesignResolutionYText.setText($levelEditor::DesignResolutionY);
   T2DShowCameraToggle.setValue($levelEditor::ShowCamera);
   T2DShowGuidesToggle.setValue($levelEditor::ShowGuides);
     
   ToolManager.setGridVisibility($levelEditor::ShowGrid);

   ToolManager.setSnapToGridX($levelEditor::SnapX);
   ToolManager.setGridSizeX($levelEditor::GridSizeX);     
   ToolManager.setSnapToGridY($levelEditor::SnapY);
   ToolManager.setGridSizeY($levelEditor::GridSizeY);
   
   ToolManager.setFillColor(mFloor(getWord($levelEditor::BackgroundColor,0)*255),
                            mFloor(getWord($levelEditor::BackgroundColor,1)*255), 
                            mFloor(getWord($levelEditor::BackgroundColor,2)*255));
   
   ToolManager.setGridColor(mFloor(getWord($levelEditor::GridColor,0)*255),
                            mFloor(getWord($levelEditor::GridColor,1)*255), 
                            mFloor(getWord($levelEditor::GridColor,2)*255), 60);
   
   GuiFormManager::SendContentMessage($LBCreateSiderBar, "", "changeProfiles" SPC $levelEditor::ObjectLibraryBackgroundColor);
                            
   ToolManager.setDesignResolution($levelEditor::DesignResolutionX, $levelEditor::DesignResolutionY);
   ToolManager.setCameraVisibility($levelEditor::ShowCamera);
   ToolManager.setGuidesVisibility($levelEditor::ShowGuides);

   if(isObject(LevelEditorSelectionTool))
   {
      LevelEditorSelectionTool.setFullContainSelect($levelEditor::FullContainSelection);
      LevelEditorSelectionTool.setUndoSelections($levelEditor::UndoSelections);
   }
   
   ToolManager.setSnapThreshold($levelEditor::SnapThreshold);
   ToolManager.setRotationSnap($levelEditor::rotationSnap);
   ToolManager.setRotationSnapAngle($levelEditor::rotationSnapAngle);
   ToolManager.setRotationSnapThreshold($levelEditor::rotationSnapThreshold);
   T2DDisplayErrorsToggle.setValue($levelEditor::DisplayScriptErrors);
   
   GuiFormManager::sendContentMessage( $LBSelectionToolPropertiesContent, "", "refreshToolProperties" );
   GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspectUpdate" );
}

function revertLevelEdOptionChanges()
{
   $levelEditor::ShowGrid = T2DLVLShowGrid.oldValue;
   $levelEditor::SnapX = T2DLVLGridCheckX.oldValue;
   $levelEditor::SnapY = T2DLVLGridCheckY.oldValue;
   $levelEditor::GridSizeX = T2DLVLGridUnitX.oldValue;
   $levelEditor::GridSizeY = T2DLVLGridUnitY.oldValue;
   
   $levelEditor::BackgroundColor = T2DBGColorControl.oldValue1;
   $levelEditor::BackgroundColorPos = T2DBGColorControl.oldValue2;
   $levelEditor::GridColor = T2DGridColorControl.oldValue1;
   $levelEditor::GridColorPos = T2DGridColorControl.oldValue2;
   
   // these should be all the same value
   $levelEditor::ObjectLibraryBackgroundColor = T2DObjectLibraryColorLight.oldValue;
   //$levelEditor::ObjectLibraryBackgroundColor = T2DObjectLibraryColorMedium.oldValue;
   //$levelEditor::ObjectLibraryBackgroundColor = T2DObjectLibraryColorDark.oldValue;
   
   $levelEditor::SnapThreshold = T2DSnapThresholdText.oldValue;
   $levelEditor::FullContainSelection = T2DFullContainToggle.oldValue;
   $levelEditor::UndoSelections = T2DUndoSelectionsToggle.oldValue;
   $levelEditor::DesignResolutionX = T2DDesignResolutionXText.oldValue;
   $levelEditor::DesignResolutionY = T2DDesignResolutionYText.oldValue;
   $levelEditor::ShowCamera = T2DShowCameraToggle.oldValue;
   $levelEditor::ShowGuides = T2DShowGuidesToggle.oldValue;
                                       
   // rdbnote: it doesn't look like you can even change these values ?
   //$levelEditor::rotationSnap
   //$levelEditor::rotationSnapAngle
   //$levelEditor::rotationSnapThreshold
   
   $levelEditor::DisplayScriptErrors = T2DDisplayErrorsToggle.oldValue;
   
   // call this to update prefs before closing the options dialog
   applyLevelEdOptions();
}


//---------------------------------------------------------------------------------------------


function Scene::setDesignResolutionX( %this, %resX )
{
   if( %resX $= "" || %resX < $levelEditor::MinDesignResolutionX )
      %resX = $levelEditor::MinDesignResolutionX;
      
   $levelEditor::DesignResolutionX = mFloatLength( %resX, 0 );
   applyLevelEdOptions();
}

function Scene::setDesignResolutionY( %this, %resY )
{
   // Small Sanity
   if( %resY $= "" || %resY < $levelEditor::MinDesignResolutionY )
      %resY = $levelEditor::MinDesignResolutionY;
      
   $levelEditor::DesignResolutionY = mFloatLength( %resY, 0 );
   applyLevelEdOptions();
}

function Scene::getDesignResolutionX( %this )
{
   return $levelEditor::DesignResolutionX;
}

function Scene::getDesignResolutionY( %this )
{
   return $levelEditor::DesignResolutionY;
}

