//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------



/// void(AnimationBuilderGui this, GuiControl control, Point2F position)
/// Catch all in case there's something that catches drag and drops beneath us.
/// @param this The gui.
/// @param control The control that was dropped.
/// @param position The position the control was dropped at.
function AnimationBuilderGui::onControlDropped( %this, %control, %position )
{
    if ( ABStoryboardWindow.pointInControl( %position.x, %position.y ) )
        ABStoryboardWindow.onControlDropped( %control, %position );
    if ( %control.spriteClass $= "ABStoryboardPreviewSprite" )
        AnimationBuilder.removeFrame( %control.frameNumber );
}

function AnimationBuilderGui::setSelectedAsset(%this, %assetID)
{
    AnimationBuilder.updating = true;
    AnimationBuilder.setImageMap(%assetID);

    if ($AssetAutoTag !$= "")
    {
        AnimationBuilder.applyTag($AssetAutoTag);

        $AssetAutoTag = "";
    }
}

function AnimationBuilderGui::removeTag(%this, %tag)
{
    %assetTagManifest = AssetDatabase.getAssetTags();
    %assetTagManifest.untag(AnimationBuilder.animationId, %tag);
    ABTagContainer.removeTagItem(%tag);
    AnimationBuilder.updateGui();
}

function AnimationBuilderGui::onWake(%this)
{
    AnimationBuilder.updateGui();
}

function ABStoryboardWindow::onControlDropped(%this, %control, %position)
{
    %dropPosition = Vector2Sub(%position, %this.getGlobalPosition());
    %worldX = getWord(%dropPosition, 0);
    %sizeX = 64 + 6;

    %index = mFloor((%worldX / %sizeX) + 0.5);
    AnimationBuilder.insertFrame( %control.Frame, %index );

    if ( %control.spriteClass $= "ABStoryboardPreviewSprite" )
        AnimationBuilder.removeFrame( %control.frameNumber );
}

function ABAddTagButton::onClick(%this)
{
    AnimationBuilder.addTag(ABTagList.getText());
}

function ABSpeedDownBtn::onClick(%this)
{
    %speed = ABFPSField.getText();
    %frames = mCeil(%speed);
    %frames--;
    ABFPSField.setText(%frames);
    AnimationBuilder.validateFPS();
}

function ABSpeedUpBtn::onClick(%this)
{
    %speed = ABFPSField.getText();
    %frames = mCeil(%speed);
    %frames++;
    ABFPSField.setText(%frames);
    AnimationBuilder.validateFPS();
}

function ABChangeImageClick::onMouseEnter(%this)
{
    ABChangeImageButton.setNormalImage(ABChangeImageButton.HoverImage);
}

function ABChangeImageClick::onMouseLeave(%this)
{
    ABChangeImageButton.setNormalImage(ABChangeImageButton.NormalImageCache);
}

function ABChangeImageClick::onMouseDown(%this)
{
    ABChangeImageButton.setNormalImage(ABChangeImageButton.DownImage);
}

function ABChangeImageClick::onMouseUp(%this)
{
    ABChangeImageButton.setNormalImage(ABChangeImageButton.HoverImage);
    ABChangeImageButton.onClick();
}

function ABChangeImageButton::onClick(%this)
{
    AnimationBuilder.openAssetPicker("ImageAsset" SPC "Any" SPC AnimationBuilderGui.getId());
}

function AnimBuilderHelpButton::onClick(%this)
{
    gotoWebPage("http://docs.3stepstudio.com/" @ $LoadedTemplate.Name @ "/animation/");
}

function ABNameField::onWake(%this)
{
    %this.initialize($ABAssetNameTextEditMessageString);
}

function ABImageMapField::onWake(%this)
{
    %this.initialize($ABAssetLocationTextEditMessageString);
    %this.onReturnFired = false;
}

function ABNameField::onReturn(%this)
{
    if ( %this.onReturnFired )
        return;
    if ( %this.getText() $= $ABAssetNameTextEditMessageString )
        return;
    %this.onReturnFired = true;
    %this.onValidate();
}

function ABNameField::onValidate(%this)
{
    AnimationBuilder.validateName();
}

function ABNameField::filterText(%this)
{
    %temp = %this.getText();
    %cleanName = stripChars(%temp, "-+*/%$&§=()[].?\"#,;:!~<>|°^{}'` ");
    %this.animName = %cleanName;
    %this.setText(%this.animName);
    AnimationBuilder.nameDirty = !%this.onReturnFired;
    return (%temp !$= %cleanName);
}

function ABSaveButton::update(%this)
{
    %active = true;    
    
    // Don't allow saving if the name is blank
    if (ABNameField.isEmpty()) 
        %active = false;

    // Don't allow saving if the image location is blank
    if (ABImageMapField.isEmpty()) 
        %active = false;

    if (AnimationBuilder.tempAnimation $= "")
        %active = false;
    else
    {
        %tempAsset = AssetDatabase.acquireAsset(AnimationBuilder.tempAnimation);
        %frames = getWordCount(%tempAsset.AnimationFrames);
        AssetDatabase.releaseAsset(AnimationBuilder.tempAnimation);
        if (%frames < 1)
            %active = false;
    }
    %this.setActive(%active);
}

function Ab_FillStoryboardBtn::onClick(%this)
{
    AnimationBuilder.setAllFrames();
}

function Ab_ClearStoryboardBtn::onClick(%this)
{
    AnimationBuilder.clearFrames();
}