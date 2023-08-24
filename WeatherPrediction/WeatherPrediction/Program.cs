using System.Text;
using WeatherPrediction;

Console.WriteLine("Beginning Program!");
//GlobalAttributes
DatabaseInterface databaseInterface = new DatabaseInterface();
//SB don't forget to uncomment
//WebServer webServer = new WebServer();
//webServer.Main();



//webserver.XXXXXXXXX += new EventHandler<UserData>     (HandleAddUserRequest);
//webserver.XXXXXXXXX += new EventHandler<WeatherData>  (HandleAddWeatherDataRequest);
//webserver.XXXXXXXXX += new EventHandler<UserData>     (HandleUpdateUserData);
//webserver.XXXXXXXXX += new EventHandler<WeatherData>  (HandleUpdateWeatherData);
//webserver.XXXXXXXXX += new EventHandler<UserData>     (HandleGetSingleUserProfile);
//webserver.XXXXXXXXX += new EventHandler               (HandleReadAllUserDataFromDatabase);
//webserver.XXXXXXXXX += new EventHandler<Counties>     (HandleReadWeatherDataSetFromDatabase);
//webserver.XXXXXXXXX += new EventHandler<UserData>     (HandleRemoveUserDataFromDatabase);
//webserver.XXXXXXXXX += new EventHandler<WeatherData>  (HandleRemoveWeatherDataFromDatabase);
//webserver.XXXXXXXXX += new EventHandler<WeatherData>  (HandleWeatherPrediction);

void HandleAddUserRequest(object sender, UserData theData)
{
    bool commandSuccesful;
    commandSuccesful = databaseInterface.AddUserDataToDatabase(theData.userName, theData.password, theData.permissions);
    //webserver.DoneCommand(commandSuccesful);
}

void HandleAddWeatherDataRequest(object sender, WeatherData theData)
{
    bool commandSuccesful;
    commandSuccesful = databaseInterface.AddWeatherDataToDatabase(theData.reporterId, theData.temperature, theData.pressure, theData.humidity, theData.windSpeed, theData.date, theData.county, theData.WeatherCondition);
    //webserver.DoneCommand(commandSuccesful);
}

void HandleUpdateUserData(object sender, UserData theData)
{
    bool commandSuccesful;
    commandSuccesful = databaseInterface.UpdateUserDataToDatabase(theData.userName, theData.password, theData.permissions);
    //webserver.DoneCommand(commandSuccesful);
}

void HandleUpdateWeatherData(object sender, WeatherData theData)
{
    bool commandSuccesful;
    commandSuccesful = databaseInterface.UpdateWeatherDataToDatabase(theData.reporterId, theData.temperature, theData.pressure, theData.humidity, theData.windSpeed, theData.date, theData.county, theData.WeatherCondition);
    //webserver.DoneCommand(commandSuccesful);
}

void HandleGetSingleUserProfile(object sender, UserData theData)
{
    UserDataDoneCommand dataForWebserver = new UserDataDoneCommand();
    UserData theUserData;
    theUserData = databaseInterface.ReadSingleUserDataFromDatabase(theData.userName);


    if (theUserData != null)
    {
        dataForWebserver.commandSuccessful = true;
        dataForWebserver.userName = theUserData.userName;
        dataForWebserver.password = theUserData.password;
        dataForWebserver.permissions = theUserData.permissions;
    }
    else
    {
        dataForWebserver.commandSuccessful = false;
    }
    //SB: This wasn't the way to do it, cause we're just calling an operation there was no need for a whole new class, this can just be 2 parameters
    //webserver.DoneCommandWithUserData(dataForWebserver);      //Note this is different to the previous commands because we actually want to send data back to the web server not just a bool
}

void HandleReadAllUserDataFromDatabase(object sender)
{
    bool commandSuccesful = false;
    List<UserData> theData = databaseInterface.ReadAllUserDataFromDatabase();
    if (theData != null)
    {
        commandSuccesful = true;
    }
    //webserver.DoneCommandWithWeatherData(commandSuccesful, theData);
}

void HandleReadWeatherDataSetFromDatabase(object sender, Counties county) 
{
    bool commandSuccesful = false;
    List<WeatherData> theData = databaseInterface.ReadWeatherDataSetFromDatabase(county);
    if (theData != null)
    {
        commandSuccesful = true;
    }
    //webserver.DoneCommandWithWeatherData(commandSuccesful, theData);
}

void HandleRemoveUserDataFromDatabase(object sender, UserData theData)
{
    bool commandSuccesful;
    commandSuccesful = databaseInterface.RemoveUserDataFromDatabase(theData.userName);
    //webserver.DoneCommand(commandSuccesful);
}

void HandleRemoveWeatherDataFromDatabase(object sender, WeatherData theData)
{
    bool commandSuccesful;
    commandSuccesful = databaseInterface.RemoveWeatherDataFromDatabase(theData.reporterId, theData.date);
    //webserver.DoneCommand(commandSuccesful);
}

void HandleWeatherPrediction(object sender, WeatherData theData)
{
    string thePrediction;
    List<WeatherData> theHistoricalData = databaseInterface.ReadWeatherDataSetFromDatabase(theData.county);
    thePrediction = MakePrediction(theHistoricalData, theData);


    //webserver.DoneWeatherCommand(commandSuccesful, thePrediction);
}


//SB: This function is going to be moved into it's own class when it is working correctly
/// <summary>
/// This fucntion carries out the whole of the Prediction process when passed with a set of historical Data and User Weather Data
/// </summary>
string MakePrediction(List<WeatherData> historicalData, WeatherData userWeatherData)
{
    int currentEpochTime = userWeatherData.date;
    int oneMonthEpochTime = currentEpochTime - PredicitionConstants.epochMonth;
    //First we need to get all the data that is relevant to the prediction we are making (Like in the last month?)
    //Do this by finding a month ago in Epoch and then filtering the list of data based off of that
    List<WeatherData> filteredData = new List<WeatherData>();
    List<WeatherData> Level2Match = new List<WeatherData>();
    List<WeatherData> Level3Match = new List<WeatherData>();

    foreach (WeatherData weatherData in historicalData)
    {
        if(weatherData.date >= oneMonthEpochTime)
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
        if(weatherData.temperature <= tempHigh      && weatherData.temperature >= tempLow
            && weatherData.pressure <= pressureHigh && weatherData.pressure >= pressureLow
            && weatherData.humidity <= humidHigh    && weatherData.humidity >= humidLow)
        {
            Level3Match.Add(weatherData);
        }
        else if(weatherData.pressure <= pressureHigh && weatherData.pressure >= pressureLow
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

    //SB: TODO
    //If there's enough data with a very good match what we can do is we will use the data to get a mean conditional value and then use averages to allow for us to round it
    if (IsEnoughLevel3HistoricData)
    {
        double totalCondition = 0;
        double meanCondition;
        foreach(WeatherData weatherData in Level3Match)
        {
            totalCondition = totalCondition + (double)weatherData.WeatherCondition;
        }
        meanCondition = totalCondition / Level3Match.Count();

    }

    //SB: TODO
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

    }
    //SB: TODO
    //we will then use some very basic prediction using the pressure and humidity and time of year hopefully they match
    else
    {

    }



    return "";
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

while (true)
{
    Console.ReadKey();

}
