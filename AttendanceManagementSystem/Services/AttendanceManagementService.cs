using AttendanceManagementSystem.Interface;
using AttendanceManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AttendanceManagementSystem.Services
{
    public class AttendanceManagementService : IAttendanceManagementService
    {
        private readonly AppDbContext _context;
        #region 祝日マスタ
        private static readonly HashSet<DateTime> JapanHolidays = new HashSet<DateTime>
        {
            new DateTime(2025, 1, 1),   // 元日（水）
            new DateTime(2025, 1, 13),  // 成人の日（月）
            new DateTime(2025, 2, 11),  // 建国記念の日（火）
            new DateTime(2025, 2, 23),  // 天皇誕生日（日）
            new DateTime(2025, 2, 24),  // 振替休日（月）
            new DateTime(2025, 3, 20),  // 春分の日（木）
            new DateTime(2025, 4, 29),  // 昭和の日（火）
            new DateTime(2025, 5, 3),   // 憲法記念日（土）
            new DateTime(2025, 5, 4),   // みどりの日（日）
            new DateTime(2025, 5, 5),   // こどもの日（月）
            new DateTime(2025, 5, 6),   // 振替休日（火）
            new DateTime(2025, 7, 21),  // 海の日（月）
            new DateTime(2025, 8, 11),  // 山の日（月）
            new DateTime(2025, 9, 15),  // 敬老の日（月）
            new DateTime(2025, 9, 23),  // 秋分の日（火）
            new DateTime(2025, 10, 13), // スポーツの日（月）
            new DateTime(2025, 11, 3),  // 文化の日（月）
            new DateTime(2025, 11, 23), // 勤労感謝の日（日）
            new DateTime(2025, 11, 24), // 振替休日（月）
        };
        #endregion

        public AttendanceManagementService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 日次一覧画面
        /// </summary>
        /// <param name="iModel">日次一覧画面入力情報</param>
        /// <returns>日次一覧表示情報</returns>
        public async Task<DailyViewModel> Index(IndexInputModel iModel)
        {
            // 返却用ViewModelを宣言
            DailyViewModel dailyViewModels = new DailyViewModel();
            try
            {
                var employeeId = iModel.EmployeeId;

                dailyViewModels.EmployeeId = iModel.EmployeeId;
                dailyViewModels.TargetMonth = iModel.TargetMonth;

                // 従業員情報取得
                var employee = await GetEmployee(employeeId);
                dailyViewModels.Name = employee.Name;

                // 今月・前月・前々月をリスト化
                DateTime today = DateTime.Today;
                dailyViewModels.MonthList = new List<DateTime> { today, today.AddMonths(-1), today.AddMonths(-2) };

                // 指定月の勤務表を生成
                dailyViewModels.DailyRecord = await CreateMonthlyDailyRecords(employeeId, iModel.TargetMonth);
            }
            catch (Exception ex)
            {
                await WriteLogAsync($"[ERROR] {ex.Message} {ex.StackTrace}");
                dailyViewModels.ErrorFlg = true;
            }

            return dailyViewModels;
        }

        /// <summary>
        /// 日次登録画面
        /// </summary>
        /// <param name="iModel">日次登録画面入力情報</param>
        /// <returns>日次登録画面表示情報</returns>
        public async Task<EditViewModel> GetDailyForEdit(EditInputModel iModel)
        {
            // 返却用ViewModelを宣言
            EditViewModel editViewModel = new EditViewModel();

            try
            {
                var employeeId = iModel.EmployeeId;

                editViewModel.EmployeeId = iModel.EmployeeId;
                editViewModel.DisplayDate = iModel.TargetDate;

                // 従業員情報取得
                var employee = await GetEmployee(employeeId);
                editViewModel.Name = employee.Name;

                // 指定月の勤務表を生成
                var daily = await GetDailyRecords(employeeId, iModel.TargetDate);

                if (daily != null)
                {
                    editViewModel.WorkStatus = daily.WorkStatus;
                    editViewModel.StartTime = daily.StartTime;
                    editViewModel.EndTime = daily.EndTime;
                    editViewModel.Note = daily.Note;
                    editViewModel.ApprovalStatus = daily.ApprovalStatus;
                }
            }
            catch (Exception ex)
            {
                await WriteLogAsync($"[ERROR] {ex.Message} {ex.StackTrace}");
                editViewModel.ErrorFlg = true;
            }

            return editViewModel;
        }

        /// <summary>
        /// 日次登録
        /// </summary>
        /// <param name="iModel">日次登録情報</param>
        /// <returns>日次登録結果</returns>
        public async Task<bool> UpdateDaily(DailyInputModel iModel)
        {
            try
            {
                var existing = await _context.Daily
                .FirstOrDefaultAsync(d => d.EmployeeId == iModel.EmployeeId && d.DisplayDate == iModel.DisplayDate);

                if (existing == null)
                {
                    // Dailyテーブルにデータが存在しない場合はINSERT処理
                    var dairy = new Daily
                    {
                        EmployeeId = iModel.EmployeeId,
                        DisplayDate = iModel.DisplayDate,
                        WorkStatus = iModel.WorkStatus,
                        StartTime = iModel.StartTime,
                        EndTime = iModel.EndTime,
                        Note = iModel.Note,
                        ApprovalStatus = iModel.ApprovalStatus
                    };
                    _context.Daily.Add(dairy);
                }
                else
                {
                    // Dailyテーブルにデータが存在する場合はUPDATE処理
                    existing.WorkStatus = iModel.WorkStatus;
                    existing.StartTime = iModel.StartTime;
                    existing.EndTime = iModel.EndTime;
                    existing.Note = iModel.Note;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLogAsync($"[ERROR] {ex.Message} {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// 承認ステータス更新
        /// </summary>
        /// <param name="iModel">承認ステータス更新情報</param>
        /// <returns>承認ステータス更新結果</returns>
        public async Task<bool> Approve(ApproveInputModel iModel)
        {
            try
            {
                var existing = await _context.Daily
                .FirstOrDefaultAsync(d => d.EmployeeId == iModel.EmployeeId && d.DisplayDate == iModel.DisplayDate);

                // 更新対象レコードがみつからない場合
                if (existing == null)
                {
                    throw new Exception("承認ステータス更新対象レコードがありません");
                }
                else
                {
                    // Dailyテーブルにデータが存在する場合はUPDATE処理
                    existing.ApprovalStatus = ApprovalStatus.承認済み;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                await WriteLogAsync($"[ERROR] {ex.Message} {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// ユーザー選択画面
        /// </summary>
        /// <returns>ユーザー選択画面表示情報</returns>
        public async Task<SelectUserViewModel> SelectUser()
        {
            // 返却用ViewModelを宣言
            SelectUserViewModel selectUserViewModel = new SelectUserViewModel();

            try
            {
                // DBから従業員情報を取得
                selectUserViewModel.EmployeeList = await _context.Employee
                    .Where(e =>
                        e.AdminFlg == 0
                    )
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                await WriteLogAsync($"[ERROR] {ex.Message} {ex.StackTrace}");
                selectUserViewModel.ErrorFlg = true;
            }

            return selectUserViewModel;
        }

        public async Task<List<MonthlySummaryViewModel.MonthlyRow>> GetMonthlySummariesAsync(int employeeId)
        {
            int year = DateTime.Now.Year;
            var startOfYear = new DateTime(year, 1, 1);
            var endOfYear = new DateTime(year, 12, 31);

            // 今年の1月1日から12月31日までの日次データ取得
            var records = await _context.Daily
                .Where(d => d.EmployeeId == employeeId && d.DisplayDate >= startOfYear && d.DisplayDate <= endOfYear)
                .ToListAsync();

            // 1月から12月までのリスト生成
            var monthlySummaries = Enumerable.Range(1, 12).Select(month =>
            {
                var monthStart = new DateTime(year, month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                // 各月の日次データ取得
                var monthRecords = records
                    .Where(r => r.DisplayDate >= monthStart && r.DisplayDate <= monthEnd)
                    .ToList();

                // 月次レコードを生成して、各項目に集計結果を格納
                return new MonthlySummaryViewModel.MonthlyRow
                {
                    Month = month,
                    WorkDays = monthRecords.Count(r => r.WorkStatus == WorkStatus.出勤),
                    AbsenceDays = monthRecords.Count(r => r.WorkStatus == WorkStatus.欠勤),
                    PaidLeaveDays = monthRecords.Count(r => r.WorkStatus == WorkStatus.有給休暇),
                    // 残業時間の集計
                    TotalOvertime = TimeSpan.FromMinutes(
                        monthRecords.Sum(r =>
                        {
                            var workMinutes = (r.EndTime - r.StartTime).TotalMinutes;
                            var breakMinutes = TimeSpan.FromMinutes(60).TotalMinutes;
                            var netMinutes = Math.Max(0, workMinutes - breakMinutes);

                            if (netMinutes > 480)
                            {
                                return netMinutes - 480;
                            }
                            else
                            {
                                return 0;
                            }
                        })
                     ),
                    UnapprovedDays = monthRecords.Count(r => r.ApprovalStatus == ApprovalStatus.未承認)
                };
            }).ToList();

            return monthlySummaries;
        }

        /// <summary>
        /// CSV出力
        /// </summary>
        /// <param name="employeeId">従業員ID</param>
        /// <param name="targetMonth">指定月</param>
        public async Task<List<Daily>> GetMonthlyRecordsAsync(int employeeId, DateTime targetMonth)
        {
            var start = new DateTime(targetMonth.Year, targetMonth.Month, 1);
            var end = start.AddMonths(1).AddDays(-1);

            // 指定月の日次データを取得して返却
            return await _context.Daily
                .Where(d => d.EmployeeId == employeeId && d.DisplayDate >= start && d.DisplayDate <= end)
                .OrderBy(d => d.DisplayDate)
                .ToListAsync();
        }

        /// <summary>
        /// 従業員情報取得
        /// </summary>
        /// <param name="employeeId">従業員ID</param>
        /// <returns>従業員情報</returns>
        private async Task<Employee> GetEmployee(int employeeId)
        {
            // DBから従業員情報を取得
            var employee = await _context.Employee
                .Where(e =>
                    e.EmployeeId == employeeId
                )
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                throw new Exception("従業員が見つかりません");
            }
            return employee;
        }

        private async Task<List<DailyRecord>> CreateMonthlyDailyRecords(int employeeId, DateTime yearMonth)
        {
            // 返却用Listを宣言
            List<DailyRecord> dailyRecord = new List<DailyRecord>();

            // 指定月の月初日と月末日を取得
            var startDate = new DateTime(yearMonth.Year, yearMonth.Month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // DBから勤怠情報を取得
            var dailyList = await _context.Daily
                .Where(daily =>
                    daily.EmployeeId == employeeId &&
                    daily.DisplayDate >= startDate &&
                    daily.DisplayDate <= endDate
                )
                .OrderBy(daily => daily.DisplayDate)
                .ToListAsync();

            // 年・月を使って対象月の末日を取得
            int daysInMonth = DateTime.DaysInMonth(yearMonth.Year, yearMonth.Month);

            for (int day = 1; day <= daysInMonth; day++)
            {
                var date = new DateTime(yearMonth.Year, yearMonth.Month, day);

                var daily = dailyList.FirstOrDefault(d => d.DisplayDate.Date == date.Date);

                TimeSpan workTime;
                TimeSpan holidayWorkTime;
                bool nationalHolidayFlg = IsHoliday(date);

                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday || nationalHolidayFlg)
                {
                    // 土日祝の場合は平日勤務時間は00:00、休日勤務時間に勤務時間差を格納
                    workTime = TimeSpan.Zero;
                    holidayWorkTime = daily != null ? CalcWorkDuration(daily) : TimeSpan.Zero;
                }
                else
                {
                    // 平日の場合は平日勤務時間は勤務時間差、休日勤務時間に00:00を格納
                    workTime = daily != null ? CalcWorkDuration(daily) : TimeSpan.Zero;
                    holidayWorkTime = TimeSpan.Zero;
                }

                TimeSpan overtime = daily != null ? CalcOvertime(daily) : TimeSpan.Zero;

                // 返却用Listに追加
                dailyRecord.Add(new DailyRecord
                {
                    DisplayDate = date,
                    NationalHolidayFlg = nationalHolidayFlg,
                    WorkStatus = daily?.WorkStatus ?? WorkStatus.未設定,
                    StartTime = daily?.StartTime ?? DateTime.MinValue,
                    EndTime = daily?.EndTime ?? DateTime.MinValue,
                    WorkTime = workTime,
                    HolidayWorkTime = holidayWorkTime,
                    Overtime = overtime,
                    Note = daily?.Note ?? "",
                    ApprovalStatus = daily?.ApprovalStatus ?? ApprovalStatus.未承認
                });
            }
            return dailyRecord;
        }

        /// <summary>
        /// 勤務時間取得
        /// </summary>
        /// <param name="dailyRecord">日次レコード</param>
        /// <returns>勤務時間</returns>
        private static TimeSpan CalcWorkDuration(Daily dailyRecord)
        {
            if (dailyRecord?.StartTime == null || dailyRecord?.EndTime == null)
                return TimeSpan.Zero;

            TimeSpan rawDuration = dailyRecord.EndTime - dailyRecord.StartTime;
            TimeSpan breakTime = TimeSpan.FromHours(1); // 休憩1時間
            TimeSpan actualDuration = rawDuration - breakTime;

            return actualDuration > TimeSpan.Zero ? actualDuration : TimeSpan.Zero;
        }

        /// <summary>
        /// 残業時間取得
        /// </summary>
        /// <param name="dailyRecord">日次レコード</param>
        /// <returns>残業時間</returns>
        private static TimeSpan CalcOvertime(Daily dailyRecord)
        {
            // 勤務時間を計算（退勤 - 出勤）
            TimeSpan workDuration = dailyRecord.EndTime - dailyRecord.StartTime;

            // 休憩時間（1時間を想定）
            TimeSpan breakTime = TimeSpan.FromHours(1);

            // 実労働時間 = 勤務時間 - 休憩時間
            TimeSpan actualWorkTime = workDuration - breakTime;

            // 所定労働時間（8時間）
            TimeSpan standardWorkTime = TimeSpan.FromHours(8);

            // 残業時間 = 実労働時間 - 所定労働時間（マイナスにならないように）
            TimeSpan overtime = actualWorkTime > standardWorkTime ? actualWorkTime - standardWorkTime : TimeSpan.Zero;

            return overtime;
        }

        /// <summary>
        /// 祝日フラグ取得
        /// </summary>
        /// <param name="date">日付</param>
        /// <returns>祝日フラグ</returns>
        private bool IsHoliday(DateTime date)
        {
            return JapanHolidays.Contains(date.Date);
        }

        /// <summary>
        /// 表示月の日次情報取得
        /// </summary>
        /// <param name="employeeId">従業員ID</param>
        /// <param name="targetDate">取得日付</param>
        /// <returns>表示月の日次情報</returns>
        private async Task<Daily?> GetDailyRecords(int employeeId, DateTime targetDate)
        {
            // DBから勤怠情報を取得
            var daily = await _context.Daily
                .Where(daily =>
                    daily.EmployeeId == employeeId &&
                    daily.DisplayDate == targetDate
                )
                .OrderBy(daily => daily.DisplayDate)
                .FirstOrDefaultAsync();

            return daily;
        }

        /// <summary>
        /// ログ出力
        /// </summary>
        /// <param name="message">ログ出力メッセージ</param>
        private async Task WriteLogAsync(string message)
        {
            using (var writer = new StreamWriter("error.log", true))
            {
                await writer.WriteLineAsync($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");
            }
        }
    }
}
