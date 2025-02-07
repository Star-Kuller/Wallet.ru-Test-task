using Microsoft.AspNetCore.Mvc;
using TestTask.Server.DTOs;
using TestTask.Server.Exceptions;
using TestTask.Server.Interfaces;
using TestTask.Server.Models;

namespace TestTask.Server.Controllers
{
    [ApiController]
    [Route("messages")]
    public class MessagesController(
        ILogger<MessagesController> logger,
        IMessagesRepository repository,
        IMessagesWebsocketController websocketController) : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<Message>> GetMessages([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            if (from > to)
                throw new ValidationException("Начальная дата не может быть больше конечной");
            var messages = await repository.GetMessagesAsync(from, to);
            return messages;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody]MessageDto message)
        {
            if (message.Content.Length > 128)
                throw new ValidationException("Строка в сообщении превышает 128 символов");
            
            var messageEntity = new Message()
            {
                OrderNumber = message.Number,
                Content = message.Content
            };
            await repository.AddMessageAsync(messageEntity);
            await websocketController.BroadcastMessageAsync(messageEntity);
            return Created();
        }
    }
}
