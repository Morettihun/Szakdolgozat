using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using AllConnect;
using System.IO;
using MySql.Data.MySqlClient;
using Npgsql;
using Couchbase;
using Couchbase.Configuration.Client;
using System.Diagnostics;
using Newtonsoft.Json;
namespace SzakDolg_SP_2018
{
    public partial class Form1 : Form
    {
        
        static string connectionString = "SERVER=localhost;PORT=3306;" + "DATABASE=szakdolgozat;" + "UID=sarlpa;" + "PASSWORD=Patrik;";
        MySqlConnection conn = new MySqlConnection(connectionString);

        static string connectionString2 = "SERVER=localhost;PORT=3306;" + "DATABASE=information_schema;" + "UID=sarlpa;" + "PASSWORD=Patrik;";
        MySqlConnection conn2 = new MySqlConnection(connectionString2);
        
        string generatedpath = @"datas\generatedstudents.csv";
        string logpath_mysql = @"log\mysql_log.csv";
        string logpath_postgresql = @"log\postgresql_log.csv";
        Stopwatch run_Timing = new Stopwatch();
        public Form1()
        {
            InitializeComponent();

        }

        //Logba írás
        private void log_Query(string source, string querytype, int rows, long el_time)
        {
            string newLine = string.Format("{0};{1};{2};{3}", DateTime.Now.ToString("yyyy.MM.dd_HH:mm:ss"), querytype, rows, el_time + Environment.NewLine);
            File.AppendAllText(source, newLine, Encoding.UTF8);
        }

        public int runMySQL(MySqlCommand cmd)
        {
            int rows = 0;
            
            cmd.CommandType = CommandType.Text;
            cmd.Connection = conn;
            
            run_Timing.Start();
            rows += cmd.ExecuteNonQuery();
            
            run_Timing.Stop();
            return rows;
        }

        //Adatok generálása random
        private void button_Generate_Click(object sender, EventArgs e)
        {

            GC.Collect();
            //CSV létrehozó
            StringBuilder csvbuild = new StringBuilder();
            //Nevek kigyűjtése a 2 txt-ből
            string[] ffinevek = File.ReadAllLines(@"datas\osszesffi.txt");
            string[] noinevek = File.ReadAllLines(@"datas\osszesnoi.txt");

            string NKchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; //Karakterek a neptun kódhoz.
            Random rnd = new Random();

            //Feltöltendő adatok stringje
            string rndNK = null;
            string rndname = null;
            string rndemail = null;
            int varosid = 0;

            for (int NoS = 0; NoS < Convert.ToInt32(nUD_Generate.Value); NoS++)
            {
                //Random Neptun Kód
                rndNK = null;
                for (int nki = 0; nki < 6; nki++)
                {
                    rndNK += NKchars[rnd.Next(0, NKchars.Length)];
                }
                //Random név
                rndname = ffinevek[rnd.Next(0, ffinevek.Length - 1)] + " " + noinevek[rnd.Next(0, noinevek.Length - 1)];
                //Random email
                rndemail = rndname.Replace(" ", "_") + rnd.Next(0, 99999) + "@gmail.com";
                //Random város ID.
                varosid = rnd.Next(0, 4154);
                //CSV-be írás
                string newLine = string.Format("{0};{1};{2};{3}", rndNK, rndname, rndemail, varosid);

                csvbuild.AppendLine(newLine);
            }
            try
            {
                File.WriteAllText(generatedpath, csvbuild.ToString(), Encoding.UTF8);
                MessageBox.Show("Sikeres generálás.", "Siker");
                GC.Collect();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                GC.Collect();
            }

        }

#region MySQL
        //Random adatok feltöltése
        private void button1_Click(object sender, EventArgs e)
        {
            int rows = 0;
            run_Timing.Reset();
            try
            {
                StreamReader srgen = new StreamReader(generatedpath);               
                conn.Open();

                MySqlCommand comm2 = new MySqlCommand();
                comm2.CommandType = CommandType.Text;
                comm2.Connection = conn;
                comm2.CommandText = "SET profiling=1;";
                runMySQL(comm2);

                

                while (!srgen.EndOfStream)
                {
                    var line = srgen.ReadLine();
                    var datas = line.Split(';');

                    MySqlCommand comm = new MySqlCommand();
                    comm.CommandText = "INSERT INTO tanulok VALUES(null,@NK,@Nev,@Email,@Varos_id);";
                    comm.Parameters.Add(new MySqlParameter("@NK", MySqlDbType.VarChar)).Value = datas[0];
                    comm.Parameters.Add(new MySqlParameter("@Nev", MySqlDbType.VarChar)).Value = datas[1];
                    comm.Parameters.Add(new MySqlParameter("@Email", MySqlDbType.VarChar)).Value = datas[2];
                    comm.Parameters.Add(new MySqlParameter("@Varos_id", MySqlDbType.Int32)).Value = datas[3];

                    rows = runMySQL(comm);

                    //comm2.CommandText = "SHOW PROFILE;";

                    //MySqlDataReader dr = comm2.ExecuteReader();


                   // while (dr.Read())
                    //{
                    //    richTextBox1.Text += dr[0].ToString() + "-" + dr[1].ToString() + Environment.NewLine;
                        //MessageBox.Show(dr[0].ToString());
                   // }
                  
                    


                    label_MySqlConn.Text = "A query futása " + run_Timing.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: "+rows;                   
                }
                conn.Close();
                log_Query(logpath_mysql, "DML_INSERT", rows, run_Timing.ElapsedMilliseconds);
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                conn.Close();
            }
        }

        

