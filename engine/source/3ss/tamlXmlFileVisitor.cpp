//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#include "tamlXmlFileVisitor.h"
#include "tamlXmlFileVisitor_ScriptBinding.h"

//-----------------------------------------------------------------------------

IMPLEMENT_CONOBJECT( TamlXmlFileVisitor );

//-----------------------------------------------------------------------------
void TamlXmlFileVisitor::initPersistFields()
{
    Parent::initPersistFields();
}

bool TamlXmlFileVisitor::findElementByAttribValue( const char* pFileName, const char* pFieldName, const char* pFieldValue )
{
	mAttribName = StringTable->insert(pFieldName);
	mAttribValue = StringTable->insert(pFieldValue);
	parse( pFileName );
	return mFound;
}

ConsoleMethod(TamlXmlFileVisitor, parse, bool, 3, 3,   "(fileName) - parses the selected file.\n"
                                                    "@return true if successful, false if not.")
{
    // Fetch format mode.
    return object->parse( argv[1] );
}
