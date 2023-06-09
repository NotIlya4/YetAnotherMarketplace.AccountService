﻿using Api.UserController.Helpers;
using Api.UserController.Views;
using Domain.Entities;
using Domain.Primitives;
using Infrastructure.JwtTokenPairService;
using Infrastructure.UserService;
using Infrastructure.ValidJwtTokenSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.UserController;

[ApiController]
[Route("users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ViewMapper _mapper;

    public UsersController(IUserService userService, ViewMapper mapper)
    {
        _userService = userService;
        _mapper = mapper;
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult<JwtTokenPairView>> Register(RegisterCredentialsView registerCredentialsView)
    {
        JwtTokenPair jwtTokenPair = await _userService.Register(_mapper.MapRegisterCredentials(registerCredentialsView));
        JwtTokenPairView responseJwtTokenPairView = _mapper.MapJwtTokenPair(jwtTokenPair);
        return Ok(responseJwtTokenPairView);
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<JwtTokenPairView>> Login(LoginCredentialsView loginCredentialsView)
    {
        JwtTokenPair jwtTokenPair = await _userService.Login(_mapper.MapLoginCredentials(loginCredentialsView));
        JwtTokenPairView responseJwtTokenPairView = _mapper.MapJwtTokenPair(jwtTokenPair);
        return Ok(responseJwtTokenPairView);
    }

    [HttpPost]
    [Route("updateJwtPair")]
    public async Task<ActionResult<JwtTokenPairView>> UpdateJwtPair(JwtTokenPairView jwtTokenPairView)
    {
        JwtTokenPair jwtTokenPair = await _userService.UpdateJwtPair(_mapper.MapJwtTokenPair(jwtTokenPairView));
        JwtTokenPairView responseJwtTokenPairView = _mapper.MapJwtTokenPair(jwtTokenPair);
        return Ok(responseJwtTokenPairView);
    }

    [HttpPost]
    [Route("logout")]
    public async Task<ActionResult> Logout(JwtTokenPairView jwtTokenPairView)
    {
        await _userService.Logout(_mapper.MapJwtTokenPair(jwtTokenPairView));
        return NoContent();
    }

    [HttpPost]
    [Route("logoutFromAllDevices")]
    public async Task<ActionResult> LogoutFromAllDevices(UserIdView userIdView)
    {
        await _userService.LogOutInAllEntries(new UserId(userIdView.UserId));
        return NoContent();
    }

    [HttpGet]
    [Route("id/{id}")]
    public async Task<ActionResult<UserView>> GetUserById(string id)
    {
        User address = await _userService.GetUserById(new UserId(id));
        UserView addressView = _mapper.MapUser(address);
        return Ok(addressView);
    }

    [HttpGet]
    [Route("name/{name}")]
    public async Task<ActionResult<UserView>> GetUserByName(string name)
    {
        User address = await _userService.GetUserByUsername(new Username(name));
        UserView addressView = _mapper.MapUser(address);
        return Ok(addressView);
    }

    [HttpGet]
    [Route("jwt/{jwt}")]
    public async Task<ActionResult<UserView>> GetUserByJwt(string jwt)
    {
        User address = await _userService.GetUserByJwtToken(new JwtToken(jwt));
        UserView addressView = _mapper.MapUser(address);
        return Ok(addressView);
    }

    [HttpGet]
    [Route("email/{email}/busy")]
    public async Task<ActionResult<bool>> IsEmailBusy(string email)
    {
        bool isBusy = await _userService.IsEmailBusy(new Email(email));
        return Ok(isBusy);
    }
    
    [HttpGet]
    [Route("username/{username}/busy")]
    public async Task<ActionResult<bool>> IsUsernameBusy(string username)
    {
        bool isBusy = await _userService.IsUsernameBusy(new Username(username));
        return Ok(isBusy);
    }
}