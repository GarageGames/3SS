//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

$ImageEditorMinCellSizeX = 4;
$ImageEditorMinCellSizeY = 4;

function ImageEditor::editImageMap(%this, %imageMap)
{
    %isStockAsset = getIsStockAsset(%imageMap);
    if ( !AssetDatabase.isDeclaredAsset(%imageMap) || %isStockAsset )
    {
        // if the incoming asset ID is invalid or if it is a stock asset, create
        // a new temporary asset and work with it.
        %this.generateTemporaryImage();
        if (%isStockAsset)
        {
            %temp = AssetDatabase.acquireAsset(%this.tempImageID);
            %temp.copy(%imageMap);
            AssetDatabase.releaseAsset(%this.tempImageID);
        }
        %imageMap = %this.tempImageID;
    }
    else
    {
        %this.tempImageID = %imageMap;
    }
      
    if (isObject($ImageEditorScene))
        $ImageEditorScene.delete();
        
    $ImageEditorScene = new Scene(ImageEditorScene);

    Canvas.pushDialog(ImageBuilderGui);
    %this.launchImageEditor(%imageMap);
}

// --------------------------------------------------------------------
// function ImageEditor::launchImageEditor()
//
// This is called both when a new image is created and when an already
// existing image map is being edited.  We copy the image map datablock
// into a temporary holder so if we cancel we can reset the settings.
// --------------------------------------------------------------------
function ImageEditor::launchImageEditor(%this, %imageMap)
{  
    %this.setupPreviewWindow();

    if ( %this.editing )
    {
        %isStock = getIsStockAsset(%imageMap);
        if ( %this.tempImageID !$= "" && !%isStock )
        {
            // release and destroy temporary image asset
            if ( AssetDatabase.isDeclaredAsset(%this.tempImageID) && %this.tempImageID !$= %imageMap )
            {
                AssetDatabase.removeSingleDeclaredAsset(%this.tempImageID);
            }
            // create new, fresh temporary asset and populate it
            %this.generateTemporaryImage();
            %tempImage = AssetDatabase.acquireAsset(%this.tempImageID);
            %tempImage.copy(%imageMap);
            AssetDatabase.releaseAsset(%this.tempImageID);
        }
    }
    %this.sourceImageID = %imageMap;

    %this.sourceAsset = AssetDatabase.acquireAsset(%this.sourceImageID);
    %this.tempAsset = AssetDatabase.acquireAsset(%this.tempImageID);
    if ( %this.editing )
        %this.linkProperties = %this.sourceAsset.linkProperties;
    else
        %this.tempAsset.linkProperties = %this.linkProperties;

    %this.selectedImage = %this.tempAsset;
    // Store the starting name for name checks
    %this.sourceName = %this.sourceAsset.AssetName;
    %this.assetName = %this.sourceAsset.AssetName;
    
    if (%this.sourceAsset.ImageFile !$= "")
    {
        ImageBuilderImageLocation.setText(%this.sourceAsset.ImageFile); 
    }
        
    ImageEditorTagList.refresh(0, "");

    %this.maxWidth = PreviewContainerOTDialog.Extent.x - 16;
    %this.maxHeight = PreviewContainerOTDialog.Extent.y - 16;

    %this.loadPreview(%this.tempImageID);

    ImageEditorTagContainer.populateTagList(%this.tempImageID);
    
    ImageEditorSaveButton.update();
    
    ImageEditorAutoApply();
}

function ImageEditor::refreshImageAsset(%this, %flag)
{
    %this.linkProperties = %flag;
    %this.tempAsset.linkProperties = %flag;
    %this.tempAsset.refreshAsset();
    if ( %this.editing )
    {
        %this.sourceAsset.linkProperties = %flag;
        %this.sourceAsset.refreshAsset();
    }
}

