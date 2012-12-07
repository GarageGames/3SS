//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

function ImageEditor::hidePreview(%this)
{
    // set the selection preview to invisible and all of the lines
    %this.selectionPreview.setVisible(false); 
    %this.selectionPreview.leftLine.setVisible(false);
    %this.selectionPreview.rightLine.setVisible(false);
    %this.selectionPreview.topLine.setVisible(false);
    %this.selectionPreview.bottomLine.setVisible(false);
}

function ImageEditor::unhidePreview(%this)
{
    // set the preview to visible and all of the lines
    %this.selectionPreview.setVisible(true); 
    %this.selectionPreview.leftLine.setVisible(true);
    %this.selectionPreview.rightLine.setVisible(true);
    %this.selectionPreview.topLine.setVisible(true);
    %this.selectionPreview.bottomLine.setVisible(true);
}

function ImageEditor::setPreview(%this, %preview)
{
    // set the bool show preview  value
    %this.showPreview = %preview;
}

function ImageEditor::updatePreview(%this, %pos)
{
    // first lets unhide the preview box in case it was hidden
    %this.unhidePreview();

    // now lets get the dimensions that we'd zoom into
    // if we were to click at this point
    %dimensions = %this.getZoomIntoDimensions(0.25, %pos);
    %width = getWord(%dimensions, 0);
    %height = getWord(%dimensions, 1);

    %size = %width SPC %height;

    // get a relative padding size (this controls line width for the border box)
    %div = 40;
    %wDiv = %width / %div;
    %hDiv = %hieght / %div;

    %padding = %wDiv + %hDiv;

    // now lets resize the border to show a preview of where to zoom
    %this.resizeImageBorder(%this.selectionPreview, %pos, %size, %padding);
}

// --------------------------------------------------------------------
// ImageEditor::setupPreviewWindow()
//
// This sets up the preview window
// --------------------------------------------------------------------
function ImageEditor::setupPreviewWindow(%this)
{  
    // store the background, border, and preview layers
    %this.backgroundLayer = 10;
    %this.borderLayer = 5;
    %this.previewLayer = 5;

    // set all of the background info GUIs to not be visible
    //ImageBuilderBackgroundInfoWindow.setVisible(false);
    //ImageBuilderBackgroundInfoControl.setVisible(false);
    //ImageBuilderBackgroundColorPickerControl.setVisible(false);
    //ImageBuilderObjectBorderColorPickerControl.setVisible(false);

    // set the more options text
    //ImageBuilderColorPickerButton.setText("More Options");
    //ImageBuilderObjectBorderColorPickerButton.setText("More Options");

    // set the extents of the background info
    //ImageBuilderBackgroundInfoControl.setExtent(338, 151);
    //ImageBuilderBackgroundInfoWindow.setExtent(347, 181);

    // get the extent of the preview window
    %extent = ImageBuilderImageSceneWindow.getExtent();

    // set the border padding (this will also effect the image
    // border line size
    %this.borderPadding = 3;

    // grab the width and height of the extent
    %this.maxWidth = getWord(%extent, 0);
    %this.maxHeight = getWord(%extent, 1);

    // calculate out the rowspace
    %this.rowSpace = %this.maxHeight * 0.015;
    %this.colSpace = %this.maxWidth * 0.015;

    // store the base X and Y positions
    %this.baseX = 0;
    %this.baseY = 0;

    // set the scene and the camera position
    ImageBuilderImageSceneWindow.setScene( $ImageEditorScene );
    ImageBuilderImageSceneWindow.setCurrentCameraPosition(%this.baseX SPC %this.baseY SPC %this.maxWidth SPC %this.maxHeight);   

    //// create and store the background sprite, this is for color manipulation
    //%this.backgroundSprite = new sprite() { scene = $ImageEditorScene; };
    //%this.backgroundSprite.setSize(%this.maxWidth, %this.maxHeight);
    //%this.backgroundSprite.setPosition(%this.baseX, %this.baseY);
    //%this.backgroundSprite.setSceneLayer(%this.backgroundLayer);
//
    //%assetQuery = new AssetQuery();
    //AssetDatabase.findAssetName(%assetQuery, "ImageBuilderBackgroundImageMap");
    //%assetId = %assetQuery.getAsset(0);
    //%assetQuery.delete();
//
    //%this.backgroundSprite.ImageMap = %assetId;
    //%this.backgroundSprite.border = true;
    //%this.backgroundSprite.setBlendColor("0.500000 0.500000 0.500000 1.000000");

    //%this.previewWindowInit();
}

