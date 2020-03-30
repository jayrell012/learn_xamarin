using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CDR_IT_SIDE_MONITORING
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        
        DBConnection dbcon = new DBConnection();
        ExcelExport excelExport = new ExcelExport();
        SendingEmail sendingEmail = new SendingEmail();

        DataTable dtable = new DataTable();
        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                dtable = new DataTable();
                dbcon.connect();
                dtable = dbcon.queryToDataTable(File.ReadAllText(Environment.CurrentDirectory + @"\Query.sql"), dtable);
                dbcon.close();

                txtStatus.Text = "Success";
                txtRecord.Text = dtable.Rows.Count.ToString();

                ProgressStatus = "ExcelExport";
                backgroundWorker1.RunWorkerAsync();
            }
            catch (Exception ee)
            {
                txtStatus.Text = ee.ToString();
            }
        }

        string ProgressStatus = "";
        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            excelExport.Export(dtable, sender as BackgroundWorker);
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            bunifuCircleProgressbar1.Value = e.ProgressPercentage;

            if(ProgressStatus == "ExcelExport")
            {
                txtProgressStatus.Text = "Exporting Excel...";
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Cursor = Cursors.Default;
            txtStatus.Text = "--";
            txtRecord.Text = "--";
            txtProgressStatus.Text = "--";
            System.Diagnostics.Process.Start(Environment.CurrentDirectory + @"\Result");
            Application.Exit();
        }

        public int CountProgress(int count)
        {
            return count;
        }

        private void sendEmailAttachment()
        {
            try
            {
                string[] emailRecipients = File.ReadAllLines(Environment.CurrentDirectory + @"\EmailRecipients.ref");
                string[] emailMessage = File.ReadAllLines(Environment.CurrentDirectory + @"\EmailMessage.ref");
                string emailSubject = File.ReadAllText(Environment.CurrentDirectory + @"\EmailSubject.ref");
                string[] emailConfig = File.ReadAllLines(Environment.CurrentDirectory + @"\EmailConfig.ref");
                string[] emailAccount = File.ReadAllLines(Environment.CurrentDirectory + @"\EmailAccount.ref");

                //sendingEmail.SendEmailFunction(

                //    );
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }
    }
}
