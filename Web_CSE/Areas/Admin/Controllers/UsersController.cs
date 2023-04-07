using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_CSE.Areas.Admin.Models;
using Web_CSE.Models;

namespace Web_CSE.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
{
    private readonly CnttCseContext _context;
    public UsersController(CnttCseContext context)
    {
        _context = context;
    }

   public IActionResult Index()
    {
        // Lấy thông tin người dùng hiện tại
        var userIdString = HttpContext.Session.GetString("AccountId");
        int userId;
        if (!int.TryParse(userIdString, out userId))
        {
            // Xử lý trường hợp chuyển đổi không thành công
            return RedirectToAction("Login", "Accounts");
        }

        // Tiếp tục xử lý khi đã có giá trị của userId
        var user = _context.Accounts.FirstOrDefault(a => a.AccountId == userId);

        if (user == null)
        {
            // Xử lý trường hợp không tìm thấy tài khoản người dùng
            return RedirectToAction("Login", "Accounts");
        }

        // Trả về trang tài khoản của người dùng
        var model = new UserViewModel { Account = user };
        return View(model);
    }

    public IActionResult UpdateAccount()
    {
        // Trả về view để hiển thị form thay đổi mật khẩu
        
        var userIdString = HttpContext.Session.GetString("AccountId");
        int userId;
        if (!int.TryParse(userIdString, out userId))
        {
            // Xử lý trường hợp chuyển đổi không thành công
            return RedirectToAction("Login", "Accounts");
        }

        // Tiếp tục xử lý khi đã có giá trị của userId
        var user = _context.Accounts.FirstOrDefault(a => a.AccountId == userId);

        if (user == null)
        {
            // Xử lý trường hợp không tìm thấy tài khoản người dùng
            return RedirectToAction("Login", "Accounts");
        }

        // Trả về trang tài khoản của người dùng
        var model = new UpdateAccountViewModel { Account = user };
        return View(model);
    }

    [HttpPost]
    public IActionResult UpdateAccount(UpdateAccountViewModel model)
    {
        // Kiểm tra tính hợp lệ của thông tin người dùng mới
        if (!ModelState.IsValid)
        {
            // Nếu thông tin nhập vào không hợp lệ, trả về view và hiển thị lỗi
            return View(model);
        }

        // Lấy thông tin người dùng hiện tại
         var userIdString = HttpContext.Session.GetString("AccountId");
        int userId;
        if (!int.TryParse(userIdString, out userId) || string.IsNullOrEmpty(userIdString))
        {
            // Xử lý trường hợp chuyển đổi không thành công
            return RedirectToAction("Login", "Accounts");
        }
        var user = _context.Accounts.FirstOrDefault(a => a.AccountId == userId);

        // Cập nhật thông tin tài khoản người dùng nếu có
        if (model.FullName != null)
        {
            user.FullName = model.FullName;
        }
        if (model.Phone != null)
        {
            user.Phone = model.Phone;
        }

        _context.Update(user);
        _context.SaveChanges();

        // Trả về thông báo cập nhật thành công
        return RedirectToAction("Index", new { success = true });
    }

    public IActionResult ChangePassword()
    {
        // Trả về view để hiển thị form thay đổi mật khẩu
        return View();
    }

    [HttpPost]
    public IActionResult ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // Nếu thông tin nhập vào không hợp lệ, trả về view và hiển thị lỗi
            return View(model);
        }

        // Lấy thông tin người dùng hiện tại
         var userIdString = HttpContext.Session.GetString("AccountId");
        int userId;
        if (!int.TryParse(userIdString, out userId) || string.IsNullOrEmpty(userIdString))
        {
            // Xử lý trường hợp chuyển đổi không thành công
            return RedirectToAction("Login", "Accounts");
        }
        var user = _context.Accounts.FirstOrDefault(a => a.AccountId == userId);

        // Kiểm tra mật khẩu cũ
        if (!PasswordHelper.VerifyPassword(model.OldPassword, user.Password, user.Salt))
        {
            ModelState.AddModelError("OldPassword", "Mật khẩu cũ không đúng");
            return View(model);
        }

        // Đổi mật khẩu
        var salt = PasswordHelper.GenerateSalt();
        user.Salt = salt;
        user.Password = PasswordHelper.HashPassword(model.NewPassword, salt);
        _context.Update(user);
        _context.SaveChanges();

        // Trả về thông báo đổi mật khẩu thành công
        return RedirectToAction("Index", new { success = true });
    }
}
}