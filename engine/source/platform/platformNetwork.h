//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _PLATFORM_NETWORK_H_
#define _PLATFORM_NETWORK_H_

#ifndef _TORQUE_TYPES_H_
#include "platform/types.h"
#endif

//-----------------------------------------------------------------------------

struct NetAddress;

typedef S32 NetSocket;
const NetSocket InvalidSocket = -1;

//-----------------------------------------------------------------------------

struct Net
{
   enum Error
   {
      NoError,
      WrongProtocolType,
      InvalidPacketProtocol,
      WouldBlock,
      NotASocket,
      UnknownError
   };

   enum Protocol
   {
      UDPProtocol,
      IPXProtocol,
      TCPProtocol
   };

   static bool init();
   static void shutdown();
   static void process();

   // Unreliable network functions (UDP)
   static bool openPort(S32 connectPort);
   static void closePort();
   static Error sendto(const NetAddress *address, const U8 *buffer, S32 bufferSize);

   // Reliable network functions (TCP)
   static NetSocket openListenPort(U16 port);
   static NetSocket openConnectTo(const char *stringAddress); // does the DNS resolve etc.
   static void closeConnectTo(NetSocket socket);
   static Error sendtoSocket(NetSocket socket, const U8 *buffer, S32 bufferSize);

   static bool compareAddresses(const NetAddress *a1, const NetAddress *a2);
   static bool stringToAddress(const char *addressString, NetAddress *address);
   static void addressToString(const NetAddress *address, char addressString[256]);

   // Lower-level socket-based functions.
   static NetSocket openSocket();
   static Error closeSocket(NetSocket socket);
   static Error connect(NetSocket socket, const NetAddress *address);
   static Error listen(NetSocket socket, S32 maxConcurrentListens);
   static NetSocket accept(NetSocket acceptSocket, NetAddress *remoteAddress);
   static Error bind(NetSocket socket, U16    port);
   static Error setBufferSize(NetSocket socket, S32 bufferSize);
   static Error setBroadcast(NetSocket socket, bool broadcastEnable);
   static Error setBlocking(NetSocket socket, bool blockingIO);
   static Error send(NetSocket socket, const U8 *buffer, S32 bufferSize);
   static Error recv(NetSocket socket, U8 *buffer, S32 bufferSize, S32 *bytesRead);
};

#endif // _PLATFORM_NETWORK_H_
