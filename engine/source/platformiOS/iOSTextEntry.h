//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef IOSTEXTENTRY_INCLUDED
#define IOSTEXTENTRY_INCLUDED

#include "string/stringBuffer.h"

namespace iOSTextEntry
{
	// Returns false if the user cancels.
	// text [in/out]: The contents of the text entry field.  The text entry field is initialized
	// with the value of the StringBuffer.  On exit, if the function returns true, the StringBuffer
	// contains the text entered by the user; otherwise, the StringBuffer is unchanged.
	bool getUserText(StringBuffer& text);
	void onUserTextFinished(bool cancelled, const char* &result);
}


#endif