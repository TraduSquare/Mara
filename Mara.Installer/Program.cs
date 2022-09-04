using System.Diagnostics;
using System.Runtime.InteropServices;
using Mara.Lib.Common.Steam.Api;

Console.WriteLine("Mara.Installer - v1.0.0");
Console.WriteLine("Descargando parcheador...");

Mara.Lib.Common.Internet.GetFile("", Path.Combine(Path.GetTempPath(), "patcher.zip"));
Console.WriteLine("Descargado!");
string installfolder = "";
string ExeName = "./Luancher";
string AppName = "Mara.Patcher";

if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
{
    // TODO: Detectar el usuario actual
    installfolder = Path.Combine($"{Path.DirectorySeparatorChar}home", "deck", "HyperDeck");
    if (Directory.Exists(installfolder))
        Directory.Delete(installfolder, true);
    Console.WriteLine("Extrayendo...");
    Mara.Lib.Common.Lzma.Unpack(Path.Combine(Path.GetTempPath(), "patcher.zip"),installfolder);
    Console.WriteLine("Hecho!");
    Process.Start("chmod", $"777 {Path.Combine(installfolder, ExeName)}");
    Shortcut.addGameAllUsers(AppName, Path.Combine(installfolder, ExeName));
}