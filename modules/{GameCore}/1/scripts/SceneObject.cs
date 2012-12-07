//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function SceneObject::onSafeDelete(%this)
{
}

function SceneObject::onLevelLoaded(%this, %scene)
{
    $ActiveObjectSet = new SimSet();
}

function SceneObject::onLevelEnded(%this, %scene)
{
}

/// <summary>
/// Sets the mass of the object to the specified value
/// by setting the density of all the collision shapes
/// on the object.
/// </summary>
/// <param name="mass">Float mass to set on the object.</param>
function SceneObject::setObjectMass(%this, %object, %mass)
{
    %totalArea = 0;
    
    // Sum up the areas for all collision shapes
    for (%i = 0; %i < %object.getCollisionShapeCount(); %i++)
    {
        %totalArea += %object.getCollisionShapeArea(%i);
    }
    
    // Calculate the density
    %density = %mass / %totalArea;

    // Set the density
    %object.setDefaultDensity(%density);    

    // Set the density on each collision shape
    for (%i = 0; %i < %object.getCollisionShapeCount(); %i++)
    {
        %object.setCollisionShapeDensity(%i, %density);
    }
}

/// <summary>
/// Gets the mass of the object.
/// </summary>
/// <return>The mass of the object.</return>
function SceneObject::getObjectMass(%this, %object)
{
    %totalArea = 0;
    
    // Sum up the areas for all collision shapes
    for (%i = 0; %i < %object.getCollisionShapeCount(); %i++)
    {
        %totalArea += %object.getCollisionShapeArea(%i);
    }
    
    // Calculate the density
    %density = %object.getDefaultDensity();
    %mass = %density * %totalArea;
    return %mass;
}