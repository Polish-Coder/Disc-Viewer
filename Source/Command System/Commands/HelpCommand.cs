namespace DiscViewer.Commands;

public class HelpCommand : Command
{
    public override string Name => "help";
    public override string Usage => "";
    public override string Description => "Shows available commands";
    
    public override Task Execute(string[] args)
    {
        Command[] commands = CommandSystem.GetAllCommands().OrderBy(x => x.Name).ToArray();
        
        int indentLevel = commands.Max(x => x.Name.Length + x.Usage.Length) + 5;
        
        List<string> commandList = new List<string>();
        
        foreach (Command command in commands)
        {
            string padded = $"{command.Name} {command.Usage}".PadRight(indentLevel);
            string formatted = command.Usage.Length > 0
                ? padded.Replace(command.Usage, $"{ConsoleColors.Gray}{command.Usage}{ConsoleColors.Reset}")
                : padded;
            commandList.Add($"{formatted}{command.Description}");
        }
        
        PrintUtils.PrintTitledFrame("Available commands", commandList.ToArray());
        
        return Task.CompletedTask;
    }
}