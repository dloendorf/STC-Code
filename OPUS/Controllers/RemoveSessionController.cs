using System;
using OPUS.DAL;
using System.Linq;
using System.Data.SqlClient;
using System.Web.Mvc;
using System.Collections.Generic;
using OPUS.ViewModels;

namespace OPUS.Controllers
{
    public class RemoveSessionController : Controller
    {
        private Util util = new Util();
        private OpusContext db = new OpusContext();
        private OpusContext db1 = new OpusContext();
        private OpusContext db2 = new OpusContext();
        private OpusContext db3 = new OpusContext();

        // GET: CloseSession
        public ActionResult Index()
        {
            string playcode = Session["PlayCode"].ToString();
            return View(new RemoveSessionViewModel { Message = " ", Removed = false, Sessions = util.GetSeasons(Session["Group"].ToString(), playcode) });
        }

        [HttpPost]
        public ActionResult Index(RemoveSessionViewModel removing)
        {
            string Group = Session["Group"].ToString();
            try
            {
                //Insure session name valid.

                if ((from a in db3.PastOpusPlayers where a.Season.Equals(removing.Session) select a).Count() == 0)
                {
                    removing.Message = "Session " + removing.Session + " not found";
                    removing.Removed = true;
                    return View(removing);
                }


                //Delete Session OPUS Players
                string Group2 = "";
                string playcode = Session["PlayCode"].ToString();
                if (playcode == "S") { Group = "F"; Group2 = "M"; }
                var players = from a in db.PastOpusPlayers
                              where a.Season.Equals(removing.Session) && (a.Group.Equals(Group) | a.Group.Equals(Group2))
                              select a;
                foreach (var player in players)
                {
                    db.PastOpusPlayers.Remove(player);
                }
                //Delete session CourtAssignments
                var courts = from a in db1.PastCourtAssignments
                             where a.Group.Equals(Group) && a.Season.Equals(removing.Session)
                             select a;
                foreach (var court in courts)
                {
                    db1.PastCourtAssignments.Remove(court);
                }

                if(playcode == "O")
                {
                    //Delete session Scoring
                    var scores = from a in db2.PastScorings
                                 where a.Group.Equals(Group) && a.Season.Equals(removing.Session)
                                 select a;
                    foreach (var score in scores)
                    {
                        db2.PastScorings.Remove(score);
                    }
                }
            }
            catch (SqlException ex)
            {
                removing.Message = ex.Message + " removal not successfull";
                removing.Removed = false;
                return View(removing);
            }
            catch (NullReferenceException ex)
            {
                removing.Message = ex.Message + " removal not successfull";
                removing.Removed = false;
                return View(removing);
            }

            try
            {
                db.SaveChanges();
                db1.SaveChanges();
                db2.SaveChanges();
            }
            catch (SqlException ex)
            {
                removing.Message = ex.Message + " removing not successfull";
                removing.Removed = false;
                return View(removing);
            }


            removing.Message = removing.Session + " removed successfully";
            removing.Removed = true;
            removing.Sessions = util.GetSeasons(Session["Group"].ToString(), Session["PlayCode"].ToString());
            return View(removing);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
                db1.Dispose();
                db2.Dispose();
                db3.Dispose();
                util.Dispose(true);
            }
            base.Dispose(disposing);
        }
    }
}