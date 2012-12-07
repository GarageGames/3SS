//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function initializeBlackJack()
{
    EditorEventManager.postEvent("_TemplateLoaded", "");
}

function destroyBlackJack()
{
    
}

package BlackjackPackage
{
function BlackjackPackage::getFrameCount(%this)
{
    return getWordCount(%this.animationFrames);
}
};