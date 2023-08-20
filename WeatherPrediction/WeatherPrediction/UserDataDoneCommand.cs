using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherPrediction
{
    public class UserDataDoneCommand
    {
        public string userName { get; set; }
        public string password { get; set; }
        public int permissions { get; set; }
        public bool commandSuccessful { get; set; }

        public UserDataDoneCommand()
        {
            userName = "";
            password = "";
            permissions = 0;
            commandSuccessful = false;
        }

        public UserDataDoneCommand(string theUserName, string thePassword, int thePermissions, bool isSuccessful)
        {
            userName = theUserName;
            password = thePassword;
            permissions = thePermissions;
            commandSuccessful = isSuccessful;
        }

        
    }
}
