using BChat.Events;
using BChat.Models;
using BChat.Models.Meta_Business;
using Microsoft.Data.SqlClient;

namespace BChat.Data.DataStore
{
    public static class TemplateRepository
    {
        private static string _connectionString = DatabaseConfig.ConnectionString;

        public static List<Template> GetAll()
        {
            List<Template> templates = new List<Template>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT Id, Name, Content, Category, CreatedAt, 
                         Language, HeaderType, HeaderText, ComponentsJson 
                         FROM Templates";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        templates.Add(new Template
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Content = reader.IsDBNull(2) ? "" : reader.GetString(2),
                            Category = reader.IsDBNull(3) ? null : reader.GetString(3),
                            CreatedAt = reader.GetDateTime(4),
                            Language = reader.IsDBNull(5) ? "ar" : reader.GetString(5),
                            HeaderType = reader.IsDBNull(6) ? "NONE" : reader.GetString(6),
                            HeaderText = reader.IsDBNull(7) ? "" : reader.GetString(7),
                            ComponentsJson = reader.IsDBNull(8) ? "[]" : reader.GetString(8)
                        });
                    }
                }
            }

            return templates;
        }

        public static Template GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT Id, Name, Content, Category, CreatedAt, 
                         Language, HeaderType, HeaderText, ComponentsJson 
                         FROM Templates WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Template
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Content = reader.IsDBNull(2) ? "" : reader.GetString(2),
                                Category = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CreatedAt = reader.GetDateTime(4),
                                Language = reader.IsDBNull(5) ? "ar" : reader.GetString(5),
                                HeaderType = reader.IsDBNull(6) ? "NONE" : reader.GetString(6),
                                HeaderText = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                ComponentsJson = reader.IsDBNull(8) ? "[]" : reader.GetString(8)
                            };
                        }
                    }
                }
            }

            return null;
        }

        public static void Upsert(WhatsAppTemplate t)
        {
            using var conn = new SqlConnection(DatabaseConfig.ConnectionString);
            conn.Open();

            string query = @"
    IF EXISTS (SELECT 1 FROM Templates WHERE Name = @Name)
        UPDATE Templates 
        SET [Content] = @Content, Category = @Category, 
            Language = @Language, ComponentsJson = @ComponentsJson,
            HeaderType = @HeaderType, HeaderText = @HeaderText
        WHERE Name = @Name
    ELSE
        INSERT INTO Templates (Name, [Content], Category, Language, ComponentsJson, HeaderType, HeaderText, CreatedAt)
        VALUES (@Name, @Content, @Category, @Language, @ComponentsJson, @HeaderType, @HeaderText, GETDATE())";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@Name", t.Name);
            cmd.Parameters.AddWithValue("@Content", t.BodyText);
            cmd.Parameters.AddWithValue("@Category", t.Category);
            cmd.Parameters.AddWithValue("@Language", t.Language ?? "ar");
            cmd.Parameters.AddWithValue("@ComponentsJson", t.ComponentsJson ?? "[]");
            cmd.Parameters.AddWithValue("@HeaderType", t.HeaderType ?? "NONE");
            cmd.Parameters.AddWithValue("@HeaderText", t.HeaderText ?? "");


            cmd.ExecuteNonQuery();
        }
        public static bool Add(Template template)
        {
            bool result = false;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Templates (Name, Content, Category) VALUES (@Name, @Content, @Category)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", template.Name);
                    cmd.Parameters.AddWithValue("@Content", template.Content);
                    cmd.Parameters.AddWithValue("@Category", (object)template.Category ?? DBNull.Value);

                    result = cmd.ExecuteNonQuery() > 0;
                }
            }

            AppEvents.ChangeRefreshTemplatesTable();

            return result;
        }

        public static bool Update(Template template)
        {
            bool result = false;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE Templates SET Name=@Name, Content=@Content, Category=@Category WHERE Id=@Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", template.Id);
                    cmd.Parameters.AddWithValue("@Name", template.Name);
                    cmd.Parameters.AddWithValue("@Content", template.Content);
                    cmd.Parameters.AddWithValue("@Category", (object)template.Category ?? DBNull.Value);

                    result = cmd.ExecuteNonQuery() > 0;
                }
            }

            AppEvents.ChangeRefreshTemplatesTable();

            return result;

        }

        public static bool Delete(int id)
        {
            bool result = false;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Templates WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    result = cmd.ExecuteNonQuery() > 0;
                }
            }

            AppEvents.ChangeRefreshTemplatesTable(); 

            return result;
        }
    }
}