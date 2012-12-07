//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// Defines functionality for object behaviors dialog window

//you might need to be able to order this
//when deleting, offer an option to keep dynamic fields

// The dialog becomes active - fill Behaviors lists.
function ObjectBehaviorsDlg::onWake( %this )
{
   // Clear dependent Behaviors list  
   ObjectAppliedBehaviorsList.clearItems();
   
   // Make the behavior tree
   %this.createAvailableBehaviorTree();

   %count = %this.object.getBehaviorCount();
   for (%i = 0; %i < %count; %i++)
   {              
      %behavior = %this.object.getBehaviorByIndex(%i);
      ObjectAppliedBehaviorsList.addItem(%behavior.getTemplateName().internalName);
   }
   
   %this.toggleShowBehaviorInfo(0);
   %this.setSelectedBehavior();
}

// (simObject this)
// Create the available behavior tree
// Layout depends on state of Resource and Type checkboxes
// @param this This ObjectBehaviorsDlg
function ObjectBehaviorsDlg::createAvailableBehaviorTree(%this)
{
   %tree = ObjectAvailableBehaviorsTree; //just for ease of typing/reading
   %tree.clear(); // we're gonna set it up from scratch...
   
   // get checkbox info
   %byType = ObjectBehaviorsDlg.findObjectByInternalName("treeTypeCheckbox",1).getValue(); 
   %byResource = ObjectBehaviorsDlg.findObjectByInternalName("treeResourceCheckbox",1).getValue(); 
   
   // create an invisible guiTextListCtrl object used only for sorting
   %sorter = new guiTextListCtrl();
   %sorter.visible = false;
   
   // go through available behaviors list
   %availableBehaviors = behaviorSet;  //the global set of behaviors
   %count = %availableBehaviors.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %behavior = %availableBehaviors.getObject(%i);
      if (%behavior.validateObjectClass( %this.object)) // the object is of the correct class
      {
         //create a string to organize the tree by: rootFolder SPC folder...
         %sortString = "";
         if (%byResource) %sortString = %sortString @ %behavior.fromResource;
         if (%byType)
         {
            if (%sortString !$= "" && %behavior.behaviorType !$= "")
               %sortString = %sortString @ " ";
            %sortString = %sortString @ %behavior.behaviorType;
         }
         %sorter.addRow( %behavior.getID(), %sortString);
      }
   }
   
   // we've created our list to sort, now sort it 
   %sorter.sort(0);

   
   // now we can turn it into an alphabetical tree
   %lastSortString = "";
   %sortIDs = ""; // SPC delimited list of treeItemIDs
   %count = %sorter.rowCount();
   for (%i = 0; %i < %count; %i++)
   {
      // get the sorted string
      %sortString = %sorter.getRowText( %i);
      %behavior = %sorter.getRowID( %i).internalName;
      
      // analyze the string to determine if folders need to be created
      %folderCount = getWordCount(%sortString);
      for (%j = 0; %j < %folderCount; %j++)
      {
         if (getWord(%sortString, %j) !$= getWord (%lastSortString, %j))
            break;
      }
      
      // if we found a place where the sort strings differ, make new folders starting there
      if (%j < %folderCount)
      {
         // recreate sortIDs, starting at the point where identical to last sortIDs
         %sortIDs = getWords( %sortIDs, 0 ,%j);  // same IDs
         for (%k = %j; %k < %folderCount; %k++)
         {
            %startID = getWord(%sortIDs, %k-1);
            %newID = %tree.insertItem(%startID, getWord(%sortString, %k),"",1,2,1);
            %sortIDs = setWord(%sortIDs, %k, %newID);
         }
      }
      
      //finally we're ready to add the actual behavior
      %tree.insertItem( getWord(%sortIDs, getWordCount(%sortIDs)-1), %behavior,"", 0);
      
      %lastSortString = %sortString;
   }
}

