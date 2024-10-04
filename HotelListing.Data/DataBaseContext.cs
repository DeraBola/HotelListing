using HotelListing.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelListing.Data
{
	public class DataBaseContext : IdentityDbContext<ApiUser>
	{
        public DataBaseContext(DbContextOptions options) : base(options)
        { }

		public DbSet<Country> Countries { get; set; }
        public DbSet<Hotel> Hotels { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfiguration(new RoleConfiguration());
			modelBuilder.ApplyConfiguration(new HotelConfiguration());
			modelBuilder.ApplyConfiguration(new CountryConfiguration());
		}
	}
}
