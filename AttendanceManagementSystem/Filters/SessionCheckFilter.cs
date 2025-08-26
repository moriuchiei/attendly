using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AttendanceManagementSystem.Filters
{
    public class SessionCheckFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // メソッドに[AllowAnonymousSession]が付いている場合はチェックしない
            var hasAllowAnonymous = context.ActionDescriptor.EndpointMetadata
                .OfType<AllowAnonymousSessionAttribute>().Any();
            if (hasAllowAnonymous) return;

            // セッションにログイン情報があるか確認
            var userId = context.HttpContext.Session.GetString("EmployeeId");

            if (string.IsNullOrEmpty(userId))
            {
                // セッションがなければログイン画面へリダイレクト
                context.Result = new RedirectToActionResult("Login", "Employee", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
