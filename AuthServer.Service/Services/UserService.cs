﻿using AuthServer.Core.Dtos;
using AuthServer.Core.Models;
using AuthServer.Core.Services;
using Microsoft.AspNetCore.Identity;
using SharedLibrary.Dtos;

namespace AuthServer.Service.Services
{
    public class UserService : IUserService
    {

        private readonly UserManager<UserApp> _userManager;

        public UserService(UserManager<UserApp> userManager)
        {
            _userManager = userManager;
        }


        public async Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            var user = new UserApp { Email= createUserDto.Email, UserName=createUserDto.UserName };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if(!result.Succeeded)
            {
                var errors = result.Errors.Select(x=>x.Description).ToList();

                return Response<UserAppDto>.Fail(new ErrorDto(errors, true), 400);
            }

            return Response<UserAppDto>.Success(ObjectMapper.CreateMapper.Map<UserAppDto>(user), 200);
        }
        // user name async
        public async Task<Response<UserAppDto>> GetUserByNameAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null) return Response<UserAppDto>.Fail("User not found", 400, true);

            return Response<UserAppDto>.Success(ObjectMapper.CreateMapper.Map<UserAppDto>(user), 200);
        }
    }
}
