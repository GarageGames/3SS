//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function parseTemplates()
{
    $templateCount = 0;
    
    // Scan for templates
    %templateIDs = ModuleDatabase.findModuleTypes("Template", false);

    %templateCount = getWordCount(%templateIDs);
    
    for(%i = 0; %i < %templateCount; %i++)
    {
        %templateModule = getWord(%templateIDs, %i);
        
        %template = new ScriptObject();
        
        %template.path = "^modules/";
        
        %templateName = %templateModule.ModuleId;
        
        %template.moduleId = %templateName;
        %template.type = %templateModule.Genre;
        %template.version = %templateModule.VersionId;
        %template.creator = %templateModule.Creator;
        %template.description = %templateModule.Description;
        %template.baseGui = %templateModule.BaseGui;
        %template.startLevel = %templateModule.StartLevel;
        %template.icon = %templateModule.Icon;
        %template.module = %templateModule;
        
        $templateList[$templateCount] = %template;
                    
        $templateCount++;        
    }
}
function synchronizeGame()
{
    %gameLocation = expandPath("^project");
    
    // Store the location of the directory that will contain the game's module dependencies
    %gameModulesDirectory = %gameLocation @ "/modules";
    
    // Scan the module dependencies folder
    //ModuleDatabase.scanModules(%gameLocation);
    
    %gameModuleDefinition = ModuleDatabase.getDefinitionFromId(LBProjectObj.templateModule, "Template");
    
    if (%gameModuleDefinition $= "")
    {
        error("### ERROR: Could not find template module for copying!!!");
        return;
    }
    
    // Synchronize the dependencies to the new game location    
    %synchResult = ModuleDatabase.synchronizeDependencies(%gameModuleDefinition, %gameModulesDirectory);
    
    if (!%synchResult)
    {
        error("### Error synchronizing new game project");
        return;
    }
}

function newProject(%name)
{
    if ($templateCount <= 0)
        parseTemplates();
    
    showNewProjectDialog(%name);
}

