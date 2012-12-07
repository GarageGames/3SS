//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

///
/// Returns Projects API's EventManager Singleton
/// 

function Input::GetEventManager()
{
   if( !isObject( $_Tools::InputEventManager ) )
      $_Tools::InputEventManager = new EventManager() { queue = "InputEventManager"; };
      
   return $_Tools::InputEventManager;
}
