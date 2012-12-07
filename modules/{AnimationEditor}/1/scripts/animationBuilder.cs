//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function AnimationBuilder::onAdd(%this)
{
    %this.tempAnimation = "";
    %this.newAnimation = false;
    %this.animationIsDirty = false;
    %this.sourceImageMap = "";
    %this.animationName = "";
    
    // Default animation values.
    %this.defaultFPS = 30;
    %this.defaultAnimationCycle = true;
    %this.defaultRandomStart = false;
    %this.defaultName = "Animation";
    %this.defaultImageMapName = "Sprite";

    // Bounds
    %this.maxFPS = $MaxFPS;
    %this.minFPS = $MinFPS;

    // The scene to use for the drag and drop controls.
    %this.draggingScene = new Scene();
}

function AnimationBuilder::updateGui(%this)
{
    ABTagList.refresh(0);
    ABTagContainer.populateTagList(%this.animationId);

    if (AssetDatabase.isDeclaredAsset(%this.tempAnimation))
    {
        %tempAnim = AssetDatabase.acquireAsset(%this.tempAnimation);
        %image = %tempAnim.getImageMap();
        %frames = %tempAnim.AnimationFrames;
        %numFrames = getWordCount(%frames);
        if ( %numFrames <= 0 )
            %frames = "";

        AnimBuilderEventManager.postEvent("_StoryboardContentPaneUpdateComplete", ABStoryboardWindow @ " " @ %image @ " " @ %frames);
        ABCycleAnimationCheck.setValue(%tempAnim.AnimationCycle);
        %tempName = ABNameField.getText();
        if (%this.animationName !$= %tempAnim.AssetName && %this.animationName $= "")
        {
            if (%tempName $= "")
                ABNameField.initialize($ABAssetNameTextEditMessageString);
            else
                %this.animationName = %tempName;
            %this.nameDirty = false;
        }
        if ( %this.animationName $= $ABAssetNameTextEditMessageString && %tempName !$= $ABAssetNameTextEditMessageString )
        {
            %this.animationName = %tempName;
            %this.nameDirty = false;
        }
        if ( %this.nameDirty )
        {
            ABNameField.onReturn();
            %tempName = ABNameField.getText();
            %this.animationName = %tempName;
            %this.nameDirty = false;
        }
        ABNameField.setText(%this.animationName);

        ABImageMapField.setText(AssetDatabase.getAssetName(%this.sourceImageMap));

        if ( ABImageMapField.getText() $= "" )
            ABImageMapField.initialize($ABAssetNameTextEditMessageString);

        if (%numFrames > 0)
        {
            ABFPSField.setText(%this.fps);
            AnimBuilderEventManager.postEvent("_AnimPreviewUpdateRequest", "");
        }
        else
        {
            ABFPSField.setText(mCeil(%this.defaultFPS));
            if (AssetDatabase.isDeclaredAsset(%this.sourceImageMap))
                ABAnimationPreviewWindow.setImage(%this.sourceImageMap);
            else
                ABAnimationPreviewWindow.setImage("{EditorAssets}:transparent");
        }
        AssetDatabase.releaseAsset(%this.tempAnimation);
    }
    
    ABSaveButton.update();
    %this.updating = false;
}

function AnimationBuilder::updatePreviewAnimation(%this)
{
    %anim = AssetDatabase.AcquireAsset(%this.tempAnimation);
    %frameCount = getWordCount(%anim.AnimationFrames);
    AssetDatabase.releaseAsset(%this.tempAnimation);
    if ( %frameCount > 0 )
        ABAnimationPreviewWindow.setAnimation(%this.tempAnimation);
    else
        ABAnimationPreviewWindow.setImage(%this.sourceImageMap);
}

function AnimationBuilder::onRemove(%this)
{
    if (isObject(%this.draggingScene))
        %this.draggingScene.delete();
}

