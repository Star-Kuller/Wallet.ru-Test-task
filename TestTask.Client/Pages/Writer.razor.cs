using Microsoft.AspNetCore.Components;
using TestTask.Client.DTOs;
using TestTask.Client.Infrastructure;
using TestTask.Client.Infrastructure.Exceptions;
using TestTask.Client.Interfaces;

namespace TestTask.Client.Pages;

public partial class Writer: ComponentBase
{
    private const int Maxlength = 128;
    private string _userInput = string.Empty;
    private Error? _error = null;
    [Inject]
    private IMessageService MessageService { get; set; }
    [Inject]
    private ICounterService Counter { get; set; }

    private void UpdateInput(ChangeEventArgs e)
    {
        _userInput = e.Value?.ToString() ?? string.Empty;
    }

    private async Task Submit()
    {
        _error = null;
        var message = new MessageDto(Counter.Count, _userInput);
        Console.WriteLine($"Отправка сообщения: номер: {message.Number} текст: {message.Content}");
        try
        {
            Counter.Increment();
            await MessageService.AddMessageAsync(message);
            _userInput = string.Empty;
        }
        catch (BadRequestHttpException e)
        {
            Counter.Decrement();
            _error = new Error(e.Message);
        }
        catch (NoConnectHttpException e)
        {
            Counter.Decrement();
            _error = new Error("Нет соединения с сервером");
        }
        catch (HttpRequestException e)
        {
            Counter.Decrement();
            _error = new Error(e.Message);
        }
        catch (Exception e)
        {
            Counter.Decrement();
            Console.WriteLine(e);
            throw;
        }
    }
}