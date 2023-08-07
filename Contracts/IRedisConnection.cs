using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Represents a Redis connection interface for interacting with the Redis server.
/// </summary>

namespace Contracts
{
    internal interface IRedisConnection
    {

        /// <summary>
        /// Pushes a value to the left end of a Redis list identified by the specified key.
        /// </summary>
        /// <param name="RedisKeyName">The name of the Redis list where the value will be pushed.</param>
        /// <param name="RedisValue">The value to be pushed to the Redis list.</param>
        void PushToList(string RedisKeyName, string RedisValue);

        /// <summary>
        /// Adds a value to a Redis set identified by the specified key.
        /// </summary>
        /// <param name="RedisKeyName">The name of the Redis set where the value will be added.</param>
        /// <param name="RedisValue">The value to be added to the Redis set.</param>
        void AddToSet(string RedisKeyName, string RedisValue);
    }
}
