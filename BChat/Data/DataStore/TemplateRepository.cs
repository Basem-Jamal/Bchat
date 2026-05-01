using BChat.Events;
using BChat.Models.Meta_Business;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace BChat.Data.DataStore
{
    public static class TemplateRepository
    {
        private static string _connectionString = DatabaseConfig.ConnectionString;

        // ── جلب كل القوالب ───────────────────────────────────────────────────
        public static List<WhatsAppTemplate> GetAll()
        {
            var templates = new List<WhatsAppTemplate>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT Id, Name, Content, Category, CreatedAt, 
                                        Language, HeaderType, HeaderText, ComponentsJson, MetaTemplateId
                                 FROM Templates";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        templates.Add(Map(reader));
                }
            }

            return templates;
        }

        // ── جلب قالب بالـ ID ─────────────────────────────────────────────────
        public static WhatsAppTemplate GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT Id, Name, Content, Category, CreatedAt, 
                                        Language, HeaderType, HeaderText, ComponentsJson, MetaTemplateId
                                 FROM Templates WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) return Map(reader);
                    }
                }
            }

            return null;
        }

        // ── مزامنة من Meta (يضيف أو يحدث) ───────────────────────────────────
        public static void Upsert(WhatsAppTemplate t)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"
                    IF EXISTS (SELECT 1 FROM Templates WHERE Name = @Name)
                        UPDATE Templates 
                        SET [Content]      = @Content,
                            Category       = @Category,
                            Language       = @Language,
                            ComponentsJson = @ComponentsJson,
                            HeaderType     = @HeaderType,
                            HeaderText     = @HeaderText,
                            MetaTemplateId = @MetaTemplateId
                        WHERE Name = @Name
                    ELSE
                        INSERT INTO Templates 
                            (Name, [Content], Category, Language, ComponentsJson, HeaderType, HeaderText, MetaTemplateId, CreatedAt)
                        VALUES 
                            (@Name, @Content, @Category, @Language, @ComponentsJson, @HeaderType, @HeaderText, @MetaTemplateId, GETDATE())";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    BindParams(cmd, t);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // ── حذف ──────────────────────────────────────────────────────────────
        public static bool Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Templates WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    bool result = cmd.ExecuteNonQuery() > 0;
                    AppEvents.ChangeRefreshTemplatesTable();
                    return result;
                }
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private static WhatsAppTemplate Map(SqlDataReader r) => new WhatsAppTemplate
        {
            Id = r.GetInt32(0),
            Name = r.GetString(1),
            BodyText = r.IsDBNull(2) ? "" : r.GetString(2),
            Category = r.IsDBNull(3) ? null : r.GetString(3),
            CreatedAt = r.GetDateTime(4),
            Language = r.IsDBNull(5) ? "ar" : r.GetString(5),
            HeaderType = r.IsDBNull(6) ? "NONE" : r.GetString(6),
            HeaderText = r.IsDBNull(7) ? "" : r.GetString(7),
            ComponentsJson = r.IsDBNull(8) ? "[]" : r.GetString(8),
            MetaTemplateId = r.IsDBNull(9) ? "" : r.GetString(9),
        };

        private static void BindParams(SqlCommand cmd, WhatsAppTemplate t)
        {
            cmd.Parameters.AddWithValue("@Name", t.Name);
            cmd.Parameters.AddWithValue("@Content", (object)t.BodyText ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Category", (object)t.Category ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@Language", t.Language ?? "ar");
            cmd.Parameters.AddWithValue("@ComponentsJson", t.ComponentsJson ?? "[]");
            cmd.Parameters.AddWithValue("@HeaderType", t.HeaderType ?? "NONE");
            cmd.Parameters.AddWithValue("@HeaderText", t.HeaderText ?? "");
            cmd.Parameters.AddWithValue("@MetaTemplateId", (object)t.MetaTemplateId ?? DBNull.Value);
        }
    }
} 