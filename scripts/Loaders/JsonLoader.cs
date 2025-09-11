using System.Text.Json;
using Godot;

namespace FlightGlobe.Loaders
{
    public partial class JsonLoader : Node
    {
        public static T[] GetValues<T>(string filePath)
        {
            if (FileAccess.FileExists(filePath))
            {
                var openFile = FileAccess.Open(filePath, FileAccess.ModeFlags.Read);
                return JsonSerializer.Deserialize<T[]>(openFile.GetAsText());
            }
            else
            {
                GD.PrintErr($"File not found: {filePath}");
                return null;
            }
        }
    }
}