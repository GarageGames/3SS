//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------


#include "platformiOS/iOSFont.h"
#include "platformiOS/platformiOS.h"
#include "graphics/gBitmap.h"
#include "Math/mRect.h"
#include "math/mMathFn.h"
#include "console/console.h"
#include "string/Unicode.h"
#include "memory/frameAllocator.h"





//------------------------------------------------------------------------------
// New Unicode capable font class.
PlatformFont *createPlatformFont(const char *name, U32 size, U32 charset /* = TGE_ANSI_CHARSET */)
{
    PlatformFont *retFont = new iOSFont;

    if(retFont->create(name, size, charset))
        return retFont;

    delete retFont;
    return NULL;
}

//------------------------------------------------------------------------------
iOSFont::iOSFont()
{
   //mStyle   = NULL;
   //mLayout  = NULL;
   mColorSpace = NULL;
}

iOSFont::~iOSFont()
{
   // apple docs say we should dispose the layout first.
   //ATSUDisposeTextLayout(mLayout);
   //ATSUDisposeStyle(mStyle);
   CGColorSpaceRelease(mColorSpace);
}


//------------------------------------------------------------------------------
bool iOSFont::create( const char *name, U32 size, U32 charset)
{
	//make a new UIFont
	UIFont *font = NULL;
	CGFloat num = (CGFloat) size;

	NSString *string = [[NSString alloc] initWithUTF8String:name];
	
	font = [UIFont fontWithName: string size: num];
	
	mColorSpace = CGColorSpaceCreateDeviceGray();

	mUIFont = font;
	
	CGFloat width = size * strlen( name );
	CGFloat height = size;
	mBaseline = width;
	mHeight = height;
	
	// adjust the size. win dpi = 96, mac dpi = 72. 72/96 = .75
	// Interestingly enough, 0.75 is not what makes things the right size.
	//U32 scaledSize = size - 2 - (int)((float)size * 0.1);
    
    // Use Windows as a baseline (96 DPI) and adjust accordingly.
    F32 scaledSize = size * (72.0f/96.0f);
    mSize = (U32)mRound(scaledSize); 
	
	
	// and finally cache the font's name. We use this to cheat some antialiasing options below.
	mName = StringTable->insert(name);
	
	//end of font creation
	[string release];	
	return true;
}

//------------------------------------------------------------------------------
bool iOSFont::isValidChar(const UTF8 *str) const
{
   // since only low order characters are invalid, and since those characters
   // are single codeunits in UTF8, we can safely cast here.
   return isValidChar((UTF16)*str);  
}

bool iOSFont::isValidChar( const UTF16 ch) const
{
   // We cut out the ASCII control chars here. Only printable characters are valid.
   // 0x20 == 32 == space
   if( ch < 0x20 )
      return false;

   return true;
}

PlatformFont::CharInfo& iOSFont::getCharInfo(const UTF8 *str) const
{
   return getCharInfo(oneUTF32toUTF16(oneUTF8toUTF32(str,NULL)));
}

PlatformFont::CharInfo& iOSFont::getCharInfo(const UTF16 ch) const
{
	Con::warnf("iOSFont::getCharInfo() is being used to get char info, values will almost certainly be wrong(generate better UFTs)");	
	// Declare and clear out the CharInfo that will be returned.
	static PlatformFont::CharInfo c;
	dMemset(&c, 0, sizeof(c));
	
	// prep values for GFont::addBitmap()
	c.bitmapIndex = 0;
	c.xOffset = 0;
	c.yOffset = 0;

	return c;
}

void PlatformFont::enumeratePlatformFonts( Vector<StringTableEntry>& fonts, UTF16* fontFamily )
{}

