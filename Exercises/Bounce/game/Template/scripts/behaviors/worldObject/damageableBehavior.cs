//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

// The maximum number of damage states an object with DamageableBehavior can have
$DamageableBehaviorMaxDamageStates = 5;

// Constant for converting impulse to damage
$DamagePerUnitImpulse = 10;

// Prefix for the dynamically created output signals
$DamageableBehaviorOutputCallString = "OnDamageState";
$DamageableBehaviorOutputNoSkipCallString = "OnDamageStateNoSkip";

if(!isObject(DamageableBehavior))
{
    %template = new BehaviorTemplate(DamageableBehavior);
    %template.friendlyName = "Responds To Damage";
    %template.behaviorType = "World Object";
    %template.description = "Allows the owner to respond to various levels of damage."; 
    
    // Fields
    %template.addBehaviorField(instanceName, "The behavior instance name.", string, "");
    %template.addBehaviorField(startingHealth, "Initial health amount of the owner", Float, 100.0);
    %template.addBehaviorField(indestructableFlag, "If true, the object ignores damage", bool, false);
    %template.addBehaviorField(minDamageThreshold, "The minumum damage that can be subtracted from health", Float, 1.0);
    %template.addBehaviorField(minSpeedThreshold, "If set, specifies the minumum collision speed necessary for damage to be dealt", Float, 0.0);
    %template.addBehaviorField(damageStates, "String specifying up to five damage states (total damage as a fraction of starting health)", Default, "0.1 0.3 0.6 0.9 1.0");
    %template.addBehaviorField(damageStatesActiveMask, "Space-separated list of bools for each damage state indicating if the damage state is active in game", Default, "1 1 1 1 1");   
    %template.addBehaviorField(damageModifierGroups, "Lets the damageable behavior respond to damage differently with different object groups", Default, "");    
    
    // Outputs
    %outputCallString = $DamageableBehaviorOutputCallString;
    %outputLabelString = "On Damage State";
    %outputDescriptionString = "Output when a certain damage state is reached";
    for (%i = 0; %i < $DamageableBehaviorMaxDamageStates; %i++)
    {
        %template.call(addBehaviorOutput, %outputCallString @ %i, %outputLabelString, %outputDescriptionString);
    }
    
    %outputCallString = $DamageableBehaviorOutputNoSkipCallString;
    %outputLabelString = "On Damage State No Skip";
    %outputDescriptionString = "Output when a certain damage state is reached or exceeded";
    for (%i = 0; %i < $DamageableBehaviorMaxDamageStates; %i++)
    {
        %template.call(addBehaviorOutput, %outputCallString  @ %i, %outputLabelString, %outputDescriptionString);
    }
}

/// <summary>
/// Called when behavior is added to an object.
/// </summary>
function DamageableBehavior::onBehaviorAdd(%this)
{
    %this.currentHealth = %this.startingHealth;
    
    %this.damageStateCount = getWordCount(%this.damageStates);
    
    // Check if the word count is within bounds
    if (%this.damageStateCount > $DamageableBehaviorMaxDamageStates)
    {
        %this.damageStateCount = $DamageableBehaviorMaxDamageStates;
        warn("DamageableBehavior::onBehaviorAdd - Too many damage states. Max allowed:" SPC $DamageableBehaviorMaxDamageStates);
    }
    
    // Put the damage states into an array
    for (%i = 0; %i < %this.damageStateCount; %i++)
    {
        %value = getWord(%this.damageStates, %i);
        
        // Check that the value is between 0.0 and 1.0
        if (!(%value >= 0.0 && %value <= 1.0))
        {
            error("DamageableBehavior::onBehaviorAdd - Invalid value for damage state.");  
            break; 
        }
        
        %this.damageStateArray[%i] = %value;
    }
}

/// <summary>
/// Called when the behavior is added to the scene
/// </summary>
/// <param name="scene">The scene object the behavior was added to.</param>
function DamageableBehavior::onAddToScene(%this, %scene)
{
    %this.scene = %scene;
}

function DamageableBehavior::getStartingHealth(%this)
{
    return %this.startingHealth;
}

function DamageableBehavior::setStartingHealth(%this, %value)
{
    %this.startingHealth = %value;
    
    //TODO update damage states
}

function DamageableBehavior::getIndestructableFlag(%this)
{
    return %this.indestructableFlag;
}

function DamageableBehavior::setIndestructableFlag(%this, %value)
{
    %this.indestructableFlag = %value;
}

function DamageableBehavior::getDamageModifierGroups(%this)
{
    return %this.damageModifierGroups;
}

function DamageableBehavior::setDamageModifierGroups(%this, %value)
{
    %this.damageModifierGroups = %value;
}