function ImageEditor::createImageAsset(%this, %name)
{
    %imageAsset = new ImageAsset()
    {
        AssetName = %name;
        ImageFile = "^{EditorAssets}/data/images/DefaultImage";
        ImageMode = FULL;
        CellCountX   = 1;
        CellCountY   = 1;
        CellHeight   = 128;
        CellWidth    = 128;
        CellOffsetX  = 0;
        CellOffsetY  = 0;
        CellStrideX  = 0;
        CellStrideY  = 0;
        CellRowOrder = true;
        FilterMode = "NONE";
        FilterPad = false;
    };

    TamlWrite(%imageAsset, expandPath("^{UserAssets}/images/" @ %imageAsset.AssetName @ ".asset.taml"));

    %userAssetModule = ModuleDatabase.findModule("{UserAssets}", 1);
    AssetDatabase.addSingleDeclaredAsset(%userAssetModule, expandPath("^{UserAssets}/images/" @ %imageAsset.AssetName @ ".asset.taml"));
    %newAssetID = "{UserAssets}:" @ %imageAsset.AssetName;

    return %newAssetID;
}

function ImageEditor::generateTemporaryImage(%this)
{
    %newImage = new ImageAsset()
    {
        ImageFile = "^{EditorAssets}/data/images/DefaultImage";
        ImageMode = FULL;
        AssetInternal = false;
        CellCountX   = 1;
        CellCountY   = 1;
        CellHeight   = 128;
        CellWidth    = 128;
        CellOffsetX  = 0;
        CellOffsetY  = 0;
        CellStrideX  = 0;
        CellStrideY  = 0;
        CellRowOrder = true;
        FilterMode = "NONE";
        FilterPad = false;
    };

    %newAssetID = AssetDatabase.addPrivateAsset(%newImage);
    %this.tempImageID = %newAssetID;
}


// --------------------------------------------------------------------
// ImageEditor::saveImage()
//
// This gets called when the user clicks the save button, it clears
// out the proper data and adds the newly created imageMap to the
// managed datablock (if it isn't already there in the case of editing), 
// as well as calling the object libraries to refresh.
// --------------------------------------------------------------------
function ImageEditor::saveImage(%this)
{
    // hide away our Image Builder GUI
    %this.forcePreviewClear = false;
    Canvas.popDialog(ImageBuilderGui);
    
    %imageMap = %this.sourceImageID;
    %previewImageMap = %this.tempImageID;

    %imageDBName = ImageEditorTxtImageName.getValue();
    %imageDBName = strreplace(%imageDBName, " ", "_");   

    restoreDB(%previewImageMap, %imageMap, %imageDBName);

    // Copy tags
    %this.copyTags(%this.tempImageID, %this.sourceImageID); 

    %imageAsset = AssetDatabase.acquireAsset(%imageMap);
    // if we're creating a new image then add it to the asset database
    if (!%this.editing)
    {
        %newAssetId = %this.createImageAsset(%imageDBName);
        %newAsset = AssetDatabase.acquireAsset(%newAssetId);
        %newAsset.copy(%imageMap);
        %this.copyTags(%imageMap, %newAssetId);
        
        %pathNoExtension = expandPath("^{UserAssets}/images/") @ fileBase(%imageDBName);

        // Calculate target filename.
        %targetFilename = expandPath("^{UserAssets}/images/" @ fileName(%imageAsset.ImageFile) );

        // Is the file in the correct target location?
        if ( %targetFilename !$= %imageAsset.ImageFile )
        {
            // No, so copy.
            if ( !pathCopy( %imageAsset.ImageFile, %targetFilename, true ) )
            {
                warn( " @@@ Target file '" @ %targetFilename @ " already exists.  File not copied." );
            }
        }
        // Update image filename.
        %newAsset.ImageFile = %this.stripExtension( %targetFilename );

        AssetDatabase.releaseAsset(%imageMap);
        removeResPath(filePath(%newFileLocation));
    }
    else
    {   
        // We are modifying an existing asset.  Take appropriate action based on 
        // the origin of the asset.
        %module = AssetDatabase.getAssetModule(%this.sourceImageID);
        if ( %module.ModuleId !$= "{UserAssets}" )
        {
            // This is a modified version of a stock asset.  Create a user copy
            // in {UserAssets}.
            %newAssetId = %this.createImageAsset(%imageDBName);
            %newAsset = AssetDatabase.acquireAsset(%newAssetId);
            %newAsset.copy(%imageMap);
            %this.copyTags(%imageMap, %newAssetId);
            
            %pathNoExtension = expandPath("^{UserAssets}/images/") @ fileBase(%imageDBName);

            // Calculate target filename.
            %targetFilename = expandPath("^{UserAssets}/images/" @ fileName(%imageAsset.ImageFile) );

            // Is the file in the correct target location?
            if ( %targetFilename !$= %imageAsset.ImageFile )
            {
                // No, so copy.
                if ( !pathCopy( %imageAsset.ImageFile, %targetFilename, true ) )
                {
                    warn( " @@@ Target file '" @ %targetFilename @ " already exists.  File not copied." );
                }
            }
            // Update image filename.
            %newAsset.ImageFile = %this.stripExtension( %targetFilename );

            AssetDatabase.releaseAsset(%imageMap);
            removeResPath(filePath(%newFileLocation));
        }
        else
        {
            // Up here we need to check for changes to image file, cell counts and cell sizes.  If
            // any of these have changed we will find any dependent animations and then ask the user
            // if he wants to proceed.  If the user changes cell counts or sizes, update any dependent 
            // animations to eliminate any frame indices that no longer exist.
            // ----------------------
            %path = expandPath("^{UserAssets}/images/");

            %query = new AssetQuery();
            %depCount = AssetDatabase.findAssetIsDependedOn(%query, %this.sourceImageID);

            if ( %this.cellCountChanged(%this.sourceImageID, %this.tempImageID) && %depCount > 0 )
            {
                Ie_ConfirmChangeGui.display("", %this, updateDependentAnimations, %this.sourceImageID);
                return;
            }
            %this.updateAsset();
        }
    }

    %this.close();
    AssetLibrary.schedule(100, "updateGui");
}

