using AutoMapper;
using HotelListing.Core.DTOs;
using HotelListing.Data;

namespace HotelListing.core.Configurations
{
	public class MapperInitializer : Profile
	{
		public MapperInitializer()
		{
			CreateMap<Country, CountryDTO>()
			 .ForMember(dest => dest.Hotels, opt => opt.MapFrom(src => src.Hotels)) // Map Hotels
			 .ReverseMap();
			CreateMap<Country, CreateCountryDTO>().ReverseMap();

			CreateMap<Hotel, HotelDTO>()
	.ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country)) // Include Country mapping
	.ReverseMap();
			CreateMap<Hotel, CreateHotelDTO>().ReverseMap();
			CreateMap<ApiUser, UserDTO>().ReverseMap();
		}
	}
}
