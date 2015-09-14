using System;
using OPUS.DAL;
using System.Linq;
using System.Data.SqlClient;
using System.Data.Linq;
using System.Web.Mvc;
using OPUS.Models;
using OPUS.ViewModels;

namespace OPUS.Controllers
{
    public class CloseSessionController : Controller
    {
        private OpusContext db = new OpusContext();
        private OpusContext db1 = new OpusContext();
        private OpusContext db2 = new OpusContext();
        private OpusContext db3 = new OpusContext();
        private OpusContext db4 = new OpusContext();
        private OpusContext db5 = new OpusContext();
        private OpusContext db6 = new OpusContext();

        // GET: CloseSession
        public ActionResult Index()
        {
            return View(new CloseSessionViewModel { Message = " ", Closed = false });
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = "Name")] CloseSessionViewModel closing, string Gender)
        {
            string Group = Gender;
            string playcode = Session["PlayCode"].ToString();
            try
            {
                //Insure season name has not been used.

                if((from a in db6.PastOpusPlayers where a.Season == closing.Name select a).Count() > 0)
                {
                    closing.Message = "Session " + closing.Name + " already closed";
                    closing.Closed = true;
                    return View(closing);
                }


                //Copy OPUS Players to PastOpusPlayers
                string Group2 = "";
                if (playcode == "S") { Group = "F"; Group2 = "M"; }
                var players = from a in db.OpusPlayers
                              where (a.Group.Equals(Group) | a.Group.Equals(Group2)) & a.PlayCodes.Equals(playcode)
                              select a;
                foreach (var player in players)
                {
                    PastOpusPlayer sRow = new PastOpusPlayer
                    {
                        Season = closing.Name,
                        Group = player.Group,
                        First = player.First,
                        Last = player.Last,
                        PlayCodes = player.PlayCodes,
                        Name = player.Name,
                        STCRank = player.STCRank,
                        Factor = player.Factor,
                        WeeksPlayed = (from a in db6.Scores where a.Name == player.NameRank select a).Count(),
                        OverallPercentWon = player.OverallPercentWon,
                        Rank = player.Rank
                    };
                    db1.PastOpusPlayers.Add(sRow);
                    //Zero out fields for next season
                    player.OverallPercentWon = 0;
                    player.Rank = 100;
                }

                //Copy CourtAssignments to PastCourtAssignments and then delete all rows in CourtAssignments
                var courts = from a in db2.Assignments
                             where a.Group.Equals(Group) & a.PlayCode.Equals(playcode)
                             select a;
                foreach (var court in courts)
                {
                    PastCourtAssignment sRow = new PastCourtAssignment
                    {
                        Season = closing.Name,
                        Group = court.Group,
                        Date = court.Date,
                        Court = court.Court,
                        Player1 = court.Player1,
                        Player2 = court.Player2,
                        Player3 = court.Player3,
                        Player4 = court.Player4,
                    };
                    db3.PastCourtAssignments.Add(sRow);
                    db2.Assignments.Remove(court);
                }

                if (playcode == "O")
                {
                    //Copy Scoring to PastScoring and then delete all rows in Scoring
                    var scores = from a in db4.Scores
                                 where a.Group.Equals(Group)
                                 select a;
                    foreach (var score in scores)
                    {
                        PastScoring sRow = new PastScoring
                        {
                            Season = closing.Name,
                            Group = score.Group,
                            Date = score.Date,
                            Name = score.Name,
                            Played = score.Played,
                            Won = score.Won,
                            PercentWon = score.PercentWon,
                            OverallPercentWon = score.OverallPercentWon,
                            Rank = score.Rank,
                            OpusRank = score.OpusRank,
                        };
                        db5.PastScorings.Add(sRow);
                        db4.Scores.Remove(score);
                    }
                }
            }
            catch (SqlException ex)
            {
                closing.Message = ex.Message + " closing not successfull";
                closing.Closed = false;
                return View(closing);
            }
            catch (NullReferenceException ex)
            {
                closing.Message = ex.Message + " closing not successfull";
                closing.Closed = false;
                return View(closing);
            }

            try
            {
                db.SaveChanges();
                db1.SaveChanges();
                db2.SaveChanges();
                db3.SaveChanges();
                db4.SaveChanges();
                db5.SaveChanges();
            }
            catch (SqlException ex)
            {
                closing.Message = ex.Message + " closed not successfull";
                closing.Closed = false;
                return View(closing);
            }


            closing.Message = closing.Name + " closed successfully";
            closing.Closed = true;
            return View(closing);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db1.Dispose();
                db2.Dispose();
                db3.Dispose();
                db4.Dispose();
                db5.Dispose();
                db6.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
