//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
$LBTreeViewContent = GuiFormManager::AddFormContent( "LevelBuilderSidebarEdit", "Scene TreeView", "LBSceneTreeView::CreateForm", "LBSceneTreeView::SaveForm", 2 );

function ProjectiDevicePanelDeviceType::onChanged(%this)
{
   //Update the other settings now
   ProjectiDevicePanelScreenOrientation.onChanged();   
}

function loadiOSSettings()
{
   if(isObject(ProjectiFeaturePanel))
   {
      //Add the needed options to the gui on startup
      if(isObject(ProjectiFeaturePanelStatusBarType))
      {
         // Torque 2D does not have true, GameKit networking support. 
         // The old socket network code is untested, undocumented and likely broken. 
         // This will eventually be replaced with GameKit. 
         // For now, it is confusing to even have a checkbox in the editor that no one uses or understands. 
         // If you are one of the few that uses this, uncomment the next line. -MP 1.5
         //ProjectiFeaturePanelEnableNetwork.setValue($pref::iOS::UseNetwork);
          
          ProjectiFeaturePanelEnableMusic.setValue($pref::iOS::UseMusic);
          ProjectiFeaturePanelEnableMoviePlayer.setValue($pref::iOS::UseMoviePlayer);
          ProjectiFeaturePanelEnableAutorotate.setValue($pref::iOS::EnableOtherOrientationRotation);
          ProjectiFeaturePanelEnableOrientation.setValue($pref::iOS::EnableOrientationRotation);
          ProjectiFeaturePanelEnableGameCenter.setValue($pref::iOS::UseGameKit);
         
         ProjectiFeaturePanelStatusBarType.clear();
         ProjectiFeaturePanelStatusBarType.add("Hidden", 0);
         ProjectiFeaturePanelStatusBarType.add("Black Opaque", 1);         
         ProjectiFeaturePanelStatusBarType.add("Black Translucent", 2);
         
         ProjectiFeaturePanelStatusBarType.setSelected($pref::iOS::StatusBarType);
      }
   }
   
   if(isObject(ProjectiDevicePanelDeviceType))
      ProjectiDevicePanelDeviceType.setSelected($pref::iOS::DeviceType);
}

function ProjectiDevicePanelScreenOrientation::onChanged(%this)
{
   %val = ProjectiDevicePanelScreenOrientation.getSelected();
   
   if(%val == $iOS::constant::Landscape)
   {
      ProjectiFeaturePanelEnableAutorotate.setText("Allow rotate to portrait");
   }
   else
   {
      ProjectiFeaturePanelEnableAutorotate.setText("Allow rotate to landscape");
   }
}

function ProjectiFeaturePanelEnableAutorotate::onClick(%this)
{
   //ProjectiFeaturePanelEnableOrientation.setValue(%this.getValue());
}

function ProjectiFeaturePanelEnableOrientation::onClick(%this)
{
   if(%this.getValue())
   {
      ProjectiFeaturePanelEnableAutorotate.setActive(1);
   }
   else
   {
      ProjectiFeaturePanelEnableAutorotate.setValue(0);
      ProjectiFeaturePanelEnableAutorotate.setActive(0);
   }
}

function ProjectiDeviceSettings::setiOSOptions()
{
   %lastWindow = ToolManager.getLastWindow();
   %scene = %lastWindow.getScene();
   %count = %scene.getSceneObjectCount();
   
   //This manages build configuration
                  
   //New values include Device Type, Device Screen Orientation and Device Resolution if it applies.
   $pref::iOS::DeviceType          = ProjectiDevicePanelDeviceType.getSelected();
   $pref::iOS::ScreenOrientation   = ProjectiDevicePanelScreenOrientation.getSelected();
   $pref::iOS::EnableOrientationRotation  = ProjectiFeaturePanelEnableOrientation.getValue();
   $pref::iOS::EnableOtherOrientationRotation   = ProjectiFeaturePanelEnableAutorotate.getValue();
      
   //Save the new settings to the xml comfig
   //%fname = expandPath("^project/common/commonConfig.xml");
   echo("Saving file for iOS Build settings!");// @ %fname);
   _saveGameConfigurationData();
         
   echo("iOS settings applied!");
}

