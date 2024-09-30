using AutoMapper;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
	[ApiVersion("2.0")]
	[Route("api/v{version:apiVersion}/country")]
	[ApiController]
	public class CountryV2Controller : ControllerBase
	{
		private readonly IUnitOfWork? _unitOfWork;
		private readonly ILogger<CountryV2Controller> _logger;
		private readonly IMapper _mapper;

		public CountryV2Controller(IUnitOfWork unitOfWork, ILogger<CountryV2Controller> logger, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
		}

		// Gets all countries
		[HttpGet("all")]
		public async Task<IActionResult> GetCountries()
		{
			try
			{
				var countries = await _unitOfWork.Countries.GetAllAsync();
				var results = _mapper.Map<IList<CountryDTO>>(countries);
				return Ok(results);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, $"Something went wrong in the {nameof(GetCountries)}");
				return StatusCode(500, "Internal Server Error. Please try again later");
			}
		}
	}
}
