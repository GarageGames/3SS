//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "platformWin32/platformWin32.h"
#include "platform/platformVideo.h"
#include "platformWin32/winDirectInput.h"
#include "platformWin32/winDInputDevice.h"
#include "platform/event.h"
#include "console/console.h"
#include "console/consoleTypes.h"

//------------------------------------------------------------------------------
// Static class variables:
bool DInputManager::smKeyboardEnabled = true;
bool DInputManager::smMouseEnabled = false;
bool DInputManager::smJoystickEnabled = false;

// Type definitions:
typedef HRESULT (WINAPI* FN_DirectInputCreate)(HINSTANCE hinst, DWORD dwVersion, REFIID riidltf, LPVOID *ppvOut, LPUNKNOWN punkOuter);

//------------------------------------------------------------------------------
DInputManager::DInputManager()
{
   mEnabled          = false;
   mDInputLib        = NULL;
   mDInputInterface  = NULL;
   mKeyboardActive   = mMouseActive = mJoystickActive = false;
}

//------------------------------------------------------------------------------
void DInputManager::init()
{
   Con::addVariable( "pref::Input::KeyboardEnabled",  TypeBool, &smKeyboardEnabled );
   Con::addVariable( "pref::Input::MouseEnabled",     TypeBool, &smMouseEnabled );
   Con::addVariable( "pref::Input::JoystickEnabled",  TypeBool, &smJoystickEnabled );
}

//------------------------------------------------------------------------------
bool DInputManager::enable()
{
   FN_DirectInputCreate fnDInputCreate;

   disable();
#ifdef LOG_INPUT
   Input::log( "Enabling DirectInput...\n" );
#endif
   mDInputLib = LoadLibrary( dT("DInput8.dll") );
   if ( mDInputLib )
   {
	  fnDInputCreate = (FN_DirectInputCreate) GetProcAddress( mDInputLib, "DirectInput8Create" );
      if ( fnDInputCreate )
      {
		 bool result = SUCCEEDED( fnDInputCreate( winState.appInstance, DIRECTINPUT_VERSION,
			IID_IDirectInput8, reinterpret_cast<void**>(&mDInputInterface), NULL ));

         if ( result )
         {
            enumerateDevices();
            mEnabled = true;
            return true;
         }
      }
   }

   disable();

   return false;
}

//------------------------------------------------------------------------------
void DInputManager::disable()
{
   unacquire( SI_ANY, SI_ANY );

   DInputDevice* dptr;
   iterator ptr = begin();
   while ( ptr != end() )
   {
      dptr = dynamic_cast<DInputDevice*>( *ptr );
      if ( dptr )
      {
         removeObject( dptr );
         //if ( dptr->getManager() )
            //dptr->getManager()->unregisterObject( dptr );
         delete dptr;
         ptr = begin();
      }
      else
         ptr++;
   }

   if ( mDInputInterface )
   {
      mDInputInterface->Release();
      mDInputInterface = NULL;
   }

   if ( mDInputLib )
   {
      FreeLibrary( mDInputLib );
      mDInputLib = NULL;
   }

   mEnabled = false;
}

//------------------------------------------------------------------------------
void DInputManager::onDeleteNotify( SimObject* object )
{
   Parent::onDeleteNotify( object );
}

//------------------------------------------------------------------------------
bool DInputManager::onAdd()
{
   if ( !Parent::onAdd() )
      return false;

   acquire( SI_ANY, SI_ANY );
   return true;
}

//------------------------------------------------------------------------------
void DInputManager::onRemove()
{
   unacquire( SI_ANY, SI_ANY );
   Parent::onRemove();
}

//------------------------------------------------------------------------------
bool DInputManager::acquire( U8 deviceType, U8 deviceID )
{
   bool anyActive = false;
   DInputDevice* dptr;
   for ( iterator ptr = begin(); ptr != end(); ptr++ )
   {
      dptr = dynamic_cast<DInputDevice*>( *ptr );
      if ( dptr
        && ( ( deviceType == SI_ANY ) || ( dptr->getDeviceType() == deviceType ) )
        && ( ( deviceID == SI_ANY ) || ( dptr->getDeviceID() == deviceID ) ) )
      {
         if ( dptr->acquire() )
            anyActive = true;
      }
   }

   return anyActive;
}