// Show/Hide Behavior Info
function ObjectBehaviorsDlg::toggleShowBehaviorInfo(%this, %toggleTo)
{
   %oldShowBehaviorInfo = %this.showBehaviorInfo;
   if (%toggleTo $= "") 
      %this.showBehaviorInfo = !%this.showBehaviorInfo;
   else 
      %this.showBehaviorInfo = %toggleTo;
   %window = %this.findObjectByInternalName("windowControl");

   if (%this.showBehaviorInfo)
   {  
      //expand
      %window.minExtent = setWord(%window.minExtent, 0, %window.infoExpandedMinExtent);
      %window.setExtent(%window.infoExpandedWidth, getWord(%window.Extent, 1));
      ObjectBehaviorInfoControl.HorizSizing = width;
      ObjectBehaviorInfoControl.setExtent(getWord(%window.InfoControlStartExtent,0),getWord(%window.extent,1) + %window.InfoControlHeightOffset);
      ObjectBehaviorInfoControl.setVisible(1);
      %window.resizeWidth = 1;
   }
   else 
   {
      //compress
      if (%oldShowBehaviorInfo == true) //if we are coming from expanded
      {
         //save current extent
         %window.infoControlStartExtent = setWord(%window.infoControlStartExtent, 0, getWord(ObjectBehaviorInfoControl.extent, 0));
         %window.infoExpandedWidth = setWord(%window.infoExpandedWidth, 0, getWord(%window.extent, 0));
      }

      //resize
      ObjectBehaviorInfoControl.HorizSizing = right;
      %window.minExtent = setWord(%window.minExtent, 0, %window.infoCompressedWidth);
      %window.setExtent(%window.infoCompressedWidth, getWord(%window.Extent, 1));
      ObjectBehaviorInfoControl.setVisible(0);
      %window.resizeWidth = 0;
   }
}

// (simObject this)
// The user has confirmed behavior applications and removals.  Make actual changes to the object.
// If new behaviors are applied, check for unmet dependencies and, if found, offer to fix them.
// @param this This SaveObjectBehaviorsButton

function SaveObjectBehaviorsButton::onClick( %this )
{   
   //%autoDefault = ObjectBehaviorsDlg.findObjectByInternalName("AutoDefaultCheckbox",1).getValue(); 
   //%autoRequirement = ObjectBehaviorsDlg.findObjectByInternalName("AutoRequirementCheckbox",1).getValue(); 
   %object = ObjectBehaviorsDlg.object;
   //%addBehaviorList = "";  //space delimited list of behaviors to add
   //%removeBehaviorList = "";  //space delimited list of behaviors to remove
   
   //determine which behaviors need to be removed, and remove them
   %count = %object.getBehaviorCount();
   for ( %i = 0; %i < %count; %i++ )
   {
      %behaviorInstance = %object.getBehaviorByIndex(%i);
      %word = %behaviorInstance.class;
      if (ObjectAppliedBehaviorsList.findItemText(%word) == -1)
      {
         %object.removeBehavior( %behaviorInstance );
      }
   }
   
   // determine which behaviors are new
   %listCount = ObjectAppliedBehaviorsList.getItemCount();
   for( %i = 0; %i < %listCount; %i++ )
   {
      %behaviorTemplateName = ObjectAppliedBehaviorsList.getItemText( %i );
      if (!isObject(%object.getBehavior( %behaviorTemplateName) ) )
      {
         // Add behavior to object
         addBehavior( %behaviorTemplateName, %object );
         
         // Object should use behaviors now... we just added one.
         %object.doBehaviors = true;
         
         //%addBehaviorList = setWord(%addBehaviorList, getWordCount(%addBehaviorList), %word);
      }
   }

   ////add new behaviors and check for unmet requirements
   //%count = getWordCount(%addBehaviorList);
   //for( %i = 0; %i < %Count; %i++ )
   //{
      //%word = getWord(%addBehaviorList, %i );
      //if (wordPos(%oldBehaviors,%word) == -1)  //if we didn't already have the behavior
      //{
         //%behaviorInstance = addBehavior(%word, %object);  //set it up
         ////if (%behaviorInstance.getTemplateName().checkForUnmetDependencies(%object) !$= "")
         ////{
            ////%unmetList = setWord(%unmetList, getWordCount(%unmetList), %word);
         ////}
      //}
   //}
   
   // Done above in behavior add
   //   
   //// If going from no behaviors to having behaviors, set doBehaviors flag to true.  And if going from having behaviors to none, set it to false
   //if (%listCount && %listCount == getWordCount(%addBehaviorList))
      //%object.doBehaviors = true;
   //else if (%listCount == 0 && getWordCount(%removeBehaviorList))
      //%object.doBehaviors = false; 
   
   //// if there are behaviors with unmet dependencies...
   //if (%unmetList !$= "") 
   //{
      //if (%autoRequirement)
      //{
         //// go through the unmet requirement list and just fix 'em
         //%count = getWordCount(%unmetList);
         //for (%i = 0; %i < %count; %i++)
         //{
            //%behavior = getWord(%unmetList, %i);
            //%behaviorT = %behavior@"_Template";
            //%behaviorT.autoFixObjectRequirements( ObjectBehaviorsDlg.object, %behavior);        
         //}
         //GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspect" SPC ObjectBehaviorsDlg.object );        
      //}
      //else
      //{
         //// open dialogs that offer to fix each one in the list 
         //ObjectBehaviorsDlg.unmetRequirementsList = %unmetList;
         //ObjectBehaviorsDlg.openUnmetRequirementsDialog();
      //}
   //}
   //else
   //{
      // Reinspect object
      //GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspect" SPC ObjectBehaviorsDlg.object );
   //}
   // Pop Behavior Dialog
   Canvas.popDialog( ObjectBehaviorsDlg );
}

