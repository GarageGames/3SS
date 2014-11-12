//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
function AssetLibrary::open(%this, %tab, %tagFilter, %lockToTag, %cellSpritesOnly)
{
    if ( !isObject(%this.helpManager) )
        %this.helpManager = createHelpMarqueeObject("AssetLibTips", 10000, "{AssetLibrary}");

    %this.helpManager.openHelpSet("assetLibraryHelp");
    %this.helpManager.start();

    activatePackage(AssetLibraryPackage);
    
    // Make sure the filter box is clear.
    ASSearchBox.setText("");

    cleanTemporaryAssets();

    if (%tab !$= "")
    {
        %this.currentPage = %tab;
        %this.lockPage = true;
    }

    if (isFunction(GetTemplateTypesTagExclusions))
        ASTagDropdown.refresh(true, GetTemplateTypesTagExclusions());
    else
        ASTagDropdown.refresh(true);
   
    if (%tagFilter !$= "")
    {
        %this.tagFilter = %tagFilter;
        %entry = ASTagDropdown.findText(%this.tagFilter);
        
        ASTagDropdown.setSelected(%entry);

        ASTagDropdown.setActive(!(%lockToTag == true));
    }
    else
        ASTagDropdown.setActive(true);
   
    if (%requestTool $= "")
    {
        %this.requestTool = "";

        ASOkButton.Visible = 0;

        ASCancelButton.Visible = 0;
        %this.cellSpritesOnly = %cellSpritesOnly;
    }
    else
    {
        %this.requestTool = %requestTool;

        %this.cellSpritesOnly = %cellSpritesOnly;

        ASOkButton.Visible = 1;

        ASCancelButton.Visible = 1;
    }    
   
    // Clear any other tools from the view
    EditorShellGui.clearViews();
    EditorShellGui.addView(AssetLibraryWindow, "");
    
    %this.resetPageButtons();
    
    ASObjectArray.initialize(%this, true, true, false, true);
}

function AssetLibrary::resetPageButtons(%this)
{
    if (%this.lockPage)
    {
        switch(%this.currentPage)
        {
            case $AssetLibrary::SpriteSheetPage:
                ASSpriteSheetButton.setActive(true);
                ASSpriteSheetButton.setStateOn(true);
                
                ASAnimationsButton.setActive(false);
                ASSoundsButton.setActive(false);
                ASGuisButton.setActive(false);
            
                if (isFunction(SetTemplateTypeButtonsActiveState))
                    SetTemplateTypeButtonsActiveState(false);
         
            case $AssetLibrary::AnimatedSpritePage:
                ASAnimationsButton.setActive(true);
                ASAnimationsButton.setStateOn(true);

                ASSpriteSheetButton.setActive(false);
                ASSoundsButton.setActive(false);
                ASGuisButton.setActive(false);

                if (isFunction(SetTemplateTypeButtonsActiveState))
                    SetTemplateTypeButtonsActiveState(false);
         
            case $AssetLibrary::SoundsPage:
                ASSoundsButton.setActive(true);
                ASSoundsButton.setStateOn(true);

                ASSpriteSheetButton.setActive(false);
                ASAnimationsButton.setActive(false);
                ASGuisButton.setActive(false);

                if (isFunction(SetTemplateTypeButtonsActiveState))
                    SetTemplateTypeButtonsActiveState(false);
            
            case $AssetLibrary::GuisPage:
                ASGuisButton.setActive(true);
                ASGuisButton.setStateOn(true);

                ASSpriteSheetButton.setActive(false);
                ASAnimationsButton.setActive(false);
                ASSoundsButton.setActive(false);
                
                if (isFunction(SetTemplateTypeButtonsActiveState))
                    SetTemplateTypeButtonsActiveState(false);

            default:
                if (isFunction(ActivateTemplateTypeButton))
                    ActivateTemplateTypeButton(%this.currentPage);

                if (isFunction(SelectTemplateTypeButton))
                    SelectTemplateTypeButton(%this.currentPage);

                ASSpriteSheetButton.setActive(false);
                ASAnimationsButton.setActive(false);
                ASSoundsButton.setActive(false);
                ASGuisButton.setActive(false);
        }      
    }
    else
    {
        ASSpriteSheetButton.setActive(true);
        ASAnimationsButton.setActive(true);
        ASSoundsButton.setActive(true);
        ASGuisButton.setActive(true);

        if (isFunction(SetTemplateTypeButtonsActiveState))
            SetTemplateTypeButtonsActiveState(true);
      
        switch(%this.currentPage)
        {
            case $AssetLibrary::SpriteSheetPage:
                ASSpriteSheetButton.setStateOn(true);

            case $AssetLibrary::AnimatedSpritePage:
                ASAnimationsButton.setStateOn(true);

            case $AssetLibrary::SoundsPage:
                ASSoundsButton.setStateOn(true);

            case $AssetLibrary::GuisPage:
                ASGuisButton.setStateOn(true);

            default:
                if (isFunction(SelectTemplateTypeButton))
                    SelectTemplateTypeButton(%this.currentPage);
        }      
    }
}

