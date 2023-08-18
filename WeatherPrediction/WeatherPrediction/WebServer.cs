using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WeatherPrediction
{
    public class WebServer
    {
        static void SendHttpResponse(HttpListenerContext context, int statusCode, string data = "")
        {
            using HttpListenerResponse resp = context.Response;
            resp.Headers.Set("Content-Type", "text/plain");

            resp.StatusCode = statusCode;

            byte[] buffer = Encoding.UTF8.GetBytes(data);
            resp.ContentLength64 = buffer.Length;
            using Stream ros = resp.OutputStream;

            ros.Write(buffer, 0, buffer.Length);
            resp.Close();
        }

        static void SendNotFoundResponse(HttpListenerContext context)
        {
            Console.WriteLine("Unknown path");
            SendHttpResponse(context, (int)HttpStatusCode.NotFound, "Not Found");
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

        static bool AttemptLogin(string details)
        {
            if (details == "secretTunnel")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static string SignUpUser(string data)
        {
            if (data == "fight")
            {
                return ("conflict");
            }
            else
            {
                return ("123456789");
            }
        }

        static string GeneratePrediction(string data)
        {
            string prediction = "IDK cloudy I guess?";

            return (prediction);
        }

        static void SendDataToDatabase(string data)
        {
            Console.WriteLine($"Sending data to database\n{data}");
        }

        static void UpdateDataInDatabase(string data, string entryId)
        {
            Console.WriteLine($"Updating data for entry {entryId} in database\n{data}");
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

        public void Main()
        {
            using var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:80/");

            listener.Start();

            Console.WriteLine("Listening on port 80...");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest req = context.Request;

                Console.WriteLine($"Received {req.HttpMethod} request to {req.Url.AbsolutePath} from {req.Url}");

                if (req.HttpMethod == "GET")
                {
                    if (req.Url.AbsolutePath == "/weather-data")
                    {
                        string data = RetrieveWeatherData();
                        SendHttpResponse(context, (int)HttpStatusCode.OK, data);
                    }
                    else if (req.Url.AbsolutePath == "/weather-data/county")
                    {
                        string county = req.QueryString["county"];
                        string data = RetrieveWeatherData(county);

                        SendHttpResponse(context, (int)HttpStatusCode.OK, data);
                    }

                    else
                    {
                        SendNotFoundResponse(context);
                    }
                }
                else if (req.HttpMethod == "POST")
                {
                    if (req.Url.AbsolutePath == "/login")
                    {
                        string data = GetRequestData(req);

                        bool authenticated = AttemptLogin(data);

                        if (authenticated)
                        {
                            SendHttpResponse(context, (int)HttpStatusCode.OK, "{id: abcd12345}");
                        }
                        else
                        {
                            SendHttpResponse(context, (int)HttpStatusCode.Unauthorized, "Unknown user");
                        }

                    }
                    else if (req.Url.AbsolutePath == "/signup")
                    {
                        string signUpRequest = GetRequestData(req);

                        string signUpResult = SignUpUser(signUpRequest);

                        if (signUpResult == "conflict")
                        {
                            SendHttpResponse(context, (int)HttpStatusCode.Conflict, "userConflict");
                        }
                        else
                        {
                            SendHttpResponse(context, (int)HttpStatusCode.Accepted, signUpResult);
                        }

                    }
                    // put something here to make sure everything is authenticated
                    else if (req.Url.AbsolutePath == "/predict")
                    {
                        string data = GetRequestData(req);
                        string prediction = GeneratePrediction(data);

                        SendHttpResponse(context, (int)HttpStatusCode.OK, prediction);
                    }
                    else if (req.Url.AbsolutePath == "/weather-data")  //need admin auth here
                    {
                        string data = GetRequestData(req);
                        SendDataToDatabase(data);
                        SendHttpResponse(context, (int)HttpStatusCode.Created);
                    }
                    else
                    {
                        SendNotFoundResponse(context);
                    }
                }
                else if (req.HttpMethod == "PUT")
                {
                    if (req.Url.AbsolutePath == "weather-data")
                    {
                        string data = GetRequestData(req);
                        UpdateDataInDatabase(data, "1234567890");
                        SendHttpResponse(context, (int)HttpStatusCode.Accepted);
                    }
                    else
                    {
                        SendNotFoundResponse(context);
                    }
                }

                else
                {
                    SendNotFoundResponse(context);
                }

            }
        }
    }
}
