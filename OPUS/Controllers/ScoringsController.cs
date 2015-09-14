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
    public class ScoringsController : Controller
    {
        private OpusContext db = new OpusContext();
        private OpusContext db1 = new OpusContext();
        private OpusContext db2 = new OpusContext();
        private OpusContext db3 = new OpusContext();
        private OpusContext dbOverall = new OpusContext();
        Util util = new Util();
        // GET: Scorings
        [Authorize]
        public ActionResult Index(string date, string sortOrder)
        {
            string sGroup = Session["Group"].ToString();
            if (Session["playedDates"] == null)
            {
                // Get unique dates OPUS Played
                List<SelectListItem> items = new List<SelectListItem>();
                var dates = db.Assignments;
                var result = (from m in dates where m.Group.Equals(sGroup) select m.Date).Distinct().ToList();
                foreach (var item in result) items.Add(new SelectListItem { Text = item, Value = item });
                Session["playedDates"] = items;
            }
            ViewBag.PlayedDates = Session["playedDates"];

            if (Session["playedDate"] == null)
            {
                if (date != null)
                {
                    Session["playedDate"] = date;
                }
            }
            else date = Session["playedDate"].ToString();

            var query = from a in db.Scores
                        where a.Date.Equals(date) && a.Group.Equals(sGroup)
                        select a;
            int i = 0;
            foreach (var item in query) i++;

            if (i == 0 && date != null)
            {
                OpusContext db1 = new OpusContext();
                //Add player data from Assignment table
                var assignmentData = from a in db.Assignments
                                     where a.Date.Equals(date) && a.Group.Equals(sGroup)
                                     select a;
                foreach (var item in assignmentData)
                {
                    //Load a record into Scores table for each player (4 per item found)
                    util.AddCourtToScoring(db1, db3, date, item);
                }
                db1.SaveChanges();
            }

            ViewBag.CurrentDate = date;
            ViewBag.OpusRankSortParm = String.IsNullOrEmpty(sortOrder) ? "stc_desc" : "";
            ViewBag.FirstSortParm = sortOrder == "First" ? "first_desc" : "First";
            ViewBag.LastSortParm = sortOrder == "Last" ? "last_desc" : "Last";
            //ViewBag.OpusRankSortParm = sortOrder == "OpusRank" ? "stc_desc" : "OpusRank";
            ViewBag.PercentWonSortParm = sortOrder == "PercentWon" ? "per_desc" : "PercentWon";
            ViewBag.OverallPercentWonSortParm = sortOrder == "OverallPercentWon" ? "oper_desc" : "OverallPercentWon";

            var scores = from s in db.Scores
                         where s.Date.Equals(date) && s.Group.Equals(sGroup)
                         select s;
            switch (sortOrder)
            {
                case "first_desc":
                    scores = scores.OrderByDescending(s => s.First);
                    break;
                case "First":
                    scores = scores.OrderBy(s => s.First);
                    break;
                case "last_desc":
                    scores = scores.OrderByDescending(s => s.Last);
                    break;
                case "Last":
                    scores = scores.OrderBy(s => s.Last);
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
            if(sortOrder != string.Empty) return View(scores.ToList());
            return View(query.ToList());
        }

        // Date Selected
        public ActionResult SelectDate(string Date)
        {
            Session["playedDate"] = Date;
            return RedirectToAction("Index", new { date = Date });
        }

        // GET: Scorings/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Scoring scoring = db.Scores.Find(id);
            if (scoring == null)
            {
                return HttpNotFound();
            }
            return View(scoring);
        }

        // GET: Scorings/Create
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Scorings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Create([Bind(Include = "ID,Group,Date,Name,Played,Won")] Scoring scoring)
        {
            if (ModelState.IsValid)
            {
                db.Scores.Add(scoring);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(scoring);
        }

        // Rank Players
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Rank()
        {
            RankOverall();
            return RedirectToAction("Index");
        }

        private void RankIndividual(Scoring scores)
        {
            OpusPlayer player = db3.OpusPlayers.Single(q => q.PlayerID == scores.STCPlayerID);
            double percent = 0;
            if (scores.Played > 0)
            {
                percent = (double)scores.Won / (double)scores.Played;
            }
            Scoring per = db2.Scores.Single(q => q.ID == scores.ID);
            per.PercentWon = Convert.ToInt32(percent * 100.0);
            per.Rank = per.PercentWon * player.Factor;
            per.OverallPercentWon = OverallPercentWon(scores.ID);
            player.OverallPercentWon = per.OverallPercentWon;
            player.Rank = per.Rank;
            db2.SaveChanges();
            db3.SaveChanges();
            RankOverall();
        }

        private void RankOverall()
        {
            string sGroup = Session["Group"].ToString();
            string date = (string)Session["playedDate"];
            var scores = from s in db.Scores
                         where s.Date.Equals(date) && s.Group.Equals(sGroup)
                         select s;
            var final = scores.OrderByDescending(x => x.Rank).ThenByDescending(x => x.OverallPercentWon);

            int i = 1;
            int iPos = 0;
            foreach (var player in final)
            {
                if(player.Played != null)
                {
                    iPos = i++;
                }
                else
                {
                    iPos = 100;
                }
                Scoring per = db2.Scores.Single(q => q.ID == player.ID);
                per.OpusRank = iPos;
                OpusPlayer p1 = db3.OpusPlayers.Single(q => q.PlayerID == player.STCPlayerID);
                p1.Rank = per.OpusRank;
            }
            db2.SaveChanges();
            db3.SaveChanges();
        }

        private int OverallPercentWon(int id)
        {
            var scores = from s in dbOverall.Scores
                         where s.ID.Equals(id)
                         select s;
            int overallPlayed = 0;
            int overallWon = 0;
            foreach (var score in scores)
            {
                if (score.Played != null)
                {
                    overallPlayed += (int)score.Played;
                    overallWon += (int)score.Won;
                }
            }
            if (overallPlayed == 0) return 0;
            else return (int)((Convert.ToDouble(overallWon) / Convert.ToDouble(overallPlayed)) * 100.0);
        }

        // GET: Scorings/Edit/5
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Scoring scoring = db.Scores.Find(id);
            if (scoring == null)
            {
                return HttpNotFound();
            }
            if(scoring.Played == 0)
            {
                scoring.Won = null;
                scoring.Played = null;
            }
            return View(scoring);
        }

        // POST: Scorings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Edit([Bind(Include = "ID,Group,Date,First,Last,Name,Played,Won,Rank,STCPlayerID")] Scoring scoring)
        {
            if (ModelState.IsValid)
            {
                db.Entry(scoring).State = EntityState.Modified;
                scoring.Rank = 11;
                db.SaveChanges();
                RankIndividual(scoring);
                return RedirectToAction("Index");
            }
            return View(scoring);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db1.Dispose();
                db2.Dispose();
                db3.Dispose();
                dbOverall.Dispose();
                util.Dispose(true);
            }
            base.Dispose(disposing);
        }
    }
}
