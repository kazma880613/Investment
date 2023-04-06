using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace Investment
{
    public class dbService
    {
        DBConfig _dbconfig = new DBConfig();
        string DBConnString = "";

        public dbService()
        {
            MySqlConnectionStringBuilder sqlConnString = new MySqlConnectionStringBuilder();
            sqlConnString.UserID = _dbconfig.dbUsername;
            sqlConnString.Password = _dbconfig.dbPassword;
            sqlConnString.Database = _dbconfig.dbName;
            sqlConnString.Server = _dbconfig.dbIP;
            sqlConnString.Port = (uint)_dbconfig.dbPort;

            sqlConnString.Pooling = true;
            sqlConnString.MinimumPoolSize = 3;
            sqlConnString.MaximumPoolSize = 10;

            DBConnString = sqlConnString.ConnectionString;
        }

        public List<Stock> Load()
        {
            StockInformation_List _StockList = new StockInformation_List();
            MySqlConnection UserSqlConnection = new MySqlConnection();
            UserSqlConnection.ConnectionString = DBConnString;
            UserSqlConnection.Open();
            string cmdString;

            try
            {
                cmdString = "SELECT Name FROM investment.stockinfo;";
                MySqlCommand DBCommand = new MySqlCommand(cmdString, UserSqlConnection);
                MySqlDataReader DBReader = DBCommand.ExecuteReader();
                while (DBReader.Read())
                {
                    Stock _tmp = new Stock();
                    _tmp.StockName = DBReader["Name"].ToString();

                    _StockList.StockInfo.Add(_tmp);
                }
            }
            catch
            {

            }
            UserSqlConnection.Close();

            return _StockList.StockInfo;
        }

        public Stock Search(string StockName)
        {
            Stock response = new Stock();

            MySqlConnection UserSqlConnection = new MySqlConnection();
            UserSqlConnection.ConnectionString = DBConnString;
            UserSqlConnection.Open();
            string cmdString;

            try
            {
                cmdString = "SELECT * FROM investment.stockinfo WHERE Name = @StockName;";
                MySqlCommand DBCommand = new MySqlCommand(cmdString, UserSqlConnection);
                DBCommand.Parameters.Clear();
                DBCommand.Parameters.AddWithValue("@StockName", StockName);

                MySqlDataReader DBReader = DBCommand.ExecuteReader();
                while (DBReader.Read())
                {
                    Stock _tmp = new Stock();
                    response.StockName = DBReader["Name"].ToString();
                    response.StockNo = DBReader["No"].ToString();
                    response.DecadesInfo = DBReader["Data"].ToString();
                }
            }
            catch
            {

            }
            UserSqlConnection.Close();

            return response;
        }

        public bool Save(string StockName, string StockNo, string StockData)
        {
            bool _ifsuccess = true;
            MySqlConnection UserSqlConnection = new MySqlConnection();

            UserSqlConnection.ConnectionString = DBConnString;
            UserSqlConnection.Open();
            string cmdString;

            try
            {
                cmdString = "DELETE FROM investment.stockinfo WHERE Name = @StockName;";
                MySqlCommand DBCommand = new MySqlCommand(cmdString, UserSqlConnection);

                DBCommand.Parameters.Clear();
                DBCommand.Parameters.AddWithValue("@StockName", StockName);
                DBCommand.ExecuteNonQuery();
                _ifsuccess = true;
            }
            catch
            {
                _ifsuccess = false;
            }

            if (_ifsuccess == true )
            {
                try
                {
                    cmdString = "INSERT INTO investment.stockinfo(Name,No,Data) VALUES(@StockName,@StockNo,@StockData);";
                    MySqlCommand DBCommand = new MySqlCommand(cmdString, UserSqlConnection);

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
            UserSqlConnection.Close();

            return _ifsuccess;
        }

        public bool Delete(string StockName)
        {
            bool _ifsuccess = false;
            MySqlConnection UserSqlConnection = new MySqlConnection();

            UserSqlConnection.ConnectionString = DBConnString;
            UserSqlConnection.Open();
            string cmdString;

            cmdString = "SELECT * FROM investment.stockinfo WHERE Name = @StockName;";
            MySqlCommand DBCommand = new MySqlCommand(cmdString, UserSqlConnection);
            DBCommand.Parameters.Clear();
            DBCommand.Parameters.AddWithValue("@StockName", StockName);

            MySqlDataReader DBReader = DBCommand.ExecuteReader();
            if (DBReader.Read())
            {
                _ifsuccess= true;
            }
            DBReader.Close();

            if(_ifsuccess == true)
            {
                try
                {
                    cmdString = "DELETE FROM investment.stockinfo WHERE Name = @StockName;";
                    DBCommand = new MySqlCommand(cmdString, UserSqlConnection);

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
            UserSqlConnection.Close();

            return _ifsuccess;
        }
    }
}
