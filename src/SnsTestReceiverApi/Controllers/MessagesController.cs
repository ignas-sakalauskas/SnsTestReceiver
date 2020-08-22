using Microsoft.AspNetCore.Mvc;
using SnsTestReceiverApi.Models;
using SnsTestReceiverApi.Services;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SnsTestReceiverApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly IRepository _repository;

        public MessagesController(IRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [ActionName("GetAll")]
        public IActionResult GetAll(string search, int? limit)
        {
            if (limit > 100)
                return UnprocessableEntity(new ErrorResponse { Error = "Limit must be less then 100" });

            return Ok(_repository.Search(search, limit ?? 100));
        }

        [HttpPost]
        [ActionName("Post")]
        public async Task<IActionResult> Post()
        {
            string content;
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                content = await reader.ReadToEndAsync();
            }

            SnsMessage message;
            try
            {
                message = JsonSerializer.Deserialize<SnsMessage>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                });
            }
            catch (Exception e)
            {
                return BadRequest(new ErrorResponse { Error = $"Parsing error: {e.Message}" });
            }

            if (!_repository.TryCreate(message.MessageId, message))
                return Conflict(new { Error = $"Message ID={message.MessageId} already exists" });

            return CreatedAtAction("Post", new CreatedResponse { Id = message.MessageId });
        }

        [HttpGet("{id}")]
        [ActionName("Get")]
        public IActionResult Get(string id)
        {
            var item = _repository.Get(id);
            if (item == null)
                return NotFound();

            return Ok(item);
        }

        [HttpDelete("{id}")]
        [ActionName("Delete")]
        public IActionResult Delete(string id)
        {
            if (!_repository.Delete(id))
                return NotFound();

            return NoContent();
        }
    }
}
