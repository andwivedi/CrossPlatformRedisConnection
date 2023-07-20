using StackExchange.Redis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Security.Cryptography;
using System.Text;

class Program
{
    static void Main()
    {
        // Connect to Redis
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost:3003");
        IDatabase db = redis.GetDatabase();

        // Define the list key and elements to push
        string Queue = GetJobQueueForRemoteWorker("AadJobs::IdentitySyncJobs::LicenseRefreshJob");
        string RedisKeyName = GetRedisKeyNameForRemoteWorker(Queue);

        object[] JobParameters = { new Dictionary<string, object>()
        {
            { "tenant_aad_id", "6280a022-fe33-4047-9df3-c102d3f7b258" },
            { "license_refresh_job_queue_id", 1 }
        } };
        
        string RedisValue = GetRedisValueForRemoteWorker(new Dictionary<string, object>()
        {
            { "queue", Queue },
            { "retry", 3 },
            { "backtrace", true }
        }, JobParameters, "AadJobs::IdentitySyncJobs::LicenseRefreshJob");

        Console.WriteLine($"Writing this key {RedisKeyName} with value as {RedisValue}");

        db.ListLeftPush(RedisKeyName, RedisValue);

        // Close the Redis connection
        redis.Close();

        Console.WriteLine("Data pushed to Redis list successfully!");
        Console.ReadLine();
    }

    private static string GetJobQueueForRemoteWorker(string JobType)
    {
        string filePath =
        @"C:\Users\andwivedi\source\repos\RedisConnection\Constants\QueueMappings.json";


        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            Dictionary<string, string> data = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);

            if (data.ContainsKey(JobType))
            {
                return data[JobType];
            }
            else
            {
                return "default";
            }
        }
        else
        {
            Console.WriteLine("File does not exist.");
        }
        return "";
    }

    private static string GetRedisKeyNameForRemoteWorker(string QueueName)
    {
        return $"queue:{QueueName}";
    }

    private static Dictionary<string, object> GetFormattedJobOption(string JobClass, object[] JobParameters)
    {
        Dictionary<string, object> hash = new Dictionary<string, object>();
        hash.Add("class", JobClass);
        hash.Add("args", JobParameters);
        return hash;
    }

    private static Dictionary<string, object> AppendAdditionalParameters(Dictionary<string, object> RemoteWorkerOptions)
    {
        Dictionary<string, object> newDictionary = new Dictionary<string, object>(RemoteWorkerOptions);
        newDictionary.Add("created_at", DateTimeOffset.Now.ToUnixTimeMilliseconds());
        newDictionary.Add("jid", GenerateSecureRandomHex(12));
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

    private static string GetRedisValueForRemoteWorker(Dictionary<string, object> RemoteWorkerOptions, object[] RemoteWorkerJobParameters, string JobClass)
    {
        Dictionary<string, object> JobParameters = GetFormattedJobOption(JobClass, RemoteWorkerJobParameters);
        Dictionary<string, object> NormalizedArguments = JobParameters.Union(RemoteWorkerOptions).ToDictionary(k => k.Key, v => v.Value);
        Dictionary<string, object> OptionsForRedis = AppendAdditionalParameters(NormalizedArguments);
        OptionsForRedis.Add("enqueued_at", DateTimeOffset.Now.ToUnixTimeMilliseconds());
        return JsonConvert.SerializeObject(OptionsForRedis);
    }
}
