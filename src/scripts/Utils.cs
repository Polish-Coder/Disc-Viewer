using System.Runtime.InteropServices;

public static class Utils
{
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern uint GetCompressedFileSizeW(string lpFileName, out uint lpFileSizeHigh);

    public static long GetSize(string path)
    {
        try
        {
            uint low = GetCompressedFileSizeW(path, out uint high);
            long output = ((long)high << 32) + low;

            if (low != 0xFFFFFFFF) return output;
            
            int error = Marshal.GetLastWin32Error();
            return error != 0 ? 0 : output;
        }
        catch
        {
            return 0;
        }
    }
    
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