// (simObject this)
// Recurring function that goes through a list of dependencies with unmet requirements.
// For each dependency, open a dialog offering to meet requirements
// @param this This saveObjectDlg

function ObjectBehaviorsDlg::openUnmetRequirementsDialog(%this)
{
   // %this.unmetRequirementsList must be defined before calling this function.
   %unmetList = %this.unmetRequirementsList;
   if (getWordCount (%unmetList))
   {
      //pop a behavior of the unmetlist
      %behavior = getWord(%unmetList, 0);
      ObjectBehaviorsDlg.unmetRequirementsList = removeWord( %unmetList, 0);      
      
      //make sure it still has unmet dependencies (previous autofixes might have fixed it)
      %dependencyList = BehaviorManager.checkForUnmetRequirements(%this.object, %behavior);
      if (%dependencyList !$= "")
      {
         %count = getFieldCount(%dependencyList);
         for (%i = 0; %i < %count; %i+=2)
         {
            %dependency = getField(%dependencyList, %i);
            %dependencyValue = getField(%dependencyList, %i+1);
            %details = %details @ %dependency SPC "should be set to \"" @ %dependencyValue @"\"\n";
         }                 
         //open dialog to offer autofix
         //recursion occurs in callbacks from messagebox
         messageBoxOkCancelDetails(
            %behavior @" Requirements", 
            %behavior SPC "has unmet dependency requirements.  Do you want to automatically set the appropriate values? (recommended)",
            %details,
            "BehaviorManager.autoFixObjectRequirements("@%this.object@", "@%behavior@");ObjectBehaviorsDlg.openUnmetRequirementsDialog();",
            "ObjectBehaviorsDlg.openUnmetRequirementsDialog();");      
      }
   }
   else
   {
      //if all behaviors have been popped from unmet list, just reinspect.  The dialogs have already popped themselves from the canvas
      GuiFormManager::SendContentMessage( $LBQuickEdit, %this, "inspect" SPC ObjectBehaviorsDlg.object );
   }
}

//set selected behavior... basically only for filling out the info box
function ObjectBehaviorsDlg::setSelectedBehavior(%this, %behavior, %isInAppliedList)
{
   %infobutton = ObjectBehaviorsDlgInfoButton;
   if (%behavior !$= "")
   {   
      %behavior = %behavior@"_Template";
       //title
      ObjectBehaviorInfoTitle.setText(%behavior.internalName);
      
      //description
      if (%isInAppliedList)  //selection is in currently applied list
      {
         //set up object as if it had all the selected behaviors, so we can check for conflicts
         %realBehaviorList = %this.object.behaviorList;
         %this.object.behaviorList = "";
         for (%i = 0; %i < ObjectAppliedBehaviorsList.getItemCount(); %i++)
            %this.object.BehaviorList = setWord(%this.object.BehaviorList, %i, ObjectAppliedBehaviorsList.getItemText( %i ));
            
         //do actual checking   
         %description = %behavior.getFullDescription(%this.object, 1);  //check against object
         if (%behavior.getConflictList( %this.object, %behavior, 1) !$= "")  //and update button color
            %infoButton.setBitmap(%infoButton.iconWarningBitmap);
         else
            %infoButton.setBitmap(%infoButton.iconNormalBitmap);
            
         //restore real behavior list
         %this.object.behaviorList = %realBehaviorList;
      }
      else
      {
         %description = %behavior.getFullDescription(); //just display info   
         %infoButton.setBitmap(%infoButton.iconNormalBitmap); //and use normal button color
      }
      if (trim(%description) $= "") 
         %description = "No Description Available";
      ObjectBehaviorInfoDescription.setText(%description);
      
      //fields
      ObjectBehaviorInfoFieldStack.deleteChildren();
      for(%i = 0; %i < %behavior.fieldCount; %i++)
      {
         %newFieldData = %behavior.field[%i];
         %name = getField(%newFieldData,0);
         %default = getField(%newFieldData,1);
         %type = getField(%newFieldData,2);
         %extraInfo = %behavior.fieldExtra[%i];
         %tooltip = getField(%newFieldData,3);           
         if (%tooltip !$= "") 
            %tooltip = " : "@%tooltip;
         %fieldText = %name @ %tooltip; 
         ObjectBehaviorInfoFieldStack.addTextLine(%fieldText);
      }
   }
   else
   {
      %infoButton.setBitmap(%infoButton.iconNormalBitmap); //use normal button color
      ObjectBehaviorInfoTitle.setText("No Behavior Selected");
      ObjectBehaviorInfoDescription.setText("");
      ObjectBehaviorInfoFieldStack.deleteChildren();
   }
      
}

