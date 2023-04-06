using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Security.Cryptography.X509Certificates;

namespace Investment
{
    public class SQLiteService
    {
        public string DBName = Directory.GetCurrentDirectory() + "\\SQL.db";
        public string connectionString = "data source = " + Directory.GetCurrentDirectory() + "\\SQL.db";
        public SQLiteService()
        {
            if(File.Exists(DBName) != true)
            {
                SQLiteConnection.CreateFile(DBName);
            }

            SQLiteConnection dbConnection = new SQLiteConnection(connectionString);
            dbConnection.Open();

            string[] colNames = new string[] { "Name", "No", "Data" };
            string[] colTypes = new string[] { "TEXT", "TEXT", "TEXT" };

            string tableName = "stockinfo";

            string queryString = "CREATE TABLE IF NOT EXISTS " + tableName + "( " + colNames[0] + " " + colTypes[0];

            for (int i = 1; i < colNames.Length; i++)
            {
                queryString += ", " + colNames[i] + " " + colTypes[i];
            }
            queryString += "  ) ";
            SQLiteCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = queryString;
            SQLiteDataReader dataReader = dbCommand.ExecuteReader();
            dataReader.Read();
            dataReader.Close();

            dbConnection.Close();
        }

        public List<Stock> Load()
        {
            StockInformation_List _StockList = new StockInformation_List();
            SQLiteConnection dbConnection = new SQLiteConnection(connectionString);
            dbConnection.Open();
            try
            {
                string cmdString = "SELECT Name FROM stockinfo;";
                SQLiteCommand DBCommand = new SQLiteCommand(cmdString, dbConnection);
                SQLiteDataReader DBReader = DBCommand.ExecuteReader();
                while (DBReader.Read())
                {
                    Stock _tmp = new Stock();
                    _tmp.StockName = DBReader["Name"].ToString();

                    _StockList.StockInfo.Add(_tmp);
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message);
            }
            dbConnection.Close();

            return _StockList.StockInfo;
        }

        public Stock Search(string StockName)
        {
            Stock response = new Stock();
            SQLiteConnection dbConnection = new SQLiteConnection(connectionString);
            dbConnection.Open();
            try
            {
                string cmdString = "SELECT * FROM stockinfo WHERE Name = @StockName;";
                SQLiteCommand DBCommand = new SQLiteCommand(cmdString, dbConnection);
                DBCommand.Parameters.Clear();
                DBCommand.Parameters.AddWithValue("@StockName", StockName);

                SQLiteDataReader DBReader = DBCommand.ExecuteReader();
                while (DBReader.Read())
                {
                    Stock _tmp = new Stock();
                    response.StockName = DBReader["Name"].ToString();
                    response.StockNo = DBReader["No"].ToString();
                    response.DecadesInfo = DBReader["Data"].ToString();
                }
            }
            catch(Exception err )
            {
                MessageBox.Show(err.Message);
            }
            dbConnection.Close();

            return response;
        }

        public bool Save(string StockName, string StockNo, string StockData)
        {
            bool _ifsuccess = true;
            SQLiteConnection dbConnection = new SQLiteConnection();
            dbConnection.ConnectionString = connectionString;
            dbConnection.Open();
            try
            {
                string cmdString = "DELETE FROM stockinfo WHERE Name = @StockName;";
                SQLiteCommand DBCommand = new SQLiteCommand(cmdString, dbConnection);

                DBCommand.Parameters.Clear();
                DBCommand.Parameters.AddWithValue("@StockName", StockName);
                DBCommand.ExecuteNonQuery();
                _ifsuccess = true;
            }
            catch
            {
                _ifsuccess = false;
            }

            if (_ifsuccess == true)
            {
                try
                {
                    string cmdString = "INSERT INTO stockinfo(Name,No,Data) VALUES(@StockName,@StockNo,@StockData);";
                    SQLiteCommand DBCommand = new SQLiteCommand(cmdString, dbConnection);

                    DBCommand.Parameters.Clear();
                    DBCommand.Parameters.AddWithValue("@StockName", StockName);
                    DBCommand.Parameters.AddWithValue("@StockNo", StockNo);
                    DBCommand.Parameters.AddWithValue("@StockData", StockData);
                    DBCommand.ExecuteNonQuery();
                    _ifsuccess = true;
                }
                catch
                {
                    _ifsuccess = false;
                }
            }
            dbConnection.Close();

            return _ifsuccess;
        }

        public bool Delete(string StockName)
        {
            bool _ifsuccess = false;
            SQLiteConnection dbConnection = new SQLiteConnection();

            dbConnection.ConnectionString = connectionString;
            dbConnection.Open();
            string cmdString;

            cmdString = "SELECT * FROM stockinfo WHERE Name = @StockName;";
            SQLiteCommand DBCommand = new SQLiteCommand(cmdString, dbConnection);
            DBCommand.Parameters.Clear();
            DBCommand.Parameters.AddWithValue("@StockName", StockName);

            SQLiteDataReader DBReader = DBCommand.ExecuteReader();
            if (DBReader.Read())
            {
                _ifsuccess = true;
            }
            DBReader.Close();

            if (_ifsuccess == true)
            {
                try
                {
                    cmdString = "DELETE FROM stockinfo WHERE Name = @StockName;";
                    DBCommand = new SQLiteCommand(cmdString, dbConnection);

                    DBCommand.Parameters.Clear();
                    DBCommand.Parameters.AddWithValue("@StockName", StockName);
                    DBCommand.ExecuteNonQuery();
                    _ifsuccess = true;
                }
                catch
                {
                    _ifsuccess = false;
                }
            }
            dbConnection.Close();

            return _ifsuccess;
        }
    }
}
