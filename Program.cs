using Disc_Viewer.src.scripts;

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
        
            DirectoryInfo directoryInfo = new(directory);
            long directorySize = await GetDirectorySizeAsync(directoryInfo);
            Console.WriteLine($"Size of {directory} is {Utils.GetSizeText(directorySize)}");
        
            List<FileObject> fileObjects = await GetFileObjectsAsync(directory);
            DisplayFileObjects(fileObjects);
        }
    }
    
    private static async Task<long> GetDirectorySizeAsync(DirectoryInfo directoryInfo)
    {
        return await Task.Run(() =>
            directoryInfo.EnumerateFiles("*", new EnumerationOptions
            {
                IgnoreInaccessible = true,
                RecurseSubdirectories = true
            }).Sum(file => file.Length));
    }
    
    private static async Task<List<FileObject>> GetFileObjectsAsync(string directoryPath)
    {
        List<FileObject> fileObjects = new List<FileObject>();

        string[] files = Directory.GetFiles(directoryPath);
        string[] folders = Directory.GetDirectories(directoryPath);

        foreach (string file in files)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(file);
                long size = fileInfo.Length;

                fileObjects.Add(new FileObject(fileInfo, fileInfo.Name, file, size, Utils.GetSizeText(size)));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ConsoleColors.Red}Error -> {file}\n{ex.Message}{ConsoleColors.Reset}");
            }
        }

        IEnumerable<Task<FileObject?>> folderTasks = folders.Select(async folder =>
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folder);
                long size = await GetDirectorySizeAsync(directoryInfo);

                return new FileObject(directoryInfo, directoryInfo.Name, folder, size, Utils.GetSizeText(size));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ConsoleColors.Red}Error -> {folder}\n{ex.Message}{ConsoleColors.Reset}");
                return null;
            }
        });

        FileObject?[] folderResults = await Task.WhenAll(folderTasks);
        fileObjects.AddRange(folderResults.Where(f => f != null)!);

        fileObjects.Sort((x, y) => y!.SizeInBytes.CompareTo(x!.SizeInBytes));
        return fileObjects!;
    }
    
    private static void DisplayFileObjects(List<FileObject> fileObjects)
    {
        foreach (FileObject fileObj in fileObjects)
        {
            string directoryName = Path.GetDirectoryName(fileObj.Directory) ?? "";
            string filePath = $"{ConsoleColors.Blue}{directoryName}{Path.DirectorySeparatorChar}{ConsoleColors.Reset}{ConsoleColors.Cyan}{fileObj.Name}";

            Console.WriteLine($"{filePath}{ConsoleColors.White} - {ConsoleColors.Yellow}{fileObj.Size}{ConsoleColors.White}");
        }
    }
}