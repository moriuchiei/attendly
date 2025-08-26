using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementSystem.Models
{
    public class DailyViewModel
    {
        // 従業員ID
        public int EmployeeId { get; set; }

        // 表示月
        public DateTime TargetMonth { get; set; }

        // 従業員名
        public string Name { get; set; } = string.Empty;

        // 検索月リスト
        public List<DateTime> MonthList { get; set; } = new List<DateTime>();

        // 日次データ
        public List<DailyRecord> DailyRecord { get; set; } = new List<DailyRecord>();

        // エラーフラグ
        public bool ErrorFlg { get; set; } = false;
    }
}
