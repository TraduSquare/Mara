using System;
using System.Net;

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
    }
}
