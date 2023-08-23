using Chat.Core.Entities;
using Chat.DataAccess.Contexts;
using Chat.DataAccess.Repository.Interfaces;

namespace Chat.DataAccess.Repository.Implementations;

public class PostRepository : Repository<Post>, IPostRepository
{
    public PostRepository(AppDbContexts context) : base(context)
    {
    }
}
