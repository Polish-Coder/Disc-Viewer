using Disc_Viewer.src.scripts;

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

    fileObjects.Add(new FileObject(info, file, sizeInBytes, size));
}

foreach (string folder in folders)
{
    DirectoryInfo info = new DirectoryInfo(folder);

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

    fileObjects.Add(new FileObject(info, folder, sizeInBytes, size));
}

fileObjects.Sort((x, y) => y.SizeInBytes.CompareTo(x.SizeInBytes));

for (int i = 0; i < fileObjects.Count; i++)
{
    Console.WriteLine(fileObjects[i].Directory + " - " + fileObjects[i].Size);
}