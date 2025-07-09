public class FileObject
{
    public readonly string Name;
    public readonly bool IsDirectory;
    public readonly string Directory;
    public readonly float SizeInBytes;
    public readonly List<FileObject> Children;

    public FileObject(string name, bool isDirectory, string directory, float sizeInBytes)
    {
        Name = name;
        IsDirectory = isDirectory;
        Directory = directory;
        SizeInBytes = sizeInBytes;
        Children = new List<FileObject>();
    }
}
