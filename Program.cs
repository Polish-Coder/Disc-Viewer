public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("Welcome to Disc Viewer!");
        
        while (true)
        {
            Console.WriteLine("Please enter folder directory");
        
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

            (long directorySize, List<FileObject> fileObjects) = await ScanDirectoryAsync(directory);
            Console.WriteLine($"Size of {directory} is {Utils.GetSizeText(directorySize)}");
            DisplayFileObjects(fileObjects);
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
                
                FileObject fileObject = new FileObject(fileInfo.Name, file, size);

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
                
                var folderObject = new FileObject(directoryInfo.Name, folder, size);

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
        foreach (FileObject fileObj in fileObjects)
        {
            string directoryName = Path.GetDirectoryName(fileObj.Directory) ?? "";
            string filePath = $"{ConsoleColors.Blue}{directoryName}{Path.DirectorySeparatorChar}{ConsoleColors.Reset}{ConsoleColors.Cyan}{fileObj.Name}";
            string fileSize = Utils.GetSizeText(fileObj.SizeInBytes);

            Console.WriteLine($"{filePath}{ConsoleColors.White} - {ConsoleColors.Yellow}{fileSize}{ConsoleColors.Reset}");
        }
    }
}