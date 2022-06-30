using System;

namespace Mara.Lib.Platforms.PS3;

public class Main : PatchProcess
{
    public Main(string oriFolder, string outFolder, string filePath) : base(oriFolder, outFolder, filePath)
    {
        
    }

    public override (int, string) ApplyTranslation()
    {
        throw new NotImplementedException();
    }
}