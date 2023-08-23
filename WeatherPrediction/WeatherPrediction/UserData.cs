using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherPrediction
{
    public class UserData
    {
        public string userName = "";
        public string password = "";
        public int permissions;

        public UserData(string theUserName, string thePassword, int thePermissions)
        {
            userName = theUserName;
            password = thePassword;
            permissions = thePermissions;
        }

        public UserData()
        {

        }
    }
}