function AnimationBuilder::open(%this)
{
    %this.animationIsDirty = false;
    %this.initializing = true;

    if (AssetDatabase.isDeclaredAsset(%this.tempAnimation))
    {
        %temp = AssetDatabase.acquireAsset(%this.tempAnimation);
        if (getWordCount(%temp.animationFrames) > 0)
        {
            %this.validateFPS();
            Ab_ImageMapPreviewWindow.display(%this.sourceImageMap);
        }
        AssetDatabase.releaseAsset(%this.tempAnimation);
        ABTagContainer.populateTagList(%this.animationId);
    }
    else if (isObject(%this.sourceImageMap))
        %this.generateTemporaryAnimation();
    else
        warn("AnimationBuilder::open - no animation to edit.");

    Canvas.pushDialog(AnimationBuilderGui);

    %this.updateGui();
    %this.initializing = false;
}

function AnimationBuilder::close(%this)
{
    Canvas.popDialog(AnimationBuilderGui);
    Ab_ImageMapPreviewWindow.clear();
    ABAnimationPreviewWindow.setImage(%this.sourceImageMap);

    %this.newAnimation = false;

    if (AssetDatabase.isDeclaredAsset(%this.tempAnimation))
        %this.tempAnimation.delete();

    %this.animationName = "";
    %this.tempAnimation = "";
    %this.sourceImageMap = "";
    %this.animationIsDirty = false;

    cleanTemporaryAssets();

    $AssetAutoTag = "";
}

function AnimationBuilder::createAnimation(%this, %tag)
{
    // We want celled and linked image maps
    %this.newAnimation = true;

    if (isObject(ABStoryBoardWindow.staticSpriteGroup))
        ABStoryBoardWindow.staticSpriteGroup.deleteContents();

    Ab_ImageMapPreviewWindow.clear();
    ABStoryboardWindow.clear();
    $AssetAutoTag = %tag;

    %this.newAnimation();
}

function AnimationBuilder::newAnimation(%this)
{
    cleanTemporaryAssets();

    %this.newAnimation = true;
    %this.animationId = "";
    %this.animationName = "";
    %this.generateTemporaryAnimation();
    %this.animationId = %this.tempAnimation;
    ABNameField.initialize($ABAssetNameTextEditMessageString);
    ABImageMapField.initialize($ABAssetNameTextEditMessageString);
    %this.open();
}

function AnimationBuilder::editAnimation(%this, %animationAssetId)
{
    if (%this.tempAnimation !$= "")
    {
        AssetDatabase.releaseAsset(%this.tempAnimation);
        %this.tempAnimation = "";
    }
    Ab_ImageMapPreviewWindow.clear();

    %this.newAnimation = false;

    %assetDatablock = AssetDatabase.acquireAsset(%animationAssetId);

    %this.animationId = %animationAssetId;
    %this.animationName = %assetDatablock.AssetName;
    %this.sourceImageMap = %assetDatablock.imageMap;

    %this.generateTemporaryAnimation();
    %tempAnim = AssetDatabase.acquireAsset(%this.tempAnimation);
    %tempAnim.copy(%animationAssetId);
    AssetDatabase.releaseAsset(%this.tempAnimation);

    AssetDatabase.releaseAsset(%animationAssetId);
    
    %this.open();
}

function AnimationBuilder::copyAnimToTemp(%this, %objectToCopy)
{
    %targetAsset = AssetDatabase.acquireAsset(%this.tempAnimation);
    %targetAsset.ImageMap = %objectToCopy.ImageMap;
    %targetAsset.animationFrames = %objectToCopy.animationFrames;
    %targetAsset.animationTime = %objectToCopy.animationTime;
    %targetAsset.animationCycle = %objectToCopy.animationCycle;
    %targetAsset.randomStart = %objectToCopy.randomStart;

    %assetTagManifest = AssetDatabase.getAssetTags();
    %assetTagCount = %assetTagManifest.getAssetTagCount(%objectToCopy);
    for (%i = 0; %i < %assetTagCount; %i++)
    {
        %tag = %assetTagManifest.getAssetTag(%objectToCopy, %i);
        %assetTagManifest.tag(%targetAsset, %tag);
    }
    AssetDatabase.releaseAsset(%this.tempAnimation);
}

