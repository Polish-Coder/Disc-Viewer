using System.Collections.Concurrent;
using System.Diagnostics;

public static class ScanUtils
{
    private static readonly EnumerationOptions EnumerationOptions = new()
    {
        IgnoreInaccessible = true,
        RecurseSubdirectories = true
    };

    private static readonly ParallelOptions ParallelOptions = new()
    {
        MaxDegreeOfParallelism = Environment.ProcessorCount
    };

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

    private static Task<(long totalSize, List<FileObject> fileObjects)> ScanDirectoryAsync(string directoryPath, byte depth)
        => ScanDirectoryInternalAsync(directoryPath, depth);
    
    private static async Task<(long totalSize, List<FileObject> fileObjects)> ScanDirectoryInternalAsync(string directoryPath, byte remainingDepth)
    {
        ConcurrentBag<FileObject> fileBag = new();
        long totalSize = 0;

        IEnumerable<string> entries = Directory.EnumerateFileSystemEntries(directoryPath);
        List<string> files = new();
        List<string> folders = new();
        
        try
        {
            foreach (string entry in entries)
            {
                try
                {
                    if (File.GetAttributes(entry).HasFlag(FileAttributes.Directory))
                    {
                        folders.Add(entry);
                    }
                    else
                    {
                        files.Add(entry);
                    }
                }
                catch
                {
                    // Ignored
                }
            }
        }
        catch
        {
            return (0, new List<FileObject>());
        }

        Parallel.ForEach(files, ParallelOptions, file =>
        {
            try
            {
                string name = Path.GetFileName(file);
                long size = FileUtils.GetSize(file);
                fileBag.Add(new FileObject(name, false, file, size));
                Interlocked.Add(ref totalSize, size);
            }
            catch
            {
                // Ignored
            }
        });
        
        ConcurrentBag<FileObject> folderBag = new();
        SemaphoreSlim semaphore = new(Environment.ProcessorCount);
        
        IEnumerable<Task> folderTasks = folders.Select(async folder =>
        {
            await semaphore.WaitAsync();

            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folder);
                long size = await GetDirectorySizeAsync(directoryInfo);
                bool isAccessible = FileUtils.IsDirectoryAccessible(folder);
                
                FileObject folderObject = new FileObject(directoryInfo.Name, true, folder, size, isAccessible);

                if (remainingDepth > 0)
                {
                    try
                    {
                        (long _, List<FileObject> children) = await ScanDirectoryInternalAsync(folder, (byte)(remainingDepth - 1));
                        folderObject.Children.AddRange(children);
                        SortItems(children);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                
                folderBag.Add(folderObject);
                Interlocked.Add(ref totalSize, size);
            }
            catch
            {
                // Ignored
            }
            finally
            {
                semaphore.Release();
            }
        });
        
        await Task.WhenAll(folderTasks);
        
        List<FileObject> allItems = fileBag.Concat(folderBag).ToList();
        SortItems(allItems);
        return (totalSize, allItems);
    }
    
    private static Task<long> GetDirectorySizeAsync(DirectoryInfo directoryInfo)
    {
        IEnumerable<FileInfo> files = directoryInfo.EnumerateFiles("*", EnumerationOptions);

        long totalSize = 0;
        
        Parallel.ForEach(files, ParallelOptions, file =>
        {
            Interlocked.Add(ref totalSize, FileUtils.GetSize(file.FullName));
        });
        
        return Task.FromResult(totalSize);
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