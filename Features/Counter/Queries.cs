using MediatR;

namespace VerticalSlice.Features.Counter;

public static class Queries
{
    public class GetCount : IRequest<int>;

    public class GetCountHandler(Repository repo) : IRequestHandler<GetCount, int>
    {
        public Task<int> Handle(GetCount request, CancellationToken cancellationToken)
        {
            return Task.FromResult(repo.GetCount());
        }
    }
}