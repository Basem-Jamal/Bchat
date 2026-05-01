using BChat.Data.DataStore.CustomerProfile_Repository;
using BChat.Data.DataStore.Customers_Repository;
using BChat.Models;
using BChat.Models.CustomerProfiles_Module;
using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Services
{
    public class ExcelImportService
    {
        public static (int added, int skipped) ImportCustomers(string filePath)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            
            int added = 0, skipped = 0;

            using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
            {
                var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    ConfigureDataTable = _ => new ExcelDataTableConfiguration()
                    {
                        UseHeaderRow = true,
                    }
                });

                var table = dataSet.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    string name = row["Full_Name"]?.ToString()?.Trim() ?? "";
                    string phone = row["Mobile"]?.ToString()?.Trim() ?? "";

                    if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(phone))
                    {
                        skipped++;
                        continue;
                    }

                    var customer = new Customer
                    {
                        Name = name,
                        Phone = phone,
                    };

                    customer.CreatedAt = DateTime.Now; // مؤقتاً
                    if (table.Columns.Contains("Created_At"))
                    {
                        string createdAtStr = row["Created_At"]?.ToString()?.Trim() ?? "";
                        if (DateTime.TryParse(createdAtStr, out DateTime createdAt))
                            customer.CreatedAt = createdAt;
                    }

                    int customerId = CustomerRepository.AddIfNotExists(customer);

                    if (customerId <= 0)
                    {
                        skipped++;
                        continue;
                    }

               


                    var profile = new CustomerProfile
                    {
                        CustomerId = customerId,
                        Email = row["Email"]?.ToString()?.Trim(),
                        Country = row["Country"]?.ToString()?.Trim(),

                        Gender = row["Gender"]?.ToString()?.Trim()?.Length > 0 
                                 ? row["Gender"].ToString().Trim()[0].ToString().ToUpper() 
                                 : null,     
                        
                        Birthday = DateTime.TryParse(row["Birthday"]?.ToString(), out var bd) ? bd : null,
                        LoyaltyPoints = int.TryParse(row["Loyalty_Points"]?.ToString(), out var lp) ? lp : null,
                        OrderCount = int.TryParse(row["Order_Count"]?.ToString(), out var oc) ? oc : null,
                        TotalSpent = decimal.TryParse(row["Total_Spent"]?.ToString(), out var ts) ? ts : null,
                        AvgOrderValue = decimal.TryParse(row["Avg_Order_Value"]?.ToString(), out var av) ? av : null,
                        LastPurchaseDate = DateTime.TryParse(row["Last_Purchase_Date"]?.ToString(), out var ld) ? ld : null,
                        CancelledOrders = int.TryParse(row["Cancelled_Orders"]?.ToString(), out var co) ? co : null,
                        WalletBalance = decimal.TryParse(row["Wallet_Balance"]?.ToString(), out var wb) ? wb : null,
                        AbandonedCartCount = int.TryParse(row["Abandoned_Cart_Count"]?.ToString(), out var ac) ? ac : null,
                    };
                    

                    var existingProfile = CustomerProfileRepository.GetByCusotmerId(customerId);
                    if (existingProfile == null)
                        CustomerProfileRepository.Add(profile);
                    else
                        CustomerProfileRepository.Update(profile);
                    
                    
                    added++;



                }

                return (added , skipped);
            }
        }

    }
}
