using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Models.Campaign_Module
{
    public enum CampaignStatus
    {
        pending,
        Sending,
        Completed,
        Failed
    }

    public class Campaign
    {
       public int Id { get; set; }
       public string Name { get; set; }
       public int GroupId { get; set; }
       public int TemplateId { get; set; }
       public DateTime SentAt { get; set; } = DateTime.Now;
       public CampaignStatus Status { get; set; }
       public int TotalCount { get; set; }
       public int SuccessCount { get; set; }
       public int FailedCount { get; set; }

    }
}
