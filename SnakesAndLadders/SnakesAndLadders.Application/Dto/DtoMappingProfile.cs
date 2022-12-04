using AutoMapper;
using SnakesAndLadders.Data.Entities;

namespace SnakesAndLadders.Application.Dto
{
    public class DtoMappingProfile : Profile
    {
        public DtoMappingProfile() => CreateMap<User, UserDto>().ReverseMap();
    }
}