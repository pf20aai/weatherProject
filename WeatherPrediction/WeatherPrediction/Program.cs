// See https://aka.ms/new-console-template for more information



using WeatherPrediction;


Console.WriteLine("Beginning Program!");

//SB: Remove this line to Not Create the Database
DatabaseInterface databaseInterface = new DatabaseInterface();
WebServer webServer = new WebServer();
webServer.Main();


webServer.AddUserEvent += new EventHandler<UserData> (HandleAddUserRequest);
webServer.AddWeatherDataEvent += new EventHandler<WeatherData>(HandleAddWeatherDataRequest);
webServer.AddWeatherDataEvent += new EventHandler<WeatherData>(HandleMakePrediction);
webServer.UpdateWeatherDataEvent += new EventHandler<WeatherData>(HandleUpdateWeatherData);
webServer.UpdateUserDataEvent += new EventHandler<UserData>(HandleUpdateUserData);
webServer.LoginUserEvent += new EventHandler<UserData>(HandleUserLogin);
webServer.GetUserEvent += new EventHandler<UserData>(HandleGetSingleUserProfile);

void HandleUserLogin(object sender, UserData userData)
{
    //PF Sam something here needs to be done to check if the user is authnticated correctly
    AuthenticationData authenticationData = new AuthenticationData(); // placeholder
    webServer.AuthenticateUser(authenticationData);
}

void HandleAddUserRequest(object sender, UserData theData) 
{
    bool commandSuccesful;
    commandSuccesful = databaseInterface.AddUserDataToDatabase(theData.userName, theData.password, theData.permissions);
    webServer.DoneNoData(commandSuccesful);
}

void HandleAddWeatherDataRequest(object sender, WeatherData theData)
{
    bool commandSuccesful;
    commandSuccesful = databaseInterface.AddWeatherDataToDatabase(theData.reporterId, theData.temperature, theData.pressure, theData.humidity, theData.windSpeed, theData.date, theData.county, theData.WeatherCondition);
    webServer.DoneNoData(commandSuccesful);
}

void HandleMakePrediction(object sender, WeatherData weatherData)
{
    //PF We need some prediction logic here
    webServer.returnPrediction(weatherData);
}

void HandleUpdateUserData(object sender, UserData theData)
{
    bool commandSuccesful;
    commandSuccesful = databaseInterface.UpdateUserDataToDatabase(theData.userName, theData.password, theData.permissions);
    webServer.DoneNoData(commandSuccesful);
}

void HandleUpdateWeatherData(object sender, WeatherData theData)
{
    bool commandSuccesful;
    commandSuccesful = databaseInterface.UpdateWeatherDataToDatabase(theData.reporterId, theData.temperature, theData.pressure, theData.humidity, theData.windSpeed, theData.date, theData.county, theData.WeatherCondition);
    webServer.DoneNoData(commandSuccesful);
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
    webServer.DoneWithUserData(dataForWebserver);      //Note this is different to the previous commands because we actually want to send data back to the web server not just a bool
}

//Makes sure the window doesn't close after everything is done
Console.ReadKey();

while (true)
{


}
