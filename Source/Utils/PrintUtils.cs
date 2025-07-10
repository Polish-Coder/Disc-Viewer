using System.Text;
using static ConsoleColors;
using static ConsoleSymbols;

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

        string directoryName = $"{Bold}{BrightBlue}{directory}{Reset}";
        string directorySizeText = $"{Yellow}{GetSizeText(directorySize)}";
        int lineLength = itemsCount != 0 ? sizeIndent - directoryName.Length + 12 : 5;
        string line = new string(Line[0], lineLength);
        Console.WriteLine($"{Folder} {directoryName} {line} {directorySizeText}{Reset}");

        if (baseCount == 0)
        {
            Console.WriteLine($"{BrightYellow}This directory is empty...{Reset}");
            skippedCount = 0;
            return;
        }
        
        if (itemsCount == 0)
        {
            Console.WriteLine($"{BrightYellow}Nothing to display - all item were excluded based on the current options.{Reset}");
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
        const string pipe = TreePipe + "   ";
        const string empty = "    ";
        
        float percentage = item.SizeInBytes / directorySize * 100;

        StringBuilder pipes = new StringBuilder();

        foreach (bool isParentLast in parentLastList)
        {
            pipes.Append(isParentLast ? empty : pipe);
        }
        
        string treeSymbol = pipes + (isLast ? TreeEnd : TreeBranch);
        string icon = item.IsAccessible
            ? item.IsDirectory ? Folder : ConsoleSymbols.File
            : Locked;
        string nameColor = item.IsAccessible ? Cyan : Gray;
        string fileName = $"{nameColor}{item.Name.PadRight(sizeIndent - treeSymbol.Length - 1)}";
        string fileSize = $"{Yellow}{GetSizeText(item.SizeInBytes),-percentageIndent}";
        string filePercentage = $"[ {percentage:F1}% ]";
        
        Console.WriteLine($"{treeSymbol} {icon} {fileName} {fileSize}{Reset}{filePercentage}");

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
    
    public static string GetSizeText(float sizeInBytes)
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

    public static void PrintBar(long value, long max, int barWidth = 30, bool colored = false)
    {
        float percentage = (float)value / max;
        int filled = (int)(percentage * barWidth);
        int empty = barWidth - filled;

        string color = colored ? percentage switch
        {
            < 0.65f => Green,
            < 0.9f => Yellow,
            _ => Red
        } : "";
        
        string bar = color + string.Concat(Enumerable.Repeat(FullBlock, filled)) +
                     Gray + string.Concat(Enumerable.Repeat(FullBlock, empty));
        
        Console.WriteLine(bar + Reset);
    }
}