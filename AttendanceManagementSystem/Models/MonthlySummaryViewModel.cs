namespace AttendanceManagementSystem.Models
{
    public class MonthlySummaryViewModel
    {
        // 従業員ID
        public int EmployeeId { get; set; }

        // 表示年
        public int Year { get; set; }

        // 各月次データ
        public List<MonthlyRow> MonthlyRows { get; set; } = new();
        public class MonthlyRow
        {
            public int Month { get; set; }
            public int WorkDays { get; set; }
            public int AbsenceDays { get; set; }
            public int PaidLeaveDays { get; set; }
            public TimeSpan TotalOvertime { get; set; }
            public int UnapprovedDays { get; set; }
        }
    }
}
