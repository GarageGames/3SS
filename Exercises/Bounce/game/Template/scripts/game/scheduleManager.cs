//-----------------------------------------------------------------------------
// Three Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------
// manage a set of schedule objects

new ScriptObject(ScheduleManager);

// Engine-side schedule manager has global console functions for:
// getEventTimeLeft
// getScheduleDuration
// getTimeSinceStart
// isEventPending
// schedule
// cancel

/// <summary>
/// This function sets or unsets the pause state for its managed events.
/// </summary>
/// <param name="pause">Flag to set(true) or unset(false) the pause state of managed events.</param>
function ScheduleManager::setPause(%this, %pause)
{
   // First, paranoia check to ensure that we have a SimSet to copy our data to
   if (!isObject(%this.ScheduleSet) )
   {
      %this.ScheduleSet = new SimSet(ScheduleSet);
      //echo(" $$ScheduleManager::scheduleEvent() - creating PausedScheduleSet");
   }
   if (!isObject(%this.PausedScheduleSet) )
   {
      %this.PausedScheduleSet = new SimSet(PausedScheduleSet);
      //echo(" $$ScheduleManager::setPause() - creating PausedScheduleSet");
   }

   if (%pause == true)
   {
      // Don't double-pause
      if (%this.paused == true)
         return;
      
      // get time remaining on all schedules and store them
      %count = %this.ScheduleSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %temp = %this.ScheduleSet.getObject(%i);
         if (%temp.eventID !$= "")
         {
            // Check for remaining time on the scheduled event
            %temp.duration = getEventTimeLeft(%temp.eventID);
            //echo(" $$ ScheduleManager::setPause(true) - ("@%temp.duration@", "@%temp.objectID@", "@%temp.method@", "@%temp.targetID@", "@%temp.param1@", "@%temp.param2@", "@%temp.param3@", "@%temp.param4@","@%temp.eventID@","@%temp.handle@")");
            
            // Set the duration to -1 if this event has fired so we don't waste
            // time copying it.
            if (!isEventPending(%temp.eventID))
               %temp.duration = -1;
            else
            {
                %remainingTime = %temp.duration - getTimeSinceStart(%temp.eventID);
                %temp.duration = (%remainingTime > 32 ? %remainingTime : 32);
            }
            
            // Cancel all events in the list
            cancel(%temp.eventID);

            // Only copy events that are still pending to the PausedScheduleSet
            if(%temp.duration != -1)
               %this.PausedScheduleSet.add(%temp);
         }
      }
      // Clear out any remaining trash in the ScheduleSet
      %this.clearScheduleSet();
      // And now we're paused
      %this.paused = true;
   }
   else if (%pause == false)
   {
      // Don't re-un-pause
      if (%this.paused == false)
         return;

      // take stored schedules and restart them
      %count = %this.PausedScheduleSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %temp = %this.PausedScheduleSet.getObject(%i);
         // paranoia check to ensure that no invalid events got through
         if (%temp.duration > 0)
            %this.rescheduleEvent(%temp);
         
         //echo(" $$ ScheduleManager::setPause(false) - ("@%temp.duration@", "@%temp.objectID@", "@%temp.method@", "@%temp.targetID@", "@%temp.param1@", "@%temp.param2@", "@%temp.param3@", "@%temp.param4@","@%temp.eventID@","@%temp.handle@")");
      }
      // clear out any remaining trash in PausedScheduleSet
      %this.clearPausedScheduleSet();
      // And now we're resumed
      %this.paused = false;
   }
   else
      return;
}

/// <summary>
/// This function cancels all schedules associated with a particular object.  If desired, 
/// this will use the associated schedule "objectID" field instead when searching for
/// schedules to cancel.
/// </summary>
/// <param name="object">The object whose schedules we want to cancel.</param>
/// <param name="useObjectID">If true, use the object field as the objectID field from the schedule() call.</param>
function ScheduleManager::clearObjectSchedules(%this, %object, %useObjectID)
{
   // Find all schedules assigned to %object and cancel them.
   // If %useObjectID is true, %object is the schedule objectID field to search for
   // instead.
   %count = %this.ScheduleSet.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %temp = %this.ScheduleSet.getObject(%i);
      if (%useObjectID)
      {
         // This cancels events based on the "optional" objID.  For example, the way that I've been 
         // using this from WaveControllerBehavior, this would cancel all events associated
         // with a particular WaveControllerBehavior object.
         if (%temp.objectID == %object)
            cancel(%temp.eventID);
      }
      else
      {
         // This cancels events based on the "target object" of the schedule.  For example,
         // if there were multiple scheduled events pending for a particular game entity
         // it would cancel all of them.
         if (%temp.targetID == %object)
            cancel(%temp.eventID);
      }
   }
}

