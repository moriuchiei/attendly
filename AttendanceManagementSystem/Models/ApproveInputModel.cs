using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementSystem.Models
{
    public class ApproveInputModel
    {
        // 従業員ID
        [Required]
        public required int EmployeeId { get; set; }

        // 承認日付
        [Required]
        public required DateTime DisplayDate { get; set; }
    }
}
