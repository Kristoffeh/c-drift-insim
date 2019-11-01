using System;
using System.Threading;
using InSimDotNet.Packets;
using System.Globalization;
using System.Windows.Forms;
using InSimDotNet.Helpers;

namespace Derp_InSim
{
    public partial class Form1
    {
        System.Timers.Timer SQLReconnectTimer = new System.Timers.Timer();
        System.Timers.Timer SaveTimer = new System.Timers.Timer();
        System.Timers.Timer DriftMeter = new System.Timers.Timer();
        System.Timers.Timer DP = new System.Timers.Timer();

        public void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

                #region ' Timer '

                System.Timers.Timer Payout = new System.Timers.Timer();
                Payout.Elapsed += new System.Timers.ElapsedEventHandler(Payout_Timer);
                Payout.Interval = 3000;
                Payout.Enabled = true;

                // SQL timer
                SQLReconnectTimer.Interval = 10000;
                SQLReconnectTimer.Elapsed += new System.Timers.ElapsedEventHandler(SQLReconnectTimer_Elapsed);

                // Save timer
                SaveTimer.Interval = 2000;
                SaveTimer.Elapsed += new System.Timers.ElapsedEventHandler(Savetimer_Elapsed);
                SaveTimer.Enabled = true;

                // Update GUI
                DriftMeter.Interval = 400;
                DriftMeter.Elapsed += new System.Timers.ElapsedEventHandler(DriftMeter_Elapsed);
                DriftMeter.Enabled = true;

                // Drift Points Generator
                DP.Interval = 1000;
                DP.Elapsed += new System.Timers.ElapsedEventHandler(DP_Elapsed);
                DP.Enabled = true;

