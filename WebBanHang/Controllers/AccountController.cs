//// =============================================
//// ACCOUNT CONTROLLER - .NET 8
//// Controllers/AccountController.cs
//// Sử dụng ASP.NET Core Identity
//// =============================================

//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
//using System.ComponentModel.DataAnnotations;
//using WebBanHang.DAL.Data;
//using WebBanHang.Models;

//namespace WebBanHang.Controllers
//{
//    [Route("[controller]/[action]")]
//    public class AccountController : Controller
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly ApplicationDbContext _context;
//        private readonly ILogger<AccountController> _logger;

//        public AccountController(
//            UserManager<ApplicationUser> userManager,
//            SignInManager<ApplicationUser> signInManager,
//            ApplicationDbContext context,
//            ILogger<AccountController> logger)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _context = context;
//            _logger = logger;
//        }

//        private string GetUserIpAddress()
//        {
//            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
//        }

//        // =============================================
//        // GET: /Account/Login
//        // =============================================
//        [HttpGet]
//        [AllowAnonymous]
//        public IActionResult Login(string? returnUrl = null)
//        {
//            ViewData["ReturnUrl"] = returnUrl;
//            return View();
//        }

//        // =============================================
//        // POST: /Account/Login
//        // =============================================
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
//        {
//            ViewData["ReturnUrl"] = returnUrl;

//            if (!ModelState.IsValid)
//                return View(model);

//            try
//            {
//                // Kiểm tra tài khoản có bị khóa không
//                var user = await _userManager.FindByEmailAsync(model.Email);
//                if (user != null )
//                {
//                    ModelState.AddModelError("", "Tài khoản của bạn đã bị vô hiệu hóa. Vui lòng liên hệ quản trị viên.");
//                    _logger.LogWarning($"Inactive user attempted login: {model.Email}");
//                    return View(model);
//                }

//                // Thử đăng nhập
//                var result = await _signInManager.PasswordSignInAsync(
//                    model.Email,
//                    model.Password,
//                    model.RememberMe,
//                    lockoutOnFailure: false
//                );

//                if (result.Succeeded)
//                {
//                    // Lưu Session
//                    HttpContext.Session.SetString("UserEmail", user.Email);
//                    HttpContext.Session.SetString("UserId", user.Id);
//                    //HttpContext.Session.SetString("UserName", user.FullName ?? user.Email);

//                    // Lưu Cookie nếu Remember Me
//                    if (model.RememberMe)
//                    {
//                        var cookieOptions = new CookieOptions
//                        {
//                            Expires = DateTime.Now.AddDays(30),
//                            HttpOnly = true,
//                            Secure = true,
//                            SameSite = SameSiteMode.Lax
//                        };
//                        Response.Cookies.Append("UserEmail", user.Email, cookieOptions);
//                    }

//                    _logger.LogInformation($"User logged in successfully: {user.Email}");

//                    // Log activity
//                    await LogActivityAsync(user.Id, "LOGIN", "Đăng nhập thành công");

//                    // Redirect based on role
//                    var userRoles = await _userManager.GetRolesAsync(user);
//                    if (userRoles.Contains("Admin") || userRoles.Contains("Staff"))
//                    {
//                        return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
//                    }

//                    return RedirectToLocal(returnUrl);
//                }

//                if (result.IsLockedOut)
//                {
//                    ModelState.AddModelError("", "Tài khoản của bạn đã bị khóa tạm thời. Vui lòng thử lại sau.");
//                    _logger.LogWarning($"User account locked: {model.Email}");
//                    return View(model);
//                }

//                if (result.RequiresTwoFactor)
//                {
//                    return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
//                }

//                ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác.");
//                _logger.LogWarning($"Failed login attempt: {model.Email}");
//                return View(model);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error during login");
//                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
//                return View(model);
//            }
//        }

//        // =============================================
//        // GET: /Account/Register
//        // =============================================
//        [HttpGet]
//        [AllowAnonymous]
//        public IActionResult Register()
//        {
//            return View();
//        }

