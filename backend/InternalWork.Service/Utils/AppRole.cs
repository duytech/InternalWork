using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWork.Auth.Data.Utils
{
    public static class AppRole
    {
        public const string Admin = "Admin";
        public const string Operator = "Operator";
        public const string Member = "Member";

        public static string[] GetArray()
        {
            return new string[] { Admin, Member, Operator };
        }
    }
}
