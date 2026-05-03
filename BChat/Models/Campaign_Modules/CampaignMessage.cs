using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Models.Campaign_Modules
{

    public enum CampaignMessageStatus
    {
        pending,
        Sending,
        Completed,
        Failed
    }

    public class CampaignMessage
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int CampaignId { get; set; }
        public CampaignMessageStatus Status { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
        
    }
}
