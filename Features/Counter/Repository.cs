namespace VerticalSlice.Features.Counter;

public interface IRepository
{
    Task Increment();
    Task<int> GetCount();
}
public class MockRepository : IRepository
{
    private int _counter = 0;

    public Task Increment() => Task.FromResult(++_counter);
    public Task<int> GetCount() => Task.FromResult(_counter);
}