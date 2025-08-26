namespace AttendanceManagementSystem.Models
{
    public class DailyRecord
    {
        // 表示日付
        public DateTime DisplayDate { get; set; }

        // 祝日フラグ
        public bool NationalHolidayFlg { get; set; }

        // 勤務ステータス
        public WorkStatus WorkStatus { get; set; }

        // 勤務開始時間
        public DateTime StartTime { get; set; }

        // 勤務終了時間
        public DateTime EndTime { get; set; }

        // 平日勤務時間
        public TimeSpan WorkTime { get; set; }

        // 休日勤務時間
        public TimeSpan HolidayWorkTime { get; set; }

        // 残業時間
        public TimeSpan Overtime { get; set; }

        // 備考
        public string Note { get; set; } = string.Empty;

        // 承認状況（例：承認済／未承認）
        public ApprovalStatus ApprovalStatus { get; set; }
    }
}
