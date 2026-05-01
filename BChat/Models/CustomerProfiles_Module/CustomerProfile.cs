using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Models.CustomerProfiles_Module
{
    public class CustomerProfile
    {
        public int CustomerId  { get; set; }
        public string? Email      { get; set; }
        public string? Country    { get; set; }
        public string? City       { get; set; }
        public string? Gender     { get; set; }
        public DateTime? Birthday  { get; set; }
        public int? OrderCount    { get; set; }
        public int? LoyaltyPoints { get; set; }
        public decimal? TotalSpent { get; set; }
        public decimal? AvgOrderValue { get; set; }
        public DateTime? LastPurchaseDate { get; set; }
        public int? CancelledOrders { get; set; }
        public decimal? WalletBalance{ get; set; }
        public int? AbandonedCartCount { get; set; }

    }
}
