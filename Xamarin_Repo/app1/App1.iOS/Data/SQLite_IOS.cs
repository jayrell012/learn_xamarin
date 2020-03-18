using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App1.Data;
using App1.iOS.Data;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_IOS))]

namespace App1.iOS.Data
{
    public class SQLite_IOS : ISQLite
    {
        public SQLite_IOS() { }
        public SQLite.SQLiteConnection GetConnection()
        {
            var fileName = "TestDB.db3";
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var libraryPath = System.IO.Path.Combine(documentsPath, "..", "Library");
            var path = System.IO.Path.Combine(libraryPath, fileName);
            var connection = new SQLite.SQLiteConnection(path);

            return connection;
        }
    }
}