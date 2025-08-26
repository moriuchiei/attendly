namespace AttendanceManagementSystem.Models
{
    public class EditViewModel
    {
        // 従業員ID
        public int EmployeeId { get; set; }

        // 従業員名
        public string Name { get; set; } = string.Empty;

        // 表示日付
        public DateTime DisplayDate { get; set; }

        // 勤務ステータス
        public WorkStatus WorkStatus { get; set; }

        // 勤務開始時間
        public DateTime StartTime { get; set; }

        // 勤務終了時間
        public DateTime EndTime { get; set; }

        // 備考
        public string Note { get; set; } = string.Empty;

        // 承認ステータス
        public ApprovalStatus ApprovalStatus { get; set; }

        // エラーフラグ
        public bool ErrorFlg { get; set; } = false;
    }
}
