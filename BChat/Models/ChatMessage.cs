using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BChat.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }

        // المحتوى
        public string Text { get; set; }

        // الاتجاه
        public bool IsOutgoing { get; set; }

        // الوقت
        public DateTime Time { get; set; }

        // المستخدم
        public string SenderName { get; set; }
        public Image Avatar { get; set; }

        // نوع الرسالة
        public MessageType Type { get; set; }

        // مرفقات
        public string AttachmentName { get; set; }
        public long? AttachmentSize { get; set; }

        // الحالة
        public MessageStatus Status { get; set; }
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