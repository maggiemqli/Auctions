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
                auctions = _dbContext.auctions.OrderByDescending(a => a.starting_bid)
                            .Include(a => a.user).ToList(),
                User = user,
            };
            foreach (AuctionEvent Allevent in _dbContext.auctions)
            {
                DateTime EndDate = Allevent.end_date;
                DateTime StartDate = DateTime.Now;
                TimeSpan span = EndDate - StartDate;
            }
            ViewBag.CurrentUser = user.first_name;
            ViewBag.CurrentBalance = user.wallet_balance;
            return View(newDashboard);
        }

        [HttpGet]
        [Route("/show/{id}")]
        public IActionResult ShowAuction(int id)
        {
            User user = _dbContext.users.Where(n => n.user_id == HttpContext.Session.GetInt32("UserId")).SingleOrDefault();

            AuctionEvent auctionInfo = _dbContext.auctions
                                    .Where(a => a.auction_id == id)
                                    .Include(a => a.user)
                                    .Include(a => a.bid).ThenInclude(b => b.bidder)
                                    .SingleOrDefault();
            HttpContext.Session.SetInt32("ItemId", auctionInfo.auction_id);                       
            return View(auctionInfo);
        }

        [HttpGet("/create")]
        public IActionResult CreateAuction(int id)
        {
            if(ActiveUser == null)
            { 
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

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
                    user_id = user.user_id
                };
                TimeSpan diff = DateTime.Now - newAuction.end_date;
                double x = diff.TotalDays;
                ViewBag.diff = x;
                _dbContext.auctions.Add(newAuction);
                _dbContext.SaveChanges();
                HttpContext.Session.SetInt32("ItemId", newAuction.auction_id);
                return Redirect("dashboard");
            }
            return View("CreateAuction");
        }

        [HttpGet("/delete/{id}")]
        public IActionResult DeleteAuction(int id)
        {
            if(ActiveUser == null)
                return RedirectToAction("Index", "Home");   
            AuctionEvent toDelete = _dbContext.auctions.Where(a => a.auction_id == id).SingleOrDefault();
            _dbContext.auctions.Remove(toDelete);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");     
        }


        [HttpPost("/process")]
        public IActionResult CreateBid(Bid bid, int id)
        {
            User user = _dbContext.users.Where(u=>u.user_id == HttpContext.Session.GetInt32("UserId")).SingleOrDefault();
            AuctionEvent auc = _dbContext.auctions.Where(a => a.auction_id == HttpContext.Session.GetInt32("ItemId")).SingleOrDefault();
            
            if(user == null)
            {
                return RedirectToAction("Index","Home");
            }

            if(ModelState.IsValid)
            {
                if(bid.bid_amount <= auc.starting_bid )
                {
                    ModelState.AddModelError("bid_amount", "Your bid must be greater than current highest bid.");
                    // TempData["Error"] = "Bid must be higher than current highest bid.";
                }
                else if(bid.bid_amount < user.wallet_balance )
                {
                    ModelState.AddModelError("bid_amount", "Insufficient balance in wallet.");
                    // TempData["Error"] = "Insufficient wallet balance.";
                }else{
                    Bid newBid = new Bid
                    {
                        bid_amount = bid.bid_amount,
                        user_id= user.user_id,
                        auction_id = auc.auction_id
                    };
                    auc.starting_bid = bid.bid_amount;
                    user.wallet_balance -= bid.bid_amount;
                    _dbContext.bids.Add(newBid);
                    _dbContext.SaveChanges();
                }
                return Redirect("dashboard");
            }
            return View("ShowAuction");
        }

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