using System;
using InSimDotNet.Helpers;
using InSimDotNet.Packets;

namespace Derp_InSim
{
    public partial class Form1
    {
        private void BTC_ClientClickedButton(IS_BTC BTC)
        {
            var conn = _connections[BTC.UCID];

            try
            {
                // DELETE a buton:

                // deleteBtn(Ucid, Reqi, true, ClickID);

                // deleteBtn(BTC.UCID, BTC.ReqI, true, 6);
                {

                    switch (BTC.ClickID)
                    {

                        case 44:

                            if (conn.DisplaysOpen == true && conn.inInfo == true)
                            {
                                conn.serverTime = false;
                                conn.DisplaysOpen = false;
                                conn.inInfo = false;

                                deleteBtn(BTC.UCID, BTC.ReqI, true, 30);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 31);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 32);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 33);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 34);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 35);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 36);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 37);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 38);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 39);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 40);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 41);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 42);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 43);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 44);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 45);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 46);
                            }

                            if (conn.DisplaysOpen == true && conn.inStats == true)
                            {
                                conn.DisplaysOpen = false;
                                conn.inStats = false;

                                deleteBtn(BTC.UCID, BTC.ReqI, true, 30);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 31);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 32);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 33);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 34);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 35);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 36);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 37);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 38);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 39);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 40);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 41);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 42);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 43);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 44);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 45);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 46);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 47);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 48);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 49);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 50);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 51);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 52);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 53);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 54);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 55);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 56);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 57);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 58);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 59);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 60);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 61);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 62);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 63);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 64);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 65);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 66);
                                deleteBtn(BTC.UCID, BTC.ReqI, true, 67);

                            }



                            break;
                    }
                }
            }
            catch (Exception e) { LogTextToFile("error", "[" + BTC.UCID + "] " + StringHelper.StripColors(_connections[BTC.UCID].PName) + "(" + _connections[BTC.UCID].UName + ") - BTC - Exception: " + e, false); }
        }
    }
}