function ImageEditor::updateAsset(%this)
{
    // We're here because the user was editing an image that needed to update
    // dependent animations or because we're editing an existing user asset.
    // We have changed the name of the asset we are editing. Need to update
    %imageDBName = ImageEditorTxtImageName.getValue();
    %imageDBName = strreplace(%imageDBName, " ", "_");   
    %imageAsset = AssetDatabase.acquireAsset(%this.sourceImageID);

    if (%imageDBName !$= %this.sourceName)
    {
        %assetModule = AssetDatabase.getAssetModule(%this.sourceImageID).ModuleId;
        %newID = %assetModule @ ":" @ %imageDBName;
        
        AssetDatabase.renameDeclaredAsset(%this.sourceImageID, %newID);
        AssetDatabase.renameReferencedAsset(%this.sourceImageID, %newID);
        
        if (isFile(%path @ %this.sourceName @ ".asset.taml"))
            fileDelete(%path @ %this.sourceName @ ".asset.taml");
        TamlWrite(%imageAsset, %path @ %imageDBName @ ".asset.taml");
    }
    else
    {
        %imageAsset.copy(%this.tempImageID);
        TamlWrite(%imageAsset, %path @ %this.sourceName @ ".asset.taml");
    }

    // If the image file does not already exist in the UserAssets directory
    // copy it there
    %newImageFileLocation = findImageFileLocation(%path, fileBase(%imageMap.ImageFile));
    if (%newImageFileLocation $= "")
    {
        %oldImageFileLocation = findImageFileLocation(filePath(%imageMap.ImageFile), fileBase(%imageMap.ImageFile));
        if (%oldImageFileLocation !$= "")
        {
            %newImageFileLocation = %path @ fileBase(%oldImageFileLocation) @ fileExt(%oldImageFileLocation);
            pathCopy(%oldImageFileLocation, %newImageFileLocation);
        }
    }

    AssetDatabase.releaseAsset(%this.sourceImageID);
    AssetDatabase.releaseAsset(%this.tempImageID);
    AssetDatabase.releaseAsset(%imageMap);

    if (isObject(%this.sourceImageID))
        %this.sourceImageID.delete();

    if (isObject(%this.tempImageID))
        %this.tempImageID.delete();
}

