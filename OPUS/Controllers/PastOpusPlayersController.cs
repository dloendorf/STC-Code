using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OPUS.DAL;
using OPUS.Models;

namespace OPUS.Controllers
{
    public class PastOpusPlayersController : Controller
    {
        private OpusContext db = new OpusContext();
        private Util util = new Util();

        // GET: PastOpusPlayers
        public ActionResult Index(string season, string sortOrder)
        {
            string Group = Session["Group"].ToString();
            string playcode = Session["PlayCode"].ToString();
            if (Session["Seasons"] == null) Session["Seasons"] = util.GetSeasons(Group, playcode);
            ViewBag.Seasons = Session["Seasons"];

            if (Session["PastSeason"] == null)
            {
                if (season != null)
                {
                    Session["PastSeason"] = season;
                }
            }
            else season = Session["PastSeason"].ToString();

            ViewBag.RankSortParm = String.IsNullOrEmpty(sortOrder) ? "rank_desc" : "";
            ViewBag.FirstSortParm = sortOrder == "First" ? "first_desc" : "First";
            ViewBag.LastSortParm = sortOrder == "Last" ? "last_desc" : "Last";
            ViewBag.STCSortParm = sortOrder == "STC Rank" ? "stc_desc" : "STC Rank";
            ViewBag.FactorSortParm = sortOrder == "Factor" ? "factor_desc" : "Factor";
            ViewBag.OverallPercentWonSortParm = sortOrder == "OverallPercentWon" ? "ofactor_desc" : "OverallPercentWon";
            ViewBag.WeeksPlayedSortParm = sortOrder == "WeeksPlayed" ? "wpl_desc" : "WeeksPlayed";
            string Group2 = "";
            if (playcode == "S") { Group = "F"; Group2 = "M"; }
            var players = from s in db.PastOpusPlayers
                          where s.Season.Equals(season) && ( s.Group.Equals(Group) | s.Group.Equals(Group2) ) && s.PlayCodes.Equals(playcode)
                          select s;
            switch (sortOrder)
            {
                case "first_desc":
                    players = players.OrderByDescending(s => s.First);
                    break;
                case "First":
                    players = players.OrderBy(s => s.First);
                    break;
                case "last_desc":
                    players = players.OrderByDescending(s => s.Last);
                    break;
                case "Last":
                    players = players.OrderBy(s => s.Last);
                    break;
                case "stc_desc":
                    players = players.OrderByDescending(s => s.STCRank);
                    break;
                case "STC Rank":
                    players = players.OrderBy(s => s.STCRank);
                    break;
                case "factor_desc":
                    players = players.OrderByDescending(s => s.Factor);
                    break;
                case "OverallPercentWon":
                    players = players.OrderBy(s => s.OverallPercentWon);
                    break;
                case "ofactor_desc":
                    players = players.OrderByDescending(s => s.OverallPercentWon);
                    break;
                case "Factor":
                    players = players.OrderBy(s => s.Factor);
                    break;
                case "rank_desc":
                    players = players.OrderByDescending(s => s.Rank);
                    break;
                case "wpl_desc":
                    players = players.OrderByDescending(s => s.WeeksPlayed);
                    break;
                case "WeeksPlayed":
                    players = players.OrderBy(s => s.WeeksPlayed);
                    break;
                default:
                    players = players.OrderBy(s => s.Rank);
                    break;
            }
            return View(players.ToList());
        }
        public ActionResult SelectSeason(string Season)
        {
            Session["PastSeason"] = Season;
            return RedirectToAction("Index", new { season = Season });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                util.Dispose(true);
            }
            base.Dispose(disposing);
        }
    }
}
