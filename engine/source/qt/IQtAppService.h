//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _IQTAPPSERVICE_H
#define _IQTAPPSERVICE_H

#include <QApplication>

class IQtAppService
{
public:
    virtual QApplication* getQtApp() = 0;
};

#endif // _IQTAPPSERVICE_H
