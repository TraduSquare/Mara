using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Mara.Lib.Common
{
    public class Git
    {
        public static void GetRepoZip(string token, string url, string name, string Folder)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var credentials = string.Format(CultureInfo.InvariantCulture, "{0}:", token);
                credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);
                var contents = client.GetByteArrayAsync(url).Result;
                File.WriteAllBytes(@Folder + "/" + name, contents);
            }
        }
    }
}