function AnimationBuilder::save(%this)
{
    %newAssetName = ABNameField.getText();
    %oldAssetName = %this.animationName;
    
    %path = expandPath("^{UserAssets}/animations/");
    
    if (%this.newAnimation)
    {
        // This is a new animation.  Save it to {UserAssets}
        %finalAsset = new AnimationAsset();
        %finalAsset.AssetName = %newAssetName;
        %newDefinitionPath = %path @ %newAssetName @ ".asset.taml";
        %moduleDefinition = ModuleDatabase.getDefinitionFromId("{UserAssets}");

        TamlWrite(%finalAsset, %newDefinitionPath);

        // Add the new asset as-is...
        AssetDatabase.addSingleDeclaredAsset(%moduleDefinition, %newDefinitionPath);
        // Acquire an instance of it from the database...
        %newAsset = AssetDatabase.acquireAsset("{UserAssets}:" @ %newAssetName);
        // Call ::copy() now - otherwise tags are not copied!
        %newAsset.copy(%this.tempAnimation);
        AssetDatabase.releaseAsset("{UserAssets}:" @ %newAssetName);
    }
    else
    {
        // We are modifying an existing asset.  Take appropriate action based on 
        // the origin of the asset.
        %module = AssetDatabase.getAssetModule(%this.animationId);
        if ( %module.ModuleId !$= "{UserAssets}" )
        {
            // This is a modified version of a stock asset.  Create a user copy
            // in {UserAssets}.
            %finalAsset = new AnimationAsset();
            %finalAsset.AssetName = %newAssetName;
            %newDefinitionPath = %path @ %newAssetName @ ".asset.taml";
            %moduleDefinition = ModuleDatabase.getDefinitionFromId("{UserAssets}");
            
            TamlWrite(%finalAsset, %newDefinitionPath);
            
            // Add the new asset as-is...
            AssetDatabase.addSingleDeclaredAsset(%moduleDefinition, %newDefinitionPath);
            // Acquire an instance of it from the database...
            %newAsset = AssetDatabase.acquireAsset("{UserAssets}:" @ %newAssetName);
            // Call ::copy() now - otherwise tags are not copied!
            %newAsset.copy(%this.tempAnimation);
            AssetDatabase.releaseAsset("{UserAssets}:" @ %newAssetName);
        }
        else
        {
            // This is a user asset.  Save changes.
            %originalDatablock = AssetDatabase.acquireAsset(%this.animationId);

            %originalDatablock.copy(%this.tempAnimation);

            // Editing existing asset, but user changed the name
            // We need to update it and its references
            if (%newAssetName !$= %oldAssetName)
            {
                %originalDatablock.rename(%newAssetName);
            }
        }
    }
    AssetDatabase.releaseAsset(%this.tempAnimation);
    AssetDatabase.removeSingleDeclaredAsset(%this.tempAnimation);
    AssetDatabase.releaseAsset(%this.animationId);

    AssetDatabase.saveAssetTags();

    AssetLibrary.schedule(100, "updateGui");

    %this.close();
}

function AnimationBuilder::cancel(%this)
{
    AssetDatabase.restoreAssetTags();
    if (AssetDatabase.isDeclaredAsset(%this.tempAnimation))
        AssetDatabase.removeSingleDeclaredAsset(%this.tempAnimation);
    %this.close();
}

function AnimationBuilder::validateCycleAnimation(%this)
{
    if (!AssetDatabase.isDeclaredAsset(%this.tempAnimation))
        return;

    %this.animationIsDirty = true;
    %asset = AssetDatabase.acquireAsset(%this.tempAnimation);
    %asset.animationCycle = ABCycleAnimationCheck.getValue();
    AssetDatabase.releaseAsset(%this.tempAnimation);
}

function AnimationBuilder::validateFPS(%this)
{
    if (!AssetDatabase.isDeclaredAsset(%this.tempAnimation))
        return;

    %fps = ABFPSField.getText();
    %fps = clamp(%fps, %this.minFPS, %this.maxFPS);

    %anim = AssetDatabase.acquireAsset(%this.tempAnimation);
    if (%anim.getFrameCount() < 1)
        return;

    %this.animationIsDirty = true;

    %anim.animationTime = %anim.getFrameCount() / %fps;
    AssetDatabase.releaseAsset(%this.tempAnimation);
    %this.fps = %fps;
    %this.updateGui();
}

