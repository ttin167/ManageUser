using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace UserManager
{
    public partial class Form1 : Form
    {
        private int currentId = 0;
        private string connectionString = "Data Source=MSI\\SQLEXPRESS;Initial Catalog=CustomerDB;Integrated Security=True";

        public Form1()
        {
            InitializeComponent();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            AddUser(txtName.Text, txtAddress.Text, txtPhone.Text);
            LoadData();
        }
        private void LoadData()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Users", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dataGridView1.DataSource = dt;
            }
        }
        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // Cập nhật các trường nhập liệu khi chọn một dòng
            if (dataGridView1.SelectedRows.Count > 0)
            {
                UpdateTextFieldsFromSelectedRow();
            }
        }
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Chỉ xử lý khi nhấp vào một ô hợp lệ (không phải header)
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                UpdateTextFieldsFromSelectedRow();
            }
        }

        private void UpdateTextFieldsFromSelectedRow()
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                DataGridViewRow row = dataGridView1.SelectedRows[0];
                txtName.Text = row.Cells["Name"].Value.ToString();
                txtAddress.Text = row.Cells["Address"].Value.ToString();
                txtPhone.Text = row.Cells["Phone"].Value.ToString();
                currentId = Convert.ToInt32(row.Cells["Id"].Value);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (currentId != 0)
            {
                DataGridViewRow selectedRow = dataGridView1.Rows
                    .Cast<DataGridViewRow>()
                    .FirstOrDefault(row => Convert.ToInt32(row.Cells["Id"].Value) == currentId);

                if (selectedRow != null)
                {
                    string currentName = selectedRow.Cells["Name"].Value.ToString();
                    string currentAddress = selectedRow.Cells["Address"].Value.ToString();
                    string currentPhone = selectedRow.Cells["Phone"].Value.ToString();

                    string newName = string.IsNullOrEmpty(txtName.Text) ? currentName : txtName.Text;
                    string newAddress = string.IsNullOrEmpty(txtAddress.Text) ? currentAddress : txtAddress.Text;
                    string newPhone = string.IsNullOrEmpty(txtPhone.Text) ? currentPhone : txtPhone.Text;

                    UpdateUser(currentId, newName, newAddress, newPhone);
                    LoadData();
                    ClearInputFields();
                    currentId = 0;
                }
            }
            else
            {
                MessageBox.Show("No user selected for update.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count > 0)
            {
                int id = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells["Id"].Value);
                DeleteUser(id);
                LoadData(); 
                ClearInputFields(); 
            }
            else
            {
                MessageBox.Show("Please select a user to delete.");
            }
        }


       


        private void AddUser(string name, string address, string phone)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Users (Name, Address, Phone) VALUES (@Name, @Address, @Phone)", conn);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.ExecuteNonQuery();
            }
        }

        private void UpdateUser(int id, string name, string address, string phone)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("UPDATE Users SET Name=@Name, Address=@Address, Phone=@Phone WHERE Id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.Parameters.AddWithValue("@Name", name);
                cmd.Parameters.AddWithValue("@Address", address);
                cmd.Parameters.AddWithValue("@Phone", phone);
                cmd.ExecuteNonQuery();
            }
        }

        private void DeleteUser(int id)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Users WHERE Id=@Id", conn);
                cmd.Parameters.AddWithValue("@Id", id);
                cmd.ExecuteNonQuery();
            }
        }

        private bool ValidateInput()
        {
            return !string.IsNullOrWhiteSpace(txtName.Text) &&
                   !string.IsNullOrWhiteSpace(txtAddress.Text) &&
                   !string.IsNullOrWhiteSpace(txtPhone.Text) &&
                   txtName.Text != "Name" &&
                   txtAddress.Text != "Address" &&
                   txtPhone.Text != "Phone";
        }

        private void ClearInputFields()
        {
            txtName.Text = "Name";
            txtAddress.Text = "Address";
            txtPhone.Text = "Phone";
            txtName.ForeColor = System.Drawing.Color.Gray;
            txtAddress.ForeColor = System.Drawing.Color.Gray;
            txtPhone.ForeColor = System.Drawing.Color.Gray;
        }
    }
}
    
