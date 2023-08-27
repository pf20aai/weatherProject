using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherPrediction
{
    public class DataWeatherPrediction
    {
        public WeatherConditions currentPredicted = WeatherConditions.Sunny;
        public WeatherConditions futurePredicted = WeatherConditions.Sunny;

        public DataWeatherPrediction(WeatherConditions theCurrentWeather, WeatherConditions theFutureWeather)
        {
            currentPredicted = theCurrentWeather;
            futurePredicted = theFutureWeather;
      
        }

        public DataWeatherPrediction()
        {

        }
    }
}
