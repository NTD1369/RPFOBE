
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Data.ViewModels;
using RPFO.Utilities.Dtos;
using RPFO.Utilities.Extensions;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _database;
        public ResponseCacheService(IConnectionMultiplexer redis /*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _database = redis.GetDatabase();
        }

        public async Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive)
        {
            if (response == null)
            {
                return;
            }
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            var s = JsonSerializer.Serialize(response, options);

            await _database.StringSetAsync(cacheKey, s, timeToLive);
        }

        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            var cachedResponse = await _database.StringGetAsync(cacheKey);
            if (cachedResponse.IsNullOrEmpty)
            {
                return null;
            }
            return cachedResponse;
        }

        public void CacheData<T>(T dataModel, string cacheKey, TimeSpan timeToLive)
        {
            if (dataModel == null)
            {
                return;
            }
            string data = dataModel.ToJsonMinify();

            _database.StringSet(cacheKey, data, expiry: timeToLive);
        }
        public void ClearCacheData<T>(T dataModel, string cacheKey, TimeSpan timeToLive)
        {
            string data = dataModel.ToJsonMinify();

            _database.StringSet(cacheKey, data, expiry: timeToLive);
        }
        public T GetCachedData<T>(string cacheKey)
        {
            var cachedResponse = _database.StringGet(cacheKey);
            if (cachedResponse.IsNullOrEmpty)
            {
                return default(T);
            }
            return cachedResponse.ToString().JsonToModel<T>();
        }

        public IEnumerable<RedisKey> GetRedisKeys()
        {
            var endPoint = _database.Multiplexer.GetEndPoints().FirstOrDefault();
            return _database.Multiplexer.GetServer(endPoint).Keys(pattern: "*");
        }
    }
}
