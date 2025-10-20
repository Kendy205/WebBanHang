using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using WebBanHang.DAL.Data;
using WebBanHang.DTOs;
using WebBanHang.FileUpload.IFileUpload;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : BaseAdminController
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly IBufferedFileUploadService _fileUploadService;
        public UsersController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context,
            IBufferedFileUploadService fileUploadService,
            ILogger<UsersController> logger) : base(logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _fileUploadService = fileUploadService;
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
            return View(new ApplicationUserDTO());
        }

        // POST: /Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApplicationUserDTO model, IFormFile img) // <-- THAY ĐỔI 1
        {
            // Kiểm tra validation cho toàn bộ model (bao gồm cả Password và Role)
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(
                    await _roleManager.Roles.ToListAsync(), "Name", "Name", model.Role // Thêm model.Role để giữ lại giá trị đã chọn
                );
                return View(model);
            }

            try
            {
                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FulName = model.FulName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address,
                    EmailConfirmed = true,
                };
                user.imgUrl= await _fileUploadService.UploadFileAsync(img);
                // Lấy mật khẩu từ model
                var result = await _userManager.CreateAsync(user, model.Password); // <-- THAY ĐỔI 2

                if (result.Succeeded)
                {
                    // Lấy role từ model
                    if (!string.IsNullOrEmpty(model.Role)) // <-- THAY ĐỔI 3
                    {
                        await _userManager.AddToRoleAsync(user, model.Role); // <-- THAY ĐỔI 4
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

            // Nếu có lỗi, nạp lại Roles và trả về view với model hiện tại để hiển thị lỗi
            ViewBag.Roles = new SelectList(
                await _roleManager.Roles.ToListAsync(), "Name", "Name", model.Role
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
            var currentRole = userRoles.FirstOrDefault();
            ViewBag.Roles = new SelectList(
                await _roleManager.Roles.ToListAsync(),
                "Name",
                "Name",
                currentRole
            );
            ViewBag.CurrentRole = userRoles.FirstOrDefault();
            var userDto = new UserEditDTO
            {
                Id = user.Id,
                FulName = user.FulName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Address = user.Address,
                ImgUrl = user.imgUrl, // Lấy đường dẫn ảnh hiện tại
                Role = currentRole ?? "" // Gán Role hiện tại
            };

            return View(userDto);
        }

        // POST: /Admin/Users/Edit/{id}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string userId, UserEditDTO model) 
        {
            // Kiểm tra tính hợp lệ cơ bản
            if (userId != model.Id)
                return BadRequest();

            // 1. CHUẨN BỊ ROLE & VIEW DATA NẾU CÓ LỖI VALIDATION
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(
                    await _roleManager.Roles.ToListAsync(),
                    "Name",
                    "Name",
                    model.Role // Giữ lại Role đã chọn
                );
                // Chuyển DTO về View để người dùng sửa lại
                return View(model);
            }

            // 2. TÌM NGƯỜI DÙNG VÀ CẬP NHẬT TRƯỜNG
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ShowError("Người dùng không tồn tại");
                return RedirectToAction("Index");
            }

            // Cập nhật các trường thông thường từ DTO vào user
            user.FulName = model.FulName;
            user.PhoneNumber = model.PhoneNumber;
            user.Address = model.Address;
            user.UpdateAt = DateTime.Now; // Giả sử bạn có trường này

            // 3. XỬ LÝ UPLOAD FILE ẢNH MỚI (NewAvatarFile)
            if (model.NewAvatarFile != null)
            {
                // TODO: TRIỂN KHAI LOGIC LƯU FILE TẠI ĐÂY
                // Ví dụ:
                // 3a. Xóa ảnh cũ (nếu user.imgUrl không null)
                // 3b. Lưu file mới vào thư mục (wwwroot/images/avatars)
                // 3c. Lấy đường dẫn mới (ví dụ: "/images/avatars/new-file-name.png")

                string newImgUrl = await _fileUploadService.UploadFileAsync(model.NewAvatarFile);
                user.imgUrl = newImgUrl;
            }
            // Nếu model.NewAvatarFile là null, user.imgUrl sẽ giữ nguyên giá trị cũ (được truyền qua hidden field)

            // 4. THỰC HIỆN CẬP NHẬT
            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                // 5. CẬP NHẬT ROLE
                if (!string.IsNullOrEmpty(model.Role))
                {
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    await _userManager.RemoveFromRolesAsync(user, currentRoles.ToArray());
                    await _userManager.AddToRoleAsync(user, model.Role);
                }

                ShowSuccess("Cập nhật người dùng thành công");
                return RedirectToAction("Index");
            }
            else
            {
                // Xử lý lỗi từ Identity (nếu có)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                // Tải lại ViewBag.Roles và trả về View với model hiện tại
                ViewBag.Roles = new SelectList(
                    await _roleManager.Roles.ToListAsync(),
                    "Name",
                    "Name",
                    model.Role // Giữ lại Role đã chọn
                );
                return View(model);
            }
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
                    if(user.imgUrl != null)
                    {
                        await _fileUploadService.DeleteFileAsync(user.imgUrl);
                    }
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
