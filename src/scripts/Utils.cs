namespace Disc_Viewer.src.scripts
{
    public static class Utils
    {
        public static string GetSizeText(float sizeInBytes)
        {
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

            return size;
        }
    }
}