function ImageEditor::cellPropertiesChanged(%this, %source, %current)
{
    %sourceAsset = AssetDatabase.acquireAsset(%source);
    %currentAsset = AssetDatabase.acquireAsset(%current);

    %changed = false;
    if ( %currentAsset.cellWidth != %sourceAsset.cellWidth )
        %changed = true;
    if ( %currentAsset.cellHeight != %sourceAsset.cellHeight )
        %changed = true;
    if ( %currentAsset.cellRowOrder != %sourceAsset.cellRowOrder )
        %changed = true;
    if ( %currentAsset.cellOffsetX != %sourceAsset.cellOffsetX )
        %changed = true;
    if ( %currentAsset.cellOffsetY != %sourceAsset.cellOffsetY )
        %changed = true;
    if ( %currentAsset.cellStrideX != %sourceAsset.cellStrideX )
        %changed = true;
    if ( %currentAsset.cellStrideY != %sourceAsset.cellStrideY )
        %changed = true;
    if ( %currentAsset.cellCountX != %sourceAsset.cellCountX )
        %changed = true;
    if ( %currentAsset.cellCountY != %sourceAsset.cellCountY )
        %changed = true;

    AssetDatabase.releaseAsset(%current);
    AssetDatabase.releaseAsset(%source);
    return %changed;
}

function ImageEditor::cellCountChanged(%this, %source, %current)
{
    %sourceAsset = AssetDatabase.acquireAsset(%source);
    %currentAsset = AssetDatabase.acquireAsset(%current);

    %changed = false;
    if ( %currentAsset.cellCountX != %sourceAsset.cellCountX )
        %changed = true;
    if ( %currentAsset.cellCountY != %sourceAsset.cellCountY )
        %changed = true;

    AssetDatabase.releaseAsset(%current);
    AssetDatabase.releaseAsset(%source);
    return %changed;
}

function ImageEditor::updateDependentAnimations(%this, %animList)
{
    %count = getWordCount(%animList);
    %this.updateAsset();
    for ( %i = 0; %i < %count; %i++ )
    {
        %anim = getWord(%animList, %i);
        %tempAnim = AssetDatabase.acquireAsset(%anim);
        %this.scrubInvalidFrames(%tempAnim);
        AssetDatabase.releaseAsset(%anim);
    }

    %this.close();
    AssetLibrary.schedule(100, "updateGui");
}

function ImageEditor::scrubInvalidFrames(%this, %animationAsset)
{
    %imageAsset = AssetDatabase.acquireAsset(%animationAsset.ImageMap);
    %imageFrames = %imageAsset.getFrameCount();
    AssetDatabase.releaseAsset(%animationAsset.ImageMap);

    %frameCount = getWordCount(%animationAsset.AnimationFrames);
    for (%i = 0; %i < %frameCount; %i++)
    {
        %frame = getWord(%animationAsset.AnimationFrames, %i);
        if ( %frame < %imageFrames )
            %frames = %frames SPC %frame;
    }
    %animationAsset.AnimationFrames = trim(%frames);
}

function ImageEditor::close(%this)
{
    %this.clearPreview();
    %this.clearData();
    
    %this.lastCopiedImageFile = "";
}

// --------------------------------------------------------------------
// ImageEditor::cancel()
//
// This gets called when the user clicks the cancel button, it cleans up
// some resources and then closes out the Image Builder.
// --------------------------------------------------------------------
function ImageEditor::cancel(%this)
{
    %this.close();
    Canvas.popDialog(ImageBuilderGui);
}

