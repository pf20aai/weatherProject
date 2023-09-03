using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace WeatherPrediction
{
    /// <summary>
    /// This class is responsible for handling all of the databsse requests from the rest of the program
    /// It allows for the creation, Deletion, Update and reading of data from the database
    /// The database is based on SQLite so it compatible with most SQL Commands which is what is used to interface to the database
    /// 
    /// On start up the program will instanciate this class which will open up a connection to the database and check if it is populated with test data.
    /// If there is no test data it will create the tables and populate them with test data
    /// </summary>
    public class DatabaseInterface
    {
        //Class Attributes
        private SqliteConnection weatherSQLConnection;
        private string weatherTable = "WeatherTable";
        private int weatherTableNumberOfColumns = 7; //This is the number of columns in the table Starting from 0
        private string userTable = "UserTable";
        private int userTableNumberOfColumns = 2;   //This is the number of columns in the table Starting from 0
        private bool tablesPreInitialised = false;

        //Constructor
        public DatabaseInterface()
        {
            weatherSQLConnection = CreateConnection();
            tablesPreInitialised = CreateInitialTables(weatherSQLConnection);
            InsertTestData(weatherSQLConnection);
            Console.WriteLine("Database Setup");

            //SB: Put any Database Test lines Here
            //RemoveWeatherDataFromDatabase("Bobby" , 54321);

        }

        //Database Functions
        private SqliteConnection CreateConnection()
        {
            Console.WriteLine("Creating Database Connection");
            SqliteConnection sqlite_conn;

            // Create a new database connection string, this is apparently an easier way to do it and means you can't get it wrong
            //"Data Source=database.db;Version = 3; New = True; Compress = True; "
            SqliteConnectionStringBuilder connectiomStringBuilder = new SqliteConnectionStringBuilder("Data Source=WeatherDatabase.db;");
            string connectionString = connectiomStringBuilder.ToString();

            // Create a new database connection:
            sqlite_conn = new SqliteConnection(connectionString);
            Console.WriteLine("Opening Connection");
            // Open the connection:
            try
            {
                sqlite_conn.Open();
                Console.WriteLine("Connection Opened Successfully");
            }
            //Catach any errors
            catch (Exception ex)
            {
                Console.WriteLine("Connection Failed:" + ex);
            }
            //Return the connection
            return sqlite_conn;
        }

        /// <summary>
        /// This adds a new user into the database with their permissions and password
        /// The Function will check the user does not already exists before adding anew set into the database
        /// The Function will return a true/false back to say whether the command was completed successfully
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public bool AddUserDataToDatabase(string userName, string password, int permissions)
        {
            bool commandSuccessful;

            try
            {
                //We will need to check if the user already exists first before we add it in
                //If a user did alread exist we need to report back a failure
                List<string> userData= findUser(userName);
                if (userData.Count == 0)
                {
                    SqliteCommand sqlite_cmd;
                    sqlite_cmd = weatherSQLConnection.CreateCommand();
                    sqlite_cmd.CommandText = "INSERT INTO " + userTable + "(Username, Password, Permission) VALUES('" + userName + "','" + password + "'," + permissions + "); ";
                    sqlite_cmd.ExecuteNonQuery();
                    commandSuccessful = true;
                }
                else
                {
                    Console.WriteLine("Failed To Execute Add User Command: User Alread Exists");
                    commandSuccessful = false;
                }
            }
            catch
            {
                Console.WriteLine("Failed To Execute Add User Command: SQL Failure");
                commandSuccessful = false;
            }

            return commandSuccessful;
        }
 

        /// <summary>
        /// This function adds a new set of weather data into the database
        /// The function will convert the enums into ints before they are passed to the database as the database can only hold basic types of data
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="temperature"></param>
        /// <param name="pressure"></param>
        /// <param name="humidity"></param>
        /// <param name="windSpeed"></param>
        /// <param name="date"></param>
        /// <param name="county"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool AddWeatherDataToDatabase(string userName, double temperature, double pressure, int humidity, double windSpeed, int date, Counties county, WeatherConditions condition)
        {

            bool commandSuccessful;

            //Note: This does not have to do the same check as the user add for if the data already exists because we have the time/date which will be unique down to the second
            try
            {
                int countyInt = ((int)county);          //We convert the Enum to an Int because we can only store simple data types in the database
                int conditionInt = ((int)condition);    //We convert the Enum to an Int because we can only store simple data types in the database
                SqliteCommand sqlite_cmd;
                sqlite_cmd = weatherSQLConnection.CreateCommand();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('" + userName + "'," + temperature + "," + pressure + "," + humidity + "," + windSpeed + "," + date + "," + countyInt + "," + conditionInt + "); ";
                sqlite_cmd.ExecuteNonQuery();
                commandSuccessful = true;
            }
            catch
            {
                Console.WriteLine("Failed To Execute Add Weather Data Command: SQL Failure");
                commandSuccessful = false;
            }

            return commandSuccessful;
        }

        //SB: Extention to break this out into 2 sperate functions, 1 called UpdateUserPassword and 1 called UpdateUserPermissions

        /// <summary>
        /// Updates a users information with the attributes provided, will return whether the operation was successful or not
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public bool UpdateUserDataToDatabase(string userName, string password, int permissions)
        {
            bool commandSuccessful;
            try
            {
                SqliteCommand sqlite_cmd;
                sqlite_cmd = weatherSQLConnection.CreateCommand();
                sqlite_cmd.CommandText = "UPDATE " + userTable + " SET Username = '" + userName + "', Password = '" + password + "', Permission = " + permissions + " WHERE Username = '" + userName + "';"; ;
                sqlite_cmd.ExecuteNonQuery();
                commandSuccessful = true;
            }
            catch
            {
                commandSuccessful = false;
                Console.WriteLine("Failed To Execute Update User Data Command: SQL Failure");
            }
            return commandSuccessful;
        }


        /// <summary>
        /// Updates a set of Weather data with the attributes provided, will return whether the operation was successful or not
        /// for this to work the date and userName must be the same as a matching piece of data in the database
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="temperature"></param>
        /// <param name="pressure"></param>
        /// <param name="humidity"></param>
        /// <param name="windSpeed"></param>
        /// <param name="date"></param>
        /// <param name="county"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool UpdateWeatherDataToDatabase(string userName, double temperature, double pressure, int humidity, double windSpeed, int date, Counties county, WeatherConditions condition)
        {
            bool commandSuccessful;
            try
            {
                int countyInt = ((int)county);          //We convert the Enum to an Int because we can only store simple data types in the database
                int conditionInt = ((int)condition);    //We convert the Enum to an Int because we can only store simple data types in the database
                SqliteCommand sqlite_cmd;
                sqlite_cmd = weatherSQLConnection.CreateCommand();
                string slqcommandtext = "UPDATE " + weatherTable + " SET Reporter = '" + userName + "', Temperature = " + temperature + ", Pressure = " + pressure + ", Humidity = " + humidity + ", Windspeed = " + windSpeed + ", Date = " + date + ", County = " + countyInt + ", Condition = " + conditionInt + " WHERE Reporter = '" + userName + "' AND Date = " + date + "; ";
                sqlite_cmd.CommandText = slqcommandtext;
                sqlite_cmd.ExecuteNonQuery();
                commandSuccessful = true;
            }
            catch
            {
                commandSuccessful = false;
                Console.WriteLine("Failed To Execute Update User Data Command: SQL Failure");
            }
            return commandSuccessful;
        }


        /// <summary>
        /// Reads a single users information from the database.
        /// This function does not do encryption or decryption that must be done in program
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public UserData ReadSingleUserDataFromDatabase(string username)
        {
            UserData userData = new UserData();
            List<string> theData = new List<string>();
            theData = findUser(username);

            try
            {
                userData.userName = theData[0];
                userData.password = theData[1];
                userData.permissions = int.Parse(theData[2]);
                return userData;
            }
            catch
            {
                return userData;
            }

        }

        public List <UserData> ReadAllUserDataFromDatabase()
        {
            List <UserData> listOfUsers = new List<UserData>();
            string getListOfUsers = "SELECT *" + " FROM " + userTable;
            SqliteCommand sqlite_cmd;
            sqlite_cmd = weatherSQLConnection.CreateCommand();
            sqlite_cmd.CommandText = getListOfUsers;
            SqliteDataReader sqliteDataReader = sqlite_cmd.ExecuteReader();

            while (sqliteDataReader.Read())
            {
                List<string> userDataAsString = new List<string>();
                UserData userData = new UserData();
                int column = 0;
                while (column <= userTableNumberOfColumns)
                {
                    userDataAsString.Add(sqliteDataReader.GetString(column));
                    column++;
                }
                userData.userName = userDataAsString[0];
                userData.password = userDataAsString[1];
                userData.permissions = int.Parse(userDataAsString[2]);
                listOfUsers.Add(userData);
            }

            return listOfUsers;

        }

        public List<WeatherData> ReadWeatherDataSetFromDatabase(Counties county)
        { 
            int countyInt = ((int)county); //We convert the Enum to an Int because we can only store simple data types in the database

            List<WeatherData> listOfWeatherData = new List<WeatherData>();
            string getWeatherForCounty = "SELECT *" + " FROM " + weatherTable + " WHERE County = '" + countyInt + "';";
            SqliteCommand sqlite_cmd;
            sqlite_cmd = weatherSQLConnection.CreateCommand();
            sqlite_cmd.CommandText = getWeatherForCounty;
            SqliteDataReader sqliteDataReader = sqlite_cmd.ExecuteReader();

            while (sqliteDataReader.Read())
            {
                List<string> weatherDataAsString = new List<string>();
                WeatherData weatherData = new WeatherData();
                int column = 0;
                while (column <= weatherTableNumberOfColumns)
                {
                    weatherDataAsString.Add(sqliteDataReader.GetString(column));
                    column++;
                }
                weatherData.reporterId = weatherDataAsString[0];
                weatherData.temperature = double.Parse(weatherDataAsString[1]);
                weatherData.pressure = double.Parse(weatherDataAsString[2]);
                weatherData.humidity = int.Parse(weatherDataAsString[3]);
                weatherData.windSpeed = double.Parse(weatherDataAsString[4]);
                weatherData.date = int.Parse(weatherDataAsString[5]);
                weatherData.county = (Counties)int.Parse(weatherDataAsString[6]);
                weatherData.WeatherCondition = (WeatherConditions)int.Parse(weatherDataAsString[7]);

                listOfWeatherData.Add(weatherData);
            }

            return listOfWeatherData;
        }

        public bool RemoveUserDataFromDatabase(string userName)
        {
            bool commandSuccessful;

            try
            {

                SqliteCommand sqlite_cmd;
                sqlite_cmd = weatherSQLConnection.CreateCommand();
                sqlite_cmd.CommandText = "DELETE FROM " + userTable + " WHERE Username = '" + userName + "';";
                sqlite_cmd.ExecuteNonQuery();
                commandSuccessful = true;
            }
            catch
            {
                commandSuccessful = false;
            }

            return commandSuccessful;
        }

        public bool RemoveWeatherDataFromDatabase(string userName, int date)
        {
            bool commandSuccessful;
            Console.WriteLine("Removing Weather From Database: " + userName + " " + date);

            try
            {

                SqliteCommand sqlite_cmd;
                sqlite_cmd = weatherSQLConnection.CreateCommand();
                sqlite_cmd.CommandText = "DELETE FROM " + weatherTable + " WHERE Reporter = '" + userName + "' AND Date = '" + date +  "';";
                sqlite_cmd.ExecuteNonQuery();
                commandSuccessful = true;
            }
            catch
            {
                commandSuccessful = false;
            }

            return commandSuccessful;
        }

        /// <summary>
        /// This function will find a user in the table and return the data for that type
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        private List<string> findUser(string theUserName)
        {
            List<string> userData = new List<string>();
            string findUserCommand = "SELECT *" + " FROM " + userTable + " WHERE userName = '" + theUserName + "';";
            SqliteCommand sqlite_cmd;
            sqlite_cmd = weatherSQLConnection.CreateCommand();
            sqlite_cmd.CommandText = findUserCommand;
            SqliteDataReader sqliteDataReader = sqlite_cmd.ExecuteReader();
            while (sqliteDataReader.Read())
            {
                int column = 0;
                while (column <= userTableNumberOfColumns)
                {
                    userData.Add(sqliteDataReader.GetString(column));
                    column++;
                }
            }
            sqliteDataReader.Close();

            return userData;
        }

        /// <summary>
        /// This Function Cretes 2 new SQL Database Table using commands
        /// It creates the first table with 2 collumns, the first is char with 20 character limit and the second a collumn with Ints
        /// Then creates a Second Identical Table with a differnet name
        /// </summary>
        /// <param name="conn"></param>
        private bool CreateInitialTables(SqliteConnection connection)
        {
            //First We Check That The Tables Don't Already Exist
            bool weatherTableInitialised = false;
            bool userTableInitialised = false;
            List<string> databaseTables = new List<string>();
            string findTableCommand = "SELECT name FROM sqlite_schema WHERE type='table'";
            SqliteCommand sqlite_cmd;
            sqlite_cmd = connection.CreateCommand();
            sqlite_cmd.CommandText = findTableCommand;
            SqliteDataReader sqliteDataReader = sqlite_cmd.ExecuteReader();
            while (sqliteDataReader.Read())
            {
                databaseTables.Add(sqliteDataReader.GetString(0));
            }
            sqliteDataReader.Close();

            if (!databaseTables.Contains("WeatherTable"))
            {
                Console.WriteLine("Weather Table Does Not Exist Creating Table");

                // Column 1: Reporter Id    (String)
                // Column 2: Temperature    (Double) In Celcius
                // Column 3: Pressure       (Double) In Millibar
                // Column 4: Humidity       (Int)    In Percentage
                // Column 5: Windspeed      (Double) In MPH
                // Column 6: Date           (Int)
                // Column 7: County         (String)
                // Column 8: Condition      (String)(Enum)
                string weatherTableCommand = "CREATE TABLE " + weatherTable + "(Reporter TEXT, Temperature REAL, Pressure REAL, Humidity INT, Windspeed REAL, Date DATE, County TEXT, Condition TEXT )";

                sqlite_cmd.CommandText = weatherTableCommand;
                sqlite_cmd.ExecuteNonQuery();


            }
            else
            {
                weatherTableInitialised = true;
            }

            if (!databaseTables.Contains("UserTable"))
            {
                Console.WriteLine("User Table Does Not Exist Creating Table");

                // Column 1: Unsername      (String)
                // Column 2: Password       (String)
                // Column 3: Permission     (Int)
                string userTableCommand = "CREATE TABLE " + userTable + "(Username TEXT, Password TEXT, Permission INT)";

                sqlite_cmd.CommandText = userTableCommand;
                sqlite_cmd.ExecuteNonQuery();

            }
            else
            {
                userTableInitialised = true;
            }
            Console.WriteLine("Database Tables Initialised");
            if (weatherTableInitialised && userTableInitialised)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// This Function Inserts Test data into the Table for the purposes of creating test predictions
        /// </summary>
        /// <param name="conn"></param>
        private void InsertTestData(SqliteConnection conn)
        {
            if (!tablesPreInitialised)
            {
                //This is a Skeleton Line for adding new Weather Data To The Table
                //sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin1', 21.1, 1002.1, 76, 5.3, 1691743250, 'Essex', 'Cloudy'); ";
                //sqlite_cmd.ExecuteNonQuery();

                //This is a Skeleton Line for adding new User Data To The Table
                //sqlite_cmd.CommandText = "INSERT INTO " + userTable + "(Username, Password, Permission) VALUES('TestUser' , 'Password1', 0); ";
                //sqlite_cmd.ExecuteNonQuery();

                //SB: now 1693554627 (1st September 9am)

                SqliteCommand sqlite_cmd;
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin1', 19.1, 1015.1, 56, 2.4, 1693767318, '12', '0'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin1', 18.3, 1015.4, 62, 7.2, 1693767018, '12', '1'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin1', 17.9, 1013.2, 67, 1.1, 1693762011, '12', '2'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin1', 21.5, 1015.1, 61, 5.1, 1693760003, '12', '1'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin1', 23.7, 1017.1, 56, 3.5, 1693758694, '12', '0'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin1', 19.3, 1013.9, 74, 2.1, 1693752058, '12', '3'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin1', 22.1, 1016.2, 60, 5.1, 1691743250, '12', '0'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin1', 20.5, 1015.4, 58, 5.3, 1691742500, '12', '0'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin1', 20.1, 1016.1, 65, 5.2, 1691742000, '12', '1'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin2', 21.0, 1017.4, 45, 5.3, 1691743250, '12', '0'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin2', 20.4, 1015.8, 69, 5.0, 1691743290, '12', '1'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin2', 20.6, 1015.3, 72, 5.1, 1691743350, '12', '2'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin2', 20.8, 1013.2, 62, 4.9, 1691743399, '12', '0'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + userTable + "(Username, Password, Permission) VALUES('TestAdmin1' , 'Admin12', 1); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + userTable + "(Username, Password, Permission) VALUES('TestAdmin2' , 'IBeAdmin', 1); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + userTable + "(Username, Password, Permission) VALUES('TestUser' , 'Password1', 0); ";
                sqlite_cmd.ExecuteNonQuery();
            }
            else
            {
                Console.WriteLine("Tables Were Preinitialised Test Data Already In Place");
            }

        }

        /// <summary>
        /// This function reads out all of the data in the database
        /// This is only going to be used for test purposes
        /// </summary>
        /// <param name="conn"></param>
        private void ReadData(SqliteConnection conn, String table)
        {
            SqliteDataReader sqlite_datareader;
            SqliteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM " + table;

            sqlite_datareader = sqlite_cmd.ExecuteReader();
            while (sqlite_datareader.Read())
            {
                string myreader = sqlite_datareader.GetString(0);
                Console.WriteLine(myreader);
            }
            conn.Close();
        }


        /// <summary>
        /// This is an XOR Encryption and Decryption Method
        /// Simply pass the string in and a key (In this case we use the Username Length) and it will either encrypt of decrypt for you
        /// Simple But Effective. The key MUST be the same for Encyption and Decryption
        /// </summary>
        static string EncryptDecrypt(string stringToConvert, int UsernameLength)
        {
            StringBuilder inputStringBuild = new StringBuilder(stringToConvert);
            StringBuilder outStringBuild = new StringBuilder(stringToConvert.Length);
            char stringAsChar;
            for (int iCount = 0; iCount < stringToConvert.Length; iCount++)
            {
                stringAsChar = inputStringBuild[iCount];
                stringAsChar = (char)(stringAsChar ^ UsernameLength);
                outStringBuild.Append(stringAsChar);
            }
            return outStringBuild.ToString();
        }
    }
}
