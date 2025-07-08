public static class PrintUtils
{
    public static void PrintDirectory(string directory, long directorySize, List<FileObject> fileObjects, out int skippedCount)
    {
        int baseCount = fileObjects.Count;
        
        FileObject[] displayedFiles = fileObjects.Where(x => x.SizeInBytes >= Options.MinSize).ToArray();
        int itemsCount = displayedFiles.Length;
        skippedCount = baseCount - itemsCount;

        int sizeIndent = itemsCount != 0 ? Math.Max(displayedFiles.Max(x => x.Name.Length), directory.Length) + 5 : 0;
        const int percentageIndent = 10;

        string directoryName = $"{ConsoleColors.Blue}{directory}{ConsoleColors.Reset}";
        string directorySizeText = $"{ConsoleColors.Yellow}{GetSizeText(directorySize)}";
        int lineLength = itemsCount != 0 ? sizeIndent - directoryName.Length + 10 : 5;
        string line = new string(ConsoleSymbols.Line[0], lineLength);
        Console.WriteLine($"{ConsoleSymbols.Folder} {directoryName} {line} {directorySizeText}{ConsoleColors.Reset}");

        if (baseCount == 0)
        {
            Console.WriteLine($"{ConsoleColors.Yellow}This directory is empty...{ConsoleColors.Reset}");
            skippedCount = 0;
            return;
        }
        
        if (itemsCount == 0)
        {
            Console.WriteLine($"{ConsoleColors.Yellow}Nothing to display - all item were excluded based on the current options.{ConsoleColors.Reset}");
            return;
        }
        
        for (int i = 0; i < itemsCount; i++)
        {
            FileObject fileObject = displayedFiles[i];

            float percentage = fileObject.SizeInBytes / directorySize * 100;
            
            string treeSymbol = i == itemsCount - 1 ? ConsoleSymbols.TreeEnd : ConsoleSymbols.TreeBranch;
            string icon = fileObject.IsDirectory ? ConsoleSymbols.Folder : ConsoleSymbols.File;
            string fileName = $"{ConsoleColors.Cyan}{fileObject.Name.PadRight(sizeIndent - treeSymbol.Length - 1)}";
            string fileSize = $"{ConsoleColors.Yellow}{GetSizeText(fileObject.SizeInBytes),-percentageIndent}";
            string filePercentage = $"[ {percentage:F1}% ]";

            Console.WriteLine($"{treeSymbol} {icon} {fileName} {fileSize}{ConsoleColors.Reset}{filePercentage}");
        }
    }
    
    private static string GetSizeText(float sizeInBytes)
    {
        string size;

        if (sizeInBytes >= 1024 * 1024 * 1024)
        {
            float newSize = sizeInBytes / (1024 * 1024 * 1024);
            size = $"{newSize:F1} GB";
        }
        else if (sizeInBytes >= 1024 * 1024)
        {
            float newSize = sizeInBytes / (1024 * 1024);
            size = $"{newSize:F1} MB";
        }
        else if (sizeInBytes >= 1024)
        {
            float newSize = sizeInBytes / 1024;
            size = $"{newSize:F0} KB";
        }
        else
        {
            size = $"{sizeInBytes:F0} B";
        }

        return size;
    }
}