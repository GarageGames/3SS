//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior plays an animation on a raised signal
/// </summary>
if (!isObject(AnimationEffectBehavior))
{
    %template = new BehaviorTemplate(AnimationEffectBehavior);

    %template.friendlyName = "AnimationEffectBehavior";
    %template.behaviorType = "Effect";
    %template.description  = "Play an animation for object events.";

    %template.addBehaviorField(asset, "The effect's assigned asset", asset, "");
    %template.addBehaviorField(frame, "The frame number for the imageMap", int, 0);
    %template.addBehaviorField(instanceName, "The animation name.", string, "");

    %template.addBehaviorInput(Play, "Play animation", "Tells the behavior that it should play its animation.");
}

/// <summary>
/// This input receives the PlayAnimation signal to play an animation
/// </summary>
/// <param name="fromBehavior">The behavior that raised the signal</param>
/// <param name="fromOutput">The output that originated the signal</param>
function AnimationEffectBehavior::Play(%this, %fromBehavior, %fromOutput)
{
    %asset = strchr(%this.asset, "{");
    if (!AssetDatabase.isDeclaredAsset(%asset))
    {
        warn("AnimationEffectBehavior::Play -- undeclared asset" @ %asset);
        return;
    }
    
    %assetType = AssetDatabase.getAssetType(%asset);
    
    if (%assetType $= "ImageAsset")
    {
        %this.owner.setImageMap(%asset);
        %this.owner.setFrame(%this.frame);
    }
    else if (%assetType $= "AnimationAsset")
    {
        %this.owner.animation = %asset;
        //%this.owner.playAnimation(%this.asset);
    }
    else
    {
        warn("AnimationEffectBehavior::Play -- invalid asset: " @ %asset); 
        return;  
    } 
}

/// <summary>
/// Sets asset for the effect to set when Play is called.
/// </summary>
/// <param name="asset">The datablock of the animation or image</param>
function AnimationEffectBehavior::setAsset(%this, %asset)
{
    %this.asset = %asset;
}

/// <summary>
/// Returns the asset the effect behavior sets when Play is called.
/// </summary>
function AnimationEffectBehavior::getAsset(%this)
{
    return %this.asset;
}

/// <summary>
/// Sets the frame number of the asset.
/// </summary>
/// <param name="frame">The frame number</param>
function AnimationEffectBehavior::setFrame(%this, %frame)
{
    %this.frame = %frame;
}

/// <summary>
/// Returns the asset frame number.
/// </summary>
function AnimationEffectBehavior::getFrame(%this)
{
    return %this.frame;
}

/// <summary>
/// Returns the total number of frames for the behavior's asset.
/// Defaults to zero if the asset is not an "ImageAsset".
/// </summary>
function AnimationEffectBehavior::getFrameCount(%this)
{
    if (AssetDatabase.getAssetType(%this.asset) $= "ImageAsset")
    {
        %imageMapDatablock = AssetDatabase.acquireAsset(%this.asset);
        return %imageMapDatablock.getFrameCount(); 
    }
         
    return 0;
}