using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace MedicalAppointmentBookingSys;

public class ManageAppointmentsForm : Form
{
    private IContainer components = null;

    private DataGridView dataGridViewAppointments;

    private TextBox SearchBox;

    private Button searchButton;

    private DataGridViewButtonColumn Delete;

    private DataGridViewButtonColumn Modify;

    public ManageAppointmentsForm()
    {
        InitializeComponent();
        LoadAppointments();
    }

    private void LoadAppointments(string patientNameFilter = "")
    {
        SqlConnection connection = new SqlConnection(Program.connectionString);
        try
        {
            string filterQuery = @"
                                SELECT a.AppointmentID, d.FullName AS Doctor, p.FullName AS Patient, a.AppointmentDate, a.Notes
                                FROM Appointments a
                                JOIN Doctors d ON a.DoctorID = d.DoctorID
                                JOIN Patients p ON a.PatientID = p.PatientID
                                WHERE (@PatientName = '' OR p.FullName LIKE '%' + @PatientName + '%')
                                
ORDER BY a.AppointmentDate";
            SqlCommand command = new SqlCommand(filterQuery, connection);
            using (connection)
            {
                connection.Open();
                command.Parameters.AddWithValue("@PatientName", patientNameFilter);
                using SqlDataReader reader = command.ExecuteReader();
                DataTable dt = new DataTable();
                dt.Load(reader);
                dataGridViewAppointments.DataSource = dt;
                dataGridViewAppointments.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridViewAppointments.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
        }
        catch (Exception e)
        {
            MessageBox.Show("Error loading appointments: " + e.Message);
        }
        finally
        {
            connection.Close();
        }
    }

    private void SearchBox_TextChanged(object sender, EventArgs e)
    {
        string searchText = SearchBox.Text.Trim();
        LoadAppointments(searchText);
    }

    private void searchButton_Click(object sender, EventArgs e)
    {
        string searchText = SearchBox.Text.Trim();
        LoadAppointments(searchText);
    }

    private void dataGridViewAppointments_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }
        int appointmentId = (int)dataGridViewAppointments.Rows[e.RowIndex].Cells["AppointmentID"].Value;
        if (dataGridViewAppointments.Columns[e.ColumnIndex].Name == "Delete")
        {
            DialogResult confirm = MessageBox.Show("Are you sure you want to delete this appointment?", "Confirm Delete", MessageBoxButtons.YesNo);
            if (confirm == DialogResult.Yes)
            {
                using (SqlConnection conn = new SqlConnection(Program.connectionString))
                {
                    conn.Open();
                    using SqlCommand cmd = new SqlCommand("DELETE FROM Appointments WHERE AppointmentID = @ID", conn);
                    cmd.Parameters.AddWithValue("@ID", appointmentId);
                    cmd.ExecuteNonQuery();
                }
                LoadAppointments(SearchBox.Text.Trim());
            }
        }
        if (dataGridViewAppointments.Columns[e.ColumnIndex].Name == "Modify")
        {
            DateTime currentDate = (DateTime)dataGridViewAppointments.Rows[e.RowIndex].Cells["AppointmentDate"].Value;
            string currentNotes = dataGridViewAppointments.Rows[e.RowIndex].Cells["Notes"].Value.ToString();
            ModifyAppointmentForm modifyForm = new ModifyAppointmentForm(appointmentId, currentDate, currentNotes);
            if (modifyForm.ShowDialog() == DialogResult.OK)
            {
                LoadAppointments(SearchBox.Text.Trim());
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
        this.dataGridViewAppointments = new System.Windows.Forms.DataGridView();
        this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
        this.Modify = new System.Windows.Forms.DataGridViewButtonColumn();
        this.SearchBox = new System.Windows.Forms.TextBox();
        this.searchButton = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)this.dataGridViewAppointments).BeginInit();
        base.SuspendLayout();
        this.dataGridViewAppointments.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridViewAppointments.Columns.AddRange(this.Delete, this.Modify);
        this.dataGridViewAppointments.Location = new System.Drawing.Point(12, 34);
        this.dataGridViewAppointments.Name = "dataGridViewAppointments";
        this.dataGridViewAppointments.Size = new System.Drawing.Size(776, 404);
        this.dataGridViewAppointments.TabIndex = 0;
        this.dataGridViewAppointments.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(dataGridViewAppointments_CellContentClick);
        this.Delete.HeaderText = "Delete";
        this.Delete.Name = "Delete";
        this.Modify.HeaderText = "Modify";
        this.Modify.Name = "Modify";
        this.SearchBox.Location = new System.Drawing.Point(12, 5);
        this.SearchBox.Name = "SearchBox";
        this.SearchBox.Size = new System.Drawing.Size(691, 23);
        this.SearchBox.TabIndex = 1;
        this.SearchBox.TextChanged += new System.EventHandler(SearchBox_TextChanged);
        this.searchButton.Location = new System.Drawing.Point(709, 5);
        this.searchButton.Name = "searchButton";
        this.searchButton.Size = new System.Drawing.Size(75, 23);
        this.searchButton.TabIndex = 2;
        this.searchButton.Text = "Search";
        this.searchButton.UseVisualStyleBackColor = true;
        this.searchButton.Click += new System.EventHandler(searchButton_Click);
        base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 15f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        base.ClientSize = new System.Drawing.Size(800, 450);
        base.Controls.Add(this.searchButton);
        base.Controls.Add(this.SearchBox);
        base.Controls.Add(this.dataGridViewAppointments);
        base.Name = "ManageAppointmentsForm";
        this.Text = "Appointment Management";
        ((System.ComponentModel.ISupportInitialize)this.dataGridViewAppointments).EndInit();
        base.ResumeLayout(false);
        base.PerformLayout();
    }
}
