using System;
using System.IO;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Configuration;
using System.Web.Mvc;
using OPUS.Models;
using OPUS.ViewModels;
using OPUS.DAL;
using SendGrid;

namespace OPUS.Controllers
{
    public class CourtAssignmentsController : Controller
    {
        private OpusContext db = new OpusContext();
        private OpusContext db1 = new OpusContext();
        private OpusContext db2 = new OpusContext();
        private OpusContext db3 = new OpusContext();
        private Util util = new Util();

        // GET: CourtAssignments
        [Authorize]
        public ActionResult Index(string date)
        {
            //string lastDate = "";
            if (Session["courtDates"] == null) GetCourtDates();
            ViewBag.CourtDates = Session["courtDates"];
            string Group = Session["Group"].ToString();

            if (Session["courtDate"] == null)
            {
                if (date != null)
                {
                    Session["courtDate"] = date;
                }
            } else date = Session["courtDate"].ToString();

            if (Session["PlayersList"] == null)
            {
                string playcode = Session["PlayCode"].ToString();
                List<PlayerList> vPlayers = util.GetRankedPlayers(Session["Group"].ToString(), playcode);
                Session["PlayersList"] = vPlayers;
                Session["PlayersSelectList"] = new SelectList((vPlayers).Select(d => new SelectListItem { Text = d.Name, Value = d.ID.ToString() }), "Value", "Text");
            }


            var assignmentData = from a in db.Assignments
                                 where a.Date.Equals(date) && a.Group.Equals(Group)
                                 select a;
            return View(assignmentData.ToList());
        }

        public ActionResult ThisWeeksPlayers()
        {
            string playcode = Session["PlayCode"].ToString();
            ViewBag.Players = util.GetRankedPlayers(Session["Group"].ToString(), playcode);
            return View();
        }
        [HttpPost]
        public ActionResult ThisWeeksPlayers(FormCollection form)
        {
            var checkedValues = form.GetValues("selectChkBx");
            if (checkedValues != null)
            {
                //List<PlayerList> playersDrop = new List<PlayerList>();
                List<OPUSPlayerInfoList> players = new List<OPUSPlayerInfoList>();
                foreach (var id in checkedValues)
                {
                    int id1 = Convert.ToInt32(id);
                    var player = db1.OpusPlayers.First(u => u.PlayerID == id1);
                    players.Add(new OPUSPlayerInfoList { ID = player.PlayerID, Name = player.NameRank, OverallPercentWon = player.OverallPercentWon, Rank = player.Rank });
                    //playersDrop.Add(new PlayerList { ID = player.PlayerID, Name = player.NameRank });
                }
                List<OPUSPlayerInfoList> vPlayers = players.OrderBy(x => x.Rank).ToList();
                Session["PlayerWebGridInfo"] = vPlayers;

                //Session["Players"] = new SelectList((vPlayers).Select(d => new SelectListItem { Text = d.Name, Value = d.ID.ToString() }), "Value", "Text");

                Session["ThisWeeksPlayers"] = new SelectList(players.OrderBy(x => x.Rank), "ID", "Name");
            }
            return RedirectToAction("index");
        }

        private void GetCourtDates()
        {
            // Get unique dates OPUS Played
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem { Text = "New Date", Value = "NewDate" });
            string Group = Session["Group"].ToString();
            var dates = db.Assignments;
            var result = (from m in dates where m.Group.Equals(Group) select m.Date).Distinct().ToList();
            foreach (var item in result)
            {
                items.Add(new SelectListItem { Text = item, Value = item });
                //lastDate = item;
            }
            Session["courtDates"] = items;
        }

        private void PreviousCourtAssignments(string date)
        {
            List<PreviousAssignment> previous = new List<PreviousAssignment>();
            var courts = db1.Assignments;
            string Group = Session["Group"].ToString();
            var result = (from m in courts where m.Date.Equals(date) && m.Group.Equals(Group) select m);
            foreach (var item in result)
            {
                previous.Add(new PreviousAssignment { Court = (int)item.Court, Player1 = item.Player1, Player2 = item.Player2, Player3 = item.Player3, Player4 = item.Player4 });
            }
            ViewBag.PreviousAssign = previous;
        }

        private void GetPreviousCourtDate(List<SelectListItem> dates)
        {
            string lDate = Session["previousCourtDate"].ToString();
            foreach (var date in dates)
            {
                if (date.Text == Session["previousCourtDate"].ToString())
                {
                    break;
                }
                else
                    lDate = date.Text;
            }
            Session["previousCourtDate"] = lDate;

        }

