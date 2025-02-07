using System.Net;
using System.Net.Http.Json;
using TestTask.Client.DTOs;
using TestTask.Client.Interfaces;

namespace TestTask.Client.Models;

public class MessageService(HttpClient httpClient) : BaseHttpService, IMessageService
{

    public async Task AddMessageAsync(MessageDto message, CancellationToken token = default)
    {
        await ExecuteRequestAsync<string>(async () =>
            await httpClient.PostAsJsonAsync("/messages", message, token), token);
    }

    public async Task<IEnumerable<Message>> GetMessagesAsync(DateTime? from, DateTime? to, CancellationToken token = default)
    {
        var query = new Dictionary<string, string>();
        if (from.HasValue) query["from"] = from.Value.ToString("yyyy-MM-ddTHH:mm:ssK");
        if (to.HasValue) query["to"] = to.Value.ToString("yyyy-MM-ddTHH:mm:ssK");
        
        return await ExecuteRequestAsync<IEnumerable<Message>>(async () => 
            await httpClient.GetAsync($"/messages?{CreateQuery(query)}", token), token)
               ?? [];
    }
}