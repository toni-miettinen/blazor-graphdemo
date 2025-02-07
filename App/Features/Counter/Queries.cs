using MediatR;

namespace VerticalSlice.Features.Counter;

public static class Queries
{
    public class GetCount : IRequest<int>;

    public class GetCountHandler(IRepository repo) : IRequestHandler<GetCount, int>
    {
        public async Task<int> Handle(GetCount request, CancellationToken cancellationToken)
        {
            return await repo.GetCount();
        }
    }
}