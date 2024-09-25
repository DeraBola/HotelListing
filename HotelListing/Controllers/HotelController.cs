﻿using AutoMapper;
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
	public class HotelController : ControllerBase
	{

		private readonly IUnitOfWork? _unitOfWork;
		private readonly ILogger<HotelController> _logger;
		private readonly IMapper _mapper;

        public HotelController(IUnitOfWork unitOfWork, ILogger<HotelController> logger, IMapper mapper)
        {
			_unitOfWork = unitOfWork;
			_logger = logger;
			_mapper = mapper;
		}

		[HttpGet]
		public async Task<IActionResult> GetHotels()
		{
			try
			{
				var hotels = await _unitOfWork.Hotels.GetAllAsync();
				var results = _mapper.Map<IList<HotelDTO>>(hotels);
				return Ok(results);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, $"Something went wrong in the {nameof(GetHotels)}");
				return StatusCode(500, "Internal Server Error. Please try again later");
			}
		}

		[HttpGet("{id:int}", Name = "GetHotel")]
		public async Task<IActionResult> GetHotel(int id)
		{
			try
			{
				var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id, new List<string> { "Hotel" });
				var result = _mapper.Map<HotelDTO>(hotel);
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger?.LogError(ex, $"Something went wrong in the {nameof(GetHotel)}");
				return StatusCode(500, "Internal Server Error. Please try again later");
			}
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> CreateHotel([FromBody] CreateHotelDTO hotelDTO)
		{

			if (!ModelState.IsValid)
			{
				_logger.LogError($"Invalid POST attempt in {nameof(CreateHotel)}");
				return BadRequest(ModelState);
			}

			try
			{
				var hotel =_mapper.Map<Hotel>(hotelDTO);
				await _unitOfWork.Hotels.Insert(hotel);
				await _unitOfWork.Save();
				return CreatedAtRoute("GetHotel", new {id =  hotel.Id}, hotel);

			}catch (Exception ex)
			{
				_logger?.LogError(ex, $"Something went wrong in the {nameof(CreateHotel)}");
				return StatusCode(500, "Internal Server Error. Please try again later");
			}
		}


		[HttpPut]
		public async Task<IActionResult> UpdateHotel(int id, [FromBody] UpdateHotelDTO updateoHotelDTO)
		{
			if(!ModelState.IsValid || id < 1)
			{
				_logger.LogError($"Invalid UPDATE attempt in {nameof(UpdateHotel)}");
				return BadRequest(ModelState);
			}
			try
			{
				var hotel = await _unitOfWork.Hotels.Get(q => q.Id == id);
				if(hotel == null)
				{
					_logger.LogError($"Hotel with ID {id} not found for UPDATE in {nameof(UpdateHotel)}");
					return NotFound($"Hotel with ID {id} not found.");
				}
				_mapper.Map(updateoHotelDTO, hotel);
				_unitOfWork.Hotels.Update(hotel);
				await _unitOfWork.Save();
				return NoContent();
			}
			catch(Exception ex)
			{
				_logger?.LogError(ex, $"Something went wrong in the {nameof(UpdateHotel)}");
				return StatusCode(500, "Internal Server Error. Please try again later");
			}
		}

	}
}
