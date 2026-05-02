using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Models.Report_Home
{
    public sealed class CustomerInsightsData
    {
        // KPIs
        public int TotalCustomers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AvgOrderValue { get; set; }
        public decimal TotalWalletBalance { get; set; }
        public int AbandonedCarts { get; set; }

        // Gender: key = "M" | "F" | null→"N", value = count
        public Dictionary<string, int> GenderCounts { get; set; } = new();

        // Top Cities / Countries: (name, count)
        public List<(string Name, int Count)> TopCities { get; set; } = new();
        public List<(string Name, int Count)> TopCountries { get; set; } = new();

        // Loyalty buckets: "0","1-100","101-500","501-1000","1000+"
        public Dictionary<string, int> LoyaltyBuckets { get; set; } = new();

        // Recency buckets
        public Dictionary<string, int> RecencyBuckets { get; set; } = new();
    }

}
