using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InternalWork.Auth.Common.SettingModels
{
    public class SampleDataSetting
    {
        public UserSetting[] Users { get; set; }
    }

    public class UserSetting
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
