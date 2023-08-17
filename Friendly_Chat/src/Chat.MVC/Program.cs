using Chat.Core.Entities;
using Chat.DataAccess.Contexts;
using Chat.DataAccess.Repository.Implementations;
using Chat.DataAccess.Repository.Interfaces;
using Chat.MVC.HelperServices;
using Chat.MVC.HelperServices.Implementations;
using Chat.MVC.HelperServices.Interfaces;
using Chat.MVC.Hubs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<AppDbContexts>(opt =>
{
	opt.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});


builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
{
	opt.Password.RequiredLength = 8;
	opt.Password.RequireDigit = false;
	opt.Password.RequireUppercase = false;
	opt.Password.RequireLowercase = false;
	opt.Password.RequireNonAlphanumeric = false;
	opt.User.RequireUniqueEmail = true;
	opt.Lockout.AllowedForNewUsers = true;
	opt.Lockout.MaxFailedAccessAttempts = 5;
	opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
}).AddEntityFrameworkStores<AppDbContexts>()
  .AddEntityFrameworkStores<AppDbContexts>()
  .AddDefaultTokenProviders();//for frogot passwod;


//add services and repository
builder.Services.AddScoped<IFriendshipRepository, FriendshipRepository>();
builder.Services.AddScoped<IRequestRepository, RequestRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();

builder.Services.AddSignalR();
builder.Services.AddSingleton<OnlineUsersService>();
builder.Services.AddScoped<IFileService,FileService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
	endpoints.MapHub<OnlineUsersHub>("/onlineusershub");
});

app.Run();
