using Disc_Viewer.src.scripts;

Console.WriteLine("Welcome to Disc Viewer!\nPlease enter folder directory");

string directory = Console.ReadLine()!;

if (!Directory.Exists(directory))
{
    Console.WriteLine(directory + " does not exists");
    return;
}

DirectoryInfo directoryInfo = new(directory);
long directorySize = await Task.Run(() => directoryInfo.EnumerateFiles("*", new EnumerationOptions {IgnoreInaccessible = true, RecurseSubdirectories = true}).Sum(file => file.Length));
Console.WriteLine($"Size of {directory} is {Utils.GetSizeText(directorySize)}");

string[] files = Directory.GetFiles(directory);
string[] folders = Directory.GetDirectories(directory);

List<FileObject> fileObjects = new();

foreach (string file in files)
{
    FileInfo info = new(file);

    try
    {
        float sizeInBytes = info.Length;

        fileObjects.Add(new FileObject(info, info.Name, file, sizeInBytes, Utils.GetSizeText(sizeInBytes)));
    }
    catch
    {
        Console.WriteLine(ConsoleColors.Red + "Error -> " + file + ConsoleColors.Reset);
        continue;
    }
}

foreach (string folder in folders)
{
    DirectoryInfo info = new(folder);

    try
    {
        long sizeInBytes = await Task.Run(() => info.EnumerateFiles("*", SearchOption.AllDirectories).Sum(file => file.Length));

        fileObjects.Add(new FileObject(info, info.Name, folder, sizeInBytes, Utils.GetSizeText(sizeInBytes)));
    }
    catch
    {
        Console.WriteLine(ConsoleColors.Red + "Error -> " + folder + ConsoleColors.Reset);
        continue;
    }
}

fileObjects.Sort((x, y) => y.SizeInBytes.CompareTo(x.SizeInBytes));

for (int i = 0; i < fileObjects.Count; i++)
{
    string filePath = $"{ConsoleColors.Blue}{fileObjects[i].Directory[..^fileObjects[i].Name.Length]}{ConsoleColors.Reset}{ConsoleColors.Cyan}{fileObjects[i].Name}";

    Console.WriteLine($"{filePath}{ConsoleColors.White} - {ConsoleColors.Yellow}{fileObjects[i].Size}{ConsoleColors.White}");
}