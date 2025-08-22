using System;
using System.Configuration;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace PharmacyInventoryManagementSys;

internal static class Program
{
    public static string connectionString = ConfigurationManager.ConnectionStrings["PharmacyDB"].ConnectionString;

    [STAThread]
    private static void Main()
    {
        SqlConnection connection = new SqlConnection(connectionString);
        try
        {
            using (connection)
            {
                connection.Open();
                string createDbQuery = "\r\n                    IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'PharmacyDB')\r\n                    CREATE DATABASE PharmacyDB;";
                using (SqlCommand sqlCommand = new SqlCommand(createDbQuery, connection))
                {
                    sqlCommand.ExecuteNonQuery();
                }
                string createTablesQuery = "\r\n                    USE PharmacyDB;\r\n                    \r\n                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Medicines' AND xtype='U')\r\n                    CREATE TABLE Medicines (\r\n                        MedicineID INT PRIMARY KEY IDENTITY(1,1),\r\n                        Name VARCHAR(100) NOT NULL,\r\n                        Category VARCHAR(100) NOT NULL,\r\n                        Price DECIMAL(10, 2) NOT NULL,\r\n                        Quantity INT NOT NULL\r\n                    );\r\n\r\n                    IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Sales' AND xtype='U')\r\n                    CREATE TABLE Sales (\r\n                        SaleID INT PRIMARY KEY IDENTITY(1,1),\r\n                        MedicineID INT FOREIGN KEY REFERENCES Medicines(MedicineID),\r\n                        QuantitySold INT NOT NULL,\r\n                        SaleDate DATETIME NOT NULL\r\n                    );";
                using (SqlCommand sqlCommand2 = new SqlCommand(createTablesQuery, connection))
                {
                    sqlCommand2.ExecuteNonQuery();
                }
                string insertDataQuery = "\r\n                    USE PharmacyDB;\r\n                    \r\n                    IF NOT EXISTS (SELECT * FROM Medicines)\r\n                    INSERT INTO Medicines (Name, Category, Price, Quantity) VALUES\r\n                    ('Aspirin', 'Pain Killer', 12.2, 88),\r\n                    ('Ibupofren', 'Pain Killer', 7.6, 90),\r\n                    ('Paracetamol', 'Pain Killer', 6.0, 145);";
                using (SqlCommand cmd = new SqlCommand(insertDataQuery, connection))
                {
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Database PharmacyDB created successfully with sample data.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        catch (Exception e)
        {
            MessageBox.Show("Error creating database: " + e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
        }
        finally
        {
            connection.Close();
        }
        Application.Run(new MainForm());
    }
}
