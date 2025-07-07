using System.Diagnostics;

namespace DiscViewer.Commands;

public class ScanCommand : Command
{
    public override string Name => "scan";
    public override string Usage => "<path>";
    public override string Description => "Scan a directory";
    
    public override async Task Execute(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine($"{ConsoleColors.Red}No path given. Correct usage: '{Name} {Usage}'.{ConsoleColors.Reset}");
            return;
        }
        
        string path = args[0];
        
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"{ConsoleColors.Red}'{path}' is not a directory.{ConsoleColors.Reset}");
            return;
        }
        
        Stopwatch stopwatch = Stopwatch.StartNew();
        (long directorySize, List<FileObject> fileObjects) = await ScanDirectoryAsync(path);
        stopwatch.Stop();
            
        PrintUtils.PrintDirectory(path, directorySize, fileObjects);
        Console.WriteLine($"Scan completed in: {stopwatch.Elapsed.TotalSeconds:F3} seconds.\n");
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
                long size = FileUtils.GetSize(file);
                
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
            }).Sum(file => FileUtils.GetSize(file.FullName)));
    }
}