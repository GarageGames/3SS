//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// This module is reponsible setting variables and calling functions that
// will be used during the life of the entire editor. Examples include DSO
// preferences, reacting to specific OS requirements, etc.
function initializeEditorBoot()
{
    //-------------------------------------------------------------------------
    // Call necessary system functions
    //-------------------------------------------------------------------------
   
    // Perform file assocations based on the OS
    checkPlatformAssociations();
    
    //-------------------------------------------------------------------------
    // Set global system variables
    //-------------------------------------------------------------------------
    
    exec("./scripts/defaultPrefs.cs");

    // Set to true to ignore creation and loading of DSO files
    $Scripts::ignoreDSOs = false;
    
    // Set to true to track when non-default log mode is selected
    $logModeSpecified = false;

    // MICHTODO: This should be moved into the template module
    $Tools::TemplateToolDirty = false;

    // Script Override path may not be set until we hit this point or it will be erased, by design.
    $Scripts::OverrideDSOPath = "";
    
    // This set will be used to keep track of datablocks used only in the editor
    $ignoredDatablockSet = new SimSet();

    // We are running the editor    
    $LevelEditorActive = true;
    
    // Load the base tools groups.
    %loadSuccess = ModuleDatabase.LoadGroup( "baseTools" );

    // If the base loading finished, we are going to use an EventManager to post a message
    // This object will take over and handle loading of the next module set, as well
    // as pushing any special GUIs to the screen.
    if (%loadSuccess)
        EditorEventManager.postEvent("_StartUpComplete", "");
}

function destroyEditorBoot()
{
    // Unload the tools groups.
    ModuleDatabase.UnloadGroup( "baseTools" );
}

function checkPlatformAssociations()
{
    if( $platform $= "windows" )
    {
        %tgbr = new TGBRegistryManager();
       
        if( isObject( %tgbr ) )
        {
            %tgbr.setFileAssociations();
            %tgbr.registerExecutablePaths();
            %tgbr.delete();
        }
    }
    else
    {
        // Running on OS X. Is there an equivalent to the above code?
        // If not, maybe we should reconsider what we are doing here
    }
}

function onExit()
{
}