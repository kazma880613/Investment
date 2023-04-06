using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Investment
{
    public class DBConfig
    {
        public string dbIP = "localhost";
        public int dbPort = 3306;
        public string dbUsername = "root";
        public string dbPassword = "kazma1999";
        public string dbName = "investment";
    }
    public class Stock
    {
        public string StockName;
        public string StockNo;
        public string DecadesInfo;
    }

    public class Decade
    {
        public string Time;
        public string EPS;
        public string Annual_growth;
        public string Profit;
        public string Shares;
        public string Payout;
        public string Dividend;
        public string Dividend_yield;
        public string Cash;
        public string US_Annual_growth;
    }

    public class StockInformation_List
    {
        public List<Stock> StockInfo = new List<Stock>();
    }

    public class DataInformation_List
    {
        public List<Decade> DataInfo = new List<Decade>();
    }
    
}