/// <summary>
/// This function schedules an event.
/// </summary>
/// <param name="time">The delay time for the event.</param>
/// <param name="object">The object to schedule the event for.  This becomes the objectID used in the clearObjectSchedules() method.</param>
/// <param name="method">The method to execute at the selected time.  This needs to be the fully qualified method name for member methods.</param>
/// <param name="targetID">The target object of the scheduled method.  Should be %this for calling member methods</param>
/// <param name="param1">Optional method parameter passed to the method to be called.</param>
/// <param name="param2">Optional method parameter passed to the method to be called.</param>
/// <param name="param3">Optional method parameter passed to the method to be called.</param>
/// <param name="param4">Optional method parameter passed to the method to be called.</param>
/// <return>Returns the event handle for the scheduled event.  This is not the scheduleID, it is used by the ScheduleManager to track and reschedule events.</return>
function ScheduleManager::scheduleEvent(%this, %time, %object, %method, %targetID, %param1, %param2, %param3, %param4)
{
    // NOTE!  The fourth parameter passed (%targetID) should be %this for calling member
    // methods.  The %method parameter should be the fully qualified method name -
    // i.e. "WaveControllerBehavior::startWave".
    // Example - ScheduleManager.scheduleEvent(%nextWave.startDelay * 1000, %this, "WaveControllerBehavior::startWave", %this);

    // First, ensure that we're all clean.
    if (!%this.initialized)
        %this.initialize();

    if (%time < 0)
        %time = 0;

    // Start the schedule, then store the schedule information and keep the ID
    if (%object !$= "")
    {
        if(%targetID $= "")
            %eventID = %object.schedule(%time, %method);
        else if (%param1 $= "")
            %eventID = %object.schedule(%time, %method, %targetID);
        else if (%param2 $= "")
            %eventID = %object.schedule(%time, %method, %targetID, %param1);
        else if (%param3 $= "")
            %eventID = %object.schedule(%time, %method, %targetID, %param1, %param2);
        else if (%param4 $= "")
            %eventID = %object.schedule(%time, %method, %targetID, %param1, %param2, %param3);
        else
            %eventID = %object.schedule(%time, %method, %targetID, %param1, %param2, %param3, %param4);
    }
    else
    {
        if(%targetID $= "")
            %eventID = schedule(%time, 0, %method);
        else if (%param1 $= "")
            %eventID = schedule(%time, 0, %method, %targetID);
        else if (%param2 $= "")
            %eventID = schedule(%time, 0, %method, %targetID, %param1);
        else if (%param3 $= "")
            %eventID = schedule(%time, 0, %method, %targetID, %param1, %param2);
        else if (%param4 $= "")
            %eventID = schedule(%time, 0, %method, %targetID, %param1, %param2, %param3);
        else
            %eventID = schedule(%time, 0, %method, %targetID, %param1, %param2, %param3, %param4);
    }

    if (%eventID == 0)
        echo(" $> InvalidEventId returned from schedule.  %object does not exist.");


    %eventData = new ScriptObject()
    {
        duration = %time;
        objectID = %object;
        method = %method;
        targetID = %targetID;
        param1 = %param1;
        param2 = %param2;
        param3 = %param3;
        param4 = %param4;      
        eventID = %eventID;
        handle = %eventID;
    };

    while (%this.handleExists(%eventData.handle))
        %eventData.handle++;

    // Add our new schedule data to our SimSet
    %this.ScheduleSet.add(%eventData);

    // Return the event handle
    return %eventData.handle;
}

