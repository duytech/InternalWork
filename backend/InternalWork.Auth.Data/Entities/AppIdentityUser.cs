using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWork.Auth.Data.Entities
{
    public class AppIdentityUser : IdentityUser<Guid>
    {
        public User User { get; set; }
    }
}
