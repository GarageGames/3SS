//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

// Called to setup the base expandos
function setupBaseExpandos()
{
   addPathExpando("templates", getMainDotCsDir() @ "/../templates" );
   addPathExpando("modules", getMainDotCsDir() @ "/modules" );
   addPathExpando("tool", getMainDotCsDir() );   
   addPathExpando("common", getMainDotCsDir() @ "/common" );
}
