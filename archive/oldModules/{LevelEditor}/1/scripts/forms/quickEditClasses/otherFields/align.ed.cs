//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function AlignTools::getText( %dir, %side )
{
   %dirText = %dir == 0 ? "H" : "V";
   if( %dir == 2 ) %dirText = "B";
   %sideText = "C";
   if( %dir == 0 )
   {
      if( %side == -1 ) %sideText = "L";
      else if( %side == 1 ) %sideText = "R";
   }
   else if( %dir == 1 )
   {
      if( %side == -1 ) %sideText = "T";
      else if( %side == 1 ) %sideText = "B";
   }
   
   return %dirText @ %sideText;
}

function LBQuickEditContent::createAlignTools(%this, %singleObject)
{
   %container = new GuiControl()
   {
      Profile = "EditorContainerProfile";
      Position = "0 0";
      Extent = "300 270";
   };
      
   %array = new GuiDynamicCtrlArrayControl()
   {
      Profile = "EditorRolloutProfile";
      position = "112 18";
      Extent = "102 185";
      colCount = "3";
      colSize = "30";
      rowSize = "30";
      rowSpacing = "6";
      colSpacing = "6";
   };

   %labelPos = 24;
   %alignLabel = new GuiTextCtrl() {
      canSaveDynamicFields = "0";
      Profile = "EditorFontHLBold";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "16" SPC %labelPos;
      Extent = "128 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      text = "Align";
      maxLength = "1024";
   };
   %labelPos += 72;
   
   // Create align buttons.
   for( %dir = 0; %dir <= 1; %dir++ )
   {
      for( %side = -1; %side <= 1; %side++ )
      {
         %text = AlignTools::getText( %dir, %side );
         %button = new GuiIconButtonCtrl()
         {
            Profile = "EditorButton";
            Extent = "30 30";
            buttonMargin = "0 0";
            sizeIconToButton = "1";
            class = "AlignButton";
            side = %side;
            dir = %dir;
            iconBitmap = "^{EditorAssets}/gui/iconAlign" @ %text;
         };
         
         %array.add( %button );
      }
   }

   if( !%singleObject )
   {
      %distributeLabel = new GuiTextCtrl() {
         canSaveDynamicFields = "0";
         Profile = "EditorFontHLBold";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "16" SPC %labelPos;
         Extent = "128 18";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "100";
         tooltip = %tooltip;
         tooltipProfile = "EditorToolTipProfile";
         text = "Distribute";
         maxLength = "1024";
      };
      %labelPos += 72;
      
      // Create distribute buttons.
      for( %dir = 0; %dir <= 1; %dir++ )
      {
         for( %side = -1; %side <= 1; %side++ )
         {
            %text = AlignTools::getText( %dir, %side );
            %button = new GuiIconButtonCtrl()
            {
               Profile = "EditorButton";
               Extent = "30 30";
               buttonMargin = "0 0";
               sizeIconToButton = "1";
               class = "DistributeButton";
               side = %side;
               dir = %dir;
               iconBitmap = "^{EditorAssets}/gui/iconDistribute" @ %text;
            };
            
            %array.add( %button );
         }
      }
   }

   %sizeLabel = new GuiTextCtrl() {
      canSaveDynamicFields = "0";
      Profile = "EditorFontHLBold";
      HorizSizing = "right";
      VertSizing = "bottom";
      Position = "16" SPC %labelPos;
      Extent = "128 18";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "100";
      tooltip = %tooltip;
      tooltipProfile = "EditorToolTipProfile";
      text = "Match Size";
      maxLength = "1024";
   };
   %labelPos += 36;
   
   // Create size buttons.
   for( %dir = 0; %dir <= 2; %dir++ )
   {
      %text = AlignTools::getText( %dir );
      %button = new GuiIconButtonCtrl()
      {
         Profile = "EditorButton";
         Extent = "30 30";
         buttonMargin = "0 0";
         sizeIconToButton = "1";
         class = "MatchSizeButton";
         dir = %dir;
         iconBitmap = "^{EditorAssets}/gui/iconMatchSize" @ %text;
      };
      
      %array.add( %button );
   }

   if( !%singleObject )
   {
      %spaceLabel = new GuiTextCtrl() {
         canSaveDynamicFields = "0";
         Profile = "EditorFontHLBold";
         HorizSizing = "right";
         VertSizing = "bottom";
         Position = "16" SPC %labelPos;
         Extent = "128 18";
         MinExtent = "8 2";
         canSave = "1";
         Visible = "1";
         hovertime = "100";
         tooltip = %tooltip;
         tooltipProfile = "EditorToolTipProfile";
         text = "Space";
         maxLength = "1024";
      };
      %labelPos += 36;
      
      // Create space buttons.
      for( %dir = 0; %dir <= 1; %dir++ )
      {
         %text = AlignTools::getText( %dir );
         %button = new GuiIconButtonCtrl()
         {
            Profile = "EditorButton";
            Extent = "30 30";
            buttonMargin = "0 0";
            sizeIconToButton = "1";
            class = "SpaceButton";
            dir = %dir;
            iconBitmap = "^{EditorAssets}/gui/iconSpace" @ %text;
         };
         
         %array.add( %button );
      }
      
      // Camera Checkbox
      %checkbox = new GuiCheckBoxCtrl()
      {
         Profile = "EditorCheckBox";
         class = "AlignToCameraCheckBox";
         text = "Align To Camera";
         Position = "16" SPC %labelPos;
         Extent = "100 20";
      };
      %labelPos += 26;
      
      %container.add( %distributeLabel );
      %container.add( %spaceLabel );
      %container.add( %checkbox );
   }
   
   %container.extent = "300" SPC %labelPos;
   
   %container.add( %alignLabel );
   %container.add( %sizeLabel );
   %container.add( %array );
   %this.add( %container );
   return %container;
}

