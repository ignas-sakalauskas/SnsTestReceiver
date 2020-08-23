using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SnsTestReceiver.Api.Models.Request;
using SnsTestReceiver.Api.Services;
using SnsTestReceiver.Sdk.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SnsTestReceiver.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(IRepository repository, ILogger<MessagesController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] GetAllQuery query)
        {
            return Ok(_repository.Search(query.Search, query.Limit ?? 10));
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync([FromServices] IOptions<ApiBehaviorOptions> apiBehaviorOptions)
        {
            SnsMessage message;
            try
            {
                using var reader = new StreamReader(Request.Body, Encoding.UTF8);
                var content = await reader.ReadToEndAsync();
                message = JsonSerializer.Deserialize<SnsMessage>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Body", e.Message);
                return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
            }

            // TODO split into service/helpers
            var context = new ValidationContext(message);
            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(message, context, results, true);
            if (!isValid)
            {
                foreach (var result in results)
                {
                    ModelState.AddModelError(result.MemberNames.First(), result.ErrorMessage);
                }

                return apiBehaviorOptions.Value.InvalidModelStateResponseFactory(ControllerContext);
            }

            if (!_repository.TryCreate(message.MessageId, message))
                return Conflict();

            _logger.LogDebug($"Message with ID={message.MessageId} created successfully");

            return CreatedAtAction(nameof(Get), new { message.MessageId }, message);
        }

        [HttpGet("{messageId}")]
        public IActionResult Get(string messageId)
        {
            var item = _repository.Get(messageId);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpDelete("{messageId}")]
        public IActionResult Delete(string messageId)
        {
            if (!_repository.Delete(messageId))
                return NotFound();

            return NoContent();
        }
    }
}
