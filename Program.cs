using Disc_Viewer.src.scripts;

string colorYellow = "\x1b[33m";
string colorBlue = "\u001b[1;34m";
string colorCyan = "\x1b[36m";
string colorWhite = "\x1b[37m";
string colorRed = "\x1b[31m";
string colorReset = "\u001b[0m";

Console.WriteLine("Welcome to Disc Viewer!\nPlease enter folder directory");

string directory = Console.ReadLine();

if (!Directory.Exists(directory))
{
    Console.WriteLine(directory + " does not exists");
    return;
}

string[] files = Directory.GetFiles(directory);
string[] folders = Directory.GetDirectories(directory);

List<FileObject> fileObjects = new();

foreach (string file in files)
{
    FileInfo info = new FileInfo(file);

    try
    {
        float sizeInBytes = info.Length;
        string size = Math.Floor(sizeInBytes) + " B";

        if (sizeInBytes > 1024 * 1024 * 1024)
        {
            float newSize = sizeInBytes / (1024 * 1024 * 1024);
            size = Math.Floor(newSize) + " GB";
        }
        else if (sizeInBytes > 1024 * 1024)
        {
            float newSize = sizeInBytes / (1024 * 1024);
            size = Math.Floor(newSize) + " MB";
        }
        else if (sizeInBytes > 1024)
        {
            float newSize = sizeInBytes / 1024;
            size = Math.Floor(newSize) + " KB";
        }

        fileObjects.Add(new FileObject(info, info.Name, file, sizeInBytes, size));
    }
    catch
    {
        Console.WriteLine(colorRed + "Error -> " + file + colorReset);
        continue;
    }
}

foreach (string folder in folders)
{
    DirectoryInfo info = new DirectoryInfo(folder);

    try
    {
        long sizeInBytes = await Task.Run(() => info.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length));
        string size = MathF.Round(sizeInBytes) + " B";

        if (sizeInBytes > 1024 * 1024 * 1024)
        {
            float newSize = sizeInBytes / (1024 * 1024 * 1024);
            size = Math.Floor(newSize) + " GB";
        }
        else if (sizeInBytes > 1024 * 1024)
        {
            float newSize = sizeInBytes / (1024 * 1024);
            size = Math.Floor(newSize) + " MB";
        }
        else if (sizeInBytes > 1024)
        {
            float newSize = sizeInBytes / 1024;
            size = Math.Floor(newSize) + " KB";
        }

        fileObjects.Add(new FileObject(info, info.Name, folder, sizeInBytes, size));
    }
    catch
    {
        Console.WriteLine(colorRed + "Error -> " + folder + colorReset);
        continue;
    }
}

fileObjects.Sort((x, y) => y.SizeInBytes.CompareTo(x.SizeInBytes));

for (int i = 0; i < fileObjects.Count; i++)
{
    string filePath = $"{colorBlue}{fileObjects[i].Directory[..^fileObjects[i].Name.Length]}{colorReset}{colorCyan}{fileObjects[i].Name}";

    Console.WriteLine($"{filePath}{colorWhite} - {colorYellow}{fileObjects[i].Size}{colorWhite}");
}