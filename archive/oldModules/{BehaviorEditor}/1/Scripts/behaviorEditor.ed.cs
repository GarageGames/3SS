//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
//-----------------------------------------------------------------------------
// Register Form Content
//
// For more information on the Form Content and the inner workings of it,
// please refer to editorCore/script/guiFormContentManager.ed.cs and the
// documentation on the functions.
//
GuiFormManager::AddFormContent( 
  "LevelBuilderQuickEditClasses", // Content Library Name
  "SceneObject Behaviors",     // Objects C++ class name or 'class' of object
  "BehaviorsRollout::CreateContent",// Create function
  "BehaviorsRollout::SaveContent",  // Save function
  2 );                            // Deprecated 'magin' option
  
                                
//-----------------------------------------------------------------------------
// Define form save function.
//
// This is unused here, but is called on our content right before we
// get deleted.
//
function BehaviorsRollout::SaveContent( %contentCtrl ) 
{
   // Nothing
}

//-----------------------------------------------------------------------------
// Define form create function.
//
// Your standard form create function that creates a base and
// two check boxes that control setFlipX and setFlipY
function BehaviorsRollout::CreateContent( %contentCtrl, %quickEditObj )
{
   // It's important to note that the class we give the first 
   // parameter of this function should NOT be the same as the
   // one we're in right now.  We add the QE to the end to 
   // differentiate them
   %base = %contentCtrl.createBaseStack("BehaviorsRolloutQE", %quickEditObj);
   %behaviorRollout = %base.createRolloutStack("Behaviors", true);
   %behaviorRollout.createAddBehaviorList();
   %behaviorRollout.createBehaviorStack();
   
   // Whenever we create form content, we must return it to the base
   return %base;
}

function BehaviorEditor::registerFieldType(%type, %create)
{
   $BehaviorEditor::fieldTypes[%type] = %create;
}

function BehaviorEditor::createFieldGui(%parent, %behavior, %fieldIndex)
{
   %fieldInfo = %behavior.template.getBehaviorField(%fieldIndex);
   %name = getField(%fieldInfo, 0);
   %type = getField(%fieldInfo, 1);
   
   %create = $BehaviorEditor::fieldTypes[%type];
   if (%create $= "")
      %create = $BehaviorEditor::fieldTypes["Default"];
   
   %parent.call(%create, %behavior, %fieldIndex);
}
