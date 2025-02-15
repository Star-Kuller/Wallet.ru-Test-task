using Microsoft.AspNetCore.Components;
using TestTask.Client.Infrastructure;
using TestTask.Client.Infrastructure.Exceptions;
using TestTask.Client.Interfaces;
using TestTask.Client.Models;

namespace TestTask.Client.Pages;

public partial class RangeReader : ComponentBase
{
    private List<Message> _receivedMessages = [];
    private Error? _error = null;
    private DateTime _fromDate = DateTime.Now;
    private DateTime _toDate = DateTime.Now;
    [Inject]
    private IMessageService MessageService { get; set; }

    private async Task FetchMessages()
    {
        _receivedMessages.Clear();
        _error = null;
        var from = _fromDate.Date;
        var to = _toDate.Date.Add(new TimeSpan(00, 23, 59, 59, 999));
        try
        {
            _receivedMessages = (await MessageService.GetMessagesAsync(from, to)).ToList();
        }
        catch (NoConnectHttpException e)
        {
            _error = new Error("Нет соединения с сервером");
        }
        catch (HttpRequestException e)
        {
            _error = new Error(e.Message);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}