﻿using System.ComponentModel.DataAnnotations;

namespace HotelListing.Core.DTOs
{
	public class CreateHotelDTO
	{
		[Required]
		[StringLength(maximumLength: 150, ErrorMessage = "Hotel Name Is Too Long")]
		public string? Name { get; set; }
		[Required]
		[StringLength(maximumLength: 150, ErrorMessage = "Address Is Too Long")]
		public string? Address { get; set; }
		[Required]
		[Range(1,5)]
		public double Rating { get; set; }
		[Required]
		public int CountryId { get; set; }
	}

	public class UpdateHotelDTO : CreateHotelDTO
	{

	}

	public class HotelDTO : CreateHotelDTO
	{
		public int Id { get; set; }
		public CountryDTO? Country { get; set; }
	}
}
