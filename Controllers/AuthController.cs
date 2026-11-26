using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskTracker.Api.DTOs;

namespace TaskTracker.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;

    public AuthController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterAccount(RegisterDTO dto)
    {
        var user = new IdentityUser{
            UserName = dto.UserEmail,
            Email = dto.UserEmail
        };

        var result = await _userManager.CreateAsync(user, dto.Password);

        if(!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok("Account Created");
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAccount(LoginDTO dto)
    {
        var account = await _userManager.FindByNameAsync(dto.UserEmail);

        if(account == null)
        {
            return Unauthorized("Invalid email or password");
        }

        bool passwordMatch = await _userManager.CheckPasswordAsync(account, dto.Password);

        if (!passwordMatch)
        {
            return Unauthorized("Invalid email or password");
        }

        return Ok("Credential verified");

    } 
}