using Mara.Lib.Platforms.PS3.IO;

Console.WriteLine("MARA PS3 DECRYPTOR");

if (args.Length > 0) {
    if (args[0].Equals("RAP"))
    {
        var key = RAP.GetKey(args[1]);
        Console.WriteLine($"KEY: {BitConverter.ToString(key).Replace("-","")}");
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