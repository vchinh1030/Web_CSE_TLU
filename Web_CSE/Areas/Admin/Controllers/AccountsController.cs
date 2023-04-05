using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_CSE.Models;

namespace Web_CSE.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AccountsController : Controller
    {
        private readonly CnttCseContext _context;

        public AccountsController(CnttCseContext context)
        {
            _context = context;
        }

        // GET: Admin/Accounts
        public async Task<IActionResult> Index()
        {
            var cnttCseContext = _context.Accounts.Include(a => a.Role);
            return View(await cnttCseContext.ToListAsync());
        }
        // GET: Admin/Login
        [HttpGet]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "Login")]

        public IActionResult Login(string returnUrl = null)
        {
            var taikhoanID = HttpContext.Session.GetString("AccountID");
            if (taikhoanID != null)
            {
                return RedirectToAction("Index", "Posts", new { Areas = "Admin" });
            }
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("dang-nhap.html", Name = "Login")]

    public async Task<IActionResult> Login(Models.LoginViewModel model, string returnUrl = null)
    {
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Email.Contains(" "))
                    {
                        ModelState.AddModelError("Email", "Email không được chứa khoảng trắng");
                        return View(model);
                    }
                    string email = model.Email.ToLower().Trim().Replace(" ", "");
                    Account kh = _context.Accounts
                        .Include(p => p.Role)
                        .SingleOrDefault(p => p.Email.ToLower() == email);
                    if (kh == null)
                    {
                        ModelState.AddModelError("Email", "Email chưa chính xác");
                        return View(model);
                    }
                      if (model.Password.Contains(" "))
                    {
                        ModelState.AddModelError("Password", "Mật khẩu không được chứa khoảng trắng");
                        return View(model);
                    }
                    string pass = model.Password.Trim().Replace(" ", ""); // Loại bỏ khoảng trắng đầu và cuối, và khoảng trắng trong chuỗi password
                    if (string.IsNullOrEmpty(pass))
                    {
                        ModelState.AddModelError("Password", "Vui lòng nhập Password.");
                        return View(model);
                    }
                    string hashedPassword = HashPassword(pass, kh.Salt);
                    if (kh.Password != hashedPassword)
                    {
                        ModelState.AddModelError("Password", "Mật khẩu chưa chính xác");
                        return View(model);
                    }
                    //Đăng nhập thành công

                    //ghi nhận thời gian đăng nhập
                    //kh.LastLogin = DateTime.Now;
                    _context.Update(kh);
                    await _context.SaveChangesAsync();

                    var taikhoanID = HttpContext.Session.GetString("AccountId");
                    //indentity
                    //Lưu session MaKh
                    HttpContext.Session.SetString("AccountId", kh.AccountId.ToString());

                    //indentity
                    var useClaims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, kh.FullName),
                        new Claim(ClaimTypes.Email, kh.Email),
                        new Claim("AccountId", kh.AccountId.ToString()),
                        new Claim("RoleID", kh.RoleId.ToString()),
                        new Claim(ClaimTypes.Role, kh.Role.RoleName),
                    };

                    var grandmaIdentity = new ClaimsIdentity(useClaims, "User Identity");
                    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });
                    await HttpContext.SignInAsync(userPrincipal);

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Posts", new { Area = "Admin" });
                }
                 else if (string.IsNullOrWhiteSpace(model.Email) && string.IsNullOrWhiteSpace(model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Vui lòng nhập Email và Password.");
                    return View(model);
                }
                else if (string.IsNullOrWhiteSpace(model.Email))
                {
                    ModelState.AddModelError("Email", "Vui lòng nhập Email.");
                    return View(model);
                }
                else if (string.IsNullOrWhiteSpace(model.Password))
                {
                    ModelState.AddModelError("Password", "Vui lòng nhập Password.");
                    return View(model);
                }
            }
            catch
            {
                return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            }
            // return RedirectToAction("Login", "Accounts", new { Area = "Admin" });
            return RedirectToAction("Index", "Posts", new { Area = "Admin" });
    }

        //Đăng xuất 
        [AllowAnonymous]
        [Route("dang-xuat.html", Name = "Logout")]
        public IActionResult Logout(){
            try
            {
                HttpContext.SignOutAsync();
                HttpContext.Session.Remove("AccountId"); // xóa session theo id
                return RedirectToAction("Index", "Home"); //đẩy về home index
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }
        }
        
        
        
        // GET: Admin/Accounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // GET: Admin/Accounts/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            return View();
        }

        // POST: Admin/Accounts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountId,FullName,Email,Phone,Password,Salt,Active,CreatedAt,RoleId")] Account account)
        {
            if (ModelState.IsValid)
            {
                account.CreatedAt = DateTime.Now;
               
                // Tạo salt ngẫu nhiên sử dụng RNGCryptoServiceProvider
                byte[] saltBytes = new byte[16];
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(saltBytes);
                }
                  string salt = Convert.ToBase64String(saltBytes);
                 // Ma hoa mat khau truoc khi luu vao co so du lieu
                string hashedPassword = HashPassword(account.Password, salt);
                account.Password = hashedPassword;
                account.Salt = salt;
                _context.Add(account);
                
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

        // Ham ma hoa mat khau bang SHA256
       public string HashPassword(string password, string salt)
        {
            byte[] saltBytes = Convert.FromBase64String(salt);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password.Trim()); //loại bỏ khoảng trắng cuối cùng nếu có
            byte[] saltedPasswordBytes = new byte[saltBytes.Length + passwordBytes.Length];

            Buffer.BlockCopy(saltBytes, 0, saltedPasswordBytes, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, saltedPasswordBytes, saltBytes.Length, passwordBytes.Length);

            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(saltedPasswordBytes);
                return Convert.ToBase64String(hashedBytes);
            }
        }


        // GET: Admin/Accounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts.FindAsync(id);
            if (account == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

        // POST: Admin/Accounts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountId,FullName,Email,Phone,Password,Salt,Active,CreatedAt,RoleId")] Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(account);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AccountExists(account.AccountId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", account.RoleId);
            return View(account);
        }

        // GET: Admin/Accounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
                .Include(a => a.Role)
                .FirstOrDefaultAsync(m => m.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            return View(account);
        }

        // POST: Admin/Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Accounts == null)
            {
                return Problem("Entity set 'CnttCseContext.Accounts'  is null.");
            }
            var account = await _context.Accounts.FindAsync(id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AccountExists(int id)
        {
          return _context.Accounts.Any(e => e.AccountId == id);
        }
    }
}