//        // =============================================
//        // POST: /Account/Register
//        // =============================================
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Register(RegisterViewModel model)
//        {
//            if (!ModelState.IsValid)
//                return View(model);

//            try
//            {
//                var user = new ApplicationUser
//                {
//                    UserName = model.Email,
//                    Email = model.Email,
//                    FullName = model.FullName,
//                    PhoneNumber = model.PhoneNumber,
//                    Address = model.Address,
//                    IsActive = true,
//                    EmailConfirmed = true,
//                    CreatedAt = DateTime.Now
//                };

//                var result = await _userManager.CreateAsync(user, model.Password);

//                if (result.Succeeded)
//                {
//                    // Assign Customer role by default
//                    await _userManager.AddToRoleAsync(user, "Customer");

//                    // Sign in automatically
//                    await _signInManager.SignInAsync(user, isPersistent: false);

//                    // Log activity
//                    await LogActivityAsync(user.Id, "REGISTER", "Đăng ký tài khoản mới");

//                    _logger.LogInformation($"User registered successfully: {user.Email}");
//                    TempData["Success"] = "Đăng ký tài khoản thành công!";

//                    return RedirectToAction("Index", "Home", new { area = "Customer" });
//                }

//                foreach (var error in result.Errors)
//                {
//                    ModelState.AddModelError("", error.Description);
//                }

//                _logger.LogWarning($"User registration failed: {model.Email} - Errors: {string.Join(", ", result.Errors.Select(e => e.Description))}");
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error during registration");
//                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
//            }

//            return View(model);
//        }

//        // =============================================
//        // POST: /Account/Logout
//        // =============================================
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Authorize]
//        public async Task<IActionResult> Logout()
//        {
//            var userId = _userManager.GetUserId(User);

//            // Log activity
//            if (!string.IsNullOrEmpty(userId))
//            {
//                await LogActivityAsync(userId, "LOGOUT", "Đăng xuất");
//            }

//            // Clear Session
//            HttpContext.Session.Clear();

//            // Clear Cookies
//            Response.Cookies.Delete("UserEmail");
//            Response.Cookies.Delete(".AspNetCore.Identity.Application");

//            await _signInManager.SignOutAsync();
//            _logger.LogInformation($"User logged out");

//            return RedirectToAction(nameof(Login));
//        }

//        // =============================================
//        // GET: /Account/Profile
//        // =============================================
//        [HttpGet]
//        [Authorize(Roles = "Customer")]
//        public async Task<IActionResult> Profile()
//        {
//            var userId = _userManager.GetUserId(User);
//            if (string.IsNullOrEmpty(userId))
//                return RedirectToAction(nameof(Login));

//            var user = await _userManager.FindByIdAsync(userId);
//            if (user == null)
//                return RedirectToAction(nameof(Login));

//            var model = new EditProfileViewModel
//            {
//                FullName = user.FullName,
//                Email = user.Email,
//                PhoneNumber = user.PhoneNumber,
//                Address = user.Address
//            };

//            return View(model);
//        }

//        // =============================================
//        // POST: /Account/Profile
//        // =============================================
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Authorize(Roles = "Customer")]
//        public async Task<IActionResult> Profile(EditProfileViewModel model)
//        {
//            if (!ModelState.IsValid)
//                return View(model);

//            try
//            {
//                var userId = _userManager.GetUserId(User);
//                if (string.IsNullOrEmpty(userId))
//                    return RedirectToAction(nameof(Login));

//                var user = await _userManager.FindByIdAsync(userId);
//                if (user == null)
//                    return RedirectToAction(nameof(Login));

//                user.FullName = model.FullName;
//                user.PhoneNumber = model.PhoneNumber;
//                user.Address = model.Address;
//                user.UpdatedAt = DateTime.Now;

//                var result = await _userManager.UpdateAsync(user);

