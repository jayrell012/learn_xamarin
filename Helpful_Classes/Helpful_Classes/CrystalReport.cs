using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Helpful_Classes
{
    public partial class CrystalReport : Form
    {
        public CrystalReport()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            viewCRDataSource();
        }

        private void crystalReportViewer1_Load(object sender, EventArgs e)
        {

        }

        private void viewCRDataSource()
        {
            CrystalReport1 cr = new CrystalReport1();
            SqlConnection cons = new SqlConnection("Data Source=172.16.192.64;Initial Catalog=CDRDB;User ID=sa;Password=pw@1234");

            string sql = "select * from cdr_m_general";
            DataSet ds1 = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(sql, cons);
            
            da.Fill(ds1, "cdr_m_general");
            System.Data.DataTable dt = ds1.Tables["cdr_m_general"];

            cr.SetDatabaseLogon("sa","pw@1234","172.16.192.64","CDRDB");
            cr.SetDataSource(dt);
            
            crystalReportViewer1.ReportSource = cr;
            crystalReportViewer1.Refresh();
        }

        private void CrystalReport_Load(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Maximized;
            fillDGVTemporary();
        }

        private void fillDGVTemporary()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            for(int i = 1; i <= 4; i++)
            {
                dataGridView1.Columns.Add("Col_"+i, "Col_"+i);
            }

            for(int i = 1; i <= 3; i++)
            {
                dataGridView1.Rows.Add("","My Data " + i,"","");

                for (int r = 1; r <=3 ; r++)
                {
                    List<string> rows = new List<string>();
                    for (int j = 1; j <= 4; j++)
                    {
                        rows.Add("Data " + i + ";Col " + j + ";Row " + r);
                    }
                    dataGridView1.Rows.Add(rows.ToArray()); 
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            TableConvert tconvert = new TableConvert();
            DataTable dataTable = new DataTable();
            dataTable = tconvert.ConvertDGV_to_DataTable(dataGridView1);

            Helpful_Classes.CReportGenerator.CrystalReport.sampleCReport cReport = new CReportGenerator.CrystalReport.sampleCReport();
            cReport.Database.Tables["Items"].SetDataSource(dataTable); //"Items" is the name of your dataset table

            crystalReportViewer1.ReportSource = null;
            crystalReportViewer1.ReportSource = cReport;
        }
    }
}
