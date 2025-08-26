using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AttendanceManagementSystem.Models
{
    public class Employee
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EmployeeId { get; set; } // �]�ƈ�ID
        public string Email { get; set; } = string.Empty; // ���[���A�h���X
        public string Department { get; set; } = string.Empty; // ����
        public string Password { get; set; } = string.Empty; // �p�X���[�h
        public string Name { get; set; } = string.Empty; // �]�ƈ���
        public int AdminFlg { get; set; } // �Ǘ��҃t���O
    }
}
