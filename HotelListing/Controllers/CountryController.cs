using AutoMapper;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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

		[HttpGet]
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

		[HttpGet("{id:int}", Name = "GetCountry")]
		public async Task<IActionResult> GetCountry(int id)
		{
			try
			{
				var country = await _unitOfWork.Countries.Get(q => q.Id == id, new List<string> { "Hotels" });
				var result = _mapper.Map<CountryDTO>(country);
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, $"Something went wrong in the {nameof(GetCountry)}");
				return StatusCode(500, "Internal Server Error. Please try again later");
			}
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public async Task<IActionResult> CreateCountry([FromBody] CountryDTO countryDTO)
		{

			if (!ModelState.IsValid)
			{
				_logger.LogError($"Invalid POST attempt in {nameof(CreateCountry)}");
				return BadRequest(ModelState);
			}

			try
			{
				var country = _mapper.Map<Country>(countryDTO);
				await _unitOfWork.Countries.Insert(country);
				await _unitOfWork.Save();
				return CreatedAtRoute("GetCountry", new { id = country.Id }, country);

			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, $"Something went wrong in the {nameof(CreateCountry)}");
				return StatusCode(500, "Internal Server Error. Please try again later");
			}
		}
	}
}
