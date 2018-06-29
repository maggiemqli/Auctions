using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace Auctions.Models
{
     public class DashboardView
    {
        public List<AuctionEvent> auctions {get;set;}
        public User User {get;set;}
    }

    public class AuctionView
    {
        [Required]
        [Display(Name="Product Name")]
        [MinLength(3, ErrorMessage="Product name must be greater than 3 characters.")]
        public string product_name { get;set; } 
                
        [Required]
        [Display(Name="Description")]
        [MinLength(10, ErrorMessage="Description must be greater than 10 characters.")]
        public string description { get; set; }
        
        [Display(Name="Starting bid")]
        public float starting_bid {get;set;}
        
        [Display(Name="End Date")]
        [DataType(DataType.Date)]
        public DateTime end_date { get; set; }
    }
}