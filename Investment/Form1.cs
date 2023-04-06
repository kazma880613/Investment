using Newtonsoft.Json;
using System.Data;

namespace Investment
{
    public partial class Form1 : Form
    {
        readonly SQLiteService _SQLService = new SQLiteService();
        readonly Stock _stockInfo = new Stock();
        private bool isDragging = false;
        private Point lastLocation;
        public string Payout = "0";
        readonly int year = DateTime.Now.Year;
        private float x;//定義當前窗體的寬度
        private float y;//定義當前窗體的高度

        private void setTag(Control cons)
        {
            foreach (Control con in cons.Controls)
            {
                con.Tag = con.Width + ";" + con.Height + ";" + con.Left + ";" + con.Top + ";" + con.Font.Size;
                if (con.Controls.Count > 0)
                {
                    setTag(con);
                }
            }
        }

        private void setControls(float newx, float newy, Control cons)
        {
            //遍歷窗體中的控件，重新設置控件的值
            foreach (Control con in cons.Controls)
            {
                //獲取控件的Tag屬性值，並分割後存儲字符串數組
                if (con.Tag != null)
                {
                    string[] mytag = con.Tag.ToString().Split(new char[] { ';' });
                    //根據窗體縮放的比例確定控件的值
                    con.Width = Convert.ToInt32(System.Convert.ToSingle(mytag[0]) * newx);//寬度
                    con.Height = Convert.ToInt32(System.Convert.ToSingle(mytag[1]) * newy);//高度
                    con.Left = Convert.ToInt32(System.Convert.ToSingle(mytag[2]) * newx);//左邊距
                    con.Top = Convert.ToInt32(System.Convert.ToSingle(mytag[3]) * newy);//頂邊距
                    Single currentSize = System.Convert.ToSingle(mytag[4]) * newy;//字體大小
                    con.Font = new Font(con.Font.Name, currentSize, con.Font.Style, con.Font.Unit);
                    if (con.Controls.Count > 0)
                    {
                        setControls(newx, newy, con);
                    }
                }
            }
        }

        public Form1()
        {
            InitializeComponent();
            x = this.Width;
            y = this.Height;
            setTag(this);

            string VersionInfo = "";
            VersionInfo = " V 1.00.00 Beta";
            // 程式雛形建立
            VersionInfo = " V 1.00.01 Beta";
            // 前端介面部分改用 Krypton.Toolkit

            this.Text = "InvestmentSystem " + VersionInfo;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            isDragging = true;
            lastLocation = e.Location;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                this.Width += e.Location.X - lastLocation.X;
                this.Height += e.Location.Y - lastLocation.Y;
                lastLocation = e.Location;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //dataGridView1.Width = this.ClientSize.Width - 50;
            //dataGridView1.Height = this.ClientSize.Height - 100;
            float newx = (this.Width) / x;
            float newy = (this.Height) / y;
            setControls(newx, newy, this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StockInformation_List _responseList = new StockInformation_List();
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Columns["column1"].ReadOnly = true;
            refresh();
            for (int i = 0; i < 10; i++)
            {
                dataGridView1.Rows.Add($"{year + i}", "0", "0", "0", "0", "0", "0", "0", "0", "0");
            }

            dataGridView1.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            _responseList.StockInfo = _SQLService.Load();
            comboBox1.Items.Add("請選擇股票名稱");
            comboBox1.Text = "請選擇股票名稱";
            for (int i = 0; i < _responseList.StockInfo.Count(); i++)
            {
                comboBox1.Items.Add(_responseList.StockInfo[i].StockName);
            }

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell cell1 = dataGridView1.Rows[e.RowIndex].Cells["Column2"];
                DataGridViewCell cell2 = dataGridView1.Rows[e.RowIndex].Cells["Column6"];
                DataGridViewCell targetCell = dataGridView1.Rows[e.RowIndex].Cells["Column7"];

                if (cell1.Value != null && cell2.Value != null)
                {
                    double result = Convert.ToDouble(cell1.Value) * Convert.ToDouble(cell2.Value);

                    targetCell.Value = result.ToString();
                }
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == dataGridView1.Columns["Column6"].Index)
            {
                Form2 form2 = new Form2();
                form2.Owner = this;
                form2.ShowDialog();
                dataGridView1.Rows[e.RowIndex].Cells["Column6"].Value = Payout;
            }
        }

