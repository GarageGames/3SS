//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// Pauses the Game and Displays the Help Dialog
/// </summary>
function helpGui::onWake(%this)
{
    %this.setup();
}

function helpGui::setup(%this)
{
    // position the help button so it displays with a "cool overhang" look.
    %helpDisplayPos = helpScreenDisplay.getPosition();

    // Need to get a list of help images from the level file so we can display 
    // them sequentially at the user's request.
    %scene = sceneWindow2D.getScene();
    %this.allRead = false;
    if (%this.image[0] $= "" || %this.imageCount == 0)
    {
        %this.imageCount = 0;
        %this.loadHelpData(%scene);
    }
    if (%this.tutorial == false)
        Canvas.popDialog(%this);
        
    %this.currentImage = 0;
    helpScreenDisplay.setImage(%this.image[%this.currentImage]);
}

function helpGui::clear(%this)
{
    %check = %this.image[0];
    %index = 0;
    while (%check !$= "")
    {
        %this.image[%index] = "";
        %index++;
        if (%index > 50)
            break;
        %check = %this.image[%index];
    }
    %this.imageCount = 0;
}

/// <summary>
/// Checks to see if HelpObjectList exists in the current level.  If it does, 
/// it walks the list of objects in the list and looks for the associated tutorial
/// image list object(s) and then attaches the image names to the help GUI for 
/// later use.
/// </summary>
function helpGui::loadHelpData(%this, %scene)
{
    if (isObject($TutorialDataSet))
        $TutorialDataSet.delete();

    $TutorialDataSet = TamlRead("^PhysicsLauncherTemplate/managed/tutorialData.taml");
    loadTutorialData($TutorialDataSet);
    
    %this.levelSet = new SimSet();
    // check for this level's object tutorial list.
    if (isObject(%scene))
    {
        %sceneObjectList = %scene.getSceneObjectList();
        for (%i = 0; %i < getWordCount(%sceneObjectList); %i++)
        {
            %obj = getWord(%sceneObjectList, %i);
            %tutorialName = %obj.getPrefab() @ "Tutorial";
            if (%tutorialName !$= "Tutorial")
            {
                %this.tutorial = true;
                %tutorial = $TutorialDataSet.findObjectByInternalName(%tutorialName);
                if (isObject(%tutorial))
                {
                    %this.levelSet.add(%tutorial);
                }
            }
        }
        for (%i = 0; %i < 5; %i++)
        {
            %tutorialName = MainScene.AvailProjectile[%i] @ "Tutorial";
            if (%tutorialName !$= "Tutorial")
            {
                %this.tutorial = true;
                %tutorial = $TutorialDataSet.findObjectByInternalName(%tutorialName);
                if (isObject(%tutorial))
                {
                    %this.levelSet.add(%tutorial);
                }
            }
        }
        for (%i = 0; %i < %this.levelSet.getCount(); %i++)
        {
            %tutObj = %this.levelSet.getObject(%i);
            %tutObj.TutorialRead = true;
            %this.image[%i] = %tutObj.Image[0];
            %updateTutorialFile = true;
        }

        if (%this.image[0] !$= "")
            helpScreenDisplay.setImage(%this.image[0]);
        
        if (%updateTutorialFile)
        {
            TamlWrite($TutorialDataSet, "^PhysicsLauncherTemplate/managed/tutorialData.taml");
            saveTutorialData($TutorialDataSet);
        }

        %this.imageCount = %this.levelSet.getCount();
        %this.currentImage = 0;
    }
    
    if (%this.allRead)
        Canvas.popDialog(%this);
}

/// <summary>
/// Advances through the available tutorial images.
/// </summary>
function helpGui::incrementHelpImage(%this)
{
    // check to see if there are any more image in the list.  If so, display
    // the next one.  If not, clear the help screen.
    %this.currentImage++;
    if (%this.currentImage < %this.imageCount)
    {
        helpScreenDisplay.setImage(%this.image[%this.currentImage]);
    }
    else
        Canvas.popDialog(%this);
}

/// <summary>
/// Handles the OK button click, advancing to the next tutorial image or closing the 
/// help GUI if there are no more images to display.
/// </summary>
function helpOKButton::onClick(%this)
{
    // check to see if there are any more image in the list.  If so, display
    // the next one.  If not, clear the help screen.
    if (helpGui.currentImage < helpGui.imageCount - 1)
    {
        helpGui.currentImage++;
        helpScreenDisplay.setImage(helpGui.image[helpGui.currentImage]);
    }
    else
        Canvas.popDialog(helpGui);
}