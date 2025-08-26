using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; } // 従業員ID
        public string Email { get; set; } = string.Empty; // メールアドレス
        public string Department { get; set; } = string.Empty; // 所属
        public string Password { get; set; } = string.Empty; // パスワード
        public string Name { get; set; } = string.Empty; // 従業員名
        public int AdminFlg { get; set; } // 管理者フラグ
    }
}
