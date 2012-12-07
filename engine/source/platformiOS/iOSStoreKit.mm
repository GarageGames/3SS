//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

//
//  iOSStoreKit.mm
//  Torque2D for iOS
//
//  Created by Ian Eborn, based on existing files
//  Copyright 2010 Luma Arcade. All rights reserved.
//

#import "iOSStoreKit.h"

#import <StoreKit/StoreKit.h>

//Torque Stuff
#include "console/console.h"

#define kInAppPurchaseManagerProductsFetchedNotification @"kInAppPurchaseManagerProductsFetchedNotification"

@interface InAppPurchaseManager : NSObject <SKProductsRequestDelegate, SKPaymentTransactionObserver>
{
    
	//SKProduct *returnedProduct;
    SKProductsRequest *productsRequest;
	NSNumberFormatter *priceNumberFormatter;	// Number formatter used for converting prices to localized strings
}

// public methods
- (void)setStoreObserver;
- (BOOL)mayMakePurchases;
- (void)requestProductData:(NSString*)pProductIdentifier;
- (void)purchaseProduct:(NSString*)pProductIdentifier qty:(NSInteger)qty;
- (NSString*) encode:(const uint8_t*) input length:(NSInteger) length;
- (NSString *)getLocalizedPrice:(NSLocale *)priceLocale price:(NSDecimalNumber *) price;

@end


InAppPurchaseManager *gpStoreKitPurchaseManager = NULL;


@implementation InAppPurchaseManager

#pragma mark -
#pragma mark SKProductsRequestDelegate methods

- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response
{
    NSArray *products = response.products;
	
	if (products)
	{
		Con::printf("Products Size: %d", products.count);	// TEMP
		
		for (SKProduct *curProd in products)
		{
			// Return result to our shop
			const char *pTitle = [curProd.localizedTitle UTF8String];
			const char *pDesc = [curProd.localizedDescription UTF8String];
			const char *pId = [curProd.productIdentifier UTF8String];
            
			// float fPrice = [curProd.price doubleValue];
            
            NSNumberFormatter *formatter = [[NSNumberFormatter alloc] init];
            const char *pPriceString = [[formatter stringFromNumber:curProd.price] UTF8String];
            [formatter release];
            
			// U32 iPrice = (U32)(fPrice * 100.0f);
			
			// Your application can format the price using a number formatter, as shown in the following sample code:
			NSString *formattedString = [self getLocalizedPrice: curProd.priceLocale price:curProd.price];
			const char *pPriceFormated = [formattedString UTF8String];
			
			NSLog(@"Valid product id: %@    price: %@    formatPrice: %@", curProd.productIdentifier, curProd.price, formattedString);	// TEMP
			
            Con::executef(7, "shopReceiveProductFromAppStore", "1", pId, pTitle, pPriceString, pPriceFormated, pDesc);
		}
	}
	
	
	// Check invalid products
	for (NSString *invalidProductId in response.invalidProductIdentifiers)
    {
        NSLog(@"Invalid product id: %@" , invalidProductId);	// TEMP
		
		// Return result to our shop
		const char *pId = [invalidProductId UTF8String];
		
         Con::executef(3, "shopReceiveProductFromAppStore", "0", pId);
    }
	
    
    // finally release the reqest we alloc/init’ed in requestProductData
    [productsRequest release];
	productsRequest = NULL;
    
	// TODO NOTE: Is the following line needed?
    //[[NSNotificationCenter defaultCenter] postNotificationName:kInAppPurchaseManagerProductsFetchedNotification object:self userInfo:nil];
	
}


#pragma -
#pragma Public methods


