//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// helpMarquee is an object for managing a tool's hints and tips that show up
// in the help container at the bottom of Three Step Studio's main window.
//-----------------------------------------------------------------------------

/// <summary>
/// This is a "factory" function that sets up a new HelpMarquee object.
/// </summary>
/// <param name="name">The name of the new object (optional).</param>
/// <param name="updateTime">The time in milliseconds between changes to the help text.</param>
/// <param name="moduleName">The name of the module this help manager will belong to.  For example, "{AssetLibrary}".</param>
/// <return>Returns the new help marquee manager object.</return>
function createHelpMarqueeObject(%name, %updateTime, %moduleName)
{
    %object = new ScriptObject()
    {
        class = "HelpMarquee";
    };
    %object.setName(%name);
    %object.updateFrequency = %updateTime;
    %object.tipTarget = "ESToolTipsShell";
    %object.moduleName = %moduleName;
    %object.helpPage = "docs.3stepstudio.com/";
    return %object;
}

/// <summary>
/// This opens the taml file with the hint data that you want to display for the 
/// current tool.
/// </summary>
/// <param name="setFile">The file base name of the new help data file.</param>
function HelpMarquee::openHelpSet(%this, %setFile)
{
    %fileName = expandPath("^" @ %this.moduleName @ "/data/help/" @ %setFile @ ".taml");
    %this.helpSet = TamlRead(%fileName);
    %this.tipCount = %this.helpSet.getCount();
}

/// <summary>
/// This sets the URL of the online documentation to open when the More Help button
/// is clicked.  To use this, it must be set before calling ::start() - ::start() 
/// creates and assigns the .command for the More Help button.
/// </summary>
/// <param name="page">The URL of the online documentation. The default is "docs.3StepStudio.com/".</param>
function HelpMarquee::setHelpPage(%this, %page)
{
    %this.helpPage = %page;
}

/// <summary>
/// This starts the Marquee.  First it checks to see if another help manager has
/// placed a text field in the hints and tips container and clears it.  Next, it
/// creates its own text control and adds it to the container.  It then sets the
/// action commands for the Previous Tip, Next Tip and More Help buttons.  Finally,
/// it selects a random tip to display and starts the update schedule.
/// </summary>
function HelpMarquee::start(%this)
{
    %this.createTextField();

    %this.setupHelpButtons();

    %tip = %this.getRandomTip();
    %this.textField.setText(%tip.Text);
    %this.nextTipEvent = %this.schedule(%this.updateFrequency, "update");
}

/// <summary>
/// This handles setup for the Next Tip, Previous Tip and More Help buttons.
/// If the help manager is unnamed, it will be given a name based on the 
/// name of the help set.  If the set is unnamed, a name will be created from 
/// the module name.
/// </summary>
function HelpMarquee::setupHelpButtons(%this)
{
    %name = %this.getName();

    if ( %name !$= "" )
    {
        %nextCmd = %name @ ".getNextTip();";
        %prevCmd = %name @ ".getPrevTip();";
    }
    else
    {
        %name = %this.helpSet.getName() @ "Manager";
        if ( %name !$= "")
        {
            %this.setName(%name);
            %nextCmd = %name @ ".getNextTip();";
            %prevCmd = %name @ ".getPrevTip();";
        }
        else
        {
            %temp = strreplace(%this.moduleName, "{", "");
            %name = strreplace(%temp, "}", "");
            %newName = %name @ %this.getId() @ "HelpManager";
            %this.setName(%newName);
            %nextCmd = %newName @ ".getNextTip();";
            %prevCmd = %newName @ ".getPrevTip();";
        }
    }

    %this.nextBtn = "ESNextTipButton";
    %this.nextBtn.command = %nextCmd;

    %this.prevBtn = "ESPreviousTipButton";
    %this.prevBtn.command = %prevCmd;

    ESMoreHelpButton.command="gotoWebPage(\"http://" @ %this.helpPage @ "\");";
}

