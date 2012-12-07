//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------
#ifndef _PNGIMAGE_H_
#define _PNGIMAGE_H_

#ifndef _CONSOLEINTERNAL_H_
#include "console/consoleInternal.h"
#endif

#include <png.h>
#include "platform/types.h"
#include "console/console.h"
#include "console/consoleTypes.h"

enum PNGImageType { PNGTYPE_UNKNOWN = 0, PNGTYPE_RGB, PNGTYPE_RGBA };

class PNGImage : public SimObject
{
    typedef SimObject Parent;

protected:
    U32 mWidth, mHeight;

    png_byte mColorType;
    png_byte mBitDepth;

    png_structp mPng;
    png_infop mInfo;
    png_bytep* mRowPointers;

    PNGImageType mPNGImageType;

    char mReadFilePath[256];

    bool mRead;
    bool mWrite;

public:
    PNGImage();
    ~PNGImage();

    /// Called when the object is instantiated and registered 
    bool onAdd();

    /// Called when the object is destroyed and removed from script memory
    void onRemove();

    DECLARE_CONOBJECT(PNGImage);

    /// Construct the png information from the .png file path provided.
    bool Read(const char* filePath);

    bool Create(U32 width, U32 height, PNGImageType imageType);

    /// Write the .png file read from Read and save it to the file path provided.
    bool Write(const char* filePath);

    // Will "merge" the incoming image onto the current image on to an x, y position.
    bool MergeOn(U32 x, U32 y, const PNGImage* inc);

    // Will clean up any allocated memory from the PNGImage. This must be called or their may be a memory leak.
    bool CleanMemoryUsage();

    bool ClearImageData();

    U32 GetWidth() const { return mWidth; }
    U32 GetHeight() const { return mHeight; }
    const png_bytep* GetRowPointers() const { return mRowPointers; }
    const png_structp GetPng() const { return mPng; }
    const png_infop GetInfo() const { return mInfo; }
    const char* GetReadFilePath() { return mReadFilePath; }
    PNGImageType GetPNGImageType() const { return mPNGImageType; }
};

#endif