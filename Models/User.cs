using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Auctions.Models;

namespace Auctions.Models
{
    public abstract class BaseEntity{}

    public class User : BaseEntity
    {
        [Key]
        public int user_id { get; set; }
        public string username { get;set; } 
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string password { get; set; } 
        public float wallet_balance {get;set;}
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }

        public List<AuctionEvent> auctions {get;set;}

        
        public User()
        {
            auctions = new List<AuctionEvent>();
            created_at = DateTime.Now;
            updated_at = DateTime.Now;
        }
    }


}