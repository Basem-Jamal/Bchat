using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Data.DataStore.Chat_Messages_DB
{
    public static class ConversationAssignmentRepository
    {
        private static string _connectionString = DatabaseConfig.ConnectionString;


        public static void Assign(int customerId, int AssignedToUserId, int AssignedByUserId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {

                conn.Open();


                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string queryDeactivate = "UPDATE ConversationAssignments SET IsActive = 0 WHERE CustomerId =@CustomerId";
                        using (SqlCommand cmd = new SqlCommand(queryDeactivate, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@CustomerId", customerId);
                            cmd.ExecuteNonQuery();
                        }

                        string queryInsert = @"INSERT INTO ConversationAssignments
                                     (CustomerId, AssignedToUserId, AssignedByUserId, AssignedAt, IsActive)
                                     VALUES (@CustomerId, @AssignedToUserId, @AssignedByUserId, GETDATE(),1)";

                        using (SqlCommand cmd = new SqlCommand(@queryInsert, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@CustomerId", customerId);
                            cmd.Parameters.AddWithValue("@AssignedToUserId", AssignedToUserId);
                            cmd.Parameters.AddWithValue("@AssignedByUserId", AssignedByUserId);
                            cmd.ExecuteNonQuery();

                            trans.Commit();

                        }



                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }

            }
        }


        public static List<int> GetAssignedCustomerIds(int userId)
        {
            var list = new List<int>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"SELECT CustomerId FROM ConversationAssignments WHERE AssignedToUserId =@UserId AND IsActive = 1";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserId", userId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            list.Add((int)reader["CustomerId"]);
                    }
                }

            }

            return list;
        }
    }
    
}
