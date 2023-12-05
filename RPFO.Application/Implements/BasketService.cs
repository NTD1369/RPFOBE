 
using Dapper;
using Dapper.Contrib.Extensions;
//using Dapper.SimpleCRUD;
//using DapperExtensions;
//using Dapper.Contrib.Extensions;
 
using Microsoft.EntityFrameworkCore.Storage;
using RPFO.Application.Interfaces;
using RPFO.Data.ViewModels;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class BasketService : IBasketService
    {
        private readonly StackExchange.Redis.IDatabase _database;
        public BasketService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }
        public async Task<bool> DeleteBasketAsync(string basketId)
        {
            return await _database.KeyDeleteAsync(basketId);
        }

        public async Task<BasketViewModel> GetBasketAsync(string basketId)
        {
            var data = await _database.StringGetAsync(basketId);
            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<BasketViewModel>(data);
        }

        public async Task<BasketViewModel> UpdateBasketAsync(BasketViewModel basket)
        {
            var created = await _database.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), TimeSpan.FromDays(30));
            if (!created)
                return null;
            return await GetBasketAsync(basket.Id);
        }
    }
}
