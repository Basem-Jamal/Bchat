using BChat.Models;
using Microsoft.Data.SqlClient;
using System;

namespace BChat.Data.DataStore
{
    public static class StoreTokenRepository
    {
        private static string _connectionString = DatabaseConfig.ConnectionString;

        public static StoreToken GetByStoreId(string storeId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT Id, StoreId, AccessToken, RefreshToken, ExpiresAt, CreatedAt, UpdatedAt 
                                 FROM StoreTokens WHERE StoreId = @StoreId";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StoreId", storeId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new StoreToken
                            {
                                Id = reader.GetInt32(0),
                                StoreId = reader.GetString(1),
                                AccessToken = reader.GetString(2),
                                RefreshToken = reader.IsDBNull(3) ? null : reader.GetString(3),
                                ExpiresAt = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
                                CreatedAt = reader.GetDateTime(5),
                                UpdatedAt = reader.GetDateTime(6)
                            };
                        }
                    }
                }
            }

            return null;
        }

        public static void Save(StoreToken token)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                // لو موجود يحدّث، لو مش موجود يضيف
                string query = @"
                    IF EXISTS (SELECT 1 FROM StoreTokens WHERE StoreId = @StoreId)
                        UPDATE StoreTokens SET
                            AccessToken  = @AccessToken,
                            RefreshToken = @RefreshToken,
                            ExpiresAt    = @ExpiresAt,
                            UpdatedAt    = GETDATE()
                        WHERE StoreId = @StoreId
                    ELSE
                        INSERT INTO StoreTokens (StoreId, AccessToken, RefreshToken, ExpiresAt)
                        VALUES (@StoreId, @AccessToken, @RefreshToken, @ExpiresAt)";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@StoreId", token.StoreId);
                    cmd.Parameters.AddWithValue("@AccessToken", token.AccessToken);
                    cmd.Parameters.AddWithValue("@RefreshToken", (object)token.RefreshToken ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@ExpiresAt", (object)token.ExpiresAt ?? DBNull.Value);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static string GetAccessToken(string storeId)
        {
            var token = GetByStoreId(storeId);
            return token?.AccessToken;
        }
    }
}