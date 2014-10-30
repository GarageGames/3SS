//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function ExerciseManagerGui::onWake(%this)
{
    if (!isObject(%this.helpManager))
    {
        %this.helpManager = createHelpMarqueeObject("ExerciseManagerHints", 10000, "{3SSHomeScreens}");
    }

    %this.helpManager.openHelpSet("exerciseManagerHelp");
    %this.helpManager.start();

    if (isObject(ExerciseListGui.helpManager))
    {
        ExerciseListGui.helpManager.stop();
        ExerciseListGui.helpManager.delete();
    }
}

function ExerciseManagerGui::onSleep(%this)
{
    if (isObject(%this.helpManager))
    {
        %this.helpManager.stop();
        %this.helpManager.delete();
    }
}

function openExerciseManager(%exerciseType, %exerciseIcon)
{
    EMG_ExercisesList.type = %exerciseType;
    EMG_ExercisesList.exerciseIcon = %exerciseIcon;

    // Clear all the views from the shell
    EditorShellGui.clearViews();
    
    // Push the exercise selector view
    EditorShellGui.addView(ExerciseManagerGui, "");
    EditorShellGui.addView(GamesLocationGui, "");
}

function EMG_BackButton::onClick(%this)
{
    Hs_HomeButton.onClick();
}

function EMG_ExercisesList::onWake(%this)
{
    %this.Refresh();
}

function EMG_ExercisesList::ClearList( %this )
{
    // Clear our list
    while (%this.getCount() > 0)
    {
        %object = %this.getObject(0);

        if (isObject(%object))
        {
            %object.delete();
        }
        else
        {
            %this.remove(%object);
        }
    }
}

function EMG_ExercisesList::Refresh(%this)
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
        %moduleId = %gameProject.type;  

        %template = %this.type;

        // Check to see if project template matches the filter
        if (%template $= %moduleId)
        {
            // Get the icon for this exercise
            %icon = %this.exerciseIcon;

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

    %createNewButton.command = "newExercise(\"" @ EMG_ExercisesList.type @ "\");";

    %buttonContainer.add(%createNewButton);

    // Add the "Create New" button to the list
    %this.add(%buttonContainer);
}