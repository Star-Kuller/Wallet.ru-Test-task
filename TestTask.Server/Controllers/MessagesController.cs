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
        IMessagesRepository repository) : ControllerBase
    {
        [HttpGet("{from:datetime}-{to:datetime}")]
        public IEnumerable<Message> GetMessages(DateTime? from, DateTime? to)
        {
            //TODO Добавить логику
            throw new NotImplementedException();
        }
        
        [HttpPost]
        public IActionResult GetMessage([FromBody]MessageDto message)
        {
            //TODO Добавить логику
            throw new NotImplementedException();
        }
    }
}
