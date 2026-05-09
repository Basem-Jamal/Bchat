using BChat.Events;
using BChat.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Data.DataStore.Customers_Repository
{
    public static class CustomerRepository
    {
        private static string _connectionString = DatabaseConfig.ConnectionString;

        public static List<Customer> GetAll()
        {
            List<Customer> customers = new List<Customer>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Name, Phone, CreatedAt, IsBlocked FROM Customers ORDER BY CreatedAt ASC";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        customers.Add(Map(reader));
                }
            }

            return customers;
        }

        public static Customer GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Id, Name, Phone, CreatedAt, IsBlocked FROM Customers WHERE Id = @Id";

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

        public static int Add(Customer customer)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO Customers (Name, Phone) VALUES (@Name, @Phone); SELECT SCOPE_IDENTITY()";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", customer.Name);
                    cmd.Parameters.AddWithValue("@Phone", customer.Phone);

                    try
                    {
                        int newId = Convert.ToInt32(cmd.ExecuteScalar());
                        customer.Id = newId;
                        AppEvents.ChangeRefreshCustomesTable();
                        AppEvents.NotifyCustomerAdded(customer);
                        return newId;
                    }
                    catch (SqlException ex) when (ex.Number == 2627)
                    {
                        return -1;
                    }
                }
            }
        }

        public static int AddIfNotExists(Customer customer)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string checkQuery = "SELECT Id FROM Customers WHERE Phone = @Phone";
                using (SqlCommand checkCmd = new SqlCommand(checkQuery, conn))
                {
                    checkCmd.Parameters.AddWithValue("@Phone", customer.Phone);
                    var existing = checkCmd.ExecuteScalar();
                    if (existing != null)
                        return Convert.ToInt32(existing);
                }

                string insertQuery = @"INSERT INTO Customers (Name, Phone, CreatedAt)
                                       VALUES (@Name, @Phone, @CreatedAt);
                                       SELECT SCOPE_IDENTITY()";

                using (SqlCommand cmd = new SqlCommand(insertQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", customer.Name);
                    cmd.Parameters.AddWithValue("@Phone", customer.Phone);
                    cmd.Parameters.AddWithValue("@CreatedAt", customer.CreatedAt);
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public static bool Delete(int Id)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                using (SqlTransaction trans = conn.BeginTransaction())
                {
                    try
                    {
                        string queryDeleteMember = "DELETE FROM CustomerGroupMembers WHERE CustomerId = @Id";
                        using (SqlCommand cmd = new SqlCommand(queryDeleteMember, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@Id", Id);
                            cmd.ExecuteNonQuery();
                        }

                        string queryDeleteCustomer = "DELETE FROM Customers WHERE Id = @Id";
                        using (SqlCommand cmd = new SqlCommand(queryDeleteCustomer, conn, trans))
                        {
                            cmd.Parameters.AddWithValue("@Id", Id);
                            bool result = cmd.ExecuteNonQuery() > 0;
                            trans.Commit();
                            AppEvents.ChangeRefreshCustomesTable();
                            return result;
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

        public static void Update(Customer customer)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"UPDATE Customers SET
                               Name = @Name,
                               Phone = @Phone,
                               IsBlocked = @IsBlocked
                               WHERE Id = @Id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", customer.Id);
                    BindParams(cmd, customer);
                    cmd.ExecuteNonQuery();
                }
            }

            AppEvents.ChangeRefreshCustomesTable();
        }

        public static void Block(int customerId, bool isBlocked)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE Customers SET IsBlocked = @IsBlocked WHERE Id = @Id";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", customerId);
                    cmd.Parameters.AddWithValue("@IsBlocked", isBlocked);
                    cmd.ExecuteNonQuery();
                }
            }
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

        private static Customer Map(SqlDataReader r)
        {
            return new Customer
            {
                Id = r.GetInt32(0),
                Name = r.GetString(1),
                Phone = r.GetString(2),
                CreatedAt = r.GetDateTime(3),
                IsBlocked = r.GetBoolean(4)
            };
        }

        private static void BindParams(SqlCommand cmd, Customer customer)
        {
            cmd.Parameters.AddWithValue("@Name", customer.Name);
            cmd.Parameters.AddWithValue("@Phone", customer.Phone);
            cmd.Parameters.AddWithValue("@IsBlocked", customer.IsBlocked);
        }
    }
}