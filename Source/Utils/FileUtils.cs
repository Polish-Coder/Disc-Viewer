using System.Runtime.InteropServices;

public static class FileUtils
{
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern uint GetCompressedFileSizeW(string lpFileName, out uint lpFileSizeHigh);
    
    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode,
        IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool CloseHandle(IntPtr hObject);
    
    private const uint GenericRead = 0x80000000;
    private const uint FileShareRead = 0x00000001;
    private const uint FileShareWrite = 0x00000002;
    private const uint OpenExisting = 3;
    private const uint FileFlagBackupSemantics = 0x02000000;

    public static bool IsDirectoryAccessible(string path)
    {
        IntPtr handle = CreateFile(path, GenericRead, FileShareRead | FileShareWrite,
            IntPtr.Zero, OpenExisting, FileFlagBackupSemantics, IntPtr.Zero);

        if (handle.ToInt64() == -1) return false;
        
        CloseHandle(handle);
        return true;
    }
    
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
}