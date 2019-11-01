using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Derp_InSim
{
    public static class Dealer
    {
        static public int GetPrice(string CarName)
        {
            switch (CarName.ToUpper())
            {
                case "XFG":
                    return 5000;

                case "XRG":
                    return 7500;

                case "LX4":
                    return 16400;

                case "LX6":
                    return 17000;

                case "RB4":
                    return 25000;

                case "FXO":
                    return 30000;

                case "XRT":
                    return 35000;

                case "RAC":
                    return 28000;

                case "FZ5":
                    return 38000;

                case "MRT":
                    return 14000;

                case "UFR":
                    return 112000;

                case "XFR":
                    return 115000;

                case "FXR":
                    return 190000;

                case "XRR":
                    return 190000;

                case "FZR":
                    return 190000;

                case "FBM":
                    return 160000;

                case "FOX":
                    return 210000;

                case "FO8":
                    return 1900000;

                case "BF1":
                    return 2750000;
            }
            return 0;
        }

        static public int GetValue(string CarName)
        {
            return (int)(GetPrice(CarName) * .25);
        }
    }
}
