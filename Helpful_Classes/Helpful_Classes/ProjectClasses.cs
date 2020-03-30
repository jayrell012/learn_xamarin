using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using IBM.Data.DB2;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Threading;
using System.IO.Compression;
using System.ComponentModel;
using System.Windows.Forms;

namespace Helpful_Classes
{
    class ProjectClasses
    {

    }

    class MSSqlExecuter
    {
        SqlConnection con;
        public void connect(string ipPort, string dbName, string uid, string pwd)
        {
            con = new SqlConnection("SERVER=" + ipPort + ";" +
                                    "DATABASE= " + dbName + ";" +
                                    "UID= " + uid + ";" +
                                    "PWD= " + pwd + " " +
                                    "");
            con.Open();
        }

        public void close()
        {
            con.Close();
        }

        public void ExecQ(string Query_, SqlConnection sqlConnection)
        {
            SqlTransaction sqlTransaction;
            sqlTransaction = sqlConnection.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand(Query_, sqlConnection, sqlTransaction);
                cmd.ExecuteNonQuery();
                cmd.CommandTimeout = 0;
                sqlTransaction.Commit();
            }
            catch (SqlException sqlError)
            {
                sqlTransaction.Rollback();
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

        public object showlistbox(string q)
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

    class DB2Executer
    {
        DB2Connection con;
        public void connect(string ipPort, string dbName, string uid, string pwd)
        {
            con = new DB2Connection("SERVER=" + ipPort + ";" +
                                    "DATABASE= " + dbName + ";" +
                                    "UID= " + uid + ";" +
                                    "PWD= " + pwd + " " +
                                    "");
            con.Open();
        }

        public void close()
        {
            con.Close();
        }

        public void ExecQ(string q)
        {
            DB2Command cmd = new DB2Command(q, con);
            cmd.ExecuteNonQuery();
        }

        public void Reader(string q)
        {
            DB2Command cmd = new DB2Command(q, con);
            DB2DataReader rd;
            rd = cmd.ExecuteReader();
        }

        public object showDGV(string q)
        {
            DB2DataAdapter sda = new DB2DataAdapter(q, con);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            object data = ds.Tables[0];
            return data;
        }

        public object showlistbox(string q)
        {
            DB2DataAdapter sda = new DB2DataAdapter(q, con);
            DataSet ds = new DataSet();
            sda.Fill(ds);
            object data = ds.Tables[0];
            return data;
        }

        public DataTable queryToDataTable(string q, DataTable ds)
        {
            DB2DataAdapter sda = new DB2DataAdapter(q, con);
            ds = new DataTable();
            sda.Fill(ds);
            return ds;
        }
    }

    class TableConvert
    {
        public DataTable ConvertDGV_to_DataTable(DataGridView dataGridView)
        {
            DataTable dt = new DataTable();

            foreach (DataGridViewColumn dgvc in dataGridView.Columns)
            {
                dt.Columns.Add(dgvc.HeaderText, typeof(string));
            }


            for (int row = 0; row < dataGridView.RowCount; row++)
            {
                List<string> rows = new List<string>();
                for (int col = 0; col < dataGridView.ColumnCount; col++)
                {
                    try
                    {
                        rows.Add(dataGridView.Rows[row].Cells[col].Value.ToString());
                    }
                    catch
                    {
                        rows.Add("");
                    }
                }
                dt.Rows.Add(rows.ToArray());
            }

            return dt;
        }
    }

    class SendingEmail
    {
        //string[] emailRecipients = File.ReadAllLines(Environment.CurrentDirectory + @"\EmailRecipients.ref");
        //string[] emailMessage = File.ReadAllLines(Environment.CurrentDirectory + @"\EmailMessage.ref");
        //string emailSubject = File.ReadAllText(Environment.CurrentDirectory + @"\EmailSubject.ref");
        //string[] emailConfig = File.ReadAllLines(Environment.CurrentDirectory + @"\EmailConfig.ref");
        //string[] emailAccount = File.ReadAllLines(Environment.CurrentDirectory + @"\EmailAccount.ref");
        //string txtFileLog = Environment.CurrentDirectory + @"\logs.txt";

        //With Attachment
        public string SendEmailFunction(string emailAcc, string userID, string decryptedPassword, string smtpIP, int smtpPort, List<string> to, List<string> cc, string attachmentDirectoryInfo, string emailSubject, List<string> emailMessage)
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
                for (int i = 0; i < emailMessage.Count; i++)
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
        public string SendEmailFunction(string emailAcc, string userID, string decryptedPassword, string smtpIP, int smtpPort, List<string> to, List<string> cc, string emailSubject, List<string> emailMessage)
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
                for (int i = 0; i < emailMessage.Count; i++)
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

    class StringCryptography
    {
        public string decryptor(string stringToDecrypt)
        {
            try
            {
                byte[] rijnKey = Encoding.ASCII.GetBytes("abcdefg_abcdefg_abcdefg_abcdefg_");
                byte[] rijnIV = Encoding.ASCII.GetBytes("abcdefg_abcdefg_");
                stringToDecrypt = DecryptIt(stringToDecrypt, rijnKey, rijnIV);
                return stringToDecrypt;
            }
            catch (Exception ee)
            {
                return ee.ToString();
            }
        }

        public string enryptor(string stringToDecrypt)
        {
            try
            {
                byte[] rijnKey = Encoding.ASCII.GetBytes("abcdefg_abcdefg_abcdefg_abcdefg_");
                byte[] rijnIV = Encoding.ASCII.GetBytes("abcdefg_abcdefg_");
                stringToDecrypt = EncryptIt(stringToDecrypt, rijnKey, rijnIV);
                return stringToDecrypt;
            }
            catch (Exception ee)
            {
                return ee.ToString();
            }
        }

        public String EncryptIt(String s, byte[] key, byte[] IV)
        {
            String result;
            RijndaelManaged rijn = new RijndaelManaged();
            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (ICryptoTransform encryptor = rijn.CreateEncryptor(key, IV))
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(s);
                        }
                    }
                }
                result = System.Convert.ToBase64String(msEncrypt.ToArray());
            }
            rijn.Clear();
            return result;
        }