//------------------------------------------------------------------------------
void DInputManager::unacquire( U8 deviceType, U8 deviceID )
{
   DInputDevice* dptr;
   for ( iterator ptr = begin(); ptr != end(); ptr++ )
   {
      dptr = dynamic_cast<DInputDevice*>( *ptr );
      if ( dptr
        && ( ( deviceType == SI_ANY ) || ( dptr->getDeviceType() == deviceType ) )
        && ( ( deviceID == SI_ANY ) || ( dptr->getDeviceID() == deviceID ) ) )
         dptr->unacquire();
   }
}

//------------------------------------------------------------------------------
void DInputManager::process()
{
   DInputDevice* dptr;
   for ( iterator ptr = begin(); ptr != end(); ptr++ )
   {
      dptr = dynamic_cast<DInputDevice*>( *ptr );
      if ( dptr )
         dptr->process();
   }
}

//------------------------------------------------------------------------------
void DInputManager::enumerateDevices()
{
   if ( mDInputInterface )
   {
      DInputDevice::init();
      DInputDevice::smDInputInterface = mDInputInterface;
      mDInputInterface->EnumDevices( DI8DEVTYPE_KEYBOARD, EnumDevicesProc, this, DIEDFL_ATTACHEDONLY );
      mDInputInterface->EnumDevices( DI8DEVTYPE_MOUSE,    EnumDevicesProc, this, DIEDFL_ATTACHEDONLY );
      mDInputInterface->EnumDevices( DI8DEVTYPE_JOYSTICK, EnumDevicesProc, this, DIEDFL_ATTACHEDONLY );
      mDInputInterface->EnumDevices( DI8DEVTYPE_GAMEPAD,  EnumDevicesProc, this, DIEDFL_ATTACHEDONLY );
   }
}

//------------------------------------------------------------------------------
BOOL CALLBACK DInputManager::EnumDevicesProc( const DIDEVICEINSTANCE* pddi, LPVOID pvRef )
{
   DInputManager* manager = (DInputManager*) pvRef;
   DInputDevice* newDevice = new DInputDevice( pddi );
   manager->addObject( newDevice );
   if ( !newDevice->create() )
   {
      manager->removeObject( newDevice );
      delete newDevice;
   }

   return (DIENUM_CONTINUE);
}

//------------------------------------------------------------------------------
bool DInputManager::enableKeyboard()
{
   DInputManager* mgr = dynamic_cast<DInputManager*>( Input::getManager() );
   if ( !mgr || !mgr->isEnabled() )
      return( false );

   if ( smKeyboardEnabled && mgr->isKeyboardActive() )
      return( true );

   smKeyboardEnabled = true;
   if ( Input::isActive() )
      smKeyboardEnabled = mgr->activateKeyboard();

   if ( smKeyboardEnabled )
   {
      Con::printf( "DirectInput keyboard enabled." );
   }
   else
   {
      Con::warnf( "DirectInput keyboard failed to enable!" );
   }

   return( smKeyboardEnabled );
}

//------------------------------------------------------------------------------
void DInputManager::disableKeyboard()
{
   DInputManager* mgr = dynamic_cast<DInputManager*>( Input::getManager() );
   if ( !mgr || !mgr->isEnabled() || !smKeyboardEnabled )
      return;

   mgr->deactivateKeyboard();
   smKeyboardEnabled = false;
   Con::printf( "DirectInput keyboard disabled." );
}

//------------------------------------------------------------------------------
bool DInputManager::isKeyboardEnabled()
{
   return( smKeyboardEnabled );
}

//------------------------------------------------------------------------------
bool DInputManager::activateKeyboard()
{
   if ( !mEnabled || !Input::isActive() || !smKeyboardEnabled )
      return( false );

   // Acquire only one keyboard:
   mKeyboardActive = acquire( KeyboardDeviceType, 0 );
   return( mKeyboardActive );
}