        // Date Selected
        [HttpPost]
        public ActionResult SelectDate(string Date)
        {
            Session["courtDate"] = Date;
            return RedirectToAction("Index", new { date = Date });
        }

        [Authorize]
        //[HttpPost]
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Print()
        {
            //List<CourtAssignmentPrint> courtassignments = new List<CourtAssignmentPrint>();
            string Group = Session["Group"].ToString();
            string playcode = Session["PlayCode"].ToString();
            var courts = db1.Assignments;
            if(Session["courtDate"] == null) return RedirectToAction("Index");
            var date = Session["courtDate"].ToString();
            var assignmentData = from a in db.Assignments
                                 where a.Date.Equals(date) && a.Group.Equals(Group) && a.PlayCode.Equals(playcode)
                                 select a;
            return View(assignmentData.ToList());
        }

        //[HttpPost]
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Email()
        {

            var settings = db.OpusSettings.First();
            //// Add multiple addresses to the To field.
            //List<String> recipients = new List<String>
            //{
            //    @"Jeff Smith <jeff@example.com>",
            //    @"Anna Lidman <anna@example.com>",
            //    @"Peter Saddow <peter@example.com>"
            //};

            //myMessage.AddTo(recipients);

            var username = System.Environment.GetEnvironmentVariable("SendGridUser");
            var pswd = System.Environment.GetEnvironmentVariable("SendGridPassword");

            if(username == null)
            {
                username = WebConfigurationManager.AppSettings["SendGridUser"];
                pswd = WebConfigurationManager.AppSettings["SendGridPassword"];
            }
            var courts = db1.Assignments;
            if (Session["courtDate"] == null) return RedirectToAction("Index");
            var date = Session["courtDate"].ToString();
            string Group = Session["Group"].ToString();
            string sMessage = "OPUS Court Assignments for " + date;
            string Monitor = "OPUS Monitor";
            if (Group == "F") { sMessage = "Ladies' " + sMessage; Monitor = "Ladies'" + Monitor; }
            if (Group == "M") { sMessage = "Men's " + sMessage; Monitor = "Men's" + Monitor; }
            IEnumerable<EmailViewModel> emailData = from a in db.Assignments
                            where a.Date.Equals(date) && a.Group.Equals(Group)
                            select new EmailViewModel { Court = (int)a.Court, Player1 = a.Player1, Player2 = a.Player2, Player3 = a.Player3, Player4 = a.Player4 };
            string viewHtml = RenderViewToString(this.ControllerContext, "Email", emailData);

            SendGridMessage myMessage = new SendGridMessage();
            myMessage.AddTo(settings.STCEmailAddress);
            myMessage.From = new MailAddress(settings.OpusCoordinatorEmailAddress, "OPUS Monitor");
            myMessage.Subject = sMessage;
            myMessage.Html = viewHtml;

            // Create credentials, specifying your user name and password.
            var credentials = new NetworkCredential(username, pswd);

            // Create an Web transport for sending email, using credentials...
            var transportWeb = new Web(credentials);

            // Send the email.
            try {
                transportWeb.DeliverAsync(myMessage);
                Session["Message1"] = "Success";
                Session["Message"] = "Email Sent To " + settings.STCEmailAddress;

            }
            catch
            {
                ViewBag.Message1 = "Failure";
                ViewBag.Message = "Failed to Send Email Sent To " + "dloendorf@outlook.com";
            }

            return RedirectToAction("ConfirmMessage");
        }

        public static string RenderViewToString(ControllerContext ct, string viewName, object viewData)
        {
            var razorViewEngine = new RazorViewEngine();
            var razorViewResult = razorViewEngine.FindView(ct, viewName, "", false);
            var writer = new StringWriter();
            var viewContext = new ViewContext(ct, razorViewResult.View, new ViewDataDictionary(viewData), new TempDataDictionary(), writer);
            razorViewResult.View.Render(viewContext, writer);
            return writer.ToString();
        }

        public ActionResult ConfirmMessage()
        {
            return View();
        }