/// <summary>
/// This function schedules an event using the %obj.schedule() form.
/// </summary>
/// <param name="time">The delay time for the event.</param>
/// <param name="object">The object to schedule the event for.</param>
/// <param name="method">The method to execute at the selected time.</param>
/// <param name="param1">Optional method parameter passed to the method to be called.</param>
/// <param name="param2">Optional method parameter passed to the method to be called.</param>
/// <param name="param3">Optional method parameter passed to the method to be called.</param>
/// <param name="param4">Optional method parameter passed to the method to be called.</param>
/// <return>Returns an event handle.</return>
function ScheduleManager::objectEvent(%this, %time, %object, %method, %param1, %param2, %param3, %param4)
{
   // version of sceduleEvent() that uses %obj.schedule()
   // %object should be the object upon which we wish to call the %method.
      
   // First, ensure that we're all clean.
   if (!%this.initialized)
      %this.initialize();
      
   if (%time < 0)
      %time = 0;
      
   // Start the schedule, then store the schedule information and keep the ID
   // Note - changed this to .call() because %object.schedule() was not working.
   // This is currently untested. (10 Feb 12)
   %eventID = %object.call(schedule, %time, %method, %param1, %param2, %param3, %param4);
   if (%eventID == 0)
      echo(" $> InvalidEventId returned from schedule.  %object does not exist.");
   
   %eventData = new ScriptObject()
   {
      duration = %time;
      objectID = %object;
      method = %method;
      targetID = %object;
      param1 = %param1;
      param2 = %param2;
      param3 = %param3;
      param4 = %param4;      
      eventID = %eventID;
      handle = %eventID;
   };
   
   while (%this.handleExists(%eventData.handle))
      %eventData.handle++;
   
   // Add our new schedule data to our SimSet
   %this.ScheduleSet.add(%eventData);
      
   //echo(" $$ ScheduleManager::objectEvent("@%time@", "@%object@", "@%method@", "@%param1@", "@%param2@", "@%param3@", "@%param4@", "@%eventID@")"@" : "@%eventData.handle);

   // Return the event handle
   return %eventData.handle;
}

/// <summary>
/// This function reschedules an event that has been paused.
/// </summary>
/// <param name="eventObject">A script object that contains event data for scheduling.</param>
/// <return>Returns an event handle.</return>
function ScheduleManager::rescheduleEvent(%this, %eventObject)
{
    // This takes an existing schedule object and restarts it.  We keep our handle so that 
    // we can cancel this event even after a pause (since the eventID is no longer the same
    // after restarting).
    %time = %eventObject.duration;
    %object = %eventObject.objectID;
    %method = %eventObject.method;
    %targetID = %eventObject.targetID;
    %param1 = %eventObject.param1;
    %param2 = %eventObject.param2;
    %param3 = %eventObject.param3;
    %param4 = %eventObject.param4;

    %handle = %this.scheduleEvent(%time, %object, %method, %targetID, %param1, %param2, %param3, %param4);
    // Return the event handle
    return %handle;
}

/// <summary>
/// This function checks to see if a specified event handle is in use.
/// </summary>
/// <param name="id">The handle to search for.</param>
/// <return>Returns true if the handle is in use, false if not.</param>
function ScheduleManager::handleExists(%this, %id)
{
   // Cancel the specified event by ID.
   %found = false;
   %count = %this.ScheduleSet.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %temp = %this.ScheduleSet.getObject(%i);
      if (%temp.handle == %id)
      {
         %found = true;
         break;
      }
   }
   if (%found == false)
   {
      %count = %this.PausedScheduleSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %temp = %this.PausedScheduleSet.getObject(%i);
         if (%temp.handle == %id)
         {
            %found = true;
            break;
         }
      }
   }
   // Return the search and cancel result.
   return %found;
}

/// <summary>
/// This function gets the remaining time until a scheduled event occurs.
/// </summary>
/// <param name="id">The event handle to search for.</param>
/// <return>Returns the time in milliseconds remaining until the event is scheduled to occur.</return>
function ScheduleManager::getTimeRemaining(%this, %id)
{
   // Get time remaining for the specified event by ID.
   %found = false;
   %targetSchedule = 0;
   %count = %this.ScheduleSet.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %temp = %this.ScheduleSet.getObject(%i);
      if (%temp.handle == %id)
      {
         %found = true;
         %targetSchedule = %temp.eventID;
         break;
      }
   }
   if (%found == false)
   {
      %count = %this.PausedScheduleSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %temp = %this.PausedScheduleSet.getObject(%i);
         // This should remove the event from the paused list if it is still pending.
         if (%temp.handle == %id)
         {
            %found = true;
            %targetSchedule = %temp.eventID;
            break;
         }
      }
   }
   // Return the time remaining on the event.
   if (%found)
      return getEventTimeLeft(%targetSchedule);
   else
      return 0;
}

