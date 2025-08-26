using System;
using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementSystem.Models
{
    public class DailyInputModel
    {
        [Required]
        public int EmployeeId { get; set; } // 従業員ID
        [Required]
        public DateTime DisplayDate { get; set; } // 表示日
        [Required]
        public WorkStatus WorkStatus { get; set; } // 勤務ステータス
        [Required]
        public DateTime StartTime { get; set; } // 出勤時間
        [Required]
        public DateTime EndTime { get; set; } // 退勤時間
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Note { get; set; } = string.Empty; // 備考
        [Required]
        public ApprovalStatus ApprovalStatus { get; set; } // 承認ステータス
    }
}
