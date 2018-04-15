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
using System.Threading;
using Couchbase.N1QL;
namespace SzakDolg_SP_2018
{
    public partial class Form1 : Form
    {
        
        static string connStringMyS = "SERVER=localhost;PORT=3306;" + "DATABASE=szakdolgozat;" + "UID=sarlpa;" + "PASSWORD=Patrik;";
        MySqlConnection connMyS = new MySqlConnection(connStringMyS);

        static string connStringPG = "SERVER=localhost;PORT=5432;" + "DATABASE=szakdolgozat;" + "UID=postgres;" + "PASSWORD=Patrik;Command Timeout=0";
        NpgsqlConnection connPG = new NpgsqlConnection(connStringPG);

        //static string connectionString2 = "SERVER=localhost;PORT=3306;" + "DATABASE=information_schema;" + "UID=sarlpa;" + "PASSWORD=Patrik;";
       // MySqlConnection conn2 = new MySqlConnection(connectionString2);
        
        string generatedpath = @"datas\generatedstudents.csv";
        string logpath_mysql = @"log\mysql_log.csv";
        string logpath_postgresql = @"log\postgresql_log.csv";
        string logpath_couchbase = @"log\couchbase_log.csv";
        Stopwatch run_Time_MyS = new Stopwatch();
        Stopwatch run_Time_PG = new Stopwatch();
        Stopwatch run_Time_CB = new Stopwatch();
        int rowsMyS = 0;
        int rowsPG = 0;
        int rowsCB = 0;

        private BackgroundWorker bgwork_MyS;
        private BackgroundWorker bgwork_PG;
        private BackgroundWorker bgwork_CB;

        string egyediPGquery = null;
        string egyediMySquery = null;
        string egyediCBN1QL = null;
        public Form1()
        {
            InitializeComponent();
        }


        #region basis
        //Logba írás
        private void log_Query(string source, string querytype, int rows, long el_time)
        {
            string newLine = string.Format("{0};{1};{2};{3}", DateTime.Now.ToString("yyyy.MM.dd_HH:mm:ss"), querytype, rows, el_time + Environment.NewLine);
            File.AppendAllText(source, newLine, System.Text.Encoding.UTF8);
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
                File.WriteAllText(generatedpath, csvbuild.ToString(), System.Text.Encoding.UTF8);
                MessageBox.Show("Sikeres generálás.", "Siker");
                GC.Collect();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                GC.Collect();
            }

        }
        #endregion

