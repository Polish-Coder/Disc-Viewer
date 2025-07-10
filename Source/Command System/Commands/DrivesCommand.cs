using static ConsoleColors;
using static ConsoleSymbols;

namespace DiscViewer.Commands;

public class DrivesCommand : Command
{
    public override string Name => "drives";
    public override string Usage => "";
    public override string Description => "Lists all drives with free and total space, including usage bar";
    
    public override Task Execute(string[] args)
    {
        foreach (DriveInfo drive in DriveInfo.GetDrives())
        {
            string volumeLabel = drive.VolumeLabel;

            if (string.IsNullOrWhiteSpace(volumeLabel))
            {
                volumeLabel = drive.DriveType == DriveType.Fixed ? "Local Disk" : "Disk";
            }
            
            string drivePath = drive.Name;

            if (!string.IsNullOrEmpty(drivePath) && drivePath.EndsWith("\\"))
            {
                drivePath = drivePath[..^1];
            }
            
            string driveName = $"{(volumeLabel)} ({drivePath})";
            string driveNameFormatted = $"{BrightBlue}{driveName}{Reset}";
            
            if (!drive.IsReady)
            {
                Console.WriteLine($"{driveNameFormatted} - {Gray}Offline{Reset}\n");
                continue;
            }

            const int barWidth = 50;

            long freeSpace = drive.TotalFreeSpace;
            long totalSize = drive.TotalSize;
            long usedSpace = totalSize - freeSpace;
            float percentage = (float)usedSpace / totalSize;

            string icon = drive.DriveType switch
            {
                DriveType.Fixed => LocalDrive,
                DriveType.Removable => RemovableDrive,
                DriveType.CDRom => CdRomDrive,
                DriveType.Network => NetworkDrive,
                DriveType.Ram => RamDrive,
                DriveType.NoRootDirectory => NoRootDrive,
                _ => UnknownDrive
            };

            string name = $"{icon} {driveNameFormatted}";
            string usedText = $"Used: {PrintUtils.GetSizeText(usedSpace)}";
            string totalText = $"Total: {PrintUtils.GetSizeText(totalSize)}";
            string freeSpaceText = $"Free: {PrintUtils.GetSizeText(freeSpace)}";
            string percentageText = $"{percentage:P}";
            
            Console.WriteLine($"{name}{drive.DriveFormat.PadLeft(barWidth - driveName.Length - icon.Length - 1)}");
            Console.WriteLine($"{usedText}{totalText.PadLeft(barWidth - usedText.Length)}");
            PrintUtils.PrintBar(usedSpace, totalSize, barWidth, true);
            Console.WriteLine($"{freeSpaceText}{percentageText.PadLeft(barWidth - freeSpaceText.Length)}");
            Console.WriteLine();
        }
        
        return Task.CompletedTask;
    }
}