/// Creates a grid with the specified dimensions and rows/cols
/// Does not currently handle ImageAsset offsets or strides
function createGridOverlay(%guiControl, %position, %extent, %rows, %columns, %cellSize, %shadeNoneCells)
{
    // Remove any existing grid overly guis
    if (isObject(%guiControl.gridOverlay))
        %guiControl.gridOverlay.delete();
        
    if (%cellSize.x == 0 || %cellSize.y == 0)
        return;   

    %guiControl.gridOverlay = new GuiControl()
    {
        Profile="GUIImageEditorGridBoxProfile";
        HorizSizing="left";
        VertSizing="top";
        Visible="1";
    };
    %guiControl.gridOverlay.setPosition(%position.x, %position.y);
    %guiControl.gridOverlay.setExtent(%extent.x, %extent.y);
    
    // Grey out areas outside the cells if the flag is set
    if (%shadeNoneCells == true)
    {
        %width = %extent.x;
        %height = %extent.y - %cellSize.y * %rows;
        %posX = 0;
        %posY = %extent.y - %height + 1;
        %rowGreyBox = new GuiControl()
        {
            Profile="GUIImageEditorGridGreyBoxProfile";
            HorizSizing="left";
            VertSizing="top";
            Visible="1";
        };
        %rowGreyBox.setPosition(mFloor(%posX), mFloor(%posY));
        %rowGreyBox.setExtent(mFloor(%width), mFloor(%height));
        %guiControl.gridOverlay.addGuiControl(%rowGreyBox);
        
        %width = %extent.x - %cellSize.x * %columns;
        %height = %cellSize.y * %rows + 1;
        %posX = %extent.x - %width + 1;
        %posY = 0;
        %columnGreyBox = new GuiControl()
        {
            Profile="GUIImageEditorGridGreyBoxProfile";
            HorizSizing="left";
            VertSizing="top";
            Visible="1";
        };
        %columnGreyBox.setPosition(mFloor(%posX), mFloor(%posY));
        %columnGreyBox.setExtent(mFloor(%width), mFloor(%height));
        %guiControl.gridOverlay.addGuiControl(%columnGreyBox);
    }
    
    // Create rows
    //%rowWidth = %extent.x;
    %rowWidth = %cellSize.x * %columns + 1;
    %rowHeight = %cellSize.y;
    %posX = 0;
    %posY = 0;
    for (%i = 0; %i < %rows; %i++)
    {
        %rowBox = new GuiControl()
        {
            Profile="GUIImageEditorGridLinesProfile";
            HorizSizing="left";
            VertSizing="top";
            Visible="1";
        };
        %rowBox.setPosition(mFloor(%posX), mFloor(%posY));
        %rowBox.setExtent(mFloor(%rowWidth), mFloor(%rowHeight));
        
        %posY += %cellSize.y;
        
        %guiControl.gridOverlay.addGuiControl(%rowBox);
    }
    
    // Create columns  
    %columnWidth = %cellSize.x;
    %columnHeight = %cellSize.y * %rows + 1;//%columnHeight = %extent.y;
    %posX = 0;
    %posY = 0;
    for (%i = 0; %i < %columns; %i++)
    {
        %columnBox = new GuiControl()
        {
            Profile="GUIImageEditorGridLinesProfile";
            HorizSizing="left";
            VertSizing="top";
            Visible="1";
        };
        %columnBox.setPosition(mFloor(%posX), mFloor(%posY));
        %columnBox.setExtent(mFloor(%columnWidth), mFloor(%columnHeight));
        
        %posX += %cellSize.x;
        
        %guiControl.gridOverlay.addGuiControl(%columnBox);
    }  
    
    // Add the grid to the preview control
    %guiControl.addGuiControl(%guiControl.gridOverlay);
}

