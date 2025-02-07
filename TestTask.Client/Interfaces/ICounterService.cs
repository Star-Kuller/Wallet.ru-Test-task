namespace TestTask.Client.Interfaces;

public interface ICounterService
{
    int Count { get; }
    void Increment();
    void Decrement();
}