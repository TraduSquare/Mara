using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mara.Lib.Common
{
    public class SplitFile
    {
        /* Code: https://stackoverflow.com/a/3967595 */
        public void Split(string inputFile, int chunkSize, string path)
        {
            const int BUFFER_SIZE = 20 * 2048;
            byte[] buffer = new byte[BUFFER_SIZE];

            using (Stream input = File.OpenRead(inputFile))
            {
                int index = 0;
                while (input.Position < input.Length)
                {
                    using (Stream output = File.Create(path + "\\" + index))
                    {
                        int remaining = chunkSize, bytesRead;
                        while (remaining > 0 && (bytesRead = input.Read(buffer, 0,
                                Math.Min(remaining, BUFFER_SIZE))) > 0)
                        {
                            output.Write(buffer, 0, bytesRead);
                            remaining -= bytesRead;
                        }
                    }
                    index++;
                }
            }
            SetArchiveBit(path + "\\");
        }

        public void SetArchiveBit(string path)
        {
            DirectoryInfo dir = new DirectoryInfo(path);
            dir.Attributes |= FileAttributes.Archive;
        }
    }
}
