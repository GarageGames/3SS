#pragma once

/* typedef a 32 bit type */
typedef unsigned long int MD5INT;

/* Data structure for MD5 (Message Digest) computation */
typedef struct {
	MD5INT i[2];                   /* number of _bits_ handled mod 2^64 */
	MD5INT buf[4];                                    /* scratch buffer */
	unsigned char in[64];                              /* input buffer */
	unsigned char digest[16];     /* actual digest after MD5Final call */
} MD5_CTX;

void MD5Init(MD5_CTX *mdContext);
void MD5Update(MD5_CTX* mdContext, unsigned char *inBuf, unsigned int inLen);
void MD5Final(MD5_CTX* mdContext);