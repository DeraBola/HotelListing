using System.Linq.Expressions;
using HotelListing.core.Models;



namespace HotelListing.core.IRepository
{
	public interface IGenericRepository<T> where T : class
	{
		Task<PaginationModel<T>> GetAllPaginated(RequestParams requestParams, List<string>? includes = null);
		Task<IList<T>> GetAllAsync(
			Expression<Func<T, bool>>? expression = null,
			Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
			List<string>? includes = null
			);

		Task<T> Get(Expression<Func<T, bool>> expression, List<string>? includes = null);
		Task Insert(T entity);

		Task InsertRange(IEnumerable<T> entities);

		Task Delete(int id);
		void DeleteRange(IEnumerable<T> entities);

		void Update(T entity);
	}
}
