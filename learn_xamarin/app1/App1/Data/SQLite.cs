using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace App1.Data
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection(); 
    }
}
