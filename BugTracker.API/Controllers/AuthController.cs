using BugTracker.API.Entities;
using BugTracker.Shared.Constants;
using BugTracker.Shared.Dtos;
using BugTracker.Shared.Helper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BugTracker.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly SignInManager<AuthUser> _signManager;
    private readonly IConfiguration _config;

    public AuthController(UserManager<AuthUser> userManager, SignInManager<AuthUser> signManager, IConfiguration config)
    {
        _userManager = userManager;
        _signManager = signManager;
        _config = config;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] AuthUserCreateUpdateDto authUserDto)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var userExist = await _userManager.FindByEmailAsync(authUserDto.Email);
                if (userExist != null) return Ok(ApiResponseHandler<string>.ErrorResponse("Registration failed. Email already used."));

                AuthUser user = new AuthUser
                {
                    Name = authUserDto.Name,
                    UserName = authUserDto.Email,
                    Email = authUserDto.Email,
                    EmailConfirmed = true
                };

                var authUserResp = await _userManager.CreateAsync(user, authUserDto.Password);

                if (authUserDto.IsUser)
                {
                    await _userManager.AddToRoleAsync(user, DefaultRoleConstant.User);
                }
                await _userManager.AddToRoleAsync(user, DefaultRoleConstant.Developer);

                if (authUserResp.Succeeded)
                {
                    return Ok(ApiResponseHandler<string>.SuccessResponse("User has been register successfully."));
                }
                return Ok(ApiResponseHandler<string>.ErrorResponse("Invalid registration."));
            }
            else
            {
                return BadRequest(ApiResponseHandler<string>.ErrorResponse("Provided info is invalid."));
            }
        }
        catch (Exception)
        {
            return BadRequest(ApiResponseHandler<string>.ErrorResponse("Provided info is invalid."));
        }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);
                if (user != null)
                {
                    var isValidPassword = await _signManager.CheckPasswordSignInAsync(user, loginDto.Password, true);

                    if (isValidPassword.Succeeded)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        var tokenKey = Encoding.UTF8.GetBytes(_config["JWT:Key"]);

                        var claims = new List<Claim>
                            {
                                new Claim("UserId", user.Id),
                                new Claim(ClaimTypes.Name, user.UserName),
                                new Claim(ClaimTypes.Email, user.Email)
                            };
                        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

                        var tokenDescription = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(claims),
                            Expires = DateTime.UtcNow.AddMinutes(5),
                            Issuer = _config["JWT:Issuer"],
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
                        };

                        //create token handler
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var token = tokenHandler.CreateToken(tokenDescription);

                        var tokenDto = new TokenDto { TokenType = "Bearer", AccessToken = tokenHandler.WriteToken(token) };
                        return Ok(ApiResponseHandler<TokenDto>.SuccessResponse(tokenDto));
                    }
                    return Ok(ApiResponseHandler<TokenDto>.ErrorResponse("Invalid Password."));
                }
                return Ok(ApiResponseHandler<TokenDto>.ErrorResponse("Invalid User."));
            }
            else
            {
                return BadRequest(ApiResponseHandler<TokenDto>.ErrorResponse("Provided info is incorrect."));
            }
        }
        catch (Exception)
        {

            throw;
        }
    }
}
