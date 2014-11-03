//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This function creates a tutorial image set object for game entity tutorials.
/// </summary>
/// <param name="name">The name of the object that this tutorial is for</param>
/// <param name="imageList">A space-separated list of image filenames for the tutorial</param>
function TutorialDataBuilder::createTutorial(%name, %imageList)
{
    if (!isObject($TutorialDataSet))
        $TutorialDataSet = TamlRead("^PhysicsLauncherTemplate/managed/tutorialData.taml");
    if (!isObject($TutorialDataSet))
        $TutorialDataSet = new SimSet(TutorialData);

    %tutorialName = %name @ "Tutorial";
    %old = TutorialDataBuilder::getTutorial(%tutorialName);
    if (isObject(%old))
        $TutorialDataSet.remove(%old);

    %tutorial = new ScriptObject(%tutorialName);
    %tutorial.internalName = %tutorialName;
    
    // All tutorial images go in the template/gui/images folder, so...
    for (%i = 0; %i < getWordCount(%imageList); %i++)
    {
        %tutorial.Image[%i] = getWord(%imageList, %i);
    }
    %tutorial.TutorialRead = "0";
    
    $TutorialDataSet.add(%tutorial);
    TamlWrite($TutorialDataSet, "^PhysicsLauncherTemplate/managed/tutorialData.taml");
    saveTutorialData($TutorialDataSet);
}

function TutorialDataBuilder::getTutorial(%name)
{
    if (!isObject($TutorialDataSet))
        $TutorialDataSet = TamlRead("^PhysicsLauncherTemplate/managed/tutorialData.taml");

    return $TutorialDataSet.findObjectByInternalName(%name);
}

function TutorialDataBuilder::getTutorialAsset(%name)
{
    if (!isObject($TutorialDataSet))
        $TutorialDataSet = TamlRead("^PhysicsLauncherTemplate/managed/tutorialData.taml");

    %tutorialName = %name @ "Tutorial";
    %tutorial = TutorialDataBuilder::getTutorial(%tutorialName);
    if (isObject(%tutorial))
        %asset = %tutorial.Image[0];
    else
        return "{EditorAssets}:noImageImageMap";

    %temp = AssetDatabase.acquireAsset(%asset);
    %type = %temp.getClassName();
    AssetDatabase.releaseAsset(%asset);
    if (%type $= "ImageAsset")
        return %asset;

    return "{EditorAssets}:noImageImageMap";
}