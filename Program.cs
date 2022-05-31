// See https://aka.ms/new-console-template for more information
Console.WriteLine("Welcome to Disc Viewer!\nPlease enter folder directory");

string directory = Console.ReadLine();

if (Directory.Exists(directory))
{
    Console.WriteLine(directory + " exists");
}
else
{
    Console.WriteLine(directory + " does not exists");
}
