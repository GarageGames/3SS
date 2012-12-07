//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

#ifndef _PLATFORM_ENDIAN_H_
#define _PLATFORM_ENDIAN_H_

#ifdef TORQUE_LITTLE_ENDIAN

inline U16 convertHostToLEndian(U16 i) { return i; }
inline U16 convertLEndianToHost(U16 i) { return i; }
inline U32 convertHostToLEndian(U32 i) { return i; }
inline U32 convertLEndianToHost(U32 i) { return i; }
inline S16 convertHostToLEndian(S16 i) { return i; }
inline S16 convertLEndianToHost(S16 i) { return i; }
inline S32 convertHostToLEndian(S32 i) { return i; }
inline S32 convertLEndianToHost(S32 i) { return i; }

inline F32 convertHostToLEndian(F32 i) { return i; }
inline F32 convertLEndianToHost(F32 i) { return i; }

inline F64 convertHostToLEndian(F64 i) { return i; }
inline F64 convertLEndianToHost(F64 i) { return i; }

inline U16 convertHostToBEndian(U16 i)
{
   return U16((i << 8) | (i >> 8));
}

inline U16 convertBEndianToHost(U16 i)
{
   return U16((i << 8) | (i >> 8));
}

inline S16 convertHostToBEndian(S16 i)
{
   return S16(convertHostToBEndian(U16(i)));
}

inline S16 convertBEndianToHost(S16 i)
{
   return S16(convertBEndianToHost(S16(i)));
}

inline U32 convertHostToBEndian(U32 i)
{
   return ((i << 24) & 0xff000000) |
          ((i <<  8) & 0x00ff0000) |
          ((i >>  8) & 0x0000ff00) |
          ((i >> 24) & 0x000000ff);
}

inline U32 convertBEndianToHost(U32 i)
{
   return ((i << 24) & 0xff000000) |
          ((i <<  8) & 0x00ff0000) |
          ((i >>  8) & 0x0000ff00) |
          ((i >> 24) & 0x000000ff);
}

inline S32 convertHostToBEndian(S32 i)
{
   return S32(convertHostToBEndian(U32(i)));
}

inline S32 convertBEndianToHost(S32 i)
{
   return S32(convertBEndianToHost(U32(i)));
}

inline U64 convertBEndianToHost(U64 i)
{
   U32 *inp = (U32 *) &i;
   U64 ret;
   U32 *outp = (U32 *) &ret;
   outp[0] = convertBEndianToHost(inp[1]);
   outp[1] = convertBEndianToHost(inp[0]);
   return ret;
}

inline U64 convertHostToBEndian(U64 i)
{
   U32 *inp = (U32 *) &i;
   U64 ret;
   U32 *outp = (U32 *) &ret;
   outp[0] = convertHostToBEndian(inp[1]);
   outp[1] = convertHostToBEndian(inp[0]);
   return ret;
}

inline F64 convertBEndianToHost(F64 in_swap)
{
   U64 result = convertBEndianToHost(* ((U64 *) &in_swap) );
   return * ((F64 *) &result);
}

inline F64 convertHostToBEndian(F64 in_swap)
{
   U64 result = convertHostToBEndian(* ((U64 *) &in_swap) );
   return * ((F64 *) &result);
}

#elif defined(TORQUE_BIG_ENDIAN)

inline U16 convertHostToBEndian(U16 i) { return i; }
inline U16 convertBEndianToHost(U16 i) { return i; }
inline U32 convertHostToBEndian(U32 i) { return i; }
inline U32 convertBEndianToHost(U32 i) { return i; }
inline S16 convertHostToBEndian(S16 i) { return i; }
inline S16 convertBEndianToHost(S16 i) { return i; }
inline S32 convertHostToBEndian(S32 i) { return i; }
inline S32 convertBEndianToHost(S32 i) { return i; }

inline U16 convertHostToLEndian(U16 i)
{
   return (i << 8) | (i >> 8);
}
inline U16 convertLEndianToHost(U16 i)
{
   return (i << 8) | (i >> 8);
}
inline U32 convertHostToLEndian(U32 i)
{
   return ((i << 24) & 0xff000000) |
          ((i <<  8) & 0x00ff0000) |
          ((i >>  8) & 0x0000ff00) |
          ((i >> 24) & 0x000000ff);
}
inline U32 convertLEndianToHost(U32 i)
{
   return ((i << 24) & 0xff000000) |
          ((i <<  8) & 0x00ff0000) |
          ((i >>  8) & 0x0000ff00) |
          ((i >> 24) & 0x000000ff);
}

inline U64 convertLEndianToHost(U64 i)
{
   U32 *inp = (U32 *) &i;
   U64 ret;
   U32 *outp = (U32 *) &ret;
   outp[0] = convertLEndianToHost(inp[1]);
   outp[1] = convertLEndianToHost(inp[0]);
   return ret;
}

inline U64 convertHostToLEndian(U64 i)
{
   U32 *inp = (U32 *) &i;
   U64 ret;
   U32 *outp = (U32 *) &ret;
   outp[0] = convertHostToLEndian(inp[1]);
   outp[1] = convertHostToLEndian(inp[0]);
   return ret;
}

inline F64 convertLEndianToHost(F64 in_swap)
{
   U64 result = convertLEndianToHost(* ((U64 *) &in_swap) );
   return * ((F64 *) &result);
}

inline F64 convertHostToLEndian(F64 in_swap)
{
   U64 result = convertHostToLEndian(* ((U64 *) &in_swap) );
   return * ((F64 *) &result);
}

inline F32 convertHostToLEndian(F32 i)
{
   U32 result = convertHostToLEndian( *reinterpret_cast<U32*>(&i) );
   return *reinterpret_cast<F32*>(&result);
}

inline F32 convertLEndianToHost(F32 i)
{
   U32 result = convertLEndianToHost( *reinterpret_cast<U32*>(&i) );
   return *reinterpret_cast<F32*>(&result);
}

inline S16 convertHostToLEndian(S16 i) { return S16(convertHostToLEndian(U16(i))); }
inline S16 convertLEndianToHost(S16 i) { return S16(convertLEndianToHost(U16(i))); }
inline S32 convertHostToLEndian(S32 i) { return S32(convertHostToLEndian(U32(i))); }
inline S32 convertLEndianToHost(S32 i) { return S32(convertLEndianToHost(U32(i))); }

#else
#error "Endian define not set"
#endif

#endif // _PLATFORM_ENDIAN_H_
