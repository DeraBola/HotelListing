using System.Linq.Expressions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

try
{
	// Add services to the container.
	Log.Logger = new LoggerConfiguration()
		.MinimumLevel.Debug()
		.WriteTo.File("log/hotelListingLogs.txt", rollingInterval: RollingInterval.Day)
		.CreateLogger();

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