// --------------------------------------------------------------------
// ImageEditor::imageFileBrowser()
//
// This gets called when the user clicks the file browser button, it 
// calls the cancel functionality then spawns the file browser.
// --------------------------------------------------------------------
function ImageEditor::imageFileBrowser(%this)
{
    if (!isObject(%this.selectedImage))
        return; 

    %dlg = new OpenFileDialog()
    {
        Filters = $T2D::ImageMapSpec;
        ChangePath = false;
        MustExist = true;
        MultipleFiles = false;
    };

    // Update Image        
    if (%dlg.Execute())
    {
        %fileName = %dlg.FileName;

        // Check file validity
        if (!isValidImageFile(%fileName))
        {
            MessageBoxOK("Warning", "'"@%fileOnlyName@"' is not a valid image file.", "");
            return;
        }

        // If we're editing an asset, remove the old image file.
        if ( %this.selectedImage.ImageFile !$= "" )
        {
            //%isInUse = AssetDatabase.findAssetDependsOn
            fileDelete(expandPath(%this.selectedImage.ImageFile));
            %this.selectedImage.ImageFile = "";
        }

        // Calculate target filename.
        %targetFilename = expandPath("^{UserAssets}/images/" @ fileName(%fileName) );

        // Is the file in the correct target location?
        if ( %targetFilename !$= %this.selectedImage.ImageFile )
        {
            // No, so copy.
            if ( !pathCopy( %fileName, %targetFilename, false ) )
            {
                warn( " @@@ Target file '" @ %targetFilename @ " already exists.  File not copied." );
            }
        }

        %this.selectedImage.setImageFile(%targetFilename);
        %this.selectedImage.reloadAsset();
        ImageEditorAutoApply();
    }
    
    %dlg.delete();
}

function ImageEditor::stripExtension(%this, %fileName)
{
    %noExt = strreplace(%fileName, ".jpg", "");
    %noExt = strreplace(%noExt, ".jpeg", "");
    %noExt = strreplace(%noExt, ".png", "");
    %noExt = strreplace(%noExt, ".pvr", "");

    return %noExt;
}

// --------------------------------------------------------------------
// ImageEditor::delete()
//
// This gets called when the user clicks the delete button, it will prompt
// the user to make sure they do in fact want to delete this image map.
// --------------------------------------------------------------------
function ImageEditor::delete(%this)
{
    // prompt the user with how many references this image map has
    checkImageMapReference(%this.sourceImageID, Toolmanager.getLastWindow().getScene(), "ImageEditor.cancel();");

    // [neo, 15/6/2007 - #3231]
    // This doesnt exist so forward it to the SBCreateTrash method.
    // checkImageMapReferences(%this.sourceImageID, Toolmanager.getLastWindow().getScene(), "ImageEditor.cancel();");   

    // Save image map ref as this will be cleared when we call cancel()
    %imageMap = %this.sourceImageID;

    // Close first so it cleans up correctly
    %this.cancel();

    // Take out the trash...
    SBCreateTrash::deleteImageMap(%imageMap);

    // set the tool to the selection tool otherwise next click you'll try and create
    // and image that doesn't exist 
    LevelBuilderToolManager::setTool(LevelEditorSelectionTool);
}

function ImageEditor::copyTags(%this, %assetSourceID, %assetDestID)
{
    // Clear tags on the destination asset
    %this.clearTags(%assetDestID);
    
    %assetTagsManifest = AssetDatabase.getAssetTags();
    %assetTagCount = %assetTagsManifest.getAssetTagCount(%assetSourceID);
    for (%i = 0; %i < %assetTagCount; %i++)
    {
        %assetTagsManifest.tag(%assetDestID, %assetTagsManifest.getAssetTag(%assetSourceID, %i));
    }
}

function ImageEditor::clearTags(%this, %assetID)
{
    %assetTagsManifest = AssetDatabase.getAssetTags();
    %assetTagCount = %assetTagsManifest.getAssetTagCount(%assetID);
    for (%i = 0; %i < %assetTagCount; %i++)
    {
        %assetTagsManifest.untag(%assetID, %assetTagsManifest.getAssetTag(%assetID, 0));
    }
}

function ImageEditor::clearData(%this)
{
    %this.startingName = "";
    if ( isObject( %this.sourceAsset ) )
    {
        AssetDatabase.releaseAsset(%this.sourceImageID);
        %this.sourceImageID = "";
    }
    if ( isObject( %this.tempAsset ) )
    {
        AssetDatabase.releaseAsset(%this.tempImageID);
        AssetDatabase.removeSingleDeclaredAsset(%this.tempImageID);
        %this.tempImageID = "";
    }
    if ( isObject( %this.selectedImage ) )
    {
        AssetDatabase.releaseAsset(%this.selectedImage.getAssetId());
        %this.selectedImage = "";
    }
}

