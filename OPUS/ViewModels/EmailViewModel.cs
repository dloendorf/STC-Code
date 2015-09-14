using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OPUS.ViewModels
{
    public class EmailViewModel
    {
        public int Court { get; set; }
        public string Player1 { get; set; }
        public string Player2 { get; set; }
        public string Player3 { get; set; }
        public string Player4 { get; set; }

        public string GetPlayerName(string player)
        {
            //Remove the STC Rank from back of name for emailing and printing
            int iFLength = player.IndexOf(" ");
            string sFirst = player.Substring(0, iFLength);
            int iLength = player.IndexOf("(") - iFLength - 2;
            string sLast = player.Substring(iFLength + 1, iLength);
            return sFirst + " " + sLast;
        }

        public string Player1Name
        {
            get
            {
                return GetPlayerName(Player1);
            }
        }

        public string Player2Name
        {
            get
            {
                return GetPlayerName(Player2);
            }
        }

        public string Player3Name
        {
            get
            {
                return GetPlayerName(Player3);
            }
        }

        public string Player4Name
        {
            get
            {
                return GetPlayerName(Player4);
            }
        }
    }
}