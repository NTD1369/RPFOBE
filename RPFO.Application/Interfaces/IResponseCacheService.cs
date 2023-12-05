
using RPFO.Application.Helpers;
using RPFO.Data.Entities;
using RPFO.Utilities.Dtos;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Interfaces
{
    public interface IResponseCacheService
    {
        
        Task CacheResponseAsync(string cacheKey, object response, TimeSpan timeToLive);
        Task<string> GetCachedResponseAsync(string cacheKey);
        void CacheData<T>(T dataModel, string cacheKey, TimeSpan timeToLive);
        void ClearCacheData<T>(T dataModel, string cacheKey, TimeSpan timeToLive);
        T GetCachedData<T>(string cacheKey);
        IEnumerable<RedisKey> GetRedisKeys();
    }
}
