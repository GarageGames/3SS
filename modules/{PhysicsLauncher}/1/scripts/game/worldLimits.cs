//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

function WorldTrigger::handleCollision(%this, %obj, %order, %details)
{
    //echo(" @@@ " @ %obj @ ": " @ %obj.getClassName() @ " collided with WorldBoundary");
}

function WorldTrigger::onEnter(%this, %obj)
{
    //echo(" @@@ " @ %obj.getClassName() @ " entered WorldBoundary");
}

function WorldTrigger::onLeave(%this, %obj)
{
    //echo(" @@@ " @ %obj.getClassName() @ " left WorldBoundary");
    if ($ActiveObjectSet.isMember(%obj))
        $ActiveObjectSet.remove(%obj);
    ScheduleManager.scheduleEvent(250, GameEventManager, "postEvent", "_Cleanup", %obj);
}

function WorldTrigger::onStay(%this, %obj)
{
    //echo(" @@@ " @ %obj.getClassName() @ " is inside of WorldBoundary");
}