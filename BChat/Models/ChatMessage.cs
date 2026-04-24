using System;
namespace BChat.Models
{
    public class ChatMessage
    {
        // ── DB Fields ─────────────────────────────────────────
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string? Text { get; set; }
        public DateTime SentAt { get; set; } = DateTime.Now;
        public bool IsSent { get; set; } = true;       // ← IsOutgoing القديم
        public bool IsRead { get; set; } = false;
        public bool HasAttachment { get; set; } = false;
        public string? AttachmentName { get; set; }
        public long? AttachmentSize { get; set; }
        public string? AttachmentType { get; set; }    // "image" / "document" / ...
        public string? AttachmentUrl { get; set; }
        public string? WhatsAppMessageId { get; set; }
        public string? Status { get; set; } = "pending";

        // ── UI-Only (لا تُحفظ في DB) ──────────────────────────
        public string? SenderName { get; set; }        // يُملأ من Customer
        public Image? Avatar { get; set; }             // يُملأ من Customer

        // ── Computed Properties ───────────────────────────────
        public MessageType Type =>
            AttachmentType?.StartsWith("image", StringComparison.OrdinalIgnoreCase) == true
                ? MessageType.Image
                : HasAttachment ? MessageType.Attachment
                : MessageType.Text;

        public MessageStatus MessageStatus =>
            Status switch
            {
                "sent" => MessageStatus.Sent,
                "delivered" => MessageStatus.Sent,
                "read" => MessageStatus.Sent,
                "failed" => MessageStatus.Failed,
                _ => MessageStatus.Pending   // "pending"
            };
    }

    public enum MessageType
    {
        Text,
        Attachment,
        Image
    }

    public enum MessageStatus
    {
        Sent,
        Pending,
        Failed
    }
}