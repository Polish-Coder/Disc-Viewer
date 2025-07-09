using System.Text;

public static class PrintUtils
{
    public static void PrintDirectory(string directory, long directorySize, List<FileObject> fileObjects, out int skippedCount)
    {
        int baseCount = fileObjects.Count;
        
        FileObject[] displayedFiles = fileObjects.Where(x => x.SizeInBytes >= Options.MinSize).ToArray();
        int itemsCount = displayedFiles.Length;
        skippedCount = baseCount - itemsCount;

        int longestName = Math.Max(GetMaxNameLength(displayedFiles), directory.Length);
        int sizeIndent = itemsCount != 0 ? longestName + 8 : 0;

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
            bool isLast = i == itemsCount - 1;
            
            PrintItem(fileObject, directorySize, sizeIndent, 0, isLast, new List<bool>(), ref skippedCount);
        }
    }

    private static void PrintItem(FileObject item, long directorySize, int sizeIndent, byte level, bool isLast, List<bool> parentLastList, ref int skippedCount)
    {
        const int percentageIndent = 10;
        const string pipe = ConsoleSymbols.TreePipe + "   ";
        const string empty = "    ";
        
        float percentage = item.SizeInBytes / directorySize * 100;

        StringBuilder pipes = new StringBuilder();

        foreach (bool isParentLast in parentLastList)
        {
            pipes.Append(isParentLast ? empty : pipe);
        }
        
        string treeSymbol = pipes + (isLast ? ConsoleSymbols.TreeEnd : ConsoleSymbols.TreeBranch);
        string icon = item.IsDirectory ? ConsoleSymbols.Folder : ConsoleSymbols.File;
        string fileName = $"{ConsoleColors.Cyan}{item.Name.PadRight(sizeIndent - treeSymbol.Length - 1)}";
        string fileSize = $"{ConsoleColors.Yellow}{GetSizeText(item.SizeInBytes),-percentageIndent}";
        string filePercentage = $"[ {percentage:F1}% ]";
        
        Console.WriteLine($"{treeSymbol} {icon} {fileName} {fileSize}{ConsoleColors.Reset}{filePercentage}");

        List<FileObject> children = item.Children.Where(x => x.SizeInBytes >= Options.MinSize).ToList();
        int childCount = children.Count;
        
        skippedCount += item.Children.Count - childCount;
        
        for (int i = 0; i < childCount; i++)
        {
            FileObject child = children[i];
            byte childLevel = (byte)(level + 1);
            bool isChildLast = i == childCount - 1;
            
            List<bool> newParentLastList = new List<bool>(parentLastList) { isLast };
            
            PrintItem(child, directorySize, sizeIndent, childLevel, isChildLast, newParentLastList, ref skippedCount);
        }
    }

    private static int GetMaxNameLength(IEnumerable<FileObject> fileObjects)
    {
        int max = 0;
        Stack<(FileObject item, int depth)> stack = new(fileObjects.Select(x => (x, 0)));

        while (stack.Count > 0)
        {
            (FileObject item, int depth) = stack.Pop();
            
            if (item.SizeInBytes < Options.MinSize) continue;
            
            int nameLength = item.Name.Length + depth * 4;
            max = Math.Max(max, nameLength);
            
            foreach (FileObject child in item.Children)
            {
                stack.Push((child, depth + 1));
            }
        }

        return max;
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