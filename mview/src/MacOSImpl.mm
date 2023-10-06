#import <Foundation/Foundation.h>

#include "NativeFunctions.h"

const char* OpenFileDialogNativeMacOS(const char* filter)
{
	NSOpenPanel* dialog = [NSOpenPanel openPanel];
	NSString* filterStr = [NSString stringWithUTF8String:filter];
	NSArray* filters = [NSArray arrayWithObjects: filterStr];
	[dialog canChooseFiles: true];
	[dialog canChooseDirectories: false];
	[dialog allowsMultipleSelection: false];
	[dialog allowedFileTypes: filters];

	if ([dialog runModal] == NSOKButton)
	{
		NSArray* urls = [dialog URLs];

		NSString* url = [urls objectAtIndex: 0];
		const char* result = [url UTF8String];
		return result;
	}

	return "";
}

const char* SaveFileDialogNativeMacOS(const char* filter)
{
	NSSavePanel* dialog = [NSSavePanel savePanel];
	NSString* filterStr = [NSString stringWithUTF8String:filter];
	NSArray* filters = [NSArray arrayWithObjects: filterStr];
	[dialog canChooseFiles: true];
	[dialog canChooseDirectories: false];
	[dialog allowsMultipleSelection: false];
	[dialog allowedFileTypes: filters];

	if ([dialog runModal] == NSOKButton)
	{
		NSArray* urls = [dialog URLs];

		NSString* url = [urls objectAtIndex: 0];
		const char* result = [url UTF8String];
		return result;
	}

	return "";
}
