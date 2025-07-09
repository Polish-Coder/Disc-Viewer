using System.Diagnostics;

public static class ScanUtils
{
    public static async Task Scan(string path)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        (long directorySize, List<FileObject> fileObjects) = await ScanDirectoryAsync(path, Options.Depth);
        stopwatch.Stop();
            
        PrintUtils.PrintDirectory(path, directorySize, fileObjects, out int skippedCount);
        
        if (skippedCount > 0)
        {
            Console.WriteLine($"Skipped {ConsoleColors.Yellow}{skippedCount}{ConsoleColors.Reset} items based on current settings.");
        }
        
        Console.WriteLine($"Scan completed in: {stopwatch.Elapsed.TotalSeconds:F3} seconds.\n");
        
        Program.CurrentDirectory = path;
    }

    private static async Task<(long totalSize, List<FileObject> fileObjects)> ScanDirectoryAsync(string directoryPath, byte depth)
    {
        return await ScanDirectoryInternalAsync(directoryPath, depth);
    }
    
    private static async Task<(long totalSize, List<FileObject> fileObjects)> ScanDirectoryInternalAsync(string directoryPath, byte remainingDepth)
    {
        List<FileObject> fileObjects = new();
        object fileLock = new();
        long totalSize = 0;
        
        string[] files;
        string[] folders;
        
        try
        {
            files = Directory.GetFiles(directoryPath);
            folders = Directory.GetDirectories(directoryPath);
        }
        catch
        {
            return (0, fileObjects);
        }

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
                Console.WriteLine($"{ConsoleColors.Red}Error -> '{file}'\n{ex.Message}{ConsoleColors.Reset}");
            }
        });
        
        IEnumerable<Task> folderTasks = folders.Select(async folder =>
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folder);
                long size = await GetDirectorySizeAsync(directoryInfo);
                bool isAccessible = FileUtils.IsDirectoryAccessible(folder);
                
                FileObject folderObject = new FileObject(directoryInfo.Name, true, folder, size, isAccessible);

                if (remainingDepth > 0)
                {
                    (long _, List<FileObject> children) = await ScanDirectoryInternalAsync(folder, (byte)(remainingDepth - 1));
                    SortItems(children);
                    folderObject.Children.AddRange(children);
                }
                
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
        
        SortItems(fileObjects);
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

    private static void SortItems(List<FileObject> items)
    {
        items.Sort((x, y) =>
        {
            int result = y.SizeInBytes.CompareTo(x.SizeInBytes);
            if (result != 0) return result;

            result = y.IsAccessible.CompareTo(x.IsAccessible);
            if (result != 0) return result;
            
            return string.Compare(x.Name, y.Name, StringComparison.Ordinal);
        });
    }
}