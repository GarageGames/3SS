//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
function Editor::onStartUpComplete(%this, %messageData)
{
    %loadSuccess = ModuleDatabase.loadGroup("projectTools");
    
    if (%loadSuccess)
        EditorEventManager.postEvent("_ProjectToolsLoaded");
}

function Editor::onProjectToolsLoaded(%this, %messageData)
{
    // Show the editor shell
    Canvas.setContent(EditorShellGui);
    
    // Clear all the views from the shell
    EditorShellGui.clearViews();
    
    // Push the template selector view
    EditorShellGui.addView(TemplateListGui, "");
    EditorShellGui.addView(GamesLocationGui, "");
    EditorShellGui.setToolBar(HomeScreenToolbar);
    
    // Present the Welcome GUI
    Canvas.pushDialog(WelcomeGui);
}

function Editor::onProjectLoaded(%this, %messageData)
{
    EditorShellGui.clearViews();
    %module = ModuleDatabase.findLoadedModule(LBProjectObj.templateModule);
    %toolsGroup = %module.ToolsGroup;
    
    ModuleDatabase.loadGroup(%toolsGroup);
    EditorShellGui.setCommonToolBar(CommonToolbar);
}

function Editor::onCoreShutdown(%this, %messageData)
{
}

function showMainEditor()
{
    Canvas.setContent(EditorShellGui);
}