        //GET: CourtAssignments/Create
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Create()
        {
            CourtAssignment ca = new CourtAssignment();
            if(Session["courtDate"] == null) Session["courtDate"] = "NewDate";
            ca.defaultDate = Session["courtDate"].ToString();
            string playcode = Session["PlayCode"].ToString();

            if (Session["courtDates"] == null) GetCourtDates();

            //Setup data for the name dropdowns
            if (ViewBag.Players == null)
            {
                if(Session["ThisWeeksPlayers"] != null)
                {
                    ViewBag.Players = Session["ThisWeeksPlayers"];
                }
                else
                    ViewBag.Players = Session["PlayersSelectList"];
            }

            //Setup data for the OPUS Player grid
            if (Session["PlayerWebGridInfo"] == null)
            {
                //GetOPUSPlayer();
                // ViewBag.ThisWeekPlayers = ViewBag.OPUSPlayers;
                ViewBag.PlayerWebGridInfo = util.GetOpusPlayerInfo(Session["Group"].ToString(), playcode);
                Session["PlayerWebGridInfo"] = ViewBag.PlayerWebGridInfo;
            }
            else
                ViewBag.PlayerWebGridInfo = Session["PlayerWebGridInfo"];
            ca.Court = null;

            //Get Previous Court Assignments
            //Check if date is not new date and get appropriate previous courts
            List<SelectListItem> dates = Session["CourtDates"] as List<SelectListItem>;

            string pcd = "";
            if(Session["previousCourtDate"] != null) pcd = Session["previousCourtDate"].ToString();
            string cd = Session["courtDate"].ToString();

            if (Session["courtDate"].ToString() == "NewDate")
            {
                Session["previousCourtDate"] = dates.Last().Text;
            }
            else if(Session["previousCourtDate"] == null)
            {
                Session["previousCourtDate"] = Session["courtDate"];
                GetPreviousCourtDate(dates);
            }
            else if (Session["courtDate"].ToString() == Session["previousCourtDate"].ToString())
            {
                GetPreviousCourtDate(dates);
            }

            if (ViewBag.PreviousAssign == null)
            {
                PreviousCourtAssignments(Session["previousCourtDate"].ToString());
            }

            return View(ca);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Create([Bind(Include = "ID,Date,Court,Player1ID,Player2ID,Player3ID,Player4ID")] CourtAssignment courtAssignment)
        {
            List<PlayerList> players = (List<PlayerList>)Session["PlayersList"];
            courtAssignment.Player1 = players.Where(x => x.ID == courtAssignment.Player1ID).First().Name;
            courtAssignment.Player2 = players.Where(x => x.ID == courtAssignment.Player2ID).First().Name;
            courtAssignment.Player3 = players.Where(x => x.ID == courtAssignment.Player3ID).First().Name;
            courtAssignment.Player4 = players.Where(x => x.ID == courtAssignment.Player4ID).First().Name;

            if (ModelState.IsValid)
            {
                courtAssignment.Group = Session["Group"].ToString();
                courtAssignment.PlayCode = Session["PlayCode"].ToString();

                db.Assignments.Add(courtAssignment);
                db.SaveChanges();
                Session["courtDate"] = courtAssignment.Date;

                //Check if date already in courtdates and if not add it
                List<SelectListItem> n = (List<SelectListItem>)Session["courtDates"];
                string last = n.Last().Text;
                if(last != courtAssignment.Date)
                {
                    //null playedDates and courtDates so they will regenerate with the new date included.
                    Session["playedDates"] = null;
                    Session["courtDates"] = null;
                }

                //Check if Scoring has been started for this date and if so add the new players
                if(db2.Scores.Where(u => u.Date == courtAssignment.Date).Any())
                {
                    //Load a record into Scores table for each player
                    util.AddCourtToScoring(db1, db3, courtAssignment.Date, courtAssignment);
                }
                db1.SaveChanges();

                //Remove players from Players and ThisWeekPlayers
                if(Session["PlayerWebGridInfo"] != null)
                {
                    List<OPUSPlayerInfoList> pt = (List < OPUSPlayerInfoList >) Session["PlayerWebGridInfo"];
                    pt.Remove(pt.Where(c => c.Name == courtAssignment.Player1).Single());
                    pt.Remove(pt.Where(c => c.Name == courtAssignment.Player2).Single());
                    pt.Remove(pt.Where(c => c.Name == courtAssignment.Player3).Single());
                    pt.Remove(pt.Where(c => c.Name == courtAssignment.Player4).Single());
                    if (pt.Count > 0)
                    {
                        Session["PlayerWebGridInfo"] = pt; //Grid
                        Session["ThisWeeksPlayers"] = new SelectList(pt.OrderBy(x => x.Rank), "ID", "NameRank"); //Dropdown List
                    }
                    else
                    {
                        Session["PlayerWebGridInfo"] = null; //Grid
                        Session["ThisWeeksPlayers"] = null; //DropdownList
                    }
                }

                return RedirectToAction("Index");
            }

            return View(courtAssignment);
        }

        // GET: CourtAssignments/Edit/5
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourtAssignment courtAssignment = db.Assignments.Find(id);
            if (ViewBag.Players == null) ViewBag.Players = Session["PlayersSelectList"];
            return View(courtAssignment);
        }

