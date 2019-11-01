using System;
using System.Collections.Generic;
using System.Windows.Forms;
using InSimDotNet;
using InSimDotNet.Packets;
using System.Globalization;
using System.Threading;
using InSimDotNet.Helpers;
using System.IO;

namespace Derp_InSim
{
    public partial class Form1 : Form
    {
        InSim insim = new InSim();
        
        // Global Vars
        //public const string Tag = "^5EC^0™";
        public const string InSimVersion = "0.4b";
        public string website = "www.website.com";
        // nonst string AVAILABLE_CARS = "UF1+XFG+XRG+LX4+LX6+RB4+FXO+XRT+RAC+FZ5+UFR+XFR+FXR+XRR+FZR+MRT+FBM+FOX+FO8+BF1";



        public string TrackName = "None";
        public string HostName = "host";
        public string LayoutName = "None";
        public int dbCount = 0;
        public int dbBans = 0;

        public string DisplaysOpenedMsg = "^1Other InSim windows are already open, close them and try again!";

        // MySQL Variables
        public SQLInfo SqlInfo = new SQLInfo();
        public bool ConnectedToSQL = false;
        public int SQLRetries = 0;

        // MySQL Connect
        string SQLIPAddress = "127.0.0.1";
        string SQLDatabase = "lfs";
        string SQLUsername = "root";
        string SQLPassword = "";

        public bool AskedPosition = false;
        public byte AskedPosUCID = 255;

        class Connections
        {
            // NCN fields
            public byte UCID;
            public string UName;
            public string PName;
            public bool IsAdmin;

            // Custom Fields
            public bool IsSuperAdmin;
            
            public bool OnTrack;

            // public byte Interface;
            public bool DisplaysOpen;
            public bool inInfo;
            public bool inStats;

            // Drift system
            public int Driftpoints;
            public int DP;
            public string Rank;
            public int timesCrashed;
            public int timesChatted;
            public int timesReset;
            public int timesJoined;
            public int timesSpectated;
            public decimal kmXRG;
            public decimal kmLX4;
            public decimal kmLX6;
            public decimal kmRB4;
            public decimal kmFXO;
            public decimal kmXRT;
            public decimal kmRAC;
            public decimal kmFZ5;

            public int totalplaytime;

            public int Angle;

            public bool serverTime;

            public int cash;
            public string regdate;
            public string lastseen;

            public string bandate;
            public string banreason;

            public string Date;
            public string DateTime;
            public int Timezone;

            public int KMHorMPH;

            public decimal TotalDistance;
            public int _todayscash;
            public int _initialcash;

            public string CurrentCar = "None";

            public int TodaysCash
            {
                get { return _todayscash; }
                set { _todayscash = value; }
            }

            public int InitialCash
            {
                get { return _initialcash; }
                set { _initialcash = value; }
            }

        }
        class Players
        {
            public byte UCID;
            public byte PLID;
            public string PName;
            public string CName;

            public int kmh;
            public int mph;
            public string Plate;
        }

        private Dictionary<byte, Connections> _connections = new Dictionary<byte, Connections>();
        private Dictionary<byte, Players> _players = new Dictionary<byte, Players>();

        public Form1()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            InitializeComponent();
            RunInSim();
        }

        void RunInSim()
        {

            // Bind packet events.
            insim.Bind<IS_NCN>(NewConnection);
            insim.Bind<IS_NPL>(NewPlayer);
            insim.Bind<IS_MSO>(MessageReceived);
            insim.Bind<IS_MCI>(MultiCarInfo);
            insim.Bind<IS_CNL>(ConnectionLeave);
            insim.Bind<IS_CPR>(ClientRenames);
            insim.Bind<IS_PLL>(PlayerLeave);
            insim.Bind<IS_STA>(OnStateChange);
            insim.Bind<IS_BTC>(ButtonClicked);
            insim.Bind<IS_BFN>(ClearButtons);
            insim.Bind<IS_VTN>(VoteNotify);
            insim.Bind<IS_AXI>(OnAutocrossInformation);
            insim.Bind<IS_TINY>(OnTinyReceived);
            insim.Bind<IS_LAP>(LAPTime);
            insim.Bind<IS_CRS>(CarReset);
            insim.Bind<IS_CON>(OnCarContact);

            // Initialize InSim
            insim.Initialize(new InSimSettings
            {
                Host = "127.0.0.1", // 93.190.143.115
                Port = 29999,
                Admin = "", // set your admin password here and log in with it in LFS
                Prefix = '!',
                Flags = InSimFlags.ISF_MCI | InSimFlags.ISF_MSO_COLS | InSimFlags.ISF_CON,

                Interval = 100
            });

            insim.Send(new[]
            {
                new IS_TINY { SubT = TinyType.TINY_NCN, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_NPL, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_ISM, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_SST, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_MCI, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_NCI, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_AXI, ReqI = 255 },
                new IS_TINY { SubT = TinyType.TINY_SST, ReqI = 255 },
                });

            insim.Send("/laps 0");
            insim.Send("/cars XRG+LX4+LX6+RB4+FXO+XRT+RAC+FZ5");
            // insim.Send(255, 0, "^8InSim connected with version ^2" + InSimVersion);

            ConnectedToSQL = SqlInfo.StartUp(SQLIPAddress, SQLDatabase, SQLUsername, SQLPassword);
            if (!ConnectedToSQL)
            {
                insim.Send(255, "SQL connect attempt failed! Attempting to reconnect in ^310 ^8seconds!");
                SQLReconnectTimer.Start();
                SaveTimer.Start();
            }
            else MessageToAdmins("Connected to MySQL database!");
        }

