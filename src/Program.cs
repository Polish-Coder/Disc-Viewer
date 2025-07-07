using System.Diagnostics;
using System.Globalization;
using System.Reflection;

public class Program
{
    public static async Task Main()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
        
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        
        Version? version = Assembly.GetExecutingAssembly().GetName().Version;
        Console.WriteLine($"Disc Viewer v{version?.Major}.{version?.Minor}.{version?.Build}");
        
        while (true)
        {
            Console.WriteLine("Please enter a directory path...");
        
            string? directory = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(directory))
            {
                Console.WriteLine($"{ConsoleColors.Red}No directory entered!{ConsoleColors.Reset}");
                continue;
            }
        
            if (!Directory.Exists(directory))
            {
                Console.WriteLine($"{ConsoleColors.Red}{directory} does not exists{ConsoleColors.Reset}");
                continue;
            }

            Stopwatch stopwatch = Stopwatch.StartNew();
            (long directorySize, List<FileObject> fileObjects) = await ScanDirectoryAsync(directory);
            stopwatch.Stop();
            
            Console.WriteLine($"Size of {ConsoleColors.Blue}{directory}{ConsoleColors.Reset} is {ConsoleColors.Yellow}{Utils.GetSizeText(directorySize)}{ConsoleColors.Reset}");
            DisplayFileObjects(fileObjects);
            Console.WriteLine($"Scan completed in: {stopwatch.Elapsed.TotalSeconds:F3} seconds\n");
        }
        // ReSharper disable once FunctionNeverReturns
    }

    private static async Task<(long totalSize, List<FileObject> fileObjects)> ScanDirectoryAsync(string directoryPath)
    {
        List<FileObject> fileObjects = new();
        object fileLock = new();
        long totalSize = 0;
        
        string[] files = Directory.GetFiles(directoryPath);
        string[] folders = Directory.GetDirectories(directoryPath);

        Parallel.ForEach(files, new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount }, file =>
        {
            try
            {
                FileInfo fileInfo = new FileInfo(file);
                long size = Utils.GetSize(file);
                
                FileObject fileObject = new FileObject(fileInfo.Name, false, file, size);

                lock (fileLock)
                {
                    fileObjects.Add(fileObject);
                    totalSize += size;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ConsoleColors.Red}Error -> {file}\n{ex.Message}{ConsoleColors.Reset}");
            }
        });
        
        IEnumerable<Task> folderTasks = folders.Select(async folder =>
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folder);
                long size = await GetDirectorySizeAsync(directoryInfo);
                
                FileObject folderObject = new FileObject(directoryInfo.Name, true, folder, size);

                lock (fileLock)
                {
                    fileObjects.Add(folderObject);
                    totalSize += size;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ConsoleColors.Red}Error -> {folder}\n{ex.Message}{ConsoleColors.Reset}");
            }
        });
        
        await Task.WhenAll(folderTasks);
        
        fileObjects.Sort((x, y) => y.SizeInBytes.CompareTo(x.SizeInBytes));
        return (totalSize, fileObjects);
    }
    
    private static async Task<long> GetDirectorySizeAsync(DirectoryInfo directoryInfo)
    {
        return await Task.Run(() =>
            directoryInfo.EnumerateFiles("*", new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true
            }).Sum(file => Utils.GetSize(file.FullName)));
    }
    
    private static void DisplayFileObjects(List<FileObject> fileObjects)
    {
        int maxNameLength = fileObjects.Max(x => x.Name.Length);
        
        foreach (FileObject fileObj in fileObjects)
        {
            string icon = fileObj.IsDirectory ? "\ud83d\udcc1" : "\ud83d\udcc4";
            string fileName = $"{ConsoleColors.Cyan}{fileObj.Name}{ConsoleColors.Reset}";
            string fileSize = Utils.GetSizeText(fileObj.SizeInBytes);

            Console.WriteLine($"{icon} {fileName.PadRight(maxNameLength + 10)} {ConsoleColors.Yellow}{fileSize}{ConsoleColors.Reset}");
        }
    }
}