using HotelListing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Entities
{
	public class CountryConfiguration : IEntityTypeConfiguration<Country>
	{
		public void Configure(EntityTypeBuilder<Country> builder)
		{
			builder.HasData
				(
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
		}
	}
}
