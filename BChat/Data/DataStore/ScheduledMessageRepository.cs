using Microsoft.Data.SqlClient;
using BChat.Models;

namespace BChat.Data.DataStore
{
    internal class ScheduledMessageRepository
    {
        private string _connectionString = DatabaseConfig.ConnectionString;

        // ── 1. جلب كل الرسائل المجدولة ─────────────────────
        public List<ScheduledMessage> GetAll()
        {
            List<ScheduledMessage> list = new List<ScheduledMessage>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT Id, CustomerId, TemplateId, SendAt, Status, TriggerEvent, CreatedAt 
                                 FROM ScheduledMessages";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new ScheduledMessage
                        {
                            Id = reader.GetInt32(0),
                            CustomerId = reader.GetInt32(1),
                            TemplateId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                            SendAt = reader.GetDateTime(3),
                            Status = reader.GetString(4),
                            TriggerEvent = reader.IsDBNull(5) ? null : reader.GetString(5),
                            CreatedAt = reader.GetDateTime(6)
                        });
                    }
                }
            }

            return list;
        }

        // ── 2. جلب الرسائل المعلقة فقط (pending) ───────────
        public List<ScheduledMessage> GetPending()
        {
            List<ScheduledMessage> list = new List<ScheduledMessage>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT Id, CustomerId, TemplateId, SendAt, Status, TriggerEvent, CreatedAt 
                                 FROM ScheduledMessages 
                                 WHERE Status = 'pending' AND SendAt <= @Now";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Now", DateTime.Now);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ScheduledMessage
                            {
                                Id = reader.GetInt32(0),
                                CustomerId = reader.GetInt32(1),
                                TemplateId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
                                SendAt = reader.GetDateTime(3),
                                Status = reader.GetString(4),
                                TriggerEvent = reader.IsDBNull(5) ? null : reader.GetString(5),
                                CreatedAt = reader.GetDateTime(6)
                            });
                        }
                    }
                }
            }

            return list;
        }

        // ── 3. إضافة رسالة مجدولة ──────────────────────────
        public bool Add(ScheduledMessage message)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO ScheduledMessages (CustomerId, TemplateId, SendAt, Status, TriggerEvent) 
                                 VALUES (@CustomerId, @TemplateId, @SendAt, @Status, @TriggerEvent)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", message.CustomerId);
                    cmd.Parameters.AddWithValue("@TemplateId", (object)message.TemplateId ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@SendAt", message.SendAt);
                    cmd.Parameters.AddWithValue("@Status", message.Status);
                    cmd.Parameters.AddWithValue("@TriggerEvent", (object)message.TriggerEvent ?? DBNull.Value);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ── 4. تحديث حالة الرسالة المجدولة ────────────────
        public bool UpdateStatus(int id, string status)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE ScheduledMessages SET Status = @Status WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Status", status);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ── 5. إلغاء رسالة مجدولة ──────────────────────────
        public bool Cancel(int id)
        {
            return UpdateStatus(id, "cancelled");
        }
    }
}