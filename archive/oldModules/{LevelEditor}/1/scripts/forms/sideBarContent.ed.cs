//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
$LBSideBarContent = GuiFormManager::AddFormContent( "LevelBuilder", "SideBar", "LBSideBarContent::CreateForm", "LBSideBarContent::SaveForm", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBSideBarContent::CreateForm( %formCtrl )
{
   %base = new GuiScriptNotifyCtrl()
   {
      class = "sideBarContentContainer";
      onChildAdded = "1";
      onChildRemoved = "1";
      onChildResized = "1";
      onParentResized = "1";
      onResize = "1";
      onLoseFirstResponder = "0";
      onGainFirstResponder = "0";
      canSaveDynamicFields = "0";
      Profile = "GuiTransparentProfile";
      HorizSizing = "width";
      VertSizing = "height";
      position = "0 0";
      Extent = "400 500";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";     
   };
   %book = new GuiTabBookCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorTabBook";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = "400 500";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      internalName = "SideBarTabBook";
      TabPosition = "Top";
      TabHeight = "25";
      TabWidth = "80";
   };
   %base.add(%book);
  
   %formCtrl.add(%base);

   if( %formCtrl.isMethod( "sizeContentsToFit" ) )
      %formCtrl.sizeContentsToFit(%base, %formCtrl.contentID.margin);


   %count = GuiFormManager::GetFormContentCount( "LevelBuilderSidebar" );

   if( %count > 0 )
   {
      for( %i = 0; %i < %count; %i++ )
      {
         %contentObj = GuiFormManager::GetFormContentByIndex( "LevelBuilderSidebar", %i );
         if( !isObject( %contentObj ) )
            continue;

         if( %contentObj.CreateFunction !$= "" )
         {
            %result = eval( %contentObj.CreateFunction @ "(" @ %book @ ");" );
            if( isObject( %result ) )
               GuiFormManager::AddContentReference( "LevelBuilderSidebar", %contentObj.Name, %result );               
         }
      }
   }
   else
      error("LevelBuilderSideBar - No Sidebar Content Found!");



   // Return back the base control to indicate we were successful
   return %base;
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBSideBarContent::SaveForm( %formCtrl )
{
   // Nothing.
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function sideBarContentContainer::onContentMessage( %this, %sender, %message )
{
   %command = getWord(%message, 0);
   %value = getWord(%message, 1);
   switch$ (%command)
   {
      case "setTabPage":
         %tabBook = %this.findObjectByInternalName("SideBarTabBook");
         if (!isObject(%tabBook))
            return;
         // Select by value            
         %tabBook.selectPageName(%value);            
   }
}

function sideBarContentContainer::sizeContentsToFit(%this, %content, %margin)
{
   %formext = %this.getExtent();
   
   %ctrlposx = 0;
   %ctrlposy = 0;
   %ctrlextx = getWord(%formext,0);
   %ctrlexty = getWord(%formext,1);
   
   %content.resize(%ctrlposx,%ctrlposy,%ctrlextx,%ctrlexty);
}

function sideBarContentContainer::onResize(%this)
{
   if( %this.getCount() > 0 && isObject( %this.getObject(0) ) )
      %this.sizeContentsToFit( %this.getObject(0) );
      
   GuiFormManager::SendContentMessage($LBCreateSiderBar, %this, "refresh");      
}

function sideBarContentContainer::onParentResized(%this)
{
   if( %this.getCount() > 0 && isObject( %this.getObject(0) ) )
      %this.sizeContentsToFit( %this.getObject(0) );
      
   GuiFormManager::SendContentMessage($LBCreateSiderBar, %this, "refresh");      
}