// User canceled the entire 'choose behaviors' operation, merely cleanup and exit
function CancelObjectBehaviorsButton::onClick( %this )
{
   Canvas.popDialog( ObjectBehaviorsDlg );
}

// Add a selected behavior.  Pop up a warning dialog if there are conflicts
function SetBehaviorActiveButton::onClick( %this )
{
   // Get selected item.
   %selItem = ObjectAvailableBehaviorsTree.getSelectedItem();
   if (%selItem == -1) 
      return; // nothing selected
   if (ObjectAvailableBehaviorsTree.getChild(%selItem) != 0)
      return; // it has a child, so it must be a folder not a behavior

   // Grab Behavior Name
   %behaviorName = ObjectAvailableBehaviorsTree.getItemText( %selItem );
   %behaviorT = %behaviorName@"_Template";

   //set up object as if it had all the selected behaviors, so we can check for conflicts
   %realBehaviorList = ObjectBehaviorsDlg.object.behaviorList;
   ObjectBehaviorsDlg.object.behaviorList = "";
   for (%i = 0; %i < ObjectAppliedBehaviorsList.getItemCount(); %i++)
      ObjectBehaviorsDlg.object.BehaviorList = setWord(ObjectBehaviorsDlg.object.BehaviorList, %i, ObjectAppliedBehaviorsList.getItemText( %i ));
   ObjectBehaviorsDlg.object.BehaviorList = setWord(ObjectBehaviorsDlg.object.BehaviorList, %i, %behaviorName); //add the new one
               
   //do actual checking   
   %conflicts = %behaviorT.getConflictList( ObjectBehaviorsDlg.object, %behaviorName, 1); //check for everything except unfulfilled dependencies
   if (%conflicts !$= "") 
   {
      %count = getFieldCount(%conflicts);
      for (%i = 0; %i < %count; %i++)
      {
         %details = %details @ getField(%conflicts,%i) @"\n";
      }
      messageBoxOkCancelDetails(
         "Behavior Conflict", 
         %behaviorName SPC "has conflicts with other applied behaviors.  Add it anyway? (not recommended)",
         %details, 
         "ObjectBehaviorsDlg.setSelectedBehaviorActive();");
   }
   else
      ObjectBehaviorsDlg.setSelectedBehaviorActive();
      
   //restore real behavior list
   ObjectBehaviorsDlg.object.behaviorList = %realBehaviorList;   
}
 
// Add an inactive Behavior to the object's used Behaviors list
function ObjectBehaviorsDlg::setSelectedBehaviorActive(%this)
{
   // Get selected item.
   %selItem = ObjectAvailableBehaviorsTree.getSelectedItem();
   if( %selItem != -1 ) 
   {
      // Grab Behavior Name
      %behaviorName = ObjectAvailableBehaviorsTree.getItemText( %selItem );
            
      // Add to applied list.
      if( ObjectAppliedBehaviorsList.findItemText( %behaviorName ) == -1 )
      {
         ObjectAppliedBehaviorsList.addItem( %behaviorName );
         ObjectAppliedBehaviorsList.setSelected( ObjectAppliedBehaviorsList.getItemCount() - 1);
      }
   }
}