function AlignToCameraCheckBox::onClick( %this )
{
   $AlignTools::alignToCamera = %this.getValue();
}

function AlignButton::onClick( %this )
{
   %objects = ToolManager.getAcquiredObjects();
   %undo = ToolManager.createUndo( %objects, "position", "Align Objects" );
   
   %toCamera = $AlignTools::alignToCamera;
   if( %objects.getCount() < 2 )
      %toCamera = true;
   
   if( %toCamera )
      AlignTools::alignToCamera( %objects, ToolManager.getLastWindow().getScene(), %this.side, %this.dir );
   else
      AlignTools::align( %objects, %this.side, %this.dir );
      
   ToolManager.updateAcquiredObjectSet();
   GuiFormManager::BroadcastContentMessage( "LevelBuilderQuickEditClasses", %this, "updateExisting" );
   
   %undo.finish();
}

function DistributeButton::onClick( %this )
{
   %objects = ToolManager.getAcquiredObjects();
   AlignTools::sort( %objects, %this.side, %this.dir );
   %undo = ToolManager.createUndo( %objects, "position", "Align Objects" );
   
   %toCamera = $AlignTools::alignToCamera;
   if( %objects.getCount() < 2 )
      %toCamera = true;
   
   if( %toCamera )
      AlignTools::distributeToCamera( %objects, ToolManager.getLastWindow().getScene(), %this.side, %this.dir );
   else
      AlignTools::distribute( %objects, %this.side, %this.dir );
   
   AlignTools::deleteSorted( %objects );   
   ToolManager.updateAcquiredObjectSet();
   GuiFormManager::BroadcastContentMessage( "LevelBuilderQuickEditClasses", %this, "updateExisting" );
   
   %undo.finish();
}

function MatchSizeButton::onClick( %this )
{
   %objects = ToolManager.getAcquiredObjects();
   %undo = ToolManager.createUndo( %objects, "size", "Match Objects Size" );
   
   %toCamera = $AlignTools::alignToCamera;
   if( %objects.getCount() < 2 )
      %toCamera = true;
   
   if( %toCamera )
      AlignTools::matchSizeToCamera( %objects, ToolManager.getLastWindow().getScene(), %this.dir );
   else
      AlignTools::matchSize( %objects, %this.dir );
   
   ToolManager.updateAcquiredObjectSet();
   GuiFormManager::BroadcastContentMessage( "LevelBuilderQuickEditClasses", %this, "updateExisting" );
   
   %undo.finish();
}

function SpaceButton::onClick( %this )
{
   %objects = ToolManager.getAcquiredObjects();
   AlignTools::sort( %objects, 0, %this.dir );
   %undo = ToolManager.createUndo( %objects, "position", "Align Objects" );
   
   %toCamera = $AlignTools::alignToCamera;
   if( %objects.getCount() < 2 )
      %toCamera = true;
   
   if( %toCamera )
      AlignTools::spaceToCamera( %objects, ToolManager.getLastWindow().getScene(), %this.dir );
   else
      AlignTools::space( %objects, %this.dir );
   
   AlignTools::deleteSorted( %objects );   
   ToolManager.updateAcquiredObjectSet();
   GuiFormManager::BroadcastContentMessage( "LevelBuilderQuickEditClasses", %this, "updateExisting" );
   
   %undo.finish();
}
