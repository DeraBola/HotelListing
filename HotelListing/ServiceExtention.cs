using HotelListing.Data;
using Microsoft.AspNetCore.Identity;

namespace HotelListing
{
	public static class ServiceExtention
	{
		public static void ConfigureIdentity(this IServiceCollection services)
		{
			var builder = services.AddIdentityCore<ApiUser>(q => q.User.RequireUniqueEmail = true);

			builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), services);
			builder.AddEntityFrameworkStores<DataBaseContext>().AddDefaultTokenProviders();
		}
	}
}

