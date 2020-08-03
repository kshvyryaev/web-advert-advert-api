using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Amazon.DynamoDBv2;
using WebAdvert.Models;
using Amazon.DynamoDBv2.DataModel;

namespace WebAdvert.AdvertApi.Services
{
    public class AdvertsService : IAdvertsService
    {
        private readonly IMapper _mapper;

        public AdvertsService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<string> Create(AdvertModel model)
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

        public async Task Confirm(ConfirmAdvertModel model)
        {
            using var client = new AmazonDynamoDBClient();
            using var context = new DynamoDBContext(client);

            var record = await context.LoadAsync(model.Id);
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
    }
}
