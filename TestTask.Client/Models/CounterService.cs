using TestTask.Client.Interfaces;

namespace TestTask.Client.Models;

/// <summary>
/// Класс, чтобы сохранять состояние счётчика при покидании страницы
/// </summary>
public class CounterService : ICounterService
{
    public int Count { get; private set; } = 1;
    
    public void Increment()
    {
        Count++;
    }

    public void Decrement()
    {
        Count--;
    }
}