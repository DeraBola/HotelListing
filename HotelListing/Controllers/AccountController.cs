using AutoMapper;
using HotelListing.Data;
using HotelListing.Models;
using HotelListing.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HotelListing.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AccountController : ControllerBase
	{
		private readonly IAuthManager _authManager;
		private readonly UserManager<ApiUser> _userManager;
		private readonly SignInManager<ApiUser> _signInManager;
		private readonly ILogger<AccountController> _logger;
		private readonly IMapper _mapper;
		public AccountController(UserManager<ApiUser> userManager, SignInManager<ApiUser> signInManager, IAuthManager authManager, ILogger<AccountController> logger, IMapper mapper)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			_authManager = authManager;
			_logger = logger;
			_mapper = mapper;
		}

		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
		{
			_logger.LogInformation($"Registration attempt for {userDTO.Email}");

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}
			try
			{
				var user = _mapper.Map<ApiUser>(userDTO);
				user.UserName = userDTO.Email;
				var result = await _userManager.CreateAsync(user, userDTO.Password);

				if (!result.Succeeded)
				{
					foreach (var error in result.Errors)
					{
						ModelState.AddModelError(error.Code, error.Description);
					}
					return BadRequest(ModelState);
				}
				// Add the user to roles if any roles are specified
				if (userDTO.Roles != null && userDTO.Roles.Any())
				{
					await _userManager.AddToRolesAsync(user, userDTO.Roles);
				}
				return Accepted();
			}
			catch (Exception ex)
			{
				_logger.LogInformation(ex, $"Something went wrong in the{nameof(Register)}");
				return Problem($"Something went wrong in the{nameof(Register)}", statusCode: 500);
			}
		}

		[HttpPost]
		[Route("login")]
		public async Task<IActionResult> Login([FromBody] LoginUserDTO userDTO)
		{
			_logger.LogInformation($"Login attempt for {userDTO.Email}");

			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			try
			{
				// Validate the user with the UserManager via AuthManager
				var isValidUser = await _authManager.ValidateUser(userDTO);
				if (!isValidUser)
				{
					_logger.LogWarning($"Login failed for {userDTO.Email}");
					return Unauthorized("Invalid credentials");
				}

				// Create the token if valid
				var token = await _authManager.CreateToken();

				return Ok(new { Token = token });
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, $"Something went wrong in the {nameof(Login)}");
				return Problem($"Something went wrong in the {nameof(Login)}", statusCode: 500);
			}
		}

	}
}
