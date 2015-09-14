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
    public class PastScoringsController : Controller
    {
        private OpusContext db = new OpusContext();
        private Util util = new Util();

        // GET: PastScorings
        public ActionResult Index(string season, string date, string sortOrder)
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

            //if (Session["playedDates"] == null)
            //{
            //    // Get unique dates OPUS Played
            //    List<SelectListItem> items = new List<SelectListItem>();
            //    var dates = db.Assignments;
            //    var result = (from m in dates select m.Date).Distinct().ToList();
            //    foreach (var item in result) items.Add(new SelectListItem { Text = item, Value = item });
            //    Session["playedDates"] = items;
            //}
            if (Session["PastCourtDates"] == null) Session["PastCourtDates"] = util.GetCourtDates(season, Group);
            ViewBag.PlayedDates = Session["PastCourtDates"];

            if (Session["playedDate"] == null)
            {
                if (date != null)
                {
                    Session["playedDate"] = date;
                }
            }
            else date = Session["playedDate"].ToString();

            var query = from a in db.PastScorings
                        where a.Season.Equals(season) && a.Date.Equals(date) && a.Group.Equals(Group)
                        select a;

            ViewBag.CurrentDate = date;
            ViewBag.OpusRankSortParm = String.IsNullOrEmpty(sortOrder) ? "stc_desc" : "";
            ViewBag.NameSortParm = sortOrder == "Name" ? "name_desc" : "Name";
            ViewBag.PercentWonSortParm = sortOrder == "PercentWon" ? "per_desc" : "PercentWon";
            ViewBag.OverallPercentWonSortParm = sortOrder == "OverallPercentWon" ? "oper_desc" : "OverallPercentWon";

            var scores = from s in db.PastScorings
                         where s.Season.Equals(season) && s.Date.Equals(date)
                         select s;
            switch (sortOrder)
            {
                case "name_desc":
                    scores = scores.OrderByDescending(s => s.Name);
                    break;
                case "Name":
                    scores = scores.OrderBy(s => s.Name);
                    break;
                case "stc_desc":
                    scores = scores.OrderByDescending(s => s.OpusRank);
                    break;
                //case "OpusRank":
                //    scores = scores.OrderBy(s => s.OpusRank);
                //    break;
                case "per_desc":
                    scores = scores.OrderByDescending(s => s.PercentWon);
                    break;
                case "PercentWon":
                    scores = scores.OrderBy(s => s.PercentWon);
                    break;
                case "oper_desc":
                    scores = scores.OrderByDescending(s => s.OverallPercentWon);
                    break;
                case "OverallPercentWon":
                    scores = scores.OrderBy(s => s.OverallPercentWon);
                    break;
                default:
                    scores = scores.OrderBy(s => s.OpusRank);
                    break;
            }
            if (sortOrder != string.Empty) return View(scores.ToList());
            return View(query.ToList());
        }

        public ActionResult SelectSeason(string Season)
        {
            Session["PastSeason"] = Season;
            Session["PastCourtDates"] = null;
            return RedirectToAction("Index", new { season = Season });
        }

        // Date Selected
        public ActionResult SelectDate(string Date)
        {
            Session["playedDate"] = Date;
            return RedirectToAction("Index", new { date = Date });
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