        #region ' Misc '

        bool TryParseCommand(IS_MSO mso, out string[] args)
        {
            if (mso.UserType == UserType.MSO_PREFIX)
            {
                var message = mso.Msg.Substring(mso.TextStart);
                args = message.Split();
                return args.Length > 0;
            }

            args = null;
            return false;
        }

        /// <summary>Returns true if method needs invoking due to threading</summary>
        private bool DoInvoke()
        {
            foreach (Control c in this.Controls)
            {
                if (c.InvokeRequired) return true;
                break;	// 1 control is enough
            }
            return false;
        }
        #endregion

        // Player joins server
        void NewConnection(InSim insim, IS_NCN packet)
        {
            try
            {
                _connections.Add(packet.UCID, new Connections
                {
                    UCID = packet.UCID,
                    UName = packet.UName,
                    PName = packet.PName,
                    IsAdmin = packet.Admin,

                    IsSuperAdmin = GetUserAdmin(packet.UName),
                    OnTrack = false,
                    TotalDistance = 0,
                    DisplaysOpen = false,
                    inInfo = false,
                    DateTime = "" + DateTime.UtcNow.Hour + DateTime.UtcNow.Minute,
                    Date = "" + DateTime.UtcNow.Hour + DateTime.UtcNow.Minute,
                    Timezone = 0,
                    bandate = "",
                    banreason = "",
                    serverTime = false,
                    KMHorMPH = 0,
                    Angle = 0,
                    DP = 0,
                    Rank = "",
                    Driftpoints = 0,
                    timesCrashed = 0,
                    timesChatted = 0,
                    timesReset = 0,
                    timesSpectated = 0,
                    timesJoined = 0,
                    kmXRG = 0,
                    kmLX4 = 0,
                    kmLX6 = 0,
                    kmRB4 = 0,
                    kmFXO = 0,
                    kmXRT = 0,
                    kmRAC = 0,
                    kmFZ5 = 0,
                    totalplaytime = 0,
                });

                if (ConnectedToSQL && packet.UName != "")
                {
                    try
                    {
                        if (SqlInfo.UserExist(packet.UName))
                        {
                            
                            SqlInfo.UpdateUser(packet.UName, packet.PName, true);//Updates the last joined time to the current one

                            string[] LoadedOptions = SqlInfo.LoadUserOptions(packet.UName);
                            _connections[packet.UCID].TotalDistance = Convert.ToDecimal(LoadedOptions[0]);
                            _connections[packet.UCID].regdate = LoadedOptions[1];
                            _connections[packet.UCID].lastseen = LoadedOptions[2];
                            _connections[packet.UCID].Timezone = Convert.ToInt32(LoadedOptions[3]);
                            _connections[packet.UCID].KMHorMPH = Convert.ToInt32(LoadedOptions[4]);
                            _connections[packet.UCID].Rank = LoadedOptions[5];
                            _connections[packet.UCID].Driftpoints = Convert.ToInt32(LoadedOptions[6]);
                            _connections[packet.UCID].timesCrashed = Convert.ToInt32(LoadedOptions[7]);
                            _connections[packet.UCID].timesChatted = Convert.ToInt32(LoadedOptions[8]);
                            _connections[packet.UCID].timesReset = Convert.ToInt32(LoadedOptions[9]);
                            _connections[packet.UCID].timesJoined = Convert.ToInt32(LoadedOptions[10]);
                            _connections[packet.UCID].timesSpectated = Convert.ToInt32(LoadedOptions[11]);
                            _connections[packet.UCID].kmXRG = Convert.ToDecimal(LoadedOptions[12]);
                            _connections[packet.UCID].kmLX4 = Convert.ToDecimal(LoadedOptions[13]);
                            _connections[packet.UCID].kmLX6 = Convert.ToDecimal(LoadedOptions[14]);
                            _connections[packet.UCID].kmRB4 = Convert.ToDecimal(LoadedOptions[15]);
                            _connections[packet.UCID].kmFXO = Convert.ToDecimal(LoadedOptions[16]);
                            _connections[packet.UCID].kmXRT = Convert.ToDecimal(LoadedOptions[17]);
                            _connections[packet.UCID].kmRAC = Convert.ToDecimal(LoadedOptions[18]);
                            _connections[packet.UCID].kmFZ5 = Convert.ToDecimal(LoadedOptions[19]);
                            _connections[packet.UCID].totalplaytime = Convert.ToInt32(LoadedOptions[20]);

                            if (packet.PName != HostName && packet.UCID != 0)
                            {
                                insim.Send(255, "" + packet.PName + " ^8was last seen at ^3" + _connections[packet.UCID].lastseen);
                                insim.Send(packet.UCID, "^8Welcome back, " + packet.PName);
                                insim.Send(packet.UCID, "^8Visit ^3" + website + " ^8for global statistics and more info");
                            }


                        }
                        else
                        {
                            if (packet.PName != HostName && packet.UCID != 0)
                            {
                                insim.Send(255, packet.PName + " ^8(" + packet.UName + ") joined the server for the first time!");
                            }

                            SqlInfo.AddUser(_connections[packet.UCID].UName, StringHelper.StripColors(SqlInfo.RemoveStupidCharacters(_connections[packet.UCID].PName)), _connections[packet.UCID].TotalDistance, _connections[packet.UCID].Timezone, _connections[packet.UCID].KMHorMPH, _connections[packet.UCID].Rank, _connections[packet.UCID].Driftpoints, _connections[packet.UCID].timesCrashed, _connections[packet.UCID].timesChatted, _connections[packet.UCID].timesReset, _connections[packet.UCID].timesJoined, _connections[packet.UCID].timesSpectated, _connections[packet.UCID].kmXRG, _connections[packet.UCID].kmLX4, _connections[packet.UCID].kmLX6, _connections[packet.UCID].kmRB4, _connections[packet.UCID].kmFXO, _connections[packet.UCID].kmXRT, _connections[packet.UCID].kmRAC, _connections[packet.UCID].kmFZ5, _connections[packet.UCID].totalplaytime);
                        }


                        _connections[packet.UCID].timesJoined += 1;
                        UpdateGui(packet.UCID, true, true, true);
                        GetRank(packet.UCID);

                        // Rank value
                        insim.Send(new IS_BTN
                        {
                            Text = "^7" + _connections[packet.UCID].Rank,
                            UCID = packet.UCID,
                            ReqI = 6,
                            ClickID = 6,
                            BStyle = ButtonStyles.ISB_LEFT,
                            H = 4,
                            W = 16,
                            T = 0,
                            L = 91,
                        });

                        dbCount = SqlInfo.userCount();
                        dbBans = SqlInfo.bansCount();

                    }
                    catch (Exception EX)
                    {
                        if (!SqlInfo.IsConnectionStillAlive())
                        {
                            ConnectedToSQL = false;
                            SQLReconnectTimer.Start();
                        }
                        LogTextToFile("sqlerror", "[" + packet.UCID + "] " + StringHelper.StripColors(packet.PName) + "(" + packet.UName + ") NCN - Exception: " + EX, false);
                    }
                }

                #region ' Retrieve HostName '
                if (packet.UCID == 0 && packet.UName == "")
                {
                    HostName = packet.PName;
                }
                #endregion
            }
            catch (Exception e) { LogTextToFile("error", "[" + packet.UCID + "] " + StringHelper.StripColors(packet.PName) + "(" + packet.UName + ") NCN - Exception: " + e.Message, false); }
        }