function ImageEditor::removeLastCopiedImageFile(%this)
{
    // Check if the file exists
    if (!isFile(%this.lastCopiedImageFile))
    {
        %this.lastCopiedImageFile = "";
        return;   
    }

    fileDelete(%this.lastCopiedImageFile);
    %this.lastCopiedImageFile = "";
}

// --------------------------------------------------------------------
// ImageEditor::reCreateImage()
//
// re-creates the image for the image, this does a majority of the work
// it will loop through and set each imagemap setting appropriately
// based on image mode.  It will also set some defaults and some fail 
// safes to make sure people cannot set invalid values.
// --------------------------------------------------------------------
function ImageEditor::reCreateImage(%this, %imageMap)
{  
    if (!isObject(%imageMap))
        return;

    %image = %imageMap.getImageFile();

    %srcSize = %imageMap.getImageSize();

    %sizeX = getWord(%srcSize, 0);
    %sizeY = getWord(%srcSize, 1);

    %xSizeCheck = $ImageEditorMinCellSizeX;
    %ySizeCheck = $ImageEditorMinCellSizeY;
    %xCountCheck = mRound(%sizeX / $ImageEditorMinCellSizeX);
    %yCountCheck = mRound(%sizeY / $ImageEditorMinCellSizeY);

    %this.AssetName = ImageEditorTxtImageName.getValue();

    if (isObject(%this.sourceAsset))
        %sourceName = %this.sourceAsset.AssetName;
    else
        %sourceName = %this.AssetName;
   
    %assetQuery = new AssetQuery();
    AssetDatabase.findAssetName(%assetQuery, %this.AssetName);
    
    %assetExists = %assetQuery.Count > 0;
    
    // Check to make sure we aren't conflicting with another object's name
    if ((%this.sourceName !$= %this.AssetName && %assetExists) && %this.AssetName !$= %sourceName)
    {
        MessageBoxOK("Warning", "An image with this name already exists.", "");

        %newName = %sourceName;

        %name = strreplace(%newName, "ImageAsset_", "");
        if ( %name $= %newName )
            ImageEditorTxtImageName.setText(%newName);

        %this.AssetName = %newName;
    }

    // Fetch cell count.
    %cellCountX = ImageEditorCellCountX.getValue();
    %cellCountY = ImageEditorCellCountY.getValue();      
    %cellHeight = ImageEditorCellHeight.getValue();
    %cellWidth = ImageEditorCellWidth.getValue();
   
    // first we check the cell width and height for
    // reasonable values
    if (%cellWidth == 0)
        %cellWidth = %sizeX;  
    else if (%cellWidth < %xSizeCheck)
        %cellWidth = %xSizeCheck;
        
    if (%cellWidth > %sizeX)
        %cellWidth = %sizeX;
        
    if (%cellHeight == 0)
        %cellHeight = %sizeY;  
    else if (%cellHeight < %ySizeCheck)
        %cellHeight = %ySizeCheck;
    
    if (%cellHeight > %sizeY)
        %cellHeight = %sizeY;

    // Clamp cell counts.
    if (%cellCountX < 1)
        %cellCountX = 1;

    if (%cellCountY < 1)
        %cellCountY = 1;
        
    if (%cellCountX > %xCountCheck)
        %cellCountX = %xCountCheck;
    
    if (%cellCountY > %yCountCheck)
        %cellCountY = %yCountCheck;
         
    //if (%cellCountX > 1 || %cellCountY > 1)
    //{
        %div = mFloor(%sizeX / %cellWidth);

        if (%this.CountExtentOverride $= "Extent")
        {
            if (%this.linkProperties == true)           
                %cellCountX = %div;
            
            if (%cellCountX > %div)
                %cellCountX = %div;
        }
        else
        {
            %widthFloor = mFloor(%sizeX / %cellCountX);
            if (%this.linkProperties == true)
                %cellWidth = %widthFloor;

            if (%cellWidth >= 2048)
                %cellWidth = %sizeX;
            else if (%cellWidth <= 0)
                %cellWidth = $ImageEditorMinCellSizeX;
            if (%cellWidth > %widthFloor)
                %cellWidth = %widthFloor;
            
        }

        %div = mFloor(%sizeY / %cellHeight);

        if (%this.CountExtentOverride $= "Extent")
        {
            if (%this.linkProperties == true)            
                %cellCountY = %div;
                
            if (%cellCountY > %div)
                %cellCountY = %div;
        }
        else
        {
            %heightFloor = mFloor(%sizeY / %cellCountY);
            if (%this.linkProperties == true)
                %cellHeight = %heightFloor;

            if (%cellHeight >= 2048)
                %cellHeight = %sizeY;
            else if (%cellHeight <= 0)
                %cellHeight = $ImageEditorMinCellSizeY;
            if (%cellHeight > %heightFloor)
                %cellHeight = %heightFloor;
        }
    //}
 
    // Set common configuration.
    %imageMap.imageFile = %image;
    %imageMap.cellCountX = %cellCountX;
    %imageMap.cellCountY = %cellCountY;

    %srcSize = %imageMap.getImageSize();
  
    // Split-up bitmap size.
    %sizeX = getWord(%srcSize, 0);
    %sizeY = getWord(%srcSize, 1);      

    // Calculate cell configuration.
    %imageMap.cellCountX   = %cellCountX;
    %imageMap.cellCountY   = %cellCountY;
    %imageMap.cellHeight   = %cellHeight;
    %imageMap.cellWidth    = %cellWidth;
    %imageMap.cellOffsetX  = 0;
    %imageMap.cellOffsetY  = 0;
    %imageMap.cellStrideX  = 0;
    %imageMap.cellStrideY  = 0;
    %imageMap.cellRowOrder = true;
    %imageMap.filterMode = "NONE";
    %imageMap.filterPad = false;

   
    //if (%this.sourceName !$= %this.AssetName)
        //updateDependentObjects(%this.sourceName, %this.AssetName);

    %this.sourceName = %this.AssetName;
}

