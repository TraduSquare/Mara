#include "ui/MainActivity.hpp"

namespace Mara::ui
{
    MainActivity::MainActivity()
    {
        brls::TabFrame* rootFrame = new brls::TabFrame();
        rootFrame->setTitle("main/title"_i18n);
        view = rootFrame;
    }

    brls::View* MainActivity::GetView()
    {
        return view;
    }

}
