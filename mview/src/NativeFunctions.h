#pragma once

#include "mview.h"

#ifdef MV_WINDOWS
const char* OpenFileDialogNativeWindows(const char* filter, void* owner);
const char* SaveFileDialogNativeWindows(const char* filter, void* owner);
#endif

#ifdef MV_MACOSX
const char* OpenFileDialogNativeMacOS(const char* filter);
const char* SaveFileDialogNativeMacOS(const char* filter);
#endif
