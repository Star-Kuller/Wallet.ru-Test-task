using Microsoft.AspNetCore.Components;
using TestTask.Client.Infrastructure;
using TestTask.Client.Interfaces;
using TestTask.Client.Models;

namespace TestTask.Client.Pages;

public partial class RangeReader : ComponentBase
{
    private List<Message> _receivedMessages = [];
    private Error? _error = null;
    private string _startDate;
    private string _endDate;
    [Inject]
    private IMessageService MessageService { get; set; }

    private async Task FetchMessages()
    {
        _receivedMessages.Clear();
        _error = null;
        if (!DateTime.TryParse(_startDate, out var from))
        {
            _error = new Error("Не получилось распознать начальную дату");
            return;
        }
        if (!DateTime.TryParse(_startDate, out var to))
        {
            _error = new Error("Не получилось распознать начальную дату");
            return;
        }
        _receivedMessages = (await MessageService.GetMessagesAsync(from, to)).ToList();
    }
}