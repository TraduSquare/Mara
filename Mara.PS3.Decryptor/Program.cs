using System.Text;
using Mara.Lib.Platforms.PS3.Crypto;
using Mara.Lib.Platforms.PS3.IO;

byte[] rif = new byte[0x10], dev, flags, type, version;
var contentid = new byte[0x30];

Console.WriteLine("MARA PS3 DECRYPTOR");

if (args.Length > 0)
{
    if (args[0].Equals("RAP"))
    {
        var key = RAP.GetKey(args[1]);
        Console.WriteLine($"KEY: {BitConverter.ToString(key).Replace("-", "")}");
    }
    else if (args[0].Equals("EDAT"))
    {
        switch (args[1])
        {
            case "DECRYPT":
                rif = Utils.StringToByteArray(args[3]);
                dev = Utils.StringToByteArray(args[4]);

                Console.WriteLine($"DevKLic: {BitConverter.ToString(dev).Replace("-", "")}");
                Console.WriteLine($"Rif key: {BitConverter.ToString(rif).Replace("-", "")}");

                EDAT.decryptFile(args[2], args[5], dev, rif);
                break;
            case "ENCRYPT":
                var input = args[2];
                var output = args[3];

                dev = Utils.StringToByteArray(args[4]);
                var CID = Utils.StringToByteArray(BitConverter.ToString(Encoding.UTF8.GetBytes(args[5]))
                    .Replace("-", ""));
                Array.Copy(CID,0,contentid,0,CID.Length);
                flags = Utils.StringToByteArray(args[6]);
                type = Utils.StringToByteArray(args[7]);
                version = Utils.StringToByteArray(args[8]);
                rif = null;

                Console.WriteLine($"Content-ID: {BitConverter.ToString(contentid).Replace("-", "")}");
                Console.WriteLine($"DevKLic: {BitConverter.ToString(dev).Replace("-", "")}");
                Console.WriteLine(rif != null
                    ? $"KeyFromRif: {BitConverter.ToString(rif).Replace("-", "")}"
                    : "KeyFromRif: null");
                Console.WriteLine($"Flags: {BitConverter.ToString(flags).Replace("-", "")}");

                EDAT.encryptFile(input, output, dev, rif, contentid, flags, type, version);
                break;
        }
    }
}
else
{
    PrintInfo();
}


static void PrintInfo()
{
    Console.WriteLine("EXTRACT RIFKEY");
    Console.WriteLine("RAP [FILEPATH]");
}