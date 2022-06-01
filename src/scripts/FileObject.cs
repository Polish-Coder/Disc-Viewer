using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Disc_Viewer.src.scripts
{
    public class FileObject
    {
        public object Info;
        public string Directory;
        public float SizeInBytes;
        public string Size;

        public FileObject(object info, string directory, float sizeInBytes, string size)
        {
            Info = info;
            Directory = directory;
            SizeInBytes = sizeInBytes;
            Size = size;
        }
    }
}