/*function ImageEditor::loadPreview(%this, %imageMap)
{  
    // do some sanity checks
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

    // grab the base x and y
    %baseX = %this.baseX;
    %baseY = %this.baseY;

    // grab the rowspace and colspace
    %rowSpace = %this.rowSpace;
    %colSpace = %this.colSpace;

    // default cleared to be false, this is an efficiency check
    // for reuse of static sprites
    %cleared = false;
   
    // first make sure that we have an object as our selected image
    if (isObject(%this.selectedImage))
    {
        // if our image mode is the same and we're in cell mode
        // then check if our previous frame count is greater
        // than our current framecount, if so we can clear
        // those frames & borders that will be un-used
        if (%this.frameCount > %imageMap.getFrameCount())
            %this.clearPreview(%imageMap.getFrameCount());
    } 
   
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
    
    %maxWidth = %srcWidth / %scale;
    %maxHeight = %srcHeight / %scale;

    %frameCount = %imageMap.getFrameCount();
    %this.frameCount = %frameCount;
     
    %sqrt = mSqrt(%frameCount);
    %div = mCeil(%sqrt);

    %rowSpace = (%maxWidth / %div) * 0.05;
    %colSpace = (%maxHeight / %div) * 0.05;

    %this.rowSpace = %rowSpace;
    %this.colSpace = %colSpace;

    if (%rowSpace < 1.5)
        %rowSpace = 1.5;

    if (%colSpace < 1.5)
        %colSpace = 1.5;

    %objWidth = %maxWidth; //(%maxWidth / %div) - (%rowSpace + %this.borderPadding);
    %objHeight = %maxHeight; //(%maxHeight / %div) - (%colSpace + %this.borderPadding);

    //%baseX += %this.borderPadding;
    //%baseY += %this.borderPadding;   

    %posX = %baseX; // - (%maxWidth/2) + (%objWidth/2);
    %posY = %baseY; // - (%maxHeight/2) + (%objHeight/2);
    %posY *= -1;

    %colCount = 0;
    %rowCount = 0;

    %this.sprite = new sprite() { scene = $ImageEditorScene; }; 
    %this.sprite.setSize(%objWidth, %objHeight);
    %this.sprite.setPosition(%posX, %posY);
    %this.sprite.setSceneLayer(%this.previewLayer); 
    %assetQuery = new AssetQuery();            
    AssetDatabase.findAssetName(%assetQuery, %imageMap.AssetName);
    %asset = %assetQuery.getAsset(0);
    %assetQuery.delete(); 
    
    %this.sprite.setImageMap(%asset);
    
    //%tempImageAsset = new ImageAsset();
    //%tempImageAsset.ImageFile = %imageMap.ImageFile;
    //%userAssetModule = ModuleDatabase.findModule("{UserAssets}", 1);
    //AssetDatabase.addSingleDeclaredAsset(%userAssetModule, expandPath("^{UserAssets}/audio/" @ %soundProfile.AssetName @ ".asset.taml"));
    //%this.sprite.setImageMap(%tempImageAsset);

    
     
    for(%i = 0; %i < %frameCount; %i++)
    {
        if (%cleared || !isObject(%this.sprite[%i]))
        {
            %this.sprite[%i] = new sprite() { scene = $ImageEditorScene; };
            %this.border[%i] = %this.createImageBorder();
        } 
     
        %this.sprite[%i].setSize(%objWidth, %objHeight);
        %this.sprite[%i].setPosition(%posX, %posY);
        %this.sprite[%i].setSceneLayer(%this.previewLayer);
        
        %assetQuery = new AssetQuery();            
        AssetDatabase.findAssetName(%assetQuery, %imageMap.AssetName);
        
        %this.sprite[%i].setImageMap(%assetQuery.getAsset(0), %i);
        %assetQuery.delete();
        
        %this.sprite[%i].row = %rowCount;
        %this.sprite[%i].col = %colCount;
     
        %this.resizeImageBorder(%this.border[%i], %this.sprite[%i].getPosition(), %this.sprite[%i].getSize(), %this.borderPadding);
        
        if (%colCount >= %div-1)
        {
            %rowCount++;
            %colCount = 0;
            %posX = %baseX - (%maxWidth/2) + (%objWidth/2);
            %posY = %baseY - (%maxHeight/2) + (%objHeight/2) + ((%objHeight + %rowSpace + %this.borderPadding) * %rowCount);   
            %posY *= -1;
        }
        else
        {
            %colCount++;
            %posX = %baseX - (%maxWidth/2) + (%objWidth/2) + ((%objWidth + %colSpace + %this.borderPadding) * %colCount);
        }
    }
   
    loadImageMapSettings(%imageMap);

    ImageEditor.loadingPreview = false;
}*/

