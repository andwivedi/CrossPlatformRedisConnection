using System;
using System.Security.Cryptography;
using System.Text;

namespace Utilities
{
    internal class GenerateSidekiqOptions
    {
        public static Dictionary<string, object> GetSidekiqJobOptions(string JobClass, string QueueName, Dictionary<string, object> JobParameters)
        {
            Dictionary<string, object> hash = new Dictionary<string, object>();
            // In Sidekiq args are passed as an array, so we need to convert it into array of dictionaries
            // Example: { "arg1": "value1", "arg2": "value2" } => [ { "arg1" : "value1", "args2": "value2" }]
            List<Dictionary<string, object>> args = new List<Dictionary<string, object>> { JobParameters };

            hash.Add("class", JobClass);
            hash.Add("args", args);
            hash.Add("queue", QueueName);
            return hash;
        }

        public static Dictionary<string, object> AddSidekiqGeneratedParameters(Dictionary<string, object> RemoteWorkerOptions)
        {
            Dictionary<string, object> newDictionary = new Dictionary<string, object>(RemoteWorkerOptions);
            newDictionary.Add("created_at", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            newDictionary.Add("jid", GenerateSecureRandomHex(12));
            newDictionary.Add("enqueued_at", DateTimeOffset.UtcNow.ToUnixTimeMilliseconds());
            return newDictionary;
        }

        private static string GenerateSecureRandomHex(int byteLength)
        {
            byte[] randomBytes = new byte[byteLength];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }

            StringBuilder sb = new StringBuilder(byteLength * 2);
            foreach (byte b in randomBytes)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString().ToLower();
        }
    }
}
