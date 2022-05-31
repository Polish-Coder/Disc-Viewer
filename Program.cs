Console.WriteLine("Welcome to Disc Viewer!\nPlease enter folder directory");

string directory = Console.ReadLine();

if (Directory.Exists(directory))
{
    Console.WriteLine(directory + " exists");

    string[] files = Directory.GetFiles(directory);
    string[] folders = Directory.GetDirectories(directory);

    foreach (string file in files)
    {
        Console.WriteLine(file);
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
