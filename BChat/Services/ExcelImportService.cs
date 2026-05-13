using BChat.Data.DataStore;
using BChat.Data.DataStore.CustomerProfile_Repository;
using BChat.Data.DataStore.Customers_Repository;
using BChat.Models;
using BChat.Models.Customer_Module.CustomerProfiles_Module;
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
        public static async Task<(int added, int skipped)> ImportCustomersAsync(
            string filePath, IProgress<(int current, int total)> progress)
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
                int total = table.Rows.Count;
                int current = 0;

                foreach (DataRow row in table.Rows)
                {
                    current++;
                    progress?.Report((current, total));

                    string name = row["Full_Name"]?.ToString()?.Trim() ?? "";
                    string phone = row["Mobile"]?.ToString()?.Trim() ?? "";

                    phone = phone.Replace("+", "").Replace(" ", "").Trim();
                    if (phone.StartsWith("05")) phone = "966" + phone.Substring(1);
                    if (phone.StartsWith("5") && phone.Length == 9) phone = "966" + phone;

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

                    customer.CreatedAt = DateTime.Now;
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

                    GroupMemberRepository.AddIfNotExists(6, customerId);

                    var profile = new CustomerProfile
                    {
                        CustomerId = customerId,
                        Email = table.Columns.Contains("Email") ? row["Email"]?.ToString()?.Trim() : null,
                        Country = table.Columns.Contains("Country") ? row["Country"]?.ToString()?.Trim() : null,
                        Gender = table.Columns.Contains("Gender") && row["Gender"]?.ToString()?.Trim()?.Length > 0
                           ? row["Gender"].ToString().Trim()[0].ToString().ToUpper() : null,
                        Birthday = table.Columns.Contains("Birthday") && DateTime.TryParse(row["Birthday"]?.ToString(), out var bd) ? bd : null,
                        LoyaltyPoints = table.Columns.Contains("Loyalty_Points") && int.TryParse(row["Loyalty_Points"]?.ToString(), out var lp) ? lp : null,
                        OrderCount = table.Columns.Contains("Order_Count") && int.TryParse(row["Order_Count"]?.ToString(), out var oc) ? oc : null,
                        TotalSpent = table.Columns.Contains("Total_Spent") && decimal.TryParse(row["Total_Spent"]?.ToString(), out var ts) ? ts : null,
                        AvgOrderValue = table.Columns.Contains("Avg_Order_Value") && decimal.TryParse(row["Avg_Order_Value"]?.ToString(), out var av) ? av : null,
                        LastPurchaseDate = table.Columns.Contains("Last_Purchase_Date") && DateTime.TryParse(row["Last_Purchase_Date"]?.ToString(), out var ld) ? ld : null,
                        CancelledOrders = table.Columns.Contains("Cancelled_Orders") && int.TryParse(row["Cancelled_Orders"]?.ToString(), out var co) ? co : null,
                        WalletBalance = table.Columns.Contains("Wallet_Balance") && decimal.TryParse(row["Wallet_Balance"]?.ToString(), out var wb) ? wb : null,
                        AbandonedCartCount = table.Columns.Contains("Abandoned_Cart_Count") && int.TryParse(row["Abandoned_Cart_Count"]?.ToString(), out var ac) ? ac : null,
                    };

                    var existingProfile = CustomerProfileRepository.GetByCusotmerId(customerId);
                    if (existingProfile == null)
                        CustomerProfileRepository.Add(profile);

                    // وقفته مؤقتاً عشان اضيف عملاء خدمة العملاء
                    // لكن عند التجديد واضافة عملاء المتجر يفضل تفعيله عشان يضيف المبيعات والتفاصيل حق العميل
                    // else
                    //     CustomerProfileRepository.Update(profile);

                    added++;
                }

                return (added, skipped);
            }
        }
    }
}