function ProjectiFeatureSettings::setiFeatureOptions()
{
   $pref::iOS::UseMusic        = ProjectiFeaturePanelEnableMusic.getValue();
   $pref::iOS::UseMoviePlayer  = ProjectiFeaturePanelEnableMoviePlayer.getValue();
   $pref::iOS::UseGameKit      = ProjectiFeaturePanelEnableGameCenter.getValue();
   $pref::iOS::StatusBarType   = ProjectiFeaturePanelStatusBarType.getSelected();
   
   // Torque 2D does not have true, GameKit networking support. 
   // The old socket network code is untested, undocumented and likely broken. 
   // This will eventually be replaced with GameKit. 
   // For now, it is confusing to even have a checkbox in the editor that no one uses or understands. 
   // If you are one of the few that uses this, uncomment the next line. -MP 1.5 
   //$pref::iOS::UseNetwork      = ProjectiFeaturePanelEnableNetwork.getValue();
         
   echo("Saving file for iDevice Feature settings!");
   
   _saveGameConfigurationData();
   
   echo("iOS Feature settings applied!");
}

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBSceneTreeView::CreateForm( %formCtrl )
{   
   
   //Project manager base container rollout.
   %projectBase = new GuiRolloutCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorRolloutProfile";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = "323 260";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      Caption = "Project manager";
      Margin = "6 4";
      DragSizable = true;
      DefaultHeight = "260";
};
   
   //Project manager scroll container.
   %projScroll = new GuiScrollCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorTransparentScrollProfile";
      HorizSizing = "width";
      VertSizing = "bottom";
      position = "0 0";
      Extent = "214 260";
      MinExtent = "72 260";
      canSave = "1";
      visible = "1";
      hovertime = "1000";
      willFirstRespond = "1";
      hScrollBar = "dynamic";
      vScrollBar = "alwaysOn";
      constantThumbHeight = "0";
      childMargin = "0 0";
   };
   %projectBase.add(%projScroll);
   
   //iDevice Settings base container rollout.
   %iDeviceBase = new GuiRolloutCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorRolloutProfile";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = "186 235";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      Caption = "iOS Settings";
      Margin = "6 4";
      DragSizable = true;
      DefaultHeight = "255";
   };
   
   //iDevice Settings scroll container.
   %iDeviceScroll = new GuiScrollCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorTransparentScrollProfile";
      HorizSizing = "width";
      VertSizing = "bottom";
      position = "0 0";
      Extent = "184 235";
      MinExtent = "72 235";
      canSave = "1";
      visible = "1";
      hovertime = "1000";
      willFirstRespond = "1";
      hScrollBar = "dynamic";
      vScrollBar = "alwaysOn";
      constantThumbHeight = "0";
      childMargin = "0 0";
   };
   %iDeviceBase.add(%iDeviceScroll);
   
   //iFeature Builder Settings base container rollout.
   %iDeviceFeatureBase = new GuiRolloutCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorRolloutProfile";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = "186 295";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      Caption = "iOS Feature Builder";
      Margin = "6 4";
      DragSizable = true;
      DefaultHeight = "300";
   };
   
   //iFeature Builder Settings scroll container.
   %iDeviceFeatureScroll = new GuiScrollCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorTransparentScrollProfile";
      HorizSizing = "width";
      VertSizing = "bottom";
      position = "0 0";
      Extent = "150 325";
      MinExtent = "72 325";
      canSave = "1";
      visible = "1";
      hovertime = "1000";
      willFirstRespond = "1";
      //hScrollBar = "dynamic";
      //vScrollBar = "alwaysOn";
      constantThumbHeight = "0";
      childMargin = "0 0";
   };
   %iDeviceFeatureBase.add(%iDeviceFeatureScroll);
   
   %base = new GuiRolloutCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorRolloutProfile";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = "323 231";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      Caption = "Object Tree";
      Margin = "6 4";
      DragSizable = true;
      DefaultHeight = "280";
   };

   %scroll = new GuiScrollCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorTransparentScrollProfile";
      HorizSizing = "width";
      VertSizing = "bottom";
      position = "0 0";
      Extent = "323 231";
      MinExtent = "72 400";
      canSave = "1";
      visible = "1";
      hovertime = "1000";
      willFirstRespond = "1";
      hScrollBar = "dynamic";
      vScrollBar = "alwaysOn";
      constantThumbHeight = "0";
      childMargin = "0 0";
   };
   %base.add(%scroll);

   %treeObj = new GuiTreeViewCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorTreeViewProfile";
      class = "LevelBuilderSceneTreeView";
      HorizSizing = "width";
      VertSizing = "height";
      position = "2 2";
      Extent = "171 21";
      MinExtent = "8 2";
      canSave = "1";
      visible = "1";
      hovertime = "1000";
      tabSize = "16";
      textOffset = "2";
      fullRowSelect = "1";
      itemHeight = "21";
      destroyTreeOnSleep = "1";
      MouseDragging = "0";
      MultipleSelections = "1";
      DeleteObjectAllowed = "0";
      DragToItemAllowed = "0";
      scroll = %scroll;
      base = %base;
      owner = %formCtrl;     
   };
   
   $mainSceneTree = %treeObj;
   
   //Load the project manager gui  
   exec("./gui/projectManPanel.gui");
   exec("./iDeviceTools/ProjectiDevicePanel.gui");
   exec("./iDeviceTools/ProjectiFeaturePanel.gui");
   
   if(isObject(ProjectManPanel))
   {
      %projScroll.add(ProjectManPanel);
   }
   
   if(isObject(ProjectiDevicePanel))
   {
      //First add the options to the drop down
      if(isObject(ProjectiDevicePanelDeviceType))
      {
         ProjectiDevicePanelDeviceType.add("iPhone / iPod Touch", $iOS::constant::iPhone);
         ProjectiDevicePanelDeviceType.add("iPad", $iOS::constant::iPad);
      }
         
      if(isObject(ProjectiDevicePanelScreenOrientation))
      {
         ProjectiDevicePanelScreenOrientation.add("Landscape", $iOS::constant::Landscape);
         ProjectiDevicePanelScreenOrientation.add("Portrait", $iOS::constant::Portrait);
      }
         
      //The apply the current setting, index based so text can change later
      ProjectiDevicePanelDeviceType.setSelected($pref::iOS::DeviceType);
      ProjectiDevicePanelScreenOrientation.setSelected($pref::iOS::ScreenOrientation);
      
      ProjectiDevicePanelDeviceType.onChanged();
      ProjectiDevicePanelScreenOrientation.onChanged();
      
      %iDeviceScroll.add(ProjectiDevicePanel);
   }
   
   if(isObject(ProjectiFeaturePanel))
   {
      
      //Add the needed options to the gui on startup
      if(isObject(ProjectiFeaturePanelStatusBarType))
      {
         ProjectiFeaturePanelEnableMusic.setValue($pref::iOS::UseMusic);
         ProjectiFeaturePanelEnableMoviePlayer.setValue($pref::iOS::UseMoviePlayer);
         ProjectiFeaturePanelEnableAutorotate.setValue($pref::iOS::EnableOtherOrientationRotation);
         ProjectiFeaturePanelEnableOrientation.setValue($pref::iOS::EnableOrientationRotation);
         ProjectiFeaturePanelEnableGameCenter.setValue($pref::iOS::UseGameKit);
         
         ProjectiFeaturePanelStatusBarType.clear();
         ProjectiFeaturePanelStatusBarType.add("Hidden", 0);
         ProjectiFeaturePanelStatusBarType.add("Black Opaque", 1);         
         ProjectiFeaturePanelStatusBarType.add("Black Translucent", 2);
         
         ProjectiFeaturePanelStatusBarType.setSelected($pref::iOS::StatusBarType);
         
         // Torque 2D does not have true, GameKit networking support. 
         // The old socket network code is untested, undocumented and likely broken. 
         // This will eventually be replaced with GameKit. 
         // For now, it is confusing to even have a checkbox in the editor that no one uses or understands. 
         // If you are one of the few that uses this, uncomment the next line. -MP 1.5 
          //ProjectiFeaturePanelEnableNetwork.setValue($pref::iOS::UseNetwork);       
      }      

      %iDeviceFeatureScroll.add(ProjectiFeaturePanel);
   }
   
   %scroll.add( %treeObj );
   %formCtrl.add( %projectBase );
   %formCtrl.add( %iDeviceBase );
   %formCtrl.add( %iDeviceFeatureBase );
   %formCtrl.add( %base );

   // Open the Current Scene, if any.
   %lastWindow = ToolManager.getLastWindow();
   if( isObject( %lastWindow ) && isObject( %lastWindow.getScene() ) )
      %treeObj.open( %lastWindow.getScene() );


   // Specify Message Control (Override getObject(0) on new Content which is default message control)
   %base.MessageControl = %treeObj;

   //*** Return back the base control to indicate we were successful
   return %base;

}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBSceneTreeView::SaveForm( %formCtrl )
{
   // Nothing.
}

