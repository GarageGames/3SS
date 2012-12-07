//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
$LBEditSidebarContent = GuiFormManager::AddFormContent( "LevelBuilderSidebar", "Edit", "LBProjectSideBar::CreateContent", "LBEditSideBar::SaveContent", 2 );


//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBProjectSideBar::CreateContent( %contentCtrl )
{    

   %base = new GuiTabPageCtrl() 
   {
      Text = "Project";
      internalName = "SideBarProjectPage";
      canSaveDynamicFields = "0";
      Profile = "EditorTabPage";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = "500 400";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
   };
   
   %scroll = new GuiScrollCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorTransparentScrollProfile";
      HorizSizing = "width";
      VertSizing = "height";
      position = "0 0";
      Extent = "500 400";
      MinExtent = "72 64";
      canSave = "1";
      visible = "1";
      hovertime = "1000";
      willFirstRespond = "1";
      hScrollBar = "alwaysOff";
      vScrollBar = "alwaysOn";
      constantThumbHeight = "1";
      childMargin = "0 4";
   };
   %base.add(%scroll);
   
   %stack = new GuiStackControl() 
   {
      StackingType = "Vertical";
      HorizStacking = "Left to Right";
      VertStacking = "Top to Bottom";
      Padding = "0";
      canSaveDynamicFields = "0";
      internalName = "editStack";
      class = "LBSideBarCreateMessaging";
      Profile = "EditorTransparentProfile";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = "480 400";
      MinExtent = "24 24";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
   };
   %scroll.add( %stack );
   
   // Add Base to Form
   %contentCtrl.add( %base );
   
   // Load dynamic edit bar panels
   %count = GuiFormManager::GetFormContentCount( "LevelBuilderSidebarEdit" );
   for( %i = 0; %i < %count; %i++ )
   {
      %contentObj = GuiFormManager::GetFormContentByIndex( "LevelBuilderSidebarEdit", %i );
      if( !isObject( %contentObj ) )
         continue;

      if( %contentObj.CreateFunction !$= "" )
      {
         %result = eval( %contentObj.CreateFunction @ "(" @ %stack @ ");" );
         if( isObject( %result ) )
            GuiFormManager::AddContentReference( "LevelBuilderSidebarEdit", %contentObj.Name, %result );               
      }
   }
   
   return %base;

}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBProjectSideBar::SaveContent( %contentCtrl, %contentObj )
{
   // Nothing Now.
}