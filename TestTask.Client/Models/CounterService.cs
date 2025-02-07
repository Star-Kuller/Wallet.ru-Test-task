using TestTask.Client.Interfaces;

namespace TestTask.Client.Models;

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