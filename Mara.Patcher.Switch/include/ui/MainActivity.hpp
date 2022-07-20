#pragma once

#include <borealis.hpp>
#include "Program/Main.hpp"

namespace Mara::ui
{
    class MainActivity
    {
    private:
        brls::View* view;
    public:
        MainActivity();

        brls::View* GetView();
    };
}
