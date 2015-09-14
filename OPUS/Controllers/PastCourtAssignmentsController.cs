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
    public class PastCourtAssignmentsController : Controller
    {
        private OpusContext db = new OpusContext();
        private Util util = new Util();

        // GET: PastCourtAssignments
        public ActionResult Index(string season, string date)
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

            //string lastDate = "";
            if (Session["PastCourtDates"] == null) Session["PastCourtDates"] = util.GetCourtDates(season, Group);
            ViewBag.CourtDates = Session["PastCourtDates"];

            if (Session["PastCourtDate"] == null)
            {
                if (date != null)
                {
                    Session["PastCourtDate"] = date;
                }
            }
            else date = Session["PastCourtDate"].ToString();

            var assignmentData = from a in db.PastCourtAssignments
                                 where a.Season.Equals(season) && a.Date.Equals(date) && a.PlayCode.Equals(playcode) && a.Group.Equals(Group)
                                 select a;

            return View(assignmentData.ToList());
        }

        public ActionResult SelectSeason(string Season)
        {
            Session["PastSeason"] = Season;
            Session["PastCourtDates"] = null;
            return RedirectToAction("Index", new { season = Season });
        }

        public ActionResult SelectDate(string Date)
        {
            Session["PastCourtDate"] = Date;
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