function createNewProject(%name, %sourceFilesLocation, %template, %duplicate)
{
    if (!isValidFileName(%name))
        return;
        
    // Remove any spaces the user typed
    %directory = strreplace(%name, " ", "");

    // The location of the new game project will be in the user's home directory
    // under the 3StepStudioProjects folder
    %gameLocation = expandPath($UserGamesLocation @ "/" @ %template @ "/" @ %directory @ "/");
   
    if (isFile(%gameLocation @ "project.tssproj"))
    {
        %result = messageBox("Invalid Name", "A game already exists at this location. Choose a new name or delete the old game before continuing.", "Ok");
        return false;
    }
   
    // Pop this dialog
    Canvas.popDialog(NewProjectDlg);

    // Game directory that will contain the template module
    pathCopy(%sourceFilesLocation @ "game", %gameLocation @ "game", false);
    
    // Modules directory that will contain module dependencies
    pathCopy(%sourceFilesLocation @ "modules", %gameLocation @ "modules", false);

    // Main Script
    pathCopy(%sourceFilesLocation @ "main.cs", %gameLocation @ "main.cs");
    
    // T2D project
    pathCopy(%sourceFilesLocation @ "project.tssproject", %gameLocation @ "project.tssproj");
    
    // OpenAL DLL
    pathCopy(%sourceFilesLocation @ "OpenAL32.dll", %gameLocation @ "OpenAL32.dll");
    
    // OpenGL DLL
    pathCopy(%sourceFilesLocation @ "opengl2d3d.dll", %gameLocation @ "opengl2d3d.dll");
    
    // Torsion file
    pathCopy(%sourceFilesLocation @ "Game.torsion", %gameLocation @ "Game.torsion");
    
    // iOS loading screen
    pathCopy(%sourceFilesLocation @ "Default.png", %gameLocation @ "Default.png");

    // Set the new game's executables to match the name the user provided
    copyProjectGameBinaries(%gameLocation, %directory);
     
    %projectFile = %gameLocation @ "project.tssproj";

    // Convert the human-readable template name to the corresponding module ID
    %moduleID = %template;
    
    // Mutate the original module ID into something unique for the created game
    %mutatedID = "{UserGame}";
    
    if (!%duplicate)    
    {
        %sourceModule = ModuleDatabase.getDefinitionFromId(%moduleID, "Template");
        
        if (%sourceModule $= "")
        {
            error("!!! ERROR: Could not find template module for copying!!!");
            return;        
        }
        
        // Store the folder path of the module definition we are going to copy
        %sourceModulePath = %sourceModule.ModulePath;
        
        // Store the file path of the module definition (.module)
        %moduleFilePath = %sourceModule.ModuleFilePath;

        // Store the directory where the copied module will exist
        %newModulePath = %gameLocation @ "/game/Template";
        
        // Store the new file path for copied template module. This should
        // match the mutated ModuleId
        %newModuleFilePath = %newModulePath @ "/" @ %mutatedID @ ".module.taml";
        
        ModuleDatabase.copyModule(%sourceModule, %mutatedID, %newModulePath, false);
        
        // If the module file did not copy or did not exist, error out
        if (!isFile(%newModuleFilePath))
        {
            error("!!! Module definition not copied !!!");
            return;
        }
    }
    
    LBProjectObj.sourceModule = %moduleID;
    LBProjectObj.templateModule = %mutatedID;
    LBProjectObj.projectName = %name;
    LBProjectObj.currentLevelFile = $templateList[%templateIndex].startLevel;
    
    %dateTime = stripChars(getCurrentDate(true), "/");
    %date = getSubStr(%dateTime, 0, strstr(%dateTime, ":"));
    %time = getSubStr(%dateTime, strstr(%dateTime, ":")+1, strlen(%dateTime));
    %strippedTime = stripChars(%time, ":");
    LBProjectObj.lastModified = %date @ "." @ %strippedTime;
    
    // Post Create Project
    Projects::GetEventManager().postEvent("_ProjectCreate", %projectFile);   
    
    // Post Open Event
    if (!LBProjectObj.isActive() && !%duplicate)
    {
        Projects::GetEventManager().postEvent("_ProjectOpen", %projectFile);
        $Game::ProductName = %name;
        $Game::CompanyName = "GarageGames";
        export("$Game::ProductName", expandPath("^gameTemplate/scripts/prefs.cs"), true, true);
        export("$Game::CompanyName", expandPath("^gameTemplate/scripts/prefs.cs"), true, true);
    }
}

// Copy over game executable and any other binaries needed for a project
// Rename the game executable so it matches the project name(non mac only)
function copyProjectGameBinaries(%gameLocation, %name)
{
    %exename = %gameLocation @ %name @ ".exe";
    %appname = %gameLocation @ %name @ ".app";

    %srcpath = expandPath("^tool/templates/projectFiles/");
    
    if (isDebugBuild())
        %executeable = "3StepStudioGame_Debug";
    else
        %executeable = "3StepStudioGame";
    
    if($platform $= "windows")    
        pathCopy(%srcpath @ %executeable @ ".exe", %exename, false);
    else
        pathCopy(%srcpath @ %executeable @ ".app", %appname, false);
        
    pathCopy(%srcpath @ "unicows.dll", %gameLocation @ "unicows.dll");
    pathCopy(%srcpath @ "openAL32.dll", %gameLocation @ "openAL32.dll");
}

function isValidProject(%project)
{
    // Should be more robust check.
    return isFile(%project @ "/project.tssproj");
}

///
/// Returns Projects API's EventManager Singleton
/// 

function Projects::GetEventManager()
{
   if (!isObject($_Tools::ProjectEventManager))
      $_Tools::ProjectEventManager = new EventManager() { queue = "ProjectEventManager"; };
      
   return $_Tools::ProjectEventManager;
}