function DamageableBehavior::getDamageStateForIndex(%this, %index)
{
    if (%index > getWordCount(%this.damageStates) + 1 || %index < 0)
    {
        warn ("DamageableBehavior::getDamageStateForIndex -- invalid index");
        return 0;   
    }
    
    return getWord(%this.damageStates, %index);
}

function DamageableBehavior::setDamageStateForIndex(%this, %index, %fraction)
{
    if (%index > getWordCount(%this.damageStates) + 1 || %index < 0)
    {
        warn ("DamageableBehavior::setDamageStateForIndex -- invalid index");
        return 0;   
    }
    
    %this.damageStates = setWord(%this.damageStates, %index, %fraction);
}

/// <summary>
/// Getter for the damageStates string
/// </summary>
function DamageableBehavior::getDamageStates(%this)
{
    return %this.damageStates;
}

/// <summary>
/// Setter for the damageStates string.
/// </summary>
/// <param name="damageStatesString">Space-delimited string specifying the damage states</param>
function DamageableBehavior::setDamageStates(%this, %damageStatesString)
{
    %this.damageStates = %damageStatesString;
}

/// <summary>
/// Sets the isactive mask for all damage states
/// </summary>
function DamageableBehavior::setDamageStatesActiveMask(%this, %mask)
{    
    // Check that the word count is valid for the number of damage states
    if (getWordCount(%mask) != getWordCount(%this.damageStates)) 
    {
        warn("DamageableBehavior::setDamageStatesActiveMask -- invalid word count");
        return;
    }    
    
    %this.damageStatesActiveMask = %mask;
}

/// <summary>
/// 
/// </summary>
function DamageableBehavior::getDamageStateIsActive(%this, %stateIndex)
{
    if (%stateIndex >= getWordCount(%this.damageStatesActiveMask)) 
    {
        warn("DamageableBehavior::getDamageStateIsActive -- invalid state index: " @ %stateIndex @ "damageStatesActiveMask = " @ %this.damageStatesActiveMask);
        return 0;
    }
    
    return getWord(%this.damageStatesActiveMask, %stateIndex);
}

/// <summary>
/// 
/// </summary>
function DamageableBehavior::setDamageStateIsActive(%this, %stateIndex, %flag)
{
    if (%stateIndex >= getWordCount(%this.damageStatesActiveMask)) 
    {
        warn("DamageableBehavior::setDamageStateIsActive -- invalid state index: " @ %stateIndex);
        return;
    }
    
    %this.damageStatesActiveMask = setWord(%this.damageStatesActiveMask, %stateIndex, %flag);
}

/// <summary>
/// Applies damage to the owner if the incrimental damage exceeds the minDamageThreshold
/// and triggers damage state signals if the total damage exceeds the damage
/// state threshold.
/// </summary>
/// <param name="damageAmount">The total damage to try to apply to the owner</param>
function DamageableBehavior::takeDamage(%this, %damageAmount)
{
    if (%this.indestructableFlag == true)
        return;    
    
    if (%damageAmount < %this.minDamageThreshold)
        return;
    
    %previousHealth = %this.currentHealth;    
    
    // Apply the damage the the current health
    %this.currentHealth -= %damageAmount;
    if (%this.currentHealth < 0)
        %this.currentHealth = 0;
    
    // Calculate the current and previous damage ratios
    // (the damage that has been sustained as a fraction of the starting health)
    // 0 = no damage
    // 1.0 = full damage
    %currentDamageRatio = (%this.startingHealth - %this.currentHealth) / %this.startingHealth;
    %previousDamageRatio = (%this.startingHealth - %previousHealth) / %this.startingHealth;
    
    // Handle the special case where the previous damage ratio is zero
    if (%previousDamageRatio == 0)
        %previousDamageRatio = -1.0;   
    
    // Determine the highest damage state(s) we've reached
    %highestDamageState = 0.0;
    for (%i = 0; %i < %this.damageStateCount; %i++)
    {
        %tempDamageStateRatio = %this.damageStateArray[%i];
        
        // check if we have reached the current damage state
        if (%currentDamageRatio >= %tempDamageStateRatio)
        {
            // update highestDamageState
            if (%tempDamageStateRatio > %highestDamageState)
                %highestDamageState = %tempDamageStateRatio;
        }
    }
    
    // Loop through all the damage states again and raise
    // signals for those that have been reached since the 
    // previous state
    for (%i = 0; %i < %this.damageStateCount; %i++)
    { 
        // Skip inactive damage states
        if (getWord(%this.damageStatesActiveMask, %i) $= "0")
            continue;        
        
        %tempDamageStateRatio = %this.damageStateArray[%i];
               
        // Check if a signal should be raised for this damage state
        %raiseSignal = false;
        %raiseSignalNoSkip = false; 
        if (%tempDamageStateRatio > %previousDamageRatio)
        {      
            if (%highestDamageState == %tempDamageStateRatio)
                    %raiseSignal = true; 
            if (%highestDamageState >= %tempDamageStateRatio)
                    %raiseSignalNoSkip = true;                    
        }
        
        // Raise signal
        if (%raiseSignal == true)
            %this.owner.Raise(%this, $DamageableBehaviorOutputCallString @ %i);
        if (%raiseSignalNoSkip == true)
            %this.owner.Raise(%this, $DamageableBehaviorOutputNoSkipCallString @ %i);    
        
    }

    //echo("@@@ object = " SPC %this.owner SPC "; Damage amount =" SPC %damageAmount SPC "; current health = " SPC %this.currentHealth);
}

