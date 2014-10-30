//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2014
//-----------------------------------------------------------------------------

function ELG_ExercisesList::onWake(%this)
{
    %this.ClearList();
    %this.Refresh();
}

function ExerciseListGui::onWake(%this)
{
    %this.helpManager = createHelpMarqueeObject("ExerciseListHints", 10000, "{3SSHomeScreens}");
    %this.helpManager.openHelpSet("exerciseListHelp");
    %this.helpManager.start();
}

function ExerciseListGui::onSleep(%this)
{
    if (isObject(%this.helpManager))
    {
        %this.helpManager.stop();
        %this.helpManager.delete();
    }
}

function ELG_ExercisesList::ClearList( %this )
{
   // Clear our list
   while(%this.getCount() > 0)
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

function ELG_ExercisesList::Refresh(%this)
{
    if ($exerciseCount <= 0)
    {
        parseExercises();
    }

    for (%i = 0; %i < $exerciseCount; %i++)
    {
        %icon = $exerciseList[%i].icon;
        %moduleID = $exerciseList[%i].moduleID;
        %description = stripTrailingSpaces($exerciseList[%i].description);
        
        if (%name $= "Empty")
        {
            continue;
        }

        %normalIcon = "{3SSHomeScreens}:" @ %icon @ "_normal";
        %downIcon = "{3SSHomeScreens}:" @ %icon @ "_down";
        %hoverIcon = "{3SSHomeScreens}:" @ %icon @ "_hover";
        %inactiveIcon = "{3SSHomeScreens}:" @ %icon @ "_inactive";

        %buttonContainer = new GuiControl()
        {
            canSaveDynamicFields = "0";
            isContainer = "1";
            Profile = "GuiTransparentProfile";
            HorizSizing = "right";
            VertSizing = "bottom";
            Position = "0 0";
            Extent = "166 190";
            MinExtent = "8 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
        };
        
        %exerciseButton = new GuiImageButtonCtrl()
        {
            Profile = "GuiTransparentProfile";
            Extent = "186 188";
            MinExtent = "140 140";
            Position = "0 0";
            buttonType = "PushButton";
            isLegacyVersion = 0;
            NormalImage = %normalIcon;
            HoverImage = %hoverIcon;
            InactiveImage = %inactiveIcon;
            DownImage = %downIcon;
            toolTipProfile="GuiToolTipProfile";
            toolTip="Press to open the default " @ %description @ " exercise for editing.";
        };
      
        %exerciseName = new GuiTextCtrl()
        {
            canSaveDynamicFields = "0";
            isContainer = "0";
            Profile = "GuiTextCenterProfile";
            HorizSizing = "right";
            VertSizing = "bottom";
            Position = "15 192";
            Extent = "140 15";
            MinExtent = "90 2";
            canSave = "1";
            Visible = "1";
            hovertime = "1000";
            text = %description;
            maxLength = "1024";
            truncate = "1";
        };
      
        %buttonContainer.add(%exerciseName);

        %exerciseButton.command = "openExerciseManager(\"" @ %moduleID @ "\", " @ %icon @ ");";

        %buttonContainer.add(%exerciseButton);
        
        %this.add(%buttonContainer);
    }
   
    %buttonContainer = new GuiControl()
    {
        canSaveDynamicFields = "0";
        isContainer = "1";
        Profile = "GuiTransparentProfile";
        HorizSizing = "right";
        VertSizing = "bottom";
        Position = "0 0";
        Extent = "166 190";
        MinExtent = "8 2";
        canSave = "1";
        Visible = "1";
        hovertime = "1000";
    };
    
    %comingSoonButton = new GuiImageButtonCtrl()
    {
        Profile = "GuiTransparentProfile";
        Extent = "186 188";
        MinExtent = "186 188";
        buttonType = "PushButton";
        isLegacyVersion = 0;
        toolTipProfile="GuiToolTipProfile";
        toolTip="More exercises are under development and coming soon.";
        NormalImage = "{3SSHomeScreens}:ComingSoonIconNormal";
        HoverImage = "{3SSHomeScreens}:ComingSoonIconHilight";
        InactiveImage = "{3SSHomeScreens}:ComingSoonIconInactive";
        DownImage = "{3SSHomeScreens}:ComingSoonIconDepressed";
    };

    %buttonContainer.add(%comingSoonButton);
    
    %this.add(%buttonContainer);

    %comingSoonButton.setActive(0);
}