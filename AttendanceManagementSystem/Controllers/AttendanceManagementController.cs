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
        private const string ADMINFLG_STANDARD_USER = "0"; // ��ʃ��[�U�[

        public AttendanceManagementController(IAttendanceManagementService attendanceManagementService)
        {
            _attendanceManagementService = attendanceManagementService;

        }

        /// <summary>
        /// �����ꗗ���
        /// </summary>
        /// <param name="iModel">�����ꗗ��ʓ��͏��</param>
        [HttpGet]
        public async Task<IActionResult> Index(IndexInputModel iModel)
        {
            // �o���f�[�V����
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Employee");
            }

            // �Z�b�V�������擾
            var employeeId = HttpContext.Session.GetString("EmployeeId");
            var name = HttpContext.Session.GetString("Name");
            var adminFlg = HttpContext.Session.GetString("AdminFlg");

            // ��ʃ��[�U�[���\������]�ƈ�ID�����O�C�����̏]�ƈ�ID�ƈقȂ�ꍇ�̓G���[��ʂ�
            if (adminFlg == ADMINFLG_STANDARD_USER && employeeId != iModel.EmployeeId.ToString())
            {
                return RedirectToAction("Error", "Employee");
            }

            // �T�[�r�X�Ăяo��
            DailyViewModel dailyViewModel = await _attendanceManagementService.Index(iModel);

            // �G���[������
            if (dailyViewModel.ErrorFlg)
            {
                return RedirectToAction("Error", "Employee");
            }

            // ���ʃT�C�h�o�[�p�f�[�^
            ViewData["EmployeeId"] = employeeId;
            ViewData["Name"] = name;
            ViewData["AdminFlg"] = adminFlg;

            return View(dailyViewModel);
        }

        /// <summary>
        /// �����o�^���
        /// </summary>
        /// <param name="iModel">�����o�^��ʓ��͏��</param>
        [HttpGet]
        public async Task<IActionResult> Edit(EditInputModel iModel)
        {
            // �o���f�[�V����
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Employee");
            }

            // �Z�b�V�������擾
            var employeeId = HttpContext.Session.GetString("EmployeeId");
            var name = HttpContext.Session.GetString("Name");
            var adminFlg = HttpContext.Session.GetString("AdminFlg");

            // ��ʃ��[�U�[���\������]�ƈ�ID�����O�C�����̏]�ƈ�ID�ƈقȂ�ꍇ�̓G���[��ʂ�
            if (adminFlg == ADMINFLG_STANDARD_USER && employeeId != iModel.EmployeeId.ToString())
            {
                return RedirectToAction("Error", "Employee");
            }

            // �T�[�r�X�Ăяo��
            EditViewModel editViewModel = await _attendanceManagementService.GetDailyForEdit(iModel);

            // �G���[������
            if (editViewModel.ErrorFlg)
            {
                return RedirectToAction("Error", "Employee");
            }

            // ���ʃT�C�h�o�[�p�f�[�^
            ViewData["EmployeeId"] = employeeId;
            ViewData["Name"] = name;
            ViewData["AdminFlg"] = adminFlg;

            return View(editViewModel);
        }

        /// <summary>
        /// �����X�V
        /// </summary>
        /// <param name="iModel">�������͉�ʂœ��͂��ꂽ���</param>
        [HttpPost]
        public async Task<IActionResult> Edit(DailyInputModel iModel)
        {
            // ���O�C�����ƍX�V�Ώۂ̃��R�[�h�ɍ��ق�����ꍇ�͍X�V�����Ȃ�
            if (!ModelState.IsValid || iModel.EmployeeId.ToString() != HttpContext.Session.GetString("EmployeeId"))
            {
                return RedirectToAction("Error", "Employee");
            }

            // �T�[�r�X�Ăяo��
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
        /// ���F�X�e�[�^�X�X�V
        /// </summary>
        /// <param name="iModel">���F����]�ƈ����Ɠ��t���</param>
        [HttpPost]
        public async Task<IActionResult> Approve(ApproveInputModel iModel)
        {
            // ��ʃ��[�U�[�̏ꍇ�͍X�V�����Ȃ�
            if (!ModelState.IsValid || HttpContext.Session.GetString("AdminFlg") == ADMINFLG_STANDARD_USER)
            {
                return RedirectToAction("Error", "Employee");
            }

            // �T�[�r�X�Ăяo��
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
        /// ���[�U�[�I�����
        /// </summary>
        /// <param name="iModel">���F����]�ƈ����Ɠ��t���</param>
        [HttpGet]
        public async Task<IActionResult> SelectUser()
        {
            // �o���f�[�V����
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Error", "Employee");
            }

            // �Z�b�V�������擾
            var employeeId = HttpContext.Session.GetString("EmployeeId");
            var name = HttpContext.Session.GetString("Name");
            var adminFlg = HttpContext.Session.GetString("AdminFlg");

            // ��ʃ��[�U�[�̏ꍇ�̓G���[��ʂ�
            if (adminFlg == ADMINFLG_STANDARD_USER)
            {
                return RedirectToAction("Error", "Employee");
            }

            // �T�[�r�X�Ăяo��
            SelectUserViewModel selectUserViewModel = await _attendanceManagementService.SelectUser();

            // �G���[������
            if (selectUserViewModel.ErrorFlg)
            {
                return RedirectToAction("Error", "Employee");
            }

            // ���ʃT�C�h�o�[�p�f�[�^
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

            // ���ʃT�C�h�o�[�p�f�[�^
            ViewData["EmployeeId"] = HttpContext.Session.GetString("EmployeeId");
            ViewData["Name"] = HttpContext.Session.GetString("Name");
            ViewData["AdminFlg"] = HttpContext.Session.GetString("AdminFlg");

            return View(viewModel);
        }


        /// <summary>
        /// CSV�o��
        /// </summary>
        /// <param name="employeeId">�]�ƈ�ID</param>
        /// <param name="targetMonth">�w�茎</param>
        [HttpGet]
        public async Task<IActionResult> ExportCsv(int employeeId, DateTime targetMonth)
        {
            var records = await _attendanceManagementService.GetMonthlyRecordsAsync(employeeId, targetMonth);

            var csvBuilder = new StringBuilder();
            csvBuilder.AppendLine("���t,�Ζ��敪,�J�n����,�I������,���l,���F�X�e�[�^�X");

            foreach (var record in records)
            {
                csvBuilder.AppendLine(
                    $"{record.DisplayDate:yyyy/MM/dd},{record.WorkStatus},{record.StartTime:HH:mm},{record.EndTime:HH:mm},{record.Note},{record.ApprovalStatus}"
                    );
            }
            var csvBytes = Encoding.GetEncoding("shift_jis").GetBytes(csvBuilder.ToString());
            var fileName = $"�Α�_{employeeId}_{targetMonth:yyyyMM}.csv";

            return File(csvBytes, "text/csv", fileName);
        }
    }
}