/// <summary>
/// This handles creating a new multi-line text control, sizing it and adding it
/// to the hints and tips container.
/// </summary>
function HelpMarquee::createTextField(%this)
{
    %objCount = %this.tipTarget.getCount();
    if ( %objCount > 0 )
    {
        for (%i = 0; %i < %objCount; %i++)
            %this.tipTarget.getObject(%i).delete();
    }

    %this.textField = new GuiMLTextCtrl()
    {
        canSaveDynamicFields="1";
        isContainer="0";
        Profile="GuiTextProfile";
        HorizSizing="right";
        VertSizing="bottom";
        Position="1 1";
        MinExtent="12 12";
        canSave="1";
        Visible="1";
        Active="1";
        hovertime="1000";
        lineSpacing="4";
        allowColorChars="0";
        maxChars="-1";
    };
    %this.tipTarget.addGuiControl(%this.textField);
    %this.textField.setExtent(%this.tipTarget.Extent.x - 2, %this.tipTarget.Extent.y - 2);
}

/// <summary>
/// This updates the hint text with a new random tip and then schedules the next
/// update.
/// </summary>
function HelpMarquee::update(%this)
{
    %tip = %this.getRandomTip();
    if ( !isObject(%this.textField) )
        %this.createTextField();
    %this.textField.setText(%tip.Text);
    %this.nextTipEvent = %this.schedule(%this.updateFrequency, "update");
}

/// <summary>
/// This function gets a tip from the manager's data set selected by index.
/// </summary>
/// <param name="index">The index of the desired tip.</param>
/// <return>Returns the tip at the selected index.</return>
function HelpMarquee::getTip(%this, %index)
{
    %this.currentTip = %index;
    if ( (%this.currentTip) >= %this.tipCount )
    {
        %this.currentTip = 0;
    }
    if ( (%this.currentTip) < 0 )
    {
        %this.currentTip = %this.tipCount - 1;
    }
    %tip = %this.helpSet.getObject(%this.currentTip);
    return %tip;
}

/// <summary>
/// This function gets the next tip in the set after the currently displayed tip.
/// </summary>
/// <return>Returns the tip at the current index + 1.  Wraps around the ends.</return>
function HelpMarquee::getNextTip(%this)
{
    cancel(%this.nextTipEvent);
    %this.currentTip++;
    if ( (%this.currentTip) >= %this.tipCount )
    {
        %this.currentTip = 0;
    }

    %tip = %this.helpSet.getObject(%this.currentTip);
    %this.textField.setText(%tip.Text);
    %this.nextTipEvent = %this.schedule(%this.updateFrequency, "update");
}

/// <summary>
/// This function gets the previous tip in the set before the currently displayed tip.
/// </summary>
/// <return>Returns the tip at the current index - 1.  Wraps around the ends.</return>
function HelpMarquee::getPrevTip(%this)
{
    cancel(%this.nextTipEvent);
    %this.currentTip--;
    if ( (%this.currentTip) < 0 )
    {
        %this.currentTip = %this.tipCount - 1;
    }

    %tip = %this.helpSet.getObject(%this.currentTip);
    %this.textField.setText(%tip.Text);
    %this.nextTipEvent = %this.schedule(%this.updateFrequency, "update");
}

/// <summary>
/// This function gets a random tip and it ensures that it does not repeat tips
/// consecutively - i.e. it won't display the same tip twice in a row.
/// </summary>
/// <return>Returns a random tip from the set.</return>
function HelpMarquee::getRandomTip(%this)
{
    if (%this.tipCount == 0)
    {
        return -1;
    }
    
    %index = "";
    
    if (%this.tipCount == 1)
    {
        %index = 0;
    }
    else
    {
        while (%this.currentTip == %index)
        {
            %index = getRandom(%this.tipCount - 1);
        }
    }
    
    %this.currentTip = %index;
    %tip = %this.helpSet.getObject(%index);
    
    
    return %tip;
}

/// <summary>
/// This function stops the update schedule and clears out the manager's text field.
/// </summary>
function HelpMarquee::stop(%this)
{
    cancel(%this.nextTipEvent);
    if ( isObject(%this.textField) )
        %this.textField.delete();
}