//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function AssetViewer::onWake(%this)
{
    if (%this.getClassName() !$= "GuiDynamicCtrlArrayControl")
    {
        error("### AssetViewer class assigned to wrong GUI control type!");
        error("### AssetViewer must be a GuiDynamicCtrlArrayControl");
        return;
    }
}

function AssetViewer::initialize(%this, %requestingTool, %allowEdit, %allowDelete, %allowSelection, %allowFrameSwitching)
{
    %this.requestingTool = %requestingTool;
    
    %this.allowEditing = true;
    %this.allowDelete = true;
    %this.allowSelection = true;
    %this.allowFrameSwitching = true;
 
    %this.assetCount = 0;
    %this.containerCount = 0;
    %this.updateFrequency = 25;
    %this.allowEditing = %allowEdit;
    %this.allowDelete = %allowDelete;
    %this.allowSelection = %allowSelection;
    %this.allowFrameSwitching = %allowFrameSwitching;
    %this.searchString = "";
    
    if (!isObject(%this.baseQuery))
        %this.baseQuery = new AssetQuery();
    else
        %this.baseQuery.clear();

    if (%this.audioProfileAsset $= "")
        %this.audioProfileAsset = "{EditorAssets}:iconAudioSampleImageMap";
        
    AssetDatabase.findAllAssets(%this.baseQuery, true);
}

function AssetViewer::destroy(%this)
{
    if (isObject(%this.baseQuery))
        %this.baseQuery.delete();
    
    if (isObject(%this.tempQuery))
        %this.tempQuery.delete();
            
    %this.clearView();
    
    %this.requestingTool = "";
    %this.assetCount = 0;
    %this.containerCount = 0;
    %this.allowEditing = false;
    %this.allowDelete = false;
    %this.allowSelection = false;
    %this.allowFrameSwitching = false;
}

function AssetViewer::setSearchString(%this, %text)
{
    ASScrollCtrl.scrollToTop();
    %this.searchString = %text;
    %this.assetCount = 0;
    %this.updateQuery();
    %this.prepareAssetContainers();
    %this.update();
}

function AssetViewer::setFilters(%this, %assetType, %tag, %category)
{
    %this.assetCount = 0;
    %this.containerCount = 0;
    %this.assetType = %assetType;
    %this.assetTag = %tag;
    %this.category = %category;
    
    if (!isObject(%this.tempQuery))
        %this.tempQuery = new AssetQuery();
    else
        %this.tempQuery.clear();
    
    %this.updateQuery();
            
    %this.prepareAssetContainers();
    
    %this.update();
}

function AssetViewer::updateQuery(%this)
{
    %this.tempQuery.clear();

    // And now back to our regularly scheduled show!
    if ( !isObject(%this.assetList) )
        %this.assetList = new SimSet();

    %this.assetList.clear();

    %count = getWordCount(%this.assetType);

    AssetDatabase.findAssetName(%this.tempQuery, %this.searchString, true);
    AssetDatabase.findAssetInternal(%this.tempQuery, false, true);

    if (%this.category !$= "")
        AssetDatabase.findAssetCategory(%this.tempQuery, %this.category, true);
        
    if (%this.assetTag !$= "Any" && %this.assetTag !$= "")
        AssetDatabase.findTaggedAssets(%this.tempQuery, %this.assetTag, true);
    
    //if ( %this.assetType !$= "" && %count <= 1 )
    //{
        //AssetDatabase.findAssetType(%this.tempQuery, %this.assetType, true);
        //%assetCount = %this.tempQuery.getCount();
        //for (%i = 0; %i < %assetCount; %i++)
        //{
            //%listObj = new ScriptObject();
            //%listObj.assetName = %this.tempQuery.getAsset(%i);
            //%this.assetList.add(%listObj);
        //}
    //}
    //else
    //{
        %holdQuery = new AssetQuery();
        %holdQuery.set(%this.tempQuery);
        for (%i = 0; %i < %count; %i++)
        {
            %assetType = getWord(%this.assetType, %i);
            AssetDatabase.findAssetType(%this.tempQuery, %assetType, true);
            %assetCount = %this.tempQuery.getCount();
            for (%j = 0; %j < %assetCount; %j++)
            {
                %listObj = new ScriptObject();
                %listObj.assetName = %this.tempQuery.getAsset(%j);
                
                if( AssetDatabase.isAssetInternal(%listObj.assetName) )
                    continue;

                %this.assetList.add(%listObj);
            }
            %this.tempQuery.set(%holdQuery);
        }
    //}
}

