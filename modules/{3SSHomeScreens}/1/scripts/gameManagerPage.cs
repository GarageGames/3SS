//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function GameManagerGui::onWake(%this)
{
    if ( !isObject(%this.helpManager) )
        %this.helpManager = createHelpMarqueeObject("GameScreenHints", 10000, "{3SSHomeScreens}");

    %this.helpManager.openHelpSet("gameScreenHelp");
    %this.helpManager.start();

    if ( isObject(TemplateListGui.helpManager) )
    {
        TemplateListGui.helpManager.stop();
        TemplateListGui.helpManager.delete();
    }
}

function GameManagerGui::onSleep(%this)
{
    if ( isObject(%this.helpManager) )
    {
        %this.helpManager.stop();
        %this.helpManager.delete();
    }
}

function openGamesManager(%templateType, %templateIcon)
{
    GMGamesList.type = %templateType;
    GMGamesList.templateIcon = %templateIcon;

    // Clear all the views from the shell
    EditorShellGui.clearViews();
    
    // Push the template selector view
    EditorShellGui.addView(GameManagerGui, "");
    EditorShellGui.addView(GamesLocationGui, "");
}

function GMBackButton::onClick(%this)
{
    Hs_HomeButton.onClick();
}

function GMGamesList::onWake(%this)
{
    %this.Refresh();
}

function GMGamesList::ClearList( %this )
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

function GMGamesList::Refresh(%this)
{
    %this.ClearList();
    
    // All projects should be created in this specific location (user app folder), so set the path
    %projectLocation = $UserGamesLocation;

    %projectFile = "/project.tssproj";
    
    %projectFileSpec = %projectLocation @ "/*.tssproj";
    
    addResPath(%projectLocation);

    for (%file = findFirstFile(%projectFileSpec); %file !$= ""; %file = findNextFile(%projectFileSpec))
    {
        %gameProject = TamlRead(%file);
        %projectName = %gameProject.projectName;

        // Get the template type
        %moduleId = %gameProject.sourceModule;  

        %template = %this.type;

        // Check to see if project template matches the filter
        if(%template $= %moduleId)
        {
            %templateModule = ModuleDatabase.getDefinitionFromId(%moduleId);
            
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

            %gameButton.Command = "TemplateSelector::OpenProject(\"" @ %file @ "\");";
         
            %gameName = new GuiTextCtrl()
            {
                canSaveDynamicFields = "0";
                isContainer = "0";
                Profile = "GuiModelessTextProfile";
                HorizSizing = "right";
                VertSizing = "bottom";
                Position = "13 126";
                Extent = "100 25";
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
                Position = "138 120";
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
    
    removeResPath(%projectLocation);

    %buttonContainer = new GuiControl()
    {
        canSaveDynamicFields = "0";
        isContainer = "1";
        Profile = "GuiTransparentProfile";
        HorizSizing = "right";
        VertSizing = "bottom";
        Position = "-17 433";
        Extent = "140 170";
        MinExtent = "8 2";
        canSave = "1";
        Visible = "1";
        hovertime = "1000";
    };

    // Make a "Create New" button
    %createNewButton = new GuiImageButtonCtrl()
    {
        Profile = "GuiTransparentProfile";
        Extent = "178 158";
        MinExtent = "140 140";
        buttonType = "PushButton";
        isLegacyVersion = 0;
        hovertime = "1000";
        toolTipProfile="GuiToolTipProfile";
        toolTip="Select to open the New Game Pop Up Screen.";
        NormalImage = "{3SSHomeScreens}:newGame_normal";
        HoverImage = "{3SSHomeScreens}:newGame_hover";
        InactiveImage = "{3SSHomeScreens}:newGame_inactive";
        DownImage = "{3SSHomeScreens}:newGame_down";
    };

    %createNewButton.command = "newProject(\"" @ GMGamesList.type @ "\");";

    %buttonContainer.add(%createNewButton);

    // Add the "Create New" button to the list
    %this.add(%buttonContainer);
}