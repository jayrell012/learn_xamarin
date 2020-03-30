using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using System.Data;
using SystemData = Microsoft.Office.Interop.Excel;
using System.Net;
using System.Net.Mail;
using System.ComponentModel;

namespace CDR_IT_SIDE_MONITORING
{
    class DBConnection
    {
        SqlConnection con;

        string[] ConfigDB = File.ReadAllLines(Environment.CurrentDirectory + @"\DBCONFIG.dbconfig");
        public void connect()
        {
            con = new SqlConnection("SERVER="+ ConfigDB[0] + ";" +
                                    "DATABASE=" + ConfigDB[1] + ";" +
                                    "UID=" + ConfigDB[2] + ";" +
                                    "PWD="+ ConfigDB[3] + "" +
                                    "");
            con.Open();
        }

        public void close()
        {
            con.Close();
        }

        public string ExecQ(string q)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(q, con);
                cmd.ExecuteNonQuery();
                return "success";
            }
            catch (Exception ee)
            {
                return ee.ToString();
            }
        }

        public void Reader(string q)
        {
            SqlCommand cmd = new SqlCommand(q, con);
            SqlDataReader rd;
            rd = cmd.ExecuteReader();

        }

        public object showDGV(string q)
        {
            SqlDataAdapter sda = new SqlDataAdapter(q, con);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            object data = ds.Tables[0];
            return data;
        }

        public DataTable queryToDataTable(string q, DataTable ds)
        {
            SqlDataAdapter sda = new SqlDataAdapter(q, con);
            ds = new DataTable();
            sda.Fill(ds);
            return ds;
        }
    }

    class ExcelExport
    {
        
        public int Export(DataTable dataTable, BackgroundWorker bw)
        {
            decimal totalRow = dataTable.Rows.Count;
            int countRow = 0;
            int percentage = 0;

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
                countRow++;
                percentage = Convert.ToInt32(Math.Round((countRow / totalRow) * 100));
                bw.ReportProgress(percentage);

                for (int j = 0; j < dataTable.Columns.Count; j++)
                {
                    try
                    {
                        Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[StartRow + i, StartCol + j];
                        myRange.Value2 = dataTable.Rows[i][j].ToString() == null ? "" : "'" + dataTable.Rows[i][j].ToString();
                    }
                    catch (Exception ee)
                    {
                        return 0;
                    }
                }
            }

            excel.Visible = true;

            excel.WindowState = Microsoft.Office.Interop.Excel.XlWindowState.xlMaximized;
            excel.DefaultFilePath = Environment.CurrentDirectory + @"\Result";
            workbook.SaveAs(@"CDR_IT_SIDE_" + DateTime.Now.ToString("ddMMMyyyy").ToUpper());

            return percentage;
        }
    }

    class SendingEmail
    {
        //With Attachment
        public string SendEmailFunction(string emailAcc, string userID, string decryptedPassword, string smtpIP, int smtpPort, string[] to, string[] cc, string attachmentDirectoryInfo, string emailSubject, string[] emailMessage)
        {
            try
            {
                MailMessage msg = new MailMessage();
                SmtpClient smtp = new SmtpClient(smtpIP, smtpPort);

                smtp.Credentials = new NetworkCredential(userID, decryptedPassword);
                msg.From = new MailAddress(emailAcc);

                for (int i = 0; i < to.Count(); i++)
                {
                    msg.To.Add(to[i]);
                }
                for (int i = 0; i < cc.Count(); i++)
                {
                    msg.CC.Add(cc[i]);
                }

                DirectoryInfo info = new DirectoryInfo(attachmentDirectoryInfo + @"\");
                FileInfo[] fileInfo = info.GetFiles();
                foreach (FileInfo file in fileInfo)
                {
                    msg.Attachments.Add(new Attachment(attachmentDirectoryInfo + @"\" + file));
                }

                msg.Subject = emailSubject;

                StringBuilder strbuil = new StringBuilder();
                for (int i = 0; i < emailMessage.Count(); i++)
                {
                    strbuil.AppendLine(emailMessage[i] + "<br>");
                }

                msg.Body =

                    strbuil.ToString() +
                    "<br>" +
                    "<br>" +
                    "Sent Time Stamp: " + DateTime.Now.ToString("MMMM,dd yyyy HH:mm:ss") +
                    "<br>" +
                    "<i><small>----- This mail was sent via Inhouse Email. Please Do Not Reply -----</small></i>";
                msg.IsBodyHtml = true;

                smtp.EnableSsl = false;
                smtp.Send(msg);

                msg.Dispose();
                smtp.Dispose();

                return "Message Sent Succesfully";

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }


        //without attachment
        public string SendEmailFunction(string emailAcc, string userID, string decryptedPassword, string smtpIP, int smtpPort, string[] to, string[] cc, string emailSubject, string[] emailMessage)
        {
            try
            {
                MailMessage msg = new MailMessage();
                SmtpClient smtp = new SmtpClient(smtpIP, smtpPort);

                smtp.Credentials = new NetworkCredential(userID, decryptedPassword);
                msg.From = new MailAddress(emailAcc);

                for (int i = 0; i < to.Count(); i++)
                {
                    msg.To.Add(to[i]);
                }
                for (int i = 0; i < cc.Count(); i++)
                {
                    msg.CC.Add(cc[i]);
                }

                msg.Subject = emailSubject;

                StringBuilder strbuil = new StringBuilder();
                for (int i = 0; i < emailMessage.Count(); i++)
                {
                    strbuil.AppendLine(emailMessage[i] + "<br>");
                }

                msg.Body =

                    strbuil.ToString() +
                    "<br>" +
                    "<br>" +
                    "Sent Time Stamp: " + DateTime.Now.ToString("MMMM,dd yyyy HH:mm:ss") +
                    "<br>" +
                    "<i><small>----- This mail was sent via Inhouse Email. Please Do Not Reply -----</small></i>";
                msg.IsBodyHtml = true;

                smtp.EnableSsl = false;
                smtp.Send(msg);

                msg.Dispose();
                smtp.Dispose();

                return "Message Sent Succesfully";

            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}
