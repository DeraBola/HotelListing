using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelListing.Core.DTOs;
using HotelListing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace HotelListing.core.Services
{
	public class AuthManager : IAuthManager
	{
		private readonly UserManager<ApiUser>? _userManager;
		private readonly IConfiguration? _configuration;
		private readonly ILogger<AuthManager> _logger;
		private ApiUser? _user;

		public AuthManager(UserManager<ApiUser> userManager, ILogger<AuthManager> logger, IConfiguration configuration)
		{
			_userManager = userManager;
			_configuration = configuration;
			_logger = logger;
		}
		public async Task<string> CreateToken()
		{

			var signingCredentials = GetSigninCredentials();
			var claims = await GetClaims();
			var token = GenerateTokenOptions(signingCredentials, claims);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, List<Claim> claims)
		{
			var jwtSettings = _configuration.GetSection("Jwt");
			var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings.GetSection("lifetime").Value));
			var token = new JwtSecurityToken(
				issuer: jwtSettings["Issuer"],
				audience: jwtSettings["Audience"],
				claims: claims,
				expires: expiration,
				signingCredentials: signingCredentials
				);
			return token;
		}

		private async Task<List<Claim>> GetClaims()
		{
			if (_user == null) throw new InvalidOperationException("User is not set");

			var claims = new List<Claim>
	{
		new Claim(ClaimTypes.Name, _user.UserName)
	};

			var roles = await _userManager.GetRolesAsync(_user);
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			return claims;
		}

		private SigningCredentials GetSigninCredentials()
		{
			var key = _configuration["Jwt:Key"];
			if (key == null || key.Length < 32)
			{
				_logger.LogError("JWT key is too short or missing");
				throw new InvalidOperationException("JWT key must be at least 256 bits (32 characters) long.");
			}
			if (string.IsNullOrEmpty(key))
				throw new InvalidOperationException("JWT Key is missing.");

			var secret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
			return new SigningCredentials(secret, SecurityAlgorithms.HmacSha256);
		}

		public async Task<bool> ValidateUser(LoginUserDTO UserDTO)
		{
			// Assign to the class-level _user variable instead of declaring a new local variable

			_user = await _userManager.FindByNameAsync(UserDTO.Email);
			return (_user != null && await _userManager.CheckPasswordAsync(_user, UserDTO.Password));
		}
	}
}
