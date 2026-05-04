using Microsoft.Data.SqlClient;
using BChat.Models;
using BChat.Events;
using BChat.Models.Campaign_Module;

namespace BChat.Data.DataStore.Campaigns_Repository
{
    public static class CampaignRepository
    {
        private static string _connectionString = DatabaseConfig.ConnectionString;

        //اخر ما وقفت اليه Add
        public static void Add(Campaign campaign)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = @"INSERT INTO Campaigns
                                  (Name , GroupId ,TemplateId ,SentAt ,Status ,TotalCount ,SuccessCount ,FailedCount)
                               VALUES
                                  (@Name , @GroupId ,@TemplateId ,@SentAt ,@Status ,@TotalCount ,@SuccessCount ,@FailedCount)
                                SCUP IDENTITY()";

                using (SqlCommand cmd = new SqlCommand(query,conn))
                {
                    BindParams(cmd, campaign);
                }
            }
        }
        private static Campaign Map(SqlDataReader r)
        {
            return new Campaign()
            {
                Id = Convert.ToInt32(r["Id"]),
                Name = Convert.ToString(r["Name"]),
                GroupId = Convert.ToInt32(r["GroupId"]),
                TemplateId = Convert.ToInt32(r["TemplateId"]),
                SentAt = Convert.ToDateTime(r["Sent"]),
                Status = (CampaignStatus)(r["Status"]),
                TotalCount = Convert.ToInt32(r["TotalCount"]),
                SuccessCount = Convert.ToInt32(r["SuccessCount"]),
                FailedCount = Convert.ToInt32(r["FailedCount"])

            };
        }
        private static void BindParams(SqlCommand cmd , Campaign campaign)
        {
            cmd.Parameters.AddWithValue("@Name", campaign.Name);
            cmd.Parameters.AddWithValue("@GroupId", campaign.GroupId);
            cmd.Parameters.AddWithValue("@TemplateId", campaign.TemplateId);
            cmd.Parameters.AddWithValue("@SentAt", campaign.SentAt);
            cmd.Parameters.AddWithValue("@Status", campaign.Status);
            cmd.Parameters.AddWithValue("@TotalCount", campaign.TotalCount);
            cmd.Parameters.AddWithValue("@SuccessCount", campaign.SuccessCount);
            cmd.Parameters.AddWithValue("@FailedCount", campaign.FailedCount);
        }
        
    }
}