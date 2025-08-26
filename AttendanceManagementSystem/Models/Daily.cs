using System;
using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementSystem.Models
{
    public class Daily
    {
        public int EmployeeId { get; set; } // 従業員ID
        public DateTime DisplayDate { get; set; } // 表示日
        public WorkStatus WorkStatus { get; set; } // 勤務ステータス
        public DateTime StartTime { get; set; } // 出勤時間
        public DateTime EndTime { get; set; } // 退勤時間
        public string Note { get; set; } = string.Empty; // 備考
        public ApprovalStatus ApprovalStatus { get; set; } // 承認ステータス
    }
}
