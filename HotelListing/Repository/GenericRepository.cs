using System.Linq.Expressions;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using X.PagedList;
using X.PagedList.Mvc.Core;

namespace HotelListing.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{

		private readonly DataBaseContext _context;
		private readonly DbSet<T> _db;

		public GenericRepository(DataBaseContext context)
		{
			_context = context;
			_db = _context.Set<T>();
		}

		public async Task Delete(int id)
		{
			var entity = await _context.Set<T>().FindAsync(id);
			if (entity != null)
			{
				_context.Set<T>().Remove(entity);
			}
		}

		public void DeleteRange(IEnumerable<T> entities)
		{
			_db.RemoveRange(entities);
		}

		public async Task<T?> Get(Expression<Func<T, bool>> expression, List<string> includes = null)
		{
			IQueryable<T> query = _db;
			if (includes != null)
			{
				foreach (var includeProperty in includes)
				{
					query = query.Include(includeProperty);
				}
			}
			return await query.AsNoTracking().FirstOrDefaultAsync(expression);
		}

		/* public async Task<IPagedList<T>> GetPagedList(RequestParams requestParams, List<string>? includes = null)
		{
			IQueryable<T> query = _db;

			if (includes != null)
			{
				foreach (var includeProperty in includes)
				{
					query = query.Include(includeProperty);
				}
			}
			return await query.AsNoTracking().ToPagedListAsync(requestParams.pageNumber, requestParams.pageSize);

		}
		*/

		public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>>? expression = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<string>? includes = null)
		{
			IQueryable<T> query = _db;

			if (expression != null)
			{
				query = query.Where(expression);
			}
			if (includes != null)
			{
				foreach (var includeProperty in includes)
				{
					query = query.Include(includeProperty);
				}
			}
			if (orderBy != null)
			{
				query = orderBy(query);
			}

			return await query.AsNoTracking().ToListAsync();
		}

		public async Task<PaginationModel<T>> GetAllPaginated(RequestParams requestParams, List<string>? includes = null)
		{
			IQueryable<T> query = _db;

			if (includes != null)
			{
				foreach (var includeProperty in includes)
				{
					query = query.Include(includeProperty);
				}
			}

			// Get total item count
			var totalItems = await query.CountAsync();

			// Fetch the paginated items
			var items = await query
				.Skip((requestParams.pageNumber - 1) * requestParams.pageSize)
				.Take(requestParams.pageSize)
				.ToListAsync();

			return new PaginationModel<T>(items, totalItems, requestParams.pageNumber, requestParams.pageSize);
		}


		public async Task Insert(T entity)
		{
			await _db.AddAsync(entity);
		}

		public async Task InsertRange(IEnumerable<T> entities)
		{
			await _db.AddRangeAsync(entities);
		}

		public void Update(T entity)
		{
			_db.Attach(entity);
			_context.Entry(entity).State = EntityState.Modified;
		}
	}
}
