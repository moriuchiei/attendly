using AttendanceManagementSystem.Filters;
using AttendanceManagementSystem.Interface;
using AttendanceManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AttendanceManagementSystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        /// <summary>
        /// ログイン画面初期表示
        /// </summary>
        [HttpGet]
        [AllowAnonymousSession]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <param name="iModel">ログインフォームに入力された情報</param>
        [HttpPost]
        [AllowAnonymousSession]
        public async Task<IActionResult> Login(LoginInputModel iModel)
        {
            // バリデーション
            if (!ModelState.IsValid)
            {
                return View(iModel);
            }

            // Employeeサービス呼び出し
            var employeeList = await _employeeService.Login(iModel);

            // ログイン可能ユーザーの場合
            if (employeeList.Count > 0)
            {
                // セッションにログイン情報を格納
                HttpContext.Session.SetString("EmployeeId", employeeList[0].EmployeeId.ToString());
                HttpContext.Session.SetString("Email", employeeList[0].Email);
                HttpContext.Session.SetString("Department", employeeList[0].Department);
                HttpContext.Session.SetString("Name", employeeList[0].Name);
                HttpContext.Session.SetString("AdminFlg", employeeList[0].AdminFlg.ToString());

                // 一覧画面へ
                return RedirectToAction("Index", "AttendanceManagement", new
                {
                    EmployeeId = employeeList[0].EmployeeId,
                    TargetMonth = DateTime.Now
                });
            }
            // ログイン不可ユーザーの場合
            else
            {
                ModelState.AddModelError("", "ログインに失敗しました。");
                return View(iModel);
            }
        }

        /// <summary>
        /// 新規ユーザー登録画面初期表示
        /// </summary>
        [HttpGet]
        [AllowAnonymousSession]
        public IActionResult RegisterAccount()
        {
            return View();
        }

        /// <summary>
        /// 新規ユーザー登録
        /// </summary>
        /// <param name="iModel">新規ユーザー登録フォームに入力された情報</param>
        [HttpPost]
        [AllowAnonymousSession]
        public async Task<IActionResult> RegisterAccount(RegisterAccountInputModel iModel)
        {
            // バリデーション
            if (!ModelState.IsValid)
            {
                return View(iModel);
            }

            bool completedFlg = await _employeeService.RegisterAccount(iModel);
            // 登録完了した場合
            if (completedFlg)
            {
                // ログイン画面へリダイレクト
                return RedirectToAction("Login", "Employee");
            }
            // 登録失敗した場合はエラー画面へ
            return RedirectToAction("Error");
        }

        /// <summary>
        /// ログアウト処理
        /// </summary>
        [HttpGet]
        [AllowAnonymousSession]
        public IActionResult Logout()
        {
            // セッション削除
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        /// <summary>
        /// エラー画面
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymousSession]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