function Projects::DeclareProjectTarget(%projectTargetNamespace, %objectGlobalName)
{
   // At some point it would be nice to have a console method
   // on SimObject that supported validating that another object
   // implemented all the methods provided by a given namespace.
   // .validateInterface("myNamespace") or some such.
   %projectObject = new ScriptMsgListener(%objectGlobalName) 
   { 
      class = %projectTargetNamespace; 
      superclass = ProjectBase; 
   };   
}

///
/// Public Project Events
///

/// ProjectOpened
///
/// is fired when a project has been opened and all bootstrap
/// processing has occured on the project object.  
/// At this point it is safe for addons to do post-load processing
/// such as creating new create entries and other specific modifications
/// to the editor. 

Projects::GetEventManager().registerEvent("ProjectOpened");

/// ProjectClosed
///
/// is fired when a project is about to be closed and it's 
/// resources destroyed by the base project class.  Addons
/// should use this event to free any project specific resources
/// they have allocated, as well as saving of data where applicable. 

Projects::GetEventManager().registerEvent("ProjectClosed");

/// ProjectDeploy 
///
/// is fired when a game is about to be run from the editor and on 
/// this event addons and third party's should without scheduling or 
/// other delaying calls, deploy any game data that the game will need
/// to it's game path.
/// 
/// Example, the core package zip code intercepts this message and
/// builds and deploys a new core.zip if is necessary 

Projects::GetEventManager().registerEvent("ProjectDeploy");

/// Currently Unused 

Projects::GetEventManager().registerEvent("ProjectFileAdded");

/// Currently Unused 

Projects::GetEventManager().registerEvent("ProjectFileRemoved");

///
/// ProjectOpen Event Handler
/// - %data is the project object to be opened 

function ProjectBase::onProjectOpen(%this, %data)
{
   error("onProjectOpen Handler not implemented for class -" SPC %this.class);
}

///
/// ProjectClose Event Handler
/// 

function ProjectBase::onProjectClose(%this, %data)
{
   error("onProjectClose Handler not implemented for class -" SPC %this.class);
}

///
/// ProjectAddFile Event Handler
/// 

function ProjectBase::onProjectAddFile(%this, %data)
{
   error("onProjectAddFile Handler not implemented for class -" SPC %this.class);
}

///
/// ProjectRemoveFile Event Handler
/// 

function ProjectBase::onProjectRemoveFile(%this, %data)
{
   error("onProjectRemoveFile Handler not implemented for class -" SPC %this.class);
}

function updateIosConfigFile(%configPath)
{
    if(isFile(%configpath))
    {
        %headerPath = expandPath("^tool/engine/source/");
        %libPath = expandPath("^tool/engine/lib/");
        
        %fo = new FileObject();
        %fo.openForWrite(%configpath);

        // Write some lines in
        %fo.writeLine("TSS_LIB_PATH = " @ %libPath);
        %fo.writeLine("TSS_SOURCE_PATH = " @ %headerPath);
         
        %fo.close();
        %fo.delete();
    }
}

/// <summary>
/// This function ensures that the desired level file name is valid.
/// </summary>
/// <param name="fileName">The file name we want to validate.</param>
/// <return>Returns true if the file name is valid, or false if not.</return>
function isValidFileName(%fileName)
{
    // Check for empty filename 
    if (strreplace(%fileName, " ", "") $= "")  
    {
        // Show message dialog
        NoticeGui.display("Level name cannot be empty!");

        
        return false;   
    }

    // Check for spaces
    if (strreplace(%fileName, " ", "") !$= %fileName)
    {
        // Show message dialog
        NoticeGui.display("Level name cannot contain spaces!");
        
        return false;
    }
    
    // Check for invalid characters
    %invalidCharacters = "-+*/%$&§=()[].?\"\\#,;!~<>|°^{}";
    %strippedName = stripChars(%fileName, %invalidCharacters);
    if (%strippedName !$= %fileName)
    {
        // Show message dialog
        NoticeGui.display("Level name contains invalid symbols!");
         
        return false;   
    }
    
    return true;
}