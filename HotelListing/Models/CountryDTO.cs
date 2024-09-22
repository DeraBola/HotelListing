using System.ComponentModel.DataAnnotations;
using HotelListing.Data;

namespace HotelListing.Models
{

	public class CreateCountryDTO
	{
		[Required]
		[StringLength(maximumLength: 50, ErrorMessage = "Country Name Is Too Long")]
		public string? Name { get; set; }
		[Required]
		[StringLength(maximumLength: 5, ErrorMessage = "Short Country Name Is Too Long")]
		public string? ShortName { get; set; }
	}
	public class CountryDTO : CreateCountryDTO
	{
		public int Id { get; set; }

		public IList<HotelDTO>? Hotels { get; set; }
	}
}
