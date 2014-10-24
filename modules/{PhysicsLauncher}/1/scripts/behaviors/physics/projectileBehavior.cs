//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// <summary>
/// This behavior simulates a physical object used as a projectile including gravity,
/// collisions and associated animations.
/// </summary>
if (!isObject(ProjectileBehavior))
{
    %template = new BehaviorTemplate(ProjectileBehavior);

    %template.friendlyName = "Projectile";
    %template.behaviorType = "Physics";
    %template.description  = "Set an object to be used as a physical projectile";

    // This is a list of possible collision shape types
    %template.colTypes = "circle" TAB "square" TAB "box" TAB "polygon";

    %template.addBehaviorInput(launchedInput, "Projectile Launched", "Lets the projectile know it has been launched.");

    %template.addBehaviorOutput(Ready, "Projectile Ready", "Tells others that the projectile is in the launcher.");
    %template.addBehaviorOutput(StartMove, "Projectile Moving", "Tells others that the projectile has started moving.");
    %template.addBehaviorOutput(EndMove, "Projectile First Hit", "Tells others that the projectile has stopped flying.");
    %template.addBehaviorOutput(Hit, "Projectile Hit", "Tells other that the projectile has hit something.");
}

/// <summary>
/// This function performs basic initialization tasks.
/// </summary>
function ProjectileBehavior::onAddToScene(%this)
{
    %this.ready = false;
    %this.setupProperties();
    %this.initialize();
}

/// <summary>
/// This function sets up the object to be used in the projectile cache.
/// </summary>
function ProjectileBehavior::setupProperties(%this)
{
}

/// <summary>
/// This function sets up the object for physics interaction by creating
/// its collision shape and setting up its physical properties.
/// </summary>
function ProjectileBehavior::initialize(%this)
{
    if (%this.owner.getCollisionShapeCount() < 1)
    {
        error(" !!! Projectile " @ %this @ " does not have a collision shape!");
        %this.createColShape();
    }

    %this.owner.setCollisionCallback(true);
    %this.owner.setBullet(true);
    %this.owner.setActive(true);
    %this.ready = true;
}

/// <summary>
/// This function creates a default collision shape in the event that the owning 
/// object does not have one defined.
/// </summary>
function ProjectileBehavior::createColShape(%this)
{
    %size = %this.owner.getSize();
    %x = getWord(%size, 0);
    %y = getWord(%size, 1);

    %this.colShape = %this.owner.createCircleCollisionShape((%x > %y ? %x : %y) / 2);
}

/// <summary>
/// This function handles the scene collision callback for this object.  This should be
/// called from the MainScene's onCollision callback.
/// </summary>
/// <param name="object">The ID of the other object in the collision.</param>
/// <param name="order">This tells us which collider we are, A or B.</param>
/// <param name="details">The collision details from the scene's onCollision() callback.</param>
function ProjectileBehavior::handleCollision(%this, %object, %order, %details)
{
    // Since the collision details might contain one or two contacts I'm only looking at
    // the first one - the second one might not exist.
    %shapeIndexA = getWord(%details, 0);
    %shapeIndexB = getWord(%details, 1);
    
    %isSensor = false;
    
    if (%order $= "A")
        %isSensor = %object.getCollisionShapeIsSensor(%shapeIndexB);
    else
        %isSensor = %object.getCollisionShapeIsSensor(%shapeIndexA);
    
    if (%isSensor)
        return;

    %x = getWord(%details, 4);
    %y = getWord(%details, 5);
    %position = %x SPC %y;
    
    // Store the contact point for use by other behaviors.
    %this.contactPoint = %position;

    if (%this.owner.activated)
    {
        sceneWindow2D.canMoveCamera = true;
        
        %this.owner.Raise(%this, Hit);
        
        if (%this.landed)
            return;

        %this.landed = true;
        %this.owner.setSceneLayer(10);
        %this.owner.Raise(%this, EndMove);
    }
}

function ProjectileBehavior::onLeaveWorldLimits(%this)
{
    //echo("@@@ ProjectileBehavior::onLeaveWorldLimits() " @ %this.owner);
    
    if (%this.owner.activated)
    {
        sceneWindow2D.canMoveCamera = true;

        %this.landed = true;
        %this.owner.setSceneLayer(10);
        %this.owner.Raise(%this, EndMove);
    }
}

/// <summary>
/// This function is used by the projectile cache to disable and clean up the 
/// projectile before returning it to the cache.
/// </summary>
function ProjectileBehavior::disableMoveProjectile(%this)
{
    %this.owner.setActive(false);
    %this.ready = false;
    %this.animScale = "";
}

/// <summary>
/// This function is used to prepare the projectile for use.
/// </summary>
function ProjectileBehavior::enableMoveProjectile(%this)
{
    %this.animScale = "";
    %this.owner.setActive(true);
    %this.ready = true;
}

function ProjectileBehavior::launchedInput(%this, %fromBehavior, %fromOutput)
{
    %this.owner.Raise(%this, StartMove);
}