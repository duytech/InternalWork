using InternalWork.Auth.Services.Models;
using InternalWork.Auth.Services.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWork.Auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [Authorize]
        [HttpPost("create-info")]
        public async Task<IActionResult> CreateUserInfo(UserDto userDto)
        {
            var identityId = Utils.GetUserIdFromToken(User);
            await this.userService.CreateUserInfo(identityId, userDto);

            return Ok();
        }
    }
}