        private void Save_button_Click(object sender, EventArgs e)
        {
            bool _ifwellformat = true;
            string responString = string.Empty;
            DataInformation_List _dataList = new DataInformation_List();
            if (textBox1.Text == "" || textBox1.Text == null)
            {
                Show_Message("名稱不可為空");
                _ifwellformat = false;
            }
            if (_ifwellformat == true)
            {
                if (textBox2.Text == "" || textBox2.Text == null)
                {
                    Show_Message("代號不可為空");
                    _ifwellformat = false;
                }
            }
            _stockInfo.StockName = textBox1.Text;
            _stockInfo.StockNo = textBox2.Text;

            if (_ifwellformat == true)
            {
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    Decade _tmpData = new Decade();

                    _tmpData.Time = dataGridView1.Rows[i].Cells["Column1"].Value.ToString();
                    _tmpData.EPS = dataGridView1.Rows[i].Cells["Column2"].Value.ToString();
                    _tmpData.Annual_growth = dataGridView1.Rows[i].Cells["Column3"].Value.ToString();
                    _tmpData.Profit = dataGridView1.Rows[i].Cells["Column4"].Value.ToString();
                    _tmpData.Shares = dataGridView1.Rows[i].Cells["Column5"].Value.ToString();
                    _tmpData.Payout = dataGridView1.Rows[i].Cells["Column6"].Value.ToString();
                    _tmpData.Dividend = dataGridView1.Rows[i].Cells["Column7"].Value.ToString();
                    _tmpData.Dividend_yield = dataGridView1.Rows[i].Cells["Column8"].Value.ToString();
                    _tmpData.Cash = dataGridView1.Rows[i].Cells["Column9"].Value.ToString();
                    _tmpData.US_Annual_growth = dataGridView1.Rows[i].Cells["Column10"].Value.ToString();

                    _dataList.DataInfo.Add(_tmpData);
                }
                responString = JsonConvert.SerializeObject(_dataList.DataInfo);

                bool _ifsuccess = _SQLService.Save(_stockInfo.StockName, _stockInfo.StockNo, responString);
                if (_ifsuccess == true)
                {
                    Show_Message("儲存成功");
                    comboBox1.Items.Add(_stockInfo.StockName);
                    refresh();
                }
                if (_ifsuccess == false)
                {
                    Show_Message("儲存失敗");
                }
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text != "請選擇股票名稱")
            {
                DataInformation_List _tmpList = new DataInformation_List();
                Stock _stock = new Stock();

                _stock = _SQLService.Search(comboBox1.Text);
                textBox1.Text = _stock.StockName;
                textBox1.ReadOnly = true;
                textBox2.Text = _stock.StockNo;
                textBox2.ReadOnly = true;
                string rs = _stock.DecadesInfo;
                _tmpList.DataInfo = JsonConvert.DeserializeObject<List<Decade>>(rs);

                if (_tmpList.DataInfo.Count() > 0)
                {
                    for (int i = 0; i < _tmpList.DataInfo.Count(); i++)
                    {
                        dataGridView1.Rows[i].Cells["Column1"].Value = _tmpList.DataInfo[i].Time;
                        dataGridView1.Rows[i].Cells["Column2"].Value = _tmpList.DataInfo[i].EPS;
                        dataGridView1.Rows[i].Cells["Column3"].Value = _tmpList.DataInfo[i].Annual_growth;
                        dataGridView1.Rows[i].Cells["Column4"].Value = _tmpList.DataInfo[i].Profit;
                        dataGridView1.Rows[i].Cells["Column5"].Value = _tmpList.DataInfo[i].Shares;
                        dataGridView1.Rows[i].Cells["Column6"].Value = _tmpList.DataInfo[i].Payout;
                        dataGridView1.Rows[i].Cells["Column7"].Value = _tmpList.DataInfo[i].Dividend;
                        dataGridView1.Rows[i].Cells["Column8"].Value = _tmpList.DataInfo[i].Dividend_yield;
                        dataGridView1.Rows[i].Cells["Column9"].Value = _tmpList.DataInfo[i].Cash;
                        dataGridView1.Rows[i].Cells["Column10"].Value = _tmpList.DataInfo[i].US_Annual_growth;
                    }
                }
            }
        }

        private void New_button_Click(object sender, EventArgs e)
        {
            refresh();
        }
        private void refresh()
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox1.ReadOnly = false;
            textBox2.ReadOnly = false;
            comboBox1.Text = "請選擇股票名稱";
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                dataGridView1.Rows[i].Cells["Column1"].Value = year + i;
                dataGridView1.Rows[i].Cells["Column2"].Value = "0";
                dataGridView1.Rows[i].Cells["Column3"].Value = "0";
                dataGridView1.Rows[i].Cells["Column4"].Value = "0";
                dataGridView1.Rows[i].Cells["Column5"].Value = "0";
                dataGridView1.Rows[i].Cells["Column6"].Value = "0";
                dataGridView1.Rows[i].Cells["Column7"].Value = "0";
                dataGridView1.Rows[i].Cells["Column8"].Value = "0";
                dataGridView1.Rows[i].Cells["Column9"].Value = "0";
                dataGridView1.Rows[i].Cells["Column10"].Value = "0";
            }
        }

        private void Delete_button_Click(object sender, EventArgs e)
        {
            bool _ifsuccess = false;
            string StockName = comboBox1.Text;

            _ifsuccess = _SQLService.Delete(StockName);

            if (_ifsuccess == true)
            {
                Show_Message("刪除成功");
                comboBox1.Items.Remove(StockName);
                refresh();
            }
            if (_ifsuccess == false)
            {
                Show_Message("刪除失敗");
            }
        }

        private void Show_Message(string Message)
        {
            MessageBox.Show(Message);
        }
    }
}