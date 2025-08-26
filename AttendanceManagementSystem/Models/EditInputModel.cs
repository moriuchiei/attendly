using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementSystem.Models
{
    public class EditInputModel
    {
        [Required]
        public required int EmployeeId { get; set; } // 従業員ID

        [Required]
        public required DateTime TargetDate { get; set; } // 表示日
    }
}