//-----------------------------------------------------------------------------
// Form Content Functionality
//-----------------------------------------------------------------------------
function LevelBuilderSceneTreeView::onContentMessage( %this, %sender, %message )
{

   %command = GetWord( %message, 0 );
   %value   = GetWord( %message, 1 );

   switch$( %command )
   {
      case "openCurrentGraph":
         // Open the Current Scene, if any.
         %lastWindow = ToolManager.getLastWindow();
         if( isObject( %lastWindow ) && isObject( %lastWindow.getScene() ) )
         {
            %this.open( %lastWindow.getScene() );
            %this.expandItem(1, true);
         }
      case "openObject":
         %this.open( %value );
         
      case "clearSelection":
         %item = %this.findItemByObjectId(%value);
         %this.removeSelection(%item);
      
      case "setSelection":
         %item = %this.findItemByObjectId(%value);
         %this.selectItem(%item);
      
      case "setSelections":
         %count = %value.getCount();
         for (%i = 0; %i < %count; %i++)
         {
            %item = %this.findItemByObjectId(%value.getObject(%i));
            %this.addSelection(%item);
         }
   }
   
   //ITGB-44 - Leaving line here for reference though.
   //%this.scroll.scrollToBottom();
   %this.base.sizeToContents();
   %this.owner.updateStack();
   
}


