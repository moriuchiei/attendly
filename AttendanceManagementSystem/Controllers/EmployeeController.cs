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
        /// ���O�C����ʏ����\��
        /// </summary>
        [HttpGet]
        [AllowAnonymousSession]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// ���O�C������
        /// </summary>
        /// <param name="iModel">���O�C���t�H�[���ɓ��͂��ꂽ���</param>
        [HttpPost]
        [AllowAnonymousSession]
        public async Task<IActionResult> Login(LoginInputModel iModel)
        {
            // �o���f�[�V����
            if (!ModelState.IsValid)
            {
                return View(iModel);
            }

            // Employee�T�[�r�X�Ăяo��
            var employeeList = await _employeeService.Login(iModel);

            // ���O�C���\���[�U�[�̏ꍇ
            if (employeeList.Count > 0)
            {
                // �Z�b�V�����Ƀ��O�C�������i�[
                HttpContext.Session.SetString("EmployeeId", employeeList[0].EmployeeId.ToString());
                HttpContext.Session.SetString("Email", employeeList[0].Email);
                HttpContext.Session.SetString("Department", employeeList[0].Department);
                HttpContext.Session.SetString("Name", employeeList[0].Name);
                HttpContext.Session.SetString("AdminFlg", employeeList[0].AdminFlg.ToString());

                // �ꗗ��ʂ�
                return RedirectToAction("Index", "AttendanceManagement", new
                {
                    EmployeeId = employeeList[0].EmployeeId,
                    TargetMonth = DateTime.Now
                });
            }
            // ���O�C���s���[�U�[�̏ꍇ
            else
            {
                ModelState.AddModelError("", "���O�C���Ɏ��s���܂����B");
                return View(iModel);
            }
        }

        /// <summary>
        /// �V�K���[�U�[�o�^��ʏ����\��
        /// </summary>
        [HttpGet]
        [AllowAnonymousSession]
        public IActionResult RegisterAccount()
        {
            return View();
        }

        /// <summary>
        /// �V�K���[�U�[�o�^
        /// </summary>
        /// <param name="iModel">�V�K���[�U�[�o�^�t�H�[���ɓ��͂��ꂽ���</param>
        [HttpPost]
        [AllowAnonymousSession]
        public async Task<IActionResult> RegisterAccount(RegisterAccountInputModel iModel)
        {
            // �o���f�[�V����
            if (!ModelState.IsValid)
            {
                return View(iModel);
            }

            bool completedFlg = await _employeeService.RegisterAccount(iModel);
            // �o�^���������ꍇ
            if (completedFlg)
            {
                // ���O�C����ʂփ��_�C���N�g
                return RedirectToAction("Login", "Employee");
            }
            // �o�^���s�����ꍇ�̓G���[��ʂ�
            return RedirectToAction("Error");
        }

        /// <summary>
        /// ���O�A�E�g����
        /// </summary>
        [HttpGet]
        [AllowAnonymousSession]
        public IActionResult Logout()
        {
            // �Z�b�V�����폜
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        /// <summary>
        /// �G���[���
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymousSession]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
