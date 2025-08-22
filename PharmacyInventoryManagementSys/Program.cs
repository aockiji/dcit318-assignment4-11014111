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
                string createDbQuery = @"
                                        IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'PharmacyDB')
                                        CREATE DATABASE PharmacyDB;";
                using (SqlCommand sqlCommand = new SqlCommand(createDbQuery, connection))
                {
                    sqlCommand.ExecuteNonQuery();
                }
                string createTablesQuery = @"
                                            USE PharmacyDB;
                                            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Medicines' AND xtype='U')
                                            CREATE TABLE Medicines (
                                            MedicineID INT PRIMARY KEY IDENTITY(1,1),
                                            Name VARCHAR(100) NOT NULL,
                                            Category VARCHAR(100) NOT NULL,
                                            Price DECIMAL(10, 2) NOT NULL,
                                            Quantity INT NOT NULL);

                                            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Sales' AND xtype='U')
                                            CREATE TABLE Sales (
                                            SaleID INT PRIMARY KEY IDENTITY(1,1),
                                            MedicineID INT FOREIGN KEY REFERENCES Medicines(MedicineID),
                                            QuantitySold INT NOT NULL,
                                            SaleDate DATETIME NOT NULL);";
                using (SqlCommand sqlCommand2 = new SqlCommand(createTablesQuery, connection))
                {
                    sqlCommand2.ExecuteNonQuery();
                }
                string insertDataQuery = @"
                                        USE PharmacyDB;
                                        IF NOT EXISTS (SELECT * FROM Medicines)
                                        INSERT INTO Medicines (Name, Category, Price, Quantity) VALUES
                                        ('Aspirin', 'Pain Killer', 12.2, 88),
                                        ('Ibupofren', 'Pain Killer', 7.6, 90),
                                        ('Paracetamol', 'Pain Killer', 6.0, 145);";
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
