using InternalWork.Auth.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWork.Auth.Services.Services
{
    public interface IUserService
    {
        Task<int> CreateUserInfo(Guid identityId, UserDto userDto);
    }
}