/// <summary>
/// Called when the behavior's owner collides with another object.
/// Handles taking damage based on the force of the collision.
/// collisionDetails contains the following information:
/// isSensorA, isSensorB, normal.x, normal.y, point1.x, point1.y, normalImpulse1, tangentImpulse1 [point2.x, point2.y, normalImpulse2, tangentImpulse2]
/// </summary>
/// <param name="sceneObject">SceneObject that the owner collided with.</param>
/// <param name="collisionMember">String set to "A" or "B" to reflect which object this is in the collision pair.</param>
/// <param name="collisionDetails">String containing the collision details.</param>
function DamageableBehavior::handleCollision(%this, %sceneObject, %collisionMember, %collisionDetails)
{  
    %shapeIndexA = getWord(%collisionDetails, 0);
    %shapeIndexB = getWord(%collisionDetails, 1);
    
    %isSensor = false;
    
    if (%collisionMember $= "A")
        %isSensor = %sceneObject.getCollisionShapeIsSensor(%shapeIndexB);
    else
        %isSensor = %sceneObject.getCollisionShapeIsSensor(%shapeIndexA);
    
    if (%isSensor)
        return;

    // Get normal
    %normalVector = getWords(%collisionDetails, 2, 3);

    // Get the first collision point        
    %collisionPoint = getWords(%collisionDetails, 4, 5);
    
    // If a second point exists, set collisionPoint to the average of the two
    if (getWordCount(%collisionDetails) == 12)
    {
        %collisionPoint2 = getWords(%collisionDetails, 8, 9);
        %collisionPoint = Vector2Scale(Vector2Add(%collisionPoint, %collisionPoint2), 0.5);   
    }
  
    // Calculate the velocities of each object in the normal direction
    %normalVelocity1 = Vector2Dot(%normalVector, %this.owner.getLinearVelocityFromWorldPoint(%collisionPoint));
    %normalVelocity2 = Vector2Dot(%normalVector, %sceneObject.getLinearVelocityFromWorldPoint(%collisionPoint));
    
    // Get the "speed of the impact"
    %impactSpeed = mAbs(%normalVelocity1 - %normalVelocity2);
    
    // Calculate damage
    %damage = getWord(%collisionDetails, 6);
    if (getWordCount(%collisionDetails) == 12)
    {
        %damage2 = getWord(%collisionDetails, 10);
        %damage = (%damage + %damage2)/2;
    }
    
    %damage = mAbs(%damage * $DamagePerUnitImpulse);
    
    // If the impact speed is too low, deal no damage
    if (%impactSpeed < %this.minSpeedThreshold)
        %damage = 0;
    
    
    // Respond to damage modifiers
    %objectDamageModifierGroups = %sceneObject.callOnBehaviors(getDamageModifierGroups);

    if (%objectDamageModifierGroups !$= "ERR_CALL_NOT_HANDLED")
    {
        if (t2dGetCommonElements(%objectDamageModifierGroups, %this.damageModifierGroups) !$= "")
        {
            %destroyOnDamage = %sceneObject.callOnBehaviors(getDestroyOnDamage);            
            
            // If the destroyOnDamage flag is set, destroy the object, otherwise
            // apply the damage multiplier
            if (%destroyOnDamage == 1)
            {
                // Set the damage to a lethal value
                %damage = mGetMax(%this.currentHealth, %this.minDamageThreshold);
            }
            else
            {
                // Call modifyDamage on the DamageModiferBehavior
                %newDamage = %sceneObject.callOnBehaviors(modifyDamage, %damage);
            
                if (%newDamage !$= "ERR_CALL_NOT_HANDLED")
                    %damage = %newDamage;
            }
        }
    }
    
    // Apply the damage
    %this.takeDamage(%damage);
}

