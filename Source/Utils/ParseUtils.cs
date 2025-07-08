public static class ParseUtils
{
    public static bool TryParseSize(string input, out long value)
    {
        value = 0;
        
        input = input.Trim().ToLowerInvariant();
        
        string number = new string(input.TakeWhile(char.IsDigit).ToArray());
        string unit = input[number.Length..].Trim();

        if (!long.TryParse(number, out long result))
        {
            Console.WriteLine($"{ConsoleColors.Red}The given value '{number}' is not a number.{ConsoleColors.Reset}");
            return false;
        }

        if (unit is not ("" or "b" or "m" or "mb" or "g" or "gb" or "t" or "tb"))
        {
            Console.WriteLine($"{ConsoleColors.Red}Unknown size unit: '{unit}'.{ConsoleColors.Reset}");
            return false;
        }
        
        value = unit switch
        {
            "k" or "kb" => result * 1024,
            "m" or "mb" => result * 1024 * 1024,
            "g" or "gb" => result * 1024 * 1024 * 1024,
            "t" or "tb" => result * 1024 * 1024 * 1024 * 1024,
            _ => result
        };
        
        return true;
    }
}