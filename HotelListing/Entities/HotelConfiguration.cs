using HotelListing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Entities
{
	public class HotelConfiguration : IEntityTypeConfiguration<Hotel>
	{
		public void Configure(EntityTypeBuilder<Hotel> builder)
		{
			builder.HasData
				(
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
