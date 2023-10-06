#pragma once

#ifdef _WIN32
#define MV_EXPORT extern "C" __declspec(dllexport)
#define MV_IMPORT extern "C" __declspec(dllimport)
#else
#define MV_EXPORT
#define MV_IMPORT extern
#endif

#if defined(WIN32) || defined(_WIN32) || defined(__WIN32__) || defined(__NT__)
   #ifdef _WIN64
        #define MV_WINDOWS
   #endif
#elif __APPLE__
    #include <TargetConditionals.h>
    #if TARGET_IPHONE_SIMULATOR
    #elif TARGET_OS_MACCATALYST
    #elif TARGET_OS_IPHONE
    #elif TARGET_OS_MAC
        #define MV_MACOSX
    #else
        #error "Unknown Apple platform"
    #endif
#elif __ANDROID__
#elif __linux__
    #define MV_LINUX
#elif defined(_POSIX_VERSION)
#else
    #error "Unknown compiler"
#endif
