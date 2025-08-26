using AttendanceManagementSystem.Models;

namespace AttendanceManagementSystem.Interface
{
    public interface IEmployeeService
    {
        Task<List<Employee>> Login(LoginInputModel iModel);
        Task<bool> RegisterAccount(RegisterAccountInputModel iModel);
    }
}
