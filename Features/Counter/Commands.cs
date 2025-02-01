using MediatR;

namespace VerticalSlice.Features.Counter;

public static class Commands
{
    public class IncrementCounter : IRequest<int>;

    public class IncrementCounterHandler(Repository repo) : IRequestHandler<IncrementCounter, int>
    {
        public Task<int> Handle(IncrementCounter request, CancellationToken cancellationToken)
        {
            repo.Increment();
            return Task.FromResult(repo.GetCount());
        }
    }
}