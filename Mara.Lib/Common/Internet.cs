using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;

namespace Mara.Lib.Common
{
    public class Internet
    {
        public static void GetFile(string url, string path)
        {
            var random = new Random();
            url += $"?random={random.Next()}";
            using var client = new WebClient();
            client.DownloadFile(url, path);
        }
        
        public static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
