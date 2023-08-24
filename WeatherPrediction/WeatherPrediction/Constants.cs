using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherPrediction
{
    
    public static class PredicitionConstants
    {
        public const int epochMonth = 2592000;
        public const double TemperatureDiff = 1.0;
        public const double PressureDiff = 1.0;
        public const int HumidityDiff = 2;
        public const double HighPressure = 1015.0;
        public const double LowPressure = 1011.0;
        public const double StandardPressure = 1013.0;
        public const int HumidityAverage = 60;
    }

}