//                if (result.Succeeded)
//                {
//                    HttpContext.Session.SetString("UserName", user.FullName ?? user.Email);
//                    TempData["Success"] = "Cập nhật thông tin thành công!";
//                    _logger.LogInformation($"User profile updated: {user.Email}");
//                    return RedirectToAction(nameof(Profile));
//                }

//                foreach (var error in result.Errors)
//                {
//                    ModelState.AddModelError("", error.Description);
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating profile");
//                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
//            }

//            return View(model);
//        }

//        // =============================================
//        // GET: /Account/ChangePassword
//        // =============================================
//        [HttpGet]
//        [Authorize(Roles = "Customer")]
//        public IActionResult ChangePassword()
//        {
//            return View();
//        }

//        // =============================================
//        // POST: /Account/ChangePassword
//        // =============================================
//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        [Authorize(Roles = "Customer")]
//        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
//        {
//            if (!ModelState.IsValid)
//                return View(model);

//            try
//            {
//                var userId = _userManager.GetUserId(User);
//                if (string.IsNullOrEmpty(userId))
//                    return RedirectToAction(nameof(Login));

//                var result = await _userManager.ChangePasswordAsync(
//                    userId,
//                    model.OldPassword,
//                    model.NewPassword
//                );

//                if (result.Succeeded)
//                {
//                    var user = await _userManager.FindByIdAsync(userId);
//                    if (user != null)
//                    {
//                        await _signInManager.RefreshSignInAsync(user);
//                    }

//                    TempData["Success"] = "Đổi mật khẩu thành công!";
//                    _logger.LogInformation($"Password changed: {user?.Email}");
//                    return RedirectToAction("Index", "Home", new { area = "Customer" });
//                }

//                foreach (var error in result.Errors)
//                {
//                    ModelState.AddModelError("", error.Description);
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error changing password");
//                ModelState.AddModelError("", "Có lỗi xảy ra. Vui lòng thử lại.");
//            }

//            return View(model);
//        }

//        // =============================================
//        // GET: /Account/ForgotPassword
//        // =============================================
//        [HttpGet]
//        [AllowAnonymous]
//        public IActionResult ForgotPassword()
//        {
//            return View();
//        }

//        // =============================================
//        // POST: /Account/ForgotPassword
//        // =============================================
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
//        {
//            if (!ModelState.IsValid)
//                return View(model);

//            try
//            {
//                var user = await _userManager.FindByEmailAsync(model.Email);
//                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
//                {
//                    // Không reveal thông tin người dùng
//                    return View("ForgotPasswordConfirmation");
//                }

//                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
//                var callbackUrl = Url.Action(
//                    nameof(ResetPassword),
//                    values: new { userId = user.Id, code = code },
//                    protocol: Request.Scheme
//                );

//                // TODO: Send email with callbackUrl
//                // await _emailService.SendPasswordResetEmailAsync(user.Email, callbackUrl);

//                _logger.LogInformation($"Password reset email requested: {user.Email}");
//                TempData["Info"] = "Link đặt lại mật khẩu đã được gửi đến email của bạn.";

//                return RedirectToAction(nameof(ForgotPasswordConfirmation));
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error in ForgotPassword");
//            }

//            return View(model);
//        }

//        // =============================================
//        // GET: /Account/ForgotPasswordConfirmation
//        // =============================================
//        [HttpGet]
//        [AllowAnonymous]
//        public IActionResult ForgotPasswordConfirmation()
//        {
//            return View();
//        }

//        // =============================================
//        // GET: /Account/ResetPassword
//        // =============================================
//        [HttpGet]
//        [AllowAnonymous]
//        public IActionResult ResetPassword(string? code = null)
//        {
//            if (code == null)
//            {
//                return View("Error");
//            }

//            return View();
//        }

//        // =============================================
//        // POST: /Account/ResetPassword
//        // =============================================
//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
//        {
//            if (!ModelState.IsValid)
//                return View(model);

//            try
//            {
//                var user = await _userManager.FindByEmailAsync(model.Email);
//                if (user == null)
//                {
//                    return RedirectToAction(nameof(ResetPasswordConfirmation));
//                }

//                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);

