using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.AdvertApi.Services;
using WebAdvert.Models;

namespace WebAdvert.AdvertApi.Controllers
{
    [ApiController]
    [Route("adverts/v1")]
    public class AdvertsController : ControllerBase
    {
        private readonly IAdvertsService _advertsService;

        public AdvertsController(IAdvertsService advertsService)
        {
            _advertsService = advertsService;
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
    }
}
