namespace DiscViewer.Commands;

public class BackCommand : Command
{
    public override string Name => "back";
    public override string Usage => "";
    public override string Description => "Scans the parent directory of the last scanned path";
    
    public override async Task Execute(string[] args)
    {
        Command scanCommand = CommandSystem.GetCommand("scan")!;
        
        if (Program.CurrentDirectory == null)
        {
            Console.WriteLine($"{ConsoleColors.Red}No directory has been scanned recently. Use '{scanCommand.Name} {scanCommand.Usage}' first.{ConsoleColors.Reset}");
            return;
        }
        
        string? path = Directory.GetParent(Program.CurrentDirectory)?.FullName;
        
        if (path == null || !Directory.Exists(path))
        {
            Console.WriteLine($"{ConsoleColors.Red}'{path}' is not a directory.{ConsoleColors.Reset}");
            return;
        }
        
        await ScanUtils.Scan(path);
    }
}