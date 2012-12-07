//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function ImagePreviewGui::display(%this, %image)
{
    %temp = AssetDatabase.acquireAsset(%image);
    %size = %temp.getImageSize();
    %this.displayImage(%temp, %size);
    AssetDatabase.releaseAsset(%image);
    Canvas.pushDialog(%this);
}

function ImagePreviewGui::displayImage(%this, %image, %size)
{
    %assetId = %image.getAssetId();    
    %scale = 1;
    if (%size !$= "")
    {
        if (%size.x > Ip_BackgroundContainer.Extent.x || %size.y > Ip_BackgroundContainer.Extent.y)
        {
            %scale = mGetMin(Ip_BackgroundContainer.Extent.x/%size.x, Ip_BackgroundContainer.Extent.y/%size.y);
            

            %width = getWord(%size, 0) * %scale;
            %height = getWord(%size, 1) * %scale;
            %size = %width SPC %height;
        }

        Ip_ImagePreview.Extent = %size;

        %xPos = mRound(Ip_BackgroundContainer.Extent.x / 2) - (mRound(%size.x) / 2);
        %yPos = mRound(Ip_BackgroundContainer.Extent.y / 2) - (mRound(%size.y) / 2);
        
        Ip_ImagePreview.Position = %xPos SPC %yPos;
    }
    %assetInstance = AssetDatabase.acquireAsset(%assetId);
    if (isObject(%assetInstance))
    {
        %imageFile = %image.getImageFile();
        Ip_ImagePreview.setBitmap(%imageFile);
    }
        
    %rowCount = %image.getCellCountY();
    %colCount = %image.getCellCountX();
    %cellWidth = %image.getCellWidth();
    %cellHeight = %image.getCellHeight();

    if (%rowCount > 1 || %colCount > 1)
    {
        // Change the color of the grid profile
        %this.oldBoxBorderColor = GUIImageEditorGridBoxProfile.borderColor;
        %this.oldLinesBorderColor = GUIImageEditorGridLinesProfile.borderColor;
        GUIImageEditorGridBoxProfile.borderColor = "100 100 100 255";
        GUIImageEditorGridLinesProfile.borderColor = "100 100 100 255";     
        
        createGridOverlay(Ip_ImagePreview, %posX SPC %posY, %size.x SPC %size.y, %rowCount, %colCount, (%cellWidth / %scale) SPC (%cellHeight / %scale), false);
    }
}

function Ip_DoneBtn::onClick(%this)
{
    Canvas.popDialog(ImagePreviewGui);
    
    // Restore old grid profiles
    GUIImageEditorGridBoxProfile.borderColor = ImagePreviewGui.oldBoxBorderColor;
    GUIImageEditorGridLinesProfile.borderColor = ImagePreviewGui.oldLinesBorderColor;
}