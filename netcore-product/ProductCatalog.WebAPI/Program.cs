using System;
using System.Data.SqlClient;
namespace MSSQLConnectionCheck
{
    class Program
    {
        static void Main(string[] args)
        {
            // Replace 'YOUR_MSSQL_SERVER' with the actual server name or IP address
            string serverName = "localhost";
            string databaseName = "ProductDatabase"; // Replace with the name of your database
            string username = "sa"; // Replace with the username
            string password = "Test-hackerearth"; // Replace with the password
            string connectionString = $"Data Source={serverName};Initial Catalog={databaseName};User ID={username};Password={password};";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connection successful! You are connected to the MSSQL server.");
                    connection.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }
        }
    }
}