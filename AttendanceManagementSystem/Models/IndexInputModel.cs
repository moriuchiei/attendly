using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementSystem.Models
{
    public class IndexInputModel
    {
        [Required]
        public required int EmployeeId { get; set; } // 従業員ID

        [Required]
        public required DateTime TargetMonth { get; set; } // 表示月
    }
}
