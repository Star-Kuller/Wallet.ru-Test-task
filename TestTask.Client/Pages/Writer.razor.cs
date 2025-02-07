using Microsoft.AspNetCore.Components;
using TestTask.Client.DTOs;
using TestTask.Client.Infrastructure.Exceptions;
using TestTask.Client.Interfaces;

namespace TestTask.Client.Pages;

public partial class Writer: ComponentBase
{
    private record Error(string Message);
    private string _userInput = string.Empty;
    private Error? _error = null;
    [Inject]
    private IMessageService _messageService { get; set; }
    [Inject]
    private ICounterService _counter { get; set; }

    private void UpdateInput(ChangeEventArgs e)
    {
        _userInput = e.Value?.ToString() ?? string.Empty;
    }

    private async Task Submit()
    {
        _error = null;
        var message = new MessageDto(_counter.Count, _userInput);
        Console.WriteLine($"Отправка сообщения: номер: {message.Number} текст: {message.Content}");
        try
        {
            _counter.Increment();
            await _messageService.AddMessageAsync(message);
            _userInput = string.Empty;
        }
        catch (BadRequestHttpException e)
        {
            _counter.Decrement();
            _error = new Error(e.Message);
        }
        catch (NoConnectHttpException e)
        {
            _counter.Decrement();
            _error = new Error("Нет соединения с сервером");
        }
        catch (HttpRequestException e)
        {
            _counter.Decrement();
            _error = new Error(e.Message);
        }
        catch (Exception e)
        {
            _counter.Decrement();
            Console.WriteLine(e);
            throw;
        }
    }
}