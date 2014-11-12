//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior leaves a trail of images behind a moving object.
/// </summary>
if (!isObject(ImageTrailBehavior))
{
    %template = new BehaviorTemplate(ImageTrailBehavior);

    %template.friendlyName = "Image Trail";
    %template.behaviorType = "Effect";
    %template.description  = "Create a trail of images behind a moving object.";

    // If we want to support multiple animated or static images for this effect this 
    // should be an animation set.
    %template.addBehaviorField(asset, "The effect's assigned asset", Default, "");
    %template.addBehaviorField(imageFrame, "The frame number for the imageMap if this is not an animation.", int, 0);
    %template.addBehaviorField(dropTimer, "The number of seconds between each image drop.", float, 0.1);
    %template.addBehaviorField(imageSize, "The size of the image in meters.", float, 0.5);
    
    %template.addBehaviorInput(Start, "Start Trail", "Tells us when a launch or start move event occurs.");
    %template.addBehaviorInput(End, "End Trail", "Tells us when a land or stop move event occurs so we can stop making images.");
}

function ImageTrailBehavior::onAddToScene(%this, %scene)
{
    %this.layer = %this.owner.getSceneLayer() + 1;
    %scene = sceneWindow2D.getScene();
    %scene.layerSortMode[%this.layer] = "X Axis";
}

/// <summary>
/// This input receives the StartMove signal to begin creating a trail
/// </summary>
/// <param name="fromBehavior">The behavior that raised the signal</param>
/// <param name="fromOutput">The output that originated the signal</param>
function ImageTrailBehavior::Start(%this, %fromBehavior, %fromOutput)
{
    // Store all of the trail markers in a SimSet so that we can clear them out
    if (!isObject($TrailSet))
    {
        $TrailSet = new SimSet();
    }
    %trailCount = $TrailSet.getCount();
    for(%i = 0; %i < %trailCount; %i++)
    {
        %obj = $TrailSet.getObject(0);
        if (isObject(%obj))
        {
            MainScene.removeFromScene(%obj);
            %obj.safeDelete();
        }
    }
    $TrailSet.clear();

    %this.makeTrail();
}

/// <summary>
/// This function is used by the projectile cache to disable and clean up the 
/// projectile before returning it to the cache.
/// </summary>
/// <param name="fromBehavior">The behavior that raised the signal</param>
/// <param name="fromOutput">The output that originated the signal</param>
function ImageTrailBehavior::End(%this, %fromBehavior, %fromOutput)
{
    ScheduleManager.cancelEvent(%this.nextTrailMarkerEvent);
}

/// <summary>
/// This function creates a new trail image and drops it
/// </summary>
function ImageTrailBehavior::makeTrail(%this)
{
    if (!isObject(%this.owner))
        return;

    %position = %this.owner.getPosition();
    %size = %this.imageSize SPC %this.imageSize;
    %temp = AssetDatabase.acquireAsset(%this.asset);
    %type = %temp.getClassName();
    AssetDatabase.releaseAsset(%this.asset);

    %obj = new Sprite()
    {
        position = %position;
        SceneLayer = %this.layer;
        size = %size;
        BodyType = "static";
        GravityScale = "0";
        CollisionSuppress = "1";
        Visible = "1";
    };

    MainScene.addToScene(%obj);
    $TrailSet.add(%obj);

    switch$(%type)
    {
        case "AnimationAsset":
            %obj.Animation = %this.asset;
        case "ImageAsset":
            %obj.ImageMap = %this.asset;
            %obj.Frame = %this.imageFrame;
    }

    %this.nextTrailMarkerEvent = ScheduleManager.scheduleEvent((%this.dropTimer * 1000), %this, "makeTrail", %this);
}

/// <summary>
/// This function sets the trail animation.
/// </summary>
/// <param name="anim">The animation to display as the trail image.</param>
function ImageTrailBehavior::setAnim(%this, %anim)
{
    %this.asset = %anim;
}

/// <summary>
/// This function sets the trail image frame.
/// </summary>
/// <param name="anim">The frame to display as the trail image.</param>
function ImageTrailBehavior::setFrame(%this, %frame)
{
    %this.imageFrame = %frame;
}

/// <summary>
/// This function gets the trail frame.
/// </summary>
function ImageTrailBehavior::getFrame(%this)
{
    return %this.imageFrame;
}