        // Player joins race or enter track
        void NewPlayer(InSim insim, IS_NPL packet)
        {
            try
            {
                var r = GetConnection(packet.PLID);

                if (_players.ContainsKey(packet.PLID))
                {
                    // Leaving pits, just update NPL object.
                    _players[packet.PLID].UCID = packet.UCID;
                    _players[packet.PLID].PLID = packet.PLID;
                    _players[packet.PLID].PName = packet.PName;
                    _players[packet.PLID].CName = packet.CName;
                    _players[packet.PLID].Plate = packet.Plate;
                }
                else
                {
                    // Add new player.
                    _players.Add(packet.PLID, new Players
                    {
                        UCID = packet.UCID,
                        PLID = packet.PLID,
                        PName = packet.PName,
                        CName = packet.CName,
                        Plate = packet.Plate,
                    });
                }

                _connections[packet.UCID].OnTrack = true;
                _connections[packet.UCID].CurrentCar = packet.CName;

            }
            catch (Exception e) { LogTextToFile("error", "[" + packet.UCID + "] " + StringHelper.StripColors(packet.PName) + "(" + GetConnection(packet.PLID).UName + ") NPL - Exception: " + e, false); }
        }

        // Player left the server
        void ConnectionLeave(InSim insim, IS_CNL CNL)
        {
            try
            {
                LogTextToFile("connections", _connections[CNL.UCID].PName + " (" + _connections[CNL.UCID].UName + ") Disconnected", false);

                // Save values of user - CNL (on disconnect)

                if (ConnectedToSQL)
                {
                    try { SqlInfo.UpdateUser(_connections[CNL.UCID].UName, StringHelper.StripColors(SqlInfo.RemoveStupidCharacters(_connections[CNL.UCID].PName)), true, _connections[CNL.UCID].TotalDistance, _connections[CNL.UCID].Timezone, _connections[CNL.UCID].KMHorMPH, _connections[CNL.UCID].Rank, _connections[CNL.UCID].Driftpoints, _connections[CNL.UCID].timesCrashed, _connections[CNL.UCID].timesChatted, _connections[CNL.UCID].timesReset, _connections[CNL.UCID].timesJoined, _connections[CNL.UCID].timesSpectated, _connections[CNL.UCID].kmXRG, _connections[CNL.UCID].kmLX4, _connections[CNL.UCID].kmLX6, _connections[CNL.UCID].kmRB4, _connections[CNL.UCID].kmFXO, _connections[CNL.UCID].kmXRT, _connections[CNL.UCID].kmRAC, _connections[CNL.UCID].kmFZ5, _connections[CNL.UCID].totalplaytime); }
                    catch (Exception EX)
                    {
                        if (!SqlInfo.IsConnectionStillAlive())
                        {
                            ConnectedToSQL = false;
                            SQLReconnectTimer.Start();
                        }
                        LogTextToFile("sqlerror", "[" + CNL.UCID + "] " + StringHelper.StripColors(_connections[CNL.UCID].PName) + "(" + _connections[CNL.UCID].UName + ") CNL - Exception: " + EX.Message, false);
                    }
                }

                _connections.Remove(CNL.UCID);
            }
            catch (Exception e) {  LogTextToFile("error", "[" + CNL.UCID + "] " + StringHelper.StripColors(_connections[CNL.UCID].PName) + "(" + _connections[CNL.UCID].UName + ") CNL - Exception: " + e, false); }
        }

