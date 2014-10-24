//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function showAllGames()
{
    AllGGamesList.filter = "All Games";
    queryCreatedProjects();
    
    // Clear all the views from the shell
    EditorShellGui.clearViews();
    
    // Push the template selector view
    EditorShellGui.addView(AllGamesGui, "");
    EditorShellGui.addView(GamesLocationGui, "");
    
    if($templateCount <= 0)
        parseTemplates();
    
    AG_SortBox.clear();
    AG_SortBox.add("Alphabetical", 0);
    AG_SortBox.add("Date", 1);
    AG_SortBox.setSelected(0);
    
    AG_FilterBox.clear();
    
    AG_FilterBox.add("All Games", 0);
    
    for(%i = 0; %i < $templateCount; %i++)
        AG_FilterBox.add($templateList[%i].description, %i+1);
    
    AG_FilterBox.setSelected(0);
}

function clearProjectSet()
{
    while(CreatedProjectSet.getCount())
    {
        %object = CreatedProjectSet.getObject(0);
        CreatedProjectSet.remove(%object);
        %object.delete();
    }
}

function queryCreatedProjects()
{
    if (!isObject(CreatedProjectSet))
        %set = new SimSet(CreatedProjectSet);
    else
        clearProjectSet(); //CreatedProjectSet.clear();
    
    // All projects should be created in this specific location (user app folder), so set the path
    %projectLocation = $UserGamesLocation;

    %projectFile = "/project.tssproj";

    %projectFileSpec = %projectLocation @ "/*.tssproj";
    
    addResPath(%projectLocation);
    
    for (%file = findFirstFile(%projectFileSpec); %file !$= ""; %file = findNextFile(%projectFileSpec))
    {
        %project = TamlRead(%file);
        CreatedProjectSet.add(%project);
    }
    
    removeResPath(%projectLocation);
}

function findProjectFileByName(%name)
{
    %projectFound = false;
    
    // All projects should be created in this specific location (user app folder), so set the path
    %projectLocation = $UserGamesLocation;

    %projectFile = "/project.tssproj";

    %projectFileSpec = %projectLocation @ "/*.tssproj";
    %fileLocation = "";
    
    addResPath(%projectLocation);
    
    for (%file = findFirstFile(%projectFileSpec); %file !$= ""; %file = findNextFile(%projectFileSpec))
    {
        %project = TamlRead(%file);
        
        
        if (%name $= %project.projectName)
        {
            %project.delete();
            %projectFound = true;
            %fileLocation = %file;
            break;
        }
    }
    
    removeResPath(%projectLocation);
    
    if (%projectFound)
        return %fileLocation;
}

// Thanks to Charlie Patterson for this
function easySort(%set, %comparator)
{
    for (%i = 0; %i < %set.getCount() - 1; %i++)
    {
        %minIndex = %i;
        
        for (%j = %i + 1; %j < %set.getCount(); %j++)
        {
            %which = call(%comparator, %set.getObject(%minIndex), %set.getObject(%j));
            
            if (%which > 0)
                %minIndex = %j;
        }
      
        if (%minIndex != %i)
            %set.reorderChild(%set.getObject(%minIndex).getId(), %set.getObject(%i).getId());
   }
}

function projectDateSort(%projectOne, %projectTwo)
{
    %projectOneDate = getSubStr(%projectOne.lastModified, 0, strstr(%projectOne.lastModified, "."));
    %projectTwoDate = getSubStr(%projectTwo.lastModified, 0, strstr(%projectTwo.lastModified, "."));
    
    if (%projectOneDate != %projectTwoDate)
        return %projectTwoDate - %projectOneDate;
        
    %projectOneTime = getSubStr(%projectOne.lastModified, strstr(%projectOne.lastModified, ".")+1, strlen(%projectOne.lastModified));
    %projectTwoTime = getSubStr(%projectTwo.lastModified, strstr(%projectTwo.lastModified, ".")+1, strlen(%projectTwo.lastModified));
    
    return %projectTwoTime - %projectOneTime;
}

// Sort type can be "Alphabetical", "Date", or "Template"
function sortProjectList(%sortType)
{
    if (%sortType $= "Alphabetical")
    {
        queryCreatedProjects();
    }
    else if (%sortType $= "Date")
    {
        queryCreatedProjects();
        easySort(CreatedProjectSet, projectDateSort);
    }
}

function AG_SortBox::onSelect(%this)
{
    %index = %this.getSelected();
    
    switch$(%index)
    {
        case 0:
            AG_GamesList.sortType = "Alphabetical";
            
        case 1:        
            AG_GamesList.sortType = "Date";
        
        case 2:
            AG_GamesList.sortType = "Template";
    }
    
    AG_GamesList.Refresh();
}

