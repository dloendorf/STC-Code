using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web.Mvc;
using OPUS.DAL;
using OPUS.Models;

namespace OPUS
{
    sealed public class Util : IDisposable
    {
        private OpusContext db = new OpusContext();
        private OpusContext db1 = new OpusContext();

        public void LoadMemberList(string[] sMembers)
        {
            OpusPlayer player;
            string s = sMembers[0];
            //string[] members = s.Substring(s.IndexOf(@"\r\n")).Split(new string[] { @"\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            string[] members = s.Split('\n');
            for (int i = 0; i < members.Length; i += 1)
                members[i] = members[i].Trim(new Char[] { '\r', ' ' });
            foreach (var member in members)
            {
                if(member.Length > 1)
                {
                    int iNameEnd = 0;
                    int iFirstSpace = member.IndexOf(' ');
                    int iSecondSpace = member.IndexOf(' ', iFirstSpace+1);
                    int iFirstLeftParen = member.IndexOf('(');
                    if(iFirstLeftParen == -1)
                        iNameEnd = iSecondSpace;
                    else if (iFirstSpace > iFirstLeftParen)
                        iNameEnd = iFirstLeftParen;
                    else
                        iNameEnd = iSecondSpace;
                    int iLengthMember = member.Length;
                    int iLastSpace = member.LastIndexOf(' ');
                    int iLastRightParen = member.LastIndexOf(')');
                    string name = member.Substring(0, iNameEnd);
                    string rating = member.Substring(iLastSpace + 1, iLengthMember - iLastSpace-1);
                    string code = "";
                    if (iFirstLeftParen != -1)
                    {
                        string codes = member.Substring(iFirstLeftParen, iLastRightParen - iFirstLeftParen + 1);
                        if (codes.Contains("N")) code = "N";
                        if (codes.Contains("O")) if (code.Length > 0) code = code + ",O"; else code = "O";
                        if (codes.Contains("P")) if (code.Length > 0) code = code + ",P"; else code = "P";
                        if (codes.Contains("S")) if (code.Length > 0) code = code + ",S"; else code = "S";
                    }
                    player = new OpusPlayer();

                    // Need to handle the one case where codes come between first and last names
                    if(iFirstLeftParen != -1 && iFirstLeftParen < iFirstSpace)
                    {
                        player.First = name.Substring(0, iFirstLeftParen);
                        player.Last = member.Substring(iFirstSpace + 1, iSecondSpace - iFirstSpace - 1);
                    }
                    else
                    {
                        player.First = name.Substring(0, iFirstSpace);
                        player.Last = name.Substring(iFirstSpace + 1, iSecondSpace - iFirstSpace - 1);
                    }
                    player.PlayCodes = code;
                    player.OverallPercentWon = 0;
                    player.Rank = 100;
                    player.Factor = 0;
                    player.STCRank = rating;
                    player.Name = player.First + " " + player.Last; ;
                    FirstNames group = db1.FirstNames.Single(q => q.FirstName.Equals(player.First.ToUpper()));
                    if(group.Code.Equals("FO"))
                        player.Group = "F";
                    else if (group.Code.Equals("MO"))
                        player.Group = "M";
                    else
                        player.Group = "";
                    db.OpusPlayers.Add(player);
                }
            }
            db.SaveChanges();
        }

        public List<PlayerList> GetPlayers(string Group, string playCode)
        {
            List<PlayerList> players = new List<PlayerList>();
            var opusPlayers = db.OpusPlayers;
            string Group2 = "";
            if (playCode == "S") { Group = "F"; Group2 = "M"; }
            var result = (from m in opusPlayers where (m.Group.Equals(Group) | m.Group.Equals(Group2)) & m.PlayCodes.Contains(playCode) select m);
            foreach (var item in result)
            {
                players.Add(new PlayerList { ID = item.PlayerID, Name = item.NameRank });
            }
            return players;
        }

        public SelectList GetPlayersSelectList(string Group, string playCode)
        {
            List<PlayerList> vPlayers = GetRankedPlayers(Group, playCode);
            return new SelectList((vPlayers).Select(d => new SelectListItem { Text = d.Name, Value = d.ID.ToString() }), "Value", "Text");
        }

        public List<OPUSPlayerInfoList> GetOpusPlayerInfo(string Group, string playCode)
        {
            List<OPUSPlayerInfoList> players = new List<OPUSPlayerInfoList>();
            var opusPlayers = db.OpusPlayers;
            string Group2 = "";
            if (playCode == "S") { Group = "F"; Group2 = "M"; }
            var result = (from m in opusPlayers where (m.Group.Equals(Group) | m.Group.Equals(Group2)) & m.PlayCodes.Contains(playCode) select m);
            foreach (var item in result)
            {
                players.Add(new OPUSPlayerInfoList { ID = item.PlayerID, Name = item.NameRank, OverallPercentWon = item.OverallPercentWon, Rank = item.Rank });
            }
            return players;
        }