function AnimationBuilder::validateName(%this)
{
    if (!AssetDatabase.isDeclaredAsset(%this.animationId))
        return;

    %assetId = %this.animationId;
    %tempAsset = AssetDatabase.acquireAsset(%assetId);
    %assetName = %tempAsset.AssetName;

    %name = ABNameField.getText();

    if (AssetDatabase.isDeclaredAsset(%name) && %name !$= %assetName)
    {
        NoticeGui.display("An animation with this name already exists. Please choose another");
        return;
    }
    else
    {
        if ( %name !$= %assetName )
            %this.animationId = %tempAsset.rename(%name);
    }

    if (%name !$= ABNameField.animName)
    {
        ABNameField.animName = %name;
        %this.animationIsDirty = true;
    }
    else
        %this.animationIsDirty = false;

    AssetDatabase.releaseAsset(%this.animationId);

    %this.animationName = %name;
    %this.nameDirty = false;
    ABNameField.onReturnFired = false;
    %this.updateGui();
}

function AnimationBuilder::insertFrame(%this, %frame, %position)
{
    if (!AssetDatabase.isDeclaredAsset(%this.tempAnimation))
        return;

    %this.animationIsDirty = true;

    %anim = AssetDatabase.acquireAsset(%this.tempAnimation);
    %frames = %anim.AnimationFrames;
    %frameCount = getWordCount(%frames);

    if (%position > %frameCount)
        %position = %frameCount;
    else if (%position < 0)
        %position = 0;

    %anim.animationTime = (%frameCount + 1) / ABFPSField.getText();

    %newFrames = "";
    for (%i = 0; %i <= %frameCount; %i++)
    {
        if (%i == %position)
            %newFrames = %newFrames SPC %frame;

        if (%i < %frameCount)
            %newFrames = %newFrames SPC getWord(%frames, %i);
    }

    %anim.AnimationFrames = trim(%newFrames);
    AssetDatabase.releaseAsset(%this.tempAnimation);

    AnimBuilderEventManager.postEvent("_AnimUpdateRequest", "");

    %scrollCtrl = ABStoryboardWindow.getParent();
    %scrollCtrl.setScrollPosition(%windowWidth * %position, 0);
}

function AnimationBuilder::removeFrame(%this, %position)
{
    if (!AssetDatabase.isDeclaredAsset(%this.tempAnimation))
        return;

    ABAnimationPreviewWindow.setImage(%this.sourceImageMap);
    AnimBuilderEventManager.postEvent("_TimelineDeleteRequest", %position);
}

function AnimationBuilder::completeFrameRemoval(%this, %position)
{
    %anim = AssetDatabase.acquireAsset(%this.tempAnimation);
    if ( %anim.getAnimationFrameCount() <= 0 )
    {
        AssetDatabase.releaseAsset(%this.tempAnimation);
        return;
    }

    %this.animationIsDirty = true;

    %frames = %anim.AnimationFrames;
    %count = getWordCount(%frames);

    %newFrames = "";
    for (%i = 0; %i < %count; %i++)
    {
        if (%i != %position)
        {
            %temp = %newFrames SPC getWord(%frames, %i);
            %newFrames = ltrim(%temp);
        }
    }

    %frameCount = getWordCount(%newFrames);

    %anim.AnimationTime = (%frameCount) / ABFPSField.getText();
    %anim.AnimationFrames = trim(%newFrames);
    AssetDatabase.releaseAsset(%this.tempAnimation);

    AnimBuilderEventManager.postEvent("_AnimUpdateRequest", "");
}

function AnimationBuilder::appendFrame(%this, %frame)
{
    if (!AssetDatabase.isDeclaredAsset(%this.tempAnimation))
        return;

    %anim = AssetDatabase.acquireAsset(%this.tempAnimation);
    %frames = %anim.getAnimationFrameCount();
    AssetDatabase.releaseAsset(%this.tempAnimation);

    %this.insertFrame(%frame, %frames);
}

function AnimationBuilder::setAllFrames(%this)
{
    if (!AssetDatabase.isDeclaredAsset(%this.tempAnimation))
        return;

    %this.clearFrames();

    AnimBuilderEventManager.postEvent("_SetTimelineRequest", "");
}