function ImageEditor::createImageBorder(%this)
{  
    %borderObj = new SceneObject()
    {
        scene = $ImageEditorScene;
        border = true;
    };

    %borderObj.leftLine = %this.createBorderLine();
    %borderObj.rightLine = %this.createBorderLine();
    %borderObj.topLine = %this.createBorderLine();
    %borderObj.bottomLine = %this.createBorderLine();

    return %borderObj;
}

function ImageEditor::createBorderLine(%this)
{
    %line = new sprite()
    {
        scene = $ImageEditorScene;
        border = true;
    };

    %assetQuery = new AssetQuery();
    AssetDatabase.findAssetName(%assetQuery, "ImageBuilderBackgroundImageMap");
    %assetId = %assetQuery.getAsset(0);
    %assetQuery.delete();

    %line.setSceneLayer(%this.borderLayer);
    %line.ImageMap = %assetId;
    %line.setBlendColor("1 1 1 1");
    %line.setVisible(false);

    return %line;   
}

function ImageEditor::safeDeleteBorder(%this, %border)
{
    %border.leftLine.safeDelete();   
    %border.rightLine.safeDelete();
    %border.topLine.safeDelete();
    %border.bottomLine.safeDelete();
    %border.safeDelete();
}

function ImageEditor::resizeImageBorder(%this, %borderObj, %pos, %size, %borderPadding)
{
    %sizeX = getWord(%size, 0);   
    %sizeY = getWord(%size, 1);

    %borderSizeX = %sizeX + %borderPadding;
    %borderSizeY = %sizeY + %borderPadding;
    %borderSize = %borderSizeX SPC %borderSizeY;   

    %borderObj.setSize(%borderSize);
    //%borderObj.mount(%obj);

    %objPosX = getWord(%pos, 0);
    %objPosY = getWord(%pos, 1);

    %lineWidth = %borderPadding / 2;
    %lineHeight = %borderPadding / 2;

    %borderObj.leftLine.setSize(%lineWidth, %sizeY + (%lineHeight * 2));
    %borderObj.rightLine.setSize(%lineWidth, %sizeY + (%lineHeight * 2));
    %borderObj.topLine.setSize(%sizeX + (%lineWidth * 2), %lineHeight);
    %borderObj.bottomLine.setSize(%sizeX + (%lineWidth * 2), %lineHeight);

    %borderObj.leftLine.setPosition((%objPosX - (%lineWidth / 2) - (%sizeX / 2)), (%objPosY));
    %borderObj.rightLine.setPosition((%objPosX + (%sizeX / 2) + (%lineWidth / 2)),(%objPosY));
    %borderObj.topLine.setPosition((%objPosX),(%objPosY - (%lineHeight / 2) - (%sizeY / 2)));
    %borderObj.bottomLine.setPosition((%objPosX),(%objPosY + (%sizeY / 2) + (%lineHeight / 2)));

    %borderObj.leftLine.setVisible(true);
    %borderObj.rightLine.setVisible(true);
    %borderObj.topLine.setVisible(true);
    %borderObj.bottomLine.setVisible(true);
}

// --------------------------------------------------------------------
// ImageEditor::clearPreview()
//
// This will clear out the preview appropriately based on the image mode.
// --------------------------------------------------------------------
function ImageEditor::clearPreview(%this, %fromFrame)
{
    %imageMap = %this.selectedImage;
    %frameCount = %imageMap.getFrameCount();
   
    if (%fromFrame $= "")
        %start = 0;

    else
        %start = %fromFrame;


    for(%i = %start; %i < %frameCount; %i++)
    {
        if (isObject(%this.sprite[%i]))
            %this.sprite[%i].safeDelete();  

        if (isObject(%this.border[%i]))   
            %this.safeDeleteBorder(%this.border[%i]);
    }
}

function ImageEditor::toggleBackgroundInfo(%this)
{
    if (ImageBuilderBackgroundInfoControl.isVisible())
    {
        %this.BackgroundInfoToggled = false;
        ImageBuilderBackgroundInfoControl.setVisible(false);  
        ImageBuilderBackgroundInfoWindow.setVisible(false);    
    }
    else
    {
        ImageBuilderBackgroundInfoControl.selectPage(0);
        %this.BackgroundInfoToggled = true;
        ImageBuilderBackgroundInfoControl.setVisible(true);
        ImageBuilderBackgroundInfoWindow.setVisible(true);
        ImageEditor.setupBackgroundBasicColors();
        ImageEditor.setupObjectBorderBasicColors();

        ImageBuilderBackgroundInfoControl.selectPage(0);
    }
}

