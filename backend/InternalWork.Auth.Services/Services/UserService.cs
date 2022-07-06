using InternalWork.Auth.Services.Models;
using InternalWork.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWork.Auth.Services.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext dbContext;

        public UserService(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<int> CreateUserInfo(Guid identityId, UserDto userDto)
        {
            var userInfo = await this.dbContext.UserInfos.Where(x => x.IdentityId == identityId).SingleOrDefaultAsync();
            if (userInfo is not null)
            {
                throw new InvalidDataException();
            }

            userInfo = new Data.Entities.User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Active = userDto.Active,
                BirthDay = userDto.BirthDay,
                Address = userDto.Address,
                IdentityId = identityId,
            };

            this.dbContext.UserInfos.Add(userInfo);

            var identity = await this.dbContext.Users.Where(x => x.Id == identityId).SingleOrDefaultAsync();
            if (identity is null)
            {
                throw new InvalidDataException();
            }

            identity.PhoneNumber = userDto.PhoneNumber;

            return await this.dbContext.SaveChangesAsync();
        }
    }
}
