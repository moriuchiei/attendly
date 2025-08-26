namespace AttendanceManagementSystem.Models
{
    public class SelectUserViewModel
    {
        // 従業員IDリスト
        public List<Employee> EmployeeList { get; set; } = new List<Employee>();

        // エラーフラグ
        public bool ErrorFlg { get; set; } = false;
    }
}
