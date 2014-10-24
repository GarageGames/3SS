//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function TSTemplateList::onWake(%this)
{
    %this.ClearList();
    %this.Refresh();
}

function TemplateListGui::onWake(%this)
{
    if ( !isObject(%this.helpManager) )
        %this.helpManager = createHelpMarqueeObject("HomeScreenHints", 10000, "{3SSHomeScreens}");

    %this.helpManager.openHelpSet("homeScreenHelp");
    %this.helpManager.start();

    if ( isObject(GameManagerGui.helpManager) )
    {
        GameManagerGui.helpManager.stop();
        GameManagerGui.helpManager.delete();
    }
}

function TemplateListGui::onSleep(%this)
{
    if ( isObject(%this.helpManager) )
    {
        %this.helpManager.stop();
        %this.helpManager.delete();
    }
}

function TSTemplateList::ClearList( %this )
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

function TSTemplateList::Refresh(%this)
{
    if($templateCount <= 0)
        parseTemplates();

    for(%i = 0; %i < $templateCount; %i++)
    {
        %icon = $templateList[%i].icon;
        %moduleID = $templateList[%i].moduleID;
        %description = stripTrailingSpaces($templateList[%i].description);
        
        if(%name $= "Empty")
            continue;

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
        
        %templateButton = new GuiImageButtonCtrl()
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
            toolTip="Press to open the default " @ %description @ " template for editing.";
        };
      
        %templateName = new GuiTextCtrl()
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
      
        %buttonContainer.add(%templateName);

        %templateButton.command = "openGamesManager(\"" @ %moduleID @ "\", " @ %icon @ ");";

        %buttonContainer.add(%templateButton);
        
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
        toolTip="More templates are under development and coming soon.";
        NormalImage = "{3SSHomeScreens}:ComingSoonIconNormal";
        HoverImage = "{3SSHomeScreens}:ComingSoonIconHilight";
        InactiveImage = "{3SSHomeScreens}:ComingSoonIconInactive";
        DownImage = "{3SSHomeScreens}:ComingSoonIconDepressed";
    };

    %buttonContainer.add(%comingSoonButton);
    
    %this.add(%buttonContainer);

    %comingSoonButton.setActive(0);
}