//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function AssetPicker::open(%this, %assetType, %tagFilter, %category, %requestingTool)
{
    activatePackage(AssetPickerPackage);
    
    %this.category = %category;
    %this.assetType = %assetType;
    %this.tagFilter = %tagFilter;
    %this.requestingTool = %requestingTool;
    
    APAssetArray.initialize(%this, false, false, true, true);
    
    Canvas.pushDialog(AssetPickerParent);
    
    APTagDropdown.refresh(true, "");
    %this.updateGui();
}

function AssetPicker::close(%this)
{
    Canvas.popDialog(AssetPickerParent);
    deactivatePackage(AssetPickerPackage);
}

function AssetPicker::updateGui(%this)
{
    APAssetArray.setFilters(%this.assetType, %this.tagFilter, %this.category);
    APScrollCtrl.scrollToTop();
    APScrollCtrl.computeSizes();
}

function AssetPicker::setSelectedAsset(%this, %assetId, %frame)
{
    if (isObject(%this.requestingTool))
    {
        %this.requestingTool.setSelectedAsset(%assetId, %frame);
        %this.close();
        return;
    }
    else
    {
        error("### Error in AssetPicker::setSelectedAsset. %this.requestingTool does not exist");
    }
}

function APTagDropdown::onSelect(%this)
{
    AssetPicker.tagFilter = %this.getText();
    AssetPicker.schedule(100, updateGui);
}