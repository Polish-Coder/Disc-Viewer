public static class OptionsManager
{
    public static void SetMinSize(string value)
    {
        if (!ParseUtils.TryParseSize(value, out long result)) return;
        
        Options.MinSize = result;
        SuccessMessage("minsize", value);
    }

    public static void SetDepth(string value)
    {
        const int maxDepth = 5;
        
        if (!byte.TryParse(value, out byte result) || result > maxDepth)
        {
            Console.WriteLine($"{ConsoleColors.Red}The given value '{value}' is not a number in the range 0 - {maxDepth}.{ConsoleColors.Reset}");
            return;
        }
        
        Options.Depth = result;
        SuccessMessage("depth", value);
    }

    private static void SuccessMessage(string option, string value)
    {
        Console.WriteLine($"{ConsoleColors.Green}Successfully set '{option}' value to '{value}'.{ConsoleColors.Reset}");
    }
}