function AG_FilterBox::onSelect(%this)
{
    AG_GamesList.filter = %this.getText();
    AG_GamesList.Refresh();
}

function AG_BackButton::onClick(%this)
{
    EditorShellGui.clearViews();
    EditorShellGui.addView(TemplateListGui, "");
    EditorShellGui.addView(GamesLocationGui, "");
    if ( isObject(AG_GamesList.helpManager) )
    {
        AG_GamesList.helpManager.stop();
        AG_GamesList.helpManager.delete();
    }
}

function AG_GamesList::onWake(%this)
{
    %this.Refresh();
    %this.helpManager = createHelpMarqueeObject("GameScreenHints", 10000, "{3SSHomeScreens}");
    %this.helpManager.openHelpSet("gameScreenHelp");
    %this.helpManager.start();
}

function AG_GamesList::onSleep(%this)
{
    if ( isObject(%this.helpManager) )
    {
        %this.helpManager.stop();
        %this.helpManager.delete();
    }
}

function AG_GamesList::ClearList( %this )
{
    // Clear our list
    while( %this.getCount() > 0 )
    {
        %object = %this.getObject( 0 );

        if( isObject( %object ) )
            %object.delete();
        else
            %this.remove( %object );
    }
}

function AG_GamesList::Refresh(%this)
{
    %this.ClearList();
    
    %projectFile = "/project.tssproj";
    
    sortProjectList(%this.sortType);
    
    for(%i = 0; %i < CreatedProjectSet.getCount(); %i++)
    {
        // Get the project from the list
        %gameProject = CreatedProjectSet.getObject(%i);
        %projectName = CreatedProjectSet.getObject(%i).projectName;

        // Get the template type
        %moduleId = %gameProject.sourceModule;  

        %templateModule = ModuleDatabase.getDefinitionFromId(%moduleId);
        
        if (%templateModule.description !$= %this.filter && %this.filter !$= "All Games")
            continue;
            
        %projectLocation = $UserGamesLocation @ "/" @ %gameProject.sourceModule;
        %project = %projectLocation @ "/" @ %projectName @ %projectFile;
            
        // Get the icon for this template
        %icon = %templateModule.Icon;

        // Set the GuiImageButtonCtrl bitmap fields based on the template icon
        %normalIcon = "{3SSHomeScreens}:" @ %icon @ "Small_normal";
        %downIcon = "{3SSHomeScreens}:" @ %icon @ "Small_down";
        %hoverIcon = "{3SSHomeScreens}:" @ %icon @ "Small_hover";
        %inactiveIcon = "{3SSHomeScreens}:" @ %icon @ "Small_inactive";
      
        // Create the game button
        %gameButton = new GuiImageButtonCtrl()
        {
            Profile = "GuiButtonProfile";
            Extent = "178 158";
            MinExtent = "178 158";
            buttonType = "PushButton";
            Position = "0 0";
            isLegacyVersion = 0;
            NormalImage = %normalIcon;
            HoverImage = %hoverIcon;
            InactiveImage = %inactiveIcon;
            DownImage = %downIcon;
            toolTipProfile="GuiToolTipProfile";
                toolTip="Select to open " @ %projectName @ " in 3 Step Studio.";
        };

        %gameButton.Command = "TemplateSelector::OpenProject(\"" @ %project @ "\");";
     
        %gameName = new GuiTextCtrl()
        {
            canSaveDynamicFields = "0";
            isContainer = "0";
            Profile = "GuiModelessTextProfile";
            HorizSizing = "right";
            VertSizing = "bottom";
            Position = "13 126";
            Extent = "95 25";
            MinExtent = "90 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
            text = %projectName;
            maxLength = "1024";
            truncate = true;
        };
     
        %gameButton.add(%gameName);

        // Create the game button
        %optionsButton = new GuiImageButtonCtrl()
        {
            Profile = "GuiTransparentProfile";
            Extent = "35 35";
            MinExtent = "1 1";
            buttonType = "PushButton";
            Position = "128 120";
            isLegacyVersion = 0;
            NormalImage = "{3SSHomeScreens}:optionsGearNormal";
            HoverImage = "{3SSHomeScreens}:optionsGearHover";
            InactiveImage = "{3SSHomeScreens}:optionsGearInactive";
            DownImage = "{3SSHomeScreens}:optionsGearDepressed";
            toolTipProfile="GuiToolTipProfile";
            toolTip="Select to open the Options Pop Up Screen.";
        };
        
        %optionsButton.command = "showProjectOptionsDialog(" @ %gameProject @ ", " @ %this @ ");";
        
        %gameButton.add(%optionsButton);
        
        // Add the game button to the list
        %this.add(%gameButton);
    }
}