using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Data.DataStore
{
    public static class CustomerGroupRepository
    {
        private static string _connectionString = DatabaseConfig.ConnectionString;

        public static List<Models.CustomerGroups> GetAll()
        {
            List<Models.CustomerGroups> groups = new List<Models.CustomerGroups>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"SELECT Id,Name,Description , IconBase64, IconColor , IsActive FROM CustomerGroups  WHERE IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        groups.Add(new Models.CustomerGroups
                        {
                            Id = Convert.ToInt32(reader["Id"]),
                            Name = reader["Name"] != DBNull.Value ? reader["Name"].ToString() : "NULL",

                            Description = reader["Description"] != DBNull.Value ? reader["Description"].ToString() : "NULL",

                            Icon = reader["IconBase64"] != DBNull.Value ? reader["IconBase64"].ToString() :  "NULL",

                            IconBoxColor = reader["IconColor"] != DBNull.Value ? reader["IconColor"].ToString() : "NULL",

                            Status = (Models.statusGroups)Convert.ToInt32(reader["IsActive"])
                        });
                    }
                }
            }

            return groups;
        }

        public static bool Add(Models.CustomerGroups group)
        {
            bool result = false;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"INSERT INTO CustomerGroups (Name, Description, IconBase64, IconColor ,IsActive) VALUES (@Name, @Description, @IconBase64, @IconColor, @IsActive)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", group.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", group.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", (int)group.Status);
                    cmd.Parameters.AddWithValue("@IconBase64", group.Icon != null ? (group.Icon) : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IconColor", group.IconBoxColor != null ? group.IconBoxColor.ToString() : (object)DBNull.Value);
                    result = cmd.ExecuteNonQuery() > 0;
                }
            }
            return result;
        }

        public static bool Update(Models.CustomerGroups group)
        {
            bool result = false;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"UPDATE CustomerGroups SET 
                                            Name = @Name,
                                            Description = @Description,
                                            IconBase64  = @IconBase64,
                                            IconColor = @IconColor,
                                            IsActive = @IsActive
                                            WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", group.Id);
                    cmd.Parameters.AddWithValue("@Name", group.Name ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Description", group.Description ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IconBase64", group.Icon != null ? (group.Icon) : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IconColor", group.IconBoxColor != null ? group.IconBoxColor.ToString() : (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", (int)group.Status);

                    result = cmd.ExecuteNonQuery() > 0;
                }
            }
            return result;

        }

        public static bool Delete(int id)
        {
            bool result = false;

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"UPDATE CustomerGroups SET
                                        IsActive = 0 
                                        WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    result = cmd.ExecuteNonQuery() > 0;
                }
            }
            return result;
        }
    }
}