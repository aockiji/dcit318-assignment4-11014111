using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace MedicalAppointmentBookingSys;

public class AppointmentForm : Form
{
    private IContainer components = null;

    private ComboBox docsComboBox;

    private ComboBox patientsComboBox;

    private DateTimePicker dateTimePicker1;

    private TextBox notesTextBox;

    private Button submitButton;

    public AppointmentForm()
    {
        InitializeComponent();
        LoadDocsAndPatients();
    }

    private void LoadDocsAndPatients()
    {
        SqlConnection connection = new SqlConnection(Program.connectionString);
        try
        {
            using (connection)
            {
                connection.Open();
                using (SqlCommand sqlCommand = new SqlCommand("SELECT DoctorID, FullName FROM Doctors WHERE Availability = 1", connection))
                {
                    using SqlDataReader reader2 = sqlCommand.ExecuteReader();
                    DataTable dtDoctors = new DataTable();
                    dtDoctors.Load(reader2);
                    docsComboBox.DataSource = dtDoctors;
                    docsComboBox.DisplayMember = "FullName";
                    docsComboBox.ValueMember = "DoctorID";
                }
                using SqlCommand cmd = new SqlCommand("SELECT PatientID, FullName FROM Patients", connection);
                using SqlDataReader reader = cmd.ExecuteReader();
                DataTable dtPatients = new DataTable();
                dtPatients.Load(reader);
                patientsComboBox.DataSource = dtPatients;
                patientsComboBox.DisplayMember = "FullName";
                patientsComboBox.ValueMember = "PatientID";
            }
        }
        catch (Exception e)
        {
            MessageBox.Show("Error loading doctors/patients: " + e.Message);
        }
        finally
        {
            connection.Close();
        }
    }

    private void submitButton_Click(object sender, EventArgs e)
    {
        int doctorId = (int)docsComboBox.SelectedValue;
        int patientId = (int)patientsComboBox.SelectedValue;
        DateTime appointmentDate = dateTimePicker1.Value;
        string notes = notesTextBox.Text;
        SqlConnection connection = new SqlConnection(Program.connectionString);
        try
        {
            using (connection)
            {
                connection.Open();
                string insertQuery = "INSERT INTO Appointments (DoctorID, PatientID, AppointmentDate, Notes) VALUES (@DoctorID, @PatientID, @Date, @Notes)";
                using SqlCommand cmd = new SqlCommand(insertQuery, connection);
                SqlParameter p1 = new SqlParameter("@DoctorID", SqlDbType.Int);
                p1.Value = doctorId;
                p1.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(p1);
                SqlParameter p2 = new SqlParameter("@PatientID", SqlDbType.Int);
                p2.Value = patientId;
                p2.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(p2);
                SqlParameter p3 = new SqlParameter("@Date", SqlDbType.DateTime);
                p3.Value = appointmentDate;
                if (appointmentDate < DateTime.Now)
                {
                    MessageBox.Show("Appointment date must be in the future.");
                    return;
                }
                p3.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(p3);
                string checkAvailabilityQuery = "SELECT Availability FROM Doctors WHERE DoctorID = @DoctorID";
                using (SqlCommand checkCmd = new SqlCommand(checkAvailabilityQuery, connection))
                {
                    checkCmd.Parameters.AddWithValue("@DoctorID", doctorId);
                    object result = checkCmd.ExecuteScalar();
                    if (result == null || !(bool)result)
                    {
                        MessageBox.Show("This doctor is not available for booking.");
                        return;
                    }
                }
                SqlParameter p4 = new SqlParameter("@Notes", SqlDbType.VarChar, 500);
                p4.Value = notes;
                p4.Direction = ParameterDirection.Input;
                cmd.Parameters.Add(p4);
                try
                {
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Appointment booked successfully!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error booking appointment: " + ex.Message);
                }
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show("Error connecting to database: " + exception.Message);
        }
        finally
        {
            connection.Close();
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.docsComboBox = new System.Windows.Forms.ComboBox();
        this.patientsComboBox = new System.Windows.Forms.ComboBox();
        this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
        this.notesTextBox = new System.Windows.Forms.TextBox();
        this.submitButton = new System.Windows.Forms.Button();
        base.SuspendLayout();
        this.docsComboBox.FormattingEnabled = true;
        this.docsComboBox.Location = new System.Drawing.Point(85, 66);
        this.docsComboBox.Name = "docsComboBox";
        this.docsComboBox.Size = new System.Drawing.Size(439, 23);
        this.docsComboBox.TabIndex = 0;
        this.patientsComboBox.FormattingEnabled = true;
        this.patientsComboBox.Location = new System.Drawing.Point(85, 112);
        this.patientsComboBox.Name = "patientsComboBox";
        this.patientsComboBox.Size = new System.Drawing.Size(439, 23);
        this.patientsComboBox.TabIndex = 1;
        this.dateTimePicker1.Location = new System.Drawing.Point(85, 165);
        this.dateTimePicker1.Name = "dateTimePicker1";
        this.dateTimePicker1.Size = new System.Drawing.Size(439, 23);
        this.dateTimePicker1.TabIndex = 2;
        this.notesTextBox.Location = new System.Drawing.Point(85, 213);
        this.notesTextBox.Name = "notesTextBox";
        this.notesTextBox.Size = new System.Drawing.Size(439, 23);
        this.notesTextBox.TabIndex = 3;
        this.submitButton.Location = new System.Drawing.Point(257, 262);
        this.submitButton.Name = "submitButton";
        this.submitButton.Size = new System.Drawing.Size(75, 23);
        this.submitButton.TabIndex = 4;
        this.submitButton.Text = "Submit";
        this.submitButton.UseVisualStyleBackColor = true;
        this.submitButton.Click += new System.EventHandler(submitButton_Click);
        base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 15f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        base.ClientSize = new System.Drawing.Size(589, 450);
        base.Controls.Add(this.submitButton);
        base.Controls.Add(this.notesTextBox);
        base.Controls.Add(this.dateTimePicker1);
        base.Controls.Add(this.patientsComboBox);
        base.Controls.Add(this.docsComboBox);
        base.Name = "AppointmentForm";
        this.Text = "Book Appointment";
        base.ResumeLayout(false);
        base.PerformLayout();
    }
}
