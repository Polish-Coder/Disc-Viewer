using System.Globalization;
using System.Reflection;
using DiscViewer.Commands;

public class Program
{
    public static async Task Main()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        CommandSystem.Setup();
        
        Version? version = Assembly.GetExecutingAssembly().GetName().Version;
        Console.WriteLine($"Disc Viewer v{version?.Major}.{version?.Minor}.{version?.Build}");
        
        await CommandLoop.Run();
    }
}