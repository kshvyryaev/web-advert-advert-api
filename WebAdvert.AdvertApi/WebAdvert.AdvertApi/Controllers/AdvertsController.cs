using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using WebAdvert.AdvertApi.Services;
using WebAdvert.Models;
using WebAdvert.Models.Messages;

namespace WebAdvert.AdvertApi.Controllers
{
    [ApiController]
    [Route("adverts/v1")]
    public class AdvertsController : ControllerBase
    {
        private readonly IAdvertsService _advertsService;
        private readonly IConfiguration _configuration;

        public AdvertsController(IAdvertsService advertsService, IConfiguration configuration)
        {
            _advertsService = advertsService;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("create")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Create(AdvertModel model)
        {
            try
            {
                var recordId = await _advertsService.CreateAsync(model);
                var response = new CreateAdvertResponse { Id = recordId };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("confirm")]
        [ProducesResponseType(200, Type = typeof(CreateAdvertResponse))]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Confirm(ConfirmAdvertModel model)
        {
            try
            {
                await _advertsService.ConfirmAsync(model);
                await RaiseAdvertConfirmedMessage(model);
                return Ok();
            }
            catch (KeyNotFoundException)
            {

                return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                var advert = await _advertsService.GetByIdAsync(id);
                return Ok(advert);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                return new StatusCodeResult(500);
            }
        }

        [HttpGet]
        [Route("all")]
        [ProducesResponseType(200)]
        [EnableCors("AllOrigin")]
        public async Task<IActionResult> GetAll()
        {
            var adverts = await _advertsService.GetAllAsync();
            return Ok(adverts);
        }

        private async Task RaiseAdvertConfirmedMessage(ConfirmAdvertModel model)
        {
            var topicArn = _configuration.GetValue<string>("TopicArn");
            var dbModel = await _advertsService.GetByIdAsync(model.Id);

            using var client = new AmazonSimpleNotificationServiceClient();

            var message = new AdvertConfirmedMessage
            {
                Id = model.Id,
                Title = dbModel.Title
            };

            var messageJson = JsonConvert.SerializeObject(message);
            await client.PublishAsync(topicArn, messageJson);
        }
    }
}
