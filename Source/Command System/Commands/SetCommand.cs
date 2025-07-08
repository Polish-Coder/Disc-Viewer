namespace DiscViewer.Commands;

public class SetCommand : Command
{
    public override string Name => "set";
    public override string Usage => "<option> <value>";

    public override string Description => "Sets a global configuration option that affects the behavior of subsequent commands";
    
    public override Task Execute(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine($"{ConsoleColors.Red}No all required arguments given. Correct usage: '{Name} {Usage}'.{ConsoleColors.Reset}");
            return Task.CompletedTask;
        }

        string option = args[0];
        string value = args[1];
        
        switch (option)
        {
            case "minsize":
                if (ParseUtils.TryParseSize(value, out long result))
                {
                    Options.MinSize = result;
                    Console.WriteLine($"{ConsoleColors.Green}Successfully set 'minsize' value to '{value}'.{ConsoleColors.Reset}");
                }
                break;
            default:
                Console.WriteLine($"{ConsoleColors.Red}Unknown option: '{option}'. Type '{Name}' for available options.{ConsoleColors.Reset}");
                break;
        }
        
        return Task.CompletedTask;
    }
}