//-----------------------------------------------------------------------------
// Torque
// Copyright GarageGames, LLC 2011
//-----------------------------------------------------------------------------

/*
 *  levelBuilderiPhoneUtil.cpp
 *  Torque2D for iPhone
 *
 *  Created by FuzzYspo0N
 *  Copyright 2009 Luma Arcade. All rights reserved.
 *
 *	Utilities for working with iPhone devices,
 *	HardwareGrowler provided code for detecting iPhone
 */

#include "levelBuilderiPhoneUtil.h"
#include "console/console.h"

#include <IOKit/IOKitLib.h>
#include <IOKit/IOCFPlugIn.h>
#include <IOKit/usb/IOUSBLib.h>
#include <IOKit/usb/USB.h>

IONotificationPortRef	ioKitNotificationPort;
CFRunLoopSourceRef		notificationRunLoopSource;

static void usbDeviceAdded(void *refCon, io_iterator_t iterator) 
{
	io_object_t	thisObject;
	while ((thisObject = IOIteratorNext(iterator))) {
		Boolean keyExistsAndHasValidFormat;
			kern_return_t	nameResult;
			io_name_t		deviceNameChars;
			
			//	This works with USB devices...
			//	but apparently not firewire
			nameResult = IORegistryEntryGetName(thisObject, deviceNameChars);
			
			CFStringRef deviceName = CFStringCreateWithCString(kCFAllocatorDefault,
															   deviceNameChars,
															   kCFStringEncodingASCII);
			if ((CFStringCompare(deviceName, CFSTR("OHCI Root Hub Simulation"), 0) == kCFCompareEqualTo) ||
				(CFStringCompare(deviceName, CFSTR("UHCI Root Hub Simulation"), 0) == kCFCompareEqualTo)) {
				CFRelease(deviceName);
				deviceName = CFCopyLocalizedString(CFSTR("USB Bus"), "");
				
			} else if (CFStringCompare(deviceName, CFSTR("EHCI Root Hub Simulation"), 0) == kCFCompareEqualTo) {
				CFRelease(deviceName);
				deviceName = CFCopyLocalizedString(CFSTR("USB 2.0 Bus"), "");
			}
			
			// NSLog(@"USB Device Attached: %@" , deviceName);
			//AppController_usbDidConnect(deviceName);
		
			//Lets test if we have a deviceName that we want,iPod
			if((CFStringCompare(deviceName, CFSTR("iPhone"), 0) == kCFCompareEqualTo))
			{
				Con::setVariable("$iPhone::device::name", "iPhone");
				Con::executef(1, "oniPhoneDeviceConnected");
			}
			else if(CFStringCompare(deviceName, CFSTR("iPod Touch"), 0) == kCFCompareEqualTo)
			{
				Con::setVariable("$iPhone::device::name", "iPod Touch");
				Con::executef(1, "oniPhoneDeviceConnected");
			}

		
			CFRelease(deviceName);
		
		IOObjectRelease(thisObject);
	}
}

static void usbDeviceRemoved(void *refCon, io_iterator_t iterator) {

	io_object_t thisObject;
	while ((thisObject = IOIteratorNext(iterator))) {
		kern_return_t	nameResult;
		io_name_t		deviceNameChars;
		
		//	This works with USB devices...
		//	but apparently not firewire
		nameResult = IORegistryEntryGetName(thisObject, deviceNameChars);
		CFStringRef deviceName = CFStringCreateWithCString(kCFAllocatorDefault,
														   deviceNameChars,
														   kCFStringEncodingASCII);
		if (CFStringCompare(deviceName, CFSTR("OHCI Root Hub Simulation"), 0) == kCFCompareEqualTo)
			deviceName = CFCopyLocalizedString(CFSTR("USB Bus"), "");
		else if (CFStringCompare(deviceName, CFSTR("EHCI Root Hub Simulation"), 0) == kCFCompareEqualTo)
			deviceName = CFCopyLocalizedString(CFSTR("USB 2.0 Bus"), "");
		
		//Lets test if we have a deviceName that we want,iPod
		if((CFStringCompare(deviceName, CFSTR("iPhone"), 0) == kCFCompareEqualTo))
		{
			Con::setVariable("$iPhone::device::name", "None Connected");
			Con::executef(1, "oniPhoneDeviceDisconnected");
		}
		else if(CFStringCompare(deviceName, CFSTR("iPod Touch"), 0) == kCFCompareEqualTo)
		{
			Con::setVariable("$iPhone::device::name", "None Connected");
			Con::executef(1, "oniPhoneDeviceDisconnected");
		}
		
		CFRelease(deviceName);

		IOObjectRelease(thisObject);
	}
}

void initiPhoneMonitors()
{
	ioKitNotificationPort = IONotificationPortCreate(kIOMasterPortDefault);
	notificationRunLoopSource = IONotificationPortGetRunLoopSource(ioKitNotificationPort);
	
	CFRunLoopAddSource(CFRunLoopGetCurrent(),
					   notificationRunLoopSource,
					   kCFRunLoopDefaultMode);
	
	//Register for the notifications
	kern_return_t	matchingResult;
	kern_return_t	removeNoteResult;
	io_iterator_t	addedIterator;
	io_iterator_t	removedIterator;
	
	//	Setup a matching Dictionary.
	CFDictionaryRef myMatchDictionary;
	myMatchDictionary = IOServiceMatching(kIOUSBDeviceClassName);
	
	//	Register our notification
	matchingResult = IOServiceAddMatchingNotification(ioKitNotificationPort,
													  kIOPublishNotification,
													  myMatchDictionary,
													  usbDeviceAdded,
													  NULL,
													  &addedIterator);
	
	usbDeviceAdded(NULL, addedIterator);
	
	//	Register for removal notifications.
	//	It seems we have to make a new dictionary...  reusing the old one didn't work.
	
	myMatchDictionary = IOServiceMatching(kIOUSBDeviceClassName);
	removeNoteResult = IOServiceAddMatchingNotification(ioKitNotificationPort,
														kIOTerminatedNotification,
														myMatchDictionary,
														usbDeviceRemoved,
														NULL,
														&removedIterator);
	
	// Matching notification must be "primed" by iterating over the
	// iterator returned from IOServiceAddMatchingNotification(), so
	// we call our device removed method here...
	//
	if (kIOReturnSuccess != removeNoteResult)
		//printf("");
	{
	}
	else
		usbDeviceRemoved(NULL, removedIterator);
	
	
	
}

void removeiPhoneMonitors()
{
	if (ioKitNotificationPort) 
	{
		CFRunLoopRemoveSource(CFRunLoopGetCurrent(), notificationRunLoopSource, kCFRunLoopDefaultMode);
		IONotificationPortDestroy(ioKitNotificationPort);
	}	
}

