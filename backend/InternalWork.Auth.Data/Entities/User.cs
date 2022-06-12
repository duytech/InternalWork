using Microsoft.AspNetCore.Identity;

namespace InternalWork.Auth.Data.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime BirthDay { get; set; }

        public Guid IdentityId { get; set; }

        public int MyProperty { get; set; }

        public AppIdentityUser AppIdentityUser { get; set; }
    }
}