function AssetViewer::prepareAssetContainers(%this)
{
    %this.clearView();
    %objCount = %this.assetList.getCount();
    for(%i = 0; %i < %objCount; %i++)
    {
        %assetId = %this.assetList.getObject(%i).assetName;
            
        %caption = AssetDatabase.getAssetName(%assetId);
        %type = AssetDatabase.getAssetType(%assetId);
        
        // Build T2D Object Container
        %container = new GuiSpriteCtrl() 
        { 
            class = "AssetPreviewControl";
            Image = "{UtilityGui}:AssetBackgroundImage";
            Profile = GuiTransparentProfile;
        };
        
        %assetPreview = new GuiSpriteCtrl()
        {
            Profile = GuiTransparentProfile;
            extent = (%this.colSize - 12) SPC (%this.colSize - 34);
            position = "5 27";
        };
        
        %assetPreview.command = "echo(\"test test\");";
       
        // -- object caption
        %containerCaption = new GuiTextCtrl() 
        {
            profile = ALAssetNameFontProfile;
            position = "10 4";
            extent = (%this.colSize - 20) SPC 20;      
            text = %caption;
        };
       
        %container.caption = %containerCaption;
        %container.add(%containerCaption);
        
        %container.assetPreview = %assetPreview;
        %container.add(%assetPreview);
       
        // Border and Input Handler
        %borderButton = new GuiImageButtonCtrl() 
        { 
            class = "AssetViewerButton";
            position = "0 0";
            extent = %this.colSize SPC %this.rowSize;
            NormalImage = "{UtilityGui}:AssetBorderButtonNormalImage";
            DownImage = "{UtilityGui}:AssetBorderButtonDepressedImage";
            Profile = GuiTransparentProfile;
            groupNum = "1";
            ButtonType = "RadioButton";
            assetID = %assetId;
            toolType = %type;
            useMouseEvents = true;
        };
       
        %borderButton.command = %this @ ".makeSelection(" @ %container @ ");";
        
        %borderButton.assetPreview = %assetPreview;
        %borderButton.assetViewer = %this;
        %container.button = %borderButton;    
        %container.add(%borderButton);
        
        if (%this.allowSelection)
        {
            // Select Button
            %selectButton = new GuiButtonCtrl()
            {
                position = (%this.colSize - 120) SPC (%this.rowSize - 40);
                extent = "100 26";
                Profile = GreenButtonProfile;
                ButtonType = "PushButton";
                text = "Select";
                Visible = false;
                toolTipProfile="GuiToolTipProfile";
                toolTip="Select this asset.";
            };
            
            %selectButton.command = %this @ ".returnSelection(" @ %borderButton @ ");";
            
            %container.selectButton = %selectButton;
            %container.add(%selectButton);
        }
        
        if (%this.allowDelete)
        {
            // Delete Button
            %deleteButton = new GuiButtonCtrl()
            {
                position = (%this.colSize - 68) SPC (%this.rowSize - 40);
                extent = "50 24";
                Profile = RedButtonProfile;
                ButtonType = "PushButton";
                Text = "Delete";
                Visible = false;
                toolTipProfile="GuiToolTipProfile";
                toolTip="Press to delete the asset from the Asset Library.  Note: This does not remove the file from your computer.";
            };
            
            %deleteButton.command = %this @ ".showDeletePrompt();";
            
            %container.deleteButton = %deleteButton;
            %container.add(%deleteButton);
        }
       
        if (%this.allowEditing)
        {
            // Edit Button
            %editButton = new GuiButtonCtrl()
            {
                position = "15" SPC (%this.rowSize - 40);
                extent = "50 24";
                Profile = BlueButtonProfile;
                ButtonType = "PushButton";
                text = "Edit";
                Visible = false;
                toolTipProfile="GuiToolTipProfile";
                toolTip="Press to edit the selected asset.";
            };
           
            %editButton.command = %this @ ".editSelection();";
           
            %container.editButton = %editButton;
            %container.add(%editButton);
        }
       
        if (%type $= "ImageAsset")
            %iconImage = "{UtilityGui}:SmallImageIconImage";
        else if (%type $= "AnimationAsset")
            %iconImage = "{UtilityGui}:SmallAnimIconImage";
        else if (%type $= "AudioAsset")
            %iconImage = "{UtilityGui}:SmallSoundIconImage";
        
        // Asset Icon
        %icon = new GuiSpriteCtrl()
        {
            Position = "125 6";
            Extent = "16 16";
            Image = %iconImage;
            Profile = GuiTransparentProfile;
            Visible = true;
        };
       
        
        %container.icon = %icon;
        %container.add(%icon);
        
        // Add to list.
        %this.add(%container);
        %this.containerCount++;
    }
}

