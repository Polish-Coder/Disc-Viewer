namespace DiscViewer.Commands;

public static class CommandLoop
{
    public static async Task Run()
    {
        while (true)
        {
            Console.Write("> ");
            string? input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine($"{ConsoleColors.Red}No command entered. Type 'help' fo available commands.{ConsoleColors.Reset}");
                continue;
            }
            
            string[] commandParts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string commandName = commandParts[0];
            string[] arguments = commandParts.Skip(1).ToArray();
            
            Command? command = CommandSystem.GetCommand(commandName);

            if (command == null)
            {
                Console.WriteLine($"{ConsoleColors.Red}Unknown command: '{commandName}'. Type 'help' fo available commands.{ConsoleColors.Reset}");
                continue;
            }

            try
            {
                await command.Execute(arguments);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ConsoleColors.Red}Error: {ex.Message}.{ConsoleColors.Reset}");
            }
        }
        // ReSharper disable once FunctionNeverReturns
    }
}