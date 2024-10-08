using HotelListing.Core.DTOs;

namespace HotelListing.core.Services
{
	public interface IAuthManager

	{
		Task<bool> ValidateUser(LoginUserDTO UserDTO);

		Task<string> CreateToken();
	}
}
