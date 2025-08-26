using AttendanceManagementSystem.Filters;
using AttendanceManagementSystem.Interface;
using AttendanceManagementSystem.Models;
using AttendanceManagementSystem.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ���O�C����Ԋm�F�p�t�B���^�[�ǉ�
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<SessionCheckFilter>();
});

// �T�[�r�X�o�^
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAttendanceManagementService, AttendanceManagementService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// DB�ڑ��֘A
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Shift-jis�Ή��̂���
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// �Z�b�V�����֘A
builder.Services.AddSession();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    System.IO.Directory.CreateDirectory("/home/data");
    db.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Login}/{id?}");

app.Run();
