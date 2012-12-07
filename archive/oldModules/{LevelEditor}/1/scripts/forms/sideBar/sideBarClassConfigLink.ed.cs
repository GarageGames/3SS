//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
GuiFormManager::AddFormContent( "LevelBuilder", "ClassConfigLink", "LBClassNamespaceLink::CreateForm", "", 2 );
GuiFormManager::AddFormContent( "LevelBuilder", "SuperClassConfigLink", "LBSuperClassNamespaceLink::CreateForm", "", 2 );


//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBClassConfigLink::CreateForm( %formCtrl )
{    
   %base = new GuiControl() 
   {
      class = "LevelBuilderClassConfigLink";
      canSaveDynamicFields = "0";
      Profile = "EditorPanelLight";
      HorizSizing = "width";
      VertSizing = "height";
      position = "0 0";
      Extent = "300 25";
      MinExtent = "150 25";
      canSave = "1";
      visible = "1";
      internalName = "ClassConfigLink";
      tooltipprofile = "GuiToolTipProfile";
      tooltip = "This Panel Links Script Classes to Objects";
      hovertime = "1000";
   };
   %formCtrl.add(%base);
   
   %classMenu = new GuiPopUpMenuCtrlEx() 
   {
      canSaveDynamicFields = "0";
      Profile = "GuiPopupMenuProfile";
      internalName = "ClassList";
      class = "LevelBuilderScriptClassLinkMenu";
      HorizSizing = "width";
      VertSizing = "bottom";
      position = "95 3";
      Extent = "95 19";
      MinExtent = "8 2";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      maxLength = "1024";
      maxPopupHeight = "200";
   };
   
   %base.add( %classMenu );
   %classCaption = new GuiTextCtrl() 
   {
      canSaveDynamicFields = "0";
      Profile = "EditorTextHLBoldRight";
      internalName = "Caption";
      HorizSizing = "right";
      VertSizing = "bottom";
      position = "10 3";
      Extent = "65 19";
      MinExtent = "8 2";
      justify = "right";
      autoSizeWidth = false;
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      text = "Class";
      maxLength = "1024";
   };
   %base.add( %classCaption );
   
   
   // Resize as appropriate.
   if( %formCtrl.isMethod("sizeContentsToFit") )
      %formctrl.sizeContentsToFit(%base, %formCtrl.contentID.margin);

   //*** Return back the base control to indicate we were successful
   return %base;

}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBClassConfigLink::SaveForm( %formCtrl, %contentObj )
{
}

//-----------------------------------------------------------------------------
// Derived Class/SuperClass content creation
//-----------------------------------------------------------------------------
function LBClassNamespaceLink::CreateForm( %formCtrl )
{
   // Let Parent do common work.
   %base = LBClassConfigLink::CreateForm( %formCtrl );
   
   // Tag list to link to SuperClass
   %list = %base.findObjectByInternalName("ClassList");
   %list.linkType = "Class";
   
   // Return
   return %base;
}

function LBSuperClassNamespaceLink::CreateForm( %formCtrl )
{
   // Let Parent do common work.
   %base = LBClassConfigLink::CreateForm( %formCtrl );
   
   // Change caption to say SuperClass
   %caption = %base.findObjectByInternalName("Caption");
   %caption.setText("SuperClass");
   
   // Tag list to link to SuperClass
   %list = %base.findObjectByInternalName("ClassList");
   %list.linkType = "SuperClass";
   
   // Return
   return %base;
   
}


//-----------------------------------------------------------------------------
// Form Content Messaging Function
//-----------------------------------------------------------------------------
function LevelBuilderClassConfigLink::onContentMessage( %this, %sender, %message )
{

   %command = GetWord( %message, 0 );
   %value   = GetWord( %message, 1 );

   switch$( %command )
   {
      case "update":
         %list = %this.findObjectByInternalName( "ClassList" );
         %list.updateClassList();
   }
}

//
//
//
function LevelBuilderScriptClassLinkMenu::onSelect(%this, %id)
{
   // Short circuit if there are no classes in list.  
   if( %id == -2 )
      %text = "";
   else      
      %text = %this.getTextById(%id);
   
   // Store Selection  
   if( %this.linkType $= "SuperClass" )
      LevelBuilderToolManager::updateSuperClassLink( %text );
   else if( %this.linkType $= "Class" )
      LevelBuilderToolManager::updateClassLink( %text );
   
}

function LevelBuilderScriptClassLinkMenu::onWake( %this )
{
   %this.updateClassList();   
}


function LevelBuilderScriptClassLinkMenu::updateClassList( %this )
{
   %this.clear();
   %this.add("None", -2);
   
   %classList = ScriptClassManager::GetScriptClassList( "TGB" );
   if( !isObject( %classList ) )
      return;
      
   %count = %classList.getCount();
   %currId = 0;
   
   for (%i = 0; %i < %count; %i++)
   {
      %class = %classList.getObject(%i);
      %this.add(%class.className, %id++);
   }
             
   // Sort the list.
   %this.sort();  
}
