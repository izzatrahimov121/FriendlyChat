using Chat.Core.Entities;
using Chat.DataAccess.Contexts;
using Chat.DataAccess.Repository.Interfaces;

namespace Chat.DataAccess.Repository.Implementations;

public class RequestRepository : Repository<FollowingRequest>, IRequestRepository
{
    public RequestRepository(AppDbContexts context) : base(context)
    {
    }
}
