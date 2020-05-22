using System.IO;
using System.Text.Json;

namespace SmartACDeviceAPI.Test.Utils
{
public static class WriteDataFile {
    public static void WriteData(string fileToWrite, object data) {
        if (File.Exists(fileToWrite)) 
                File.Delete(fileToWrite);
            File.WriteAllText(fileToWrite, JsonSerializer.Serialize(data));
    }

    public static bool FileExisits(string fileName) {
        return File.Exists(fileName);
    }
}
}