function ImageEditor::addTag(%this, %tag)
{
    if (%tag $= "" || %tag $= "Any")
        return;

    %assetTagManifest = AssetDatabase.getAssetTags();
    %assetTagCount = %assetTagManifest.getAssetTagCount(%this.tempImageID);
    for (%i = 0; %i < %assetTagCount; %i++)
    {
        %tempTag = %assetTagManifest.getAssetTag(%this.tempImageID, %i);
        if (%tag $= %tempTag)
        {
            echo(" @@@ asset already has tag " @ %tag);
            return;
        }
    }
    %assetTagManifest.tag(%this.tempImageID, %tag);
    ImageEditorTagContainer.addTagItem(%tag);
    ImageEditorTagContainer.populateTagList(%this.tempImageID);
}

function ImageEditor::removeTag(%this, %tag)
{
    %assetTagManifest = AssetDatabase.getAssetTags();
    %assetTagManifest.untag(%this.tempImageID, %tag);
    ImageEditorTagContainer.removeTagItem(%tag);
    ImageEditorTagContainer.populateTagList(%this.tempImageID);
}

// --------------------------------------------------------------------
// ImageEditor::loadPreview()
//
// This basically does all of the preview loading funcitonality.  It
// checks which mode the image map is in and loads the preview images
// appropriately.
// --------------------------------------------------------------------
function ImageEditor::loadPreview(%this, %imageAssetID)
{  
    // do some sanity checks
    if ( AssetDatabase.isDeclaredAsset(%imageAssetID) )
        %imageMap = AssetDatabase.acquireAsset(%imageAssetID);

    if (!isObject(%imageMap) || %imageMap.getClassName() !$= "ImageAsset")
        return;

    if (!$ImageEditorLoaded)
        $ImageEditorLoaded = true;

    // we are now loading the preview (this is to prevent this
    // function from being called recursively from the autoApply)
    %this.loadingPreview = true;   

    // store the selected image
    %this.selectedImage = %imageMap;

    // grab the max width and height
    %maxWidth = %this.maxWidth;
    %maxHeight = %this.maxHeight;

    // grab the source image size
    %srcSize = %imageMap.getImageSize();
    %srcWidth = getWord(%srcSize, 0);
    %srcHeight = getWord(%srcSize, 1);

    %scale = 1;
    %widthRatio = %srcWidth / %maxWidth;
    if (%widthRatio > %scale)    
        %scale = %widthRatio;
    %heightRatio = %srcHeight / %maxHeight;
    if (%heightRatio > %scale)    
        %scale = %heightRatio;

    %width = %srcWidth / %scale;
    %height = %srcHeight / %scale;
    %posX = mRound(PreviewContainerOTDialog.getExtent().x/2 - %width/2);
    %posY = mRound(PreviewContainerOTDialog.getExtent().y/2 - %height/2);
    
    %imageFile = %imageMap.getImageFile();
    
    ImageBuilderBitmapPreview.setPosition(%posX, %posY);
    ImageBuilderBitmapPreview.setExtent(%width, %height);
    ImageBuilderBitmapPreview.setBitmap(%imageFile);
    
    // Set visibility
    if (%width == 0 || %height == 0)
        ImageBuilderBitmapPreview.visible = false;
    else
        ImageBuilderBitmapPreview.visible = true;
    
    // Create grid
    %rowCount = %imageMap.getCellCountY();
    %colCount = %imageMap.getCellCountX();
    %cellWidth = %imageMap.getCellWidth();
    %cellHeight = %imageMap.getCellHeight();
    createGridOverlay(PreviewContainerOTDialog, %posX SPC %posY, %width SPC %height, %rowCount, %colCount, (%cellWidth / %scale) SPC (%cellHeight / %scale), true);
    
    loadImageMapSettings(%imageMap);

    %this.loadingPreview = false;
}

