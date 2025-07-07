namespace DiscViewer.Commands;

public class ExitCommand : Command
{
    public override string Name => "exit";
    public override string Usage => "";
    public override string Description => "Closes the application";
    
    public override Task Execute(string[] args)
    {
        Environment.Exit(0);
        return Task.CompletedTask;
    }
}