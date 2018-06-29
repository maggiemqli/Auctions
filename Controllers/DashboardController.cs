using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Auctions.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;

namespace Auctions.Controllers
{
    public class DashboardController : Controller
    {
        private AuctionsContext _dbContext;
        private User ActiveUser
        {
            get {return _dbContext.users.Where(u => u.user_id == HttpContext.Session.GetInt32("UserId")).FirstOrDefault();}
        }
        public DashboardController(AuctionsContext context)
        {
            _dbContext = context;
        }

        // GET : Dashboard
        [HttpGet]
        [Route("dashboard")]
        public IActionResult Index()
        {
            if(ActiveUser == null)
            {
                return RedirectToAction("Index", "Home");
            }
            User user = _dbContext.users.Where(n => n.user_id == HttpContext.Session.GetInt32("UserId")).SingleOrDefault();

            DashboardView newDashboard = new DashboardView
            {
                auctions = _dbContext.auctions.OrderByDescending(a => a.highest_bid)
                            .Include(a => a.user).ToList(),
                User = user,
            };

            ViewBag.CurrentUser = user.first_name;
            ViewBag.CurrentBalance = user.wallet_balance;

            return View(newDashboard);
        }

        // GET : Show Auction
        [HttpGet]
        [Route("/show/{itemid}/{userid}")]
        public IActionResult ShowAuction(int itemid, int userid)
        {
            if(ActiveUser == null)
            { 
                return RedirectToAction("Index", "Home");
            }
            AuctionEvent auctionInfo = _dbContext.auctions
                                    .Where(a => a.auction_id == itemid)
                                    .Include(a => a.user)
                                    .Include(a => a.bid).SingleOrDefault();
            Bid bidInfo = _dbContext.bids
                        .Where(b => b.auction_id == auctionInfo.auction_id)
                        .Include(b => b.bidder).LastOrDefault();
            if(bidInfo != null)
            {
                ViewBag.bidInfo = bidInfo.bidder.first_name;
            }
            HttpContext.Session.SetInt32("ItemId", auctionInfo.auction_id);                       
            return View(auctionInfo);
        }

        // POST : Create Bid 
        [HttpPost("/processbid/{itemid}/{userid}")]
        public IActionResult ProcessBid(float amt, int itemid, int userid)
        {
            if(ActiveUser == null)
            { 
                return RedirectToAction("Index", "Home");
            }
            User user = ActiveUser;
            AuctionEvent auctionInfo = _dbContext.auctions
                                    .Where(a => a.auction_id ==  HttpContext.Session
                                    .GetInt32("ItemId")).SingleOrDefault();
            if(amt == 0)
            {
                TempData["Error"] = "Please specify the amount.";
                return RedirectToAction("ShowAuction");
            }
            else if(amt <= auctionInfo.highest_bid)
            {
                TempData["Error"] = "Your bid MUST be greater than the highest bid.";
                return RedirectToAction("ShowAuction");
            }
            else if(amt > user.wallet_balance)
            {
                TempData["Error"] = "You don't have enough balance for the bid.";
                return RedirectToAction("ShowAuction");
            }
            else
            {
                auctionInfo.highest_bid = amt;
                Bid newBid = new Bid
                {
                    bidder = user,
                    auctions = auctionInfo,
                };
                if(_dbContext.bids.Where(b => b.auction_id == auctionInfo.auction_id) == null)
                {
                    Bid theBid = _dbContext.Add(newBid).Entity;
                }
                else{
                    Bid theBid = _dbContext.Update(newBid).Entity;
                }
                user.wallet_balance -= amt;
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }
        }

        // GET : Create Auction
        [HttpGet("/create")]
        public IActionResult CreateAuction(int id)
        {
            if(ActiveUser == null)
            { 
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST : Create Auction
        [HttpPost("create")]
        public IActionResult CreateAuction(AuctionView model)
        {
            if(ActiveUser == null)
            {   
                return RedirectToAction("Index", "Home"); 
            }
            
            if(model.end_date == null)
            { 
                ModelState.AddModelError("end_date", "End Date is required.");
            }
            else if(model.end_date < DateTime.Now)
            {
                ModelState.AddModelError("end_date", "End Date must be in the future.");
            }
            if(model.starting_bid == 0)
            {
                ModelState.AddModelError("starting_bid ", "Please specify your starting bid.");
            }

            if(ModelState.IsValid)
            {
                User user = _dbContext.users.Where(n => n.user_id == HttpContext.Session.GetInt32("UserId")).SingleOrDefault();
                AuctionEvent newAuction = new AuctionEvent
                {
                    product_name = model.product_name,
                    description = model.description,
                    starting_bid = model.starting_bid,
                    end_date = model.end_date,
                    user_id = user.user_id,
                    highest_bid = model.starting_bid
                };
                _dbContext.auctions.Add(newAuction);
                _dbContext.SaveChanges();
                HttpContext.Session.SetInt32("ItemId", newAuction.auction_id);
                return Redirect("dashboard");
            }
            return View("CreateAuction");
        }

        // GET : Delete Auction
        [HttpGet("/delete/{id}")]
        public IActionResult DeleteAuction(int id)
        {
            if(ActiveUser == null)
            {    
                return RedirectToAction("Index", "Home");   
            }
            AuctionEvent toDelete = _dbContext.auctions.Where(a => a.auction_id == id).Include(a => a.bid).SingleOrDefault();
            _dbContext.auctions.Remove(toDelete);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");     
        }

        // Logout 
        [HttpGet("logout")]
        public IActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}