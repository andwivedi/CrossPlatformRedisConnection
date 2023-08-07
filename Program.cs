using System;
using Contracts;
using Utilities;

class Program
{
    static void Main()
    {
        // Initialize Redis connection and Sidekiq client
        // Fetch connection string from Key Vault/ENV level flags
        IRedisConnection redisClient = new RedisConnection("localhost:3003");
        ISidekiqClient sidekiqClient = new SidekiqClient(redisClient);

        // Generate Sidekiq job options
        var JobParameters = new Dictionary<string, object>()
        {
            { "tenant_aad_id", "6280a022-fe33-4047-9df3-c102d3f7b258" },
            { "license_refresh_job_queue_id", 1 }
        };

        Dictionary<string, object> SidekiqJobOptions = new Dictionary<string, object>()
        {
            { "retry", 3 },
            { "backtrace", true }
        };
        string ClassName = "AadJobs::IdentitySyncJobs::LicenseRefreshJob";

        // Push the data to Sidekiq
        sidekiqClient.Enqueue(SidekiqJobOptions, JobParameters, ClassName);

        Console.WriteLine("Data pushed to Redis list successfully!");
        Console.ReadLine();
    }
}
