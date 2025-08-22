using System;
using System.Configuration;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace MedicalAppointmentBookingSys;

internal static class Program
{
    public static string connectionString = ConfigurationManager.ConnectionStrings["MedicalDB"].ConnectionString;

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
                                        IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = 'MedicalDB')
                                        CREATE DATABASE MedicalDB;";
                using (SqlCommand sqlCommand = new SqlCommand(createDbQuery, connection))
                {
                    sqlCommand.ExecuteNonQuery();
                }
                string createTablesQuery = @"
                                            USE MedicalDB;
                                            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Doctors' AND xtype='U')
                                            CREATE TABLE Doctors (
                                            DoctorID INT PRIMARY KEY IDENTITY(1,1),
                                            FullName VARCHAR(100) NOT NULL,
                                            Specialty VARCHAR(100) NOT NULL,
                                            Availability BIT DEFAULT 1);

                                            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Patients' AND xtype='U')
                                            CREATE TABLE Patients (
                                            PatientID INT PRIMARY KEY IDENTITY(1,1),
                                            FullName VARCHAR(100) NOT NULL,                        
                                            Email VARCHAR(100) NOT NULL );
                                            
                                            IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Appointments' AND xtype='U')
                                            CREATE TABLE Appointments (
                                            AppointmentID INT PRIMARY KEY IDENTITY(1,1),
                                            DoctorID INT FOREIGN KEY REFERENCES Doctors(DoctorID),
                                            PatientID INT FOREIGN KEY REFERENCES Patients(PatientID),
                                            AppointmentDate DATETIME NOT NULL,
                                            Notes VARCHAR(500));";

                using (SqlCommand sqlCommand2 = new SqlCommand(createTablesQuery, connection))
                {
                    sqlCommand2.ExecuteNonQuery();
                }
                string insertDataQuery = @" 
                                        USE MedicalDB;
                                        IF NOT EXISTS (SELECT * FROM Doctors)
                                        INSERT INTO Doctors (FullName, Specialty, Availability) VALUES
                                        ('Dr. Bill Kley', 'Cardiology', 1),
                                        ('Dr. Karl Meil', 'Neurosurgery', 1),
                                        ('Dr. Jo Nisa', 'Nutrition', 0);

                                        IF NOT EXISTS (SELECT * FROM Patients)
                                        INSERT INTO Patients (FullName, Email) VALUES
                                        ('Wayne Lepp', 'a@email.com'),
                                        ('Pil Juje', 'b@email.com'),
                                        ('Hali Shalagh', 'c@email.com');";

                using (SqlCommand cmd = new SqlCommand(insertDataQuery, connection))
                {
                    cmd.ExecuteNonQuery();
                }
                MessageBox.Show("Database MedicalDB created successfully", "Success", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
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
