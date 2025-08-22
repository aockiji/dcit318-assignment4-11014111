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
			SqlCommand command = new SqlCommand("getAllMeds", connection);
			command.CommandType = CommandType.StoredProcedure; 
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
		SqlCommand command = new SqlCommand("addMed", connection);
		command.CommandType = CommandType.StoredProcedure;
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
        SqlCommand command = new SqlCommand("updateMed", connection);
		command.CommandType = CommandType.StoredProcedure;
        try
		{
			connection.Open();
			
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
        SqlCommand command = new SqlCommand("recSale", connection);
        command.CommandType = CommandType.StoredProcedure;
        try
		{
            command.Parameters.AddWithValue("@MedicineID", medId);
            command.Parameters.AddWithValue("@QuantitySold", quantitySold);
            command.Parameters.AddWithValue("@SaleDate", DateTime.Now);
			connection.Open();
            command.ExecuteNonQuery();
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
        searchBox = new TextBox();
        searchBtn = new Button();
        viewAllBtn = new Button();
        dataGridViewMeds = new DataGridView();
        medNameBox = new TextBox();
        priceBox = new TextBox();
        categoryBox = new TextBox();
        quantityBox = new TextBox();
        addMedBtn = new Button();
        updateBtn = new Button();
        recSaleBtn = new Button();
        ((ISupportInitialize)dataGridViewMeds).BeginInit();
        SuspendLayout();
        // 
        // searchBox
        // 
        searchBox.Location = new Point(17, 77);
        searchBox.Name = "searchBox";
        searchBox.Size = new Size(556, 23);
        searchBox.TabIndex = 0;
        searchBox.TextChanged += searchBox_TextChanged;
        // 
        // searchBtn
        // 
        searchBtn.Location = new Point(579, 77);
        searchBtn.Name = "searchBtn";
        searchBtn.Size = new Size(95, 24);
        searchBtn.TabIndex = 1;
        searchBtn.Text = "Search";
        searchBtn.UseVisualStyleBackColor = true;
        // 
        // viewAllBtn
        // 
        viewAllBtn.Location = new Point(680, 76);
        viewAllBtn.Name = "viewAllBtn";
        viewAllBtn.Size = new Size(108, 23);
        viewAllBtn.TabIndex = 2;
        viewAllBtn.Text = "View All Meds";
        viewAllBtn.UseVisualStyleBackColor = true;
        // 
        // dataGridViewMeds
        // 
        dataGridViewMeds.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dataGridViewMeds.Location = new Point(17, 106);
        dataGridViewMeds.Name = "dataGridViewMeds";
        dataGridViewMeds.Size = new Size(771, 332);
        dataGridViewMeds.TabIndex = 3;
        dataGridViewMeds.SelectionChanged += dataGridViewMeds_SelectionChanged;
        // 
        // medNameBox
        // 
        medNameBox.ForeColor = SystemColors.WindowText;
        medNameBox.Location = new Point(17, 12);
        medNameBox.Name = "medNameBox";
        medNameBox.Size = new Size(299, 23);
        medNameBox.TabIndex = 4;
        // 
        // priceBox
        // 
        priceBox.ForeColor = SystemColors.WindowText;
        priceBox.Location = new Point(548, 12);
        priceBox.Name = "priceBox";
        priceBox.Size = new Size(126, 23);
        priceBox.TabIndex = 5;
        // 
        // categoryBox
        // 
        categoryBox.ForeColor = SystemColors.WindowText;
        categoryBox.Location = new Point(322, 12);
        categoryBox.Name = "categoryBox";
        categoryBox.Size = new Size(220, 23);
        categoryBox.TabIndex = 6;
        // 
        // quantityBox
        // 
        quantityBox.ForeColor = SystemColors.WindowText;
        quantityBox.Location = new Point(680, 12);
        quantityBox.Name = "quantityBox";
        quantityBox.Size = new Size(100, 23);
        quantityBox.TabIndex = 7;
        // 
        // addMedBtn
        // 
        addMedBtn.Location = new Point(17, 48);
        addMedBtn.Name = "addMedBtn";
        addMedBtn.Size = new Size(121, 23);
        addMedBtn.TabIndex = 8;
        addMedBtn.Text = "Add Medicine";
        addMedBtn.UseVisualStyleBackColor = true;
        addMedBtn.Click += addMedBtn_Click;
        // 
        // updateBtn
        // 
        updateBtn.Location = new Point(144, 48);
        updateBtn.Name = "updateBtn";
        updateBtn.Size = new Size(141, 23);
        updateBtn.TabIndex = 9;
        updateBtn.Text = "Update Medicine Stock";
        updateBtn.UseVisualStyleBackColor = true;
        updateBtn.Click += updateBtn_Click;
        // 
        // recSaleBtn
        // 
        recSaleBtn.Location = new Point(291, 48);
        recSaleBtn.Name = "recSaleBtn";
        recSaleBtn.Size = new Size(102, 23);
        recSaleBtn.TabIndex = 10;
        recSaleBtn.Text = "Record Sale";
        recSaleBtn.UseVisualStyleBackColor = true;
        recSaleBtn.Click += recSaleBtn_Click;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(800, 450);
        Controls.Add(recSaleBtn);
        Controls.Add(updateBtn);
        Controls.Add(addMedBtn);
        Controls.Add(quantityBox);
        Controls.Add(categoryBox);
        Controls.Add(priceBox);
        Controls.Add(medNameBox);
        Controls.Add(dataGridViewMeds);
        Controls.Add(viewAllBtn);
        Controls.Add(searchBtn);
        Controls.Add(searchBox);
        Name = "MainForm";
        Text = "Pharmacy Inventory Management";
        ((ISupportInitialize)dataGridViewMeds).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }
}