        #region MySQL
       // public int runMySQL(MySqlCommand cmd)
        private void runMySQL(MySqlCommand cmd)
        {
            //rows = 0;


            cmd.CommandType = CommandType.Text;
            cmd.Connection = connMyS;

            run_Time_MyS.Start();
            rowsMyS += cmd.ExecuteNonQuery();

            run_Time_MyS.Stop();
            //return rows;
        }
        void bgwork_MyS_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label_MySqlConn.Text = "A query futása " + run_Time_MyS.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rowsMyS;
            pictureBox_MyS.Image = null;
        }

        void bgwork_MySInsert_DoWork(object sender, DoWorkEventArgs e)
        {
           rowsMyS = 0;
           try
           {
               pictureBox_MyS.Image = new Bitmap("images/loading.gif");
           }
           catch { }
            run_Time_MyS.Reset();
            try
            {
                //StreamReader srgen = new StreamReader(generatedpath);
                connMyS.Open();

                for (int i = 0; i < nUd_HanyszorMyS.Value; i++)
                {
                    rowsMyS = 0;
                    run_Time_MyS.Reset();
                    StreamReader srgen = new StreamReader(generatedpath);
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

                        //rows += runMySQL(comm);
                        runMySQL(comm);
                    }

                    log_Query(logpath_mysql, "DML_INSERT", rowsMyS, run_Time_MyS.ElapsedMilliseconds);
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");                
            }
            finally
            {
                connMyS.Close();
            }
        }

        //Random adatok feltöltése
        private void button_MySInsert_Click(object sender, EventArgs e)
        {
            bgwork_MyS = new BackgroundWorker();
            bgwork_MyS.WorkerSupportsCancellation = false;
            bgwork_MyS.DoWork += bgwork_MySInsert_DoWork;
            bgwork_MyS.RunWorkerCompleted += bgwork_MyS_RunWorkerCompleted;
            bgwork_MyS.RunWorkerAsync();
            
        }

        //Load Data-val feltöltés
        void bgwork_MySLoad_DoWork(object sender, DoWorkEventArgs e)
        {
            rowsMyS = 0;
            try
            {
                pictureBox_MyS.Image = new Bitmap("images/loading.gif");
            }
            catch { }
            run_Time_MyS.Reset();
            try
            {
                connMyS.Open();

                MySqlCommand comm = new MySqlCommand();
                comm.CommandText = @"LOAD DATA INFILE 'generatedstudents.csv' INTO TABLE tanulok columns TERMINATED BY ';' LINES TERMINATED BY '" + Environment.NewLine + "' (NK,Nev,Email,Varos_id) SET ID=NULL;";

                //rows = runMySQL(comm);
                runMySQL(comm);

                log_Query(logpath_mysql, "LOAD_DATA", rowsMyS, run_Time_MyS.ElapsedMilliseconds);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");                
            }
            finally
            {
                connMyS.Close();
            }
        }

        //Load Data-val feltöltés
        private void button_MySLoad_Click(object sender, EventArgs e)
        {
            bgwork_MyS = new BackgroundWorker();
            bgwork_MyS.WorkerSupportsCancellation = false;
            bgwork_MyS.DoWork += bgwork_MySLoad_DoWork;
            bgwork_MyS.RunWorkerCompleted += bgwork_MyS_RunWorkerCompleted;
            bgwork_MyS.RunWorkerAsync();
        }

        //Adatok törlése
        void bgwork_MySDelete_DoWork(object sender, DoWorkEventArgs e)
        {
            rowsMyS = 0;
            run_Time_MyS.Reset();
            try
            {
                pictureBox_MyS.Image = new Bitmap("images/loading.gif");
            }
            catch { }
            //figyelmeztetés
            DialogResult dialogResult = MessageBox.Show("Biztosan törölni szeretné az összes adatot az adatbázisból?", "Figyelmeztetés!", MessageBoxButtons.YesNo);
            // ha igen
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    connMyS.Open();
                    MySqlCommand comm = new MySqlCommand();
                    comm.CommandText = "DELETE FROM tanulok WHERE 1;";

                   // rows = runMySQL(comm);
                    runMySQL(comm);
                    
                    //label_MySqlConn.Text = "A query futása " + run_Time_MyS.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rows;

                    log_Query(logpath_mysql, "DELETE", rowsMyS, run_Time_MyS.ElapsedMilliseconds);
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
                }
                finally
                {
                    connMyS.Close();
                }
            }
        }

        private void button_DeleteMys_Click(object sender, EventArgs e)
        {
            bgwork_MyS = new BackgroundWorker();
            bgwork_MyS.WorkerSupportsCancellation = false;
            bgwork_MyS.DoWork += bgwork_MySDelete_DoWork;
            bgwork_MyS.RunWorkerCompleted += bgwork_MyS_RunWorkerCompleted;
            bgwork_MyS.RunWorkerAsync();
        }

        //Select * futtatása
        void bgwork_MySSelect_DoWork(object sender, DoWorkEventArgs e)
        {
            rowsMyS = 0;
            run_Time_MyS.Reset();
            try
            {
                pictureBox_MyS.Image = new Bitmap("images/loading.gif");
            }
            catch { }
            try
            {
                connMyS.Open();
                MySqlCommand comm = new MySqlCommand();
                comm.CommandText = "SELECT * FROM Tanulok;";

                // rows = runMySQL(comm);
                runMySQL(comm);

                //label_MySqlConn.Text = "A query futása " + run_Time_MyS.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rows;

                log_Query(logpath_mysql, "SELECT *", rowsMyS, run_Time_MyS.ElapsedMilliseconds);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
            }
            finally
            {
                connMyS.Close();
            }
        }

        private void button_SelectMys_Click(object sender, EventArgs e)
        {
            bgwork_MyS = new BackgroundWorker();
            bgwork_MyS.WorkerSupportsCancellation = false;
            bgwork_MyS.DoWork += bgwork_MySSelect_DoWork;
            bgwork_MyS.RunWorkerCompleted += bgwork_MyS_RunWorkerCompleted;
            bgwork_MyS.RunWorkerAsync();
        }

        //Egyedi Query
        void bgwork_MySQuery_DoWork(object sender, DoWorkEventArgs e)
        {
            rowsMyS = 0;
            run_Time_MyS.Reset();
            try
            {
                pictureBox_MyS.Image = new Bitmap("images/loading.gif");
            }
            catch { }
            //List<int> rt = new List<int>();
            //List<long> elms = new List<long>();
            try
            {
                connMyS.Open();

                MySqlCommand comm = new MySqlCommand();
                comm.CommandText = egyediMySquery;
                for (int i = 0; i < nUd_HanyszorMyS.Value; i++)
                {
                    run_Time_MyS.Reset();
                    rowsMyS = 0;
                    // rows = runMySQL(comm);
                    runMySQL(comm);
                    log_Query(logpath_mysql, "Egyedi: " + egyediMySquery, rowsMyS, run_Time_MyS.ElapsedMilliseconds);

                }

               // label_MySqlConn.Text = "A query futása " + run_Time_MyS.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rows;

            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
            }
            finally
            {
                connMyS.Close();
            }
        }

        //Egyedi query
        private void button_QueryMys_Click(object sender, EventArgs e)
        {
            egyediMySquery = rTB_QueryMys.Text;
            bgwork_MyS = new BackgroundWorker();
            bgwork_MyS.WorkerSupportsCancellation = false;
            bgwork_MyS.DoWork += bgwork_MySQuery_DoWork;
            bgwork_MyS.RunWorkerCompleted += bgwork_MyS_RunWorkerCompleted;
            bgwork_MyS.RunWorkerAsync();
            
        }
        //Adatok frissítése
        void bgwork_MySUpdate_DoWork(object sender, DoWorkEventArgs e)
        {
            rowsMyS = 0;
            run_Time_MyS.Reset();
            try
            {
                pictureBox_MyS.Image = new Bitmap("images/loading.gif");
            }
            catch { }
            try
            {
                connMyS.Open();

                MySqlCommand comm = new MySqlCommand();
                comm.CommandText = "UPDATE Tanulok SET Email='updatelt@gmail.com' WHERE 1";

                // rows = runMySQL(comm);
                runMySQL(comm);
                //label_MySqlConn.Text = "A query futása " + run_Time_MyS.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rows;

                log_Query(logpath_mysql, "UPDATE", rowsMyS, run_Time_MyS.ElapsedMilliseconds);
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
            }

            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
            }
            finally
            {
                connMyS.Close();
            }
        }
        private void button_UpdateMys_Click(object sender, EventArgs e)
        {
            bgwork_MyS = new BackgroundWorker();
            bgwork_MyS.WorkerSupportsCancellation = false;
            bgwork_MyS.DoWork += bgwork_MySUpdate_DoWork;
            bgwork_MyS.RunWorkerCompleted += bgwork_MyS_RunWorkerCompleted;
            bgwork_MyS.RunWorkerAsync();
        }