function AnimationBuilder::completeSetFrames(%this)
{
    %anim = AssetDatabase.acquireAsset(%this.tempAnimation);
    %image = AssetDatabase.acquireAsset(%anim.ImageMap);
    %frameCount = %image.getFrameCount();
    AssetDatabase.releaseAsset(%anim.ImageMap);
    for ( %i = 0; %i < %frameCount; %i++)
    {
        %frames = %frames SPC %i;
    }
    %anim.AnimationFrames = trim(%frames);
    AssetDatabase.releaseAsset(%this.tempAnimation);
    %this.validateFPS();

    AnimBuilderEventManager.postEvent("_AnimUpdateRequest", "");
}

function AnimationBuilder::clearFrames(%this)
{
    if (!AssetDatabase.isDeclaredAsset(%this.tempAnimation))
        return;

    ABAnimationPreviewWindow.setImage(%this.sourceImageMap);
    AnimBuilderEventManager.postEvent("_ClearTimelineRequest", "");
}

function AnimationBuilder::completeClearTimeline(%this)
{
    %anim = AssetDatabase.acquireAsset(%this.tempAnimation);
    %anim.AnimationFrames = "";
    AssetDatabase.releaseAsset(%this.tempAnimation);
    %this.validateFPS();
    AnimBuilderEventManager.postEvent("_AnimUpdateRequest", "");
}

function AnimationBuilder::populateTagContainer(%this)
{
   if (!AssetDatabase.isDeclaredAsset(%this.tempAnimation))
      return;
      
   %tagCount = getWordCount(%this.tempAnimation.NameTags);
   
   for(%i = 0; %i < %tagCount; %i++)
   {
      %tagID = getWord(%this.tempAnimation.NameTags, %i);
      
      %tag = ProjectNameTags.getTagName(%tagID);
      
      %verticalPosition = (30 * %i) + 5;
      %horizontalPosition = 7;
      %position = %horizontalPosition SPC %verticalPosition;
      
      CreateTagBar(ABTagContainer, %position, %tag);
   }
}

function AnimationBuilder::createDraggingControl(%this, %sprite, %spritePosition, %mousePosition, %size)
{
    if (!AssetDatabase.isDeclaredAsset(%this.tempAnimation))
        return;

    // Create the drag and drop control.
    %dragControl = new GuiDragAndDropControl()
    {
        Profile = "GuiDragAndDropProfile";
        Position = %spritePosition;
        Extent = %size;
        deleteOnMouseUp = true;
    };

    // And the sprite to display.
    %spritePane = new GuiSpriteCtrl()
    {
        scene = %this.draggingScene;
        Extent = %size;
        Image = %sprite.Image;
        Frame = %sprite.Frame;
    };
    %spritePane.frameNumber = %sprite.frameNumber;
    %spritePane.spriteClass = %sprite.class;

    // Place the guis.
    AnimationBuilderGui.add(%dragControl);
    %dragControl.add(%spritePane);

    // Figure the position to place the control relative to the mouse.
    %xOffset = getWord(%mousePosition, 0) - getWord(%spritePosition, 0);
    %yOffset = getWord(%mousePosition, 1) - getWord(%spritePosition, 1);

    %dragControl.startDragging(%xOffset, %yOffset);
}

function AnimationBuilder::addTag(%this, %tag)
{
    if (%tag $= "")
        return;

    %assetTagManifest = AssetDatabase.getAssetTags();
    if ( AssetDatabase.isDeclaredAsset(%this.animationId) )
        %assetID = %this.animationId;
    else if ( AssetDatabase.isDeclaredAsset(%this.tempAnimation) )
        %assetID = %this.tempAnimation;
    else
    {
        echo(" @@@ Animation Builder::addTag():  Attempting to add a tag when no valid asset exists");
        return;
    }
    %assetTagCount = %assetTagManifest.getAssetTagCount(%assetID);
    for (%i = 0; %i < %assetTagCount; %i++)
    {
        %tempTag = %assetTagManifest.getAssetTag(%assetID, %i);
        if (%tag $= %tempTag)
        {
            echo(" @@@ asset already has tag " @ %tag);
            return;
        }
    }
    %assetTagManifest.tag(%assetID, %tag);
    ABTagContainer.addTagItem(%tag);
    ABTagContainer.populateTagList(%assetID);
}

