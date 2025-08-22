using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace MedicalAppointmentBookingSys;

public class ModifyAppointmentForm : Form
{
    private int appointmentId;

    private IContainer components = null;

    private DateTimePicker dateTimePicker2;

    private TextBox notes;

    private Button submitButton;

    public ModifyAppointmentForm(int appointmentId, DateTime currentDate, string currentNotes)
    {
        this.appointmentId = appointmentId;
        InitializeComponent();
        dateTimePicker2.Value = currentDate;
        notes.Text = currentNotes;
    }

    private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
    {
    }

    private void submitButton_Click(object sender, EventArgs e)
    {
        DateTime newDate = dateTimePicker2.Value;
        string newNotes = notes.Text;
        SqlConnection connection = new SqlConnection(Program.connectionString);
        using (connection)
        {
            try
            {
                connection.Open();
                string modQuery = "UPDATE Appointments SET AppointmentDate=@Date, Notes=@Notes WHERE AppointmentID=@ID";
                using SqlCommand cmd = new SqlCommand(modQuery, connection);
                cmd.Parameters.AddWithValue("@Date", newDate);
                cmd.Parameters.AddWithValue("@Notes", newNotes);
                cmd.Parameters.AddWithValue("@ID", appointmentId);
                cmd.ExecuteNonQuery();
                MessageBox.Show("Appointment updated successfully!");
                base.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating appointment: " + ex.Message);
            }
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
        this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
        this.notes = new System.Windows.Forms.TextBox();
        this.submitButton = new System.Windows.Forms.Button();
        base.SuspendLayout();
        this.dateTimePicker2.Location = new System.Drawing.Point(65, 90);
        this.dateTimePicker2.Name = "dateTimePicker2";
        this.dateTimePicker2.Size = new System.Drawing.Size(200, 23);
        this.dateTimePicker2.TabIndex = 0;
        this.dateTimePicker2.ValueChanged += new System.EventHandler(dateTimePicker2_ValueChanged);
        this.notes.Location = new System.Drawing.Point(65, 157);
        this.notes.Name = "notes";
        this.notes.Size = new System.Drawing.Size(200, 23);
        this.notes.TabIndex = 1;
        this.submitButton.Location = new System.Drawing.Point(121, 219);
        this.submitButton.Name = "submitButton";
        this.submitButton.Size = new System.Drawing.Size(75, 23);
        this.submitButton.TabIndex = 2;
        this.submitButton.Text = "Submit";
        this.submitButton.UseVisualStyleBackColor = true;
        this.submitButton.Click += new System.EventHandler(submitButton_Click);
        base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 15f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        base.ClientSize = new System.Drawing.Size(322, 450);
        base.Controls.Add(this.submitButton);
        base.Controls.Add(this.notes);
        base.Controls.Add(this.dateTimePicker2);
        base.Name = "ModifyAppointmentForm";
        this.Text = "Modify Appointment";
        base.ResumeLayout(false);
        base.PerformLayout();
    }
}
