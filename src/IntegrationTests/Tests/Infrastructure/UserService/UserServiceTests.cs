﻿using Infrastructure.EntityFramework;
using Infrastructure.JwtTokenPairService;
using Infrastructure.RefreshTokenService;
using Infrastructure.RefreshTokenService.Models;
using Infrastructure.UserService;
using Infrastructure.UserService.Models;
using Infrastructure.ValidJwtTokenSystem.Models;
using IntegrationTests.Setup;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace IntegrationTests.Tests.Infrastructure.UserService;

[Collection(nameof(AppFixture))]
public class UserServiceTests : IDisposable
{
    private readonly IUserService _service;
    private readonly IServiceScope _scope;
    private readonly SqlDbHelper _sqlDbHelper;
    private readonly RedisHelper _redisHelper;
    private readonly IRefreshTokenService _refreshTokenService;

    public UserServiceTests(AppFixture fixture)
    {
        _scope = fixture.Fixture.Services.CreateScope();
        _service = _scope.ServiceProvider.GetRequiredService<IUserService>();
        _sqlDbHelper = new SqlDbHelper(_scope.ServiceProvider.GetRequiredService<AppDbContext>());
        _redisHelper = new RedisHelper(_scope.ServiceProvider.GetRequiredService<IDatabase>());
        _refreshTokenService = _scope.ServiceProvider.GetRequiredService<IRefreshTokenService>();
    }

    [Fact]
    public async Task UpdateJwtPair_Update_DeletePreviousRefreshToken()
    {
        JwtTokenPair pair = await Login();
        await _service.UpdateJwtPair(pair);
        
        Assert.False(await _refreshTokenService.Contains(_sqlDbHelper.SampleUser.Id, pair.RefreshToken));

        await Reload();
    }

    [Fact]
    public async Task GetUserById_Get_UserWithSpecifiedId()
    {
        User result = await _service.GetUserById(_sqlDbHelper.SampleUser.Id);
        
        Assert.Equal(_sqlDbHelper.SampleUser, result);
    }
    
    [Fact]
    public async Task GetUserByEmail_Get_UserWithSpecifiedEmail()
    {
        User result = await _service.GetUserByEmail(_sqlDbHelper.SampleUser.Email);
        
        Assert.Equal(_sqlDbHelper.SampleUser, result);
    }
    
    [Fact]
    public async Task GetUserByUsername_Get_UserWithSpecifiedUsername()
    {
        User result = await _service.GetUserByUsername(_sqlDbHelper.SampleUser.Username);
        
        Assert.Equal(_sqlDbHelper.SampleUser, result);
    }
    
    [Fact]
    public async Task GetUserByJwt_Get_UserWithIdFromJwt()
    {
        JwtTokenPair pair = await Login();
        
        User result = await _service.GetUserByJwtToken(pair.JwtToken);
        
        Assert.Equal(_sqlDbHelper.SampleUser, result);
    }

    private async Task<JwtTokenPair> Login()
    {
        return await _service.Login(new LoginCredentials(_sqlDbHelper.SampleUser.Email, _sqlDbHelper.SampleUserPassword));
    }

    private async Task Reload()
    {
        await _sqlDbHelper.Reload();
        await _redisHelper.Reload();
    }
        
    public void Dispose()
    {
        _scope.Dispose();
    }
}