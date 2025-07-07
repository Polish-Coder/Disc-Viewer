namespace DiscViewer.Commands;

public class GotoCommand : Command
{
    public override string Name => "goto";
    public override string Usage => "<folder name>";
    public override string Description => "Scans a subdirectory with the given name located inside the last scanned path";
    
    public override async Task Execute(string[] args)
    {
        Command scanCommand = CommandSystem.GetCommand("scan")!;
        
        if (Program.CurrentDirectory == null)
        {
            Console.WriteLine($"{ConsoleColors.Red}No directory has been scanned recently. Use '{scanCommand.Name} {scanCommand.Usage}' first.{ConsoleColors.Reset}");
            return;
        }
        
        if (args.Length == 0)
        {
            Console.WriteLine($"{ConsoleColors.Red}No folder name given. Correct usage: '{Name} {Usage}'.{ConsoleColors.Reset}");
            return;
        }
        
        string subfolderName = args[0];
        string path = Path.Combine(Program.CurrentDirectory, subfolderName);
        
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"{ConsoleColors.Red}'{subfolderName}' is not a subfolder of '{Program.CurrentDirectory}'.{ConsoleColors.Reset}");
            return;
        }
        
        await ScanUtils.Scan(path);
    }
}