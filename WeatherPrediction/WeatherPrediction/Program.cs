using System.Text;
using WeatherPrediction;

Console.WriteLine("Beginning Program!");
//GlobalAttributes
DatabaseInterface databaseInterface = new DatabaseInterface();
//WebServer webServer = new WebServer();
//webServer.Main();



//webserver.XXXXXXXXX += HandleAddUserRequest;
//webserver.XXXXXXXXX += HandleAddWeatherDataRequest;
//webserver.XXXXXXXXX += HandleUpdateUserData;
//webserver.XXXXXXXXX += HandleUpdateWeatherData;

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

string MakePrediction(List<WeatherData> historicalData, WeatherData userData)
{
    // 30 days in epoch is 2,592,000
    int epochMonth = 2592000;
    //First we need to get all the data that is relevant to the prediction we are making (Like in the last month?)
    //Do this by finding a month ago in Epoch and then filtering the list of data based off of that
    //Check what data in that set is 'Similar' to our user data if any
    //then we use their condition as our own

    //If nothing matches we do a widen search?

    //else we will use some very basic prediction using the pressure

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
