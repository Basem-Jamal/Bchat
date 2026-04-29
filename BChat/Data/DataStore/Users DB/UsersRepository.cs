using BChat.Events;
using BChat.Models.Users;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using static BChat.Models.Users.ModulePermission.Permission;

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

        public static bool Update(User user)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string queryUpdate = @"UPDATE Users SET
                                 Name = @Name,
                                 Email= @Email,
                                 Password = @Password,
                                 Role     = @Role,
                                 BranchId = @BranchId,
                                 IsActive = @IsActive
                              WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(queryUpdate, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", user.Id);
                    cmd.Parameters.AddWithValue("@Name", user.Name);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@Password", user.Password);
                    cmd.Parameters.AddWithValue("@Role", user.Role);
                    cmd.Parameters.AddWithValue("@BranchId", user.BranchId ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@IsActive", user.IsActive);

                    int rowsAffected = cmd.ExecuteNonQuery();

                    if (rowsAffected >= 1)
                    {

                        return true;
                    }

                    
                }

                return false;
            }
        }

    }
}