        private void button3_Click(object sender, EventArgs e)
        {
            int rows = 0;
            run_Timing.Reset();
            try
            {
                conn.Open();

                    MySqlCommand comm = new MySqlCommand();
                    comm.CommandText = @"LOAD DATA INFILE 'generatedstudents.csv' INTO TABLE tanulok columns TERMINATED BY ';' LINES TERMINATED BY '" + Environment.NewLine + "' (NK,Nev,Email,Varos_id) SET ID=NULL;";

                    rows = runMySQL(comm);

                    label_MySqlConn.Text = "A query futása " + run_Timing.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rows;               

                conn.Close();
                log_Query(logpath_mysql, "LOAD_DATA", rows, run_Timing.ElapsedMilliseconds);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                conn.Close();
            }
        }

        private void button_DeleteMys_Click(object sender, EventArgs e)
        {
            int rows = 0;
            run_Timing.Reset();
             //figyelmeztetés
            DialogResult dialogResult = MessageBox.Show("Biztosan törölni szeretné az összes adatot az adatbázisból?", "Figyelmeztetés!", MessageBoxButtons.YesNo);
            // ha igen
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    conn.Open();
                    MySqlCommand comm = new MySqlCommand();
                    comm.CommandText = "DELETE FROM tanulok WHERE 1;";

                    rows = runMySQL(comm);

                    conn.Close();
                    label_MySqlConn.Text = "A query futása " + run_Timing.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rows;

                    log_Query(logpath_mysql, "DELETE", rows, run_Timing.ElapsedMilliseconds);                   
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                    conn.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                    conn.Close();
                }
            }


        }


        private void button_SelectMys_Click(object sender, EventArgs e)
        {
            int rows = 0;
            run_Timing.Reset();
            try
            {
                conn.Open();
                MySqlCommand comm = new MySqlCommand();
                comm.CommandText = "SELECT * FROM Tanulok;";

                rows = runMySQL(comm);

                conn.Close();
                label_MySqlConn.Text = "A query futása " + run_Timing.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rows;

                log_Query(logpath_mysql, "SELECT *", rows, run_Timing.ElapsedMilliseconds);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                conn.Close();
            }
        }
