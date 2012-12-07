//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _CRC_H_
#define _CRC_H_

/// Initial value for CRCs
#define INITIAL_CRC_VALUE     0xffffffff
/// Value XORd with the CRC to post condition it (ones complement)
#define CRC_POSTCOND_VALUE    0xffffffff

class Stream;

U32 calculateCRC(const void * buffer, S32 len, U32 crcVal = INITIAL_CRC_VALUE);
U32 calculateCRCStream(Stream *stream, U32 crcVal = INITIAL_CRC_VALUE);

#endif

