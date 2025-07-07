public static class PrintUtils
{
    public static void PrintDirectory(string directory, long directorySize, List<FileObject> fileObjects)
    {
        int sizeIndent = Math.Max(fileObjects.Max(x => x.Name.Length), directory.Length) + 5;
        const int percentageIndent = 10;

        string directoryName = $"{ConsoleColors.Blue}{directory}{ConsoleColors.Reset}";
        string directorySizeText = $"{ConsoleColors.Yellow}{GetSizeText(directorySize)}";
        int lineLength = sizeIndent - directoryName.Length + 10;
        string line = new string(ConsoleSymbols.Line[0], lineLength);
        Console.WriteLine($"{ConsoleSymbols.Folder} {directoryName} {line} {directorySizeText}{ConsoleColors.Reset}");

        for (int i = 0; i < fileObjects.Count; i++)
        {
            FileObject fileObject = fileObjects[i];

            float percentage = fileObject.SizeInBytes / directorySize * 100;
            
            string treeSymbol = i == fileObjects.Count - 1 ? ConsoleSymbols.TreeEnd : ConsoleSymbols.TreeBranch;
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