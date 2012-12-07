//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//
//  iOSStoreKit.h
//  Torque2D for iOS
//
//  Created by Ian Eborn, based on existing files
//  Copyright 2010 Luma Arcade. All rights reserved.
//

#include "console.h"

struct product 
{
	U32 productId;
	char storeId[255];
	char description[255];
	char name[30];
	U32 chipValue;
	char itemFile[255];
	char itemThumbnail[255];
	U8 productType;
	U32 price;
	bool bDidGetRequestFromAppStore;
	bool bIsValidStoreId;
	char priceFormated[30];
};

struct shopProduct
{
	U8 productType;
	U32 productId;
	U32 qty;
	int iStep;
	product *pProd;
	char szTransactionId[255];
	U8 *pReceiptData;
};

void storeInit();
void storeCleanup();
bool storeMayMakePurchases();
void storeRequestProductDataFromAppStore(const char *pProductIdentifiers);
bool storePurchaseProduct(const char *pProductIdentifier, int iQuantity);
bool storeGetLocalizedPrice(double fPrice, char *pGetPrice, int iGetPriceSize);

//Note:
//
// - To receive purchase results, implement
//   shopReceivePurchaseResultFromAppStore(%success, %isRestoredTransaction, %userCancelledTransaction, %productID,
//                                         %receiptData, %receiptDataCount, %transactionID)
//   in a script
//
// - To receive product information results, implement
//   shopReceiveProductFromAppStore(%success, %pId, %pTitle, %pPriceString, %pPriceFormated, %pDesc)
//   in a script.
//