#endregion
        #region PostGRESQL

        #endregion

        #region CouchBase
        private void viu()
        {
            run_Timing.Reset();
            List<string> listNK = new List<string>();
            List<string> listName = new List<string>();
            List<string> listEmail = new List<string>();
            List<string> listPostalID = new List<string>();
            using (var reader = new StreamReader(@"datas\generatedstudents.csv"))
            {
                
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    listNK.Add(values[0]);
                    listName.Add(values[1]);
                    listEmail.Add(values[2]);
                    listPostalID.Add(values[3]);
                }
            }
            try
            {
                var cb = new Cluster(new ClientConfiguration
                {
                    Servers = new List<Uri> { new Uri("https://127.0.0.1:8091/") }
                });
                cb.Authenticate("admin", "Patrik");
                var bucket = cb.OpenBucket("szakd");
                //string qq = "SELECT country FROM `travel-sample` WHERE name=\"Texas Wings\";";
                //var result = bucket.Query<dynamic>(qq);
                //MessageBox.Show(result.Metrics.ExecutionTime);
                /*//--Document--
                for (int i = 0; i < listNK.Count; i++)
                {


                    var incr = bucket.Increment("Incrementer", 1);
                    if (incr.Success)
                    {
                        
                        var newdoc = new Document<dynamic>
                        {
                            Id = "Tanulo_" + incr.Value,
                            Content = new
                            {
                                NK = listNK[i],
                                Nev = listName[i],
                                Email = listEmail[i],
                                Varos_id = listPostalID[i]
                            }
                        };
                        run_Timing.Start();
                        var ups = bucket.Insert(newdoc);
                        run_Timing.Stop();
                    }
                    else
                    {
                        MessageBox.Show(incr.Message);
                    }
                }
                MessageBox.Show(run_Timing.ElapsedMilliseconds.ToString());*/
                
                //--Dictionary--
                var dic = new Dictionary<string, dynamic>();
                
                    for (int i = 0; i < listNK.Count; i++)
                    {

                        var incr = bucket.Increment("Incrementer", 1);
                        if (incr.Success)
                        {
                        var Content = new
                                {
                                    NK = listNK[i],
                                    Nev = listName[i],
                                    Email = listEmail[i],
                                    Varos_id = listPostalID[i]
                                };
                        //string json = JsonConvert.SerializeObject(Content);
                        dic.Add(incr.Value.ToString(), Content);                    
                          }                    
                    }
                    run_Timing.Start();
                    bucket.Upsert(dic);
                    run_Timing.Stop();
                    MessageBox.Show(run_Timing.ElapsedMilliseconds.ToString());

             }
            catch(CouchbaseResponseException ex)
            {
                MessageBox.Show(ex.Message);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //var bc = new BucketConfiguration();
            //bc.Password = "Patrik";
            //bc.Username = "admin";
            //bc.BucketName = "travel-sample";

            //bc.Servers.Clear();
            //bc.Servers.Add(new Uri("http://127.0.0.1:8091/"));
            
        }
        
        #endregion

        private void button_QueryMys_Click(object sender, EventArgs e)
        {

            int rows = 0;
            run_Timing.Reset();
            //List<int> rt = new List<int>();
            //List<long> elms = new List<long>();
            try
            {
                conn.Open();

                MySqlCommand comm = new MySqlCommand();
                comm.CommandText = rTB_QueryMys.Text;
                for (int i = 0; i < nUd_HanyszorMyS.Value; i++)
                {
                    run_Timing.Reset();
                    rows = runMySQL(comm);
                    log_Query(logpath_mysql, "Egyedi: " + rTB_QueryMys.Text, rows, run_Timing.ElapsedMilliseconds);
                    //rt.Add(rows);
                    //elms.Add(run_Timing.ElapsedMilliseconds);
                }
               
                label_MySqlConn.Text = "A query futása " + run_Timing.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rows;
                conn.Close();
                //for (int i = 0; i < nUd_HanyszorMyS.Value; i++)
                //{
                //    log_Query(logpath_mysql, "Egyedi: " + rTB_QueryMys.Text, rt[i], elms[i]);
                //}
                
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                conn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                conn.Close();
            }
        }

        private void button_UpdateMys_Click(object sender, EventArgs e)
        {
            int rows = 0;
            run_Timing.Reset();
            try
            {
                conn.Open();

                MySqlCommand comm = new MySqlCommand();
                comm.CommandText = "UPDATE Tanulok SET Email='updatelt@gmail.com' WHERE 1";

                rows = runMySQL(comm);

                label_MySqlConn.Text = "A query futása " + run_Timing.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rows;
                conn.Close();

                log_Query(logpath_mysql, "UPDATE", rows, run_Timing.ElapsedMilliseconds);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                conn.Close();
            }
            
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                conn.Close();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            viu();
        }

    }
}