package ImageEditorPackage
{

// <summary>
//
//  Note - Packages don't seem to like /// j-doc comment tags....
//
// This copies all relevant attributes from a source image asset ID to this
// image asset.  Note that if this image asset does not yet have an asset
// ID the tags from the source ID won't be copied.  This means that, if you are
// trying to save a new object that is a copy of another object you must first 
// register the new asset with the asset database and then acquire an instance of 
// the asset before calling ::copy().
// </summary>
// <param name="sourceID">The Asset ID of the source asset to copy from.</param>
function ImageAsset::copy(%this, %sourceID)
{
    %sourceAsset = AssetDatabase.acquireAsset(%sourceID);
    %this.imageFile = %sourceAsset.imageFile;
    %this.cellWidth = %sourceAsset.cellWidth;
    %this.cellHeight = %sourceAsset.cellHeight;
    %this.filterMode = %sourceAsset.filterMode;
    %this.cellRowOrder = %sourceAsset.cellRowOrder;
    %this.cellOffsetX = %sourceAsset.cellOffsetX;
    %this.cellOffsetY = %sourceAsset.cellOffsetY;
    %this.cellStrideX = %sourceAsset.cellStrideX;
    %this.cellStrideY = %sourceAsset.cellStrideY;
    %this.cellCountX = %sourceAsset.cellCountX;
    %this.cellCountY = %sourceAsset.cellCountY;
    %this.force16Bit = %sourceAsset.force16Bit;
    %this.linkProperties = %sourceAsset.linkProperties;
    AssetDatabase.releaseAsset(%sourceID);

    %this.copyImageToUserAssets();

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

function ImageAsset::copyImageToUserAssets(%this)
{
    %imageLocation = expandPath(%this.ImageFile);
    
    // Calculate target filename.
    %targetFilename = expandPath("^{UserAssets}/images/" @ fileName(%this.ImageFile) );

    // Is the file in the correct target location?
    if ( %targetFilename !$= %imageLocation )
    {
        // No, so copy.
        if ( !pathCopy( %this.ImageFile, %targetFilename, true ) )
        {
            warn( " @@@ Target file '" @ %targetFilename @ " already exists.  File not copied." );
        }
    }
    // Update image filename.
    %this.ImageFile = %targetFilename;
}

};