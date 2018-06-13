using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Auctions.Models;

namespace Auctions.Models
{
    public class AuctionEvent
    {
        [Key]
        public int auction_id { get;set; }

        public string product_name { get;set; } 
        public string description { get; set; }
        public float starting_bid {get;set;}
        public DateTime end_date { get; set; }

        // public float bid_amount {get;set;}
        // public int bidder_id {get;set;}

        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

        public int user_id {get;set;}
        public User user { get;set; }
        
        public Bid bid{ get;set; }

        public AuctionEvent()
        {
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
        }
    }
}