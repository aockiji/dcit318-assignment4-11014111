using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.VisualBasic;

namespace PharmacyInventoryManagementSys;

public class MainForm : Form
{
    private IContainer components = null;

    private TextBox searchBox;

    private Button searchBtn;

    private Button viewAllBtn;

    private DataGridView dataGridViewMeds;

    private TextBox medNameBox;

    private TextBox priceBox;

    private TextBox categoryBox;

    private TextBox quantityBox;

    private Button addMedBtn;

    private Button updateBtn;

    private Button recSaleBtn;

    private Button button5;

    public MainForm()
    {
        InitializeComponent();
        GetAllMeds();
        SetPlaceholder(medNameBox, "Medicine Name");
        SetPlaceholder(categoryBox, "Category");
        SetPlaceholder(priceBox, "Price");
        SetPlaceholder(quantityBox, "Quantity");
    }

    private void viewAllBtn_Click_1(object sender, EventArgs e)
    {
        GetAllMeds();
    }

    private void GetAllMeds(string medNameFilter = "")
    {
        SqlConnection connection = new SqlConnection(Program.connectionString);
        try
        {
            string filterQuery = "\r\n                        SELECT MedicineID, Name, Category, Price, Quantity\r\n                        FROM Medicines\r\n                        WHERE (@SearchTerm = '' OR Name LIKE '%' + @SearchTerm + '%')";
            SqlCommand command = new SqlCommand(filterQuery, connection);
            command.Parameters.AddWithValue("@SearchTerm", medNameFilter);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            using (reader)
            {
                DataTable dt = new DataTable();
                dt.Load(reader);
                dataGridViewMeds.DataSource = dt;
                dataGridViewMeds.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dataGridViewMeds.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            }
        }
        catch (Exception e)
        {
            MessageBox.Show("Error loading medicines: " + e.Message);
        }
        finally
        {
            connection.Close();
        }
    }

    private void searchBtn_Click(object sender, EventArgs e)
    {
        GetAllMeds(searchBox.Text.Trim());
    }

    private void searchBox_TextChanged(object sender, EventArgs e)
    {
        GetAllMeds(searchBox.Text.Trim());
    }

