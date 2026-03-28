namespace ScholaAi.DTOs.Sessions
{
    public class SessionDetailsDto
    {
        public int SessionId { get; set; }
        public string TeacherId { get; set; }
        public string StudentId { get; set; }
        public string Status { get; set; }
        public string RoomId { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public string TeacherName { get; set; }
        public string StudentName { get; set; }
    }

    public class StartSessionResponseDto
    {
        public string RoomId { get; set; }
        public string PeerId { get; set; }
        public string Role { get; set; }
        public int SessionId { get; set; }
    }
}