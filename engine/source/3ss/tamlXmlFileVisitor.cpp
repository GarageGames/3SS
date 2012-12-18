//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _CONSOLETYPES_H_
#include "console/consoleTypes.h"
#endif

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

void TamlXmlFileVisitor::clearFound(void)
{
	mFound = false;
}