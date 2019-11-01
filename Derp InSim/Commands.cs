using System;
using System.Windows.Forms;
using InSimDotNet;
using InSimDotNet.Packets;
using InSimDotNet.Helpers;

namespace Derp_InSim
{
    public partial class Form1
    {
        #region ' Voids '
        void CommandStats(byte UCID)
        {
            #region ' Command '
            var conn = _connections[UCID];

            conn.inStats = true;
            conn.DisplaysOpen = true;
            #region ' Main '
            // Close window
            insim.Send(new IS_BTN
            {
                Text = "^7Close",
                UCID = UCID,
                ReqI = 44,
                ClickID = 44,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                H = 4,
                W = 7,
                T = 44,
                L = 119,
            });

            // DARK
            insim.Send(new IS_BTN
            {
                UCID = conn.UCID,
                ReqI = 30,
                ClickID = 30,
                BStyle = ButtonStyles.ISB_DARK,
                H = 88,
                W = 56,
                T = 40,
                L = 71,
            });

            // Server info label
            insim.Send(new IS_BTN
            {
                Text = "^2-- Your Statistics --",
                UCID = conn.UCID,
                ReqI = 31,
                ClickID = 31,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_C4,
                H = 7,
                W = 45,
                T = 43,
                L = 73,
            });
            #endregion

            #region ' Rows '
            insim.Send(new IS_BTN
            {
                Text = "^3User:",
                UCID = UCID,
                ReqI = 32,
                ClickID = 32,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 51,
                L = 73,
            });

            // User
            insim.Send(new IS_BTN
            {
                Text = "^7" + conn.PName + " ^7(" + conn.UName + ")",
                UCID = UCID,
                ReqI = 33,
                ClickID = 33,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 51,
                L = 91,
            });

            // Total Distance
            insim.Send(new IS_BTN
            {
                Text = "^3Total Distance:",
                UCID = UCID,
                ReqI = 34,
                ClickID = 34,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 55,
                L = 73,
            });
            insim.Send(new IS_BTN
            {
                Text = "^7" + string.Format("{0:0.0}", conn.TotalDistance / 1000) + " km / " + string.Format("{0:0.0}", conn.TotalDistance / 1649) + " mi",
                UCID = UCID,
                ReqI = 35,
                ClickID = 35,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 55,
                L = 91,
            });


            insim.Send(new IS_BTN
            {
                Text = "^3Total Drift Points:",
                UCID = UCID,
                ReqI = 36,
                ClickID = 36,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 59,
                L = 73,
            });

            // Drift Points
            insim.Send(new IS_BTN
            {
                Text = "^2" + conn.Driftpoints + " points",
                UCID = UCID,
                ReqI = 37,
                ClickID = 37,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 59,
                L = 91,
            });

            insim.Send(new IS_BTN
            {
                Text = "^3Current Rank:",
                UCID = UCID,
                ReqI = 38,
                ClickID = 38,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 63,
                L = 73,
            });

            // Rank
            insim.Send(new IS_BTN
            {
                Text = "^7" + conn.Rank,
                UCID = UCID,
                ReqI = 39,
                ClickID = 39,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 63,
                L = 91,
            });

            insim.Send(new IS_BTN
            {
                Text = "^3Crashes:",
                UCID = UCID,
                ReqI = 40,
                ClickID = 40,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 67,
                L = 73,
            });

            // crashes
            insim.Send(new IS_BTN
            {
                Text = "^7" + conn.timesCrashed,
                UCID = UCID,
                ReqI = 41,
                ClickID = 41,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 67,
                L = 91,
            });

            insim.Send(new IS_BTN
            {
                Text = "^3Car Resets:",
                UCID = UCID,
                ReqI = 42,
                ClickID = 42,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 71,
                L = 73,
            });

            // resets
            insim.Send(new IS_BTN
            {
                Text = "^7" + conn.timesReset,
                UCID = UCID,
                ReqI = 43,
                ClickID = 43,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 71,
                L = 91,
            });

            insim.Send(new IS_BTN
            {
                Text = "^3Times Joined:",
                UCID = UCID,
                ReqI = 45,
                ClickID = 45,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 75,
                L = 73,
            });

            // resets
            insim.Send(new IS_BTN
            {
                Text = "^7" + conn.timesJoined,
                UCID = UCID,
                ReqI = 46,
                ClickID = 46,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 75,
                L = 91,
            });

            insim.Send(new IS_BTN
            {
                Text = "^3Times Spectated:",
                UCID = UCID,
                ReqI = 47,
                ClickID = 47,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 79,
                L = 73,
            });

            // resets
            insim.Send(new IS_BTN
            {
                Text = "^7" + conn.timesSpectated,
                UCID = UCID,
                ReqI = 48,
                ClickID = 48,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 79,
                L = 91,
            });

            #endregion

            #region ' Cars '

            insim.Send(new IS_BTN
            {
                Text = "^3XR GT (XRG):",
                UCID = UCID,
                ReqI = 49,
                ClickID = 49,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 84,
                L = 73,
            });

            // xrg
            insim.Send(new IS_BTN
            {
                Text = "^7" + string.Format("{0:0.0}", conn.kmXRG / 1000) + " km / " + string.Format("{0:0.0}", conn.kmXRG / 1649) + " mi",
                UCID = UCID,
                ReqI = 50,
                ClickID = 50,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 84,
                L = 91,
            });

            insim.Send(new IS_BTN
            {
                Text = "^3LX4 (LX4):",
                UCID = UCID,
                ReqI = 51,
                ClickID = 51,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 88,
                L = 73,
            });

            // lx4
            insim.Send(new IS_BTN
            {
                Text = "^7" + string.Format("{0:0.0}", conn.kmLX4 / 1000) + " km / " + string.Format("{0:0.0}", conn.kmLX4 / 1649) + " mi",
                UCID = UCID,
                ReqI = 52,
                ClickID = 52,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 88,
                L = 91,
            });

            insim.Send(new IS_BTN
            {
                Text = "^3LX6 (LX6):",
                UCID = UCID,
                ReqI = 53,
                ClickID = 53,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 92,
                L = 73,
            });

            // lx6
            insim.Send(new IS_BTN
            {
                Text = "^7" + string.Format("{0:0.0}", conn.kmLX6 / 1000) + " km / " + string.Format("{0:0.0}", conn.kmLX6 / 1649) + " mi",
                UCID = UCID,
                ReqI = 54,
                ClickID = 54,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 92,
                L = 91,
            });

            insim.Send(new IS_BTN
            {
                Text = "^3RB4 GT (RB4):",
                UCID = UCID,
                ReqI = 55,
                ClickID = 55,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 96,
                L = 73,
            });

            // rb4
            insim.Send(new IS_BTN
            {
                Text = "^7" + string.Format("{0:0.0}", conn.kmRB4 / 1000) + " km / " + string.Format("{0:0.0}", conn.kmRB4 / 1649) + " mi",
                UCID = UCID,
                ReqI = 56,
                ClickID = 56,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 96,
                L = 91,
            });

            insim.Send(new IS_BTN
            {
                Text = "^3FXO TURBO (FXO):",
                UCID = UCID,
                ReqI = 57,
                ClickID = 57,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 100,
                L = 73,
            });

            // fxo
            insim.Send(new IS_BTN
            {
                Text = "^7" + string.Format("{0:0.0}", conn.kmFXO / 1000) + " km / " + string.Format("{0:0.0}", conn.kmFXO / 1649) + " mi",
                UCID = UCID,
                ReqI = 58,
                ClickID = 58,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 100,
                L = 91,
            });

            insim.Send(new IS_BTN
            {
                Text = "^3XRT TURBO (XRT):",
                UCID = UCID,
                ReqI = 59,
                ClickID = 59,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 104,
                L = 73,
            });

            // xrt
            insim.Send(new IS_BTN
            {
                Text = "^7" + string.Format("{0:0.0}", _connections[conn.UCID].kmXRT / 1000) + " km / " + string.Format("{0:0.0}", conn.kmXRT / 1649) + " mi",
                UCID = UCID,
                ReqI = 60,
                ClickID = 60,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 104,
                L = 91,
            });

            insim.Send(new IS_BTN
            {
                Text = "^3RACEABOUT (RAC):",
                UCID = UCID,
                ReqI = 61,
                ClickID = 61,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 108,
                L = 73,
            });

            // RAC
            insim.Send(new IS_BTN
            {
                Text = "^7" + string.Format("{0:0.0}", conn.kmRAC / 1000) + " km / " + string.Format("{0:0.0}", conn.kmRAC / 1649) + " mi",
                UCID = UCID,
                ReqI = 62,
                ClickID = 62,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 108,
                L = 91,
            });

            insim.Send(new IS_BTN
            {
                Text = "^3FZ50 (FZ5):",
                UCID = UCID,
                ReqI = 63,
                ClickID = 63,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 112,
                L = 73,
            });

            // FZ5
            insim.Send(new IS_BTN
            {
                Text = "^7" + string.Format("{0:0.0}", conn.kmFZ5 / 1000) + " km / " + string.Format("{0:0.0}", conn.kmFZ5 / 1649) + " mi",
                UCID = UCID,
                ReqI = 64,
                ClickID = 64,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 112,
                L = 91,
            });

            #endregion

            #region ' Registration Date '

            insim.Send(new IS_BTN
            {
                Text = "^3Registration Date:",
                UCID = UCID,
                ReqI = 65,
                ClickID = 65,
                BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                H = 4,
                W = 18,
                T = 117,
                L = 73,
            });

            // FZ5
            insim.Send(new IS_BTN
            {
                Text = "^7" + conn.regdate,
                UCID = UCID,
                ReqI = 66,
                ClickID = 66,
                BStyle = ButtonStyles.ISB_LIGHT,
                H = 4,
                W = 27,
                T = 117,
                L = 91,
            });

            #endregion

            #region ' text '
            insim.Send(new IS_BTN
            {
                Text = "^7Check out ^2" + website + " ^7for Global Statistics",
                UCID = UCID,
                ReqI = 67,
                ClickID = 67,
                BStyle = ButtonStyles.ISB_C4,
                H = 3,
                W = 45,
                T = 122,
                L = 73,
            });
            #endregion
            #endregion
        }

