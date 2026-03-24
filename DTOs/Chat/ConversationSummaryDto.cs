namespace ScholaAi.DTOs.Chat
{
    public class ConversationSummaryDto
    {
        public string OtherUserId { get; set; }
        public string OtherUserName { get; set; }
        public string OtherUserRole { get; set; }

        public string LastMessageText { get; set; }
        public string LastMessageType { get; set; }
        public DateTime LastMessageTime { get; set; }

        public int UnreadCount { get; set; }
    }
}
