using BChat.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Data.DataStore
{
    public static class GroupMemberRepository
    {
        private static string _connectionString = DatabaseConfig.ConnectionString;

        public static List<GroupMember> GetAll()
        {
            List<GroupMember> groupMembers = new List<GroupMember>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT CustomerId, GroupId From CustomerGroupMembers";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            groupMembers.Add(new GroupMember
                            {
                                CustomerId = (int)reader["CustomerId"],
                                GroupId = (int)reader["GroupId"]
                            });
                        }
                        return groupMembers;
                    }
                }
            }
        }
        public static void AddMany(int customerId, List<int> groupIds)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = "INSERT INTO CustomerGroupMembers (CustomerId, GroupId)" +
                    "           VALUES (@CustomerId, @GroupId)";



                using (SqlTransaction tx = conn.BeginTransaction())
                {
                    try
                    {
                        foreach (var id in groupIds)
                        {
                            using (SqlCommand cmd = new SqlCommand(query, conn, tx))
                            {
                                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                                cmd.Parameters.AddWithValue("@GroupId", id);
                                cmd.ExecuteNonQuery();
                            }
                        }
                        tx.Commit();

                    }
                    catch 
                    {
                        tx.Rollback();
                        throw;
                    }
                }
            }

        }
    }
}
