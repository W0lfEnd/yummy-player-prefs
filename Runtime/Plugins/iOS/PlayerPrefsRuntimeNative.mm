#import <Foundation/Foundation.h>
#include <stdlib.h>
#include <string.h>

extern "C" char* GetPlayerPrefsJSON()
{
    @autoreleasepool
    {
        NSDictionary* defaults = [[NSUserDefaults standardUserDefaults] dictionaryRepresentation];
        if (defaults == nil)
        {
            return nullptr;
        }

        NSError* error = nil;
        NSData* data = [NSJSONSerialization dataWithJSONObject:defaults options:0 error:&error];
        if (data == nil || error != nil)
        {
            return nullptr;
        }

        const char* json = static_cast<const char*>([data bytes]);
        NSUInteger length = [data length];
        char* buffer = static_cast<char*>(malloc(length + 1));
        if (buffer == nullptr)
        {
            return nullptr;
        }

        memcpy(buffer, json, length);
        buffer[length] = '\0';
        return buffer;
    }
}

extern "C" void FreeMemory(void* ptr)
{
    if (ptr != nullptr)
    {
        free(ptr);
    }
}
