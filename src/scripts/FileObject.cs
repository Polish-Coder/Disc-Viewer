namespace Disc_Viewer.src.scripts
{
    public class FileObject
    {
        public object Info;
        public string Name;
        public string Directory;
        public float SizeInBytes;
        public string Size;

        public FileObject(object info, string name, string directory, float sizeInBytes, string size)
        {
            Info = info;
            Name = name;
            Directory = directory;
            SizeInBytes = sizeInBytes;
            Size = size;
        }
    }
}
