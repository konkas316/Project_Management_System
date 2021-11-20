using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Text.RegularExpressions;
using System.IO;

namespace ProjectManagementSystem
{

    public partial class Form1 : Form
    {
        Dictionary<int, byte[]> _myAttachments;

        Form3 f3 = new Form3();

        public static string ConnectionString = ConfigurationManager.ConnectionStrings["dBCs"].ConnectionString;
        static SqlConnection sqlconn = new SqlConnection(ConnectionString);

        public static string ConnectionString2 = ConfigurationManager.ConnectionStrings["dbcs2"].ConnectionString;
        SqlConnection con;

        public Form1()
        {
            InitializeComponent();
            _myAttachments = new Dictionary<int, byte[]>();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            BindGrid();
            f3.richTextBox2.ReadOnly = true;
            button6_Click(sender, e);

        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.Hide();
            Form2 f2 = new Form2();
            f2.ShowDialog();
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                if (txtUserName1.Text != "" || txtPassword1.Text != "")
                {
                    string sqlquery = "select * from [dbo].[UserRegistration] where Uname=@Un and Upwd=@Up";

                    sqlconn.Open();
                    SqlCommand sqlcom = new SqlCommand(sqlquery, sqlconn);
                    sqlcom.Parameters.AddWithValue("@Un", txtUserName1.Text);
                    sqlcom.Parameters.AddWithValue("@Up", txtPassword1.Text);
                    SqlDataAdapter sda = new SqlDataAdapter(sqlcom);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    sqlcom.ExecuteNonQuery();
                    sqlconn.Close();
                    if (dt.Rows.Count > 0)
                    {
                        MessageBox.Show(" Login successful ! ");

                        f3.richTextBox2.ReadOnly = false;
                        button8.Visible = true;
                        button6.Visible = true;
                        button7.Visible = false;
                        button5.Visible = false;
                        //label2.Visible = false;
                        btnDelete.Visible = true;
                        UploadFile.Visible = true;
                    }

                    else
                    {
                        MessageBox.Show("Invalid username or password");
                    }

                    txtUserName1.Text = "";
                    txtPassword1.Text = "";

                }
                else
                {
                    MessageBox.Show("Please enter your credentials");
                }

                txtUserName1.Focus();
                return;

            }
            catch
            {
                //MessageBox.Show("Please input your credentials");
            }

        }
        private void button8_Click(object sender, EventArgs e)
        {
            try
            {
                if ("[Uname]" != null)
                {
                    DialogResult dlg = new DialogResult();
                    dlg = MessageBox.Show("Do you really want to delete the your account", "Confirm Deletion", MessageBoxButtons.YesNo);
                    if (dlg == DialogResult.Yes)
                    {
                        SqlCommand comm = new SqlCommand("delete from [dbo].[UserRegistration] where [Uname]=@Un and Upwd=@Up", sqlconn);

                        comm.Parameters.AddWithValue("@Un", txtUserName1.Text);
                        comm.Parameters.AddWithValue("@Up", txtPassword1.Text);
                        sqlconn.Open();
                        int rc = comm.ExecuteNonQuery();
                        if (rc > 0)
                        {
                            MessageBox.Show("User deleted successfully", "Delete Done", MessageBoxButtons.OK);
                            button6_Click(sender, e);
                        }
                        else
                        {
                            MessageBox.Show("There is no record found for delete!", "Delete Done", MessageBoxButtons.OK);
                        }

                        txtUserName1.Clear();
                    }
                }
                else
                {
                    //MessageBox.Show("There is no record found for delete!", "Delete Done", MessageBoxButtons.OK);
                }
            }
            catch
            {

            }
        }
        private void button6_Click(object sender, EventArgs e)
        {
            sqlconn.Close();
            button8.Visible = false;
            button6.Visible = false;
            button7.Visible = true;
            button5.Visible = true;
            txtPassword1.Visible = true;
            //label2.Visible = true;
            f3.richTextBox2.ReadOnly = true;
            btnDelete.Visible = false;
            UploadFile.Visible = false;
        }
        private void OpenFile(int id)
        {
            try
            {
                using (con = new SqlConnection(ConnectionString2))
                {
                    string query = "select UploadName, Data from tblFiles where Id=@Id";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                    con.Open();
                    var reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        var name = reader["UploadName"].ToString();
                        var data = (byte[])reader["Data"];
                        File.WriteAllBytes(Name, data);
                        System.Diagnostics.Process.Start(Name);
                    }

                }
            }
            catch
            {

            }
        }
        
      
        private void BindGrid()
        {
            using (con = new SqlConnection(ConnectionString2))
            {
                using (SqlCommand cmd = new SqlCommand("select Id, UploadName, Description from tblFiles", con))
                {
                    cmd.CommandType = CommandType.Text;
                    using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                    {
                        using (DataTable dt = new DataTable())
                        {
                            sda.Fill(dt);
                            dataGridView1.DataSource = dt;
                        }
                    }
                }
            }
        }

        private void UploadFile_Click(object sender, EventArgs e)
        {
            try
            {                
                using (OpenFileDialog openFileDialog1 = new OpenFileDialog())
                {
                    if (openFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        string fileName = openFileDialog1.FileName;
                        byte[] bytes = File.ReadAllBytes(fileName);

                        f3.richTextBox2.ReadOnly = false;
                        f3.ShowDialog();
                        
                        {
                            using (con = new SqlConnection(ConnectionString2))
                            {
                                string sql = "IF NOT EXISTS(SELECT 1 FROM tblFiles WHERE UploadName = @UploadName) BEGIN INSERT INTO tblFiles (UploadName, Data, Description) VALUES(@UploadName, @Data, @Description) END";

                                using (SqlCommand cmd = new SqlCommand(sql, con))
                                {
                                    cmd.Parameters.AddWithValue("@UploadName", Path.GetFileName(fileName));
                                    cmd.Parameters.AddWithValue("@Description", f3.richTextBox2.Text);
                                    cmd.Parameters.AddWithValue("@Data", bytes);
                                    con.Open();
                                    cmd.ExecuteNonQuery();
                                    con.Close();
                                }
                            }
                            this.BindGrid();
                            f3.richTextBox2.Clear();
                           
                        }
                    }
                }
            }
            catch
            {

            }
        }

        private void btnRead_Click(object sender, EventArgs e)
        {           
                var selectRow = dataGridView1.SelectedRows;
                foreach (var row in selectRow)
                {   
                    int id = (int)((DataGridViewRow)row).Cells[0].Value;
                    OpenFile(id); 
                    
                }
           
        }

        private void DeleteFile(int id)
        {
            using (con = new SqlConnection(ConnectionString2))
            {
                string query = "delete from tblFiles where Id=@Id";
                SqlCommand cmd = new SqlCommand(query, con);
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                con.Open();
                int a = cmd.ExecuteNonQuery();
                if (a > 0)
                {
                    MessageBox.Show("Data deleted!");
                }
                else
                {
                    MessageBox.Show("Data not deleted!");
                }
                con.Close();

            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            var selectRow = dataGridView1.SelectedRows;
            foreach (var row in selectRow)
            {
                int id = int.Parse(((DataGridViewRow)row).Cells[0].Value.ToString());
                DeleteFile(id);
                BindGrid();
            }
        }
                
        private void dataGridView1_RowHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {           

           //  richTextBox1.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();            

        }

        public void ThirdDatagridOpen()
        {
            string nrCode = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();

            if (nrCode != "")
            {
               
                f3.richTextBox2.Text = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                
                f3.ShowDialog();
            }
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                ThirdDatagridOpen();
            }
            catch
            {

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            
            if (dataGridView1.SelectedCells.Count == 1 && dataGridView1.SelectedCells[0].ColumnIndex == 1 && dataGridView1.SelectedCells[0].Value != null)
            {
                DownloadAttachment(dataGridView1.SelectedCells[0]);              
            }
            else
                MessageBox.Show("Select a single cell from Attachment column", "Error uploading file", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void DownloadAttachment(DataGridViewCell dgvCell)
        {
            string fileName = Convert.ToString(dgvCell.Value);

            
            if (fileName == string.Empty)
                return;

            FileInfo fileInfo = new FileInfo(fileName);
            string fileExtension = fileInfo.Extension;

            //byte[] byteData = null;

         
            using (SaveFileDialog saveFileDialog1 = new SaveFileDialog())
            {
                
                saveFileDialog1.Filter = "Files (*" + fileExtension + ")|*" + fileExtension;
                saveFileDialog1.Title = "Save File as";
                saveFileDialog1.CheckPathExists = true;
                saveFileDialog1.FileName = fileName;

                if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    //byteData = _myAttachments[dgvCell.RowIndex];
                    //File.WriteAllBytes(saveFileDialog1.FileName, byteData);
                }
            }
        }
    }
}


