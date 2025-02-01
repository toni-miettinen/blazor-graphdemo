using MediatR;

namespace VerticalSlice.Features.Counter;

public static class Commands
{
    public class IncrementCounter : IRequest<int>;

    public class IncrementCounterHandler(IRepository repo) : IRequestHandler<IncrementCounter, int>
    {
        public async Task<int> Handle(IncrementCounter request, CancellationToken cancellationToken)
        {
            await repo.Increment();
            return await repo.GetCount();
        }
    }
}