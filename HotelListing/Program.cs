using System.Linq.Expressions;
using HotelListing.Configurations;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Repository;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

try
{
	// Add services to the container.
	Log.Logger = new LoggerConfiguration()
		.MinimumLevel.Debug()
		.WriteTo.File("log/hotelListingLogs.txt", rollingInterval: RollingInterval.Day)
		.CreateLogger();

	builder.Services.AddAutoMapper(typeof(MapperInitializer));

	builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();

	// Add DbContext for SQL connection
	builder.Services.AddDbContext<DataBaseContext>(option =>
	{
		option.UseSqlServer(builder.Configuration.GetConnectionString("sqlConnection"));
	});

	builder.Services.AddControllers().AddNewtonsoftJson(op =>
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

	builder.Host.UseSerilog();

	builder.Services.AddControllers();
	builder.Services.AddEndpointsApiExplorer();
	builder.Services.AddSwaggerGen();

	var app = builder.Build();

	// Configure the HTTP request pipeline.
	if (app.Environment.IsDevelopment())
	{
		app.UseSwagger();
		app.UseSwaggerUI();
	}

	app.UseHttpsRedirection();
	app.UseCors("AllowSpecificOrigins");
	app.UseAuthorization();
	app.MapControllers();

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

