using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWork.Auth.Services.Models
{
    public class UserDto : BaseDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDay { get; set; }

        public string Address { get; set; }

        public bool Active { get; set; }

        public string PhoneNumber { get; set; }
    }
}
