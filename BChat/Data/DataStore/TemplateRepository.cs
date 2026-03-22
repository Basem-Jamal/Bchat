using Microsoft.Data.SqlClient;
using BChat.Models;
using BChat.Events;

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
                string query = "SELECT Id, Name, Content, Category, CreatedAt FROM Templates";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        templates.Add(new Template
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Content = reader.GetString(2),
                            Category = reader.IsDBNull(3) ? null : reader.GetString(3),
                            CreatedAt = reader.GetDateTime(4)
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
                string query = "SELECT Id, Name, Content, Category, CreatedAt FROM Templates WHERE Id = @Id";

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
                                Content = reader.GetString(2),
                                Category = reader.IsDBNull(3) ? null : reader.GetString(3),
                                CreatedAt = reader.GetDateTime(4)
                            };
                        }
                    }
                }
            }

            return null;
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