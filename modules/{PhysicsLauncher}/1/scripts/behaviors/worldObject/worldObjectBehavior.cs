//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

if(!isObject(WorldObjectBehavior))
{
    %template = new BehaviorTemplate(WorldObjectBehavior);
    %template.friendlyName = "World Object";
    %template.behaviorType = "World Object";
    %template.description = "Defines an object that can interact physically with other objects in the scene";
    
    %template.addBehaviorField(instanceName, "The behavior instance name.", string, "");    
    
    // Outputs
    %template.addBehaviorOutput(addedToSceneOutput, "Added to Scene Output", "Output Raised when the behavior is added to the scene.");
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function WorldObjectBehavior::onBehaviorAdd(%this)
{
    
}

/// <summary>
/// Called when the behavior is added to the scene
/// </summary>
/// <param name="scene">The scene object the behavior was added to.</param>
function WorldObjectBehavior::onAddToScene(%this, %scene)
{
    %this.scene = %scene;
    
    %this.owner.Raise(%this, addedToSceneOutput);
}

function WorldObjectBehavior::onWake(%this)
{
    if (isObject($ActiveObjectSet) && %this.owner.getBodyType() !$= "static")
        $ActiveObjectSet.addObj(%this.owner);
}

function WorldObjectBehavior::onSleep(%this)
{
    if (isObject($ActiveObjectSet))
        $ActiveObjectSet.removeObj(%this.owner);
}

/// <summary>
/// Sets the mass of the object to the specified value
/// by setting the density of all the collision shapes
/// on the object.
/// </summary>
/// <param name="mass">Float mass to set on the object.</param>
function WorldObjectBehavior::setObjectMass(%this, %mass)
{
    %totalArea = 0;
    
    // Sum up the areas for all collision shapes
    for (%i = 0; %i < %this.owner.getCollisionShapeCount(); %i++)
    {
        %totalArea += %this.owner.getCollisionShapeArea(%i);
    }
    
    // Calculate the density
    %density = %mass / %totalArea;

    // Set the density
    %this.setObjectDensity(%density);    
}

/// <summary>
/// Sets the default density on the object and the density on
/// all collision shapes on the object.
/// </summary>
/// <param name="density">Float density to set on the object.</param>
function WorldObjectBehavior::setObjectDensity(%this, %density)
{
    // Update default density
    %this.owner.setDefaultDensity(%density);    
    
    // Set the density on each collision shape
    for (%i = 0; %i < %this.owner.getCollisionShapeCount(); %i++)
    {
        %this.owner.setCollisionShapeDensity(%i, %density);
    }
}

/// <summary>
/// Gets the default density on the object
/// </summary>
function WorldObjectBehavior::getObjectDensity(%this)
{
    return %this.owner.getDefaultDensity();
}

/// <summary>
/// Sets the default friction on the object and the friction on
/// all collision shapes on the object.
/// </summary>
/// <param name="friction">Float friction to set on the object.</param>
function WorldObjectBehavior::setObjectFriction(%this, %friction)
{
    // Update default friction
    %this.owner.setDefaultFriction(%friction);    
    
    // Set the friction on each collision shape
    for (%i = 0; %i < %this.owner.getCollisionShapeCount(); %i++)
    {
        %this.owner.setCollisionShapeFriction(%i, %friction);
    }
}

/// <summary>
/// Gets the default friction on the object
/// </summary>
function WorldObjectBehavior::getObjectFriction(%this)
{
    return %this.owner.getDefaultFriction();
}

/// <summary>
/// Sets the default restitution on the object and the restitution on
/// all collision shapes on the object.
/// </summary>
/// <param name="restitution">Float restitution to set on the object.</param>
function WorldObjectBehavior::setObjectRestitution(%this, %restitution)
{
    // Update default restitution
    %this.owner.setDefaultRestitution(%restitution);    
    
    // Set the restitution on each collision shape
    for (%i = 0; %i < %this.owner.getCollisionShapeCount(); %i++)
    {
        %this.owner.setCollisionShapeRestitution(%i, %restitution);
    }
}

/// <summary>
/// Gets the default restitution on the object
/// </summary>
function WorldObjectBehavior::getObjectRestitution(%this)
{
    return %this.owner.getDefaultRestitution();
}