using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Mara.Lib.Common
{
    public class Open
    {
        //https://stackoverflow.com/questions/4580263/how-to-open-in-default-browser-in-c-sharp
        public static void OpenPath(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url.Replace("&", "^&")}")
                        {CreateNoWindow = true});
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    Process.Start("xdg-open", url);
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    Process.Start("open", url);
                else
                    throw;
            }
        }
    }
}
