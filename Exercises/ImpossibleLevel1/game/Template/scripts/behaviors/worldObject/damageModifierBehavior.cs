//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

if(!isObject(DamageModifierBehavior))
{
    %template = new BehaviorTemplate(DamageModifierBehavior);
    %template.friendlyName = "Damage Modifier";
    %template.behaviorType = "World Object";
    %template.description = "Allows the owner to modify the damage it deals to damageable objects."; 

    // Fields
    %template.addBehaviorField(instanceName, "The behavior instance name.", string, "");
    %template.addBehaviorField(damageModifierGroups, "Lets the damageable behavior respond to damage differently with different object groups", Default, "");
    %template.addBehaviorField(damageMultiplier, "Value that the damage is multiplied by when modifying damage.", Float, 1.0);
    %template.addBehaviorField(destroyOnDamage, "If true, any amount of damage will destroy the object the owner collides with", Bool, false);
}

/// <summary>
/// Getter for damageModifierGroups
/// </summary>
function DamageModifierBehavior::getDamageModifierGroups(%this)
{
    return %this.damageModifierGroups;
}

/// <summary>
/// Getter for destroyOnDamage
/// </summary>
function DamageModifierBehavior::getDestroyOnDamage(%this)
{
    return %this.destroyOnDamage;
}

/// <summary>
/// Takes a damage value and returns a new damage value that
/// that is the product of the old damage and the damageMultiplier.
/// </summary>
/// <param name="damage">Float damage value</param>
/// <return>Float damage value after modification.</return>
function DamageModifierBehavior::modifyDamage(%this, %damage)
{
    return %this.damageMultiplier * %damage;   
}
