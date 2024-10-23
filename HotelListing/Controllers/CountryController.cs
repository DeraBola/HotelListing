using AutoMapper;
using HotelListing.core.IRepository;
using HotelListing.core.Models;
using HotelListing.Core.DTOs;
using HotelListing.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CountryController : ControllerBase
	{
		private readonly IUnitOfWork? _unitOfWork;
		private readonly ILogger<CountryController> _logger;
		private readonly IMapper _mapper;

		public CountryController(IUnitOfWork unitOfWork, ILogger<CountryController> logger, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
		}


		// Gets all countries
		//[ResponseCache(CacheProfileName = "120secondsDuration")]
		[HttpGet("all")]
		public async Task<IActionResult> GetCountries()
		{
			try
			{
				var countries = await _unitOfWork.Countries.GetAllAsync();
				var results = _mapper.Map<IList<CountryDTO>>(countries);
				// Set response cache only if needed
				// Response.Cache.SetCacheability(HttpCacheability.Public);
				// Response.Cache.SetExpires(DateTime.UtcNow.AddSeconds(60));
				return Ok(results);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, $"Something went wrong in the {nameof(GetCountries)}");
				return StatusCode(500, "Internal Server Error. Please try again later");
			}
		}

		// Gets paginated countries
		[HttpGet("paginated")]
		public async Task<IActionResult> GetPaginatedCountry([FromQuery] RequestParams requestParams)
		{
			var result = await _unitOfWork.Countries.GetAllPaginated(requestParams);
			return Ok(result);
		}

		// Gets a country by ID
		[HttpGet("{id:int}", Name = "GetCountry")]
		public async Task<IActionResult> GetCountry(int id)
		{
			if (id == 0)
			{
				_logger.LogError($"Invalid ID: {id}. The ID must be greater than 0.");
				return BadRequest("Invalid ID. The ID must be greater than 0.");
			}

			var country = await _unitOfWork.Countries.Get(q => q.Id == id, new List<string> { "Hotels" });

			if (country == null)
			{
				_logger.LogError($"Country with ID: {id} was not found.");
				return NotFound($"Country with ID: {id} was not found.");
			}
			var result = _mapper.Map<CountryDTO>(country);
			return Ok(result);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> CreateCountry([FromBody] CreateCountryDTO countryCreateDTO)
		{

			if (!ModelState.IsValid)
			{
				_logger.LogError($"Invalid POST attempt in {nameof(CreateCountry)}");
				return BadRequest(ModelState);
			}

			var country = _mapper.Map<Country>(countryCreateDTO);
			await _unitOfWork.Countries.Insert(country);
			await _unitOfWork.Save();
			return CreatedAtRoute("GetCountry", new { id = country.Id }, country);
		}

		[HttpPut]
		public async Task<IActionResult> UpdateCountry(int id, [FromBody] UpdateCountryDTO updateCountry)
		{
			if (!ModelState.IsValid || id < 1)
			{
				_logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateCountry)}");
				return BadRequest(ModelState);
			}
			try
			{
				var country = await _unitOfWork.Countries.Get(q => q.Id == id);
				if (country == null)
				{
					_logger.LogError($"Hotel with ID {id} not found for UPDATE in {nameof(UpdateCountry)}");
					return NotFound($"Hotel with ID {id} not found.");
				}
				_mapper.Map(updateCountry, country);
				_unitOfWork.Countries.Update(country);
				await _unitOfWork.Save();
				return NoContent();
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, $"Something went wrong in the {nameof(UpdateCountry)}");
				return StatusCode(500, "Internal Server Error. Please try again later");
			}
		}

		[HttpDelete("{id:int}", Name = "DeleteCountry")]
		public async Task<IActionResult> DeleteCountry(int id)
		{

			if (id < 1)
			{
				_logger.LogError($"Invalid Delete attempt in {nameof(DeleteCountry)}");
				return BadRequest(ModelState);
			}
			var country = await _unitOfWork.Countries.Get(q => q.Id == id);

			if (country == null)
			{
				_logger.LogError($"Hotel with ID {id} not found for Delete in {nameof(DeleteCountry)}");
				return NotFound($"Hotel with ID {id} not found.");
			}
			await _unitOfWork.Countries.Delete(id);
			await _unitOfWork.Save();
			return NoContent();
		}
	}
}