        void CommandInfo(byte UCID)
        {
            #region ' Command '
            const string TimeFormat = "HH:mm";//ex: 23/03/2003


            #region ' info '
            if (_connections[UCID].DisplaysOpen == false && _connections[UCID].inInfo == false)
            {
                _connections[UCID].serverTime = true;

                // DARK
                insim.Send(new IS_BTN
                {
                    UCID = UCID,
                    ReqI = 30,
                    ClickID = 30,
                    BStyle = ButtonStyles.ISB_DARK,
                    H = 44,
                    W = 56,
                    T = 40,
                    L = 71,
                });

                // Server info label
                insim.Send(new IS_BTN
                {
                    Text = "^2-- Server Information --",
                    UCID = UCID,
                    ReqI = 31,
                    ClickID = 31,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_C4,
                    H = 7,
                    W = 45,
                    T = 43,
                    L = 73,
                });

                // track1
                insim.Send(new IS_BTN
                {
                    Text = "^3Track:",
                    UCID = UCID,
                    ReqI = 32,
                    ClickID = 32,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 18,
                    T = 51,
                    L = 73,
                });
                insim.Send(new IS_BTN
                {
                    Text = "^7" + TrackHelper.GetFullTrackName(TrackName) + " ^7(" + TrackName + ")",
                    UCID = UCID,
                    ReqI = 33,
                    ClickID = 33,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_C4,
                    H = 4,
                    W = 27,
                    T = 51,
                    L = 91,
                });


                // players connected
                insim.Send(new IS_BTN
                {
                    Text = "^3Players connected:",
                    UCID = UCID,
                    ReqI = 34,
                    ClickID = 34,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 18,
                    T = 55,
                    L = 73,
                });

                int conns = (_connections.Count - 1);
                insim.Send(new IS_BTN
                {
                    Text = "^7" + conns,
                    UCID = UCID,
                    ReqI = 35,
                    ClickID = 35,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_C4,
                    H = 4,
                    W = 27,
                    T = 55,
                    L = 91,
                });

                // players spectating
                insim.Send(new IS_BTN
                {
                    Text = "^3Players spectating:",
                    UCID = UCID,
                    ReqI = 36,
                    ClickID = 36,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 18,
                    T = 59,
                    L = 73,
                });
                insim.Send(new IS_BTN
                {
                    Text = "^7" + (conns - _players.Count),
                    UCID = UCID,
                    ReqI = 37,
                    ClickID = 37,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_C4,
                    H = 4,
                    W = 27,
                    T = 59,
                    L = 91,
                });

                // players on the track
                insim.Send(new IS_BTN
                {
                    Text = "^3Players on track:",
                    UCID = UCID,
                    ReqI = 38,
                    ClickID = 38,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 18,
                    T = 63,
                    L = 73,
                });
                insim.Send(new IS_BTN
                {
                    Text = "^7" + _players.Count,
                    UCID = UCID,
                    ReqI = 39,
                    ClickID = 39,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_C4,
                    H = 4,
                    W = 27,
                    T = 63,
                    L = 91,
                });

                // users registered
                insim.Send(new IS_BTN
                {
                    Text = "^3Registered users:",
                    UCID = UCID,
                    ReqI = 40,
                    ClickID = 40,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 18,
                    T = 67,
                    L = 73,
                });
                insim.Send(new IS_BTN
                {
                    Text = "^7" + dbCount.ToString(),
                    UCID = UCID,
                    ReqI = 41,
                    ClickID = 41,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_C4,
                    H = 4,
                    W = 27,
                    T = 67,
                    L = 91,
                });

                // banned users
                insim.Send(new IS_BTN
                {
                    Text = "^3Permabanned users:",
                    UCID = UCID,
                    ReqI = 42,
                    ClickID = 42,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 18,
                    T = 71,
                    L = 73,
                });
                insim.Send(new IS_BTN
                {
                    Text = "^7" + dbBans.ToString(),
                    UCID = UCID,
                    ReqI = 43,
                    ClickID = 43,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_C4,
                    H = 4,
                    W = 27,
                    T = 71,
                    L = 91,
                });

                // banned users
                insim.Send(new IS_BTN
                {
                    Text = "^3Server time:",
                    UCID = UCID,
                    ReqI = 45,
                    ClickID = 45,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_LEFT,
                    H = 4,
                    W = 18,
                    T = 76,
                    L = 73,
                });
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

                // Close window
                insim.Send(new IS_BTN
                {
                    Text = "^7Close",
                    UCID = UCID,
                    ReqI = 44,
                    ClickID = 44,
                    BStyle = ButtonStyles.ISB_LIGHT | ButtonStyles.ISB_CLICK,
                    H = 4,
                    W = 7,
                    T = 44,
                    L = 119,
                });

                _connections[UCID].DisplaysOpen = true;
                _connections[UCID].inInfo = true;
            }
            else
            {
                insim.Send(255, DisplaysOpenedMsg);
            }
            #endregion


            #endregion
        }
        #endregion