        public List<PlayerList> GetRankedPlayers(string Group, string playCode)
        {
            List<PlayerList> players = new List<PlayerList>();
            var opusplayers = db.OpusPlayers;
            string Group2 = "";
            if (playCode == "S") { Group = "F"; Group2 = "M"; }
            var result = from p in opusplayers where (p.Group.Equals(Group) | p.Group.Equals(Group2)) & p.PlayCodes.Contains(playCode) orderby p.Rank select p;
            foreach (var item in result)
            {
                players.Add(new PlayerList { ID = item.PlayerID, Name = item.NameRank });
            }
            return players;
        }

        public List<SelectListItem> GetSeasons(string Group, string playcode)
        {
            // Get unique seasons OPUS Played
            List<SelectListItem> items = new List<SelectListItem>();
            var seasons = db.PastOpusPlayers;
            string Group2 = "";
            if (playcode == "S") { Group = "F"; Group2 = "M"; }
            var result = (from m in seasons where (m.Group.Equals(Group) | m.Group.Equals(Group2)) & m.PlayCodes.Equals(playcode) select m.Season).Distinct().ToList();
            foreach (var item in result)
            {
                items.Add(new SelectListItem { Text = item, Value = item });
            }
            return items;
        }

        public List<SelectListItem> GetCourtDates(string season, string Group, string playcode)
        {
            // Get unique dates OPUS Played
            List<SelectListItem> items = new List<SelectListItem>();
            var dates = db.PastCourtAssignments;
            List<string> result = null;
            if (season == null)
                result = (from m in dates where m.Group.Equals(Group) & m.PlayCode.Equals(playcode) select m.Date).Distinct().ToList();
            else
                result = (from m in dates where m.Season == season && m.Group.Equals(Group) & m.PlayCode.Equals(playcode) select m.Date).Distinct().ToList();
            foreach (var item in result)
            {
                items.Add(new SelectListItem { Text = item, Value = item });
            }
            return items;
        }

        public List<SelectListItem> GetCourtDates(string Group)
        {
            // Get unique dates OPUS Played
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "New Date", Value = "NewDate" });
            var dates = db.Assignments;
            var result = (from m in dates where m.Group.Equals(Group) select m.Date).Distinct().ToList();
            foreach (var item in result)
            {
                items.Add(new SelectListItem { Text = item, Value = item });
            }
            return items;
        }

        public List<SelectListItem> GetCourtDates(string Group, string playcode)
        {
            // Get unique dates OPUS Played
            List<SelectListItem> items = new List<SelectListItem>();
            var dates = db.PastCourtAssignments;
            var result = (from m in dates where m.Group.Equals(Group) & m.PlayCode.Equals(playcode) select m.Date).Distinct().ToList();
            foreach (var item in result)
            {
                items.Add(new SelectListItem { Text = item, Value = item });
            }
            return items;
        }

        public void AddCourtToScoring(OpusContext db1, OpusContext db3, string date, CourtAssignment item)
        {
            //Load a record into Scores table for each player (4 per item found)
            //The data being used comes from the CourtAssignment data which keeps player namses as First, Last (STC Rank)
            //The code below just grabs First and Last for use in the Scoring table.
            int id = Convert.ToInt32(item.Player1ID);
            OpusPlayer player = db3.OpusPlayers.Single(q => q.PlayerID == id);
            Scoring sRow = new Scoring
            {
                Date = date,
                First = player.First,
                Last = player.Last,
                Group = player.Group,
                STCPlayerID = player.PlayerID,
                Name = item.Player1
            };
            db1.Scores.Add(sRow);
            id = Convert.ToInt32(item.Player2ID);
            player = db3.OpusPlayers.Single(q => q.PlayerID == id);
            sRow = new Scoring
            {
                Date = date,
                First = player.First,
                Last = player.Last,
                Group = player.Group,
                STCPlayerID = player.PlayerID,
                Name = item.Player2
            };
            db1.Scores.Add(sRow);
            id = Convert.ToInt32(item.Player3ID);
            player = db3.OpusPlayers.Single(q => q.PlayerID == id);
            sRow = new Scoring
            {
                Date = date,
                First = player.First,
                Last = player.Last,
                Group = player.Group,
                STCPlayerID = player.PlayerID,
                Name = item.Player3
            };
            db1.Scores.Add(sRow);
            id = Convert.ToInt32(item.Player4ID);
            player = db3.OpusPlayers.Single(q => q.PlayerID == id);
            sRow = new Scoring
            {
                Date = date,
                First = player.First,
                Last = player.Last,
                Group = player.Group,
                STCPlayerID = player.PlayerID,
                Name = item.Player4
            };
            db1.Scores.Add(sRow);
        }

        public string AddPlayCode(string playcode, string newcode)
        {
            string returnCode = "";
            if (playcode == string.Empty)
                returnCode = newcode;
            else
            {
                returnCode = playcode + "," + newcode;
            }
            return returnCode;
        }

        public string RemovePlayCode(string playcode, string oldcode)
        {
            string returnCode = "";
            if (playcode.Contains("," + oldcode))
            {
                // N,S,oldcode,P
                int istart = playcode.IndexOf("," + oldcode);
                returnCode = playcode.Substring(0, istart) + playcode.Substring(istart + 2);
            }
            else if (playcode.Contains(oldcode + ","))
            {
                // oldcode,S,N,P
                returnCode = playcode.Substring(2);
            }
            else
                returnCode = "";
            return returnCode;
        }

        public void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db1.Dispose();
            }

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}