        private String DecryptIt(String s, byte[] key, byte[] IV)
        {
            String result;
            RijndaelManaged rijn = new RijndaelManaged();
            using (MemoryStream msDecrypt = new MemoryStream(System.Convert.FromBase64String(s)))
            {
                using (ICryptoTransform decryptor = rijn.CreateDecryptor(key, IV))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader swDecrypt = new StreamReader(csDecrypt))
                        {
                            result = swDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            rijn.Clear();
            return result;
        }
    }

    class CreateLogs
    {
        public void logs(string value)
        {
            File.AppendAllText(Environment.CurrentDirectory + @"\logs.txt", value + Environment.NewLine);
        }
    }

    class FtpFileTransfer
    {
        public void FTP(string server, string fileName, string userName, string password, string fullName)
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
                    byteRead = fs.Read(buffer, 0, 1024);
                    ftpStream.Write(buffer, 0, byteRead);
                    read += (double)byteRead;
                    double percentage = read / total * 100;
            } while (byteRead != 0);
            fs.Close();
            ftpStream.Close();
        }

        public void FTPConnectionCheck(string hostFolder, string userName, string password)
        {
            FtpWebRequest myFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(hostFolder));
            myFTP.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
            myFTP.Credentials = new NetworkCredential(userName, password);
            WebResponse response = myFTP.GetResponse();
        }
    }

    class Compressionss
    {
        public void Compress(string sourceDirName, string destArchName)
        {
            ZipFile.CreateFromDirectory(sourceDirName, destArchName);
        }

        public void UnCompress(string archFile, string destinationDirName)
        {
            ZipFile.ExtractToDirectory(archFile, destinationDirName);
        }
    }

    class FileManagerClass
    {
        public string BinaryFileReader(string firstFile, string secondFile)
        {
            string file1 = "";
            string file2 = "";
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(firstFile))
                {
                    var hash = md5.ComputeHash(stream);
                    file1 = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }

                using (var stream1 = File.OpenRead(secondFile))
                {
                    var hash1 = md5.ComputeHash(stream1);
                    file2 = (BitConverter.ToString(hash1).Replace("-", "").ToLowerInvariant());
                }
            }

            StringBuilder strBuild = new StringBuilder();
            strBuild.AppendLine("File Binary 1: " + file1);
            strBuild.AppendLine("File Binary 2: " + file2);

            //return strBuild.ToString();

            //if(file1 == file2)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return file1 + " " + file2;
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
}