#endregion


        #region PostGRESQL

        void bgwork_PG_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label_PG.Text = "A query futása " + run_Time_PG.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rowsPG;
            pictureBox_PG.Image = null;
        }

        //Adatok feltöltése
        void bgwork_InsertPG_DoWork(object sender, DoWorkEventArgs e)
        {
            run_Time_PG.Reset();
            try
            {
                pictureBox_PG.Image = new Bitmap("images/loading.gif");
            }
            catch { }
            try
            {
                //StreamReader srgen = new StreamReader(generatedpath);
                rowsPG = 0;
                connPG.Open();
                for (int i = 0; i < nUD_EgyediPG.Value; i++)
                {
                    StreamReader srgen = new StreamReader(generatedpath);
                    rowsPG = 0;
                    run_Time_PG.Reset();
                    while (!srgen.EndOfStream)
                    {
                        var line = srgen.ReadLine();
                        var datas = line.Split(';');

                        NpgsqlCommand comm = connPG.CreateCommand();
                        comm.CommandText = "INSERT INTO szakdoga.\"Tanulok\" (\"NK\",\"Nev\",\"Email\",\"Varos_id\") VALUES (@NK,@Nev,@Email,@Varos_id);";
                        comm.Parameters.Add(new NpgsqlParameter("@NK", DbType.String)).Value = datas[0];
                        comm.Parameters.Add(new NpgsqlParameter("@Nev", DbType.String)).Value = datas[1];
                        comm.Parameters.Add(new NpgsqlParameter("@Email", DbType.String)).Value = datas[2];
                        comm.Parameters.Add(new NpgsqlParameter("@Varos_id", DbType.Int32)).Value = datas[3];

                        run_Time_PG.Start();
                        rowsPG += comm.ExecuteNonQuery();
                        run_Time_PG.Stop();

                        //label_PG.Text = "A feltöltés " + run_Time_PG.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rowsPG;
                    }
                    // connPG.Close();
                    log_Query(logpath_postgresql, "DML_INSERT", rowsPG, run_Time_PG.ElapsedMilliseconds);
                }
            }

            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connPG.Close();
            }
        }
        //Adatok feltöltése
        private void button_InsertPG_Click(object sender, EventArgs e)
        {
            bgwork_PG = new BackgroundWorker();
            bgwork_PG.WorkerSupportsCancellation = false;
            bgwork_PG.DoWork += bgwork_InsertPG_DoWork;
            bgwork_PG.RunWorkerCompleted += bgwork_PG_RunWorkerCompleted;
            bgwork_PG.RunWorkerAsync();
        }
        //Adatok törlése
        void bgwork_DeletePG_DoWork(object sender, DoWorkEventArgs e)
        {
            run_Time_PG.Reset();
            try
            {
                pictureBox_PG.Image = new Bitmap("images/loading.gif");
            }
            catch { }
            //figyelmeztetés
            DialogResult dialogResult = MessageBox.Show("Biztosan törölni szeretné az összes adatot az adatbázisból?", "Figyelmeztetés!", MessageBoxButtons.YesNo);
            // ha igen
            if (dialogResult == DialogResult.Yes)
            {
                try
                {

                    rowsPG = 0;
                    connPG.Open();

                    NpgsqlCommand comm = connPG.CreateCommand();
                    //comm.CommandText = "INSERT INTO szakdoga.\"Tanulok\" (\"NK\",\"Nev\",\"Email\",\"Varos_id\") VALUES (@NK,@Nev,@Email,@Varos_id);";
                    comm.CommandText = "DELETE FROM szakdoga.\"Tanulok\" WHERE 1=1;";
                    run_Time_PG.Start();
                    rowsPG += comm.ExecuteNonQuery();
                    run_Time_PG.Stop();

                    //label_PG.Text = "A törlés " + run_Time_PG.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rowsPG;


                    log_Query(logpath_postgresql, "DML_DELETE", rowsPG, run_Time_PG.ElapsedMilliseconds);
                }

                catch (NpgsqlException ex)
                {
                    MessageBox.Show(ex.Message);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                finally
                {
                    connPG.Close();
                }
            }
        }
        //Adatok törlése
        private void button_DeletePG_Click(object sender, EventArgs e)
        {
            bgwork_PG = new BackgroundWorker();
            bgwork_PG.WorkerSupportsCancellation = false;
            bgwork_PG.DoWork += bgwork_DeletePG_DoWork;
            bgwork_PG.RunWorkerCompleted += bgwork_PG_RunWorkerCompleted;
            bgwork_PG.RunWorkerAsync();
        }

        //Adatok lekérdezése
        void bgwork_SelectPG_DoWork(object sender, DoWorkEventArgs e)
        {
            run_Time_PG.Reset();
            try
            {
                pictureBox_PG.Image = new Bitmap("images/loading.gif");
            }
            catch { }
            try
            {

                rowsPG = 0;
                connPG.Open();

                NpgsqlCommand comm = connPG.CreateCommand();
                comm.CommandText = "SELECT * FROM szakdoga.\"Tanulok\"";
                run_Time_PG.Start();
                rowsPG += comm.ExecuteNonQuery();
                run_Time_PG.Stop();

                //label_PG.Text = "A lekérdezés " + run_Time_PG.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rows;


                log_Query(logpath_postgresql, "DML_SELECT", rowsPG, run_Time_PG.ElapsedMilliseconds);
            }

            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connPG.Close();
            }
        }
        //Adatok lekérdezése
        private void button_SelectPG_Click(object sender, EventArgs e)
        {
            bgwork_PG = new BackgroundWorker();
            bgwork_PG.WorkerSupportsCancellation = false;
            bgwork_PG.DoWork += bgwork_SelectPG_DoWork;
            bgwork_PG.RunWorkerCompleted += bgwork_PG_RunWorkerCompleted;
            bgwork_PG.RunWorkerAsync();

        }
        //Adatok frissítése
        void bgwork_UpdatePG_DoWork(object sender, DoWorkEventArgs e)
        {
            run_Time_PG.Reset();
            try
            {
                pictureBox_PG.Image = new Bitmap("images/loading.gif");
            }
            catch { }
            try
            {

                rowsPG = 0;
                connPG.Open();

                NpgsqlCommand comm = connPG.CreateCommand();
                comm.CommandText = "UPDATE szakdoga.\"Tanulok\" SET \"Email\"='updatelt@gmail.com' WHERE 1=1;";
                run_Time_PG.Start();
                rowsPG += comm.ExecuteNonQuery();
                run_Time_PG.Stop();

                //label_PG.Text = "A módosítás " + run_Time_PG.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rows;


                log_Query(logpath_postgresql, "DML_UPDATE", rowsPG, run_Time_PG.ElapsedMilliseconds);
            }

            catch (NpgsqlException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                connPG.Close();
            }
        }
        //Adatok frissítése
        private void button_UpdatePG_Click(object sender, EventArgs e)
        {
            bgwork_PG = new BackgroundWorker();
            bgwork_PG.WorkerSupportsCancellation = false;
            bgwork_PG.DoWork += bgwork_UpdatePG_DoWork;
            bgwork_PG.RunWorkerCompleted += bgwork_PG_RunWorkerCompleted;
            bgwork_PG.RunWorkerAsync();
        }
        //Egyedi Query
        void bgwork_EgyediPG_DoWork(object sender, DoWorkEventArgs e)
        {
            run_Time_PG.Reset();
            try
            {
                pictureBox_PG.Image = new Bitmap("images/loading.gif");
            }
            catch { }
            try
            {
                rowsPG = 0;
                connPG.Open();

                NpgsqlCommand comm = connPG.CreateCommand();
                comm.CommandText = egyediPGquery;
                for (int i = 0; i < nUD_EgyediPG.Value; i++)
                {
                    run_Time_PG.Reset();
                    run_Time_PG.Start();
                    rowsPG = comm.ExecuteNonQuery();
                    run_Time_PG.Stop();

                    log_Query(logpath_postgresql, "Egyedi: " + egyediPGquery, rowsPG, run_Time_PG.ElapsedMilliseconds);
                    
                }

                //label_PG.Text = "A query futása " + run_Time_PG.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rows;


            }
            catch (NpgsqlException ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hiba történt:\n" + ex.Message, "Hiba");
            }
            finally
            {
                connPG.Close();
            }
        }
        private void button_EgyediPG_Click(object sender, EventArgs e)
        {
            egyediPGquery = rTB_QueryPG.Text;
            bgwork_PG = new BackgroundWorker();
            bgwork_PG.WorkerSupportsCancellation = false;
            bgwork_PG.DoWork += bgwork_EgyediPG_DoWork;
            bgwork_PG.RunWorkerCompleted += bgwork_PG_RunWorkerCompleted;
            bgwork_PG.RunWorkerAsync();
           
        }
        #endregion
    

        #region CouchBase
        void bgwork_CB_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label_CouchBase.Text = "A query futása " + run_Time_CB.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rowsCB;
            pictureBox_CB.Image = null;
        }
        void bgwork_InsertCouchB_DoWork(object sender, DoWorkEventArgs e)
        {
            run_Time_CB.Reset();
            rowsCB = 0;

            try
            {
                pictureBox_CB.Image = new Bitmap("images/loading.gif");
            }
            catch { }

            //Adatok beolvasása
            List<string> listNK = new List<string>();
            List<string> listName = new List<string>();
            List<string> listEmail = new List<string>();
            List<string> listPostalID = new List<string>();
            try
            {
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
            }
            catch { }
            //Kapcsolódás
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
                        run_Time_CB.Start();
                        var ups = bucket.Insert(newdoc);
                        run_Time_CB.Stop();
                    }
                    else
                    {
                        MessageBox.Show(incr.Message);
                    }
                }
                MessageBox.Show(run_Time_CB.ElapsedMilliseconds.ToString());*/

                for (int j = 0; j < nUD_EgyediN1QL.Value; j++)
                {
                    run_Time_CB.Reset();
                    rowsCB = 0;

                    //--Dictionary--
                    var dic = new Dictionary<string, dynamic>();

                    for (int i = 0; i < listNK.Count; i++)
                    {
                        //ID növelés
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
                    //Mérés kezdete
                    run_Time_CB.Start();
                    var result = bucket.Upsert(dic);

                    run_Time_CB.Stop(); //..Vége
                    rowsCB = result.Count;
                    log_Query(logpath_couchbase, "INSERT", rowsCB, run_Time_CB.ElapsedMilliseconds);
                }
                //label_CouchBase.Text = "Az INSERT futása " + run_Time_CB.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + result.Count;
                //MessageBox.Show(run_Time_CB.ElapsedMilliseconds.ToString());
            }

            catch (CouchbaseResponseException ex)
            {
                MessageBox.Show(ex.Message);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void button_InsertCouchB_Click(object sender, EventArgs e)
        {
            bgwork_CB = new BackgroundWorker();
            bgwork_CB.WorkerSupportsCancellation = false;
            bgwork_CB.DoWork += bgwork_InsertCouchB_DoWork;
            bgwork_CB.RunWorkerCompleted += bgwork_CB_RunWorkerCompleted;
            bgwork_CB.RunWorkerAsync();
        }

        //Adatok törlése
        void bgwork_FlushCouchB_DoWork(object sender, DoWorkEventArgs e)
        {
            //figyelmeztetés
            DialogResult dialogResult = MessageBox.Show("Biztosan törölni szeretné az összes adatot a Bucketből?", "Figyelmeztetés!", MessageBoxButtons.YesNo);
            // ha igen
            if (dialogResult == DialogResult.Yes)
            {
                run_Time_CB.Reset();
                rowsCB = 0;
                try
                {
                    pictureBox_CB.Image = new Bitmap("images/loading.gif");
                }
                catch { }
                try
                {
                    var cb = new Cluster(new ClientConfiguration
                    {
                        Servers = new List<Uri> { new Uri("https://127.0.0.1:8091/") }
                    });
                    cb.Authenticate("admin", "Patrik");
                    var bucket = cb.OpenBucket("szakd");
                    //Adatok megszámlálása
                    string qq = "SELECT COUNT(*) FROM szakd;";
                    var queryresult = bucket.Query<dynamic>(qq); //Query futtatása
                    var jObject = Newtonsoft.Json.Linq.JObject.Parse(queryresult.Rows[0].ToString()); //Visszakapott JSON üzenet deserializálása
                    rowsCB = (int)jObject["$1"];
                    //Bucket Flush
                    Couchbase.Management.IBucketManager cc = cb.OpenBucket("szakd").CreateManager("admin", "Patrik");
                    run_Time_CB.Start();
                    IResult flushres = cc.Flush();
                    run_Time_CB.Stop();
                    if (flushres.Success)
                    {
                        log_Query(logpath_couchbase, "FLUSH", rowsCB, run_Time_CB.ElapsedMilliseconds);
                        //label_CouchBase.Text = "A Flush futása " + run_Time_CB.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rowsCB ;
                    }

                }
                catch (CouchbaseResponseException ex)
                {
                    MessageBox.Show(ex.Message);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        //Adatok törlése
        private void button_FlushCouchB_Click(object sender, EventArgs e)
        {
            bgwork_CB = new BackgroundWorker();
            bgwork_CB.WorkerSupportsCancellation = false;
            bgwork_CB.DoWork += bgwork_FlushCouchB_DoWork;
            bgwork_CB.RunWorkerCompleted += bgwork_CB_RunWorkerCompleted;
            bgwork_CB.RunWorkerAsync();
        }

        //Adatok lekérdezése
        void bgwork_SelectCouchB_DoWork(object sender, DoWorkEventArgs e)
        {
            run_Time_CB.Reset();
            rowsCB = 0;
            try
            {
                pictureBox_CB.Image = new Bitmap("images/loading.gif");
            }
            catch { }

            try
            {
                var cb = new Cluster(new ClientConfiguration
                {
                    Servers = new List<Uri> { new Uri("https://127.0.0.1:8091/") }
                });
                cb.Authenticate("admin", "Patrik");
                var bucket = cb.OpenBucket("szakd");
                //Adatok lekérése
                string qq = "SELECT * FROM szakd;";


                run_Time_CB.Start();
                var queryresult = bucket.Query<dynamic>(qq); //Query futtatása
                run_Time_CB.Stop();
                rowsCB = queryresult.Rows.Count;

                if (queryresult.Success)
                {
                    log_Query(logpath_couchbase, "SELECT *", rowsCB, run_Time_CB.ElapsedMilliseconds);
                   // label_CouchBase.Text = "A query futása " + run_Time_CB.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rowsCB;
                }

            }

            catch (CouchbaseResponseException ex)
            {
                MessageBox.Show(ex.Message);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //Adatok lekérdezése
        private void button_SelectCouchB_Click(object sender, EventArgs e)
        {
            bgwork_CB = new BackgroundWorker();
            bgwork_CB.WorkerSupportsCancellation = false;
            bgwork_CB.DoWork += bgwork_SelectCouchB_DoWork;
            bgwork_CB.RunWorkerCompleted += bgwork_CB_RunWorkerCompleted;
            bgwork_CB.RunWorkerAsync();
        }

        //Egyedi N1QL futtatása
        void bgwork_EgyediCouchB_DoWork(object sender, DoWorkEventArgs e)
        {
            run_Time_CB.Reset();
            rowsCB = -1;
            try
            {
                pictureBox_CB.Image = new Bitmap("images/loading.gif");
            }
            catch { }
            try
            {
                var cb = new Cluster(new ClientConfiguration
                {
                    QueryRequestTimeout = 750000,
                    Servers = new List<Uri> { new Uri("https://127.0.0.1:8091/") }
                });
                cb.Authenticate("admin", "Patrik");
                cb.Configuration.QueryRequestTimeout = 800000;

                using (var bucket = cb.OpenBucket("szakd"))
                {
                    var queryreq = new QueryRequest()
                    .Timeout(new TimeSpan(0, 0, 0, 0, 5));


                    //Adatok lekérése
                    //string qq = egyediCBN1QL;
                    for (int i = 0; i < nUD_EgyediN1QL.Value; i++)
                    {
                        run_Time_CB.Reset();
                        run_Time_CB.Start();
                        var queryresult = bucket.Query<dynamic>(egyediCBN1QL); //Query futtatása
                        run_Time_CB.Stop();
                        if (egyediCBN1QL.Contains("UPDATE szakd") || egyediCBN1QL.Contains("DELETE FROM"))
                        {
                            rowsCB = (int)queryresult.Metrics.MutationCount;
                        }
                        else
                        {
                            rowsCB = queryresult.Rows.Count;
                        }
                        log_Query(logpath_couchbase, "Egyéni: " + egyediCBN1QL, rowsCB, run_Time_CB.ElapsedMilliseconds);
                        //label_CouchBase.Text = "Az egyéni query(k) lefutott(ak).";
                    }
                }
            }
            catch (CouchbaseResponseException ex)
            {
                MessageBox.Show(ex.Message);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        //Egyedi N1QL futtatása
        private void button_RunN1QL_Click(object sender, EventArgs e)
        {
            egyediCBN1QL = rTB_N1QL.Text;
            bgwork_CB = new BackgroundWorker();
            bgwork_CB.WorkerSupportsCancellation = false;
            bgwork_CB.DoWork += bgwork_EgyediCouchB_DoWork;
            bgwork_CB.RunWorkerCompleted += bgwork_CB_RunWorkerCompleted;
            bgwork_CB.RunWorkerAsync();
        }

        void bgwork_UpdateCouchB_DoWork(object sender, DoWorkEventArgs e)
        {
            run_Time_CB.Reset();
            rowsCB = 0;
            try
            {
                pictureBox_CB.Image = new Bitmap("images/loading.gif");
            }
            catch { }
            try
            {
                var cb = new Cluster(new ClientConfiguration
                {
                    Servers = new List<Uri> { new Uri("https://127.0.0.1:8091/") }
                });
                cb.Authenticate("admin", "Patrik");
                var bucket = cb.OpenBucket("szakd");
                //Adatok lekérése
                string qq = "UPDATE szakd SET email=\"updatelt@gmail.com\" WHERE 1;";


                run_Time_CB.Start();
                var queryresult = bucket.Query<dynamic>(qq); //Query futtatása
                run_Time_CB.Stop();
                rowsCB = (int)queryresult.Metrics.MutationCount;

                if (queryresult.Success)
                {
                    log_Query(logpath_couchbase, "UPDATE", rowsCB, run_Time_CB.ElapsedMilliseconds);
                    //label_CouchBase.Text = "A query futása " + run_Time_CB.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rowsCB;
                }

            }

            catch (CouchbaseResponseException ex)
            {
                MessageBox.Show(ex.Message);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void button_UpdateCouchB_Click(object sender, EventArgs e)
        {
            bgwork_CB = new BackgroundWorker();
            bgwork_CB.WorkerSupportsCancellation = false;
            bgwork_CB.DoWork += bgwork_UpdateCouchB_DoWork;
            bgwork_CB.RunWorkerCompleted += bgwork_CB_RunWorkerCompleted;
            bgwork_CB.RunWorkerAsync();
        }
        #endregion

        private void VarosokInsert()
        {
            run_Time_CB.Reset();
             rowsCB = 0;

            try
            {
                pictureBox_CB.Image = new Bitmap("images/loading.gif");
            }
            catch { }

            //Adatok beolvasása
            List<string> list_cit_id = new List<string>();
            List<string> list_cit_zip = new List<string>();
            List<string> list_cit_name= new List<string>();
            List<string> list_cit_cty_code = new List<string>();
            List<string> list_cit_lat = new List<string>();
            List<string> list_cit_long = new List<string>();
            List<string> list_cit_range = new List<string>();
            List<string> list_cit_population = new List<string>();
            List<string> list_cit_homes = new List<string>();
            try
            {
                using (var reader = new StreamReader(@"datas\varosok2.csv"))
                {

                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');

                        list_cit_id.Add(values[0]);
                        list_cit_zip.Add(values[1]);
                        list_cit_name.Add(values[2]);
                        list_cit_cty_code.Add(values[3]);
                        list_cit_lat.Add(values[4]);
                        list_cit_long.Add(values[5]);
                        list_cit_range.Add(values[6]);
                        list_cit_population.Add(values[7]);
                        list_cit_homes.Add(values[8]);
                    }
                }
            }
            catch { }
            //Kapcsolódás
            try
            {
                var cb = new Cluster(new ClientConfiguration
                {
                    Servers = new List<Uri> { new Uri("https://127.0.0.1:8091/") }
                });
                cb.Authenticate("admin", "Patrik");
                var bucket = cb.OpenBucket("Varosok");

                for (int j = 0; j < nUD_EgyediN1QL.Value; j++)
                {
                    run_Time_CB.Reset();
                    rowsCB = 0;

                    //--Dictionary--
                    var dic = new Dictionary<string, dynamic>();

                    for (int i = 0; i < list_cit_zip.Count; i++)
                    {
                        //ID növelés
                        var incr = bucket.Increment("Incrementer", 1);
                        if (incr.Success)
                        {
                            var Content = new
                            {
                                cit_id = list_cit_id[i],
                                cit_zip= list_cit_zip[i],
                                cit_name= list_cit_name[i],
                                cit_cty_code = list_cit_cty_code[i],
                                cit_lat = list_cit_lat[i],
                                cit_long = list_cit_long[i],
                                cit_range = list_cit_range[i],
                                cit_population = list_cit_population[i],
                                cit_homes = list_cit_homes[i]
                            };
                            dic.Add(incr.Value.ToString(), Content);
                        }
                    }
                    //Mérés kezdete
                    run_Time_CB.Start();
                    var result = bucket.Upsert(dic);

                    run_Time_CB.Stop(); //..Vége
                    rowsCB = result.Count;
                }
                label_CouchBase.Text = "A Varosok feltöltése " + run_Time_CB.ElapsedMilliseconds + "ms ideig tartott. Érintett sorok: " + rowsCB;
            }

            catch (CouchbaseResponseException ex)
            {
                MessageBox.Show(ex.Message);
            }

            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                pictureBox_CB.Image = null;
            }
        }

        #region logs
        private void button_MySLog_Click(object sender, EventArgs e)
        {

            dGV_MyS.Rows.Clear();
            try
            {
                StreamReader srgen = new StreamReader(logpath_mysql);
                while (!srgen.EndOfStream)
                {
                    var line = srgen.ReadLine();
                    var datas = line.Split(';');
                    // DataGridViewRow row =
                    dGV_MyS.Rows.Add(datas[0], datas[1], datas[2], datas[3]);

                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           
        }

        private void button_PGLog_Click(object sender, EventArgs e)
        {

            dGV_PG.Rows.Clear();
            try
            {
                StreamReader srgen = new StreamReader(logpath_postgresql);
                while (!srgen.EndOfStream)
                {
                    var line = srgen.ReadLine();
                    var datas = line.Split(';');
                    // DataGridViewRow row =
                    dGV_PG.Rows.Add(datas[0], datas[1], datas[2], datas[3]);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button_CBLog_Click(object sender, EventArgs e)
        {

            dGV_CB.Rows.Clear();
            try
            {
                StreamReader srgen = new StreamReader(logpath_couchbase);
                while (!srgen.EndOfStream)
                {
                    var line = srgen.ReadLine();
                    var datas = line.Split(';');
                    // DataGridViewRow row =
                    dGV_CB.Rows.Add(datas[0], datas[1], datas[2], datas[3]);

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            VarosokInsert();
        }


    }
}