//------------------------------------------------------------------------------
void DInputManager::deactivateKeyboard()
{
   if ( mEnabled && mKeyboardActive )
   {
      unacquire( KeyboardDeviceType, SI_ANY );
      mKeyboardActive = false;
   }
}

//------------------------------------------------------------------------------
bool DInputManager::enableMouse()
{
   DInputManager* mgr = dynamic_cast<DInputManager*>( Input::getManager() );
   if ( !mgr || !mgr->isEnabled() )
      return( false );

   if ( smMouseEnabled && mgr->isMouseActive() )
      return( true );

   smMouseEnabled = true;
   if ( Input::isActive() )
      smMouseEnabled = mgr->activateMouse();

   if ( smMouseEnabled )
   {
      Con::printf( "DirectInput mouse enabled." );
   }
   else
   {
      Con::warnf( "DirectInput mouse failed to enable!" );
   }

   return( smMouseEnabled );
}

//------------------------------------------------------------------------------
void DInputManager::disableMouse()
{
   DInputManager* mgr = dynamic_cast<DInputManager*>( Input::getManager() );
   if ( !mgr || !mgr->isEnabled() || !smMouseEnabled )
      return;

   mgr->deactivateMouse();
   smMouseEnabled = false;
   Con::printf( "DirectInput mouse disabled." );
}

//------------------------------------------------------------------------------
bool DInputManager::isMouseEnabled()
{
   return( smMouseEnabled );
}

//------------------------------------------------------------------------------
bool DInputManager::activateMouse()
{
   if ( !mEnabled || !Input::isActive() || !smMouseEnabled )
      return( false );

   mMouseActive = acquire( MouseDeviceType, SI_ANY );
   return( mMouseActive );
}

//------------------------------------------------------------------------------
void DInputManager::deactivateMouse()
{
   if ( mEnabled && mMouseActive )
   {
      unacquire( MouseDeviceType, SI_ANY );
      mMouseActive = false;
   }
}

//------------------------------------------------------------------------------
bool DInputManager::enableJoystick()
{
   DInputManager* mgr = dynamic_cast<DInputManager*>( Input::getManager() );
   if ( !mgr || !mgr->isEnabled() )
      return( false );

   if ( smJoystickEnabled && mgr->isJoystickActive() )
      return( true );

   smJoystickEnabled = true;
   if ( Input::isActive() )
      smJoystickEnabled = mgr->activateJoystick();

   if ( smJoystickEnabled )
   {
      Con::printf( "DirectInput joystick enabled." );
   }
   else
   {
      Con::warnf( "DirectInput joystick failed to enable!" );
   }

   return( smJoystickEnabled );
}

//------------------------------------------------------------------------------
void DInputManager::disableJoystick()
{
   DInputManager* mgr = dynamic_cast<DInputManager*>( Input::getManager() );
   if ( !mgr || !mgr->isEnabled() || !smJoystickEnabled )
      return;

   mgr->deactivateJoystick();
   smJoystickEnabled = false;
   Con::printf( "DirectInput joystick disabled." );
}

//------------------------------------------------------------------------------
bool DInputManager::isJoystickEnabled()
{
   return( smJoystickEnabled );
}

//------------------------------------------------------------------------------
bool DInputManager::activateJoystick()
{
   if ( !mEnabled || !Input::isActive() || !smJoystickEnabled )
      return( false );

   mJoystickActive = acquire( JoystickDeviceType, SI_ANY );
   return( mJoystickActive );
}

//------------------------------------------------------------------------------
void DInputManager::deactivateJoystick()
{
   if ( mEnabled && mJoystickActive )
   {
      unacquire( JoystickDeviceType, SI_ANY );
      mJoystickActive = false;
   }
}