        // Button click (is_btn click ID's)
        void ButtonClicked(InSim insim, IS_BTC BTC)
        {
            try { BTC_ClientClickedButton(BTC); }
            catch (Exception e) { LogTextToFile("error", "[" + BTC.UCID + "] " + StringHelper.StripColors(_connections[BTC.UCID].PName) + "(" + _connections[BTC.UCID].UName + ") BTC - Exception: " + e, false); }
        }

        // BuTton FunctioN (IS_BFN, SHIFT + I)
        void ClearButtons(InSim insim, IS_BFN BFN)
        {
            try
            {
                insim.Send(BFN.UCID, "^8InSim buttons cleared ^7(SHIFT + I)"); _connections[BFN.UCID].serverTime = false; UpdateGui(BFN.UCID, true, true, true);

                if (_connections[BFN.UCID].inInfo == true)
                {
                    _connections[BFN.UCID].serverTime = false;
                    _connections[BFN.UCID].inInfo = false;
                }

                if (_connections[BFN.UCID].DisplaysOpen == true)
                {
                    _connections[BFN.UCID].DisplaysOpen = false;
                }

                if (_connections[BFN.UCID].inStats == true)
                {
                    _connections[BFN.UCID].inStats = false;
                }
            }
            catch (Exception e)
            { LogTextToFile("error", "[" + BFN.UCID + "] " + StringHelper.StripColors(_connections[BFN.UCID].PName) + "(" + _connections[BFN.UCID].UName + ") BFN - Exception: " + e, false); }
        }

        // Autocross information
        private void OnAutocrossInformation(InSim insim, IS_AXI AXI)
        {
            try
            {
                if (AXI.NumO != 0)
                {
                    LayoutName = AXI.LName;
                    if (AXI.ReqI == 0) insim.Send(255, "Layout loaded");
                }
            }
            catch (Exception EX) { LogTextToFile("error", "AXI - " + EX.Message); }
        }

        // Vote notify (cancel votes)
        private void VoteNotify(InSim insim, IS_VTN VTN)
        {
            try
            {
                foreach (var conn in _connections.Values)
                {
                    if (conn.UCID == VTN.UCID)
                    {
                        if (VTN.Action == VoteAction.VOTE_END)
                        {
                            if (_connections[VTN.UCID].IsAdmin != true)
                            {
                                insim.Send("/cv");
                            }
                        }

                        if (VTN.Action == VoteAction.VOTE_RESTART)
                        {
                            if (_connections[VTN.UCID].IsAdmin != true)
                            {
                                insim.Send("/cv");
                            }
                        }


                    }
                }

            }
            catch (Exception e) { LogTextToFile("error", "[" + VTN.UCID + "] " + StringHelper.StripColors(_connections[VTN.UCID].PName) + "(" + _connections[VTN.UCID].UName + ") - VTN - Exception: " + e, false); }
        }

