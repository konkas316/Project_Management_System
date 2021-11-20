using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace ProjectManagementSystem
{

    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        public static string ConnectionString = ConfigurationManager.ConnectionStrings["dBCs"].ConnectionString;
        SqlConnection con = new SqlConnection(ConnectionString);
        public bool UserNameCheck()
        {
            SqlCommand check_User_Name = new SqlCommand("SELECT * FROM [dbo].[UserRegistration] WHERE [Uname] = @Un OR [Upwd] = @Up", con);

            check_User_Name.Parameters.AddWithValue("@Un", txtUserName.Text);
            check_User_Name.Parameters.AddWithValue("@Up", txtPassword.Text);

            con.Open();
            SqlDataReader reader = check_User_Name.ExecuteReader();

            if (reader.HasRows)
            {
                MessageBox.Show("This user name or password already exist");
                return true;
            }

            else
            {
                con.Close();
                //MessageBox.Show("This User name or password doesn't exist");
                return false;
            }
            
        }
        
        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                string str;

                str = txtPassword.Text;

                Regex rgx = new Regex("[^A-Za-z0-9]");
                bool containsSpecialCharacter = rgx.IsMatch(str);

                if (!UserNameCheck())
                {
                    if (txtUserName.Text != "" && txtPassword.Text != "" && txtConfirmPassword.Text != "")
                    {
                        if (txtPassword.Text == txtConfirmPassword.Text)
                        {
                            if (txtPassword.Text.Length > 6)
                            {
                                if (txtPassword.Text.Any(char.IsUpper))
                                {
                                    if (txtPassword.Text.Any(char.IsLower))
                                    {
                                        if (txtPassword.Text.Any(char.IsDigit))
                                        {
                                            if (!txtPassword.Text.Contains(" "))
                                            {
                                                if (containsSpecialCharacter)
                                                {
                                                    string sqlquery = "insert into [dbo].[UserRegistration] values (@Un,@Up,@Urepwd)";
                                                    con.Open();
                                                    SqlCommand sqlcomm = new SqlCommand(sqlquery, con);
                                                    sqlcomm.Parameters.AddWithValue("@Un", txtUserName.Text);
                                                    sqlcomm.Parameters.AddWithValue("@Up", txtPassword.Text);
                                                    sqlcomm.Parameters.AddWithValue("@Urepwd", txtConfirmPassword.Text);
                                                    sqlcomm.ExecuteNonQuery();
                                                    con.Close();

                                                    MessageBox.Show("User registered successfully");

                                                    CloseOpenForm1();
                                                }
                                                else
                                                {
                                                    MessageBox.Show("Password does not contains special character");
                                                }
                                            }
                                            else
                                            {
                                                MessageBox.Show("Password have to not contain white spaces");
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Password have to contain atleast one digit");
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show("Password have to contain atleast one lower case");
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("Password have to contain atleast one upper case");
                                }
                            }
                            else
                            {
                                MessageBox.Show("Password must have atleast 6 characters");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Password not matching");
                        }

                    }
                    else
                    {
                        MessageBox.Show("Please enter user name and password and confirm password");
                    }

                    return;
                }
            }
            catch
            {

            }

        }

        private void CloseOpenForm1()
        {
            this.Hide();
            Form1 f1 = new Form1();
            f1.ShowDialog();
            this.Close();

        }

        private void btnCloseF2_Click(object sender, EventArgs e)
        {
            CloseOpenForm1();
        }
    }
}
