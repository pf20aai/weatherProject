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
        private string userTable = "UserTable";

        //Constructor
        public DatabaseInterface()
        {
            weatherSQLConnection = CreateConnection();
            CreateInitialTables(weatherSQLConnection);
            InsertTestData(weatherSQLConnection);
            Console.WriteLine("Database Setup");
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

        //SB: This will need to have an agreed format whenever data is being entered into the database so that it can be standardised 
        public bool AddUserDataToDatabase(string userName, string email, string password, int permissions)
        {
            try
            {
                SqliteCommand sqlite_cmd;
                sqlite_cmd = weatherSQLConnection.CreateCommand();
                sqlite_cmd.CommandText = "INSERT INTO " + userTable + "(Username, Email, Password, Permission) VALUES(" + userName + "," + email + "," + password + "," + permissions +"); ";
                sqlite_cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        //SB: This will need to have an agreed format whenever data is being entered into the database so that it can be standardised 
        public bool AddWeatherDataToDatabase(string userName, string email, string password, int permissions)
        {
            try
            {
                SqliteCommand sqlite_cmd;
                sqlite_cmd = weatherSQLConnection.CreateCommand();
                sqlite_cmd.CommandText = "INSERT INTO " + userTable + "(Username, Email, Password, Permission) VALUES(" + userName + "," + email + "," + password + "," + permissions + "); ";
                sqlite_cmd.ExecuteNonQuery();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public string ReadUserDataFromDatabase(string username, string password)
        {
            return "";

        }

        public string[] ReadWeatherDataSetFromDatabase(string county)
        {

            //SB: This will probably be better as a list as we will not know the actual size of the data that is being sent back
            string[] dataSet = new string[1];
            return dataSet;
        }

        /// <summary>
        /// This Function Cretes 2 new SQL Database Table using commands
        /// It creates the first table with 2 collumns, the first is char with 20 character limit and the second a collumn with Ints
        /// Then creates a Second Identical Table with a differnet name
        /// </summary>
        /// <param name="conn"></param>
        private void CreateInitialTables(SqliteConnection connection)
        {

            SqliteCommand sqlite_cmd;
            // Column 1: Reporter Id    (String)
            // Column 2: Temperature    (Double)
            // Column 3: Pressure       (Double)
            // Column 4: Humidity       (Int)
            // Column 5: Windspeed      (Double)
            // Column 6: Date           (Int)
            // Column 7: County         (String)
            // Column 8: Condition      (String)(Enum)
            string weatherTableCommand = "CREATE TABLE " + weatherTable + "(Reporter TEXT, Temperature REAL, Pressure REAL, Humidity INT, Windspeed REAL, Date DATE, County TEXT, Condition TEXT )";

            // Column 1: Unsername      (String)
            // Column 2: Email          (String)
            // Column 3: Password       (String)
            // Column 4: Permission     (Int)
            string userTableCommand = "CREATE TABLE "+ userTable + "(Username TEXT, Email TEXT, Password TEXT, Permission INT)";

            sqlite_cmd = connection.CreateCommand();

            sqlite_cmd.CommandText = weatherTableCommand;
            sqlite_cmd.ExecuteNonQuery();

            sqlite_cmd.CommandText = userTableCommand;
            sqlite_cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// This Function Inserts Test data into the Table for the purposes of creating test predictions
        /// </summary>
        /// <param name="conn"></param>
        private void InsertTestData(SqliteConnection conn)
        {
            //This is a Skeleton Line for adding new Weather Data To The Table
            //sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('Test User', 21.1, 1002.1, 76, 5.3, 1691743250, 'Essex', 'Cloudy'); ";
            //sqlite_cmd.ExecuteNonQuery();

            //This is a Skeleton Line for adding new User Data To The Table
            //sqlite_cmd.CommandText = "INSERT INTO " + userTable + "(Username, Email, Password, Permission) VALUES('TestUser', 'Test@Test.com' , 'Password1', 1); ";
            //sqlite_cmd.ExecuteNonQuery();

            SqliteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('Test User', 21.1, 1002.1, 74, 5.3, 1691743250, 'Essex', 'Sunny'); ";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('Test User', 20.5, 1002.4, 76, 5.3, 1691742500, 'Essex', 'Sunny'); ";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "INSERT INTO " + weatherTable + "(Reporter, Temperature, Pressure, Humidity, Windspeed, Date, County, Condition) VALUES('Test User', 20.1, 1001.1, 79, 5.3, 1691742000, 'Essex', 'Cloudy'); ";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "INSERT INTO " + userTable + "(Username, Email, Password, Permission) VALUES('TestAdmin', 'TestAdmin@Test.com' , 'Admin', 1); ";
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "INSERT INTO " + userTable + "(Username, Email, Password, Permission) VALUES('TestUser', 'Test@Test.com' , 'Password1', 0); ";
            sqlite_cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// This function reads out all of the data in the database
        /// </summary>
        /// <param name="conn"></param>
        private void ReadData(SqliteConnection conn , String table)
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
