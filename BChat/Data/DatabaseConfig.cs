using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Data
{
    internal static class DatabaseConfig
    {

        public static string ConnectionString =
    "Server=DESKTOP-7DPOSF5;Database=WaBusinessManagerDB;Trusted_Connection=True;TrustServerCertificate=True;";




    //    public static string ConnectionString =
    //"Server=DESKTOP-ROIBB36\\MSSQLSERVER1;Database=WaBusinessManagerDB;Trusted_Connection=True;TrustServerCertificate=True;";
        //public static string ConnectionString =
        //    "Server=DESKTOP-ROIBB36;Database=WaBusinessManagerDB;Trusted_Connection=True;TrustServerCertificate=True;";
    }

    //DESKTOP-ROIBB36 -- Labtop

    //DESKTOP-FA969I1 -- PC
    //   "Server=DESKTOP-FA969I1;Database=WaBusinessManagerDB;Trusted_Connection=True;TrustServerCertificate=True;";
}


// Database In Azure 
//public static string ConnectionString =
//"Server=tcp:bchat-server.database.windows.net,1433;Initial Catalog=WaBusinessManagerDB;Persist Security Info=False;" +
//    "User ID=bchat_admin;Password=Bb@!737526695;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;" +
//    "Connection Timeout=30;";

