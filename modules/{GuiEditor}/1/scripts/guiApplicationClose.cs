//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// Subscribe to application close event
if (isObject(GuiEditorGui) )
{
    Input::GetEventManager().subscribe(GuiEditor, "ClosePressed");   
    Input::GetEventManager().subscribe(GuiEditor, "BeginShutdown");
}


// Called when the application is beginning to shut down - Before onExit is called
function GuiEditor::onBeginShutdown(%this)
{      
    if (isObject(GuiEditorContent) )
    {
        %editObject = GuiEditorContent.getObject(0);
        %undoMgr =  %this.getUndoManager();
        %undoCount = %undoMgr.getUndoCount();
        %redoCount = %undoMgr.getRedoCount();

        // Prompt to save changes
        if (isObject(%editObject) && (%undoCount > 0 || %redoCount > 0) && $GuiDirty)
        {
            Canvas.setContent(GuiEditorGui);
            Canvas.repaint();

            %file = %editObject.getScriptFile();
            %file = fileName(%file);
            
            %mbResult = checkSaveChanges(%file , false);
            
            if (%mbResult $= $MROk)
                GuiEditorSaveGui();
        }
        
        // Prevent recursion when we shutdown
        %undoMgr.clearAll();      
    }
}

// Called when the application is beginning to shut down - Before onExit is called
function GuiEditor::onClosePressed(%this)
{   
    if (!isObject(GuiEditorContent) )
        return true;

    %editObject = GuiEditorContent.getObject(0);
    %undoMgr =  %this.getUndoManager();

    // Prompt to save changes
    if (isObject(%editObject) && (%undoMgr.getUndoCount() > 0 || %undoMgr.getRedoCount() > 0) && $GuiDirty)
    {
        Canvas.setContent(GuiEditorGui);
        Canvas.repaint();

        %file = %editObject.getScriptFile();
        
        if (%file $= "")
            %file = "Untitled.gui.taml";
            
        %file = fileName(%file);
        %mbResult = checkSaveChanges(%file , true);

        // Don't Quit
        if (%mbResult $= $MRCancel) 
            return false;
        else if (%mbResult $= $MROk)
            GuiEditorSaveGui();
    }
    
    // Prevent recursion when we shutdown
    %undoMgr.clearAll();      

    // Ok to quit
    return true;
}


// 
function checkSaveChanges(%documentName, %cancelOption)
{
    %msg = "Do you want to save changes to document " @ %documentName @ "?";

    if (%cancelOption == true) 
        %buttons = "SaveDontSaveCancel";
    else
        %buttons = "SaveDontSave";

    return MessageBox("3SS", %msg, %buttons, "Question");   
}
