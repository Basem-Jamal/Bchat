using BChat.Models.Customer_Module.CustomerProfiles_Module;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Data.DataStore.CustomerProfile_Repository
{
    public static class CustomerProfileRepository
    {
        private static string _connectionString = DatabaseConfig.ConnectionString;

        public static CustomerProfile? GetByCusotmerId(int cusotmerId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"SELECT CustomerId, Email, Country, City, Gender,
                                        Birthday, LoyaltyPoints, OrderCount, TotalSpent,
                                        AvgOrderValue, LastPurchaseDate, CancelledOrders,
                                        WalletBalance, AbandonedCartCount
                                FROM CustomerProfiles
                                WHERE CustomerId = @CustomerId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", cusotmerId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                            return Map(reader);
                    }
                }
            }

            return null;
        }


        public static void Add(CustomerProfile profile)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO CustomerProfiles
                                     (CustomerId, Email, Country, City, Gender, Birthday,
                                      LoyaltyPoints, OrderCount, TotalSpent, AvgOrderValue,
                                      LastPurchaseDate, CancelledOrders, WalletBalance, AbandonedCartCount)

                                     VALUES
                                     (@CustomerId, @Email, @Country, @City, @Gender, @Birthday,
                                      @LoyaltyPoints, @OrderCount, @TotalSpent, @AvgOrderValue,
                                      @LastPurchaseDate, @CancelledOrders, @WalletBalance, @AbandonedCartCount)";


                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    BindParams(cmd, profile);
                    cmd.ExecuteNonQuery();
                }

            }

        }
        
        public static void Update(CustomerProfile profile)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"UPDATE CustomerProfiles SET
                                    Email = @Email, Country = @Country, City = @City,
                                    Gender = @Gender, Birthday = @Birthday,
                                    LoyaltyPoints = @LoyaltyPoints, OrderCount = @OrderCount,
                                    TotalSpent = @TotalSpent, AvgOrderValue = @AvgOrderValue,
                                    LastPurchaseDate = @LastPurchaseDate, CancelledOrders = @CancelledOrders,
                                    WalletBalance = @WalletBalance, AbandonedCartCount = @AbandonedCartCount
                                    WHERE CustomerId = @CustomerId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    BindParams(cmd , profile);
                    cmd.ExecuteNonQuery();
                }

            }
        }
        private static CustomerProfile Map(SqlDataReader r) => new()
        {

            CustomerId         = (int)r["CustomerId"],
            Email              = r["Email"]              == DBNull.Value ? null : (string)r["Email"],
            Country            = r["Country"]            == DBNull.Value ? null : (string)r["Country"],
            City               = r["City"]               == DBNull.Value ? null : (string)r["City"],
            Gender             = r["Gender"]             == DBNull.Value ? null : ((string)r["Gender"]).Trim(),
            Birthday           = r["Birthday"]           == DBNull.Value ? null : (DateTime?)r["Birthday"],
            LoyaltyPoints      = r["LoyaltyPoints"]      == DBNull.Value ? null : (int?)r["LoyaltyPoints"],
            OrderCount         = r["OrderCount"]         == DBNull.Value ? null : (int?)r["OrderCount"],
            TotalSpent         = r["TotalSpent"]         == DBNull.Value ? null : (decimal?)r["TotalSpent"],
            AvgOrderValue      = r["AvgOrderValue"]      == DBNull.Value ? null : (decimal?)r["AvgOrderValue"],
            LastPurchaseDate   = r["LastPurchaseDate"]   == DBNull.Value ? null : (DateTime?)r["LastPurchaseDate"],
            CancelledOrders    = r["CancelledOrders"]    == DBNull.Value ? null : (int?)r["CancelledOrders"],
            WalletBalance      = r["WalletBalance"]      == DBNull.Value ? null : (decimal?)r["WalletBalance"],
            AbandonedCartCount = r["AbandonedCartCount"] == DBNull.Value ? null : (int?)r["AbandonedCartCount"],
        };

        private static void BindParams(SqlCommand cmd, CustomerProfile p)
        {
            
            cmd.Parameters.AddWithValue("@CustomerId",         p.CustomerId);
            cmd.Parameters.AddWithValue("@Email",              (object?)p.Email              ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Country",            (object?)p.Country            ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@City",               (object?)p.City               ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Gender",             (object?)p.Gender             ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Birthday",           (object?)p.Birthday           ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LoyaltyPoints",      (object?)p.LoyaltyPoints      ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@OrderCount",         (object?)p.OrderCount         ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@TotalSpent",         (object?)p.TotalSpent         ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AvgOrderValue",      (object?)p.AvgOrderValue      ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@LastPurchaseDate",   (object?)p.LastPurchaseDate   ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@CancelledOrders",    (object?)p.CancelledOrders    ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@WalletBalance",      (object?)p.WalletBalance      ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AbandonedCartCount", (object?)p.AbandonedCartCount ?? DBNull.Value);

        }
    }
}
