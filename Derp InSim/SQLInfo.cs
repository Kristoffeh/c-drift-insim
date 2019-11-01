using InSimDotNet.Helpers;
using MySql.Data.MySqlClient;
using System;
using System.IO;

namespace Derp_InSim
{
    public class SQLInfo
    {
        const string TimeFormat = "dd/MM/yyyy";//ex: 23/03/2003
        const string TimeFormatTwo = "dd/MM/yyyy HH:mm";//ex: 23/03/2003 23:23
        MySqlConnection SQL = new MySqlConnection();
        public SQLInfo() { }

        public bool IsConnectionStillAlive()
        {
            try
            {
                if (SQL.State == System.Data.ConnectionState.Open) return true;
                else return false;
            }
            catch { return false; }
        }
        
        // Load the database
        public bool StartUp(string server, string database, string username, string password)
        {
            try
            {
                if (IsConnectionStillAlive()) return true;

                SQL.ConnectionString = "Server=" + server +
                    ";Database=" + database +
                    ";Uid=" + username +
                    ";Pwd=" + password +
                    ";Connect Timeout=10;";
                SQL.Open();

                // Users table
                Query("CREATE TABLE IF NOT EXISTS drifters(PRIMARY KEY(username),username CHAR(25) NOT NULL,playername CHAR(30) NOT NULL,totaldistance decimal(11),regdate CHAR(16) NOT NULL,lastseen CHAR(16), timezone int(5), kmhormph int(5), rank CHAR(30), driftpoints int(4), timescrashed int(4), timeschatted int(4), timesreset int(4), timesjoined int(4), timesspectated int(4), kmxrg decimal(11), kmlx4 decimal(11), kmlx6 decimal(11), kmrb4 decimal(11), kmfxo decimal(11), kmxrt decimal(11), kmrac decimal(11), kmfz5 decimal(11), totalplaytime int(11));");

                // Banlist table
                Query("CREATE TABLE IF NOT EXISTS banlist(PRIMARY KEY(username),username CHAR(25) NOT NULL, playername CHAR(40) NOT NULL,bandate CHAR(10), banreason CHAR(75));");
            }
            catch { return false; }
            return true;
        }

        // Load query
        public int Query(string str)
        {
            try
            {
                MySqlCommand query = new MySqlCommand();
                query.Connection = SQL;
                query.CommandText = str;
                query.Prepare();
                return query.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                if (System.IO.File.Exists("files/sqlerror.log") == false) { FileStream CurrentFile = System.IO.File.Create("files/sqlerror.log"); CurrentFile.Close(); }

                StreamReader TextTempData = new StreamReader("files/sqlerror.log");
                string TempText = TextTempData.ReadToEnd();
                TextTempData.Close();

                StreamWriter TextData = new StreamWriter("files/sqlerror.log");
                TextData.WriteLine(TempText + DateTime.Now + ": Query Error - Exception: " + e);
                TextData.Flush();
                TextData.Close();
                return 0;
            }
        }

        #region Player Saving Stuff
        // Exist in database
        public bool UserExist(string username, string table = "drifters")
        {
            MySqlCommand query = new MySqlCommand();
            query.Connection = SQL;
            query.CommandText = "SELECT username FROM " + table + " WHERE username='" + username + "' LIMIT 1;";
            query.Prepare();
            MySqlDataReader dr = query.ExecuteReader();

            bool found = false;

            if (dr.Read()) if (dr.GetString(0) != "") found = true;
            dr.Close();

            return found;
        }


        // Users db count
        public int userCount()
        {
            MySqlCommand query = new MySqlCommand();
            query.Connection = SQL;
            query.CommandText = "SELECT COUNT(*) FROM drifters";
            query.Prepare();
            MySqlDataReader dr = query.ExecuteReader();

            if (dr.Read())
                if (dr.GetString(0) != "")
                    dr.Close();

            return Convert.ToInt32(query.ExecuteScalar());
        }

        // Banned users count
        public int bansCount()
        {
            MySqlCommand query = new MySqlCommand();
            query.Connection = SQL;
            query.CommandText = "SELECT COUNT(*) FROM banlist";
            query.Prepare();
            MySqlDataReader dr = query.ExecuteReader();

            if (dr.Read())
                if (dr.GetString(0) != "")
                    dr.Close();

            return Convert.ToInt32(query.ExecuteScalar());
        }

