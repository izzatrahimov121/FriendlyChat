using Chat.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chat.DataAccess.Contexts;

public class AppDbContexts : IdentityDbContext
{
    public AppDbContexts(DbContextOptions options): base(options)
    { 
    }

    DbSet<AppUser> AppUser { get; set; }
    DbSet<Friendship> Friendships { get; set; }
    DbSet<FollowingRequest> FollowingRequests { get; set;}
}
