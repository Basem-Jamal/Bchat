using Microsoft.Data.SqlClient;
using BChat.Models;

namespace BChat.Data.DataStore
{
    internal class OrderRepository
    {
        private string _connectionString = DatabaseConfig.ConnectionString;

        // ── 1. جلب كل الطلبات ──────────────────────────────
        public List<Order> GetAll()
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, CustomerId, OrderNumber, Status, Total, CreatedAt FROM Orders";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        orders.Add(new Order
                        {
                            Id = reader.GetInt32(0),
                            CustomerId = reader.GetInt32(1),
                            OrderNumber = reader.GetString(2),
                            Status = reader.GetString(3),
                            Total = reader.GetDecimal(4),
                            CreatedAt = reader.GetDateTime(5)
                        });
                    }
                }
            }

            return orders;
        }

        // ── 2. جلب طلبات عميل معين ─────────────────────────
        public List<Order> GetByCustomerId(int customerId)
        {
            List<Order> orders = new List<Order>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT Id, CustomerId, OrderNumber, Status, Total, CreatedAt 
                                 FROM Orders WHERE CustomerId = @CustomerId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            orders.Add(new Order
                            {
                                Id = reader.GetInt32(0),
                                CustomerId = reader.GetInt32(1),
                                OrderNumber = reader.GetString(2),
                                Status = reader.GetString(3),
                                Total = reader.GetDecimal(4),
                                CreatedAt = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }

            return orders;
        }

        // ── 3. إضافة طلب جديد ──────────────────────────────
        public bool Add(Order order)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO Orders (CustomerId, OrderNumber, Status, Total) 
                                 VALUES (@CustomerId, @OrderNumber, @Status, @Total)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", order.CustomerId);
                    cmd.Parameters.AddWithValue("@OrderNumber", order.OrderNumber);
                    cmd.Parameters.AddWithValue("@Status", order.Status);
                    cmd.Parameters.AddWithValue("@Total", order.Total);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ── 4. تحديث حالة الطلب ────────────────────────────
        public bool UpdateStatus(int id, string status)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE Orders SET Status = @Status WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Status", status);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}