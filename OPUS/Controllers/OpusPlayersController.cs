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
    public class OpusPlayersController : Controller
    {
        private OpusContext dbnew = new OpusContext();
        private OpusContext db = new OpusContext();
        Util util = new Util();

        // GET: OpusPlayers
        [Authorize]
        public ActionResult Index(string sortOrder)
        {
            ViewBag.RankSortParm = String.IsNullOrEmpty(sortOrder) ? "rank_desc" : "";
            ViewBag.FirstSortParm = sortOrder == "First" ? "first_desc" : "First";
            ViewBag.LastSortParm = sortOrder == "Last" ? "last_desc" : "Last";
            ViewBag.STCSortParm = sortOrder == "STC Rank" ? "stc_desc" : "STC Rank";
            ViewBag.FactorSortParm = sortOrder == "Factor" ? "factor_desc" : "Factor";
            ViewBag.OverallPercentWonSortParm = sortOrder == "OverallPercentWon" ? "ofactor_desc" : "OverallPercentWon";
            //ViewBag.RankSortParm = sortOrder == "Rank" ? "rank_desc" : "Rank";
            string Group = Session["Group"].ToString();
            string playcode = Session["PlayCode"].ToString();
            string Group2 = "";
            ViewBag.Title = "OPUS Players";
            if (playcode == "S") { Group = "F"; Group2 = "M"; ViewBag.Title = "Scramble Players"; }
            var players = from s in dbnew.OpusPlayers
                          where (s.Group.Equals(Group) | s.Group.Equals(Group2)) & s.PlayCodes.Contains(playcode)
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
                default:
                    players = players.OrderBy(s => s.Rank);
                    break;
            }
            return View(players.ToList());
        }

        // GET: OpusPlayers/Create
        [Authorize(Roles="Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: OpusPlayers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Create([Bind(Include = "PlayerID,First,Last,STCRank,Factor")] OpusPlayer opusPlayer)
        {
            if (ModelState.IsValid)
            {
                bool addPlayer = true;
                //Check if this player exists just not in OPUS...if so just add the playCode
                string sGroup = Session["Group"].ToString();
                string playcode = Session["PlayCode"].ToString();
                var DBPlayers = db.OpusPlayers;
                var result = (from m in DBPlayers where m.First.Equals(opusPlayer.First) && m.Last.Equals(opusPlayer.Last) select m).ToList();
                foreach (var player in result)
                {
                    player.PlayCodes = util.AddPlayCode(opusPlayer.PlayCodes, playcode);
                    player.Factor = opusPlayer.Factor;
                    player.Rank = 100;
                    db.SaveChanges();
                    addPlayer = false;
                }

                if(addPlayer)
                {
                    //Otherwise add player as new
                    opusPlayer.Group = sGroup;
                    opusPlayer.Rank = 100;
                    opusPlayer.PlayCodes = playcode;
                    db.OpusPlayers.Add(opusPlayer);
                    db.SaveChanges();
                }
            return RedirectToAction("Index");
            }

            return View(opusPlayer);
        }

        // GET: OpusPlayers/Edit/5
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpusPlayer opusPlayer = dbnew.OpusPlayers.Find(id);
            if (opusPlayer == null)
            {
                return HttpNotFound();
            }
            return View(opusPlayer);
        }

        // POST: OpusPlayers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Edit([Bind(Include = "PlayerID,Group,First,Last,PlayCodes,STCRank,Factor,OverallPercentWon,Rank")] OpusPlayer opusPlayer)
        {
            if (ModelState.IsValid)
            {
                dbnew.Entry(opusPlayer).State = EntityState.Modified;
                dbnew.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(opusPlayer);
        }

        // GET: OpusPlayers/Delete/5
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpusPlayer opusPlayer = dbnew.OpusPlayers.Find(id);
            if (opusPlayer == null)
            {
                return HttpNotFound();
            }
            return View(opusPlayer);
        }

        // POST: OpusPlayers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Ladies Monitor,Mens Monitor")]
        public ActionResult DeleteConfirmed(int id)
        {
            OpusPlayer opusPlayer = dbnew.OpusPlayers.Find(id);
            string playcode = Session["PlayCode"].ToString();
            opusPlayer.PlayCodes = util.RemovePlayCode(opusPlayer.PlayCodes, playcode);
            //dbnew.OpusPlayers.Remove(opusPlayer);
            dbnew.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbnew.Dispose();
                db.Dispose();
                util.Dispose(true);
            }
            base.Dispose(disposing);
        }
    }
}
