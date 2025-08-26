using AttendanceManagementSystem.Models;

namespace AttendanceManagementSystem.Interface
{
    public interface IAttendanceManagementService
    {
        Task<DailyViewModel> Index(IndexInputModel iModel);
        Task<EditViewModel> GetDailyForEdit(EditInputModel iModel);
        Task<bool> UpdateDaily(DailyInputModel iModel);
        Task<bool> Approve(ApproveInputModel iModel);
        Task<SelectUserViewModel> SelectUser();
        Task<List<Daily>> GetMonthlyRecordsAsync(int employeeId, DateTime targetMonth);
        Task<List<MonthlySummaryViewModel.MonthlyRow>> GetMonthlySummariesAsync(int employeeId);
    }
}
