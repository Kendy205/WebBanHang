using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebBanHang.DAL.Data;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : BaseAdminController
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            ILogger<UsersController> logger) : base(logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }
        // GET: /Admin/Users
        [HttpGet]
        public async Task<IActionResult> Index(string? role = null, string? searchTerm = null, int page = 1, int pageSize = 20)
        {
            var query = _context.Users.AsQueryable();

            // Filter by role
            if (!string.IsNullOrEmpty(role))
            {
                var roleId = (await _roleManager.FindByNameAsync(role))?.Id;
                if (roleId != null)
                {
                    var userIdsInRole = await _context.UserRoles
                        .Where(ur => ur.RoleId == roleId)
                        .Select(ur => ur.UserId)
                        .ToListAsync();

                    query = query.Where(u => userIdsInRole.Contains(u.Id));
                }
            }

            // Search
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(u =>
                    u.Email!.Contains(searchTerm) 
                    
                );
            }

            var totalUsers = await query.CountAsync();
            var users = await query
                .OrderBy(u => u.Email)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Get user roles
            var userRoles = new Dictionary<string, List<string>>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles[user.Id] = roles.ToList();
            }

            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            ViewBag.SelectedRole = role;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalUsers / pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.UserRoles = userRoles;

            return View(users);
        }

        // GET: /Admin/Users/Details/{id}
        [HttpGet]
        public async Task<IActionResult> Details(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                ShowError("ID người dùng không hợp lệ");
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ShowError("Người dùng không tồn tại");
                return RedirectToAction("Index");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            ViewBag.UserRoles = userRoles;

            return View(user);
        }

        // GET: /Admin/Users/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewBag.Roles = new SelectList(
                await _roleManager.Roles.ToListAsync(),
                "Name",
                "Name"
            );
            return View(new ApplicationUser());
        }

        // POST: /Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUser model, string password, string role)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Roles = new SelectList(
                        await _roleManager.Roles.ToListAsync(),
                        "Name",
                        "Name"
                    );
                    return View(model);
                }

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                  
                    EmailConfirmed = true,
                   
                };

                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    // Assign role
                    if (!string.IsNullOrEmpty(role))
                    {
                        await _userManager.AddToRoleAsync(user, role);
                    }

                    ShowSuccess("Thêm người dùng thành công");
                    _logger.LogInformation($"New user created: {user.Email}");
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                ShowError($"Lỗi: {ex.Message}");
            }

            ViewBag.Roles = new SelectList(
                await _roleManager.Roles.ToListAsync(),
                "Name",
                "Name"
            );
            return View(model);
        }

        // GET: /Admin/Users/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                ShowError("ID người dùng không hợp lệ");
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ShowError("Người dùng không tồn tại");
                return RedirectToAction("Index");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            ViewBag.Roles = new SelectList(
                await _roleManager.Roles.ToListAsync(),
                "Name",
                "Name"
            );
            ViewBag.CurrentRole = userRoles.FirstOrDefault();

            return View(user);
        }

        // POST: /Admin/Users/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string userId, ApplicationUser model, string role)
        {
            try
            {
                if (userId != model.Id)
                    return BadRequest();

                if (!ModelState.IsValid)
                {
                    ViewBag.Roles = new SelectList(
                        await _roleManager.Roles.ToListAsync(),
                        "Name",
                        "Name"
                    );
                    return View(model);
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    ShowError("Người dùng không tồn tại");
                    return RedirectToAction("Index");
                }

                //user.FullName = model.FullName;
                user.PhoneNumber = model.PhoneNumber;
                user.Address = model.Address;
                //user.IsActive = model.IsActive;
                //user.UpdatedAt = DateTime.Now;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Update role
                    if (!string.IsNullOrEmpty(role))
                    {
                        var currentRoles = await _userManager.GetRolesAsync(user);
                        await _userManager.RemoveFromRolesAsync(user, currentRoles.ToArray());
                        await _userManager.AddToRoleAsync(user, role);
                    }

                    ShowSuccess("Cập nhật người dùng thành công");
                    _logger.LogInformation($"User updated: {user.Email}");
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user");
                ShowError($"Lỗi: {ex.Message}");
            }

            var userRoles = await _userManager.GetRolesAsync(model);
            ViewBag.Roles = new SelectList(
                await _roleManager.Roles.ToListAsync(),
                "Name",
                "Name"
            );
            ViewBag.CurrentRole = userRoles.FirstOrDefault();

            return View(model);
        }

        // GET: /Admin/Users/Delete/{id}
        [HttpGet]
        public async Task<IActionResult> Delete(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                ShowError("ID người dùng không hợp lệ");
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ShowError("Người dùng không tồn tại");
                return RedirectToAction("Index");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            ViewBag.UserRoles = userRoles;

            return View(user);
        }

        // POST: /Admin/Users/Delete/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    ShowError("Người dùng không tồn tại");
                    return RedirectToAction("Index");
                }

                // Không cho phép xóa chính mình
                var currentUserId = _userManager.GetUserId(User);
                if (userId == currentUserId)
                {
                    ShowError("Không thể xóa tài khoản của chính bạn");
                    return RedirectToAction("Index");
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    ShowSuccess("Xóa người dùng thành công");
                    _logger.LogInformation($"User deleted: {user.Email}");
                }
                else
                {
                    ShowError($"Không thể xóa: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user");
                ShowError("Có lỗi xảy ra");
            }

            return RedirectToAction("Index");
        }

        // POST: /Admin/Users/ToggleActive (AJAX)
        //[HttpPost]
        //public async Task<IActionResult> ToggleActive(string id)
        //{
        //    try
        //    {
        //        var user = await _userManager.FindByIdAsync(id);
        //        if (user == null)
        //            return Json(new { success = false, message = "Người dùng không tồn tại" });

        //        //user.IsActive = !user.IsActive;
        //        //user.UpdatedAt = DateTime.Now;
        //        await _userManager.UpdateAsync(user);

        //        return Json(new
        //        {
        //            success = true,
        //            isActive = user.IsActive,
        //            message = user.IsActive ? "Đã kích hoạt tài khoản" : "Đã vô hiệu hóa tài khoản"
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error toggling user active status");
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //}

        // GET: /Admin/Users/ResetPassword/{id}
        [HttpGet]
        public async Task<IActionResult> ResetPassword(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                ShowError("ID người dùng không hợp lệ");
                return RedirectToAction("Index");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ShowError("Người dùng không tồn tại");
                return RedirectToAction("Index");
            }

            ViewBag.UserId = userId;
            ViewBag.UserName = user.Email;

            return View();
        }

        // POST: /Admin/Users/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string userId, string newPassword, string confirmPassword)
        {
            try
            {
                if (string.IsNullOrEmpty(newPassword))
                {
                    ModelState.AddModelError("", "Mật khẩu không được để trống");
                }
                else if (newPassword != confirmPassword)
                {
                    ModelState.AddModelError("", "Mật khẩu xác nhận không khớp");
                }
                else
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user == null)
                    {
                        ShowError("Người dùng không tồn tại");
                        return RedirectToAction("Index");
                    }

                    // Remove old password
                    await _userManager.RemovePasswordAsync(user);

                    // Add new password
                    var result = await _userManager.AddPasswordAsync(user, newPassword);

                    if (result.Succeeded)
                    {
                        ShowSuccess("Đặt lại mật khẩu thành công");
                        _logger.LogInformation($"Password reset for user: {user.Email}");
                        return RedirectToAction("Details", new { id = userId });
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError("", error.Description);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                ShowError("Có lỗi xảy ra");
            }

            ViewBag.UserId = userId;
            var userInfo = await _userManager.FindByIdAsync(userId);
            ViewBag.UserName = userInfo?.Email;

            return View();
        }
    }
}