                #endregion
            }
            catch (Exception error)
            {
                {
                    MessageBox.Show("" + error.Message, "AN ERROR OCCURED");
                }
            }
        }

        private void Payout_Timer(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");
                {
                    foreach (var Conn in _players.Values)
                    {
                        var CurrentConnection = GetConnection(Conn.PLID);

                        if ((Conn.PName == HostName && Conn.UCID == 0) == false)
                        {
                            if (Conn.CName == "UF1" || Conn.CName == "XFG" || Conn.CName == "XRG" || Conn.CName == "MRT")
                            {
                                if (Conn.kmh > 50)
                                {
                                    CurrentConnection.cash += 1;
                                    UpdateGui(Conn.UCID, true, false);
                                }
                            }
                            else if (Conn.CName == "LX4" || Conn.CName == "LX6" || Conn.CName == "RB4" || Conn.CName == "FXO" || Conn.CName == "XRT" || Conn.CName == "RAC" || Conn.CName == "FZ5")
                            {
                                if (Conn.kmh > 30)
                                {
                                    CurrentConnection.cash += 1;
                                    UpdateGui(Conn.UCID, true, false);
                                }
                            }
                            else if (Conn.CName == "UFR" || Conn.CName == "XFR" || Conn.CName == "FXR" || Conn.CName == "XRR" || Conn.CName == "FZR" || Conn.CName == "MRT" || Conn.CName == "FBM" || Conn.CName == "FOX")
                            {
                                if (Conn.kmh > 30)
                                {
                                    CurrentConnection.cash += 1;
                                    UpdateGui(Conn.UCID, true, false);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception error)
            {
                
                {
                    MessageBox.Show("" + error.Message, "AN ERROR OCCURED");
                }
            }
        }

        private void SQLReconnectTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            {
                SQLRetries++;
                ConnectedToSQL = SqlInfo.StartUp(SQLIPAddress, SQLDatabase, SQLUsername, SQLPassword);
                if (!ConnectedToSQL)
                {
                    MessageToAdmins("SQL connect attempt failed! Attempting to reconnect in ^310 ^8seconds!");
                }
                else
                {
                    MessageToAdmins("SQL connected after ^3" + SQLRetries + " ^8times!");
                    SQLRetries = 0;
                    SQLReconnectTimer.Stop();
                }
            }
        }

        private void Savetimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                foreach (var c in _connections.Values)
                {
                    c.totalplaytime += 1;
                }

                foreach (var conn in _connections.Values)
                {
                    if (ConnectedToSQL)
                    {
                        try { SqlInfo.UpdateUser(_connections[conn.UCID].UName, StringHelper.StripColors(SqlInfo.RemoveStupidCharacters(_connections[conn.UCID].PName)), 
                            false, _connections[conn.UCID].TotalDistance, _connections[conn.UCID].Timezone, _connections[conn.UCID].KMHorMPH, _connections[conn.UCID].Rank, 
                            _connections[conn.UCID].Driftpoints, _connections[conn.UCID].timesCrashed, _connections[conn.UCID].timesChatted, _connections[conn.UCID].timesReset, 
                            _connections[conn.UCID].timesJoined, _connections[conn.UCID].timesSpectated, _connections[conn.UCID].kmXRG, _connections[conn.UCID].kmLX4, 
                            _connections[conn.UCID].kmLX6, _connections[conn.UCID].kmRB4, _connections[conn.UCID].kmFXO, _connections[conn.UCID].kmXRT, _connections[conn.UCID].kmRAC, 
                            _connections[conn.UCID].kmFZ5, _connections[conn.UCID].totalplaytime); }
                        catch (Exception EX)
                        {
                            if (!SqlInfo.IsConnectionStillAlive())
                            {
                                ConnectedToSQL = false;
                                SQLReconnectTimer.Start();
                            }
                            LogTextToFile("sqlerror", "[" + conn.UCID + "] " + StringHelper.StripColors(_connections[conn.UCID].PName) + "(" + _connections[conn.UCID].UName + ") conn - Exception: " + EX.Message, false);
                        }
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show("" + f.Message, "AN ERROR OCCURED");
            }
        }

        private void DriftMeter_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                foreach (var conn in _players.Values)
                {
                    var CurrentConnection = GetConnection(conn.PLID);
                    if (CurrentConnection.PName != HostName && CurrentConnection.UCID != 0)
                    {

                        // Drift points total
                        insim.Send(new IS_BTN
                        {
                            Text = "^7" + _connections[CurrentConnection.UCID].Driftpoints + " dp",
                            UCID = CurrentConnection.UCID,
                            ReqI = 3,
                            ClickID = 3,
                            BStyle = ButtonStyles.ISB_LEFT,
                            H = 4,
                            W = 10,
                            T = 0,
                            L = 75,
                        });

                        // Drift points temp
                        insim.Send(new IS_BTN
                        {
                            Text = "^2+" + _connections[CurrentConnection.UCID].DP + " dp",
                            UCID = CurrentConnection.UCID,
                            ReqI = 11,
                            ClickID = 11,
                            BStyle = ButtonStyles.ISB_C2,
                            H = 4,
                            W = 20,
                            T = 0,
                            L = 108,
                        });

                        // Drift angle
                        insim.Send(new IS_BTN
                        {
                            Text = "^7" + _connections[CurrentConnection.UCID].Angle + "°",
                            UCID = _connections[CurrentConnection.UCID].UCID,
                            ReqI = 9,
                            ClickID = 9,
                            BStyle = ButtonStyles.ISB_C2,
                            H = 4,
                            W = 5,
                            T = 0,
                            L = 136,
                        });

                        // Drift angle
                        insim.Send(new IS_BTN
                        {
                            Text = "",
                            UCID = _connections[CurrentConnection.UCID].UCID,
                            ReqI = 10,
                            ClickID = 10,
                            BStyle = ButtonStyles.ISB_C2 | ButtonStyles.ISB_DARK,
                            H = 5,
                            W = 5,
                            T = 0,
                            L = 136,
                        });

                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show("" + f.Message, "AN ERROR OCCURED");
            }
        }

        private void DP_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                foreach (var conn in _players.Values)
                {
                    var CConn = GetConnection(conn.PLID);
                    if (CConn.PName != HostName && CConn.UCID != 0)
                    {
                        GetRank(CConn.UCID);

                        


                        #region ' Payout '
                        if (CConn.DP > 1)
                        {
                            if (conn.kmh == 0 || conn.kmh < 30)
                            {
                                CConn.DP = 0;
                                bool msgSent = false;

                                if (msgSent == false)
                                {
                                    insim.Send(CConn.UCID, "^1You lost all of your drift points!");
                                    msgSent = true;
                                }

                                msgSent = false;
                            }
                        }

                        if (conn.kmh >= 30 && conn.kmh <= 40)
                        {
                            if (CConn.Angle >= 7 && CConn.Angle <= 10)
                            {
                                CConn.DP += 2;
                            }
                            else if (CConn.Angle >= 11 && CConn.Angle <= 20)
                                {
                                CConn.DP += 4;
                            }
                            else if (CConn.Angle >= 21 && CConn.Angle <= 30)
                                {
                                CConn.DP += 6;
                            }
                            else if (CConn.Angle >= 31 && CConn.Angle <= 40)
                                {
                                CConn.DP += 8;
                            }
                        }
                        else if (conn.kmh >= 41 && conn.kmh <= 60)
                        {
                            if (CConn.Angle >= 7 && CConn.Angle <= 10)
                            {
                                CConn.DP += 2;
                            }
                            else if (CConn.Angle >= 11 && CConn.Angle <= 20)
                            {
                                CConn.DP += 4;
                            }
                            else if (CConn.Angle >= 21 && CConn.Angle <= 30)
                            {
                                CConn.DP += 6;
                            }
                            else if (CConn.Angle >= 31 && CConn.Angle <= 40)
                            {
                                CConn.DP += 7;
                            }
                        }
                        else if (conn.kmh >= 61 && conn.kmh <= 80)
                        {
                            if (CConn.Angle >= 7 && CConn.Angle <= 10)
                            {
                                CConn.DP += 2;
                            }
                            else if (CConn.Angle >= 11 && CConn.Angle <= 20)
                            {
                                CConn.DP += 4;
                            }
                            else if (CConn.Angle >= 21 && CConn.Angle <= 30)
                            {
                                CConn.DP += 6;
                            }
                            else if (CConn.Angle >= 31 && CConn.Angle <= 40)
                            {
                                CConn.DP += 7;
                            }
                        }
                        else if (conn.kmh >= 81 && conn.kmh <= 100)
                        {
                            if (CConn.Angle >= 7 && CConn.Angle <= 10)
                            {
                                CConn.DP += 7;
                            }
                            else if (CConn.Angle >= 11 && CConn.Angle <= 20)
                            {
                                CConn.DP += 8;
                            }
                            else if (CConn.Angle >= 21 && CConn.Angle <= 30)
                            {
                                CConn.DP += 10;
                            }
                            else if (CConn.Angle >= 31 && CConn.Angle <= 40)
                            {
                                CConn.DP += 11;
                            }
                        }
                        else if (conn.kmh >= 101 && conn.kmh <= 130)
                        {
                            if (CConn.Angle >= 7 && CConn.Angle <= 10)
                            {
                                CConn.DP += 7;
                            }
                            else if (CConn.Angle >= 11 && CConn.Angle <= 20)
                            {
                                CConn.DP += 8;
                            }
                            else if (CConn.Angle >= 21 && CConn.Angle <= 30)
                            {
                                CConn.DP += 10;
                            }
                            else if (CConn.Angle >= 31 && CConn.Angle <= 40)
                            {
                                CConn.DP += 11;
                            }
                        }
                        else if (conn.kmh >= 131 && conn.kmh <= 150)
                        {
                            if (CConn.Angle >= 7 && CConn.Angle <= 10)
                            {
                                CConn.DP += 7;
                            }
                            else if (CConn.Angle >= 11 && CConn.Angle <= 20)
                            {
                                CConn.DP += 8;
                            }
                            else if (CConn.Angle >= 21 && CConn.Angle <= 30)
                            {
                                CConn.DP += 10;
                            }
                            else if (CConn.Angle >= 31 && CConn.Angle <= 40)
                            {
                                CConn.DP += 11;
                            }
                        }
                        else if (conn.kmh >= 151 && conn.kmh <= 200)
                        {
                            if (CConn.Angle >= 7 && CConn.Angle <= 10)
                            {
                                CConn.DP += 7;
                            }
                            else if (CConn.Angle >= 11 && CConn.Angle <= 20)
                            {
                                CConn.DP += 8;
                            }
                            else if (CConn.Angle >= 21 && CConn.Angle <= 30)
                            {
                                CConn.DP += 10;
                            }
                            else if (CConn.Angle >= 31 && CConn.Angle <= 40)
                            {
                                CConn.DP += 11;
                            }
                        }

                        #endregion
                    }
                }
            }
            catch (Exception f)
            {
                MessageBox.Show("" + f.Message, "AN ERROR OCCURED");
            }
        }

        private void MessageReceived(IS_MSO mso)
        {
            try
            {

                string Msg = mso.Msg.Substring(mso.TextStart, (mso.Msg.Length - mso.TextStart));

                if (Msg.StartsWith("!") == false)
                {
                    if (mso.UCID == 0 == false && _connections[mso.UCID].PName == HostName == false)
                    {
                        _connections[mso.UCID].timesChatted += 1;
                    }
                }
            }
            catch { }
        }
    }
}