//------------------------------------------------------------------------------
const char* DInputManager::getJoystickAxesString( U32 deviceID )
{
   DInputDevice* dptr;
   for ( iterator ptr = begin(); ptr != end(); ptr++ )
   {
      dptr = dynamic_cast<DInputDevice*>( *ptr );
      if ( dptr && ( dptr->getDeviceType() == JoystickDeviceType ) && ( dptr->getDeviceID() == deviceID ) )
         return( dptr->getJoystickAxesString() );
   }

   return( "" );
}

//------------------------------------------------------------------------------
ConsoleFunction( activateKeyboard, bool, 1, 1, "() Use the activateKeyboard function to enable directInput polling of the keyboard.\n"
																"@return Returns a true if polling was successfully enabled.\n"
																"@sa deactivateKeyboard")
{
   argc; argv;
   DInputManager* mgr = dynamic_cast<DInputManager*>( Input::getManager() );
   if ( mgr )
      return( mgr->activateKeyboard() );

   return( false );
}

//------------------------------------------------------------------------------
ConsoleFunction( deactivateKeyboard, void, 1, 1, "() Use the deactivateKeyboard function to disable directInput polling of the keyboard.\n"
																"@return No return value.\n"
																"@sa activateKeyboard")
{
   argc; argv;
   DInputManager* mgr = dynamic_cast<DInputManager*>( Input::getManager() );
   if ( mgr )
      mgr->deactivateKeyboard();
}

//------------------------------------------------------------------------------
ConsoleFunction( enableMouse, bool, 1, 1, "() Use the enableMouse function to enable mouse input.\n"
																"@return Returns true if a mouse is present and it was enabled, false otherwise.\n"
																"@sa disableMouse")
{
   argc; argv;
   return( DInputManager::enableMouse() );
}

//------------------------------------------------------------------------------
ConsoleFunction( disableMouse, void, 1, 1, "() Use the disableMouse function to disable mouse input.\n"
																"@return No return value.\n"
																"@sa enableMouse")
{
   argc; argv;
   DInputManager::disableMouse();
}

//------------------------------------------------------------------------------
ConsoleFunction( enableJoystick, bool, 1, 1, "() Use the enableJoystick function to enable joystick input if it is present.\n"
																"@return Will return true if the joystick is present and was successfully enabled, false otherwise.\n"
																"@sa disableJoystick, getJoystickAxes, isJoystickDetected")
{
   argc; argv;
   return( DInputManager::enableJoystick() );
}

//------------------------------------------------------------------------------
ConsoleFunction( disableJoystick, void, 1, 1, "() Use the disableJoystick function to disable joystick input.\n"
																"@return No return value.\n"
																"@sa enableJoystick, getJoystickAxes, isJoystickEnabled")
{
   argc; argv;
   DInputManager::disableJoystick();
}

//------------------------------------------------------------------------------
ConsoleFunction( echoInputState, void, 1, 1, "() Use the echoInputState function to dump the input state of the mouse, keyboard, and joystick to the console.\n"
																"@return No return value.\n"
																"@sa activateDirectInput, deactivateDirectInput, activateKeyboard, deactivateKeyboard, disableJoystick, enableJoystick, enableMouse, disableMouse")
{
   argc; argv;
   DInputManager* mgr = dynamic_cast<DInputManager*>( Input::getManager() );
   if ( mgr && mgr->isEnabled() )
   {
      Con::printf( "DirectInput is enabled %s.", Input::isActive() ? "and active" : "but inactive" );
      Con::printf( "- Keyboard is %sabled and %sactive.",
            mgr->isKeyboardEnabled() ? "en" : "dis",
            mgr->isKeyboardActive() ? "" : "in" );
      Con::printf( "- Mouse is %sabled and %sactive.",
            mgr->isMouseEnabled() ? "en" : "dis",
            mgr->isMouseActive() ? "" : "in" );
      Con::printf( "- Joystick is %sabled and %sactive.",
            mgr->isJoystickEnabled() ? "en" : "dis",
            mgr->isJoystickActive() ? "" : "in" );
   }
   else
      Con::printf( "DirectInput is not enabled." );
}
