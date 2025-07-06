public class FileObject
{
    public readonly string Name;
    public readonly bool IsDirectory;
    public readonly string Directory;
    public readonly float SizeInBytes;

    public FileObject(string name, bool isDirectory, string directory, float sizeInBytes)
    {
        Name = name;
        IsDirectory = isDirectory;
        Directory = directory;
        SizeInBytes = sizeInBytes;
    }
}
