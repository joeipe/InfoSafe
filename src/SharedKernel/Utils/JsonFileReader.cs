using Newtonsoft.Json;

namespace SharedKernel.Utils
{
    public static class JsonFileReader
    {
        public static T Read<T>(string fileName, string folderName)
        {
            var relativeFilePath = $@"{folderName}{fileName}";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, relativeFilePath);
            string json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}