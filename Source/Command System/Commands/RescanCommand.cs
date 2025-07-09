namespace DiscViewer.Commands;

public class RescanCommand : Command
{
    public override string Name => "rescan";
    public override string Usage => "";
    public override string Description => "Scans the previous directory and calculates the total size including subdirectories";
    
    public override async Task Execute(string[] args)
    {
        Command scanCommand = CommandSystem.GetCommand("scan")!;
        
        if (Program.CurrentDirectory == null)
        {
            Console.WriteLine($"{ConsoleColors.Red}No directory has been scanned recently. Use '{scanCommand.Name} {scanCommand.Usage}' instead.{ConsoleColors.Reset}");
            return;
        }

        await ScanUtils.Scan(Program.CurrentDirectory);
    }
}