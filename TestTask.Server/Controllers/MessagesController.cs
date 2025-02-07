using Microsoft.AspNetCore.Mvc;
using TestTask.Server.DTOs;
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
        public async Task<IActionResult> GetMessages([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            if (from > to)
                return BadRequest("Начальная дата не может быть больше конечной");
            
            //TODO Добавить логику
            throw new NotImplementedException();
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody]MessageDto message)
        {
            if (message.Content.Length > 128)
                return BadRequest("Строка в сообщении превышает 128 символов");
            
            logger.LogInformation("Создано сообщение {Message}", message);
            var messageEntity = new Message()
            {
                OrderNumber = message.Number,
                Content = message.Content
            };
            await repository.AddMessageAsync(messageEntity);
            await websocketController.SendMessageAsync(messageEntity);
            return Created();
        }
    }
}
