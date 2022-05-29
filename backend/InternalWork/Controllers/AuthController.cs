using InternalWork.Auth;
using InternalWork.Auth.Common.Models;
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
        private readonly SignInManager<IdentityUser<Guid>> signInManager;
        private readonly UserManager<IdentityUser<Guid>> userManager;
        private readonly AuthSetting authSetting;

        public AuthController(SignInManager<IdentityUser<Guid>> signInManager, 
            UserManager<IdentityUser<Guid>> userManager,
            IOptions<AuthSetting> authSetting)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.authSetting = authSetting.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var user = await this.userManager.FindByNameAsync(loginRequest.UserName);
            var isValid = await this.signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
            if (isValid.Succeeded)
            {
                var token = Utils.GenerateJwtToken(user, this.authSetting);

                return Ok(new { token });
            }

            return BadRequest();
        }
    }
}
