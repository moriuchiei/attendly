using System.ComponentModel.DataAnnotations;

namespace AttendanceManagementSystem.Models
{
    public class LoginInputModel
    {
        [Required(ErrorMessage = "メールアドレスは必須です。")]
        [RegularExpression(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "正しいメールアドレス形式で入力してください。")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "パスワードは必須です")]
        public required string Password { get; set; }
    }
}
