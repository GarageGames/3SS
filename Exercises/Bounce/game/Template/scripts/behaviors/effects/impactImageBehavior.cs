//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior creates an animation on impact.
/// </summary>
if (!isObject(ImpactImageBehavior))
{
    %template = new BehaviorTemplate(ImpactImageBehavior);

    %template.friendlyName = "Impact Image";
    %template.behaviorType = "Effect";
    %template.description  = "Create an image or animation when the object hits something.";

    // If we want to support multiple animated or static images for this effect this 
    // should be an animation set.
    %template.addBehaviorField(asset, "The effect's assigned asset", Default, "");
    //%template.addBehaviorField(impactAnim, "The object's trail object animation.", object, "PL_DefaultDustAnim", AnimationAsset);
    %template.addBehaviorField(imageSize, "The size of the image in meters.", float, 0.75);

    %template.addBehaviorInput(Impact, "Impact", "The object has hit something.");
}

/// <summary>
/// This function receives the Impact signal and creates an impact animation
/// sprite.
/// </summary>
/// <param name="fromBehavior">The behavior that raised the signal</param>
/// <param name="fromOutput">The output that originated the signal</param>
function ImpactImageBehavior::Impact(%this, %fromBehavior, %fromOutput)
{
    %this.makeImage(%fromBehavior.contactPoint);
}

/// <summary>
/// This function sets up a simset for tracking active impact image objects
/// and creates the image objects.
/// </summary>
/// <param name="point">The position at which to create the image</param>
function ImpactImageBehavior::makeImage(%this, %point)
{
    if (!isObject($ImpactSet))
    {
        $ImpactSet = new SimSet();
    }
    %anim = strchr(%this.asset, "{");
    %size = %this.imageSize SPC %this.imageSize;

    %obj = new Sprite()
    {
        animation = %anim;
        position = %point;
        SceneLayer = %this.owner.getSceneLayer() - 1;
        size = %size;
        CollisionGroups = "10";
        CollisionLayers = "10";
        CollisionSuppress = "1";
        DefaultDensity = "0.1";
        BodyType = "dynamic";
        Visible = "1";
    };
    $ImpactSet.add(%obj);
    MainScene.addToScene(%obj);
    
    %obj.playAnimation(%anim);
    %obj.setGravityScale(0);
    %torque = (getRandom() * 60) + 30;
    %coinToss = getRandom(1);
    if (%coinToss)
        %torque *= -1;
    %obj.setAngularVelocity(%torque);
    ScheduleManager.scheduleEvent((%anim.animationTime * 1000), GameEventManager, "postEvent", "_Cleanup", %obj);
}