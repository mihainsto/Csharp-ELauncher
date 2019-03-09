using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using MySql.Data.MySqlClient;
using System.Net;
using System.IO;
using System.Media;
namespace E
{

    public partial class Meniu : MetroFramework.Forms.MetroForm
    {

        public Meniu()
        {
            InitializeComponent();
        }
         
        int safe = 0;
        MySqlConnection conn;

        private string ToUpperFirstLetter(string source)
        {
            if (string.IsNullOrEmpty(source))
                return string.Empty;
            
            char[] letters = source.ToCharArray();
            
            letters[0] = char.ToUpper(letters[0]);
            
            return new string(letters);
        }
        private void Meniu_Load(object sender, EventArgs e)
        {
           string  stringnumeptlabel = Variabile.username;
            

            metroLabel1.Text = "Welcome, " + ToUpperFirstLetter(stringnumeptlabel) + "!";
            string connString = "SERVER = " + sql.ip + ";PORT=" + sql.port + ";DATABASE=" + sql.database + ";UID=" + sql.username + ";PASSWORD='" + sql.password + "'";
            int versiuneauser = 0;

            try
            {

                conn = new MySqlConnection();
                conn.ConnectionString = connString;
                conn.Open();


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show(ex.Message);
                MessageBox.Show("Eroare 180 / Posibil sa fie nevoie de update");
            }

            try
            {


                MySqlCommand cmd = new MySqlCommand("SELECT * FROM Status  ", conn);
                
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                 safe = reader.GetInt32(1);
                int versiune = reader.GetInt32(0);
                reader.Close();
                //conn.Open();
                cmd = new MySqlCommand("SELECT (version) FROM Users WHERE username = '"+Variabile.username+"';", conn);
                reader = cmd.ExecuteReader();
                reader.Read();
                versiuneauser = reader.GetInt32(0);
               

                if (safe == 1) { label1.ForeColor = System.Drawing.Color.Lime; label1.Text = "Working";   }
                if (safe == 0) { label1.ForeColor = System.Drawing.Color.Red; label1.Text = "Not Working"; startBTN.Enabled = false; }
                if (versiune == versiuneauser) { label4.ForeColor = System.Drawing.Color.Lime; label4.Text = "Updated";  }
                else { label4.ForeColor = System.Drawing.Color.Red; label4.Text = "Needs update"; label1.ForeColor = System.Drawing.Color.Orange; label1.Text = "Not Working (update needed)"; startBTN.Enabled = false; }
                conn.Close();
                reader.Close();
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                // MessageBox.Show(ex.Message);
                MessageBox.Show("Eroare 211 / Posibil sa fie nevoie de update");
            }
        }
       protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if ( Variabile.Closeflag == true)
                Environment.Exit(0);
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            label4.ForeColor = System.Drawing.Color.Lime; label4.Text = "Updated*";
            if (safe == 1) { label1.ForeColor = System.Drawing.Color.Lime; label1.Text = "Working"; startBTN.Enabled = true; }
            if (safe == 0) { label1.ForeColor = System.Drawing.Color.Red; label1.Text = "Not Working"; startBTN.Enabled = false; }

            string connString = "SERVER = " + sql.ip + ";PORT=" + sql.port + ";DATABASE=" + sql.database + ";UID=" + sql.username + ";PASSWORD='" + sql.password + "'";

            try
            {

                conn = new MySqlConnection();
                conn.ConnectionString = connString;
                conn.Open();


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show(ex.Message);
                MessageBox.Show("Eroare 271 / Posibil sa fie nevoie de update");
            }

            
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM Status  ", conn);

            MySqlDataReader reader = cmd.ExecuteReader();
            reader.Read();
            int versiunenoua = reader.GetInt32(0);
            reader.Close();

            MySqlCommand update = new MySqlCommand("UPDATE  Users SET version = "+versiunenoua+" WHERE (username = '"+Variabile.username+"');", conn);
            reader = update.ExecuteReader();
            reader.Close();
            conn.Close();
        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            metroButton1.Enabled = false;
            string url;
            string connString = "SERVER = " + sql.ip + ";PORT=" + sql.port + ";DATABASE=" + sql.database + ";UID=" + sql.username + ";PASSWORD='" + sql.password + "'";
            try
            {

                conn = new MySqlConnection();
                conn.ConnectionString = connString;
                conn.Open();


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                //MessageBox.Show(ex.Message);
                MessageBox.Show("Eroare 309 / Posibil sa fie nevoie de update");
            }
            try
            {
                MySqlCommand cmd = new MySqlCommand("SELECT (link) FROM Users WHERE username = '" + Variabile.username + "';", conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                reader.Read();
                url = reader.GetString(0);

                reader.Close();
                conn.Close();

                WebClient client = new WebClient();
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(url), @"C:\desc.txt");
            }
            catch
            {
                MessageBox.Show("Eroare Fatala 328 / Eroare update");
                return;
            }
            
            
           
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void startBTN_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"C:\desc.txt");

        }
    }
    }

