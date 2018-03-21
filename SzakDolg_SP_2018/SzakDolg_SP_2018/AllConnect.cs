using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Windows.Forms;
using Npgsql;


namespace AllConnect
{
    public class MySQLConnect
    {
        private MySqlConnection connection;
        private string server;
        private string port;
        private string database;
        private string uid;
        private string password;
       
        //Constructor
        public MySQLConnect()
        {
            Initialize();
           
        }

        //Initialize values
        private void Initialize()
        {
            server = "localhost";   //MySQL szerver elérési cime
            port = "3306";
            database = "szakdolgozat";           //Adatbázis neve
            uid = "sarlpa";               //Felhasználói név
            password = "Patrik";   //Jelszó
            string connectionString;         //Kapcsolódási parancs
            connectionString = "SERVER=" + server + ";" +"PORT=" +port+";"+ "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);  //Kapcsolat létrehozása
        }

        public MySqlConnection returnConnection()
        {
            server = "localhost:3306";   //MySQL szerver elérési cime
            port = "3306";
            database = "szakdolgozat";           //Adatbázis neve
            uid = "sarlpa";               //Felhasználói név
            password = "Patrik";   //Jelszó
            string connectionString;         //Kapcsolódási parancs
            connectionString = "SERVER=" + server + ";" + "PORT=" + port + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";

            return connection = new MySqlConnection(connectionString);  //Kapcsolat létrehozása
        }

        /// <summary>
        /// Kapcsolat megnyitása
        /// </summary>
        /// <returns>igen ha sikerült, nem ha nem sikerült</returns>
        public bool OpenConnection()
        {
            try
            {
                connection.Open(); //Megnyitja a kapcsolatot
                return true;
            }
            catch (MySqlException ex)
            {
                switch (ex.Number) //milyen számú hiba jön
                {
                    case 0:
                        MessageBox.Show("Nincs kapcsolat az adatbázissal. Kérjük vegye fel a kapcsolatot az adminisztrátorral!");
                        break; //0 akkor nincs kapcsolat
                }
                return false;
            }
        }

        /// <summary>
        /// Kapcsolat lezárása
        /// </summary>
        /// <returns>igen ha sikerült, nem ha nem sikerült</returns>
        public bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

    }

    public class PostGREConnect
    {
        private Npgsql.NpgsqlConnection connection;
        private string server;
        private string port;
        private string database;
        private string uid;
        private string password;

        //Constructor
        public PostGREConnect()
        {
            Initialize();
           
        }

        //Initialize values
        private void Initialize()
        {
            server = "localhost"; //PostGRESQL szerver elérési cime
            port = "5432";
            database = "postgres";           //Adatbázis neve
            uid = "postgres";               //Felhasználói név
            password = "admin";   //Jelszó
            string connectionString;         //Kapcsolódási parancs
            connectionString = "Host=" + server + ";"+"Port="+port+";" + "Username=" + uid + ";" + "Password=" + password + ";" + "Database=" + database;

            connection = new NpgsqlConnection(connectionString);  //Kapcsolat létrehozása
        }

        public bool OpenConnection()
        {
            try
            {
                connection.Open(); //Megnyitja a kapcsolatot
                return true;
            }
            catch (NpgsqlException ex)
            {
                        MessageBox.Show(ex.Message);

                return false;
            }
        }
    }
}
