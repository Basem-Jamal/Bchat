using Microsoft.Data.SqlClient;
using BChat.Models;

namespace BChat.Data.DataStore
{
    internal class MessageRepository
    {
        private string _connectionString = DatabaseConfig.ConnectionString;

        public List<ChatMessage> GetAll()
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

        public List<ChatMessage> GetByCustomerId(int customerId)
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

        public bool Add(ChatMessage message)
        {
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

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool UpdateStatus(int id, string status)
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
    }
}