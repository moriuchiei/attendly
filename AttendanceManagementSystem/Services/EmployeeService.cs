using AttendanceManagementSystem.Interface;
using AttendanceManagementSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AttendanceManagementSystem.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly AppDbContext _context;

        public EmployeeService(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// ログイン処理
        /// </summary>
        /// <param name="iModel">ログインフォームに入力された情報</param>
        /// <returns>従業員情報リスト</returns>
        public async Task<List<Employee>> Login(LoginInputModel iModel)
        {
            var result = new List<Employee>();
            try
            {
                // DBから従業員情報を取得
                result = await _context.Employee
               .Where(e => e.Email == iModel.Email && e.Password == iModel.Password)
               .ToListAsync();                
            }
            catch (Exception ex)
            {
                await WriteLogAsync($"[ERROR] {ex.Message} {ex.StackTrace}");
            }
            
            return result;
        }

        /// <summary>
        /// 新規ユーザー登録
        /// </summary>
        /// <param name="iModel">新規ユーザー登録フォームに入力された情報</param>
        /// <returns>insert成功時true、失敗時false</returns>
        public async Task<bool> RegisterAccount(RegisterAccountInputModel iModel)
        {
            try
            {
                // Insert処理
                var employee = new Employee
                {
                    Email = iModel.Email,
                    Department = iModel.Department,
                    Password = iModel.Password,
                    Name = iModel.Name,
                    AdminFlg = iModel.AdminFlg
                };                
                _context.Employee.Add(employee);
                var result = await _context.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                await WriteLogAsync($"[ERROR] {ex.Message} {ex.StackTrace}");
                return false;
            }
        }

        /// <summary>
        /// エラーログ
        /// </summary>
        /// <param name="message">エラーメッセージ</param>
        private async Task WriteLogAsync(string message)
        {
            using (var writer = new StreamWriter("./logs/error.log", true))
            {
                await writer.WriteLineAsync($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");
            }
        }
    }
}
