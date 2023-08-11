using System.Text;
using System.Data.SqlTypes;
using Microsoft.Data.Sqlite;

namespace SQLiteDemo
{
    class Program
    {

        static void Main(string[] args)
        {
            SqliteConnection sqlite_conn;
            sqlite_conn = CreateConnection(); // Creates the initial connection
            CreateTable(sqlite_conn);       //creates a new table using a custom function
            InsertData(sqlite_conn);        //Inserts data into a table using a custom function
            ReadData(sqlite_conn);          //Reads all the data out of a table
        }

        static SqliteConnection CreateConnection()
        {

            SqliteConnection sqlite_conn;
            // Create a new database connection string, this is apparently an easier way to do it and means you can't get it wrong
            //"Data Source=database.db;Version = 3; New = True; Compress = True; "
            SqliteConnectionStringBuilder connectiomStringBuilder = new SqliteConnectionStringBuilder("Data Source=database.db;");
            string connectionString = connectiomStringBuilder.ToString();

            // Create a new database connection:
            sqlite_conn = new SqliteConnection(connectionString);

            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            //Catach any errors
            catch (Exception ex)
            {

            }
            //Return the connection
            return sqlite_conn;
        }


        /// <summary>
        /// This Function Cretes 2 new SQL Database Table using commands
        /// It creates the first table with 2 collumns, the first is char with 20 character limit and the second a collumn with Ints
        /// Then creates a Second Identical Table with a differnet name
        /// </summary>
        /// <param name="conn"></param>
        static void CreateTable(SqliteConnection conn)
        {

            SqliteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE SampleTable(Col1 VARCHAR(20), Col2 INT)";
           string Createsql1 = "CREATE TABLE SecondTable(Col1 VARCHAR(20), Col2 INT)";
           sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = Createsql1;
            sqlite_cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// THis Function Inserts Test data into the two test tables that were created in the above Function
        /// </summary>
        /// <param name="conn"></param>
        static void InsertData(SqliteConnection conn)
        {
            SqliteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "INSERT INTO SampleTable(Col1, Col2) VALUES('Test Text ', 1); ";
           sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "INSERT INTO SampleTable(Col1, Col2) VALUES('Test1 Text1 ', 2); ";
           sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "INSERT INTO SampleTable(Col1, Col2) VALUES('Test2 Text2 ', 3); ";
           sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = "INSERT INTO SecondTable(Col1, Col2) VALUES('Test3 Text3 ', 3); ";
           sqlite_cmd.ExecuteNonQuery();

        }

        /// <summary>
        /// This function reads out all of the data in the database
        /// </summary>
        /// <param name="conn"></param>
        static void ReadData(SqliteConnection conn)
        {
            SqliteDataReader sqlite_datareader;
            SqliteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT * FROM SampleTable";

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