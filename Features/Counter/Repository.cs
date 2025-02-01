namespace VerticalSlice.Features.Counter;

public class Repository
{
    private int _counter = 0;

    public void Increment() => _counter++;
    public int GetCount() => _counter;
}