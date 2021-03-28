using System;
using System.Collections.Generic;
using System.Text;

namespace FastXdeltaGenerator
{
    class PatchGenerator
    {
        private string oriPath;
        private string modPath;
        private string outPath;

        public PatchGenerator(string oriPassed, string modPassed, string outPassed)
        {
            oriPath = oriPassed;
            modPath = modPassed;
            outPath = outPassed;
        }

        public void GeneratePatch()
        {

        }
    }
}