function AssetLibraryWindow::onSleep(%this)
{
    AssetLibrary.close();
}

function AssetLibrary::close(%this)
{
    %this.currentPage = $AssetLibrary::SpriteSheetPage;
    %this.lockPage = false;
    %this.cellSpritesOnly = false;

    if ( isObject(AssetLibrary.helpManager) )
    {
        AssetLibrary.helpManager.stop();
        AssetLibrary.helpManager.delete();
    }

    ASTagDropdown.setActive(true);
    Canvas.popDialog(AssetLibraryGui);
    deactivatePackage(AssetLibraryPackage);
}

function AssetLibrary::destroy(%this)
{
    
}

function AssetLibrary::updateGui(%this)
{
    switch (%this.currentPage)
    {
         case $AssetLibrary::SpriteSheetPage:
            %type = "ImageAsset";
            %category = "";
         case $AssetLibrary::AnimatedSpritePage:
            %type = "AnimationAsset";
            %category = "";
         
         case $AssetLibrary::GuisPage:
            %type = "ImageAsset";
            %category = "gui";
         
         case $AssetLibrary::SoundsPage:
            %type = "AudioAsset";
            %category = "";
    }
    
    ASObjectArray.setFilters(%type, %this.tagFilter, %category);
    ASScrollCtrl.scrollToTop();
    ASScrollCtrl.computeSizes();
}

function AssetLibrary::setPage(%this, %pageID)
{
    if (!%this.lockPage)
    {
        %this.currentPage = %pageID;

        %this.schedule(10, updateGui);
    }
}

function AssetLibraryNewButton::onClick(%this)
{
    %tagFilter = AssetLibrary.tagFilter;

    switch (AssetLibrary.currentPage)
    {
        case $AssetLibrary::SpriteSheetPage:
            CreateNewSpriteSheet(%tagFilter);

        case $AssetLibrary::AnimatedSpritePage:
            CreateNewAnimation(%tagFilter);

        case $AssetLibrary::SoundsPage:
            CreateNewAudioProfile(%tagFilter);

        case $AssetLibrary::GuisPage:
            CreateNewGuiImage(%tagFilter);

        default:
            if (isFunction(CreateNewTemplateType))
                CreateNewTemplateType(AssetLibrary.currentPage, %tagFilter);
    }
}

function ASTagDropdown::onSelect(%this)
{
    AssetLibrary.tagFilter = %this.getText();
    AssetLibrary.schedule(100, updateGui, true);
}

//--------------------------------
// Asset Library Help
//--------------------------------
function AssetLibHelpButton::onClick(%this)
{
    gotoWebPage("http://docs.3stepstudio.com/" @ $LoadedTemplate.Name @ "/assetlibrary/");
}