/// <summary>
/// This function cancels an event.
/// </summary>
/// <param name="id">The handle of the event to cancel.</param>
/// <return>Returns true on success or false if the event could not be found.</return>
function ScheduleManager::cancelEvent(%this, %id)
{
   // Cancel the specified event by ID.
   %found = false;
   %count = %this.ScheduleSet.getCount();
   for (%i = 0; %i < %count; %i++)
   {
      %temp = %this.ScheduleSet.getObject(%i);
      if (%temp.handle == %id)
      {
         //echo(" $$ ScheduleManager::cancelEvent() - ("@%temp.duration@", "@%temp.objectID@", "@%temp.method@", "@%temp.targetID@", "@%temp.param2@", "@%temp.param3@", "@%temp.param4@","@%temp.eventID@")");
         cancel(%temp.eventID);
         %this.ScheduleSet.remove(%temp);
         // schedule found and cancelled
         %found = true;
         break;
      }
   }
   if (%found == false)
   {
      %count = %this.PausedScheduleSet.getCount();
      for (%i = 0; %i < %count; %i++)
      {
         %temp = %this.PausedScheduleSet.getObject(%i);
         // This should remove the event from the paused list if it is still pending.
         if (%temp.handle == %id)
         {
            //echo(" $$ ScheduleManager::cancelEvent() - ("@%temp.duration@", "@%temp.objectID@", "@%temp.method@", "@%temp.targetID@", "@%temp.param2@", "@%temp.param3@", "@%temp.param4@","@%temp.eventID@")");
            %this.ScheduleSet.remove(%temp);
            // schedule found and cancelled
            %found = true;
            break;
         }
      }
   }
   // Return the search and cancel result.
   return %found;
}

/// <summary>
/// This function clears all members of the current schedule set.  This does not
/// actually cancel the events.
/// </summary>
function ScheduleManager::clearScheduleSet(%this)
{
    if (!isObject(ScheduleSet) )
    {
        %this.ScheduleSet = new SimSet(ScheduleSet);
        //echo(" $$ScheduleManager::scheduleEvent() - creating ScheduleSet");
    }
    for (%i = 0; %i < %this.ScheduleSet.getCount(); %i++)
    {
        %schedule = %this.ScheduleSet.getObject(%i);
        cancel(%schedule.eventID);
    }
    // clear all members from the SimSet
    %this.ScheduleSet.clear();
}

/// <summary>
/// This function clears all members of the current paused schedule set.  This does not
/// actually cancel the events.
/// </summary>
function ScheduleManager::clearPausedScheduleSet(%this)
{
   if (!isObject(%this.PausedScheduleSet) )
   {
      %this.PausedScheduleSet = new SimSet(PausedScheduleSet);
      //echo(" $$ScheduleManager::scheduleEvent() - creating PausedScheduleSet");
   }
   // clear all members from the SimSet
   %this.PausedScheduleSet.clear();
}

/// <summary>
/// This function initializes the ScheduleManager for use.
/// </summary>
function ScheduleManager::initialize(%this)
{
   // This clears our SimSets and preps the ScheduleManager for use.
   // This method is called in scheduleEvent() if it has not already been
   // called, but if the ScheduleManager needs to be reused between levels
   // it should be called again from any menu item that loads another level, 
   // or called from within the Scene's onLevelLoaded() callback to ensure that
   // the ScheduleManager is ready for use.
   if (!isObject(ScheduleSet) )
   {
      %this.ScheduleSet = new SimSet(ScheduleSet);
      //echo(" $$ScheduleManager::scheduleEvent() - creating ScheduleSet");
   }
   if (!isObject(%this.PausedScheduleSet) )
   {
      %this.PausedScheduleSet = new SimSet(PausedScheduleSet);
      //echo(" $$ScheduleManager::scheduleEvent() - creating PausedScheduleSet");
   }
   %this.clearScheduleSet();
   %this.clearPausedScheduleSet();
   %this.paused = false;
   %this.initialized = true;
}