using AutoMapper;
using CakeShop.Areas.Admin.Data;
using CakeShop.Controllers;
using CakeShop.Data;
using CakeShop.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace CakeShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly CakeshopContext db;
        private readonly IMapper _mapper;

        public AdminController(CakeshopContext context, IMapper mapper)
        {
            db = context;
            _mapper = mapper;
        }

        [Route("")]
        [Route("Index")]
        [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
            return View();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(AdminLoginVM model)
        {
            if (ModelState.IsValid)
            {
                var nhanVien = db.NhanViens.SingleOrDefault(nv => nv.MaNv == model.UserName);
                if (nhanVien == null)
                {
                    ModelState.AddModelError("loi", "Sai thông tin đăng nhập");
                }
                else
                {
                    // BỎ KIỂM TRA MẬT KHẨU Ở ĐÂY

                    var phancong = db.PhanCongs.FirstOrDefault(x => x.MaNv == nhanVien.MaNv);
                    if (phancong == null)
                    {
                        ModelState.AddModelError("loi", "Không tìm thấy thông tin phân công cho tài khoản này.");
                    }
                    else if (phancong.HieuLuc != true)
                    {
                        ModelState.AddModelError("loi", "Tài khoản đã hết hiệu lực!");
                    }
                    else
                    {
                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name, nhanVien.HoTen),
                            new Claim(MySetting.CLAIM_ADMINID, nhanVien.MaNv),
                            new Claim(ClaimTypes.Role, SD.Role_Admin),
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, "AdminCookie");
                        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                        await HttpContext.SignOutAsync("CustomerCookie");
                        await HttpContext.SignInAsync("AdminCookie", claimsPrincipal);

                        return RedirectToAction("Index", "Admin", new { area = "Admin" });
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        [Route("Profile")]
        [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
        public IActionResult Profile()
        {
            var adminId = User.FindFirst(MySetting.CLAIM_ADMINID)?.Value;
            var nhanvien = db.NhanViens.SingleOrDefault(nv => nv.MaNv == adminId);
            if (nhanvien == null)
            {
                return NotFound();
            }
            return View(nhanvien);
        }

        [HttpPost]
        [Route("Profile")]
        [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
        public IActionResult Profile(NhanVien nhanVien)
        {
            if (ModelState.IsValid)
            {
                nhanVien.MatKhau = nhanVien.MatKhau.ToMd5Hash("4dm!n");
                db.Entry(nhanVien).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(nhanVien);
        }

        [Route("Logout")]
        [Authorize(AuthenticationSchemes = "AdminCookie", Roles = SD.Role_Admin)]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AdminCookie");
            TempData["Message"] = "Đã đăng xuất quyền quản trị viên!";
            return Redirect("/Admin/ThongBao");
        }

        [Route("NotFound")]
        public IActionResult NotFound()
        {
            return View();
        }

        [Route("ThongBao")]
        public IActionResult ThongBao()
        {
            return View();
        }
    }
}
