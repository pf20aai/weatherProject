using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherPrediction;

namespace WeatherPrediction
{
    public class WeatherPredictor
    {
        public WeatherPredictor()
        {

        }

        /// <summary>
        /// This fucntion carries out the whole of the Prediction process when passed with a set of historical Data and User Weather Data
        /// </summary>
        public DataWeatherPrediction makePrediction(List<WeatherData> historicalData, WeatherData userWeatherData)
        {
            WeatherConditions dataPredictedCurrentCondition = new WeatherConditions();
            WeatherConditions algorithmPredictedCurrentCondition = new WeatherConditions();
            WeatherConditions algorithmPredictedComingCondition = new WeatherConditions();

            int currentEpochTime = userWeatherData.date;
            int oneMonthEpochTime = currentEpochTime - PredicitionConstants.epochMonth;
            //First we need to get all the data that is relevant to the prediction we are making (Like in the last month?)
            //Do this by finding a month ago in Epoch and then filtering the list of data based off of that
            List<WeatherData> filteredData = new List<WeatherData>();
            List<WeatherData> Level2Match = new List<WeatherData>();
            List<WeatherData> Level3Match = new List<WeatherData>();

            foreach (WeatherData weatherData in historicalData)
            {
                if (weatherData.date >= oneMonthEpochTime)
                {
                    filteredData.Add(weatherData);
                }
            }

            double tempLow = userWeatherData.temperature - PredicitionConstants.TemperatureDiff;
            double tempHigh = userWeatherData.temperature + PredicitionConstants.TemperatureDiff;
            double pressureLow = userWeatherData.pressure - PredicitionConstants.PressureDiff;
            double pressureHigh = userWeatherData.pressure + PredicitionConstants.PressureDiff;
            int humidLow = userWeatherData.humidity - PredicitionConstants.HumidityDiff;
            int humidHigh = userWeatherData.humidity + PredicitionConstants.HumidityDiff;


            //Check what data in that set is 'Similar' to our user data if any
            foreach (WeatherData weatherData in filteredData)
            {
                if (weatherData.temperature <= tempHigh && weatherData.temperature >= tempLow
                    && weatherData.pressure <= pressureHigh && weatherData.pressure >= pressureLow
                    && weatherData.humidity <= humidHigh && weatherData.humidity >= humidLow)
                {
                    Level3Match.Add(weatherData);
                }
                else if (weatherData.pressure <= pressureHigh && weatherData.pressure >= pressureLow
                        && weatherData.humidity <= humidHigh && weatherData.humidity >= humidLow)
                {
                    Level2Match.Add(weatherData);
                }
            }

            //then we use their condition as our own, as a mean for all the level 3s
            bool IsEnoughLevel3HistoricData = false;
            bool IsEnoughLevel2HistoricData = false;
            if (Level3Match.Count > 3)
            {
                IsEnoughLevel3HistoricData = true;
            }

            if (Level2Match.Count > 3)
            {
                IsEnoughLevel2HistoricData = true;
            }

            //If there's enough data with a very good match what we can do is we will use the data to get a mean conditional value and then use averages to allow for us to round it
            if (IsEnoughLevel3HistoricData)
            {
                double totalCondition = 0;
                double meanCondition;
                foreach (WeatherData weatherData in Level3Match)
                {
                    totalCondition = totalCondition + (double)weatherData.WeatherCondition;
                }
                meanCondition = totalCondition / Level3Match.Count();
                int meanRounded;
                meanRounded = (int)Math.Round(meanCondition, 0);
                dataPredictedCurrentCondition = (WeatherConditions)meanRounded;

            }

            //If there's enough data with a good match what we can do is we will use the data to get a mean conditional value and then use averages to allow for us to round it with more weighting
            else if (IsEnoughLevel2HistoricData)
            {
                double totalCondition = 0;
                double meanCondition;
                foreach (WeatherData weatherData in Level3Match)
                {
                    totalCondition = totalCondition + (double)weatherData.WeatherCondition;
                }
                meanCondition = totalCondition / Level2Match.Count();
                int meanRounded;
                meanRounded = (int)Math.Round(meanCondition, 0);
                dataPredictedCurrentCondition = (WeatherConditions)meanRounded;

            }
            //SB: TODO
            //we will then use some very basic prediction using the pressure and humidity and time of year hopefully they match
            //First Find out the time of year it is we do this based on seasons
            DateTime userDate = ConvertEpochToTimeDateFormat(userWeatherData.date);
            if (userDate.Month >= 5 && userDate.Month <= 9) //We use summer prediction
            {
                //High Pressure, High Humidity
                if (userWeatherData.pressure >= PredicitionConstants.HighPressure && userWeatherData.humidity > PredicitionConstants.HumidityAverage)
                {
                    algorithmPredictedCurrentCondition = WeatherConditions.Overcast;
                    if(IsEnoughLevel3HistoricData || IsEnoughLevel2HistoricData)
                    {
                        if ((int)dataPredictedCurrentCondition <= 4)
                        {
                            int newWeather = (int)dataPredictedCurrentCondition + 1;
                            algorithmPredictedComingCondition = (WeatherConditions)newWeather;
                        }
                        else
                        {
                            algorithmPredictedComingCondition = WeatherConditions.Rain;
                        }
                    }
                    else
                    {
                        algorithmPredictedComingCondition = WeatherConditions.Rain;
                    }
                }
                //High Pressure, Low Humidity
                else if (userWeatherData.pressure >= PredicitionConstants.HighPressure && userWeatherData.humidity < PredicitionConstants.HumidityAverage)
                {
                    algorithmPredictedCurrentCondition = WeatherConditions.PartiallyCloudy;
                    if (IsEnoughLevel3HistoricData || IsEnoughLevel2HistoricData)
                    {
                        if ((int)dataPredictedCurrentCondition >= 2)
                        {
                            int newWeather = (int)dataPredictedCurrentCondition - 1;
                            algorithmPredictedComingCondition = (WeatherConditions)newWeather;
                        }
                        else
                        {
                            algorithmPredictedComingCondition = WeatherConditions.Rain;
                        }
                    }
                    else
                    {
                        algorithmPredictedComingCondition = WeatherConditions.Sunny;
                    }
                }
                //low Pressure, High Humidity
                else if (userWeatherData.pressure <= PredicitionConstants.LowPressure && userWeatherData.humidity > PredicitionConstants.HumidityAverage)
                {
                    algorithmPredictedCurrentCondition = WeatherConditions.Drizzle;
                    if (IsEnoughLevel3HistoricData || IsEnoughLevel2HistoricData)
                    {
                        if ((int)dataPredictedCurrentCondition <= 4)
                        {
                            int newWeather = (int)dataPredictedCurrentCondition + 1;
                            algorithmPredictedComingCondition = (WeatherConditions)newWeather;
                        }
                        else
                        {
                            algorithmPredictedComingCondition = WeatherConditions.Rain;
                        }
                    }
                    else
                    {
                        algorithmPredictedComingCondition = WeatherConditions.Drizzle;
                    }
                }
                //low Pressure, Low Humidity
                else if (userWeatherData.pressure <= PredicitionConstants.LowPressure && userWeatherData.humidity > PredicitionConstants.HumidityAverage)
                {
                    algorithmPredictedCurrentCondition = WeatherConditions.Cloudy;
                    if (IsEnoughLevel3HistoricData || IsEnoughLevel2HistoricData)
                    {
                        if ((int)dataPredictedCurrentCondition <= 2)
                        {
                            int newWeather = (int)dataPredictedCurrentCondition + 1;
                            algorithmPredictedComingCondition = (WeatherConditions)newWeather;
                        }
                        else
                        {
                            algorithmPredictedComingCondition = WeatherConditions.Overcast;
                        }
                    }
                    else
                    {
                        algorithmPredictedComingCondition = WeatherConditions.Overcast;
                    }
                }
                //Standard Pressure, Standard Humidity
                else
                {
                    algorithmPredictedCurrentCondition = WeatherConditions.PartiallyCloudy;
                    if (IsEnoughLevel3HistoricData || IsEnoughLevel2HistoricData)
                    {
                        if ((int)dataPredictedCurrentCondition <= 4)
                        {
                            algorithmPredictedComingCondition = dataPredictedCurrentCondition;
                        }
                        else
                        {
                            algorithmPredictedComingCondition = WeatherConditions.Rain;
                        }
                    }
                    else
                    {
                        algorithmPredictedComingCondition = WeatherConditions.PartiallyCloudy;
                    }
                }
            }
            else //We use winter prediction
            {
                //High Pressure, High Humidity
                if (userWeatherData.pressure >= PredicitionConstants.HighPressure && userWeatherData.humidity > PredicitionConstants.HumidityAverage)
                {
                    algorithmPredictedCurrentCondition = WeatherConditions.Overcast;
                    if (IsEnoughLevel3HistoricData || IsEnoughLevel2HistoricData)
                    {
                        if ((int)dataPredictedCurrentCondition <= 3)
                        {
                            int newWeather = (int)dataPredictedCurrentCondition + 1;
                            algorithmPredictedComingCondition = (WeatherConditions)newWeather;
                        }
                        else if(userWeatherData.temperature < -2)
                        {
                            algorithmPredictedComingCondition = WeatherConditions.Snow;
                        }
                        else
                        {
                            algorithmPredictedComingCondition = WeatherConditions.Rain;
                        }
                    }
                    else
                    {
                        algorithmPredictedComingCondition = WeatherConditions.Overcast;
                    }
                }
                //High Pressure, Low Humidity
                else if (userWeatherData.pressure >= PredicitionConstants.HighPressure && userWeatherData.humidity < PredicitionConstants.HumidityAverage)
                {
                    algorithmPredictedCurrentCondition = WeatherConditions.Sunny;
                    if (IsEnoughLevel3HistoricData || IsEnoughLevel2HistoricData)
                    {
                        if ((int)dataPredictedCurrentCondition >= 2)
                        {
                            int newWeather = (int)dataPredictedCurrentCondition - 1;
                            algorithmPredictedComingCondition = (WeatherConditions)newWeather;
                        }
                        else
                        {
                            algorithmPredictedComingCondition = WeatherConditions.Cloudy;
                        }
                    }
                    else
                    {
                        algorithmPredictedComingCondition = WeatherConditions.Sunny;
                    }
                }
                //low Pressure, High Humidity
                else if (userWeatherData.pressure <= PredicitionConstants.LowPressure && userWeatherData.humidity > PredicitionConstants.HumidityAverage)
                {
                    algorithmPredictedCurrentCondition = WeatherConditions.Overcast;
                    if (IsEnoughLevel3HistoricData || IsEnoughLevel2HistoricData)
                    {
                        if ((int)dataPredictedCurrentCondition <= 5)
                        {
                            int newWeather = (int)dataPredictedCurrentCondition + 1;
                            algorithmPredictedComingCondition = (WeatherConditions)newWeather;
                        }
                        else if (userWeatherData.temperature < -2)
                        {
                            algorithmPredictedComingCondition = WeatherConditions.Snow;
                        }
                        else
                        {
                            algorithmPredictedComingCondition = WeatherConditions.Rain;
                        }
                    }
                    else
                    {
                        algorithmPredictedComingCondition = WeatherConditions.Drizzle;
                    }
                }
                //low Pressure, Low Humidity
                else if (userWeatherData.pressure <= PredicitionConstants.LowPressure && userWeatherData.humidity > PredicitionConstants.HumidityAverage)
                {
                    algorithmPredictedCurrentCondition = WeatherConditions.Overcast;
                    if (IsEnoughLevel3HistoricData || IsEnoughLevel2HistoricData)
                    {
                        if ((int)dataPredictedCurrentCondition <= 3)
                        {
                            int newWeather = (int)dataPredictedCurrentCondition + 1;
                            algorithmPredictedComingCondition = (WeatherConditions)newWeather;
                        }
                        else if (userWeatherData.temperature < -2)
                        {
                            algorithmPredictedComingCondition = WeatherConditions.Snow;
                        }
                        else
                        {
                            algorithmPredictedComingCondition = WeatherConditions.Overcast;
                        }
                    }
                    else
                    {
                        algorithmPredictedComingCondition = WeatherConditions.Overcast;
                    }
                }
                //Standard Pressure, Standard Humidity
                else
                {
                    algorithmPredictedCurrentCondition = WeatherConditions.PartiallyCloudy;
                    if (IsEnoughLevel3HistoricData || IsEnoughLevel2HistoricData)
                    {
                        if ((int)dataPredictedCurrentCondition <= 4)
                        {
                            algorithmPredictedComingCondition = dataPredictedCurrentCondition;
                        }
                        else if (userWeatherData.temperature < -4)
                        {
                            algorithmPredictedComingCondition = WeatherConditions.Snow;
                        }
                        else
                        {
                            algorithmPredictedComingCondition = WeatherConditions.Rain;
                        }
                    }
                    else
                    {
                        algorithmPredictedComingCondition = WeatherConditions.PartiallyCloudy;
                    }
                }

            }

            DataWeatherPrediction predictedCondition = new DataWeatherPrediction();
            if (IsEnoughLevel3HistoricData || IsEnoughLevel2HistoricData)
            {
                predictedCondition.currentPredicted = dataPredictedCurrentCondition;
                predictedCondition.futurePredicted = algorithmPredictedComingCondition;
            }
            else
            {
                predictedCondition.currentPredicted = algorithmPredictedCurrentCondition;
                predictedCondition.futurePredicted = algorithmPredictedComingCondition;
            }

            return predictedCondition;
        }

        static int ConvertDateTimeToEpochFormat(DateTime theDateTime)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = theDateTime.ToUniversalTime() - origin;
            return (int)Math.Floor(diff.TotalSeconds);
        }

        static DateTime ConvertEpochToTimeDateFormat(int timeInEpoch)
        {
            DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(timeInEpoch);
            DateTime dateTime = dateTimeOffset.DateTime;
            return dateTime;
        }

    }
}
