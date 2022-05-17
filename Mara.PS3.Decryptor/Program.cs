using Mara.Lib.Platforms.PS3.IO;

Console.WriteLine("MARA PS3 DECRYPTOR");

if (args.Length > 0) {
    if (args[0].Equals("RAP"))
    {
        var key = RAP.GetKey(args[1]);
        Console.WriteLine($"KEY: {BitConverter.ToString(key).Replace("-","")}");
    }
    else if (args[0].Equals("EDAT"))
    {
        byte[] rif = StringToByteArray(args[2]);
        byte[] dev = StringToByteArray(args[3]);
        
        Console.WriteLine($"DevKLic: {BitConverter.ToString(dev).Replace("-","")}");
        Console.WriteLine($"Rif key: {BitConverter.ToString(rif).Replace("-","")}");

        EDAT.decryptFile(args[1], args[4], dev, rif);
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