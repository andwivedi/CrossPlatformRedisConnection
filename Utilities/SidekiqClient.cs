using Newtonsoft.Json;
using Contracts;

namespace Utilities
{
    internal class SidekiqClient : ISidekiqClient
    {
        private readonly IRedisConnection _redisConnection;

        public SidekiqClient(IRedisConnection redisConnection) 
        {
            _redisConnection = redisConnection;
        }
        public void Enqueue(Dictionary<string, object> SidekiqJobOptions, Dictionary<string, object> JobParameters, string ClassName)
        {
            // 1. Get the queue name for the job
            string QueueName = JsonUtilities.GetJobQueueNameForSidekiq(ClassName);
            string RedisKeyName = GetRedisKeyNameForSidekiq(QueueName);

            // 2. Get the Redis value for the job
            string RedisValue = GetValueForSidekiq(SidekiqJobOptions, JobParameters, ClassName, QueueName);

            // 3. Push the Redis value to the Redis list and add it to the set
            _redisConnection.AddToSet("queues", QueueName);
            _redisConnection.PushToList(RedisKeyName, RedisValue);
        }

        private static string GetRedisKeyNameForSidekiq(string QueueName)
        {
            return $"queue:{QueueName}";
        }

        private static string GetValueForSidekiq(Dictionary<string, object> RemoteWorkerOptions, Dictionary<string, object> JobParams, string JobClass, string QueueName)
        {
            Dictionary<string, object> JobParameters = GenerateSidekiqOptions.GetSidekiqJobOptions(JobClass, QueueName, JobParams);
            Dictionary<string, object> JobOptions = JobParameters.Union(RemoteWorkerOptions).ToDictionary(k => k.Key, v => v.Value);
            Dictionary<string, object> SidekiqGeneratedOptions = GenerateSidekiqOptions.AddSidekiqGeneratedParameters(JobOptions);
            return JsonConvert.SerializeObject(SidekiqGeneratedOptions);
        }
    }
}
