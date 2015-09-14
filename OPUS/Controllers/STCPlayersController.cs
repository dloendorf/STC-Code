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
    public class STCPlayersController : Controller
    {
        private OpusContext dbnew = new OpusContext();

        // GET: OpusPlayers
        [Authorize]
        public ActionResult Index(string sortOrder)
        {
            //ViewBag.RankSortParm = String.IsNullOrEmpty(sortOrder) ? "rank_desc" : "";
            ViewBag.FirstSortParm = sortOrder == "First" ? "first_desc" : "First";
            ViewBag.LastSortParm = sortOrder == "Last" ? "last_desc" : "Last";
            ViewBag.STCSortParm = sortOrder == "STC Rank" ? "stc_desc" : "STC Rank";
            //ViewBag.FactorSortParm = sortOrder == "Factor" ? "factor_desc" : "Factor";
            //ViewBag.OverallPercentWonSortParm = sortOrder == "OverallPercentWon" ? "ofactor_desc" : "OverallPercentWon";
            //ViewBag.RankSortParm = sortOrder == "Rank" ? "rank_desc" : "Rank";
            var players = from s in dbnew.OpusPlayers
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
                default:
                    players = players.OrderBy(s => s.Last);
                    break;
            }
            return View(players.ToList());
        }

        // GET: OpusPlayers/Create
        [Authorize(Roles = "Admin")]
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
        public ActionResult Create([Bind(Include = "PlayerID,Group,First,Last,PlayerCodes,STCRank")] OpusPlayer opusPlayer)
        {
            if (ModelState.IsValid)
            {
                opusPlayer.Rank = 100;
                dbnew.OpusPlayers.Add(opusPlayer);
                dbnew.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(opusPlayer);
        }

        // GET: OpusPlayers/Edit/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            OpusPlayer opusPlayer = dbnew.OpusPlayers.Find(id);
            dbnew.OpusPlayers.Remove(opusPlayer);
            dbnew.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                dbnew.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
