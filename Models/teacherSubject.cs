namespace ScholaAi.Models
{
    public class teacherSubject
    {
        public int teacherId { get; set; }
        public int subjectId { get; set; }

        public teacher? teacher { get; set; }
        public subject? subject { get; set; }
    }
}
