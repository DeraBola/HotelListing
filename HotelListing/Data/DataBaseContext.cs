using Microsoft.EntityFrameworkCore;

namespace HotelListing.Data
{
	public class DataBaseContext : DbContext
	{
        public DataBaseContext(DbContextOptions options) : base(options)
        { }

		public DbSet<Country> Countries { get; set; }
        public DbSet<Hotel> Hotels { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Country>().HasData(
				new Country
				{
					Id = 1,
					Name = "Nigeria",
					ShortName = "NG"
				},
				new Country
				{
					Id = 2,
					Name = "Jamica",
					ShortName = "JM"
				},
				new Country
				{
					Id = 3,
					Name = "United States",
					ShortName = "US"
				}
				);

			modelBuilder.Entity<Hotel>().HasData(
				new Hotel
				{
					Id = 1,
					Name = "Sheralton",
					Address = "Vi",
					CountryId = 1,
					Rating = 4.5
				},
				new Hotel
				{
					Id = 2,
					Name = "Sandals resort and spa",
					Address = "Negril",
					CountryId = 2,
					Rating = 4.5
				},
				new Hotel
				{
					Id = 3,
					Name = "Ovation",
					Address = "New york",
					CountryId = 3,
					Rating = 4.5
				}
				);
		}
	}
}
