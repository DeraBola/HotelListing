using HotelListing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HotelListing
{
	public static class ServiceExtention
	{
		public static void ConfigureIdentity(this IServiceCollection services)
		{
			services.AddIdentity<ApiUser, IdentityRole>(options =>
			{
				options.User.RequireUniqueEmail = true;
			})
			.AddEntityFrameworkStores<DataBaseContext>()
			.AddDefaultTokenProviders();
		}

		public static void ConfigureJWT(this IServiceCollection services, IConfiguration Configuration)
		{
			var jwtSettings = Configuration.GetSection("Jwt");
			var Key = Environment.GetEnvironmentVariable("KEY");
			services.AddAuthentication(o =>
			{
				o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
			})
				.AddJwtBearer(o =>
				{
					o.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = Configuration["Jwt:Issuer"],
						ValidAudience = Configuration["Jwt:Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
					};
				});
			services.AddAuthorization();
		}
	}
}

