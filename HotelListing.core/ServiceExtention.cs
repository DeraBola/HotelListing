using System.Reflection;
using System.Text;
using AspNetCoreRateLimit;
using HotelListing.core.Configurations;
using HotelListing.core.Models;
using HotelListing.Data;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

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
			app.UseExceptionHandler(error =>
			{
				error.Run(async context =>
				{
					context.Response.StatusCode = StatusCodes.Status500InternalServerError;
					context.Response.ContentType = "application/json";
					var contextFeature = context.Features.Get<IExceptionHandlerPathFeature>();
					if (contextFeature != null)
					{
						Serilog.Log.Error($"Something went wrong in {contextFeature.Error}");
						await context.Response.WriteAsync(new Error
						{
							StatusCode = context.Response.StatusCode,
							Message = "Internal Server Error. Please try again later."

						}.ToString());
					}
				});
			});
		}

		public static void ConfigureVersioning(this IServiceCollection services)
		{
			services.AddApiVersioning(opt =>
			{
				opt.ReportApiVersions = true;
				opt.AssumeDefaultVersionWhenUnspecified = true;
				opt.DefaultApiVersion = new ApiVersion(1, 0);
				//	opt.ApiVersionReader = new HeaderApiVersionReader("api-version");
			});
		}

		public static void ConfigureHttpCacheHeaders(this IServiceCollection services)
		{
			services.AddResponseCaching();
			services.AddHttpCacheHeaders(
				(expirationOpt) =>
				{
					expirationOpt.MaxAge = 120;
					expirationOpt.CacheLocation = CacheLocation.Private;
				},
				(validationOpt) =>
				{
					validationOpt.MustRevalidate = true;
				});
		}
		
		public static void ConfigureAutoMapper(this IServiceCollection services)
		{
			// services.AddAutoMapper(typeof(MapperInitializer));
			services.AddAutoMapper(Assembly.GetExecutingAssembly());
			
		}


		public static void ConfigureRateLimiting(this IServiceCollection services)
		{
			var rateLimitRules = new List<RateLimitRule>
			{
			   new RateLimitRule
	{
		Endpoint = "*",
		Limit = 1,
		Period = "5s"
	}
				};
			services.Configure<IpRateLimitOptions>(opt =>
			{
				opt.GeneralRules = rateLimitRules;
			});
			services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
			services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
			services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
		}

	}
}

