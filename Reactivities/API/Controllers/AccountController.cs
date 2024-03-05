using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using API.DTOs;
using API.DTOs.Facebook;
using API.Services;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;
    private readonly TokenService _tokenService;
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public AccountController(UserManager<AppUser> userManager, TokenService tokenService, IConfiguration config)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _config = config;
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri("https://graph.facebook.com")
        };
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await _userManager.Users
            .Include(x => x.Photos)
            .FirstOrDefaultAsync(x => x.Email == loginDto.Email);

        if (user is null)
            return Unauthorized();

        var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);

        if (result)
        {
            await SetRefreshToken(user);
            return CreateUserObject(user);
        }

        return Unauthorized();
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
    {
        if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
            return BadRequest("Username is already taken");

        if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
            return BadRequest("Email is already taken");

        var user = new AppUser()
        {
            DisplayName = registerDto.DisplayName,
            Email = registerDto.Email,
            UserName = registerDto.Username
        };

        var result = await _userManager.CreateAsync(user, registerDto.Password);

        if (result.Succeeded)
        {
            await SetRefreshToken(user);
            return CreateUserObject(user);
        }

        return BadRequest(result.Errors);
    }

    [AllowAnonymous]
    [HttpPost("fblogin")]
    public async Task<ActionResult<UserDto>> FacebookLogin(string accessToken)
    {
        var fbVerifyKeys = _config["Facebook:AppId"] + "|" + _config["Facebook:AppSecret"];

        var verifyTokenResponse = await _httpClient
            .GetAsync($"debug_token?input_token={accessToken}&access_token={fbVerifyKeys}");

        if (!verifyTokenResponse.IsSuccessStatusCode)
            return Unauthorized();

        var fbUrl = $"me?access_token={accessToken}&fields=name,email,picture.width(100).height(100)";

        var fbInfo = await _httpClient.GetFromJsonAsync<FacebookDto>(fbUrl);

        var user = await _userManager.Users
            .Include(x => x.Photos)
            .FirstOrDefaultAsync(x => x.Email == fbInfo.Email);

        if (user != null)
            return CreateUserObject(user);

        user = new AppUser()
        {
            DisplayName = fbInfo.Name,
            Email = fbInfo.Email,
            UserName = fbInfo.Email,
            Photos = new List<Photo>()
            {
                new Photo()
                {
                    Id = "fb_" + fbInfo.Id,
                    Url = fbInfo.Picture.Data.Url,
                    IsMain = true
                }
            }
        };

        var result = await _userManager.CreateAsync(user);

        if (!result.Succeeded)
            return BadRequest("Problem creating user account");

        await SetRefreshToken(user);
        return CreateUserObject(user);
    }

    [Authorize]
    [HttpPost("refreshToken")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];
        var user = await _userManager.Users
            .Include(x => x.RefreshTokens)
            .Include(x => x.Photos)
            .FirstOrDefaultAsync(x => x.UserName == User.FindFirstValue(ClaimTypes.Name));

        if (user is null)
            return Unauthorized();

        var oldToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken);

        if (oldToken != null && !oldToken.IsActive)
            return Unauthorized();

        if (oldToken != null)
            oldToken.Revoked = DateTime.UtcNow;

        return CreateUserObject(user);
    }

    [HttpGet]
    public async Task<ActionResult<UserDto>> GetCurrentUser()
    {
        var user = await _userManager.Users
            .Include(x => x.Photos)
            .FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        await SetRefreshToken(user);
        return CreateUserObject(user);
    }

    private async Task SetRefreshToken(AppUser appUser)
    {
        var refreshToken = _tokenService.GenerateRefreshToken();

        appUser.RefreshTokens.Add(refreshToken);
        await _userManager.UpdateAsync(appUser);

        var cookieOptions = new CookieOptions()
        {
            HttpOnly = true,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken.Token, cookieOptions);
    }

    private UserDto CreateUserObject(AppUser user)
    {
        return new UserDto()
        {
            DisplayName = user.DisplayName,
            Image = user?.Photos?.FirstOrDefault(x => x.IsMain)?.Url,
            UserName = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
    }
}