Console.WriteLine("Welcome to Disc Viewer!\nPlease enter folder directory");

string directory = Console.ReadLine();

if (Directory.Exists(directory))
{
    Console.WriteLine(directory + " exists");

    string[] files = Directory.GetFiles(directory);
    string[] folders = Directory.GetDirectories(directory);

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

        Console.WriteLine(info.Name + " - " + size);
    }

    foreach (string folder in folders)
    {
        Console.WriteLine(folder);
    }
}
else
{
    Console.WriteLine(directory + " does not exists");
}
