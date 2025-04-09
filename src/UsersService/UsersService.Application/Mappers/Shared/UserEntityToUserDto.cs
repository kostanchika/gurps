using AutoMapper;
using UsersService.Application.DTOs.Shared;
using UsersService.Domain.Entities;

namespace UsersService.Application.Mappers.Shared
{
    public class UserEntityToUserDto : Profile
    {
        public UserEntityToUserDto()
        {
            CreateMap<UserEntity, UserDto>();
        }
    }
}
