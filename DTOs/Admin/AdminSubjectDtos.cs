
using System.ComponentModel.DataAnnotations;

namespace ScholaAi.DTOs.Admin
{
    public class AdminSubjectDto
    {
        public int SubjectId { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int TeacherCount { get; set; }
    }

    public class CreateSubjectDto
    {
        [Required] public string Name { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateSubjectDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
    }
}