function ImageEditor::setupBackgroundBasicColors(%this)
{
    %this.backgroundBasicColorWindow = ImageBuilderBackgroundBasicColorSceneWindow;

    %extent = %this.backgroundBasicColorWindow.getExtent();

    %this.basicColorsborderPadding = 3;

    %this.basicColorsMaxWidth = getWord(%extent, 0);
    %this.basicColorsMaxHeight = getWord(%extent, 1);

    %this.basicColorscolSpace = %this.basicColorsmaxWidth * 0.015;

    %this.basicColorsbaseX = 2000;
    %this.basicColorsbaseY = 2000;

    %this.backgroundBasicColorWindow.setScene( $ImageEditorScene );
    %this.backgroundBasicColorWindow.setCurrentCameraPosition(%this.basicColorsbaseX SPC %this.basicColorsbaseY SPC %this.basicColorsmaxWidth SPC %this.basicColorsmaxHeight);   

    %colors = 6;
    %color[0] = "1 0 0";
    %color[1] = "0 1 0";
    %color[2] = "0 0 1";
    %color[3] = "0 0 0";
    %color[4] = "0.5 0.5 0.5";
    %color[5] = "1 1 1";

    %colSpace = (%this.basicColorsmaxWidth / %colors) * 0.05;

    if (%colSpace < 1.5)
        %colSpace = 1.5;

    %maxWidth = %this.basicColorsmaxWidth;
    %maxheight = %this.basicColorsmaxHeight;

    %objWidth = (%maxWidth / %colors) - (%colSpace + %this.basicColorsborderPadding);
    %objHeight = (%maxHeight) - (%this.basicColorsborderPadding);
    %objHeight = %objWidth;

    %baseX = %this.basicColorsbaseX;
    %baseY = %this.basicColorsbaseY;

    %baseX += %this.basicColorsborderPadding;
    %baseY += %this.basicColorsborderPadding;   

    %posY = %baseY - (%maxHeight/2) + (%objHeight/2);
   
    %assetQuery = new AssetQuery();
    AssetDatabase.findAssetName(%assetQuery, "ImageBuilderBackgroundImageMap");
    %assetId = %assetQuery.getAsset(0);
    %assetQuery.delete();
    
    for(%i=0;%i<%colors;%i++)
    {
        %posX = %baseX - (%maxWidth/2) + (%objWidth/2) + ((%objWidth + %colSpace + %this.basicColorsborderPadding) * %i);

        %colorBox = new sprite()
        {
            scene = $ImageEditorScene;
        };

        %colorBox.setSize(%objWidth, %objHeight);
        %colorBox.setPosition(%posX, %posY);
        %colorBox.ImageMap = %assetId;
        %colorBox.setBlendColor(%color[%i]);
    }
}

