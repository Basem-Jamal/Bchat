using System;

namespace BChat.Models
{
    public class StoreToken
    {
        public int Id { get; set; }
        public string StoreId { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}