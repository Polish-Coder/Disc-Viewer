﻿namespace DiscViewer.Commands;

public static class CommandSystem
{
    private static readonly Dictionary<string, Command> Commands = new();

    private static void Register(Command command)
    {
        Commands[command.Name] = command;
    }

    public static void Setup()
    {
        Register(new ScanCommand());
        Register(new HelpCommand());
        Register(new ExitCommand());
        Register(new GotoCommand());
        Register(new BackCommand());
        Register(new SetCommand());
        Register(new RescanCommand());
        Register(new OpenCommand());
        Register(new DrivesCommand());
    }

    public static Command? GetCommand(string name)
    {
        Commands.TryGetValue(name, out Command? command);
        return command;
    }

    public static IEnumerable<Command> GetAllCommands() => Commands.Values;
}