using Microsoft.EntityFrameworkCore;

namespace Auctions.Models
{
    public class AuctionsContext : DbContext
    {
        public AuctionsContext(DbContextOptions <AuctionsContext> options) : base(options){}

        public DbSet<User> users {get;set;}
        public DbSet<AuctionEvent> auctions {get;set;}
        public DbSet<Bid> bids {get;set;}
    }
}