        // MCI - Multi Car Info
        private void MultiCarInfo(InSim insim, IS_MCI mci)
        {
            try
            {
                {
                    foreach (CompCar car in mci.Info)
                    {
                        Connections conn = GetConnection(car.PLID);
                        {
                            int Sped = Convert.ToInt32(MathHelper.SpeedToKph(car.Speed));
                            int kmh1 = car.Speed / 91;

                            decimal SpeedMS = (decimal)(((car.Speed / 32768f) * 100f) / 2);
                            decimal Speed = (decimal)((car.Speed * (100f / 32768f)) * 3.6f);

                            
                            int kmh = car.Speed / 91;
                            int mph = car.Speed / 146;
                            var X = car.X;
                            var Y = car.Y;
                            var Z = car.Z;
                            var angle = car.AngVel / 30;
                            string anglenew = "";

                            int Angle = AbsoluteAngleDifference(car.Direction, car.Heading);

                            if (kmh > 2)
                            {
                                _connections[conn.UCID].Angle = Angle;
                                
                            }
                            else
                            {
                                _connections[conn.UCID].Angle = 0;
                            }

                            

                            _players[car.PLID].kmh = kmh;
                            _players[car.PLID].mph = mph;

                            conn.TotalDistance += Convert.ToDecimal(SpeedMS);

                            #region ' Car Distances '
                            if (conn.CurrentCar == "XRG")
                            {
                                conn.kmXRG += Convert.ToDecimal(SpeedMS);

                            }

                            if (conn.CurrentCar == "LX4")
                            {
                                conn.kmLX4 += Convert.ToDecimal(SpeedMS);

                            }

                            if (conn.CurrentCar == "LX6")
                            {
                                conn.kmLX6 += Convert.ToDecimal(SpeedMS);

                            }


                            if (conn.CurrentCar == "RB4")
                            {
                                conn.kmRB4 += Convert.ToDecimal(SpeedMS);

                            }


                            if (conn.CurrentCar == "FXO")
                            {
                                conn.kmFXO += Convert.ToDecimal(SpeedMS);

                            }


                            if (conn.CurrentCar == "XRT")
                            {
                                conn.kmXRT += Convert.ToDecimal(SpeedMS);

                            }


                            if (conn.CurrentCar == "RAC")
                            {
                                conn.kmRAC += Convert.ToDecimal(SpeedMS);

                            }


                            if (conn.CurrentCar == "FZ5")
                            {
                                conn.kmFZ5 += Convert.ToDecimal(SpeedMS);

                            }

                            #endregion


                            if (AskedPosition == true && AskedPosUCID == conn.UCID)
                            {
                                // SendRCMToUsername(CurrentConnection.UName, "Your Position is: " + (car.X / 65535) + ", " + (car.Y / 65535), 5000);//keep the message for 5seconds(5000ms)
                                insim.Send(AskedPosUCID, "^3X: ^7" + (car.X / 65535) + " ^3Y: ^7" + (car.Y / 65535));
                                insim.Send(AskedPosUCID, "^3Speed: ^1" + Speed + " ^3Angle: ^1" + Angle);

                                AskedPosition = false;
                                AskedPosUCID = 255;
                            }

                            // if (GetDistXY(car.X, car.Y, -80, 1002) < 100)
                            /*
                            if (conn.OnTrack == true)
                            {
                                conn.InShopDist = (GetDistXY(car.X, car.Y, -80, 1002));
                                if (conn.InShopDist < 10)

                                {
                                    insim.Send(conn.UCID, "^8Welcome to the ^2" + "Mechanics");
                                }
                            }*/

                            /*string text = String.Format(
                            "^1X: {0:F2} ^2Y: {1:F2} ^3Z: {2:F2}",
                            car.X / 65536.0,
                            car.Y / 65536.0,
                            car.Z / 65536.0);

                            if (conn.UName == "kristofferandersen" || conn.UName == "bass-driver")
                            {
                                // mci
                                insim.Send(new IS_BTN
                                {
                                    Text = "^7" + text,
                                    UCID = conn.UCID,
                                    ReqI = 17,
                                    ClickID = 17,
                                    BStyle = ButtonStyles.ISB_DARK,
                                    H = 4,
                                    W = 30,
                                    T = 0,
                                    L = 137,
                                });
                            }*/

                            UpdateGui(conn.UCID, false, true);

                            anglenew = angle.ToString().Replace("-", "");
                        }
                    }
                }
            }
            catch (Exception e) { LogTextToFile("error", "MCI - Exception: " + e, false); }
        }

        void ClientRenames(InSim insim, IS_CPR CPR)
        {
            try
            {
                _connections[CPR.UCID].PName = CPR.PName;
                foreach (var CurrentPlayer in _players.Values) if (CurrentPlayer.UCID == CPR.UCID) CurrentPlayer.PName = CPR.PName;//make sure your code is AFTER this one
            }
            catch (Exception e) { LogTextToFile("error", "[" + CPR.UCID + "] " + StringHelper.StripColors(CPR.PName) + "(" + _connections[CPR.UCID].UName + ") - CPR - Exception: " + e, false); }
        }

