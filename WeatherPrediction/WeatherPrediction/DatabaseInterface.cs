using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace WeatherPrediction
{
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
            ReadSingleUserDataFromDatabase("TestAdmin1");
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
                    sqlite_cmd.CommandText = "INSERT INTO " + userTable + "(Username, Password, Permission) VALUES(" + userName + "," + password + "," + permissions + "); ";
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

            try
            {
                int countyInt = ((int)county);          //We convert the Enum to an Int because we can only store simple data types in the database
                int conditionInt = ((int)condition);    //We convert the Enum to an Int because we can only store simple data types in the database
                SqliteCommand sqlite_cmd;
                sqlite_cmd = weatherSQLConnection.CreateCommand();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES(" + userName + "," + temperature + "," + pressure + "," + humidity + "," + windSpeed + "," + date + "," + countyInt + "," + conditionInt + "); ";
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

        //SB: NOT COMPLETE
        public UserData ReadSingleUserDataFromDatabase(string username)
        {
            UserData userData = new UserData();
            List<string> theData = new List<string>();
            theData = findUser(username);

            userData.userName = theData[0];
            userData.password = theData[1];
            userData.permissions = int.Parse(theData[2]);
            return userData;

        }

        //SB: NOT COMPLETE
        public string ReadAllUserDataFromDatabase()
        {
            return "";

        }

        //SB: NOT COMPLETE
        public string ReadWeatherDataSetFromDatabase(string county)
        {
            return "";
        }

        //SB: NOT COMPLETE
        public bool RemoveUserDataFromDatabase(string userName)
        {
            bool commandSuccessful;

            commandSuccessful = true;

            return commandSuccessful;
        }

        //SB: NOT COMPLETE
        public bool RemoveWeatherDataFromDatabase(string userName, int date)
        {
            bool commandSuccessful;

            commandSuccessful = true;

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

            Console.WriteLine("User Data Size Is: " + userData.Count);

            foreach (string nextValue in userData)
            {
                Console.WriteLine("User Data For " + theUserName + " is: " + nextValue);
            }

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
                // Column 2: Temperature    (Double)
                // Column 3: Pressure       (Double)
                // Column 4: Humidity       (Int)
                // Column 5: Windspeed      (Double)
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

                SqliteCommand sqlite_cmd;
                sqlite_cmd = conn.CreateCommand();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin1', 21.1, 1002.1, 74, 5.1, 1691743250, '12', '0'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin1', 20.5, 1002.4, 76, 5.3, 1691742500, '12', '0'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin1', 20.1, 1001.1, 79, 5.2, 1691742000, '12', '1'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin2', 21.0, 1002.1, 74, 5.3, 1691743250, '12', '0'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin2', 20.4, 1001.1, 69, 5.0, 1691743290, '12', '1'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin2', 20.6, 1001.3, 72, 5.1, 1691743350, '12', '2'); ";
                sqlite_cmd.ExecuteNonQuery();
                sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('TestAdmin2', 20.8, 1001.2, 74, 4.9, 1691743399, '12', '0'); ";
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
    }
}
