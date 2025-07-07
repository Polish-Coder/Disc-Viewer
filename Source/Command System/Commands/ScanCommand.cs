using System.Diagnostics;

namespace DiscViewer.Commands;

public class ScanCommand : Command
{
    public override string Name => "scan";
    public override string Usage => "<path>";
    public override string Description => "Scans a directory and calculates the total size including subdirectories";
    
    public override async Task Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine($"{ConsoleColors.Red}No path given. Correct usage: '{Name} {Usage}'.{ConsoleColors.Reset}");
            return;
        }
        
        string path = args[0];
        
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"{ConsoleColors.Red}'{path}' is not a directory.{ConsoleColors.Reset}");
            return;
        }

        await ScanUtils.Scan(path);
    }
}