using Mara.Lib.Platforms.PS3.IO;

byte[] rif = new byte[0x10], dev, flags, type, version;
byte[] contentid = new byte[0x30];

Console.WriteLine("MARA PS3 DECRYPTOR");

if (args.Length > 0) {
    if (args[0].Equals("RAP"))
    {
        var key = RAP.GetKey(args[1]);
        Console.WriteLine($"KEY: {BitConverter.ToString(key).Replace("-","")}");
    }
    else if (args[0].Equals("EDAT"))
    {
        switch (args[1])
        {
            case "DECRYPT":
                rif = StringToByteArray(args[3]);
                dev = StringToByteArray(args[4]);
        
                Console.WriteLine($"DevKLic: {BitConverter.ToString(dev).Replace("-","")}");
                Console.WriteLine($"Rif key: {BitConverter.ToString(rif).Replace("-","")}");

                EDAT.decryptFile(args[2], args[5], dev, rif);
                break;
            case "ENCRYPT":
                string input = args[2];
                string output = args[3];
                
                dev = StringToByteArray(args[4]);
                contentid = StringToByteArray(args[5]);
                flags = StringToByteArray(args[6]);
                type = StringToByteArray(args[7]);
                version = StringToByteArray(args[8]);
                
                Console.WriteLine($"Content-ID: {BitConverter.ToString(contentid).Replace("-","")}");
                Console.WriteLine($"DevKLic: {BitConverter.ToString(dev).Replace("-","")}");
                Console.WriteLine($"KeyFromRif: {BitConverter.ToString(rif).Replace("-","")}");
                Console.WriteLine($"Flags: {BitConverter.ToString(flags).Replace("-","")}");
                
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

static byte[] StringToByteArray(string hex) {
    return Enumerable.Range(0, hex.Length)
        .Where(x => x % 2 == 0)
        .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
        .ToArray();
}