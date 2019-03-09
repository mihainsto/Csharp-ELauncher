using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Security;
using System.Windows;
using System.Management;
using MySql.Data.MySqlClient;
using System.Media;
using System.Diagnostics;
using System.IO;
using MessageBox = System.Windows.Forms.MessageBox;
namespace E
{
    public partial class Login : MetroFramework.Forms.MetroForm
    {
        public Login()
        {
            InitializeComponent();
        }
        private static string identifier(string wmiClass, string wmiProperty)
        {
            string result = "";
            System.Management.ManagementClass mc =
        new System.Management.ManagementClass(wmiClass);
            System.Management.ManagementObjectCollection moc = mc.GetInstances();
            foreach (System.Management.ManagementObject mo in moc)
            {

                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch
                    {
                    }
                }
            }
            return result;
        }
        private static string Hashhhh()
        {

            string retVal = "E";
            retVal += identifier("Win32_Processor", "UniqueId");
            {
                retVal += identifier("Win32_Processor", "ProcessorId");

                {
                    retVal += identifier("Win32_Processor", "Name");

                    {
                        retVal += identifier("Win32_Processor", "Manufacturer");
                    }

                    retVal += identifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            retVal += identifier("Win32_BaseBoard", "Manufacturer");
            retVal += identifier("Win32_BaseBoard", "SerialNumber");
            retVal += identifier("Win32_BIOS", "Version");
            retVal += identifier("Win32_BIOS", "IdentificationCode");
            retVal += identifier("Win32_BIOS", "SMBIOSBIOSVersion");
            retVal += identifier("Win32_VideoController", "Name");



            return retVal;
        }
        public static string ConvertStringToHex(string asciiString)
        {
            string hex = "";
            foreach (char c in asciiString)
            {
                int tmp = c;
                hex += String.Format("{0:x2}", (uint)System.Convert.ToUInt32(tmp.ToString()));
            }
            return hex;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            RememberCrd("LoadCredentials");
            string connString = "SERVER = " + sql.ip + ";PORT="+ sql.port +";DATABASE="+ sql.database +";UID=" + sql.username + ";PASSWORD='" + sql.password + "'";
          
            
            try
            {

               conn = new MySqlConnection();
                conn.ConnectionString = connString;
                conn.Open();


            }
            
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show("Eroare 107 / Posibil sa fie nevoie de update");
            }
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM Status",conn);
            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
           metroLabel4.Text = reader.GetString(2);
            if (reader.GetString(2) == "Up") metroButton1.Enabled = true;
            conn.Close();
            reader.Close();
        }

        MySqlConnection conn;
        bool okflaglogare = false;
        int cateaumers = 0;
        private bool RememberCrd (string WhatTodo)
        {
            
            string location = System.IO.Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents").ToString() + @"\E";
            if (WhatTodo == "SaveCredentials"  )
            {
                RememberCrd("DeleteCredentials");
                if (!Directory.Exists(location))
                {
                    Directory.CreateDirectory(location);
                    
                }
                using (StreamWriter file = new StreamWriter(location + @"\CRD"))
                { file.WriteLine(metroTextBox1.Text + " " + metroTextBox2.Text);  }

            } else
            if (WhatTodo == "LoadCredentials" && RememberCrd("CheckIfFileExist"))
            {
                
                    using (StreamReader sr = new StreamReader(location + @"\CRD", true))
                {
                    string line = sr.ReadToEnd();
                    string nume = line.Split(' ')[0];
                  
                    string parola = line.Substring(line.IndexOf(' ') + 1);                   
                   
                    metroTextBox1.Text = nume;
                    metroTextBox2.Text = parola;
                    metroTextBox2.Text = metroTextBox2.Text.Replace(" ", "");
                    metroTextBox2.Text = metroTextBox2.Text.Replace("\r\n", "");
                    RememberBOX.Checked = true;
                }
            
            } else
            if (WhatTodo == "CheckIfFileExist")
            {
                return (File.Exists(location + @"\CRD"));
            } else
            if (WhatTodo =="DeleteCredentials")
            {
                if (RememberCrd("CheckIfFileExist")) File.Delete(location + @"\CRD");
                return true;
            }

        

        
          return false;
        }
        private void metroButton1_Click(object sender, EventArgs e)
        {
            if (RememberBOX.Checked) RememberCrd("SaveCredentials"); else RememberCrd("DeleteCredentials");
            
            string nume = metroTextBox1.Text;
            string parola = metroTextBox2.Text;
            string idpc = Hashhhh(); idpc.Replace(" ", ""); idpc = ConvertStringToHex(idpc);

            // MessageBox.Show(idpc);
            string connString = "SERVER = " + sql.ip + ";PORT=" + sql.port + ";DATABASE=" + sql.database + ";UID=" + sql.username + ";PASSWORD='" + sql.password + "'";
            try
            {

                conn = new MySqlConnection();
                conn.ConnectionString = connString;
                conn.Open();
               

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show("Eroare 142 / Posibil sa fie nevoie de update");
            }

            try
            {


                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Users WHERE username = '"+nume+"' ;", conn);
                                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();

                if (reader.GetString(0) == nume) cateaumers++;
               if (reader.GetString(1) == parola) cateaumers++;
                if (reader.GetString(2) == idpc) cateaumers++;
                Variabile.username = nume;
               
                if (cateaumers == 3 ) okflaglogare = true;
                

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show("Eroare 164 / Posibil sa fie nevoie de update");
            }
           
            if (okflaglogare == true)
            {
                Form meniu = new Meniu();
                this.Hide();
                meniu.Show();
            }

            if (okflaglogare == false)
            {
                MessageBox.Show("Eroare 180 : Datele de logare gresite / HWID invalid");
            }

        }

        private void metroButton2_Click(object sender, EventArgs e)
        {
            Form meniu = new Meniu();
            this.Hide();
            meniu.Show();
        }

        private void metroLabel2_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void RememberBOX_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