function ImageEditor::setupObjectBorderBasicColors(%this)
{
    %this.objectBorderBasicColorWindow = ImageBuilderObjectBorderBasicColorSceneWindow;

    %extent = %this.objectBorderBasicColorWindow.getExtent();

    %this.objectBorderbasicColorsborderPadding = 3;

    %this.objectBorderbasicColorsMaxWidth = getWord(%extent, 0);
    %this.objectBorderbasicColorsMaxHeight = getWord(%extent, 1);

    %this.objectBorderbasicColorscolSpace = %this.objectBorderbasicColorsmaxWidth * 0.015;

    %this.objectBorderbasicColorsbaseX = 3000;
    %this.objectBorderbasicColorsbaseY = 3000;

    %this.objectBorderBasicColorWindow.setScene( $ImageEditorScene );
    %this.objectBorderBasicColorWindow.setCurrentCameraPosition(%this.objectBorderbasicColorsbaseX SPC %this.objectBorderbasicColorsbaseY SPC 
        %this.objectBorderbasicColorsmaxWidth SPC %this.objectBorderbasicColorsmaxHeight);   
                                                         
    %colors = 6;
    %color[0] = "1 0 0";
    %color[1] = "0 1 0";
    %color[2] = "0 0 1";
    %color[3] = "0 0 0";
    %color[4] = "0.5 0.5 0.5";
    %color[5] = "1 1 1";
   
    %colSpace = (%this.objectBorderbasicColorsmaxWidth / %colors) * 0.05;

    if (%colSpace < 1.5)
        %colSpace = 1.5;

    %maxWidth = %this.objectBorderbasicColorsmaxWidth;
    %maxheight = %this.objectBorderbasicColorsmaxHeight;

    %objWidth = (%maxWidth / %colors) - (%colSpace + %this.objectBorderbasicColorsborderPadding);
    %objHeight = (%maxHeight) - (%this.objectBorderbasicColorsborderPadding);
    %objHeight = %objWidth;

    %baseX = %this.objectBorderbasicColorsbaseX;
    %baseY = %this.objectBorderbasicColorsbaseY;

    %baseX += %this.objectBorderbasicColorsborderPadding;
    %baseY += %this.objectBorderbasicColorsborderPadding;   
         
    %posY = %baseY - (%maxHeight/2) + (%objHeight/2);

    %assetQuery = new AssetQuery();
    AssetDatabase.findAssetName(%assetQuery, "ImageBuilderBackgroundImageMap");
    %assetId = %assetQuery.getAsset(0);
    %assetQuery.delete();
      
    for(%i=0;%i<%colors;%i++)
    {
        %posX = %baseX - (%maxWidth/2) + (%objWidth/2) + ((%objWidth + %colSpace + %this.objectBorderbasicColorsborderPadding) * %i);
        %colorBox = new sprite()
        {
            scene = $ImageEditorScene;
        };
        %colorBox.setSize(%objWidth, %objHeight);
        %colorBox.setPosition(%posX, %posY);
        %colorBox.ImageMap = %assetId;
        %colorBox.setBlendColor(%color[%i]);
    }
}

function ImageBuilderBackgroundInfoControl::onTabSelected(%this, %text)
{
    if (%text $= "Background Color")
        ImageEditor.toggleBackgroundTab();
    else
        ImageEditor.toggleObjectBorderTab();
}


function ImageEditor::toggleBackgroundTab(%this)
{
    if (!ImageBuilderGui.isAwake())
        return;

    if (%this.backgroundColorPickerToggled)
        %this.unHideBackgroundColorPicker();            
    else
        %this.hideBackgroundColorPicker();      
}

function ImageEditor::toggleObjectBorderTab(%this)
{
    if (!ImageBuilderGui.isAwake())
        return;

    if (%this.ObjectBorderColorPickerToggled)
         %this.unHideObjectBorderColorPicker();            
    else
        %this.hideObjectBorderColorPicker();      
}

function ImageEditor::hideBackgroundColorPicker(%this)
{
    ImageBuilderBackgroundColorPickerControl.setVisible(false);

    ImageBuilderBackgroundInfoControl.setExtent(338, 151);
    ImageBuilderBackgroundInfoWindow.setExtent(347, 181);
    ImageBuilderBackgroundTabPage.setExtent(331, 127);

    ImageBuilderColorPickerButton.setText("More Options");

    %this.backgroundColorPickerToggled = false;   
}

function ImageEditor::unHideBackgroundColorPicker(%this)
{
    ImageBuilderBackgroundColorPickerControl.setVisible(true);

    ImageBuilderBackgroundInfoControl.setExtent(338, 314);
    ImageBuilderBackgroundInfoWindow.setExtent(347, 348);   

    ImageBuilderColorPickerButton.setText("Less Options");
    %this.backgroundColorPickerToggled = true;   
}

function ImageEditor::toggleBackgroundColorPicker(%this)
{
    if (ImageBuilderBackgroundColorPickerControl.isVisible())
        %this.hideBackgroundColorPicker();
    else
        %this.unHideBackgroundColorPicker();
}

function ImageEditor::hideObjectBorderColorPicker(%this)
{
    ImageBuilderObjectBorderColorPickerControl.setVisible(false);   

    ImageBuilderBackgroundInfoControl.setExtent(338, 151);
    ImageBuilderBackgroundInfoWindow.setExtent(347, 181);
    ImageBuilderObjectBorderTabPage.setExtent(331, 127);

    ImageBuilderObjectBorderColorPickerButton.setText("More Options");

    %this.objectBorderColorPickerToggled = false;    
}

function ImageEditor::unHideObjectBorderColorPicker(%this)
{
    ImageBuilderObjectBorderColorPickerControl.setVisible(true);

    ImageBuilderBackgroundInfoControl.setExtent(338, 314);
    ImageBuilderBackgroundInfoWindow.setExtent(347, 348); 

    ImageBuilderObjectBorderColorPickerButton.setText("Less Options");

    %this.objectBorderColorPickerToggled = true;  
}

