using Newtonsoft.Json;

namespace Utilities
{
    internal class JsonUtilities
    {
        public static string GetJobQueueNameForSidekiq(string JobType)
        {
            string filePath =
            @"C:\Users\andwivedi\source\repos\RedisConnection\Constants\QueueMappings.json";


            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

                return data.ContainsKey(JobType) ? data[JobType] : "default";
            }
            else
            {
                Console.WriteLine("File does not exist.");
            }
            return "";
        }
    }
}
