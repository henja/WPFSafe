using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Forms;

namespace WPFSafe
{

    class Database
    {

        public SQLiteConnection myConnection;

        public Database()
        {
            myConnection = new SQLiteConnection("Data Source=C:\\Users\\jason\\Source\\dbs\\database.sqlite3");


            /*below uses relative path
            string relativePath = @"Database\Database.sqlite";
            var parentdir = Path.GetDirectoryName(Application.StartupPath);
            string myString = parentdir.Remove(parentdir.Length - 34, 34);
            string absolutePath = Path.Combine(myString, relativePath);
            string connectionString = string.Format("Data Source={0};Version=3;Pooling=True;Max Pool Size=100;", absolutePath);
            myConnection = new SQLiteConnection(connectionString);
            myConnection.Open();
            */
        }

        public void OpenConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Open)
            {
                myConnection.Open();
            }
        }

        public void CloseConnection()
        {
            if (myConnection.State != System.Data.ConnectionState.Closed)
            {
                myConnection.Close();
            }
        }

    }
}
