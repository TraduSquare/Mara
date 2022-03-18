using System.IO;
using PleOps.XdeltaSharp.Decoder;

namespace Unity.Mara.Lib.Common
{
    public class Xdelta
    {
        public static void Apply(FileStream file, byte[] xdeltaFile, string outfile)
        {
            var outStream = new FileStream(outfile, FileMode.Create, FileAccess.ReadWrite, FileShare.Read);
            var xdelta = new MemoryStream(xdeltaFile);

            var decoder = new Decoder(file, xdelta, outStream);
            decoder.Run();
            outStream.Close();
            file.Close();
        }
    }
}
