using AttendanceManagementSystem.Interface;
using AttendanceManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;

namespace AttendanceManagementSystem.Controllers
{
    public class AttendanceManagementController : Controller
    {
        private readonly IAttendanceManagementService _attendanceManagementService;
        private const string ADMINFLG_STANDARD_USER = "0"; // 一般ユーザー

        public AttendanceManagementController(IAttendanceManagementService attendanceManagementService)
        {
            _attendanceManagementService = attendanceManagementService;

        }

        /// <summary>
        /// 日次一覧画面
        /// </summary>
        /// <param name="iModel">日次一覧画面入力情報</param>
        [HttpGet]
        public async Task<IActionResult> Index(IndexInputModel iModel)
        {
            // バリデーション
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Employee");
            }

            // セッション情報取得
            var employeeId = HttpContext.Session.GetString("EmployeeId");
            var name = HttpContext.Session.GetString("Name");
            var adminFlg = HttpContext.Session.GetString("AdminFlg");

            // 一般ユーザーかつ表示する従業員IDがログイン情報の従業員IDと異なる場合はエラー画面へ
            if (adminFlg == ADMINFLG_STANDARD_USER && employeeId != iModel.EmployeeId.ToString())
            {
                return RedirectToAction("Error", "Employee");
            }

            // サービス呼び出し
            DailyViewModel dailyViewModel = await _attendanceManagementService.Index(iModel);

            // エラー発生時
            if (dailyViewModel.ErrorFlg)
            {
                return RedirectToAction("Error", "Employee");
            }

            // 共通サイドバー用データ
            ViewData["EmployeeId"] = employeeId;
            ViewData["Name"] = name;
            ViewData["AdminFlg"] = adminFlg;

            return View(dailyViewModel);
        }

        /// <summary>
        /// 日次登録画面
        /// </summary>
        /// <param name="iModel">日次登録画面入力情報</param>
        [HttpGet]
        public async Task<IActionResult> Edit(EditInputModel iModel)
        {
            // バリデーション
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Employee");
            }

            // セッション情報取得
            var employeeId = HttpContext.Session.GetString("EmployeeId");
            var name = HttpContext.Session.GetString("Name");
            var adminFlg = HttpContext.Session.GetString("AdminFlg");

            // 一般ユーザーかつ表示する従業員IDがログイン情報の従業員IDと異なる場合はエラー画面へ
            if (adminFlg == ADMINFLG_STANDARD_USER && employeeId != iModel.EmployeeId.ToString())
            {
                return RedirectToAction("Error", "Employee");
            }

            // サービス呼び出し
            EditViewModel editViewModel = await _attendanceManagementService.GetDailyForEdit(iModel);

            // エラー発生時
            if (editViewModel.ErrorFlg)
            {
                return RedirectToAction("Error", "Employee");
            }

            // 共通サイドバー用データ
            ViewData["EmployeeId"] = employeeId;
            ViewData["Name"] = name;
            ViewData["AdminFlg"] = adminFlg;

            return View(editViewModel);
        }

        /// <summary>
        /// 日次更新
        /// </summary>
        /// <param name="iModel">日次入力画面で入力された情報</param>
        [HttpPost]
        public async Task<IActionResult> Edit(DailyInputModel iModel)
        {
            // ログイン情報と更新対象のレコードに差異がある場合は更新させない
            if (!ModelState.IsValid || iModel.EmployeeId.ToString() != HttpContext.Session.GetString("EmployeeId"))
            {
                return RedirectToAction("Error", "Employee");
            }

            // サービス呼び出し
            bool completedFlg = await _attendanceManagementService.UpdateDaily(iModel);

            if (completedFlg)
            {
                return RedirectToAction("Index", new
                {
                    EmployeeId = iModel.EmployeeId,
                    TargetMonth = iModel.DisplayDate
                });
            }
            else
            {
                return RedirectToAction("Error", "Employee");
            }
        }

        /// <summary>
        /// 承認ステータス更新
        /// </summary>
        /// <param name="iModel">承認する従業員情報と日付情報</param>
        [HttpPost]
        public async Task<IActionResult> Approve(ApproveInputModel iModel)
        {
            // 一般ユーザーの場合は更新させない
            if (!ModelState.IsValid || HttpContext.Session.GetString("AdminFlg") == ADMINFLG_STANDARD_USER)
            {
                return RedirectToAction("Error", "Employee");
            }

            // サービス呼び出し
            bool completedFlg = await _attendanceManagementService.Approve(iModel);

            if (completedFlg)
            {
                return RedirectToAction("Index", new
                {
                    EmployeeId = iModel.EmployeeId,
                    TargetMonth = iModel.DisplayDate
                });
            }
            else
            {
                return RedirectToAction("Error", "Employee");
            }
        }

        /// <summary>
        /// ユーザー選択画面
        /// </summary>
        /// <param name="iModel">承認する従業員情報と日付情報</param>
        [HttpGet]
        public async Task<IActionResult> SelectUser()
        {
            // バリデーション
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Employee");
            }

            // セッション情報取得
            var employeeId = HttpContext.Session.GetString("EmployeeId");
            var name = HttpContext.Session.GetString("Name");
            var adminFlg = HttpContext.Session.GetString("AdminFlg");

            // 一般ユーザーの場合はエラー画面へ
            if (adminFlg == ADMINFLG_STANDARD_USER)
            {
                return RedirectToAction("Error", "Employee");
            }

            // サービス呼び出し
            SelectUserViewModel selectUserViewModel = await _attendanceManagementService.SelectUser();

            // エラー発生時
            if (selectUserViewModel.ErrorFlg)
            {
                return RedirectToAction("Error", "Employee");
            }

            // 共通サイドバー用データ
            ViewData["EmployeeId"] = employeeId;
            ViewData["Name"] = name;
            ViewData["AdminFlg"] = adminFlg;

            return View(selectUserViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> MonthlySummary(int employeeId)
        {
            var now = DateTime.Now;
            var summaries = await _attendanceManagementService.GetMonthlySummariesAsync(employeeId);

            var viewModel = new MonthlySummaryViewModel
            {
                EmployeeId = employeeId,
                Year = now.Year,
                MonthlyRows = summaries
            };

            // 共通サイドバー用データ
            ViewData["EmployeeId"] = HttpContext.Session.GetString("EmployeeId");
            ViewData["Name"] = HttpContext.Session.GetString("Name");
            ViewData["AdminFlg"] = HttpContext.Session.GetString("AdminFlg");

            return View(viewModel);
        }


        /// <summary>
        /// CSV出力
        /// </summary>
        /// <param name="employeeId">従業員ID</param>
        /// <param name="targetMonth">指定月</param>
        [HttpGet]
        public async Task<IActionResult> ExportCsv(int employeeId, DateTime targetMonth)
        {
            var records = await _attendanceManagementService.GetMonthlyRecordsAsync(employeeId, targetMonth);

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("日付,勤務区分,開始時刻,終了時刻,備考,承認ステータス");

            foreach (var record in records)
            {
                csvBuilder.AppendLine(
                    $"{record.DisplayDate:yyyy/MM/dd},{record.WorkStatus},{record.StartTime:HH:mm},{record.EndTime:HH:mm},{record.Note},{record.ApprovalStatus}"
                    );
            }
            var csvBytes = Encoding.GetEncoding("shift_jis").GetBytes(csvBuilder.ToString());
            var fileName = $"勤怠_{employeeId}_{targetMonth:yyyyMM}.csv";

            return File(csvBytes, "text/csv", fileName);
        }
    }
}
