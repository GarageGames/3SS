//-----------------------------------------------------------------------------
// 3 Step Studio
// Copyright GarageGames, LLC 2012
//-----------------------------------------------------------------------------

#include "xcodeTemplate/ElementReference.h"

void ElementReference::Write(ofstream& file)
{
    char prevFill = file.fill('0');
    file.width(8);
    file << right << hex << uppercase << m_Reference[0];
    file.width(8);
    file << right << hex << uppercase << m_Reference[1];
    file.width(8);
    file << right << hex << uppercase << m_Reference[2];
    file << nouppercase;
    file.fill(prevFill);
}
