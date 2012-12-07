//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$QuickEditFloatPrecision = 3;

//-----------------------------------------------------------------------------
// Register Form Content.
//-----------------------------------------------------------------------------
$LBQuickEdit = GuiFormManager::AddFormContent( "LevelBuilderSidebar", "Quick Edit", "LBQuickEdit::CreateContent", "LBQuickEdit::SaveContent", 2 );

//-----------------------------------------------------------------------------
// Form Content Creation Function
//-----------------------------------------------------------------------------
function LBQuickEdit::CreateContent( %contentCtrl )
{ 
   %extent = %contentCtrl.getExtent();
   %extentX = GetWord( %extent, 0 );
   %extentY = GetWord( %extent, 1 );
   
   %base = new GuiTabPageCtrl() 
   {
      Text = "Edit";
      internalName = "SideBarEditPage";
      canSaveDynamicFields = "0";
      Profile = "EditorTabPage";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = %extent;
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
      Extent = %extent;
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
   %stack = new GuiStackControl() 
   {
      StackingType = "Vertical";
      HorizStacking = "Left to Right";
      VertStacking = "Top to Bottom";
      Padding = "0";
      canSaveDynamicFields = "0";
      class = "LBQuickEditClass";
      internalName = "editStack";
      Profile = "EditorTransparentProfile";
      HorizSizing = "width";
      VertSizing = "height";
      Position = "0 0";
      Extent = (%extentX - 20) SPC %extentY;
      MinExtent = "24 24";
      canSave = "1";
      Visible = "1";
      hovertime = "1000";
      scrollCtrl = %scroll;

   };
   %scroll.add( %stack );
   %base.add(%scroll);

   // This set will cache all form content so it is not rebuilt on every inspect.
   %stack.formCache = new SimSet();
   
   $LB::QuickEditGroup.add( %stack.formCache );
   
  // Add Base to Form
   %contentCtrl.add( %base );

   %base.MessageControl = %stack;
   
   // Return Ref to Base.
   return %base;
}

//-----------------------------------------------------------------------------
// Form Content Save Function
//-----------------------------------------------------------------------------
function LBQuickEdit::SaveContent( %contentCtrl )
{
   // Nothing.
}

function updateQuickEdit()
{
   GuiFormManager::SendContentMessage( $LBQuickEdit, 0, "inspectUpdate" );
}

//-----------------------------------------------------------------------------
// Form Content Message Callback
//-----------------------------------------------------------------------------
function LBQuickEditClass::onContentMessage( %this, %sender, %message )
{
   %messageCommand = GetWord( %message, 0 );
   %messageValue   = GetWord( %message, 1 );
   switch$( %messageCommand )
   {
      case "inspect":
         if( %messageValue $= "" )
            %this.syncQuickEdit(ToolManager.getLastWindow().getScene());
         else
            %this.syncQuickEdit( %messageValue );
            
         %this.sizeStack();
         ToolManager.getLastWindow().setFirstResponder();
         
      case "inspectUpdate":
         %this.syncQuickEdit( %this.currentQuickEdit );
         %this.sizeStack();
         ToolManager.getLastWindow().setFirstResponder();
      
      case "inspectSpatial":
         if( %messageValue !$= "" )
            GuiFormManager::BroadcastContentMessage( "LevelBuilderQuickEditClasses", %this, "updateSpatialQuickEdit" SPC %this.currentQuickEdit );
            
         %this.sizeStack();
         ToolManager.getLastWindow().setFirstResponder();
         
      case "resize":
         %this.sizeStack();
         
      case "clearCache":
         %this.clearCache();
   }
}

function LBQuickEditClass::sizeStack( %this )
{
   // Early Out.
   if( !isObject( %this ) )
      return;
      
   // Get to the children first.   
   %count = %this.getCount();
   for( %i = 0; %i < %count; %i++ )
      LBQuickEditClass::sizeStack( %this.getObject( %i ) );

   // Now act appropriately.   
   if( %this.getClassName() $= "GuiStackControl" )
      %this.updateStack();
   else if( %this.getClassName() $= "GuiRolloutCtrl" && %this.isExpanded() )
      %this.sizeToContents();
   else if( %this.getClassName() $= "GuiDynamicCtrlArrayControl" )
      %this.refresh();
      
}

function LBQuickEditClass::clearQuickEdit( %this )
{
   %this.currentQuickEdit = "";
   if( %this.getcount() == 0 )
      return;
      
   // Save off the scroll position.
   %form = %this.getObject(0);
   if (isObject(%form))
      %this.scrollPosition[%form.showClass] = %this.scrollCtrl.getScrollPositionY();
   
   // remove and de-ref the stack contents
   while( %this.getcount() > 0 )
   {
      %form = %this.getObject(0);
      if (%this.isMember(%form))
      {
         %this.remove(%form);
         //RootGroup.add(%form);
         $LB::QuickEditGroup.add( %form );
         GuiFormManager::RemoveContentReference( "LevelBuilderQuickEditClasses", %form.showClass, %form );
      }
   }
}

function LBQuickEditClass::clearCache(%this)
{
   if (!isObject(%this.formCache))
      return;
   
   %this.clearQuickEdit();
   
   while (%this.formCache.getCount())
   {
      %form = %this.formCache.getObject(0);
      %form.delete();
   }
}

function LBQuickEditClass::syncQuickEdit( %this, %quickEditObj )
{
   if( %quickEditObj == %this.currentQuickEdit )
   {
      // broadcast to existent classes to update their gui elements
      GuiFormManager::BroadcastContentMessage( "LevelBuilderQuickEditClasses", %this, "syncQuickEdit" SPC %quickEditObj );
      
      return;
   }
   
   // Clear the stack - No Duplicates here!
   %this.clearQuickEdit();
   
   // Store quickedit object.
   %this.currentQuickEdit = %quickEditObj;   

   
   // Load dynamic edit bar panels
   %formsToAdd = new SimSet();

   %formLibrary = GuiFormManager::FindLibrary( "LevelBuilderQuickEditClasses" );   
   if( !isObject( %formLibrary ) )
   {
      error("Unable to find any quickedit content forms!");
      return false;
   }
   
   %count = GuiFormManager::GetFormContentCount( %formLibrary );
   for( %i = 0; %i < %count; %i++ )
   {
      %contentObj = GuiFormManager::GetFormContentByIndex( %formLibrary, %i );
      if( !isObject( %contentObj ) )
         continue;
      
      %name = GetWord( %contentObj.Name, 0 );
      if( !%quickEditObj.isMemberOfClass( %name ) && !( %quickEditObj.class $= %name ) )
         continue;
      
      if( %name $= "t2dStaticSprite" && %quickEditObj.isMemberOfClass( "BitmapFontObject" ) )
         continue;
         
      %form = %this.findInCache(%contentObj.Name);

      // Has it already been created?
      if (isObject(%form))
      {
         if( !%form.floater )
            %formsToAdd.add(%form);
         GuiFormManager::AddContentReference( %formLibrary, %contentObj.Name, %form );
      }
      
      // Not found, create it.
      else
      {
         if( %contentObj.CreateFunction !$= "" )
         {
            %result = eval( %contentObj.CreateFunction @ "(" @ %this @ "," @ %quickEditObj @ ");" );
            if( isObject( %result ) )
            {
               GuiFormManager::AddContentReference( %formLibrary, %contentObj.Name, %result );
               if( !%result.floater )
                  %formsToAdd.add(%result);
                  
               %this.addToCache(%result, %contentObj.Name);
            }
         }
      }
   }
   
   %count = %formsToAdd.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %form = %formsToAdd.getObject(%i);
      if (%form.showClass $= "SceneObject")
      {
         %formsToAdd.pushToBack(%form);
         break;
      }
   }
   // Message the quick edit classes so they can inspect objects as they are selected.
   // This is purposely being called before the objects are added so the quick edit obj can be updated correctly.
   GuiFormManager::BroadcastContentMessage( "LevelBuilderQuickEditClasses", %this, "syncQuickEdit" SPC %quickEditObj );
   
   for (%i = 0; %i < %count; %i++)
      %this.add(%formsToAdd.getObject(%i));
      
   %form = %this.getObject(0);
   if (isObject(%form))
   {
      %yScroll = %this.scrollPosition[%form.showClass];
      if (%yScroll $= "") %yScroll = 0;
      %this.scrollCtrl.setScrollPosition(0, %yScroll);
   }

   GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "resize");
}

function LBQuickEditClass::addToCache(%this, %form, %class)
{
   %form.showClass = %class;
   %this.formCache.add(%form);
}

function LBQuickEditClass::findInCache(%this, %class)
{
   for (%i = 0; %i < %this.formCache.getCount(); %i++)
   {
      %form = %this.formCache.getObject(%i);
      if (%form.showClass $= %class)
         return %form;
   }
   return 0;
}
