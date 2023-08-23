using Chat.Core.Entities;
using Chat.DataAccess.Contexts;
using Chat.DataAccess.Repository.Interfaces;

namespace Chat.DataAccess.Repository.Implementations;

public class LikeRepository : Repository<Like>, ILikeRepository
{
    public LikeRepository(AppDbContexts context) : base(context)
    {
    }
}