        // POST: CourtAssignments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Edit([Bind(Include = "ID,Group,PlayCode,Date,Court,Player1ID,Player2ID,Player3ID,Player4ID")] CourtAssignment courtAssignment)
        {
            List<PlayerList> players = (List<PlayerList>)Session["PlayersList"];
            courtAssignment.Player1 = players.Where(x => x.ID == courtAssignment.Player1ID).First().Name;
            courtAssignment.Player2 = players.Where(x => x.ID == courtAssignment.Player2ID).First().Name;
            courtAssignment.Player3 = players.Where(x => x.ID == courtAssignment.Player3ID).First().Name;
            courtAssignment.Player4 = players.Where(x => x.ID == courtAssignment.Player4ID).First().Name;

            if (ModelState.IsValid)
            {
                //Check if Scoring has been started for this date and if so update players
                string[] toBeDeleted = new string[4];
                string[] toBeAdded = new string[4];
                string[] currentNames = new string[4];
                if (db2.Scores.Where(u => u.Date == courtAssignment.Date).Any())
                {
                    var current = (from c in db2.Assignments where (c.Date.Equals(courtAssignment.Date) && c.Court == courtAssignment.Court) select c);
                    int iDelete = 0;
                    int iAdd = 0;
                    bool bSaveChanges = false;
                    foreach(var item in current)
                    {
                        //Check for deletions
                        if(ShouldDelete(item.Player1, courtAssignment))
                        {
                            toBeDeleted[iDelete++] = item.Player1;
                        }
                        currentNames[0] = item.Player1;
                        if (ShouldDelete(item.Player2, courtAssignment))
                        {
                            toBeDeleted[iDelete++] = item.Player2;
                        }
                        currentNames[1] = item.Player2;
                        if (ShouldDelete(item.Player3, courtAssignment))
                        {
                            toBeDeleted[iDelete++] = item.Player3;
                        }
                        currentNames[2] = item.Player3;
                        if (ShouldDelete(item.Player4, courtAssignment))
                        {
                            toBeDeleted[iDelete++] = item.Player4;
                        }
                        currentNames[3] = item.Player4;

                        //Check for additions
                        if (ShouldAdd(currentNames, courtAssignment.Player1))
                        {
                            toBeAdded[iAdd++] = courtAssignment.Player1;
                        }
                        if (ShouldAdd(currentNames, courtAssignment.Player2))
                        {
                            toBeAdded[iAdd++] = courtAssignment.Player2;
                        }
                        if (ShouldAdd(currentNames, courtAssignment.Player3))
                        {
                            toBeAdded[iAdd++] = courtAssignment.Player3;
                        }
                        if (ShouldAdd(currentNames, courtAssignment.Player4))
                        {
                            toBeAdded[iAdd++] = courtAssignment.Player4;
                        }
                    }
                    for(int i = 0; i <= 3; i++)
                    {
                        //Remove Player i
                        if (toBeDeleted[i] != null)
                        {
                            var deleteName = toBeDeleted[i];
                            var player = db2.Scores.First(u => u.Name == deleteName && u.Date == courtAssignment.Date);
                            if (player != null)
                            {
                                db2.Scores.Remove(player);
                                bSaveChanges = true;
                            }
                        }

                        //Add Player
                        if(toBeAdded[i] != null)
                        {
                            //Load a record into Scores table
                            Scoring sRow = new Scoring
                            {
                                Date = courtAssignment.Date,
                                Name = toBeAdded[i]
                            };
                            db2.Scores.Add(sRow);
                            bSaveChanges = true;
                        }
                    }
                    if(bSaveChanges) db2.SaveChanges();
                }
                db.Entry(courtAssignment).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(courtAssignment);
        }

        private bool ShouldDelete(string name, CourtAssignment assigned)
        {
            if (name != assigned.Player1 &&
                name != assigned.Player2 &&
                name != assigned.Player3 &&
                name != assigned.Player4) return true;
            else return false;
        }

        private bool ShouldAdd(string[] name, string assigned)
        {
            if (name[0] != assigned &&
                name[1] != assigned &&
                name[2] != assigned &&
                name[3] != assigned)
                return true;
            else return false;
        }

        // GET: CourtAssignments/Delete/5
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourtAssignment courtAssignment = db.Assignments.Find(id);
            if (courtAssignment == null)
            {
                return HttpNotFound();
            }
            return View(courtAssignment);
        }

        // POST: CourtAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult DeleteConfirmed(int id)
        {
            CourtAssignment courtAssignment = db.Assignments.Find(id);
            db.Assignments.Remove(courtAssignment);
            db.SaveChanges();
            return RedirectToAction("Index");
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
