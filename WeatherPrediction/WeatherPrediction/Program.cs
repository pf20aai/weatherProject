using System.Text;
using WeatherPrediction;

Console.WriteLine("Beginning Program!");
//GlobalAttributes
DatabaseInterface databaseInterface = new DatabaseInterface();
WeatherPredictor weatherPredictor = new WeatherPredictor();

WebServer webServer = new WebServer();
webServer.AddUserEvent += new EventHandler<UserData>(HandleAddUserRequest);
webServer.AddWeatherDataEvent += new EventHandler<WeatherData>(HandleAddWeatherDataRequest);
webServer.PredictWeatherEvent += new EventHandler<WeatherData>(HandleWeatherPrediction);
webServer.UpdateWeatherDataEvent += new EventHandler<WeatherData>(HandleUpdateWeatherData);
webServer.UpdateUserDataEvent += new EventHandler<UserData>(HandleUpdateUserData);
webServer.LoginUserEvent += new EventHandler<UserData>(HandleUserLogin);
webServer.GetUserEvent += new EventHandler<UserData>(HandleGetSingleUserProfile);
webServer.GetWeatherDataEvent += new EventHandler<Counties>(HandleReadWeatherDataSetFromDatabase);
webServer.DeleteUserDataEvent += new EventHandler<UserData>     (HandleRemoveUserDataFromDatabase);
webServer.DeleteWeatherDataEvent += new EventHandler<WeatherData>  (HandleRemoveWeatherDataFromDatabase);
//webServer.GetAllUsersEvent += new EventHandler<EventArgs> (HandleReadAllUserDataFromDatabase);
//webServer.GetAllAdminsEvent += new EventHandler<EventArgs> (HandleReadAllAdminDataFromDatabase);
webServer.Main();

/// <summary>
/// When using the server with PostMan use this command
/// 
/// userName=Jim&password=Alpha22&permissions=0
/// 
/// </summary>

void HandleUserLogin(object sender, UserData userData)
{
    UserData dbUserData = databaseInterface.ReadSingleUserDataFromDatabase(userData.userName);
    AuthenticationData authenticationData = new AuthenticationData();
    if (dbUserData.permissions == 0)
    {
        authenticationData.isAuthenticated = true;
        authenticationData.isAdmin = false;
    }
    else if(dbUserData.permissions == 1)
    {
        authenticationData.isAuthenticated = true;
        authenticationData.isAdmin = true;
    }
    else
    {
        authenticationData.isAuthenticated = false;
        authenticationData.isAdmin = false;
    }
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
    List<UserData> allUserData = databaseInterface.ReadAllUserDataFromDatabase();
    List<UserData> users = new List<UserData>();
    foreach (UserData userData in allUserData)
    {
        if(userData.permissions == 0)
        {
            users.Add(userData);
        }
    }
    if (users.Count > 0)
    {
        commandSuccesful = true;
    }
    webServer.DoneWithUserDataList(commandSuccesful, users);
}

void HandleReadAllAdminDataFromDatabase(object sender, EventArgs e)
{
    bool commandSuccesful = false;
    List<UserData> allUserData = databaseInterface.ReadAllUserDataFromDatabase();
    List<UserData> admins = new List<UserData>();
    foreach (UserData userData in allUserData)
    {
        if (userData.permissions == 1)
        {
            admins.Add(userData);
        }
    }
    if (admins.Count > 0)
    {
        commandSuccesful = true;
    }
    webServer.DoneWithUserDataList(commandSuccesful, admins);
}

void HandleReadWeatherDataSetFromDatabase(object sender, Counties county)
{
    bool commandSuccesful = false;
    List<WeatherData> theData = databaseInterface.ReadWeatherDataSetFromDatabase(county);
    if (theData != null)
    {
        commandSuccesful = true;
    }
    webServer.DoneCommandWithWeatherData(commandSuccesful, theData);
}

void HandleRemoveUserDataFromDatabase(object sender, UserData theData)
{
    bool commandSuccesful;
    commandSuccesful = databaseInterface.RemoveUserDataFromDatabase(theData.userName);
    webServer.DoneNoData(commandSuccesful);
}

void HandleRemoveWeatherDataFromDatabase(object sender, WeatherData theData)
{
    bool commandSuccesful;
    commandSuccesful = databaseInterface.RemoveWeatherDataFromDatabase(theData.reporterId, theData.date);
    webServer.DoneNoData(commandSuccesful);
}

void HandleWeatherPrediction(object sender, WeatherData theData)
{
    DataWeatherPrediction thePrediction;
    List<WeatherData> theHistoricalData = databaseInterface.ReadWeatherDataSetFromDatabase(theData.county);
    thePrediction = weatherPredictor.makePrediction(theHistoricalData, theData);
    webServer.ReturnPrediction(thePrediction);
}


