#if __INTELLISENSE__
typedef unsigned int __SIZE_TYPE__;
typedef unsigned long __PTRDIFF_TYPE__;
#define __attribute__(q)
#define __builtin_strcmp(a,b) 0
#define __builtin_strlen(a) 0
#define __builtin_memcpy(a,b) 0
#define __builtin_va_list void*
#define __builtin_va_start(a,b)
#define __extension__
#endif

#if defined(_MSC_VER)
#include <BaseTsd.h>
typedef SSIZE_T ssize_t;
#endif

#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include <switch.h>
#include <borealis.hpp>

#include "Program/Main.hpp"
#include "ui/MainActivity.hpp"

// Main program entrypoint
int main(int argc, char* argv[])
{
    appletInitializeGamePlayRecording();

    i18n::loadTranslations();
    
    brls::Logger::setLogLevel(brls::LogLevel::DEBUG);
    
    if (!brls::Application::init("main/title"_i18n))
    {
        brls::Logger::error("Unable to init Mara");
        return EXIT_FAILURE;
    }

    // Establece que se pueda salir de la app
    //brls::Application::setGlobalQuit(true);
    auto win = new Mara::ui::MainActivity();
    brls::Application::pushView(win->GetView());
    
    while (brls::Application::mainLoop());

    return EXIT_SUCCESS;
}
