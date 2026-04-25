using BChat.Models.Users;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;

namespace BChat.Data.DataStore.Users_DB
{
    public class UsersRepository
    {
        private static string _connectionString = DatabaseConfig.ConnectionString;

        // جلب كل المستخدمين
        public static List<User> GetAll()
        {
            var list = new List<User>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Name, Email, Password, Role, BranchId, IsActive, CreatedAt FROM Users";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new User
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3),
                                Role = reader.GetString(4),
                                BranchId = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                                IsActive = reader.GetBoolean(6),
                                CreatedAt = reader.GetDateTime(7)
                            });
                        }
                    }
                }
            }
            return list;
        }

        // تسجيل الدخول
        public static User? Login(string email, string password)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Name, Email, Password, Role, BranchId, IsActive, CreatedAt FROM Users WHERE Email = @Email AND Password = @Password AND IsActive = 1";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3),
                                Role = reader.GetString(4),
                                BranchId = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                                IsActive = reader.GetBoolean(6),
                                CreatedAt = reader.GetDateTime(7)
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}