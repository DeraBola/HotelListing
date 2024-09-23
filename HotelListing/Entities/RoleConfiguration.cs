using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HotelListing.Entities
{
	public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
	{
		public void Configure(EntityTypeBuilder<IdentityRole> builder)
		{
			builder.HasData
				(
				new IdentityRole
				{
					Name = "user",
					NormalizedName = "USER",
				},
				new IdentityRole
				{
					Name = "Administrator",
					NormalizedName = "ADMINISTRATOR",
				}
				);
		}
	}
}
