using HotelListing.core.IRepository;
using HotelListing.Data;

namespace HotelListing.core.Repository
{
    public class UnitOfWork : IUnitOfWork
	{
		private readonly DataBaseContext _context;
		private IGenericRepository<Country> ? _countryRepository;
		private IGenericRepository<Hotel> ? _hotelRepository;

		public UnitOfWork(DataBaseContext context)
		{
			_context = context;
		}
		public IGenericRepository<Country> Countries => _countryRepository ??= new GenericRepository<Country>(_context);

		public IGenericRepository<Hotel> Hotels => _hotelRepository ??= new GenericRepository<Hotel>(_context);

		public void Dispose()
		{
			_context.Dispose();
			GC.SuppressFinalize(this);
		}

		public async Task Save()
		{
			await _context.SaveChangesAsync();
		}
	}
}
