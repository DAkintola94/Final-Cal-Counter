using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace finalCounter
{
    public partial class Form1 : Form
    {
        public static Form1 instance;
        public DataGridView dg1;

  
        public Form1()
        {
            InitializeComponent();
            instance = this;
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (double.TryParse(textBox1.Text, out double tb1) && double.TryParse(textBox2.Text, out double tb2))
            {
                double caloriesCalculation = (tb1 / 100) * tb2; //could also do gramConverter(tb1) * tb2, for good "outer_method" practice
                richTextBox1.Text = caloriesCalculation.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e) // method to add everything to each boxes depending on what is empty and whats not,
                                                               // with a click of a button
        {

            bool textBox3_filled = double.TryParse(textBox3.Text, out double tb3);
            bool textBox4_filled = double.TryParse(textBox4.Text, out double tb4);
            bool textBox5_filled = double.TryParse(textBox5.Text, out double tb5);
            
            if(textBox3_filled && textBox4_filled && textBox5_filled)
            {
                double calculation = tb3 + tb4 + tb5;
                richTextBox1.Text = calculation.ToString();
                
               
            }

            else if (textBox3_filled && textBox4_filled) // else if the former "if statement" returns false,
                                                         // we just add the "basic" 
            {
                double filledCalculation = tb3 + tb4;
                richTextBox1.Text = filledCalculation.ToString();
            }
            
           
        }

        private void button5_Click(object sender, EventArgs e)
        {

            string connectionString = "Data Source=DENNIS-PC\\SQLEXPRESS;Initial Catalog=results;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
            using (SqlConnection connect = new SqlConnection(connectionString))
            {
                try
                {
                    connect.Open();
                    using (SqlCommand commando = new SqlCommand("insert into content(savedResult) values('" + richTextBox1.Text + "')", connect))
                    {
                        int i = commando.ExecuteNonQuery();
                        commando.Parameters.AddWithValue("@savedResult", richTextBox1.Text);
                        if (i!= 0)
                        {
                            MessageBox.Show("The following value have been saved: " + richTextBox1.Text);
                            loadDataIntoGrid();
                        }

                        else
                        {
                            MessageBox.Show("An error occured");
                        }
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("An error occured: " + ex.Message);
                }

            }

        }

        private void button6_Click(object sender, EventArgs e)
        {
            string dataConnection = "Data Source=DENNIS-PC\\SQLEXPRESS;Initial Catalog=results;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
            using (SqlConnection anotherConnect = new SqlConnection(dataConnection))
            {
                try
                {
                    anotherConnect.Open();
                    using (SqlCommand commandToSelect = new SqlCommand("SELECT * FROM content", anotherConnect))
                    {
                        SqlDataReader first_reader = commandToSelect.ExecuteReader();
                        DataTable first_dataTable = new DataTable();
                        first_dataTable.Load(first_reader);
                    }
                }

                catch (Exception exp)
                {
                    MessageBox.Show("Something when wrong " + exp.Message);
                }
            }

        }

        private void loadDataIntoGrid()
        {
            string dataConnection = "Data Source=DENNIS-PC\\SQLEXPRESS;Initial Catalog=results;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
            using(SqlConnection connected = new SqlConnection(dataConnection))
            {
                try
                {
                    connected.Open();
                    using(SqlDataAdapter dtAdapter = new SqlDataAdapter("SELECT * FROM content", connected))
                    {
                        DataTable dtTable1 = new DataTable();
                        dtAdapter.Fill(dtTable1);
                        dataGridView1.DataSource = dtTable1;
                    }
                }

                catch (Exception ex)
                {
                    MessageBox.Show("Something when wrong " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            if (double.TryParse(richTextBox1.Text, out double resultCounted) && textBox3.Text == string.Empty)
            {
                textBox3.Text = resultCounted.ToString();
            }

            else if (textBox4.Text == string.Empty && textBox3.Text!= string.Empty)
            {
                textBox4.Text = resultCounted.ToString();
            }

            else if (textBox3.Text != string.Empty && textBox4.Text != string.Empty)
            {
                textBox5.Text = resultCounted.ToString();
            }

     

            
        }

        

        private void button4_Click(object sender, EventArgs e) //reset all boxes
        {
            textBox1.Text = null;
            textBox2.Text = null;
            textBox3.Text = null;
            textBox4.Text = null;
            textBox5.Text = null;
            richTextBox1.Text = null;
            

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //empty, can't delete the method or i get an error in visual studio for some reason.

        }

        private void button7_Click(object sender, EventArgs e)
        {
            
            if (dataGridView1.CurrentCell == null)
            {
                MessageBox.Show("Please, select a value to interract with");
                return; 

            }
           
            int index_row = dataGridView1.CurrentCell.RowIndex;
            object savedResultValue = null;

            try
            {
                savedResultValue = dataGridView1.Rows[index_row].Cells["savedResult"].Value;
            }
            
            catch (ArgumentException)
            {
                int columRowInt = 0;

                try
                {
                    savedResultValue = dataGridView1.Rows[index_row].Cells[columRowInt].Value;
                }

                catch (Exception ex)
                {
                    MessageBox.Show("An error occured " + ex.Message);
                    return;
                }


            }

            if (savedResultValue == null)
            {
                MessageBox.Show("Please, select a value to continue");
                return;
            }

            dataGridView1.Rows.RemoveAt(index_row);

            string connectingString = "Data Source=DENNIS-PC\\SQLEXPRESS;Initial Catalog=results;Integrated Security=True;Encrypt=False;TrustServerCertificate=True";
            string query1 = "DELETE FROM content WHERE savedResult = @savedResult";

            using (SqlConnection connect2 = new SqlConnection(connectingString))
            {
                try
                {
                    connect2.Open();
                    using (SqlCommand command = new SqlCommand(query1, connect2))
                    {
                        command.Parameters.AddWithValue("@savedResult", savedResultValue);
                        int rowSelected = command.ExecuteNonQuery();

                    }
                }

                catch (Exception ex) 
                {
                    MessageBox.Show("An error occured while trying to delete " + ex.Message);
                }
            }
            

        }

    }
}
