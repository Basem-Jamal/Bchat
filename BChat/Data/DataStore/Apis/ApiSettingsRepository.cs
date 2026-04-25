using BChat.Models.Apis;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Data.DataStore.Apis
{
    public static class ApiSettingsRepository
    {
        private static string _connectionString = DatabaseConfig.ConnectionString;
        public static List<ApiSetting> GetByProvider(string provider)
        {
            var list = new List<ApiSetting>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT Id, Provider, [Key], Value From ApiSettings WHERE Provider = @Provider";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Provider", provider);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new ApiSetting
                            {
                                Id = reader.GetInt32(0),
                                Provider = reader.GetString(1),
                                Key = reader.GetString(2),
                                Value = reader.IsDBNull(3) ? "" : reader.GetString(3)
                            });
                        }
                    }
                }
            }

            return list;
        }

        public static void UpdateValue(string provider, string key, string value)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "UPDATE ApiSettings SET Value = @Value WHERE Provider = @Provider AND [Key] = @Key";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Value", value);
                    cmd.Parameters.AddWithValue("@Provider", provider);
                    cmd.Parameters.AddWithValue("@Key", key);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void Add(ApiSetting setting)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "INSERT INTO ApiSettings (Provider, [Key], Value) VALUES (@Provider, @Key, @Value)";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Provider", setting.Provider);
                    cmd.Parameters.AddWithValue("@Key", setting.Key);
                    cmd.Parameters.AddWithValue("@Value", setting.Value);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static string GetValue(string provider, string kye)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = "SELECT Value FROM ApiSettings WHERE Provider = @Provider AND [Key] = @Key";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Provider", provider);
                    cmd.Parameters.AddWithValue("@Key", kye);
                    var result = cmd.ExecuteScalar();
                    return result == null ? "" : result.ToString() ?? "";
                }
            }
        }
    }
}