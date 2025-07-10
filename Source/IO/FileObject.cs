public class FileObject
{
    public readonly string Name;
    public readonly bool IsDirectory;
    public readonly string Directory;
    public readonly long SizeInBytes;
    public readonly bool IsAccessible;
    public readonly List<FileObject> Children;

    public FileObject(string name, bool isDirectory, string directory, long sizeInBytes, bool isAccessible = true)
    {
        Name = name;
        IsDirectory = isDirectory;
        Directory = directory;
        SizeInBytes = sizeInBytes;
        IsAccessible = isAccessible;
        Children = new List<FileObject>();
    }
}
