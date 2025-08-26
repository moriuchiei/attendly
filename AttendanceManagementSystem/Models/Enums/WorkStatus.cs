using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementSystem.Models
{
    public enum WorkStatus
    {
        [Display(Name = "")]
        未設定 = 0,
        [Display(Name = "出勤")]
        出勤 = 1,
        [Display(Name = "有給休暇")]
        有給休暇 = 2,
        [Display(Name = "欠勤")]
        欠勤 = 3
    }
}