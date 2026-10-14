using System.ComponentModel.DataAnnotations;

namespace AttendanceSystem.Models
{
    public class Student
    {
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Student name is required.")]
        [StringLength(100)]
        [Display(Name = "Student Name")]
        public string StudentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Registration number is required.")]
        [StringLength(30)]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required.")]
        [StringLength(50)]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Attendance percentage is required.")]
        [Range(0, 100, ErrorMessage = "Attendance must be between 0 and 100.")]
        [Display(Name = "Attendance %")]
        public decimal AttendancePercentage { get; set; }
    }
}