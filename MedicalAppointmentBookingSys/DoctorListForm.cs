using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace MedicalAppointmentBookingSys;

public class DoctorListForm : Form
{
    private IContainer components = null;

    private DataGridView dataGridViewDoctors;

    public DoctorListForm()
    {
        InitializeComponent();
        LoadDoctors();
    }

    private void LoadDoctors()
    {
        SqlConnection connection = new SqlConnection(Program.connectionString);
        try
        {
            SqlCommand command = new SqlCommand("SELECT DoctorID, FullName, Specialty, Availability FROM Doctors", connection);
            using (connection)
            {
                connection.Open();
                using SqlDataReader reader = command.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                dataGridViewDoctors.DataSource = dt;
                dataGridViewDoctors.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridViewDoctors.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
        }
        catch (Exception e)
        {
            MessageBox.Show("Error loading doctors: " + e.Message);
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
        this.dataGridViewDoctors = new System.Windows.Forms.DataGridView();
        ((System.ComponentModel.ISupportInitialize)this.dataGridViewDoctors).BeginInit();
        base.SuspendLayout();
        this.dataGridViewDoctors.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridViewDoctors.Location = new System.Drawing.Point(12, 12);
        this.dataGridViewDoctors.Name = "dataGridViewDoctors";
        this.dataGridViewDoctors.Size = new System.Drawing.Size(776, 426);
        this.dataGridViewDoctors.TabIndex = 0;
        base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 15f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        base.ClientSize = new System.Drawing.Size(800, 450);
        base.Controls.Add(this.dataGridViewDoctors);
        base.Name = "DoctorListForm";
        this.Text = "List of Doctors";
        ((System.ComponentModel.ISupportInitialize)this.dataGridViewDoctors).EndInit();
        base.ResumeLayout(false);
    }
}