function ImageEditor::toggleObjectBorderColorPicker(%this)
{
    if (ImageBuilderObjectBorderColorPickerControl.isVisible())
        %this.hideObjectBorderColorPicker();
    else
        %this.unHideObjectBorderColorPicker();
}

function ImageEditor::selectBasicColor(%this, %color)
{ 
    $pref::ImageBuilder::backgroundPreviewColor = %color;
    %this.backgroundSprite.fadeToColor(5, %color, 20);   
}

function ImageEditor::selectBasicColorObjectBorder(%this, %color)
{
    %this.setAllBorderColorFade(5, %color, 20);   
}

function ImageEditor::setAllBorderBasicColor(%this, %color)
{
    $pref::ImageBuilder::borderPreviewColor = %color;

    %frameCount = %this.selectedImage.getFrameCount();

    for(%i=0;%i<%frameCount;%i++)
        %this.setBorderBasicColor(%this.border[%i], %color);      
}

function ImageEditor::setBorderBasicColor(%this, %border, %color)
{
    if (!isObject(%border))
        return;

    $pref::ImageBuilder::borderPreviewColor = %color;

    %border.leftLine.setBlendColor(%color);      
    %border.rightLine.setBlendColor(%color);
    %border.topLine.setBlendColor(%color);
    %border.bottomLine.setBlendColor(%color);
}

function ImageEditor::setAllBorderColorFade(%this, %time, %color, %inc)
{
    $pref::ImageBuilder::borderPreviewColor = %color;

    %frameCount = %this.selectedImage.getFrameCount();

    for(%i=0;%i<%frameCount;%i++)
        %this.setBorderColorFade(%this.border[%i], %time, %color, %inc);
}

function ImageEditor::setBorderColorFade(%this, %border, %time, %color, %inc)
{
    if (!isObject(%border.leftLine) && !isObject(%border.rightLine) && !isObject(%border.topLine) && !isObject(%border.bottomLine))
        return;

    %border.leftLine.fadeToColor(%time, %color, %inc);      
    %border.rightLine.fadeToColor(%time, %color, %inc);
    %border.topLine.fadeToColor(%time, %color, %inc);
    %border.bottomLine.fadeToColor(%time, %color, %inc);
}

function ImageBuilderBackgroundBasicColorSceneWindow::onTouchUp( %this, %mod, %worldPos, %mouseClicks )
{
    if (ImageEditor.BackgroundInfoToggled)
    {
        %objList = $ImageEditorScene.pickPoint(%worldPos);
        %objCount = getWordCount(%objList);

        for(%i=0;%i<%objCount;%i++)
        {
            if (!%obj.border)       
            {
                %obj = getWord(%objList, %i);
                break;            
            }
        }

        if (%obj !$= "")
            ImageEditor.selectBasicColor(%obj.getBlendColor());
    }
}

function ImageBuilderObjectBorderBasicColorSceneWindow::onTouchUp( %this, %mod, %worldPos, %mouseClicks )
{
    if (ImageEditor.BackgroundInfoToggled)
    {
        %objList = $ImageEditorScene.pickPoint(%worldPos);
        %objCount = getWordCount(%objList);

        for(%i=0;%i<%objCount;%i++)
        {
            if (!%obj.border)       
            {
                %obj = getWord(%objList, %i);
                break;            
            }
        }

        if (%obj !$= "")
            ImageEditor.selectBasicColorObjectBorder(%obj.getBlendColor());
    }
}

function ImageEditor::onColorPicked(%this, %val)
{
    if (!%val)
    {
        %color = ImageBuilderBackgroundColorPicker.pickColor;

        $pref::ImageBuilder::backgroundPreviewColor = %color;
        %this.backgroundSprite.setBlendColor(%color);   
    }
    else
    {
        %color = ImageBuilderObjectBorderColorPicker.pickColor; 
        %this.setAllBorderBasicColor(%color);
    }
}

