using System.ComponentModel.DataAnnotations;

namespace HotelListing.Models
{

	public class LoginUserDTO
	{
		[Required]
		[DataType(DataType.EmailAddress)]
		public string? Email { get; set; }

		[Required]
		[StringLength(15, ErrorMessage = "Your password is limited to {2} t0 {1} characters", MinimumLength = 8)]
		public string? Password { get; set; }
	}
	public class UserDTO : LoginUserDTO
	{
		public string ? FirstName { get; set; }	
		public string ? LastName { get; set; }

		[DataType(DataType.PhoneNumber)]
		public string? PhoneNumber { get; set; }

		public ICollection<string> Roles { get; set; }
	}
}