function AssetViewer::addAssetPreview(%this, %assetId)
{
    %container = %this.getObject(%this.assetCount);
    %type = AssetDatabase.getAssetType(%assetId);
    
    switch$(%type)
    {
        case "ImageAsset":
            %container.assetPreview.Image = %assetID;            
        
        case "AnimationAsset":
            %container.assetPreview.Animation = %assetID;
                
        case "AudioAsset":
             %container.assetPreview.Image = %this.audioProfileAsset;
             %container.assetPreview.Frame = 0;
    }    
    
    if ( %type $= "ImageAsset" && %this.allowFrameSwitching)
    {
        if (getImageCellCount(%assetId) > 1)
        {
            %assetPreview.Frame = 0;
            
            %decrementFrameButton = new GuiImageButtonCtrl()
            {
                Profile = "GuiButtonProfile";
                HorizSizing = "right";
                VertSizing = "bottom";
                Position = "5 65";
                Extent = "16 18";
                MinExtent = "8 2";
                hovertime = "100";
                tooltip = "Press to move one frame backward.";
                tooltipProfile = "GuiToolTipProfile";
                NormalImage = "{UtilityGui}:DecrementFrameButtonNormalImage";
                DownImage = "{UtilityGui}:DecrementFrameButtonDepressedImage";
                HoverImage = "{UtilityGui}:DecrementFrameButtonHighlightImage";
                InactiveImage = "{UtilityGui}:DecrementFrameButtonInactiveImage";
                Visible = true;
            };
          
            %decrementFrameButton.command = %container.button @ ".decrementFrame();";

            %borderButton.decrementFrameButton = %decrementFrameButton;

            %incrementFrameButton = new GuiImageButtonCtrl()
            {
                Profile = "GuiButtonProfile";
                HorizSizing = "right";
                VertSizing = "bottom";
                Position = "130 65";
                Extent = "16 18";
                MinExtent = "8 2";
                hovertime = "100";
                tooltip = "Press to move one frame forward.";
                tooltipProfile = "GuiToolTipProfile";
                NormalImage = "{UtilityGui}:IncrementFrameButtonNormalImage";
                DownImage = "{UtilityGui}:IncrementFrameButtonDepressedImage";
                HoverImage = "{UtilityGui}:IncrementFrameButtonHighlightImage";
                InactiveImage = "{UtilityGui}:IncrementFrameButtonInactiveImage";
                Visible = true;
            };
          
            %incrementFrameButton.command = %container.button @ ".incrementFrame();";
          
            %borderButton.incrementFrameButton = %incrementFrameButton;
          
            %container.add(%decrementFrameButton);
            %container.add(%incrementFrameButton);            
        }   
    }
}

function AssetViewer::update(%this)
{
    if (%this.assetCount >= %this.assetList.getCount())
        return;
    
    %assetId = %this.assetList.getObject(%this.assetCount).assetName;
    
    %this.addAssetPreview(%assetId);
    %this.assetCount++;
    
    %this.updateSchedule = %this.schedule(%this.updateFrequency, "update");
}

function AssetViewer::clearView(%this)
{
    while(%this.getCount() > 0)
    {
        %object = %this.getObject(0);
        
        if (isObject(%object) )
            %object.delete();
        else
            %this.remove(%object);
    }
}

function AssetViewer::makeSelection(%this, %selectedContainer)
{
    %this.clearSelections();

    %selectedContainer.button.setStateOn(true);

    if (isObject(%selectedContainer.decrementFrameButton))
    {
        %selectedContainer.decrementFrameButton.setVisible(true);
        %selectedContainer.incrementFrameButton.setVisible(true);
    }
    
    if (isObject(%selectedContainer.selectButton) && %this.allowSelection)
        %selectedContainer.selectButton.setVisible(true);
   
    if (isObject(%selectedContainer.editButton) && %container.assetTag !$= "Gui")
        %selectedContainer.editButton.setVisible(true);

    if (isObject(%selectedContainer.deleteButton) && %this.allowDelete)
        %selectedContainer.deleteButton.setVisible(true);

    %this.selectedAsset = %selectedContainer.button.assetID;
    %this.selectedContainer = %selectedContainer;
}

