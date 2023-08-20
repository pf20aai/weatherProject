﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeatherPrediction
{
    public class WeatherData
    {
        public string reporterId;
        public double temperature;
        public double pressure;
        public int humidity;
        public double windSpeed;
        public int date;
        public Counties county;
        public WeatherConditions WeatherCondition;

        public WeatherData(string theUserName, double theTemperature, double thePressure, int theHumidity, double theWindSpeed, int theDate, Counties theCounty, WeatherConditions theCondition)
        {
            reporterId = theUserName;
            temperature = theTemperature;
            pressure = thePressure;
            humidity = theHumidity;
            windSpeed = theWindSpeed;
            date = theDate;
            county = theCounty;
            WeatherCondition = theCondition;
        }
    }
}