// Call this method once on startup
- (void)setStoreObserver
{
    // restarts any purchases if they were interrupted last time the app was open
    [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
    
} 


// Call this before making a purchase. It tests if the user has enabled in-app purchases on the device.
- (BOOL)mayMakePurchases
{
    return [SKPaymentQueue canMakePayments];
}


// pProductIdentifiers - String of product identifiers, separated by a colon (:) character.
- (void)requestProductData:(NSString*)pProductIdentifiers
{
	//NSString *string = @"oop:ack:bork:greeble:ponies";
	NSArray *chunks = [pProductIdentifiers componentsSeparatedByString: @":"];
	
	NSSet *productIdentifiers = [NSSet setWithArray:chunks];
	//NSSet *productIdentifiers = [NSSet setWithObject:@"co.za.luma.idealtest2.safari" ];	// TEMP - Test single product
	
    productsRequest = [[SKProductsRequest alloc] initWithProductIdentifiers:productIdentifiers];
    productsRequest.delegate = self;
    [productsRequest start];
	
	//[chunks release];
    
    // we will release the request object in the delegate callback
}


// Start a purchase.
- (void)purchaseProduct:(NSString*)pProductIdentifier qty:(NSInteger)qty
{
    /*
	SKMutablePayment *payment = [SKMutablePayment paymentWithProductIdentifier:pProductIdentifier];
	if (payment)
	{
		payment.quantity = qty;
		[[SKPaymentQueue defaultQueue] addPayment:payment];
	}*/
}


#pragma -
#pragma Purchase helpers

// Saves a record of the transaction by storing the receipt to disk
- (void)recordTransaction:(SKPaymentTransaction *)transaction
{
	// TODO NOTE: Record chip purchase receipts, then remove it when the game server has validated the purchase.
	//			If the app starts up and there are still records of the chip purchases then the player did Not receive the chips.
	//			Then ask the player to connect to the game server and validate the receipt and update the chips.
	
	/*
     if ([transaction.payment.productIdentifier isEqualToString:kInAppPurchaseProUpgradeProductId])
     {
     // TODO NOTE: Save each product receipt with a specific key related to the product.
     // save the transaction receipt to disk
     //[[NSUserDefaults standardUserDefaults] setValue:transaction.transactionReceipt forKey:@"proUpgradeTransactionReceipt" ];
     //[[NSUserDefaults standardUserDefaults] synchronize];
     }
     */
}


// Provide the content related to the purchase.
- (void)provideContent:(NSString *)productId
{
	/*
     if ([productId isEqualToString:kInAppPurchaseProUpgradeProductId])
     {
     // TODO NOTE: Do we need to save it to defaults?
     // enable the pro features
     //[[NSUserDefaults standardUserDefaults] setBool:YES forKey:@"isProUpgradePurchased" ];
     //[[NSUserDefaults standardUserDefaults] synchronize];
     }
     */
}


// Removes the transaction from the queue and posts a notification with the transaction result
- (void)finishTransaction:(SKPaymentTransaction *)transaction wasSuccessful:(BOOL)wasSuccessful
{
    // remove the transaction from the payment queue.
    [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
    
    NSDictionary *userInfo = [NSDictionary dictionaryWithObjectsAndKeys:transaction, @"transaction" , nil];
    if (wasSuccessful)
    {
        // send out a notification that we’ve finished the transaction
		// TODO NOTE: Is the following line needed? Or is this how they update their UI?
        //[[NSNotificationCenter defaultCenter] postNotificationName:kInAppPurchaseManagerTransactionSucceededNotification object:self userInfo:userInfo];
    }
    else
    {
        // send out a notification for the failed transaction
		// TODO NOTE: Is the following line needed? Or is this how they update their UI?
        //[[NSNotificationCenter defaultCenter] postNotificationName:kInAppPurchaseManagerTransactionFailedNotification object:self userInfo:userInfo];
    }
}


// Called when the transaction was successful
- (void)completeTransaction:(SKPaymentTransaction *)transaction
{
    [self recordTransaction:transaction];
    [self provideContent:transaction.payment.productIdentifier];
	
	// Send results to our shop
	const char *pId = [transaction.payment.productIdentifier UTF8String];
	const char *pTransId = [transaction.transactionIdentifier UTF8String];
	
	NSString* jsonObjectString = [self encode: (uint8_t*)[[transaction transactionReceipt] bytes]  length: [[transaction transactionReceipt] length]];
	const char *pReceiptData = NULL;
	NSNumber* iReceiptSize = [NSNumber numberWithInt:0];
	if (jsonObjectString)
	{
		iReceiptSize = [NSNumber numberWithUnsignedInteger:jsonObjectString.length];
		pReceiptData = [jsonObjectString UTF8String];
	}
    
    NSNumberFormatter *formatter = [[NSNumberFormatter alloc] init];
    const char *pReceiptSize = [[formatter stringFromNumber:iReceiptSize] UTF8String];
    [formatter release];
	
    Con::executef(8, "shopReceivePurchaseResultFromAppStore", "1", "0", "0", pId, (const U8*)pReceiptData, pReceiptSize, pTransId);
    
    [self finishTransaction:transaction wasSuccessful:YES];
}


// Called when a transaction has been restored and successfully completed
- (void)restoreTransaction:(SKPaymentTransaction *)transaction
{
    [self recordTransaction:transaction.originalTransaction];
    [self provideContent:transaction.originalTransaction.payment.productIdentifier];
	
	// Send results to our shop
	const char *pId = [transaction.payment.productIdentifier UTF8String];
	const char *pTransId = [transaction.transactionIdentifier UTF8String];
	
	NSString* jsonObjectString = [self encode: (uint8_t*)[[transaction transactionReceipt] bytes]  length: [[transaction transactionReceipt] length]];
	const char *pReceiptData = NULL;
    
    NSNumber* iReceiptSize = [NSNumber numberWithInt:0];
    if (jsonObjectString)
    {
        iReceiptSize = [NSNumber numberWithUnsignedInteger:jsonObjectString.length];
        pReceiptData = [jsonObjectString UTF8String];
    }
    
    NSNumberFormatter *formatter = [[NSNumberFormatter alloc] init];
    const char *pReceiptSize = [[formatter stringFromNumber:iReceiptSize] UTF8String];
    [formatter release];
    
    Con::executef(8, "shopReceivePurchaseResultFromAppStore", "1", "1", "0", pId, (const U8*)pReceiptData, pReceiptSize, pTransId);
    
    [self finishTransaction:transaction wasSuccessful:YES];
}


// Called when a transaction has failed
- (void)failedTransaction:(SKPaymentTransaction *)transaction
{
	bool bCancelled = (transaction.error.code == SKErrorPaymentCancelled);
    
    Con::printf("%d", transaction.error.code);
    	
	// Send results to our shop
	const char *pId = [transaction.payment.productIdentifier UTF8String];
    
    const char* pCancelled = (bCancelled ? "1" : "0");
    
    Con::executef(5, "shopReceivePurchaseResultFromAppStore", "0", "0", pCancelled, pId);
    
    if (bCancelled == false)
    {
        // error!
        [self finishTransaction:transaction wasSuccessful:NO];
    }
    else
    {
        // this is fine, the user just cancelled, so don’t notify
        [[SKPaymentQueue defaultQueue] finishTransaction:transaction];
    }
}


#pragma mark -
#pragma mark SKPaymentTransactionObserver methods


// Called when the transaction status is updated
- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions
{
    for (SKPaymentTransaction *transaction in transactions)
    {
        switch (transaction.transactionState)
        {
            case SKPaymentTransactionStatePurchased:
                [self completeTransaction:transaction];
                break;
            case SKPaymentTransactionStateFailed:
                [self failedTransaction:transaction];
                break;
            case SKPaymentTransactionStateRestored:
                [self restoreTransaction:transaction];
                break;
            default:
                break;
        }
    }
}



// Encode the JSON string to Base64. Taken from: http://kroucis.wordpress.com/2009/07/27/in-app-purchase-receipt-validation/
// Example: NSString* jsonObjectString = [YourClass encode: (uint8_t*)[[transaction transactionReceipt] bytes]  length: [[transaction transactionReceipt] length];

- (NSString*) encode:(const uint8_t*) input length:(NSInteger) length {
    static char table[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    
    NSMutableData* data = [NSMutableData dataWithLength:((length + 2) / 3) * 4];
    uint8_t* output = (uint8_t*)data.mutableBytes;
	
    for (NSInteger i = 0; i < length; i += 3) {
        NSInteger value = 0;
        for (NSInteger j = i; j < (i + 3); j++) {
            value <<= 8;
			
            if (j < length) {
                value |= (0xFF & input[j]);
            }
        }
		
        NSInteger index = (i / 3) * 4;
        output[index + 0] =                    table[(value >> 18) & 0x3F];
        output[index + 1] =                    table[(value >> 12) & 0x3F];
        output[index + 2] = (i + 1) < length ? table[(value >> 6)  & 0x3F] : '=';
        output[index + 3] = (i + 2) < length ? table[(value >> 0)  & 0x3F] : '=';
    }
	
    return [[[NSString alloc] initWithData:data
                                  encoding:NSASCIIStringEncoding] autorelease];
}


// Get the localized string of a product's price.
- (NSString *)getLocalizedPrice:(NSLocale *)priceLocale price:(NSDecimalNumber *) price
{
	if ((priceNumberFormatter == NULL) && (priceLocale))
	{
		priceNumberFormatter = [[NSNumberFormatter alloc] init];
		[priceNumberFormatter setFormatterBehavior:NSNumberFormatterBehavior10_4];
		[priceNumberFormatter setNumberStyle:NSNumberFormatterCurrencyStyle];
		[priceNumberFormatter setLocale:priceLocale];
	}
	if (priceNumberFormatter == NULL)
	{
		return (NULL);
	}
    
    NSString *formattedString = [priceNumberFormatter stringFromNumber:price];
    return formattedString;
}


@end


void storeInit()
{
	// TODO NOTE: This may Not be the best place to create this, because we may Not need it throughout the life of 
	//			  the app, only when the store it needed.
	InAppPurchaseManager *purMan = [[InAppPurchaseManager alloc] init];		// Is this "init" correct???
	gpStoreKitPurchaseManager = purMan;
	if (gpStoreKitPurchaseManager)
	{
		[gpStoreKitPurchaseManager setStoreObserver];
	}
	
	Con::printf("\nLumaStoreKit::Init\n");	// TEMP
}


void storeCleanup()
{
	[gpStoreKitPurchaseManager release];
	gpStoreKitPurchaseManager = NULL;
}


// Test if the user enabled in-app purchases on the device.
bool storeMayMakePurchases()
{
	if (gpStoreKitPurchaseManager)
	{
		BOOL bResult = [gpStoreKitPurchaseManager mayMakePurchases];
		return (bResult);
	}
	return (false);
}

// Request the product(s) data from the App Store.
// Parameters:	*pProductIdentifiers - String of product identifiers, separated by a colon (:) character.
void storeRequestProductDataFromAppStore(const char *pProductIdentifiers)
{
	Con::printf("\nSending product data request to App Store:\n%s\n", pProductIdentifiers);	// TEMP
	
	if ((gpStoreKitPurchaseManager) && (pProductIdentifiers) && (pProductIdentifiers[0]))
	{
		NSString* pText = [NSString stringWithUTF8String:pProductIdentifiers];
		
		[gpStoreKitPurchaseManager requestProductData:pText];
        
        Con:printf("test 1");
	}
    
    Con::printf("test 2");
}

// Start a purchase.
bool storePurchaseProduct(const char *pProductIdentifier, int iQuantity)
{
	Con::printf("\nStart product purchase (qty:%d): %s", iQuantity, pProductIdentifier);	// TEMP
	
	if ((gpStoreKitPurchaseManager) && (pProductIdentifier) && (pProductIdentifier[0]))
	{
		NSString* pText = [NSString stringWithUTF8String:pProductIdentifier];
		NSInteger iQty = iQuantity;
		
		[gpStoreKitPurchaseManager purchaseProduct:pText qty:iQty];
		
		return (true);
	}
    
	return (false);
}

// Convert the price to a localized string.
bool storeGetLocalizedPrice(double fPrice, char *pGetPrice, int iGetPriceSize)
{
	if ((gpStoreKitPurchaseManager) && (pGetPrice) && (iGetPriceSize > 2))
	{
		NSDecimalNumber *usePrice = [NSDecimalNumber numberWithDouble:fPrice];
		NSString *formattedString = [gpStoreKitPurchaseManager getLocalizedPrice: NULL price:usePrice];
		if (formattedString == NULL)
		{
			return (false);
		}
		const char *pPriceFormated = [formattedString UTF8String];
		
		dStrncpy(pGetPrice, pPriceFormated, iGetPriceSize - 1);
		pGetPrice[iGetPriceSize - 1] = NULL;
		
		return (true);
	}
	return (false);
}

ConsoleFunction(storeInit, void, 1, 1, "storeInit() - Initialise the App Store interface.")
{
    storeInit();
}
ConsoleFunction(storeCleanup, void, 1, 1, "storeCleanup() - Clean up the App Store interface once its use is passed.")
{
    storeCleanup();
}
ConsoleFunction(storeMayMakePurchases, bool, 1, 1, "storeMayMakePurchases() - Test whether the user may make App Store purchases."
                                                    " - Returns: Whether or not the player may make App Store purchases.")
{
    return storeMayMakePurchases();
}
ConsoleFunction(storeRequestProductDataFromAppStore, void, 2, 2, "storeRequestProductDataFromAppStore(const char *pProductIdentifiers"
                                                                 " - Request product data from the App Store."
                                                                 " - Parameters:"
                                                                 " --- pProductIdentifiers - A string containing product IDs, separated by colons (\':\').")
{
    storeRequestProductDataFromAppStore(argv[1]);
}
ConsoleFunction(storePurchaseProduct, bool, 3, 3, "storePurchaseProduct(const char *pProductIdentifier, int iQuantity)"
                                                  " - Attempt to purchase a product from the App Store."
                                                  " - Parameters:"
                                                  " --- pProductIdentifier - A string containing the product ID of the desired product."
                                                  " --- iQuantity - The number of this product to buy."
                                                  " - Returns: Whether or not the purchase request succeeded.")
{
    return storePurchaseProduct(argv[1], dAtoi(argv[2]));
}
ConsoleFunction(storeGetLocalizedPrice, const char*, 3, 3, "storeGetLocalizedPrice(double fPrice, int iGetPriceSize)"
                                                    " - Get a localised version of the price."
                                                    " - Parameters:"
                                                    " --- fPrice - The price value to convert."
                                                    " --- iGetPriceSize - The maximum number of characters in the above string representation."
                                                    " - Returns: A string containing the resulting conversion, or NULL if iGetPriceSize is 0 or less.")
{
    int priceSize = dAtoi(argv[3]);
    if(priceSize > 0)
    {
        char* resultString = Con::getReturnBuffer(priceSize);
        resultString[0] = NULL;
        if(storeGetLocalizedPrice(dAtof(argv[1]), resultString, priceSize))
        {
            return resultString;
        }

        return NULL;
    }
    
    return NULL;
}


