using BChat.Events;
using BChat.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Data.DataStore
{
    public static class CustomerRepository
    {
        private static string _connectionString = DatabaseConfig.ConnectionString;

        public static List<Customer> GetAll()
        {
            List <Customer> customers = new List<Customer>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT Id, Name, Phone, CreatedAt FROM Customers";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        customers.Add(new Customer
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                            Phone = reader.GetString(2),
                            CreatedAt = reader.GetDateTime(3)
                        });
                    }
                }
            }

            return customers;

        }

        public static Customer GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Name, Phone, CreatedAt FROM Customers WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Customer
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Phone = reader.GetString(2),
                                CreatedAt = reader.GetDateTime(3)
                            };
                        }
                    }
                }
            }

            return null;
        }

        public static bool Add(Customer customer)
        {
            bool Status = false;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Customers (Name, Phone) VALUES (@Name, @Phone)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", customer.Name);
                    cmd.Parameters.AddWithValue("@Phone", customer.Phone);

                    try
                    {
                        cmd.ExecuteNonQuery();
                        AppEvents.ChangeRefreshCustomesTable();
                        return true;

                    }
                    catch (SqlException ex) when (ex.Number == 2627)
                    {
                        return false;

                    }
                }
            }
        }

        public static bool Delete(int id)
        {
            bool result = false;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "DELETE FROM Customers WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                   result = cmd.ExecuteNonQuery() > 0;

                }
            }

            AppEvents.ChangeRefreshCustomesTable();
            
            return result;
        }

        public static void Update(Customer customer)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"UPDATE Customers SET
                               Name = @Name,                                                           
                               Phone= @Phone
                               WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", customer.Id);
                    cmd.Parameters.AddWithValue("@Name", customer.Name);
                    cmd.Parameters.AddWithValue("@Phone", customer.Phone);

                    cmd.ExecuteNonQuery();
                }
            }

            AppEvents.ChangeRefreshCustomesTable();
        }

        public static bool TestConnection()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}