function AnimationBuilder::applyTag(%this, %tag)
{
}

function AnimationBuilder::deleteTagFromList(%this, %tag)
{
}

function AnimationBuilder::openAssetPicker(%this, %data)
{
    %type = getWord(%data, 0);
    %tag = getWord(%data, 1);
    %tool = getWord(%data, 2);
    %category = "";
    AssetPicker.open(%type, %tag, %category, %tool);
}

function AnimationBuilder::setImageMap(%this, %imageMapID)
{
    ABAnimationPreviewWindow.setImage("{EditorAssets}:transparent");
    %this.sourceImageMap = %imageMapID;
    %anim = AssetDatabase.acquireAsset(%this.tempAnimation);
    %image = AssetDatabase.acquireAsset(%imageMapID);
    %frameCount = %image.getFrameCount();
    AssetDatabase.releaseAsset(%imageMapID);
    %this.updateAnimationFrames(%this.tempAnimation, %frameCount);
    %anim.ImageMap = %imageMapID;
    AssetDatabase.releaseAsset(%this.tempAnimation);
    %this.clearFrames();

    Ab_ImageMapPreviewWindow.clear();
    Ab_ImageMapPreviewWindow.display(%this.sourceImageMap);
}

function AnimationBuilder::updateAnimationFrames(%this, %animAssetID, %frameCount)
{
    %anim = AssetDatabase.acquireAsset(%animAssetID);
    %animFrames = getWordCount(%anim.animationFrames);
    for ( %i = 0; %i < %animFrames; %i++ )
    {
        %frameNum = getWord(%anim.animationFrames, %i);
        if ( %frameNum < %frameCount )
        {
            %frameBuff = %frameBuff SPC %frameNum;
        }
    }
    %anim.animationFrames = trim(%frameBuff);
    AssetDatabase.releaseAsset(%animAssetID);
}

function AnimationBuilder::generateTemporaryAnimation(%this)
{
    %newAnim = new AnimationAsset()
    {
        ImageMap = %this.sourceImageMap;
    };

    %newAssetID = AssetDatabase.addPrivateAsset(%newAnim);
    %this.tempAnimation = %newAssetID;
}

package AnimationBuilderPackage
{

function AnimationAsset::getFrameCount(%this)
{
    return getWordCount(%this.animationFrames);
}

// <summary>
//
//  Note - Packages don't seem to like /// j-doc comment tags....
//
// This copies all relevant attributes from a source animation asset ID to this
// animation asset.  Note that if this animation asset does not yet have an asset
// ID the tags from the source ID won't be copied.  This means that, if you are
// trying to save a new object that is a copy of another object you must first 
// register the new asset with the asset database and then acquire an instance of 
// the asset before calling ::copy().
// </summary>
// <param name="sourceID">The Asset ID of the source asset to copy from.</param>
function AnimationAsset::copy(%this, %sourceID)
{
    %sourceAsset = AssetDatabase.acquireAsset(%sourceID);
    %this.animationFrames = %sourceAsset.animationFrames;
    %this.ImageMap = %sourceAsset.ImageMap;
    %this.animationTime = %sourceAsset.animationTime;
    %this.animationCycle = %sourceAsset.animationCycle;
    %this.randomStart = %sourceAsset.randomStart;
    AssetDatabase.releaseAsset(%sourceID);

    %assetTagManifest = AssetDatabase.getAssetTags();
    %assetTagCount = %assetTagManifest.getAssetTagCount(%sourceID);
    for (%i = 0; %i < %assetTagCount; %i++)
    {
        %tag = %assetTagManifest.getAssetTag(%sourceID, %i);
        // ::getAssetId() does not work if this asset was not acquired from 
        // the asset database.
        %assetTagManifest.tag(%this.getAssetId(), %tag);
    }
}

function AnimationAsset::rename(%this, %name)
{
    %assetId = %this.getAssetId();
    %oldName = %this.AssetName;
    %module = AssetDatabase.getAssetModule(%assetId);
    if ( %module !$= "" )
    {
        %newId = strreplace(%assetId, %oldName, %name);
        AssetDatabase.renameDeclaredAsset(%assetId, %newId);
    }
    return %this.getAssetId();
}

};
