//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

//-----------------------------------------------------------------------------
// Adds pseudo-callbacks for SimSet add and remove.  This will not catch 
// anything except explicit removal via SimSet::removeObj().  On the up side, 
// using SimSet::removeObj() eliminates console spam if the object has already
// been removed.
//-----------------------------------------------------------------------------

function SimSet::addObj(%this, %object)
{
    if (!%this.isMember(%object))
    {
        %this.add(%object);
        %this.onAddObj(%object);
    }
}

function SimSet::onAddObj(%this, %object)
{
    //echo(" @@@ Added " @ %object @ ":" @ %object.getClassName() @ ":" @ %object.getInternalName() @ " to active set: " @ %this.getCount());
}

function SimSet::removeObj(%this, %object)
{
    if (%this.isMember(%object))
    {
        %this.remove(%object);
        %this.onRemoveObj(%object);
    }
}

function SimSet::onRemoveObj(%this, %object)
{
    //echo(" @@@ Removed " @ %object @ ":" @ %object.getClassName() @ ":" @ %object.getInternalName() @ " from active set: " @ %this.getCount());
}