//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef MAC_CARB_EVENTS_H
#define MAC_CARB_EVENTS_H

/// Hides the mouse cursor when the mouse is locked, when we are in fullscreen
/// mode, and when the cursor is inside the window bounds.
/// Optionally takes the cursor location, in window local coords.
//void iOSCheckHideCursor(S32 x=-1, S32 y=-1);

/// Hides or shows the system mouse cursor, regardless of the mouse position,
/// and tracks the hidden state.
//void iOSSetHideCursor(bool shouldHide);

/// Install event loop timer, replacing any existing event loop timer.
/// This is useful for updating the sleep time
void iOSRunEventLoopTimer(S32 intervalMs);

/// Install event handlers on the current window & on the app.
void iOSInstallCarbonEventHandlers(void);

/// Remove all event handlers, except the alert event handlers.
void iOSRemoveCarbonEventHandlers(void);

/// Install event handlers for custom Torque inter-thread communication.
///  This must be called before starting multithreaded execution in main().
void iOSInstallTorqueCarbonEventHandlers(void);

/// Handle an HICommand that comes from a native MacOSX menu.
/// Returns false if the command can't be handled.
bool iOSHandleMenuCommand(void* hiCommand);

/// create an event of class kEventClassTorque, and send it to the main thread
void iOSSendTorqueEventToMain( U32 eventKind, void* userData = NULL );

/// event type for alerts. The event class is an arbitrary val, it must not collide w/ kEventApp* .
const U32 kEventClassTorque   = 'TORQ';
const U32 kEventTorqueAlert   = 1;
const U32 kEventTorqueFadeInWindow = 2;
const U32 kEventTorqueFadeOutWindow = 3;
const U32 kEventTorqueReleaseWindow = 4;
const U32 kEventTorqueShowMenuBar = 5;
const U32 kEventTorqueModalDialog = 6;
const U32 kEventTorqueDrawMenuBar = 7;

const U32 kEventParamTorqueData           = 'tDAT'; // typeVoidPtr void*
//const U32 kEventParamTorqueSemaphorePtr   = 'tSEM'; // typeVoidPtr void*
//const U32 kEventParamTorqueDialogRef      = 'tDRF'; // typeDialogRef DialogRef
//const U32 kEventParamTorqueHitPtr         = 'tHIT'; // typeVoidPtr U32*
//const U32 typeDialogRef                   = 'dlog'; // oddly, not provided in Frameworks.


// this command id is used for all dynamically created menus.
const U32 kHICommandTorque = 'TORQ';

#endif
