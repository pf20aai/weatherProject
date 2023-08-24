using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WeatherPrediction
{
    public class WebServer
    {
        // attributes

        private bool runningCommand = false;
        private HttpListenerContext context = null;
        private requestTypes lastRequest = requestTypes.None;
        private bool isAuthenticated = false;
        private bool isAdmin = false;


        //events

        public event EventHandler<UserData> AddUserEvent;

        public event EventHandler<WeatherData> AddWeatherDataEvent;

        public event EventHandler<WeatherData> PredictWeatherEvent;
        
        public event EventHandler<WeatherData> UpdateWeatherDataEvent;

        public event EventHandler<UserData> UpdateUserDataEvent;

        public event EventHandler<UserData> LoginUserEvent;

        public event EventHandler<UserData> GetUserEvent;

        public event EventHandler GetAllUsersEvent;

        // methods

        // Event methods
        public void AuthenticateUser(AuthenticationData authenticationData)
        {
            if (!authenticationData.isAuthenticated)
            {
                SendHttpResponse((int)HttpStatusCode.Unauthorized);
            }
            else 
            {
                this.isAuthenticated = authenticationData.isAuthenticated;
                this.isAdmin = authenticationData.isAdmin;
                SendHttpResponse((int)HttpStatusCode.Accepted);
            }
            StoppingCommand();

        }

        public void DoneNoData(bool isDone)
        {
            if (isDone)
            {
                SendHttpResponse((int)HttpStatusCode.OK);
            }
            else 
            { 
                SendHttpResponse((int)HttpStatusCode.BadRequest);
            }

            StoppingCommand();
        }

        public void DoneWithSingleUserData(UserDataDoneCommand userData)
        {
            if (userData.commandSuccessful)
            {
                SendHttpResponse((int)HttpStatusCode.OK, FormatCommandUserDataIntoHtmlString(userData));
            }
            else
            {
                SendHttpResponse((int)HttpStatusCode.NotFound);
            }

            StoppingCommand();
        }

        public void DoneWithUserDataList(bool commandSuccessful, List<UserData> userData)
        {
            if (commandSuccessful)
            {
                string fullUserDataString = "";

                foreach (UserData user in userData)
                {
                    fullUserDataString += FormatUserDataIntoHtmlString(user);
                }

            }
            StoppingCommand();
        }

        public void returnPrediction(WeatherData weatherData)
        {
            string prediction = FormatWeatherDataIntoHtmlString(weatherData);

            SendHttpResponse((int)HttpStatusCode.OK, prediction);
            StoppingCommand();
        }

        // Webserver methods
        void StartingCommand(requestTypes requestType)
        {
            this.lastRequest = requestType;
            this.runningCommand = true;
        }

        void StoppingCommand()
        {
            this.lastRequest = requestTypes.None; 
            this.runningCommand = false;
        }

        void SendHttpResponse(int statusCode, string data = "")
        {
            using HttpListenerResponse resp = this.context.Response;
            resp.Headers.Set("Content-Type", "text/plain");

            resp.StatusCode = statusCode;

            byte[] buffer = Encoding.UTF8.GetBytes(data);
            resp.ContentLength64 = buffer.Length;
            using Stream ros = resp.OutputStream;

            ros.Write(buffer, 0, buffer.Length);
            resp.Close();
        }

        void SendNotFoundResponse()
        {
            Console.WriteLine("Unknown path");
            SendHttpResponse((int)HttpStatusCode.NotFound, "Not Found");
        }

        static string GetRequestData(HttpListenerRequest req)
        {
            System.IO.Stream body = req.InputStream;
            System.Text.Encoding encoding = req.ContentEncoding;
            System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);

            string requestData = reader.ReadToEnd();
            body.Close();
            reader.Close();


            return (requestData);
        }

        static string RetrieveWeatherData(string county = "")
        {
            var countys = new List<string>()
        {
            "devon",
            "cornwall"
        };
            if (county != "")
            {
                if (countys.Contains(county.ToLower()))
                {
                    Console.WriteLine($"Get data for {county}");
                    return ($"Data for {county}");
                }
                else
                {
                    Console.WriteLine($"Couldn't find {county}");
                    return ("not found");
                }
            }
            else
            {
                Console.WriteLine("Get weather data");
                return ("Weather data here");
            }
        }

        static string FormatWeatherDataIntoHtmlString(WeatherData weatherData)
        {
            string weatherDataString = "";

            weatherDataString += $"\nEstimated temperature: {weatherData.temperature}";
            weatherDataString += $"\nEstimated pressure: {weatherData.pressure}";
            weatherDataString += $"\nEstimated humidity: {weatherData.humidity}";
            weatherDataString += $"\nEstimated wind speed: {weatherData.windSpeed}";
            weatherDataString += $"\nEstimated conditions: {weatherData.WeatherCondition}";

            return weatherDataString;
        }

        static string FormatUserDataIntoHtmlString(UserData userData)
        {
            string userDataString = "";

            userDataString += $"\nUser Id: {userData.userName}, User permissions: {userData.permissions}";

            return userDataString;
        }

        static string FormatCommandUserDataIntoHtmlString(UserDataDoneCommand userData)
        {
            string userDataString = "";

            userDataString += $"\nUser Id: {userData.userName}";
            userDataString += $"\nUser permissions: {userData.permissions}";

            return userDataString;
        }

        static List<List<string>> SeperateHtmlReponseIntoKeyPairs(string data)
        {
            List<List<string>> seperatedKeyPairs = new List<List<string>>();
            List<string> valueKeyPairs = data.Split("&").ToList();

            foreach (string keyPair in valueKeyPairs)
            {
                seperatedKeyPairs.Add(keyPair.Split("=").ToList());
            }

            return seperatedKeyPairs;
        }

        static WeatherData SeperateHtmlIntoWeatherData(string data)
        {
            WeatherData weatherData = new WeatherData();
            List<List<string>> keyPairs = SeperateHtmlReponseIntoKeyPairs(data);

            foreach (List<string> keyPair in keyPairs)
            {
                if (keyPair.Contains("reporterId"))
                {
                    keyPair.Remove("reporterId");
                    weatherData.reporterId = keyPair[0];
                }
                else if (keyPair.Contains("temperature"))
                {
                    keyPair.Remove("temperature");
                    try
                    {
                        weatherData.temperature = double.Parse(keyPair[0]);
                    }
                    catch (System.FormatException)
                    {
                        weatherData.temperature = 0;
                    }
                }
                else if (keyPair.Contains("pressure"))
                {
                    keyPair.Remove("pressure");
                    try
                    {
                        weatherData.pressure = double.Parse(keyPair[0]);
                    }
                    catch (System.FormatException)
                    {
                        weatherData.pressure = 0;
                    }
                }
                else if (keyPair.Contains("humidity"))
                {
                    keyPair.Remove("humidity");
                    try
                    {
                        weatherData.humidity = int.Parse(keyPair[0]);
                    }
                    catch (System.FormatException)
                    {
                        weatherData.humidity = 0;
                    }
                }
                else if (keyPair.Contains("windSpeed"))
                {
                    keyPair.Remove("windSpeed");
                    try
                    {
                        weatherData.windSpeed = double.Parse(keyPair[0]);
                    }
                    catch (System.FormatException)
                    {
                        weatherData.windSpeed = 0.0;
                    }
                }
                else if (keyPair.Contains("date"))
                {
                    keyPair.Remove("date");
                    try
                    {
                        weatherData.date = int.Parse(keyPair[0]);
                    }
                    catch (System.FormatException)
                    {
                        weatherData.date = 0;
                    }
                }
                else if (keyPair.Contains("county"))
                {
                    keyPair.Remove("county");
                    try
                    {
                        weatherData.county = (Counties)Enum.Parse(typeof(Counties), keyPair[0]);
                    }
                    catch (System.FormatException)
                    {
                        weatherData.county = Counties.Hertfordshire;
                    }
                }
                else if (keyPair.Contains("WeatherCondition"))
                {
                    keyPair.Remove("WeatherCondition");
                    try
                    {
                        weatherData.WeatherCondition = (WeatherConditions)Enum.Parse(typeof(WeatherConditions), keyPair[0]);
                    }
                    catch (System.FormatException)
                    {
                        weatherData.WeatherCondition = WeatherConditions.PartiallyCloudy;
                    }

                }

            }

            return weatherData;

        }

        static UserData SeperateHtmlIntoUserData(string data)
        {
            UserData userData = new UserData();
            List<List<string>> seperatedKeyPairs = SeperateHtmlReponseIntoKeyPairs(data);

            foreach (List<string> keyPair in seperatedKeyPairs) 
            {
                if (keyPair.Contains("userName")) 
                {
                    keyPair.Remove("userName");
                    userData.userName = keyPair[0];
                }
                else if (keyPair.Contains("password"))
                {
                    keyPair.Remove("password");
                    userData.password = keyPair[0];
                }
                else if (keyPair.Contains("permissions"))
                {
                    keyPair.Remove("permissions");
                    try
                    {
                        userData.permissions = int.Parse(keyPair[0]);
                    }
                    catch (System.FormatException)
                    {
                        userData.permissions = 0;
                    }
                    
                }
            }

            return userData;

        }

        public void Main()
        {
            using var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:80/");

            listener.Start();

            Console.WriteLine("Listening on port 80...");

            while (true)
            {
                context = listener.GetContext();
                HttpListenerRequest req = context.Request;

                Console.WriteLine($"Received {req.HttpMethod} request to {req.Url.AbsolutePath} from {req.Url}");

                if (!runningCommand)
                {
                    if (req.HttpMethod == "GET")
                    {
                        if (req.Url.AbsolutePath == "/weather-data")
                        {
                            StartingCommand(requestTypes.GetWeatherData);
                            string data = RetrieveWeatherData();
                            SendHttpResponse((int)HttpStatusCode.OK, data);
                        }
                        else if (req.Url.AbsolutePath == "/weather-data/county")
                        {
                            StartingCommand(requestTypes.GetWeatherData);
                            string county = req.QueryString["county"];
                            string data = RetrieveWeatherData(county);
                            SendHttpResponse((int)HttpStatusCode.OK, data);
                        }
                        else if (req.Url.AbsolutePath == "/user")
                        {
                            StartingCommand(requestTypes.GetWeatherData);
                            UserData userData = new UserData();
                            userData.userName = req.QueryString["id"];
                            GetUserEvent?.Invoke(this, userData);
                        }
                        else if (req.Url.AbsolutePath == "/users")
                        {
                            StartingCommand(requestTypes.GetWeatherData);
                            GetAllUsersEvent?.Invoke(this);
                        }
                        else
                        {
                            SendNotFoundResponse();
                        }
                    }
                    else if (req.HttpMethod == "POST")
                    {
                        if (req.Url.AbsolutePath == "/login")
                        {
                            StartingCommand(requestTypes.PostLogin);
                            LoginUserEvent?.Invoke(this, SeperateHtmlIntoUserData(GetRequestData(req)));

                        }
                        else if (req.Url.AbsolutePath == "/signup")
                        {
                            StartingCommand(requestTypes.PostSignUp);
                            UserData userData = SeperateHtmlIntoUserData(GetRequestData(req));
                            AddUserEvent?.Invoke(this, userData);

                        }
                        // put something here to make sure everything is authenticated
                        else if (req.Url.AbsolutePath == "/predict")
                        {
                            StartingCommand(requestTypes.PostPredict);
                            WeatherData data = SeperateHtmlIntoWeatherData(GetRequestData(req));

                            PredictWeatherEvent?.Invoke(this, data);
                        }
                        else if (req.Url.AbsolutePath == "/weather-data")  //need admin auth here
                        {
                            StartingCommand(requestTypes.PostWeatherData);
                            AddWeatherDataEvent?.Invoke(this, SeperateHtmlIntoWeatherData(GetRequestData(req)));
                        }
                        else
                        {
                            SendNotFoundResponse();
                        }
                        }
                    else if (req.HttpMethod == "PUT")
                    {
                        if (req.Url.AbsolutePath == "weather-data")
                        {
                            // PF put isn't working right now
                            StartingCommand(requestTypes.PutWeatherData);
                            UpdateWeatherDataEvent?.Invoke(this, SeperateHtmlIntoWeatherData(GetRequestData(req)));
                        }
                        else if(req.Url.AbsolutePath == "users")
                        {
                            StartingCommand(requestTypes.PutWeatherData);
                            UpdateUserDataEvent?.Invoke(this, SeperateHtmlIntoUserData(GetRequestData(req)));
                        }
                        else
                        {
                            SendNotFoundResponse();
                        }
                    }
                    else
                    {
                        SendNotFoundResponse();
                    }
                }
                else 
                {
                    SendHttpResponse((int)HttpStatusCode.Conflict); 
                }

            }
        }
    }
}
