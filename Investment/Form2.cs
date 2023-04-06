using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Investment
{
    public partial class Form2 : Form
    {
        public string value = "0";
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 form1 = (Form1)this.Owner;
            form1.Payout = value;
            this.Close();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.Columns["column1"].ReadOnly = true;
            dataGridView1.Columns["column4"].ReadOnly = true;
            DateTime thisyear = DateTime.Now;
            int year = thisyear.Year;
            for (int i = 0; i < 5; i++)
            {
                dataGridView1.Rows.Add($"{(year - 1) - i}");
            }
            dataGridView1.Rows.Add("Total");
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                DataGridViewCell CellEPS = dataGridView1.Rows[e.RowIndex].Cells["Column2"];
                DataGridViewCell CellDividend = dataGridView1.Rows[e.RowIndex].Cells["Column3"];
                DataGridViewCell TargetCell = dataGridView1.Rows[e.RowIndex].Cells["Column4"];
                DataGridViewCell TotalEPSCell = dataGridView1.Rows[5].Cells[1];
                DataGridViewCell TotalDividendCell = dataGridView1.Rows[5].Cells[2];
                DataGridViewCell TotalAverageCell = dataGridView1.Rows[5].Cells[3];
                double TotalEPS = 0;
                double TotalDividend = 0;

                if (CellEPS.Value != null && CellDividend.Value != null)
                {
                    double result = Convert.ToDouble(CellDividend.Value) / Convert.ToDouble(CellEPS.Value);

                    TargetCell.Value = result.ToString();
                }

                if (Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells["Column2"].Value) > 0)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        TotalEPS += Convert.ToDouble(dataGridView1.Rows[j].Cells["Column2"].Value);
                        TotalEPS = Math.Round(TotalEPS, 2);
                    }
                    TotalEPSCell.Value = TotalEPS.ToString();
                }

                if (Convert.ToDouble(dataGridView1.Rows[e.RowIndex].Cells["Column3"].Value) > 0)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        TotalDividend += Convert.ToDouble(dataGridView1.Rows[j].Cells["Column3"].Value);
                    }
                    TotalDividend = Math.Round(TotalDividend, 2);
                    TotalDividendCell.Value = TotalDividend.ToString();
                }

                if(TotalAverageCell.Value != null)
                {
                    value = TotalAverageCell.Value.ToString();
                    value = Math.Round(Convert.ToDouble(value), 2).ToString();
                }
            }
        }

    }
}