        private void MessageReceived(InSim insim, IS_MSO mso)
        {
            try
            {
                {

                    if (mso.UserType == UserType.MSO_PREFIX)
                    {
                        string Text = mso.Msg.Substring(mso.TextStart, (mso.Msg.Length - mso.TextStart));

                        string[] command = Text.Split(' ');
                        command[0] = command[0].ToLower();
                        var conn = _connections[mso.UCID];

                        switch (command[0])
                        {
                            case "!find":

                                insim.Send(mso.UCID, "^8Been online for ^3" + conn.totalplaytime + " seconds");

                                break;

                            case "!info":

                                CommandInfo(mso.UCID);

                                break;

                            case "!stats":

                                if (conn.DisplaysOpen == false && conn.inStats == false)
                                {
                                    CommandStats(mso.UCID);
                                }

                                break;


                            case "!ac":
                                {//Admin chat
                                    if (mso.UCID == _connections[mso.UCID].UCID)
                                    {
                                        if (!IsConnAdmin(_connections[mso.UCID]))
                                        {
                                            insim.Send(mso.UCID, 0, "You are not an admin");
                                            break;
                                        }
                                        if (command.Length == 1)
                                        {
                                            insim.Send(mso.UCID, 0, "^1Invalid command format. ^2Usage: ^7!ac <text>");
                                            break;
                                        }

                                        string atext = Text.Remove(0, command[0].Length + 1);

                                        foreach (var Conn in _connections.Values)
                                        {
                                            {
                                                if (IsConnAdmin(Conn) && Conn.UName != "")
                                                {
                                                    insim.Send(Conn.UCID, 0, "^3Admin Chat ^7" + _connections[mso.UCID].PName + " ^8(" + _connections[mso.UCID].UName + "):");
                                                    insim.Send(Conn.UCID, 0, "^7" + atext);
                                                }
                                            }
                                        }
                                    }

                                    break;
                                }

                            case "!pos":

                                if (_connections[mso.UCID].IsAdmin == false)
                                {
                                    insim.Send(mso.UCID, "^1Error: ^7You are not an Admin!");
                                    break;
                                }
                                if (AskedPosition == true)
                                {
                                    insim.Send(mso.UCID, "^1Error: ^7Someone else already wants their position, please try again.");
                                    break;
                                }

                                //position @ MCI Packet
                                AskedPosUCID = mso.UCID;
                                AskedPosition = true;
                                break;

                            case "!teamspeak":
                            case "!ts":

                                insim.Send(mso.UCID, "^8Teamspeak 3 Server: ^3" + "ts.eugaming.org");

                                break;

                            case "!website":
                            case "!web":

                                insim.Send(mso.UCID, "^8Our website: ^3" + "www.eugaming.org");
                                insim.Send(mso.UCID, "^8Global statistics can be found on the website too.");

                                break;

                            case "!p":
                            case "!pen":
                            case "!penalty":

                                insim.Send("/p_clear " + _connections[mso.UCID].UName);

                                break;

                            case "!ban":
                                insim.Send(mso.UCID, "^8This command is inactive");
                                SqlInfo.AddtoBanlist(k.UName, StringHelper.StripColors(k.PName), "14.01.2016", "no longer welcome");
                                break;

                            case "!help":
                                insim.Send(mso.UCID, 0, "^3Commands:");
                                insim.Send(mso.UCID, 0, "^7!help ^8- See a list of available commands");
                                insim.Send(mso.UCID, 0, "^7!info ^8- See a few lines of server info");
                                insim.Send(mso.UCID, 0, "^7!stats ^8- Check your personal stats");
                                insim.Send(mso.UCID, 0, "^7!pen (!p) ^8- Remove the pit penalty");
                                insim.Send(mso.UCID, 0, "^7!gmt <timezone> ^8- Set your own timezone, ex: !gmt +12");
                                insim.Send(mso.UCID, 0, "^7!teamspeak (!ts) ^8- To view our teamspeak address");
                                insim.Send(mso.UCID, 0, "^7!website (!web) ^8- To view our website address");

                                // Admin commands
                                foreach (var CurrentConnection in _connections.Values)
                                {
                                    if (CurrentConnection.UCID == mso.UCID)
                                    {
                                        if (IsConnAdmin(CurrentConnection) && CurrentConnection.UName != "")
                                        {
                                            insim.Send(CurrentConnection.UCID, 0, "^3Administrator commands:");
                                            insim.Send(CurrentConnection.UCID, 0, "^7!ac ^8- Talk with the other admins that are online");
                                            insim.Send(CurrentConnection.UCID, 0, "^7!pos ^8- Check your current position in x, y, z");
                                            insim.Send(CurrentConnection.UCID, 0, "^7!ban <username> <days> ^8- Ban the selected player for x days (0 = 12 hours)");
                                        }
                                    }
                                }

                                break;

                            case "!gmt":

                                var connn = _connections[mso.UCID];
                                if (command.Length == 1)
                                {
                                    insim.Send(mso.UCID, "^1Invalid command. ^7Usage: ^3!gmt <timezone>");
                                }
                                else if (command.Length == 2)
                                {
                                    #region ' Command '
                                    if (command[1] == "-12" || command[1] == "-12:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -12;
                                    }
                                    else if (command[1] == "-11" || command[1] == "-11:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -11;
                                    }
                                    else if (command[1] == "-10" || command[1] == "-10:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -10;
                                    }
                                    else if (command[1] == "-9" || command[1] == "-9:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -9;
                                    }
                                    else if (command[1] == "-8" || command[1] == "-8:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -8;
                                    }
                                    else if (command[1] == "-7" || command[1] == "-7:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -7;
                                    }
                                    else if (command[1] == "-6" || command[1] == "-6:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -6;
                                    }
                                    else if (command[1] == "-5" || command[1] == "-5:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -5;
                                    }
                                    else if (command[1] == "-4" || command[1] == "-4:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -4;
                                    }
                                    else if (command[1] == "-3" || command[1] == "-3:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -3;
                                    }
                                    else if (command[1] == "-2" || command[1] == "-2:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -2;
                                    }
                                    else if (command[1] == "-1" || command[1] == "-1:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = -1;
                                    }
                                    else if (command[1] == "0" || command[1] == "0:00" || command[1] == "+0" || command[1] == "+0:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "default (GMT 0:00)");
                                        connn.Timezone = 0;
                                    }
                                    else if (command[1] == "1" || command[1] == "1:00" || command[1] == "+1" || command[1] == "+1:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 1;
                                    }
                                    else if (command[1] == "2" || command[1] == "2:00" || command[1] == "+2" || command[1] == "+2:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 2;
                                    }
                                    else if (command[1] == "3" || command[1] == "3:00" || command[1] == "+3" || command[1] == "+3:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 3;
                                    }
                                    else if (command[1] == "4" || command[1] == "4:00" || command[1] == "+4" || command[1] == "+4:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 4;
                                    }
                                    else if (command[1] == "5" || command[1] == "5:00" || command[1] == "+5" || command[1] == "+5:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 5;
                                    }
                                    else if (command[1] == "6" || command[1] == "6:00" || command[1] == "+6" || command[1] == "+6:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 6;
                                    }
                                    else if (command[1] == "7" || command[1] == "7:00" || command[1] == "+7" || command[1] == "+7:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 7;
                                    }
                                    else if (command[1] == "8" || command[1] == "8:00" || command[1] == "+8" || command[1] == "+8:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 8;
                                    }
                                    else if (command[1] == "9" || command[1] == "9:00" || command[1] == "+9" || command[1] == "+9:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 9;
                                    }
                                    else if (command[1] == "10" || command[1] == "10:00" || command[1] == "+10" || command[1] == "+10:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 10;
                                    }
                                    else if (command[1] == "11" || command[1] == "11:00" || command[1] == "+11" || command[1] == "+11:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 11;
                                    }
                                    else if (command[1] == "12" || command[1] == "12:00" || command[1] == "+12" || command[1] == "+12:00")
                                    {
                                        insim.Send(mso.UCID, "^8Your timezone has been set to ^3" + "GMT " + command[1]);
                                        connn.Timezone = 12;
                                    }
                                    #endregion

                                }
                                break;

                            default:
                                insim.Send(mso.UCID, 0, "^8Invalid command, type {0} to see available commands", "^2!help^8");
                                break;
                        }
                    }
                    else
                    {
                        chat.Text += SqlInfo.RemoveStupidCharacters(StringHelper.StripColors(mso.Msg + " \r\n"));
                    }
                }
            }
            catch (Exception e) { LogTextToFile("commands", "[" + mso.UCID + "] " + StringHelper.StripColors(_connections[mso.UCID].PName) + "(" + GetConnection(mso.PLID).UName + ") NPL - Exception: " + e, false); }
        }
    }

}