function LevelBuilderSceneTreeView::onWake( %this )
{
   // Open the Current Scene, if any.
   %lastWindow = ToolManager.getLastWindow();
   if( isObject( %lastWindow ) && isObject( %lastWindow.getScene() ) )
      %this.open( %lastWindow.getScene() );
}

function LevelBuilderSceneTreeView::onSelect(%this, %object)
{
   if (ToolManager.isAcquired(%object))
      return;
      
   ToolManager.clearAcquisition();

   %lastWindow = ToolManager.getLastWindow();  
   // Tell the Inspector to Inspect the Scene
   if ( isObject( %lastWindow ) && %object == %lastWindow.getScene())
   {
      GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspect" SPC %object );
   }
   else 
   {
      ToolManager.acquireObject(%object);
      GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspect" SPC %object );
   }
}

function LevelBuilderSceneTreeView::onUnSelect(%this, %object)
{
   if (!ToolManager.isAcquired(%object))
      return;
      
   ToolManager.clearAcquisition(%object);
}

function LevelBuilderSceneTreeView::onAddSelection(%this, %object)
{
   if (ToolManager.isAcquired(%object))
      return;
      
   ToolManager.acquireObject(%object);
}

function LevelBuilderSceneTreeView::onRemoveSelection(%this, %object)
{
   if (!ToolManager.isAcquired(%object))
      return;
      
   ToolManager.clearAcquisition(%object);
}

function LevelBuilderSceneTreeView::onClearSelection(%this)
{
   // if we pass nothing in, it will clear all objects
   ToolManager.clearAcquisition();
}
