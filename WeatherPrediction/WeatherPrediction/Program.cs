using System.Text;
using WeatherPrediction;

Console.WriteLine("Beginning Program!");
//GlobalAttributes
DatabaseInterface databaseInterface = new DatabaseInterface();
WeatherPredictor weatherPredictor = new WeatherPredictor();

WebServer webServer = new WebServer();
webServer.AddUserEvent += new EventHandler<UserData>(HandleAddUserRequest);
webServer.AddWeatherDataEvent += new EventHandler<WeatherData>(HandleAddWeatherDataRequest);
webServer.PredictWeatherEvent += new EventHandler<WeatherData>(HandleMakePrediction);
webServer.UpdateWeatherDataEvent += new EventHandler<WeatherData>(HandleUpdateWeatherData);
webServer.UpdateUserDataEvent += new EventHandler<UserData>(HandleUpdateUserData);
webServer.LoginUserEvent += new EventHandler<UserData>(HandleUserLogin);
webServer.GetUserEvent += new EventHandler<UserData>(HandleGetSingleUserProfile);
//webserver.XXXXXXXXX += new EventHandler<Counties>     (HandleReadWeatherDataSetFromDatabase);
//webserver.XXXXXXXXX += new EventHandler<UserData>     (HandleRemoveUserDataFromDatabase);
//webserver.XXXXXXXXX += new EventHandler<WeatherData>  (HandleRemoveWeatherDataFromDatabase);
//webserver.XXXXXXXXX += new EventHandler<WeatherData>  (HandleWeatherPrediction);
webServer.GetAllUsersEvent += new EventHandler<EventArgs> (HandleReadAllUserDataFromDatabase);
webServer.Main();

/// <summary>
/// When using the server with PostMan use this command
/// 
/// userName=Jim&password=Alpha22&permissions=0
/// 
/// </summary>


void HandleUserLogin(object sender, UserData userData)
{
    //PF Sam something here needs to be done to check if the user is authnticated correctly
    //SB: Yes we need to check who the user is, see what their permission is and then we will return this to you
    AuthenticationData authenticationData = new AuthenticationData(); // placeholder
    authenticationData.isAuthenticated = true;
    authenticationData.isAdmin = true;
    webServer.AuthenticateUser(authenticationData);
}

void HandleAddUserRequest(object sender, UserData theData) 
{
    bool commandSuccesful;
    commandSuccesful = databaseInterface.AddUserDataToDatabase(theData.userName, theData.password, theData.permissions);
    Console.WriteLine("Sending Reply To Webserver");
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
    DataWeatherPrediction thePrediction;
    List<WeatherData> theHistoricalData = databaseInterface.ReadWeatherDataSetFromDatabase(weatherData.county);
    thePrediction = weatherPredictor.makePrediction(theHistoricalData, weatherData);
    //PF you'll want to change this to the new data Type
    //webServer.returnPrediction(weatherData);
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
    webServer.DoneWithSingleUserData(dataForWebserver);      //Note this is different to the previous commands because we actually want to send data back to the web server not just a bool
}

void HandleReadAllUserDataFromDatabase(object sender, EventArgs e)
{
    bool commandSuccesful = false;
    List<UserData> theData = databaseInterface.ReadAllUserDataFromDatabase();
    if (theData != null)
    {
        commandSuccesful = true;
    }
    webServer.DoneWithUserDataList(commandSuccesful, theData);
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
    DataWeatherPrediction thePrediction;
    List<WeatherData> theHistoricalData = databaseInterface.ReadWeatherDataSetFromDatabase(theData.county);
    thePrediction = weatherPredictor.makePrediction(theHistoricalData, theData);
}