function SceneObject::fadeToColor(%this, %time, %red, %green, %blue, %inc)
{
    // store the starting color
    %this.startColorBeforeFade = %this.getBlendColor();

    // grab the starting colors
    %startRed = getWord(%this.startColorBeforeFade, 0);      
    %startGreen = getWord(%this.startColorBeforeFade, 1);
    %startBlue = getWord(%this.startColorBeforeFade, 2);

    // this will allow you to pass in a target color as "0.5 0.5 0.5" as well as "0.5, 0.5, 0.5"
    if (getWord(%red, 2) !$= "")
    {
        %inc = %green;
        %green = getWord(%red, 1);
        %blue = getWord(%red, 2);      
        %red = getWord(%red, 0);
    }

    // if our target colors and  our present colors are the same then we need to do nothing
    if ((%startRed == %red) && (%startGreen == %green) && (%startBlue == %blue))
        return;

    // if we are already doing a color change then cancel it out
    if (isEventPending(%this.colorFadeSchedule))
    cancel(%this.colorFadeSchedule);


    // how many time incriments do we want
    if (%inc $= "" || %inc < 1 || %inc > 100)
        %inc = 10;

    // divide out what each time inc will be
    %timeInc = (%time * 100) / %inc;

    // get the different between starting and ending colors
    %redDiff = %red - %startRed;
    %greenDiff = %green - %startGreen;
    %blueDiff = %blue - %startBlue;

    // get the ammount we must inc each color each scheduled change
    %redInc = %redDiff / %inc;
    %greenInc = %greenDiff / %inc;
    %blueInc = %blueDiff / %inc;

    // call the color change
    %this.fadingColor(%inc, %timeInc, %redInc, %greenInc, %blueInc);
}

function SceneObject::fadingColor(%this, %inc, %timeInc, %redInc, %greenInc, %blueInc, %count)
{
    // init the count if not specified
    if (%count $= "")
        %count = 0;

    // grab the current color
    %color = %this.getBlendColor();

    // divide up the colors and inc them
    %red = getWord(%color, 0) + %redInc;      
    %green = getWord(%color, 1) + %greenInc;
    %blue = getWord(%color, 2) + %blueInc;

    // set the new colors
    %this.setBlendColor(%red, %green, %blue);

    // inc the count
    %count++;

    // if the count is less than the planned incs then schedule this to be called again
    if (%count < %inc)
        %this.colorFadeSchedule = %this.schedule(%timeInc, "fadingColor", %inc, %timeInc, %redInc, %greenInc, %blueInc, %count);
}

function SceneObject::fadeToAlpha(%this, %time, %alpha, %inc, %callback)
{
    // store the starting color
    %this.startAlphaBeforeFade = %this.getBlendAlpha();

    // grab the starting colors
    %startAlpha = %this.startAlphaBeforeFade;    

    // if our target alpha and our present alpha is the same then we need to do nothing
    if (%startAlpha == %alpha)
        return;

    // if we are already doing a color change then cancel it out
    if (isEventPending(%this.alphaFadeSchedule))
        cancel(%this.alphaFadeSchedule);

    // how many time incriments do we want
    if (%inc $= "" || %inc < 1 || %inc > 100)
        %inc = 10;

    // divide out what each time inc will be
    %timeInc = (%time * 100) / %inc;

    // get the different between starting and ending colors
    %alphaDiff = %alpha - %startAlpha;

    // get the ammount we must inc each color each scheduled change
    %alphaInc = %alphaDiff / %inc;

    // call the color change
    %this.fadingAlpha(%inc, %timeInc, %alphaInc, "", %callback);
}

function SceneObject::fadingAlpha(%this, %inc, %timeInc, %alphaInc, %count, %callback)
{
    // init the count if not specified
    if (%count $= "")
        %count = 0;

    // grab the current color
    %alpha = %this.getBlendAlpha();

    // divide up the colors and inc them
    %alpha = %alpha + %alphaInc;      

    // set the new colors
    %this.setBlendAlpha(%alpha);

    // inc the count
    %count++;

    // if the count is less than the planned incs then schedule this to be called again
    if (%count < %inc)
        %this.alphaFadeSchedule = %this.schedule(%timeInc, "fadingAlpha", %inc, %timeInc, %alphaInc, %count, %callback);
    else
        schedule(0, %this, "eval", %callback);

}

function trimAfter(%string, %char)
{  
    %pos = strpos(%string,%char);

    if (%pos > -1)
        %stringToReturn = getSubStr(%string, 0, %pos);
    else
        %stringToReturn = getSubStr(%string, 0, strLen(%string));

    return %stringToReturn;
}