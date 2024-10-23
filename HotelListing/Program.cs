using HealthChecks.UI.Client;
using HotelListing;
using HotelListing.core.Configurations;
using HotelListing.core.IRepository;
using HotelListing.core.Repository;
using HotelListing.core.Services;
using HotelListing.Data;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

try
{
	// Add services to the container.
	Log.Logger = new LoggerConfiguration()
		.MinimumLevel.Debug()
		.WriteTo.File("log/hotelListingLogs.txt", rollingInterval: RollingInterval.Day)
		.CreateLogger();

	//builder.Services.AddAutoMapper(typeof(MapperInitializer));
	//builder.Services.AddResponseCaching();
	builder.Services.ConfigureHttpCacheHeaders();
	builder.Services.AddMemoryCache();
	// builder.Services.ConfigureRateLimiting();
	// builder.Services.AddHttpContextAccessor();
	builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
	builder.Services.AddScoped<IAuthManager, AuthManager>();
	builder.Host.UseSerilog();
	builder.Services.AddControllers();
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.ConfigureIdentity();
	builder.Services.ConfigureJWT(builder.Configuration);
	builder.Services.ConfigureVersioning();
	builder.Services.ConfigureAutoMapper();
	builder.Services.AddHealthChecksUI(setup =>
	{
		setup.AddHealthCheckEndpoint("Health Checks Dashboard", "/healthcheck"); // This is where the name should be configured
	}).AddInMemoryStorage();


	var connectionString = builder.Configuration.GetConnectionString("sqlConnection");

	if (string.IsNullOrEmpty(connectionString))
	{
		throw new InvalidOperationException("SQL connection string is missing.");
	}

	builder.Services.AddHealthChecks().AddSqlServer(connectionString);

	// Add DbContext for SQL connection
	builder.Services.AddDbContext<DataBaseContext>(option =>
	{
		option.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection"));
	});

	//Added Catche service
	builder.Services.AddControllers(config =>
	{
		config.CacheProfiles.Add("120secondsDuration", new CacheProfile
		{
			Duration = 120
		});
	}).AddNewtonsoftJson(op =>
	op.SerializerSettings.ReferenceLoopHandling =
	Newtonsoft.Json.ReferenceLoopHandling.Ignore);

	// Add CORS policy
	builder.Services.AddCors(options =>
	{
		options.AddPolicy("AllowSpecificOrigins",
			policy =>
			{
				policy.AllowAnyOrigin()
					.AllowAnyHeader()
					.AllowAnyMethod();
			});
	});


	// Add SecurityDefinition and SecurityRequirement
	builder.Services.AddSwaggerGen(options =>
	{
		options.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelListing", Version = "v1" });

		// Adding JWT Bearer Token Authentication
		options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
		{
			Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
			Name = "Authorization",
			In = ParameterLocation.Header,
			Type = SecuritySchemeType.ApiKey,
			Scheme = "Bearer"
		});

		options.AddSecurityRequirement(new OpenApiSecurityRequirement
	{
		{
			new OpenApiSecurityScheme
			{
				Reference = new OpenApiReference
				{
					Type = ReferenceType.SecurityScheme,
					Id = "Bearer"
				},
				Scheme = "oauth2",
				Name = "Bearer",
				In = ParameterLocation.Header,
			},
			new List<string>()
		}
	});
	});

	var app = builder.Build();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}
	app.ConfigureExceptionHandler();
	app.UseHttpsRedirection();
	app.UseCors("AllowSpecificOrigins");
	app.UseResponseCaching();
	app.UseHttpCacheHeaders();
	// app.UseIpRateLimiting();
	app.UseAuthentication();
	app.UseAuthorization();
	app.MapControllers();
	app.MapHealthChecks("/healthcheck", new HealthCheckOptions
	{
		ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
	});

	app.MapHealthChecksUI(options =>
	{
		options.UIPath = "/healthchecks-ui"; // HealthCheck UI path
	});


	Log.Information("Starting up the application");
	app.Run();
}
catch (Exception ex)
{
	Log.Fatal(ex, "Application start-up failed");
}
finally
{
	Log.CloseAndFlush();
}

