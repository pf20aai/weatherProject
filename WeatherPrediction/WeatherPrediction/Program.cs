// See https://aka.ms/new-console-template for more information



using WeatherPrediction;


Console.WriteLine("Beginning Program!");

//SB: Remove this line to Not Create the Database
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
    //webserver.DoneCommandWithUserData(dataForWebserver);      //Note this is different to the previous commands because we actually want to send data back to the web server not just a bool
}

//Makes sure the window doesn't close after everything is done
Console.ReadKey();

while (true)
{


}
