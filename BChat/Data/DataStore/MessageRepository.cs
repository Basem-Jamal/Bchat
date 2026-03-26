using Microsoft.Data.SqlClient;
using BChat.Models;
using BChat.Events;

namespace BChat.Data.DataStore
{
    public static class MessageRepository
    {
        private static string _connectionString = DatabaseConfig.ConnectionString;


        public static List<MessageDetail> GetAllWithDetails()
        {
            var list = new List<MessageDetail>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
            SELECT 
                m.Id,
                c.Name AS CustomerName,
                c.Phone AS CustomerPhone,
                t.Name AS TemplateName,
                m.Status,
                m.TriggerType,
                m.SentAt
            FROM Messages m
            LEFT JOIN Customers c ON m.CustomerId = c.Id
            LEFT JOIN Templates t ON m.TemplateId = t.Id
            ORDER BY m.SentAt DESC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new MessageDetail
                        {
                            Id = reader.GetInt32(0),
                            CustomerName = reader.IsDBNull(1) ? "-" : reader.GetString(1),
                            CustomerPhone = reader.IsDBNull(2) ? "-" : reader.GetString(2),
                            TemplateName = reader.IsDBNull(3) ? "-" : reader.GetString(3),
                            Status = reader.GetString(4),
                            TriggerType = reader.IsDBNull(5) ? "-" : reader.GetString(5),
                            SentAt = reader.GetDateTime(6)
                        });
                    }
                }
            }

            return list;
        }
        public static List<ChatMessage> GetAll()
        {
            List<ChatMessage> messages = new List<ChatMessage>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, CustomerId, TemplateId, Status, TriggerType, SentAt FROM Messages";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        messages.Add(new ChatMessage
                        {
                            Id = reader.GetInt32(0),
                            CustomerId = reader.GetInt32(1),
                            TemplateId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                            Status = reader.GetString(3),
                            TriggerType = reader.IsDBNull(4) ? null : reader.GetString(4),
                            SentAt = reader.GetDateTime(5)
                        });
                    }
                }
            }

            return messages;
        }

        public static List<ChatMessage> GetByCustomerId(int customerId)
        {
            List<ChatMessage> messages = new List<ChatMessage>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, CustomerId, TemplateId, Status, TriggerType, SentAt FROM Messages WHERE CustomerId = @CustomerId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            messages.Add(new ChatMessage
                            {
                                Id = reader.GetInt32(0),
                                CustomerId = reader.GetInt32(1),
                                TemplateId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                                Status = reader.GetString(3),
                                TriggerType = reader.IsDBNull(4) ? null : reader.GetString(4),
                                SentAt = reader.GetDateTime(5)
                            });
                        }
                    }
                }
            }

            return messages;
        }

        public static bool Add(ChatMessage message)
        {
            bool result = false;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                
                conn.Open();
                string query = @"INSERT INTO Messages (CustomerId, TemplateId, Status, TriggerType) 
                                 VALUES (@CustomerId, @TemplateId, @Status, @TriggerType)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", message.CustomerId);
                    cmd.Parameters.AddWithValue("@TemplateId", (object)message.TemplateId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Status", message.Status);
                    cmd.Parameters.AddWithValue("@TriggerType", (object)message.TriggerType ?? DBNull.Value);

                    result = cmd.ExecuteNonQuery() > 0;
                }
            }

            AppEvents.ChangeRefreshMessagesTable();

            return result;
        }

        public static bool UpdateStatus(int id, string status)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE Messages SET Status = @Status WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Status", status);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public static bool Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Messages WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}