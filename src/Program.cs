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
            
            PrintDirectory(directory, directorySize, fileObjects);
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
    
    private static void PrintDirectory(string directory, long directorySize, List<FileObject> fileObjects)
    {
        int sizeIndent = Math.Max(fileObjects.Max(x => x.Name.Length), directory.Length) + 5;

        string directoryName = $"{ConsoleColors.Blue}{directory}{ConsoleColors.Reset}";
        string directorySizeText = $"{ConsoleColors.Yellow}{Utils.GetSizeText(directorySize)}";
        int lineLength = sizeIndent - directoryName.Length + 10;
        string line = new string(ConsoleSymbols.Line[0], lineLength);
        Console.WriteLine($"{ConsoleSymbols.Folder} {directoryName} {line} {directorySizeText}{ConsoleColors.Reset}");

        for (int i = 0; i < fileObjects.Count; i++)
        {
            FileObject fileObject = fileObjects[i];
            
            string treeSymbol = i == fileObjects.Count - 1 ? ConsoleSymbols.TreeEnd : ConsoleSymbols.TreeBranch;
            string icon = fileObject.IsDirectory ? ConsoleSymbols.Folder : ConsoleSymbols.File;
            string fileName = $"{ConsoleColors.Cyan}{fileObject.Name.PadRight(sizeIndent - treeSymbol.Length - 1)}";
            string fileSize = $"{ConsoleColors.Yellow}{Utils.GetSizeText(fileObject.SizeInBytes)}";

            Console.WriteLine($"{treeSymbol} {icon} {fileName} {fileSize}{ConsoleColors.Reset}");
        }
    }
}