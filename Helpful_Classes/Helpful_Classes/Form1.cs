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
using System.Threading;
using System.IO.Compression;
using System.Net;
using Microsoft.Office.Interop.Excel;
using SystemDataTable = System.Data.DataTable;
using System.Data.SqlClient;

namespace Helpful_Classes
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        StringCryptography stringCryptography = new StringCryptography();
        SendingEmail sendingEmail = new SendingEmail();
        CreateLogs createLogs = new CreateLogs();
        FileManagerClass fileBinReader = new FileManagerClass();

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                List<string> to = new List<string>();
                List<string> cc = new List<string>();
                List<string> emailMess = new List<string>();
                
                string sendEmail = sendingEmail.SendEmailFunction(
                    "no-reply.sales-report@lawson-philippines.com"
                    , "no-reply.sales-report@lawson-philippines.com"
                    , stringCryptography.decryptor("9n3c6z3umrDdrWhbCkEySg==")
                    , "172.16.195.10"
                    , 25
                    , to
                    , cc
                    , @"C:\Users\jntaller\Desktop\SampleDirectory"
                    , "TEST MAIL SUBJECT"
                    , emailMess
                    );

                createLogs.logs("Message Sent");

                MessageBox.Show(sendEmail);
            }
            catch (Exception ee)
            {
                createLogs.logs(ee.ToString());
                MessageBox.Show(ee.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            StringCryptography sTR = new StringCryptography();

            createLogs.logs(textBox1.Text + "Encrypted to " + textBox2.Text);
            textBox2.Text = sTR.enryptor(textBox1.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            StringCryptography sTR = new StringCryptography();

            createLogs.logs(textBox3.Text + "Decrypted to " + textBox4.Text);
            textBox4.Text = sTR.decryptor(textBox3.Text);
        }

        struct FtpSetting
        {
            public string Server { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
            public string FileName { get; set; }
            public string FullName { get; set; }
        }

        FtpSetting _inputParameter;

        private void Form1_Load(object sender, EventArgs e)
        {
            //txtServer.Text = @"ftp://172.16.192.2//LAWSON/";
            //txtUser.Text = @"Lawson\jntaller";
            //txtPWD.Text = "Philippians413";

            txtServer.Text = @"ftp://138.108.148.101//234/";
            txtUser.Text = "234";
            txtPWD.Text = "N@234Law";
        }

        string fileName;
        string fullName;
        string userName;
        string password;
        string server;

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(new Uri(string.Format("{0}/{1}", server, fileName)));
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(userName, password);


                Stream ftpStream = request.GetRequestStream();
                FileStream fs = File.OpenRead(fullName);
                byte[] buffer = new byte[1024];
                double total = (double)fs.Length;
                int byteRead = 0;
                double read = 0;
                do
                {
                    if (!backgroundWorker.CancellationPending)
                    {
                        byteRead = fs.Read(buffer, 0, 1024);
                        ftpStream.Write(buffer, 0, byteRead);
                        read += (double)byteRead;
                        double percentage = read / total * 100;
                        backgroundWorker.ReportProgress((int)percentage);
                    }
                } while (byteRead != 0);
                fs.Close();
                ftpStream.Close();
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            label1.Text = $"Upload Complete {e.ProgressPercentage}%";
            progressBar1.Value = e.ProgressPercentage;
            progressBar1.Update();
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            label1.Text = "Upload Complete";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = true, ValidateNames = true, Filter = "All files|*.*" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        FileInfo fi = new FileInfo(ofd.FileName);
                        fileName = fi.Name;
                        fullName = fi.FullName;
                        userName = txtUser.Text;
                        password = txtPWD.Text;
                        server = txtServer.Text;

                        backgroundWorker.RunWorkerAsync();
                    }
                }

                //FtpFileTransfer ftpFile = new FtpFileTransfer();

                //using (OpenFileDialog ofd = new OpenFileDialog() { Multiselect = true, ValidateNames = true, Filter = "All files|*.*" })
                //{
                //    if (ofd.ShowDialog() == DialogResult.OK)
                //    {
                //        FileInfo fi = new FileInfo(ofd.FileName);
                //        fileName = fi.Name;
                //        fullName = fi.FullName;
                //        userName = txtUser.Text;
                //        password = txtPWD.Text;
                //        server = txtServer.Text;

                //        ftpFile.FTP(server, fileName, userName, password, fullName);

                //    }
                //}
            }
            catch (Exception ee)
            {
                MessageBox.Show(ee.ToString());
            }

            

            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            MSSqlExecuter mssql = new MSSqlExecuter();
            System.Data.DataTable dataTable = new System.Data.DataTable();

            mssql.connect("172.16.192.64","CDRDB","sa","pw@1234");
            dataTable = mssql.queryToDataTable("select * from cdr_t_request", dataTable);
            dataGridView1.DataSource = dataTable;
            mssql.close();


            Microsoft.Office.Interop.Excel.Application excel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Microsoft.Office.Interop.Excel.Worksheet sheet1 = (Microsoft.Office.Interop.Excel.Worksheet)workbook.Sheets[1];
            int StartCol = 1;
            int StartRow = 1;

            //Write Headers
            for (int j = 0; j < dataTable.Columns.Count; j++)
            {
                Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[StartRow, StartCol + j];
                myRange.Value2 = dataTable.Columns[j].ToString();
            }
            StartRow++;

            //Write datagridview content
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    try
                    {
                        //count_cell2++;
                        //percent = (count_cell2 / count_cell) * 100;
                        //percentage = Convert.ToInt32(Math.Round(percent));
                        //backgroundWorker1.ReportProgress(percentage);

                        Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[StartRow + i, StartCol + j];
                        myRange.Value2 = dataTable.Rows[i][j].ToString() == null ? "" : "'" + dataTable.Rows[i][j].ToString();
                    }
                    catch (Exception ee)
                    {
                        MessageBox.Show(ee.Message);
                    }
                }
            }

            excel.Visible = true;

            excel.WindowState = Microsoft.Office.Interop.Excel.XlWindowState.xlMaximized;

        }

        private void button6_Click(object sender, EventArgs e)
        {
            string finalValue = fileBinReader.BinaryFileReader(textBox5.Text, textBox6.Text);

            MessageBox.Show(finalValue.ToString());
        }

        MSSqlExecuter sqlCon = new MSSqlExecuter();
        private void button7_Click(object sender, EventArgs e)
        {
            string que = String.Format(File.ReadAllText(Environment.CurrentDirectory + @"\Queries\OTWReport.sql"), "2019-08-21", "2019-08-27") + " Fetch Next 5 rows only";
            MessageBox.Show(que);

            SystemDataTable dataTable = new SystemDataTable();
            sqlCon.connect(
                "172.16.192.64"
                , "merch_inhouse_db"
                , "sa"
                , "pw@1234"
                );
            dataTable = sqlCon.queryToDataTable(que, dataTable);
            sqlCon.close();

            dataGridView1.DataSource = dataTable;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            int i = 0;
            try
            {
                i = int.Parse(textBox7.Text) + 2;
                MessageBox.Show(i.ToString());
            }
            catch (Exception ex)
            {
                try
                {
                    MessageBox.Show(
                                Environment.NewLine + "Data: " + ex.Data + Environment.NewLine +
                                Environment.NewLine + "HelpLink: " + ex.HelpLink + Environment.NewLine +
                                Environment.NewLine + "HResult: " + ex.HResult.ToString(textBox7.Text) + Environment.NewLine +
                                Environment.NewLine + "InnerException: " + ex.InnerException + Environment.NewLine +
                                Environment.NewLine + "Message: " + ex.Message + Environment.NewLine +
                                Environment.NewLine + "Source: " + ex.Source + Environment.NewLine +
                                Environment.NewLine + "StrackTrace: " + ex.StackTrace + Environment.NewLine +
                                Environment.NewLine + "TargetSite: " + ex.TargetSite + Environment.NewLine
                                );
                }
                catch (Exception exx)
                {
                    MessageBox.Show(exx.Message);
                }
            }
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            //MessageBox.Show(dataGridView1.CurrentCell.Value.ToString());

            dateTimePicker1.Value = Convert.ToDateTime(dataGridView1.CurrentCell.Value.ToString());
        }

        List<tempFiles> lList = new List<tempFiles>();
        private void button10_Click(object sender, EventArgs e)
        {
            tempFiles t = new tempFiles();
            t.TableName = "A";
            t.Size = "175 MB";
            t.External = "200 MB";
            lList.Add(t);

            t = new tempFiles();
            t.TableName = "B";
            t.Size = "176 MB";
            t.External = "230 MB";
            lList.Add(t);

            t = new tempFiles();
            t.TableName = "C";
            t.Size = "180 MB";
            t.External = "500 MB";
            lList.Add(t);

            dataGridView2.DataSource = lList;

        }

        class tempFiles
        {
            public string TableName { get; set; }
            public string Size { get; set; }
            public string External { get; set; }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dRow in dataGridView2.Rows)
            {
                int count = 0;
                for (int i = 1; i < dataGridView2.ColumnCount; i++)
                {
                    count++;
                    if(count == 1)
                    {
                        MessageBox.Show(String.Format("INSERT INTO TABLE (TableName, SIZE) VALUES({0},{1})", dRow.Cells[0].Value.ToString(), dRow.Cells[i].Value.ToString()));
                    }
                    else if(count == 2)
                    {
                        MessageBox.Show(String.Format("INSERT INTO TABLE (TableName, EXTERNAL) VALUES({0},{1})", dRow.Cells[0].Value.ToString(), dRow.Cells[i].Value.ToString()));
                    }
                } 
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            CrystalReport crystalReport = new CrystalReport();
            crystalReport.ShowDialog();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            OpenFileDialog oFD = new OpenFileDialog() { Filter = "Office Files|*.csv" };
            List<ReadCSV> readCSVList = new List<ReadCSV>();

            if (oFD.ShowDialog() == DialogResult.OK)
            {
                string[] CSVFILES = File.ReadAllLines(oFD.FileName);

                for (int i = 0; i < CSVFILES.Length; i++)
                {
                    ReadCSV readCSV = new ReadCSV();
                    readCSV.ID = int.Parse(CSVFILES[i].Split(',')[0]);
                    readCSV.Name = CSVFILES[i].Split(',')[1];
                    readCSVList.Add(readCSV);
                }
            }

            dataGridView1.DataSource = readCSVList;
        }
    }

    class ReadCSV
    {
        public int ID { get; set; }
        public string Name { get; set; }
    }
}
