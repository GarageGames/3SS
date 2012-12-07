//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function initializeBehaviorEditor()
{   
//   execPrefs("behaviorEditorPrefs.cs");
   exec("./scripts/behaviorManagement.ed.cs");
   exec("./scripts/behaviorEditor.ed.cs");
   exec("./scripts/behaviorList.ed.cs");
   exec("./scripts/behaviorStack.ed.cs");
   exec("./scripts/undo.ed.cs");
   exec("./scripts/fieldTypes.ed.cs");
}

function destroyBehaviorEditor()
{
   // not much to do here
}
