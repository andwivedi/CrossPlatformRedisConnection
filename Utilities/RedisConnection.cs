using Newtonsoft.Json.Linq;
using Contracts;
using StackExchange.Redis;
using System;

namespace Utilities
{
    internal class RedisConnection : IRedisConnection
    {
        private readonly IDatabase _database;
        public RedisConnection(string connectionString) 
        {
            try
            {
                _database = ConnectionMultiplexer.Connect(connectionString).GetDatabase();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error connecting to Redis: {ex.Message}");
            }
        }

        public void PushToList(string keyName, string Value)
        {
            try
            {
                _database.ListLeftPush(keyName, Value);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error pushing to Redis: {ex.Message}");
            }
        }

        public void AddToSet(string keyName, string Value)
        {
            try
            {
                _database.SetAdd(keyName, Value);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding to Redis: {ex.Message}");
            }
        }
    }
}
