//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/*
** Alive and Ticking
** (c) Copyright 2006 Burnt Wasp
**     All Rights Reserved.
**
** Filename:    platformVFS.h
** Author:      Tom Bampton
** Created:     28/10/2006
** Purpose:
**   Platform VFS
**
*/

#ifndef _PLATFORMVFS_H_
#define _PLATFORMVFS_H_

namespace Zip
{
   class ZipArchive;
}

extern Zip::ZipArchive *openEmbeddedVFSArchive();
extern void closeEmbeddedVFSArchive();

#endif // _PLATFORMVFS_H_
