#include "MainActivity.hpp"

using namespace brls::literals;

brls::View* MainActivity::createContentView()
{
    brls::TabFrame* frame = new brls::TabFrame();
    frame->setTitle("Program/title"_i18n);

    return frame;
}