// Remove an active Behavior from the object and move to unused list
function SetBehaviorInactiveButton::onClick( %this )
{
   // Get selected item.
   %selItem = ObjectAppliedBehaviorsList.getSelectedItem();
   if( %selItem != -1 ) 
   {
      // Grab Behavior Name
      %behaviorName = ObjectAppliedBehaviorsList.getItemText( %selItem );
      
      // Remove from active Behavior list.
      ObjectAppliedBehaviorsList.deleteItem( %selItem );   
   }
}

//set available behavior to 'applied' on double-clicked or Enter
function ObjectAvailableBehaviorsTree::onAltCommand( %this )
{
   SetBehaviorActiveButton.onClick();
}

//select an available behavior
function ObjectAvailableBehaviorsTree::onSelect(%this, %id)
{
   if (%id $= "") %id = %this.getSelectedItem();
   if (ObjectAvailableBehaviorsTree.getChild(%id) != 0)
      return; // it has a child, so it must be a folder not a behavior
         
   ObjectAppliedBehaviorsList.clearSelection();
   %text = %this.getItemText(%id);
   ObjectBehaviorsDlg.setSelectedBehavior(%text, 0);
}

//set applied behavior to 'available'
function ObjectAppliedBehaviorsList::onDoubleClick( %this )
{
   SetBehaviorInactiveButton.onClick();
}

//select an applied behavior
function ObjectAppliedBehaviorsList::onSelect(%this, %id, %text)
{
   ObjectAvailableBehaviorsTree.clearSelection();
   ObjectBehaviorsDlg.setSelectedBehavior(%text, 1);
}

//----------------------------------------------------------------------------------------
// (objectBehaviorInfoStack this, string text, int minSize)
// Adds a given line of text to a stack, then resizes stack to that text's horizontal size
// @param this This objectBehaviorInfoStack
// @param text The line of text to add to the stack
// @param minSize The smallest number of pixels the stack can be resized to
//----------------------------------------------------------------------------------------

function ObjectBehaviorInfoFieldStack::addTextLine(%this, %text, %minSize)
{
   if (%minSize $= "") %minSize = "1 1";
   %textline = new GuiTextCtrl() {
      canSaveDynamicFields = "0";
      Profile = "EditorTextHLLeftAutoSize";
      HorizSizing = "width";
      VertSizing = "bottom";
      position = "26 6";
      Extent = "179 14";
      MinExtent = "1 1";
      canSave = "1";
      Visible = "1";
      tooltipprofile = "EditorToolTipProfile";
      hovertime = "100";
      text = %text;
      maxLength = "1024";
      minSize = %minSize;
   };
   //set stack width to size of largest text string.
   if (getword(%textline.extent,0) > getword(%this.extent,0)) %this.setExtent(getword(%textline.extent,0), getword(%this.extent,1)); 
   %this.add(%textline);   
}

//----------------------------------------------------------------------------------------
// (ObjectBehaviorInfoFieldStack this)
// Deletes the children that contain field info for a behavior
// @param this This ObjectBehaviorInfoFieldStack
//----------------------------------------------------------------------------------------

function ObjectBehaviorInfoFieldStack::deleteChildren(%this)
{
   %count = %this.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %this.getObject(0).delete();
   }
   %this.setExtent(getword(%this.minsize,0),getword(%this.minsize,1));
}

//----------------------------------------------------------------------------------------
// (ObjectBehaviorsDlg this)
// Moves the selected behavior in the list box of currently applied behaviors
// @param this This ObjectBehaviorsDlg
// @param amount The amount to move the behavior (negative is up)
//----------------------------------------------------------------------------------------
function ObjectBehaviorsDlg::SelectedBehaviorMove(%this, %amount)
{
   %selItem = ObjectAppliedBehaviorsList.getSelectedItem();
   if (%selItem == -1) return;
   %itemCount = ObjectAppliedBehaviorsList.getItemCount();
   %newPos = %selItem + %amount;
   if (%newPos < 0 || %newPos >= %itemCount) return;
   %behavior = ObjectAppliedBehaviorsList.getItemText(%selItem);
   ObjectAppliedBehaviorsList.deleteItem(%selItem);
   ObjectAppliedBehaviorsList.insertItem(%behavior,%newPos);
   ObjectAppliedBehaviorsList.setSelected(%newPos);
}
