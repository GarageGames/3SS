//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
function TagDropdownList::refresh(%this, %useAnyTag, %excludes)
{
    %this.clear();
    %tagManifest = AssetDatabase.getAssetTags();
    
    %tagCount = %tagManifest.getTagCount();

    for(%i = 0; %i < %tagCount; %i++)
    {
        %tag = %tagManifest.getTag(%i);

        // No tags excluded by default
        %isExcluded = -1;

        // If %excluded actually contained tags, search it for the current tag
        if(%excludes !$= "")
            %isExcluded = strstr(%excludes, %tag);

        // If there was a valid tag and it was not in the %excludes list
        if(%tag !$= "" && %isExcluded == -1)
            %this.add(%tag, %i);
    }

    if(%useAnyTag)
    {
        %this.add("Any", %i++);
        %this.setSelected(%i);
    }
    else
    {
        %this.setFirstSelected();
    }
}