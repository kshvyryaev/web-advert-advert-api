using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Amazon.DynamoDBv2;
using WebAdvert.Models;
using Amazon.DynamoDBv2.DataModel;
using System.Linq;

namespace WebAdvert.AdvertApi.Services
{
    public class AdvertsService : IAdvertsService
    {
        private readonly IMapper _mapper;

        public AdvertsService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<string> CreateAsync(AdvertModel model)
        {
            var dbModel = _mapper.Map<AdvertDbModel>(model);
            dbModel.Id = Guid.NewGuid().ToString();
            dbModel.CreationDateTime = DateTime.UtcNow;
            dbModel.Status = AdvertStatus.Pending;

            using var client = new AmazonDynamoDBClient();
            using var context = new DynamoDBContext(client);
            await context.SaveAsync(dbModel);

            return dbModel.Id;
        }

        public async Task ConfirmAsync(ConfirmAdvertModel model)
        {
            using var client = new AmazonDynamoDBClient();
            using var context = new DynamoDBContext(client);

            var record = await context.LoadAsync<AdvertDbModel>(model.Id);
            if (record == null)
            {
                throw new KeyNotFoundException($"A record with id={model.Id} was not found.");
            }

            if (model.Status == AdvertStatus.Active)
            {
                model.Status = AdvertStatus.Active;
                await context.SaveAsync(record);
            }
            else
            {
                await context.DeleteAsync(record);
            }
        }

        public async Task<List<AdvertModel>> GetAllAsync()
        {
            using var client = new AmazonDynamoDBClient();
            using var context = new DynamoDBContext(client);

            var scanResult = await context
                .ScanAsync<AdvertDbModel>(new List<ScanCondition>())
                .GetNextSetAsync();

            return scanResult
                .Select(item => _mapper.Map<AdvertModel>(item))
                .ToList();
        }

        public async Task<AdvertModel> GetByIdAsync(string id)
        {
            var client = new AmazonDynamoDBClient();
            using var context = new DynamoDBContext(client);

            var dbModel = await context.LoadAsync<AdvertDbModel>(id);

            if (dbModel == null)
            {
                throw new KeyNotFoundException();
            }

            return _mapper.Map<AdvertModel>(dbModel);
        }

        public async Task<bool> CheckHealthAsync()
        {
            using var client = new AmazonDynamoDBClient();
            var tableData = await client.DescribeTableAsync("Adverts");
            return string.Compare(tableData.Table.TableStatus, "active", StringComparison.OrdinalIgnoreCase) == 0;
        }
    }
}
