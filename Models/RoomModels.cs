namespace ScholaAi.Models
{
    public class RoomState
    {
        public string RoomId { get; set; } = string.Empty;
        public HashSet<string> ConnectionIds { get; set; } = new();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class UserState
    {
        public string ConnectionId { get; set; } = string.Empty;
        public string RoomId { get; set; } = string.Empty;
        public string Role { get; set; } = "viewer";
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }

    public class RoomJoinedPayload
    {
        public string RoomId { get; set; } = string.Empty;
        public string YourId { get; set; } = string.Empty;
        public string Role { get; set; } = "viewer";
        public List<string> ExistingUsers { get; set; } = new();
    }
}
