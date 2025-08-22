using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MedicalAppointmentBookingSys;

public class MainForm : Form
{
    private IContainer components = null;

    private Button button1;

    private Button button2;

    private Button button3;

    public MainForm()
    {
        InitializeComponent();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
    }

    private void button2_Click(object sender, EventArgs e)
    {
        AppointmentForm af = new AppointmentForm();
        af.Show();
    }

    private void button1_Click(object sender, EventArgs e)
    {
        DoctorListForm dlf = new DoctorListForm();
        dlf.Show();
    }

    private void button3_Click(object sender, EventArgs e)
    {
        ManageAppointmentsForm maf = new ManageAppointmentsForm();
        maf.Show();
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
        this.button1 = new System.Windows.Forms.Button();
        this.button2 = new System.Windows.Forms.Button();
        this.button3 = new System.Windows.Forms.Button();
        base.SuspendLayout();
        this.button1.Location = new System.Drawing.Point(24, 138);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(215, 100);
        this.button1.TabIndex = 0;
        this.button1.Text = "Display Doctors";
        this.button1.UseVisualStyleBackColor = true;
        this.button1.Click += new System.EventHandler(button1_Click);
        this.button2.Location = new System.Drawing.Point(297, 138);
        this.button2.Name = "button2";
        this.button2.Size = new System.Drawing.Size(215, 100);
        this.button2.TabIndex = 1;
        this.button2.Text = "Book Appointment";
        this.button2.UseVisualStyleBackColor = true;
        this.button2.Click += new System.EventHandler(button2_Click);
        this.button3.Location = new System.Drawing.Point(563, 138);
        this.button3.Name = "button3";
        this.button3.Size = new System.Drawing.Size(215, 100);
        this.button3.TabIndex = 2;
        this.button3.Text = "Manage Appointments";
        this.button3.UseVisualStyleBackColor = true;
        this.button3.Click += new System.EventHandler(button3_Click);
        base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 15f);
        base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        base.ClientSize = new System.Drawing.Size(800, 450);
        base.Controls.Add(this.button3);
        base.Controls.Add(this.button2);
        base.Controls.Add(this.button1);
        base.Name = "MainForm";
        this.Text = "Medical Appointment Booking System";
        base.Load += new System.EventHandler(MainForm_Load);
        base.ResumeLayout(false);
    }
}
