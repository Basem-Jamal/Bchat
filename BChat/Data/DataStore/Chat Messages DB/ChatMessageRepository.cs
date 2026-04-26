using BChat.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace BChat.Data.DataStore
{
    public static class ChatMessageRepository
    {
        private static string _connectionString = DatabaseConfig.ConnectionString;

        // ── جلب كل الرسائل (للكاش) ──────────────────────────────

        public static List<ChatMessage> GetAll()
        {
            var listMessage = new List<ChatMessage>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"SELECT Id, CustomerId, Text, SentAt, IsSent, IsRead, 
                                    HasAttachment, AttachmentName, AttachmentSize, 
                                    AttachmentType, AttachmentUrl, WhatsAppMessageId, Status, SentByUserId
                                FROM ChatMessage
                                ORDER BY SentAt ASC";


                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        listMessage.Add(Map(reader));
                }
            }

            return listMessage;

        }


        // ── جلب رسائل عميل معين ──────────────────────────────────
        public static List<ChatMessage> GetByCustomerId(int customerId)
        {
            var listMessage = new List<ChatMessage>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"SELECT Id, CustomerId, Text, SentAt, IsSent, IsRead, 
                                    HasAttachment, AttachmentName, AttachmentSize, 
                                    AttachmentType, AttachmentUrl, WhatsAppMessageId, Status, SentByUserId
                                 FROM ChatMessage
                                 WHERE CustomerId = @CustomerId
                                 ORDER BY SentAt ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@CustomerId", customerId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            listMessage.Add(Map(reader));
                    }
                }
            }

            return listMessage;
        }

        // ── إضافة رسالة وإرجاع ID الجديد ─────────────────────────

        public static int Add(ChatMessage msg)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO ChatMessage

                                  (CustomerId, Text, SentAt, IsSent, IsRead,
                                     HasAttachment, AttachmentName, AttachmentSize,
                                     AttachmentType, AttachmentUrl, WhatsAppMessageId, Status, SentByUserId)
                                    VALUES
                                    (@CustomerId, @Text, @SentAt, @IsSent, @IsRead,
                                     @HasAttachment, @AttachmentName, @AttachmentSize,
                                     @AttachmentType, @AttachmentUrl, @WhatsAppMessageId, @Status, @SentByUserId);
                                 SELECT SCOPE_IDENTITY()";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    BindParams (cmd, msg);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }
        // ── تحديث حالة القراءة ────────────────────────────────────

        public static void MarkAsRead(int customerId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"UPDATE ChatMessage 
                                 SET IsRead = 1
                                 WHERE CustomerId = @CustomerId And IsRead = 0";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {

                    cmd.Parameters.AddWithValue("@CustomerId", customerId);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        // ── تحديث Status ──────────────────────────────────────────

        public static void UpdateStatus(int Id, string status)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = "UPDATE ChatMessage SET Status = @Status WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@Id", Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        

        // ── Helpers ───────────────────────────────────────────────
        private static ChatMessage Map(SqlDataReader r)
        {
            return new ChatMessage
            {
                Id = r.GetInt32(0),
                CustomerId = r.GetInt32(1),
                Text = r.IsDBNull(2) ? null : r.GetString(2),
                SentAt = r.GetDateTime(3),
                IsSent = r.GetBoolean(4),
                IsRead = r.GetBoolean(5),
                HasAttachment = r.GetBoolean(6),
                AttachmentName = r.IsDBNull(7) ? null : r.GetString(7),
                AttachmentSize = r.IsDBNull(8) ? null : r.GetInt64(8),
                AttachmentType = r.IsDBNull(9) ? null : r.GetString(9),
                AttachmentUrl = r.IsDBNull(10) ? null : r.GetString(10),
                WhatsAppMessageId = r.IsDBNull(11) ? null : r.GetString(11),
                Status = r.IsDBNull(12) ? null : r.GetString(12),
                SentByUserId = r.IsDBNull(13) ? null : r.GetInt32(13)

            };


        }

        private static void BindParams(SqlCommand cmd, ChatMessage msg)
        {
            cmd.Parameters.AddWithValue("@CustomerId", msg.CustomerId);
            cmd.Parameters.AddWithValue("@Text", (object?)msg.Text ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SentAt", msg.SentAt);
            cmd.Parameters.AddWithValue("@IsSent", msg.IsSent);
            cmd.Parameters.AddWithValue("@IsRead", msg.IsRead);
            cmd.Parameters.AddWithValue("@HasAttachment", msg.HasAttachment);
            cmd.Parameters.AddWithValue("@AttachmentName", (object?)msg.AttachmentName ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AttachmentSize", (object?)msg.AttachmentSize ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AttachmentType", (object?)msg.AttachmentType ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@AttachmentUrl", (object?)msg.AttachmentUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@WhatsAppMessageId", (object?)msg.WhatsAppMessageId ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Status", (object?)msg.Status ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@SentByUserId", (object?)msg.SentByUserId ?? DBNull.Value);

        }


    }
}

