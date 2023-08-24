using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherPrediction
{
    public class AuthenticationData
    {
        public bool isAuthenticated;
        public bool isAdmin;

        public AuthenticationData(bool authentication, bool admin)
        {
            isAuthenticated = authentication;
            isAdmin = admin;
        }
        public AuthenticationData()
        {
            isAuthenticated = false;
            isAdmin = false;

        }
    }
}



