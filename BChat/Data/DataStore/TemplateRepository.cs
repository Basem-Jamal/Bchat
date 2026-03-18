using Microsoft.Data.SqlClient;
using BChat.Models;

namespace BChat.Data.DataStore
{
    internal class TemplateRepository
    {
        private string _connectionString = DatabaseConfig.ConnectionString;

        public List<Template> GetAll()
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

        public Template GetById(int id)
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

        public bool Add(Template template)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Templates (Name, Content, Category) VALUES (@Name, @Content, @Category)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", template.Name);
                    cmd.Parameters.AddWithValue("@Content", template.Content);
                    cmd.Parameters.AddWithValue("@Category", (object)template.Category ?? DBNull.Value);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Update(Template template)
        {
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

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        public bool Delete(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Templates WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}