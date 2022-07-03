using InternalWork.Auth;
using InternalWork.Auth.Common.Models;
using InternalWork.Auth.Data.Entities;
using InternalWork.Auth.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InternalWork.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly SignInManager<AppIdentityUser> signInManager;
        private readonly UserManager<AppIdentityUser> userManager;
        private readonly AuthSetting authSetting;

        public AuthController(SignInManager<AppIdentityUser> signInManager, 
            UserManager<AppIdentityUser> userManager,
            IOptions<AuthSetting> authSetting)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.authSetting = authSetting.Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await this.userManager.FindByNameAsync(loginRequest.UserName);
            if (user is null)
            {
                return BadRequest("Invalid user name or password");
            }

            var isValid = await this.signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
            if (isValid.Succeeded)
            {
                var token = Utils.GenerateJwtToken(user, this.authSetting);

                return Ok(new { token });
            }

            return BadRequest("Invalid user name or password");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var identityUser = new AppIdentityUser
            {
                UserName = registerRequest.Email,
                Email = registerRequest.Email
            };

            var user = await this.userManager.CreateAsync(identityUser, registerRequest.Password);
            if (user.Succeeded)
            {
                return Ok();
            }

            return BadRequest(user.Errors);
        }
    }
}
