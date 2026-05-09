using BChat.Models;
using BChat.Models.Campaign_Module;
using BChat.Models.Campaign_Modules;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace BChat.Data.DataStore.Campaigns_Repository
{
    public static class CampaignMessageRepository
    {

        private static string _connectionString = DatabaseConfig.ConnectionString;


        public static List<CampaignMessage> GetAll()
        {
            List <CampaignMessage> list = new List<CampaignMessage>();
            
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"SELECT * FROM CampaignMessages";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(Map(reader));
                    }
                }

            }
            return list;
        }

        public static List<int> GetRecentlySentCustomerIds()
        {
            var ids = new List<int>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();
                string query = @"SELECT DISTINCT CustomerId FROM CampaignMessages 
                         WHERE Status = 'Completed' 
                         AND SentAt >= DATEADD(DAY, -7, GETDATE())";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        ids.Add(Convert.ToInt32(reader["CustomerId"]));
                }
            }
            return ids;
        }
        public static int Add(CampaignMessage message)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO CampaignMessages 
                                 (CustomerId ,CampaignId ,SentAt ,Status)
                                OUTPUT INSERTED.Id
                                VALUES
                                 (@CustomerId ,@CampaignId ,@SentAt ,@Status)";



                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    BindParams(cmd, message);

                    var result = cmd.ExecuteScalar();
                    return result != null ? (int)result : -1;

                }
            }
        }

        private static CampaignMessage Map(SqlDataReader r)
        {
            return new CampaignMessage()
            {
                Id = Convert.ToInt32(r["Id"]),
                CustomerId = Convert.ToInt32(r["CustomerId"]),
                CampaignId = Convert.ToInt32(r["CampaignId"]),
                SentAt = Convert.ToDateTime(r["SentAt"]),
                Status = Enum.Parse<CampaignMessageStatus>(r["Status"].ToString()),

            };
        }
        private static void BindParams(SqlCommand cmd, CampaignMessage message)
        {

            cmd.Parameters.AddWithValue("@CustomerId", message.CustomerId);
            cmd.Parameters.AddWithValue("@CampaignId", message.CampaignId);
            cmd.Parameters.AddWithValue("@SentAt", message.SentAt);
            cmd.Parameters.AddWithValue("@Status", message.Status.ToString());
        }
    }
}
