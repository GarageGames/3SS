//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

/// void(SimSet this, bool deleteObjects = false)
/// Deletes all the objects in the set.
/// @param this The SimSet
function SimSet::deleteContents(%this)
{
    // Iterate.
    while (%this.getCount() > 0)
        %this.getObject(0).delete();
}