        // Add user to database
        public void AddUser(string username, string playername, decimal totaldistance, int timezone, int KMHorMPH, string rank, int dp, int timescrashed, int timeschatted, int timesreset, int timesjoined, int timesspectated, decimal kmxrg, decimal kmlx4, decimal kmlx6, decimal kmrb4, decimal kmfxo, decimal kmxrt, decimal kmrac, decimal kmfz5, int totalplaytime)
        {
            try
            {
                if (username == "") return;
                Query("INSERT INTO drifters VALUES ('" + username + "', '" + StringHelper.StripColors(RemoveStupidCharacters(playername)) +  
                    "', " + totaldistance + ", '" + DateTime.UtcNow.ToString(TimeFormat) + "', '" + DateTime.UtcNow.ToString(TimeFormatTwo) + 
                    "', " + timezone + ", " + KMHorMPH + ", '" + rank + "', " + dp + ", " + timescrashed + ", " + timeschatted + ", " + timesreset + ", " + timesjoined + 
                    ", " + timesspectated + ", " + kmxrg + ", " + kmlx4 + ", " + kmlx6 + ", " + kmrb4 + ", " + kmfxo + ", " + kmxrt + ", " + kmrac + ", " + kmfz5 + ", " + totalplaytime + ");");
            }
            catch
            {

            }
        }
        // Add user to banlist
        public void AddtoBanlist(string username, string playername, string bandate, string banreason)
        {
            if (username == "") return;
            Query("INSERT INTO banlist VALUES ('" + username + "', '" + RemoveStupidCharacters(playername) + "', '" + bandate + "', '" + banreason + "');");
        }

        public void UpdateUser(string username, string playername, bool updatejointime, decimal totaldistance = 0, int timezone = 0, int KMHorMPH = 0, string rank = "", int driftpoints = 0, int timescrashed = 0, int timeschatted = 0, int timesreset = 0, int timesjoined = 0, int timesspectated = 0, decimal kmxrg = 0, decimal kmlx4 = 0, decimal kmlx6 = 0, decimal kmrb4 = 0, decimal kmfxo = 0, decimal kmxrt = 0, decimal kmrac = 0, decimal kmfz5 = 0, int totalplaytime = 0)
        {
            try
            {
                if (updatejointime) Query("UPDATE drifters SET lastseen='" + DateTime.UtcNow.ToString(TimeFormatTwo) + "' WHERE username='" + username + "';");
                else
                {
                    Query("UPDATE drifters SET playername='" + StringHelper.StripColors(RemoveStupidCharacters(playername)) + "', totaldistance=" + totaldistance +
                ", lastseen='" + DateTime.UtcNow.ToString(TimeFormatTwo) + "', timezone=" + timezone + ", kmhormph=" + KMHorMPH + ", rank='" + rank + "', driftpoints=" +
                driftpoints + ", timescrashed=" + timescrashed + ", timeschatted=" + timeschatted + ", timesreset=" + timesreset + ", timesjoined=" + timesjoined +
                ", timesspectated=" + timesspectated + ", kmxrg=" + kmxrg + ", kmlx4=" + kmlx4 + ", kmlx6=" + kmlx6 + ", kmrb4=" + kmrb4 + ", kmfxo=" + kmfxo +
                ", kmxrt=" + kmxrt + ", kmrac=" + kmrac + ", kmfz5=" + kmfz5 + ", totalplaytime=" + totalplaytime + " WHERE username='" + username + "';");
                }
            }
            catch {
                // error
            }
        }

        // Load their options
        public string[] LoadUserOptions(string username)
        {
            string[] options = new string[21];

            MySqlCommand query = new MySqlCommand();
            query.Connection = SQL;
            query.CommandText = "SELECT totaldistance, regdate, lastseen, timezone, kmhormph, rank, driftpoints, timescrashed, timeschatted, timesreset, timesjoined, timesspectated, kmxrg, kmlx4, kmlx6, kmrb4, kmfxo, kmxrt, kmrac, kmfz5, totalplaytime FROM drifters WHERE username='" + username + "' LIMIT 1;";
            query.Prepare();
            MySqlDataReader dr = query.ExecuteReader();

            if (dr.Read())
                if (dr.GetString(0) != "")
                {
                    options[0] = dr.GetString(0);
                    options[1] = dr.GetString(1);
                    options[2] = dr.GetString(2);
                    options[3] = dr.GetString(3);
                    options[4] = dr.GetString(4);
                    options[5] = dr.GetString(5);
                    options[6] = dr.GetString(6);
                    options[7] = dr.GetString(7);
                    options[8] = dr.GetString(8);
                    options[9] = dr.GetString(9);
                    options[10] = dr.GetString(10);
                    options[11] = dr.GetString(11);
                    options[12] = dr.GetString(12);
                    options[13] = dr.GetString(13);
                    options[14] = dr.GetString(14);
                    options[15] = dr.GetString(15);
                    options[16] = dr.GetString(16);
                    options[17] = dr.GetString(17);
                    options[18] = dr.GetString(18);
                    options[19] = dr.GetString(19);
                    options[20] = dr.GetString(20);


                }
            dr.Close();

            return options;
        }

        public string RemoveStupidCharacters(string text)
        {
            if (text.Contains("'")) text = text.Replace('\'', '`');
            if (text.Contains("‘")) text = text.Replace('‘', '`');
            if (text.Contains("’")) text = text.Replace('’', '`');
            if (text.Contains("^h")) text = text.Replace("^h", "#");

            return text;
        }
        #endregion
    }
}
