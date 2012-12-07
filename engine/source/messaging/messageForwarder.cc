//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/*
** Alive and Ticking
** (c) Copyright 2006 Burnt Wasp
**     All Rights Reserved.
**
** Filename:    messageForwarder.cc
** Author:      Tom Bampton
** Created:     26/8/2006
** Purpose:
**   Message Forwarder
**
*/

#include "messaging/messageForwarder.h"
#include "console/consoleTypes.h"

//////////////////////////////////////////////////////////////////////////
// Constructor/Destructor
//////////////////////////////////////////////////////////////////////////

MessageForwarder::MessageForwarder()
{
   mToQueue = "";
}

MessageForwarder::~MessageForwarder()
{
}

IMPLEMENT_CONOBJECT(MessageForwarder);

//////////////////////////////////////////////////////////////////////////

void MessageForwarder::initPersistFields()
{
   Parent::initPersistFields();

   addField("toQueue", TypeCaseString, Offset(mToQueue, MessageForwarder), "Queue to forward to");
}

//////////////////////////////////////////////////////////////////////////
// Public Methods
//////////////////////////////////////////////////////////////////////////

bool MessageForwarder::onMessageReceived(StringTableEntry queue, const char *event, const char *data)
{
   if(*mToQueue)
      Dispatcher::dispatchMessage(queue, event, data);
   return Parent::onMessageReceived(queue, event, data);
}

bool MessageForwarder::onMessageObjectReceived(StringTableEntry queue, Message *msg)
{
   if(*mToQueue)
      Dispatcher::dispatchMessageObject(mToQueue, msg);
   return Parent::onMessageObjectReceived(queue, msg);
}
