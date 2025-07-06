public class FileObject
{
    public readonly string Name;
    public readonly string Directory;
    public readonly float SizeInBytes;

    public FileObject(string name, string directory, float sizeInBytes)
    {
        Name = name;
        Directory = directory;
        SizeInBytes = sizeInBytes;
    }
}
