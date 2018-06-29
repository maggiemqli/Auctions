using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace Auctions.Models
{
    public class Bid : BaseEntity
    {
        [Key]
        public int bid_id { get;set; }
        public int user_id { get;set; }
        public int auction_id { get;set; }
        public DateTime created_at { get;set; }
        public DateTime updated_at { get;set; }

        public User bidder { get;set; }
        public AuctionEvent auctions { get;set; }

        public Bid()
        {
            created_at = DateTime.Now;
        }
    }
} 