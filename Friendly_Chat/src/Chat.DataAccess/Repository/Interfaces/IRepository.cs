using Chat.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Chat.DataAccess.Repository.Interfaces;

public interface IRepository<T> where T : class,IEntity,new()
{
	IQueryable<T> FindAll();
	IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression);
	Task<T?> FindByIdAsync(int id);
	Task CreateAsync(T entity);
	void Update(T entity);
	void Delete(T entity);
	Task SaveAsync();
	DbSet<T> Table { get; }
}