//                if (result.Succeeded)
//                {
//                    _logger.LogInformation($"Password reset successfully: {user.Email}");
//                    return RedirectToAction(nameof(ResetPasswordConfirmation));
//                }

//                foreach (var error in result.Errors)
//                {
//                    ModelState.AddModelError("", error.Description);
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error in ResetPassword");
//            }

//            return View(model);
//        }

//        // =============================================
//        // GET: /Account/ResetPasswordConfirmation
//        // =============================================
//        [HttpGet]
//        [AllowAnonymous]
//        public IActionResult ResetPasswordConfirmation()
//        {
//            return View();
//        }

//        // =============================================
//        // GET: /Account/AccessDenied
//        // =============================================
//        [HttpGet]
//        public IActionResult AccessDenied()
//        {
//            return View();
//        }

//        // Helper method
//        private ActionResult RedirectToLocal(string? returnUrl)
//        {
//            if (Url.IsLocalUrl(returnUrl))
//                return Redirect(returnUrl);

//            return RedirectToAction("Index", "Home", new { area = "Customer" });
//        }

//        private async Task LogActivityAsync(string userId, string action, string description)
//        {
//            try
//            {
//                var log = new ActivityLog
//                {
//                    UserId = userId,
//                    Action = action,
//                    Description = description,
//                    IpAddress = GetUserIpAddress(),
//                    CreatedAt = DateTime.Now
//                };

//                _context.ActivityLogs.Add(log);
//                await _context.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error logging activity");
//            }
//        }
//    }
//}

//// =============================================
//// VIEW MODELS - Models/AccountViewModels.cs
//// =============================================
//namespace WebBanHang.Models
//{
//    /// <summary>
//    /// Login view model
//    /// </summary>
//    public class LoginViewModel
//    {
//        [Required(ErrorMessage = "Email không được để trống")]
//        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
//        [Display(Name = "Email")]
//        public string Email { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Mật khẩu không được để trống")]
//        [DataType(DataType.Password)]
//        [Display(Name = "Mật khẩu")]
//        public string Password { get; set; } = string.Empty;

//        [Display(Name = "Ghi nhớ đăng nhập")]
//        public bool RememberMe { get; set; }
//    }

//    /// <summary>
//    /// Register view model
//    /// </summary>
//    public class RegisterViewModel
//    {
//        [Required(ErrorMessage = "Họ tên không được để trống")]
//        [StringLength(100)]
//        [Display(Name = "Họ và Tên")]
//        public string FullName { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Email không được để trống")]
//        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
//        [Display(Name = "Email")]
//        public string Email { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Số điện thoại không được để trống")]
//        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
//        [Display(Name = "Số điện thoại")]
//        public string PhoneNumber { get; set; } = string.Empty;

//        [StringLength(255)]
//        [Display(Name = "Địa chỉ")]
//        public string? Address { get; set; }

//        [Required(ErrorMessage = "Mật khẩu không được để trống")]
//        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự", MinimumLength = 6)]
//        [DataType(DataType.Password)]
//        [Display(Name = "Mật khẩu")]
//        public string Password { get; set; } = string.Empty;

//        [DataType(DataType.Password)]
//        [Display(Name = "Xác nhận mật khẩu")]
//        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp")]
//        public string ConfirmPassword { get; set; } = string.Empty;
//    }

//    /// <summary>
//    /// Edit profile view model
//    /// </summary>
//    public class EditProfileViewModel
//    {
//        [Required(ErrorMessage = "Họ tên không được để trống")]
//        [StringLength(100)]
//        [Display(Name = "Họ và Tên")]
//        public string FullName { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Email không được để trống")]
//        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
//        [Display(Name = "Email")]
//        public string Email { get; set; } = string.Empty;

//        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
//        [Display(Name = "Số điện thoại")]
//        public string? PhoneNumber { get; set; }

//        [StringLength(255)]
//        [Display(Name = "Địa chỉ")]
//        public string? Address { get; set; }
//    }