        void OnStateChange(InSim insim, IS_STA STA)
        {
            try
            {
                if (TrackName != STA.Track)
                {
                    TrackName = STA.Track;
                    insim.Send(new IS_TINY { SubT = TinyType.TINY_AXI, ReqI = 255 });
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("" + e, "AN ERROR OCCURED");
                insim.Send(255, "^8An error occured: ^1{0}", e);
            }
        }

        // Join spectators (SHIFT + S)
        void PlayerLeave(InSim insim, IS_PLL PLL)
        {
            try
            {
                Connections conn = GetConnection(PLL.PLID);

                _players.Remove(PLL.PLID);//make sure your code is BEFORE this one
                conn.CurrentCar = "None";
                conn.OnTrack = false;
                conn.timesSpectated += 1;

                UpdateGui(conn.UCID, true, true, true);
            }
            catch (Exception e)
            {
                Connections conn = GetConnection(PLL.PLID);
                LogTextToFile("error", "[" + conn.UCID + "] " + StringHelper.StripColors(conn.PName) + "(" + conn.UName + ") - PLL - Exception: " + e, false);
            }
        }

        // Lap timings
        void LAPTime(InSim insim, IS_LAP LAP)
        {
            try
            {
                Connections conn = GetConnection(LAP.PLID);

                int score = conn.DP;

                insim.Send(255, conn.PName + " ^8drifted a lap and received ^2" + score + " points!");
                conn.Driftpoints += conn.DP;
                conn.DP = 0;

            }
            catch (Exception e)
            {
                Connections conn = GetConnection(LAP.PLID);
                LogTextToFile("error", "[" + conn.UCID + "] " + StringHelper.StripColors(conn.PName) + "(" + conn.UName + ") - PLL - Exception: " + e, false);
            }
        }

        // Car contact
        private void OnCarContact(InSim insim, IS_CON CON)
        {
            try
            {
                var connOne = GetConnection(CON.A.PLID);
                var connTwo = GetConnection(CON.B.PLID);

                // A - LEFT
                int headingOneLEFT = CON.A.Heading;
                int HeadingOneLEFT = (headingOneLEFT - 50);

                // B - LEFT
                int headingTwoLEFT = CON.B.Heading;
                int LEFT = (headingTwoLEFT - 50);

                // A - RIGHT
                int headingThreeRIGHT = CON.A.Heading;
                int HeadingThreeRIGHT = (headingThreeRIGHT + 50);

                // B - RIGHT
                int headingFourRIGHT = CON.B.Heading;
                int HeadingFourRIGHT = (headingFourRIGHT + 50);


                // if (CON.A.Speed >= 20 && CON.A.Speed <= 40)
                {
                    if (CON.A.Heading >= HeadingOneLEFT && CON.A.Heading <= headingThreeRIGHT && CON.B.Heading >= LEFT && CON.B.Heading <= HeadingFourRIGHT)
                    {

                        insim.Send(connOne.UCID, "^8Car contact with ^7" + connTwo.PName);
                        insim.Send(connTwo.UCID, "^8Car contact with ^7" + connOne.PName);

                        connOne.timesCrashed += 1;
                        connTwo.timesCrashed += 1;
                    }
                }


            }
            catch (Exception e)
            {
                MessageBox.Show("" + e.Message, "ERROR");

                // LogTextToFile("error", "[" + conntwo.UCID + "] " + StringHelper.StripColors(conntwo.PName) + "(" + conntwo.UName + ") - PLL - Exception: " + e, false);
            }
        }

        // Car reset
        private void CarReset(InSim insim, IS_CRS CRS)
        {
            try
            {
                Connections conn = GetConnection(CRS.PLID);

                conn.timesReset += 1;

            }
            catch (Exception e)
            {
                Connections conn = GetConnection(CRS.PLID);
                LogTextToFile("error", "[" + conn.UCID + "] " + StringHelper.StripColors(conn.PName) + "(" + conn.UName + ") - PLL - Exception: " + e, false);
            }
        }


        #region ' Functions '
        void UpdateGui(byte UCID, bool money, bool km, bool main = false)
        {
            const string TimeFormat = "HH:mm";//ex: 23:23 PM
            const string TimeFormatTwo = "dd/MM/yy";//ex: 23:23 PM

            if (km)
            {
                // DARK
                insim.Send(new IS_BTN
                {
                    UCID = UCID,
                    ReqI = 1,
                    ClickID = 1,
                    BStyle = ButtonStyles.ISB_DARK,
                    H = 5,
                    W = 73,
                    T = 0,
                    L = 63,
                });

                // Drift Points
                insim.Send(new IS_BTN
                {
                    Text = "^7Drift points:",
                    UCID = UCID,
                    ReqI = 2,
                    ClickID = 2,
                    BStyle = ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 12,
                    T = 0,
                    L = 64,
                });

                // Rank
                insim.Send(new IS_BTN
                {
                    Text = "^7Rank:",
                    UCID = UCID,
                    ReqI = 5,
                    ClickID = 5,
                    BStyle = ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 7,
                    T = 0,
                    L = 86,
                });

                // Rank value
                insim.Send(new IS_BTN
                {
                    Text = "^7" + _connections[UCID].Rank,
                    UCID = UCID,
                    ReqI = 6,
                    ClickID = 6,
                    BStyle = ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 16,
                    T = 0,
                    L = 91,
                });

                if (_connections[UCID].OnTrack == false)
                {
                    // DP value
                    insim.Send(new IS_BTN
                    {
                        Text = "^7" + _connections[_connections[UCID].UCID].Driftpoints + "p",
                        UCID = _connections[UCID].UCID,
                        ReqI = 3,
                        ClickID = 3,
                        BStyle = ButtonStyles.ISB_LEFT,
                        H = 4,
                        W = 10,
                        T = 0,
                        L = 75,
                    });

                    // Drift angle
                    insim.Send(new IS_BTN
                    {
                        Text = "^7" + _connections[UCID].Angle + "°",
                        UCID = _connections[UCID].UCID,
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
                        UCID = _connections[UCID].UCID,
                        ReqI = 10,
                        ClickID = 10,
                        BStyle = ButtonStyles.ISB_C2 | ButtonStyles.ISB_DARK,
                        H = 5,
                        W = 5,
                        T = 0,
                        L = 136,
                    });

                    // Drift angle
                    insim.Send(new IS_BTN
                    {
                        Text = "^2+" + _connections[UCID].DP + "p",
                        UCID = _connections[UCID].UCID,
                        ReqI = 11,
                        ClickID = 11,
                        BStyle = ButtonStyles.ISB_C2,
                        H = 4,
                        W = 20,
                        T = 0,
                        L = 108,
                    });
                }


                if (_connections[UCID].serverTime == true)
                {
                    insim.Send(new IS_BTN
                    {
                        Text = "^7" + DateTime.UtcNow.ToString(TimeFormat),
                        UCID = UCID,
                        ReqI = 46,
                        ClickID = 46,
                        BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_C4,
                        H = 4,
                        W = 27,
                        T = 76,
                        L = 91,
                    });
                }

                #region ' Datetime values '
                if (_connections[UCID].Timezone == -12)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-12).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(-12).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == -11)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-11).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(-11).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == -10)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-10).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(-10).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == -9)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-9).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(-9).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == -8)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-8).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(-8).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == -7)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-7).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(-7).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == -6)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-6).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(-6).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == -5)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-5).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(-5).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == -4)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-3).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(-4).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == -3)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-3).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(-3).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == -2)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-2).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(-2).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == -1)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(-1).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(-1).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == 0)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(0).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(0).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == 1)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(1).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(1).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == 2)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(2).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(2).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == 3)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(3).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(3).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == 4)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(4).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(4).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == 5)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(5).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(5).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == 6)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(6).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(6).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == 7)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(7).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(7).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == 8)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(8).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(8).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == 9)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(9).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(9).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == 10)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(10).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(10).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == 11)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(11).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(11).ToString(TimeFormatTwo);
                }
                else if (_connections[UCID].Timezone == 12)
                {
                    _connections[UCID].DateTime = DateTime.UtcNow.AddHours(12).ToString(TimeFormat);
                    _connections[UCID].Date = DateTime.UtcNow.AddHours(12).ToString(TimeFormatTwo);
                }
                #endregion


                // Clock
                insim.Send(new IS_BTN
                {
                    Text = "^7" + _connections[UCID].DateTime,
                    UCID = UCID,
                    ReqI = 4,
                    ClickID = 4,
                    BStyle = ButtonStyles.ISB_C4,
                    H = 4,
                    W = 7,
                    T = 0,
                    L = 128,
                });
            }


        }

        private void OnTinyReceived(InSim insim, IS_TINY TINY)
        {
            if (TINY.SubT == TinyType.TINY_AXC)
            {
                try
                {
                    if (LayoutName != "None")
                    {
                        insim.Send(255, "^8Layout removed");

                    }
                    else
                    {
                        LayoutName = "None";
                    }
                }
                catch (Exception EX) { LogTextToFile("packetError", "AXC - " + EX.Message); }
            }
        }

        void ClearPen(string Username) { insim.Send("/p_clear " + Username); }

        void KickID(string Username) { insim.Send("/kick " + Username); }

        private void btn(string text, byte height, byte width, byte top, byte length, ButtonStyles bstyle, byte clickid, byte ucid)
        {
            insim.Send(new IS_BTN
            {
                Text = text,
                UCID = ucid,
                ReqI = clickid,
                ClickID = clickid,
                BStyle = bstyle,
                H = height,
                W = width,
                T = top,
                L = length
            });
        }

        byte GetPlayer(byte UCID)
        {//Get Player from UCID
            byte PLID = 255;
            foreach (var CurrentPlayer in _players.Values) if (CurrentPlayer.UCID == UCID) PLID = CurrentPlayer.PLID;

            return PLID;
        }

        private Connections GetConnection(byte PLID)
        {//Get Connection from PLID
            Players NPL;
            if (_players.TryGetValue(PLID, out NPL)) return _connections[NPL.UCID];
            return null;
        }

        private bool IsConnAdmin(Connections Conn)
        {//general admin check, both Server and InSim
            if (Conn.IsAdmin == true || Conn.IsSuperAdmin == true) return true;
            return false;
        }

        private bool GetUserAdmin(string Username)
        {//reading admins.ini when connecting to server for InSim admin
            StreamReader CurrentFile = new StreamReader("files/admins.ini");

            string line = null;
            while ((line = CurrentFile.ReadLine()) != null)
            {
                if (line == Username)
                {
                    CurrentFile.Close();
                    return true;
                }
            }
            CurrentFile.Close();
            return false;
        }

        private void LogTextToFile(string file, string text, bool AdminMessage = true)
        {

            if (System.IO.File.Exists("files/" + file + ".log") == false) { FileStream CurrentFile = System.IO.File.Create("files/" + file + ".log"); CurrentFile.Close(); }

            StreamReader TextTempData = new StreamReader("files/" + file + ".log");
            string TempText = TextTempData.ReadToEnd();
            TextTempData.Close();

            StreamWriter TextData = new StreamWriter("files/" + file + ".log");
            TextData.WriteLine(TempText + DateTime.Now + ": " + text);
            TextData.Flush();
            TextData.Close();
        }

        #endregion

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (var cenn in _connections.Values)
            {
                if (ConnectedToSQL)
                {
                    try { SqlInfo.UpdateUser(_connections[cenn.UCID].UName, StringHelper.StripColors(SqlInfo.RemoveStupidCharacters(_connections[cenn.UCID].PName)), false, _connections[cenn.UCID].TotalDistance, _connections[cenn.UCID].Timezone, _connections[cenn.UCID].KMHorMPH, _connections[cenn.UCID].Rank, _connections[cenn.UCID].Driftpoints, _connections[cenn.UCID].timesCrashed, _connections[cenn.UCID].timesChatted, _connections[cenn.UCID].timesReset, _connections[cenn.UCID].timesJoined, _connections[cenn.UCID].timesSpectated, _connections[cenn.UCID].kmXRG, _connections[cenn.UCID].kmLX4, _connections[cenn.UCID].kmLX6, _connections[cenn.UCID].kmRB4, _connections[cenn.UCID].kmFXO, _connections[cenn.UCID].kmXRT, _connections[cenn.UCID].kmRAC, _connections[cenn.UCID].kmFZ5, _connections[cenn.UCID].totalplaytime); }
                    catch (Exception EX)
                    {
                        if (!SqlInfo.IsConnectionStillAlive())
                        {
                            ConnectedToSQL = false;
                            SQLReconnectTimer.Start();
                        }
                        LogTextToFile("sqlerror", "[" + cenn.UCID + "] " + StringHelper.StripColors(_connections[cenn.UCID].PName) + "(" + _connections[cenn.UCID].UName + ") CNL - Exception: " + EX.Message, false);
                    }
                }
            }
        }

        private void deleteBtn(byte ucid, byte reqi, bool sendbfn, byte clickid)
        {
            if (sendbfn == true)
            {
                IS_BFN bfn = new IS_BFN();
                bfn.ClickID = clickid;
                bfn.UCID = ucid;
                bfn.ReqI = reqi;

                insim.Send(bfn);
            }
        }

        private int AbsoluteAngleDifference(int d, int h)
        {
            d /= 180;
            h /= 180;
            int absdiff = Math.Abs(d - h);

            if (absdiff <= 180) return absdiff;

            if (d < 180)
            {
                h -= 360;
                return d - h;
            }
            else
            {
                d -= 360;
                return h - d;
            }
        }

        private void MessageToAdmins(string Message)
        {
            foreach (var conn in _connections.Values)
            {
                if (conn.IsAdmin == true)
                {
                    if (conn.UName != "")
                    {
                        insim.Send(conn.UCID, 0, "^3Admin notice: ^8" + Message);
                    }
                }
            }
        }

        public int GetDistXY(int X1, int Y1, int X2, int Y2)
        {
            //int X;			// X map (65536 = 1 metre)
            //int Y;			// Y map (65536 = 1 metre)
            //int Z;			// Z alt (65536 = 1 metre)
            return ((int)Math.Sqrt(Math.Pow(X1 - X2, 2) + Math.Pow(Y1 - Y2, 2)) / 65536);
        }

        public void GetRank(byte UCID)
        {
            if (_connections[UCID].Driftpoints < 25000)
            {
                _connections[UCID].Rank = "Newbie Drifter";
            }
            else if (_connections[UCID].Driftpoints < 50000)
            {
                _connections[UCID].Rank = "Novice Drifter";
            }
            else if (_connections[UCID].Driftpoints < 75000)
            {
                _connections[UCID].Rank = "Apprentice Drifter";
            }
            else if (_connections[UCID].Driftpoints < 100000)
            {
                _connections[UCID].Rank = "Apprentice (Grade 2)";
            }
            else if (_connections[UCID].Driftpoints < 150000)
            {
                _connections[UCID].Rank = "Casual Drifter";
            }
            else if (_connections[UCID].Driftpoints < 200000)
            {
                _connections[UCID].Rank = "Stanced Drifter";
            }
            else if (_connections[UCID].Driftpoints < 250000)
            {
                _connections[UCID].Rank = "Jeep Drifter";
            }
            else if (_connections[UCID].Driftpoints < 300000)
            {
                _connections[UCID].Rank = "Toyota Yaris Drifter";
            }
            else if (_connections[UCID].Driftpoints < 350000)
            {
                _connections[UCID].Rank = "Nissan Micra Drifter";
            }
            else if (_connections[UCID].Driftpoints < 400000)
            {
                _connections[UCID].Rank = "FWD Drifter";
            }
            else if (_connections[UCID].Driftpoints < 450000)
            {
                _connections[UCID].Rank = "Drunk Drifter";
            }
            else if (_connections[UCID].Driftpoints < 500000)
            {
                _connections[UCID].Rank = "NWD Drifter";
            }
        }

        private void chat_TextChanged(object sender, EventArgs e)
        {
            try
            {
                chat.SelectionStart = chat.Text.Length;
                chat.ScrollToCaret();
                chat.Refresh();
            }
            catch { }
        }

        private void chatbox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                if (chatbox.Text.Length != 0 && chatbox.Text.Length < 115)
                {
                    e.Handled = true;
                    insim.Send(255, "^7Host : ^2" + chatbox.Text);
                    chatbox.Text = "";
                }
                else
                {
                    e.Handled = true;
                    MessageBox.Show("Incorrect message length, max length is 115 characters. \r\n \r\nMessage have to be more than zero", "Error");
                }
            }
        }
    }
}
