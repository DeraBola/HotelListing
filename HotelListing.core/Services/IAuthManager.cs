using HotelListing.Models;

namespace HotelListing.core.Services
{
	public interface IAuthManager

	{
		Task<bool> ValidateUser(LoginUserDTO UserDTO);

		Task<string> CreateToken();
	}
}