function AssetViewer::showDeletePrompt(%this)
{
    %module = AssetDatabase.getAssetModule(%this.selectedAsset).ModuleId;
    
    if (%module !$= "{UserAssets}")
    {
        NoticeGui.display("You cannot delete default assets.");
        return;
    }
        
    %name = AssetDatabase.getAssetName(%this.selectedAsset);
    %data = %this.selectedAsset;
    
    ConfirmDeleteGui.display("Are you sure you want to delete " @ %name @ "?", %this, deleteAsset, %data);
}

function AssetViewer::deleteAsset(%this, %assetID)
{
    // Find all assets that depend on what we are deleting
    %referenceQuery = new AssetQuery();
    %referenceCount = AssetDatabase.findAssetIsDependedOn(%referenceQuery, %assetID);
    
    // If references were found... 
    if (%referenceCount > 0)
    {
        // ...loop through and delete each one
        for (%i = 0; %i < %referenceCount; %i++)
        {
            // Get the actual asset so we can locate its source file
            %referenceAssetID = %referenceQuery.getAsset(%i);
            
            // Release the reference asset
            AssetDatabase.deleteAsset(%referenceAssetID, true, true);
        }
    }
    
    AssetDatabase.deleteAsset(%assetID, true, true);
    %this.setFilters(%this.assetType, %this.assetTag, %this.category, %this.celledImagesOnly);
}

function AssetViewer::returnSelection(%this, %button)
{
    if (isObject(%this.requestingTool))
    {
        %this.requestingTool.setSelectedAsset(%this.selectedAsset, %button.assetPreview.Frame);
        return;
    }
    else
    {
        error("### Error in AssetViewer::returnSelection. Invoker does not exist");
    }
}

function AssetViewer::editSelection(%this)
{
    %assetType = AssetDatabase.getAssetType(%this.selectedAsset);
    
    switch$(%assetType)
    {
        case "ImageAsset":
            EditSpriteSheet(%this.selectedAsset);
        
        case "AnimationAsset":
            EditAnimation(%this.selectedAsset);

        case "AudioAsset":
            EditSoundProfile(%this.selectedAsset);
    }
}

function AssetViewer::clearSelections(%this)
{
    %count = %this.getCount();

    for (%i = 0; %i < %count; %i++)
    {
        %container = %this.getObject(%i);
        %borderButton = %container.button;

        %borderButton.setStateOn(false);

        if (isObject(%container.decrementFrameButton))
        {
            %container.incrementFrameButton.setVisible(false);
            %container.decrementFrameButton.setVisible(false);
        }
        
        if (isObject(%container.selectButton) && %this.allowSelection)
            %container.selectButton.setVisible(false);

        if (isObject(%container.editButton) && %container.assetTag !$= "Gui")
            %container.editButton.setVisible(false);
        
        if (isObject(%container.deleteButton) && %this.allowDelete)
            %container.deleteButton.setVisible(false);
    }
}


function AssetViewerButton::incrementFrame(%this)
{
    %currentFrame = %this.assetPreview.Frame;
    
    %totalFrames  = getImageCellCount(%this.assetId);

    if (%currentFrame >= (%totalFrames - 1))
        %nextFrame = 0;
    else
        %nextFrame = %currentFrame + 1;

    %currentFrame = %nextFrame;

    %this.assetPreview.Frame = %currentFrame;
}

function AssetViewerButton::onMouseUp(%this, %modifier, %point, %clickCount)
{
    if (%clickCount > 1 && %this.assetViewer.allowSelection)
        %this.assetViewer.returnSelection();
}

function AssetViewerButton::decrementFrame(%this)
{  
    %currentFrame = %this.assetPreview.Frame;
    
    %totalFrames  = getImageCellCount(%this.assetId);

    if (%currentFrame <= 0)
        %nextFrame = %totalFrames - 1;
    else
        %nextFrame = %currentFrame - 1;

    %currentFrame = %nextFrame;

    %this.assetPreview.Frame = %currentFrame;
}