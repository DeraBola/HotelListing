using HotelListing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using System.Net;
using Azure;
using HotelListing.Models;

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
	
		public static void ConfigureExceptionHandler(this IApplicationBuilder app)
		{
			app.UseExceptionHandler(error => {
				error.Run(async context => {
					context.Response.StatusCode = StatusCodes.Status500InternalServerError;
					context.Response.ContentType = "application/json";
					var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>();
					if (contextFeature != null)
					{
						Serilog.Log.Error($"Something went wrong in {contextFeature.Error}");
						await context.Response.WriteAsync(new Error
						{
							StatusCode =  context.Response.StatusCode,
							Message  = "Internal Server Error. Please try again later."

						}.ToString());
					}
				});
			});
		}
	
	}
}