    private void addMedBtn_Click(object sender, EventArgs e)
    {
        SqlConnection connection = new SqlConnection(Program.connectionString);
        string q = "\r\n                    INSERT INTO Medicines (Name, Category, Price, Quantity)\r\n                    VALUES (@Name, @Category, @Price, @Quantity)";
        SqlCommand command = new SqlCommand(q, connection);
        command.Parameters.AddWithValue("@Name", medNameBox.Text.Trim());
        command.Parameters.AddWithValue("@Category", categoryBox.Text.Trim());
        command.Parameters.AddWithValue("@Price", decimal.Parse(priceBox.Text.Trim()));
        command.Parameters.AddWithValue("@Quantity", int.Parse(quantityBox.Text.Trim()));
        try
        {
            connection.Open();
            command.ExecuteNonQuery();
            MessageBox.Show("Medicine added to inventory successfully");
            GetAllMeds();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Tried adding medicine but ran into an ERROORR " + ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }

    private void SetPlaceholder(TextBox box, string placeholder)
    {
        TextBox box2 = box;
        string placeholder2 = placeholder;
        box2.Text = placeholder2;
        box2.ForeColor = Color.Gray;
        box2.Enter += delegate
        {
            if (box2.Text == placeholder2)
            {
                box2.Text = "";
                box2.ForeColor = Color.Black;
            }
        };
        box2.Leave += delegate
        {
            if (string.IsNullOrWhiteSpace(box2.Text))
            {
                box2.Text = placeholder2;
                box2.ForeColor = Color.Gray;
            }
        };
    }

    private void dataGridViewMeds_SelectionChanged(object sender, EventArgs e)
    {
        if (dataGridViewMeds.CurrentRow != null)
        {
            medNameBox.ForeColor = Color.Black;
            categoryBox.ForeColor = Color.Black;
            priceBox.ForeColor = Color.Black;
            quantityBox.ForeColor = Color.Black;
            medNameBox.Text = dataGridViewMeds.CurrentRow.Cells["Name"].Value.ToString();
            categoryBox.Text = dataGridViewMeds.CurrentRow.Cells["Category"].Value.ToString();
            priceBox.Text = dataGridViewMeds.CurrentRow.Cells["Price"].Value.ToString();
            quantityBox.Text = dataGridViewMeds.CurrentRow.Cells["Quantity"].Value.ToString();
        }
    }

    private void updateBtn_Click(object sender, EventArgs e)
    {
        if (dataGridViewMeds.CurrentRow == null)
        {
            return;
        }
        int medId = (int)dataGridViewMeds.CurrentRow.Cells["MedicineID"].Value;
        string name = medNameBox.Text.Trim();
        string category = categoryBox.Text.Trim();
        decimal price = decimal.Parse(priceBox.Text.Trim());
        int quantity = int.Parse(quantityBox.Text.Trim());
        SqlConnection connection = new SqlConnection(Program.connectionString);
        string q = "UPDATE Medicines SET Name=@Name, Category=@Category, Price=@Price, Quantity=@Quantity WHERE MedicineID=@ID";
        try
        {
            connection.Open();
            SqlCommand command = new SqlCommand(q, connection);
            command.Parameters.AddWithValue("@Name", name);
            command.Parameters.AddWithValue("@Category", category);
            command.Parameters.AddWithValue("@Price", price);
            command.Parameters.AddWithValue("@Quantity", quantity);
            command.Parameters.AddWithValue("@ID", medId);
            command.ExecuteNonQuery();
            MessageBox.Show("Medicine updated successfully");
            GetAllMeds();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error updating medicine: " + ex.Message);
        }
        finally
        {
            connection.Close();
        }
    }

    private void recSaleBtn_Click(object sender, EventArgs e)
    {
        if (dataGridViewMeds.CurrentRow == null)
        {
            return;
        }
        int medId = (int)dataGridViewMeds.CurrentRow.Cells["MedicineID"].Value;
        int currStock = (int)dataGridViewMeds.CurrentRow.Cells["Quantity"].Value;
        string input = Interaction.InputBox("Enter quantity sold:", "Record Sale", "1");
        if (!int.TryParse(input, out var quantitySold) || quantitySold <= 0)
        {
            MessageBox.Show("Please enter a valid positive number.");
            return;
        }
        if (quantitySold > currStock)
        {
            MessageBox.Show("Not enough stock to complete the sale.");
            return;
        }
        SqlConnection connection = new SqlConnection(Program.connectionString);
        try
        {
            string query = "\r\n                INSERT INTO Sales (MedicineID, QuantitySold, SaleDate)\r\n                VALUES (@MedicineID, @QuantitySold, @SaleDate);\r\n                UPDATE Medicines\r\n                SET Quantity = Quantity - @QuantitySold\r\n                WHERE MedicineID = @MedicineID;";
            SqlCommand cmd = new SqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@MedicineID", medId);
            cmd.Parameters.AddWithValue("@QuantitySold", quantitySold);
            cmd.Parameters.AddWithValue("@SaleDate", DateTime.Now);
            connection.Open();
            cmd.ExecuteNonQuery();
            MessageBox.Show("Sale recorded successfully!");
            GetAllMeds();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error recording sale: " + ex.Message);
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
        this.searchBox = new System.Windows.Forms.TextBox();
        this.searchBtn = new System.Windows.Forms.Button();
        this.viewAllBtn = new System.Windows.Forms.Button();
        this.dataGridViewMeds = new System.Windows.Forms.DataGridView();
        this.medNameBox = new System.Windows.Forms.TextBox();
        this.priceBox = new System.Windows.Forms.TextBox();
        this.categoryBox = new System.Windows.Forms.TextBox();
        this.quantityBox = new System.Windows.Forms.TextBox();
        this.addMedBtn = new System.Windows.Forms.Button();
        this.updateBtn = new System.Windows.Forms.Button();
        this.recSaleBtn = new System.Windows.Forms.Button();
        this.button5 = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)this.dataGridViewMeds).BeginInit();
        base.SuspendLayout();
        this.searchBox.Location = new System.Drawing.Point(17, 77);
        this.searchBox.Name = "searchBox";
        this.searchBox.Size = new System.Drawing.Size(556, 23);
        this.searchBox.TabIndex = 0;
        this.searchBox.TextChanged += new System.EventHandler(searchBox_TextChanged);
        this.searchBtn.Location = new System.Drawing.Point(579, 77);
        this.searchBtn.Name = "searchBtn";
        this.searchBtn.Size = new System.Drawing.Size(95, 24);
        this.searchBtn.TabIndex = 1;
        this.searchBtn.Text = "Search";
        this.searchBtn.UseVisualStyleBackColor = true;
        this.viewAllBtn.Location = new System.Drawing.Point(680, 76);
        this.viewAllBtn.Name = "viewAllBtn";
        this.viewAllBtn.Size = new System.Drawing.Size(108, 23);
        this.viewAllBtn.TabIndex = 2;
        this.viewAllBtn.Text = "View All Meds";
        this.viewAllBtn.UseVisualStyleBackColor = true;
        this.dataGridViewMeds.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridViewMeds.Location = new System.Drawing.Point(17, 106);
        this.dataGridViewMeds.Name = "dataGridViewMeds";
        this.dataGridViewMeds.Size = new System.Drawing.Size(771, 332);
        this.dataGridViewMeds.TabIndex = 3;
        this.dataGridViewMeds.SelectionChanged += new System.EventHandler(dataGridViewMeds_SelectionChanged);
        this.medNameBox.ForeColor = System.Drawing.SystemColors.WindowText;
        this.medNameBox.Location = new System.Drawing.Point(17, 12);
        this.medNameBox.Name = "medNameBox";
        this.medNameBox.Size = new System.Drawing.Size(299, 23);
        this.medNameBox.TabIndex = 4;
        this.priceBox.ForeColor = System.Drawing.SystemColors.WindowText;
        this.priceBox.Location = new System.Drawing.Point(548, 12);
        this.priceBox.Name = "priceBox";
        this.priceBox.Size = new System.Drawing.Size(126, 23);
        this.priceBox.TabIndex = 5;
        this.categoryBox.ForeColor = System.Drawing.SystemColors.WindowText;
        this.categoryBox.Location = new System.Drawing.Point(322, 12);
        this.categoryBox.Name = "categoryBox";
        this.categoryBox.Size = new System.Drawing.Size(220, 23);
        this.categoryBox.TabIndex = 6;
        this.quantityBox.ForeColor = System.Drawing.SystemColors.WindowText;
        this.quantityBox.Location = new System.Drawing.Point(680, 12);
        this.quantityBox.Name = "quantityBox";
        this.quantityBox.Size = new System.Drawing.Size(100, 23);
        this.quantityBox.TabIndex = 7;
        this.addMedBtn.Location = new System.Drawing.Point(17, 48);
        this.addMedBtn.Name = "addMedBtn";
        this.addMedBtn.Size = new System.Drawing.Size(121, 23);
        this.addMedBtn.TabIndex = 8;
        this.addMedBtn.Text = "Add Medicine";
        this.addMedBtn.UseVisualStyleBackColor = true;
        this.addMedBtn.Click += new System.EventHandler(addMedBtn_Click);
        this.updateBtn.Location = new System.Drawing.Point(144, 48);
        this.updateBtn.Name = "updateBtn";
        this.updateBtn.Size = new System.Drawing.Size(141, 23);
        this.updateBtn.TabIndex = 9;
        this.updateBtn.Text = "Update Medicine Stock";
        this.updateBtn.UseVisualStyleBackColor = true;
        this.updateBtn.Click += new System.EventHandler(updateBtn_Click);
        this.recSaleBtn.Location = new System.Drawing.Point(291, 48);
        this.recSaleBtn.Name = "recSaleBtn";
        this.recSaleBtn.Size = new System.Drawing.Size(102, 23);
        this.recSaleBtn.TabIndex = 10;
        this.recSaleBtn.Text = "Record Sale";
        this.recSaleBtn.UseVisualStyleBackColor = true;
        this.recSaleBtn.Click += new System.EventHandler(recSaleBtn_Click);
        this.button5.Location = new System.Drawing.Point(498, 48);
        this.button5.Name = "button5";
        this.button5.Size = new System.Drawing.Size(75, 23);
        this.button5.TabIndex = 11;
        this.button5.Text = "button5";
        this.button5.UseVisualStyleBackColor = true;
        base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 15f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        base.ClientSize = new System.Drawing.Size(800, 450);
        base.Controls.Add(this.button5);
        base.Controls.Add(this.recSaleBtn);
        base.Controls.Add(this.updateBtn);
        base.Controls.Add(this.addMedBtn);
        base.Controls.Add(this.quantityBox);
        base.Controls.Add(this.categoryBox);
        base.Controls.Add(this.priceBox);
        base.Controls.Add(this.medNameBox);
        base.Controls.Add(this.dataGridViewMeds);
        base.Controls.Add(this.viewAllBtn);
        base.Controls.Add(this.searchBtn);
        base.Controls.Add(this.searchBox);
        base.Name = "MainForm";
        this.Text = "Pharmacy Inventory Management";
        ((System.ComponentModel.ISupportInitialize)this.dataGridViewMeds).EndInit();
        base.ResumeLayout(false);
        base.PerformLayout();
    }
}
