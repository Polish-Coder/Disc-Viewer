using System.Diagnostics;

namespace DiscViewer.Commands;

public class OpenCommand : Command
{
    public override string Name => "open";
    public override string Usage => "";
    public override string Description => "Opens the current directory in the system's file explorer";
    
    public override Task Execute(string[] args)
    {
        Command scanCommand = CommandSystem.GetCommand("scan")!;
        
        if (Program.CurrentDirectory == null)
        {
            Console.WriteLine($"{ConsoleColors.Red}No directory has been scanned recently. Use '{scanCommand.Name} {scanCommand.Usage}' first.{ConsoleColors.Reset}");
            return Task.CompletedTask;
        }
        
        Process.Start("explorer.exe", Program.CurrentDirectory);
        Console.WriteLine($"Opening directory: '{Program.CurrentDirectory}' in File Explorer...");
        return Task.CompletedTask;
    }
}