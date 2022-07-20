#include "ui/MainActivity.hpp"

namespace Mara::ui
{
    MainActivity::MainActivity()
    {
        brls::TabFrame* rootFrame = new brls::TabFrame();
        rootFrame->setTitle("main/title"_i18n);

        brls::List *mainlist = new brls::List();
        rootFrame->addTab("main/maintab"_i18n, mainlist);
        view = rootFrame;
    }

    brls::View* MainActivity::GetView()
    {
        return view;
    }

}
