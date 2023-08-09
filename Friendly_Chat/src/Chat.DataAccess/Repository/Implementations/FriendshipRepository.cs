using Chat.Core.Entities;
using Chat.DataAccess.Contexts;
using Chat.DataAccess.Repository.Interfaces;

namespace Chat.DataAccess.Repository.Implementations;

public class FriendshipRepository : Repository<Friendship>, IFriendshipRepository
{
    public FriendshipRepository(AppDbContexts context) : base(context)
    {
    }
}