//    /// <summary>
//    /// Change password view model
//    /// </summary>
//    public class ChangePasswordViewModel
//    {
//        [Required(ErrorMessage = "Mật khẩu cũ không được để trống")]
//        [DataType(DataType.Password)]
//        [Display(Name = "Mật khẩu hiện tại")]
//        public string OldPassword { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
//        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự", MinimumLength = 6)]
//        [DataType(DataType.Password)]
//        [Display(Name = "Mật khẩu mới")]
//        public string NewPassword { get; set; } = string.Empty;

//        [DataType(DataType.Password)]
//        [Display(Name = "Xác nhận mật khẩu")]
//        [Compare("NewPassword", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp")]
//        public string ConfirmPassword { get; set; } = string.Empty;
//    }

//    /// <summary>
//    /// Forgot password view model
//    /// </summary>
//    public class ForgotPasswordViewModel
//    {
//        [Required(ErrorMessage = "Email không được để trống")]
//        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
//        [Display(Name = "Email")]
//        public string Email { get; set; } = string.Empty;
//    }

//    /// <summary>
//    /// Reset password view model
//    /// </summary>
//    public class ResetPasswordViewModel
//    {
//        [Required(ErrorMessage = "Email không được để trống")]
//        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
//        [Display(Name = "Email")]
//        public string Email { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Mật khẩu mới không được để trống")]
//        [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự", MinimumLength = 6)]
//        [DataType(DataType.Password)]
//        [Display(Name = "Mật khẩu mới")]
//        public string Password { get; set; } = string.Empty;

//        [DataType(DataType.Password)]
//        [Display(Name = "Xác nhận mật khẩu")]
//        [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp")]
//        public string ConfirmPassword { get; set; } = string.Empty;

//        [Required]
//        public string Code { get; set; } = string.Empty;
//    }

//    /// <summary>
//    /// Login with 2FA view model
//    /// </summary>
//    public class LoginWith2faViewModel
//    {
//        [Required]
//        [StringLength(7, MinimumLength = 6, ErrorMessage = "Mã 2FA phải từ 6-7 ký tự")]
//        [DataType(DataType.Text)]
//        [Display(Name = "Mã xác thực")]
//        public string TwoFactorCode { get; set; } = string.Empty;

//        [Display(Name = "Ghi nhớ máy này")]
//        public bool RememberMachine { get; set; }

//        public bool RememberMe { get; set; }
//    }
//}

//// =============================================
//// Extensions - IdentityExtensions.cs
//// =============================================
//namespace WebBanHang.Extensions
//{
//    public static class IdentityExtensions
//    {
//        /// <summary>
//        /// Configure Identity options
//        /// </summary>
//        public static IServiceCollection AddIdentityConfiguration(
//            this IServiceCollection services)
//        {
//            services.Configure<IdentityOptions>(options =>
//            {
//                // Password settings
//                options.Password.RequireDigit = false;
//                options.Password.RequireLowercase = false;
//                options.Password.RequireNonAlphanumeric = false;
//                options.Password.RequireUppercase = false;
//                options.Password.RequiredLength = 6;

//                // Lockout settings
//                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
//                options.Lockout.MaxFailedAccessAttemptsBeforeLockout = 5;
//                options.Lockout.AllowedForNewUsers = true;

//                // User settings
//                options.User.AllowedUserNameCharacters =
//                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
//                options.User.RequireUniqueEmail = true;

//                // SignIn settings
//                options.SignIn.RequireConfirmedEmail = false;
//                options.SignIn.RequireConfirmedPhoneNumber = false;
//            });

//            services.ConfigureApplicationCookie(options =>
//            {
//                options.Cookie.HttpOnly = true;
//                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//                options.Cookie.SameSite = SameSiteMode.Lax;
//                options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
//                options.LoginPath = "/Account/Login";
//                options.LogoutPath = "/Account/Logout";
//                options.AccessDeniedPath = "/Account/AccessDenied";
//                options.SlidingExpiration = true;
//            });

//            return services;
//        }
//    }
//}