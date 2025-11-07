using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebBanHang.DAL.Data;
using WebBanHang.Models.Models;

namespace WebBanHang.DAL
{
    public class DbSeeder
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<DbSeeder> _logger;

        public DbSeeder(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger<DbSeeder> logger)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        // Main Seed Method
        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Starting database seeding...");

                // Ensure database is created
                await _context.Database.EnsureCreatedAsync();

                // Seed in order
                await SeedRolesAsync();
                await SeedUsersAsync();
                await SeedCategoriesAsync();
                await SeedFoodsAsync();
                await SeedCartAsync();
                await SeedOrderAsync();
                //await SeedVouchersAsync();

                _logger.LogInformation("Database seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while seeding the database");
                throw;
            }
        }

        // =============================================
        // 1. SEED ROLES
        // =============================================
        private async Task SeedRolesAsync()
        {
            _logger.LogInformation("Seeding roles...");

            string[] roleNames = { "Admin", "Customer" };

            foreach (var roleName in roleNames)
            {
                if (!await _roleManager.RoleExistsAsync(roleName))
                {
                    var role = new IdentityRole(roleName);
                    var result = await _roleManager.CreateAsync(role);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Role '{roleName}' created successfully");
                    }
                    else
                    {
                        _logger.LogWarning($"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        // =============================================
        // 2. SEED USERS
        // =============================================
        private async Task SeedUsersAsync()
        {
            _logger.LogInformation("Seeding users...");

            // Admin User
            await CreateUserAsync(
                email: "admin@foodorder.com",
                password: "Admin@123",
                fullName: "Administrator",
                phoneNumber: "0901234567",
                address: "123 Admin Street, Hà Nội",
                role: "Admin"
            );



            // Customer 1
            await CreateUserAsync(
                email: "customer1@gmail.com",
                password: "Customer@123",
                fullName: "Trần Thị Lan",
                phoneNumber: "0912345678",
                address: "456 Láng Hạ, Đống Đa, Hà Nội",
                role: "Customer"
            );

            // Customer 2
            await CreateUserAsync(
                email: "customer2@gmail.com",
                password: "Customer@123",
                fullName: "Lê Văn Bình",
                phoneNumber: "0923456789",
                address: "789 Giảng Võ, Ba Đình, Hà Nội",
                role: "Customer"
            );

            // Customer 3
            await CreateUserAsync(
                email: "customer3@gmail.com",
                password: "Customer@123",
                fullName: "Phạm Minh Tuấn",
                phoneNumber: "0934567890",
                address: "321 Kim Mã, Ba Đình, Hà Nội",
                role: "Customer"
            );
        }

        private async Task CreateUserAsync(
            string email,
            string password,
            string fullName,
            string phoneNumber,
            string address,
            string role)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                _logger.LogInformation($"User '{email}' already exists");
                return;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FulName = fullName,
                PhoneNumber = phoneNumber,
                Address = address,
                EmailConfirmed = true,

            };

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, role);
                _logger.LogInformation($"User '{email}' created successfully with role '{role}'");
            }
            else
            {
                _logger.LogWarning($"Failed to create user '{email}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        // =============================================
        // 3. SEED CATEGORIES
        // =============================================
        private async Task SeedCategoriesAsync()
        {
            if (await _context.Categories.AnyAsync())
            {
                _logger.LogInformation("Categories already seeded");
                return;
            }

            _logger.LogInformation("Seeding categories...");

            var categories = new[]
            {
            new Category { CategoryName = "Pizza", Description = "Các loại Pizza truyền thống và hiện đại", ImageUrl = "/img/pizza.jpeg", IsActive = true, CreatedAt = DateTime.Now },
            new Category { CategoryName = "Nước Uống", Description = "Nước ngọt, nước ép, trà sữa, cà phê", ImageUrl = "/img/drinks.jpeg", IsActive = true, CreatedAt = DateTime.Now },
            new Category { CategoryName = "Cơm", Description = "Cơm truyền thống Việt Nam", ImageUrl = "/img/rice.jpeg", IsActive = true, CreatedAt = DateTime.Now },
            new Category { CategoryName = "Mì & Phở", Description = "Mì Ý, phở, bún, miến", ImageUrl = "/img/noodles.jpeg", IsActive = true, CreatedAt = DateTime.Now },
            new Category { CategoryName = "Đồ Ăn Nhanh", Description = "Burger, gà rán, khoai tây", ImageUrl = "/img/fastfood.jpeg", IsActive = true, CreatedAt = DateTime.Now },
            new Category { CategoryName = "Món Ăn Vặt", Description = "Các món ăn vặt phổ biến", ImageUrl = "/img/snacks.jpeg", IsActive = true, CreatedAt = DateTime.Now },
            new Category { CategoryName = "Hải Sản", Description = "Các món hải sản tươi sống", ImageUrl = "/img/seafood.jpeg", IsActive = true, CreatedAt = DateTime.Now },
            new Category { CategoryName = "Lẩu", Description = "Các loại lẩu", ImageUrl = "/img/hotpot.jpeg", IsActive = true, CreatedAt = DateTime.Now },
            new Category { CategoryName = "Món Chay", Description = "Các món chay thanh đạm", ImageUrl = "/img/vegan.jpeg", IsActive = true, CreatedAt = DateTime.Now },
            new Category { CategoryName = "Tráng Miệng", Description = "Các món kem, chè, bánh ngọt", ImageUrl = "/img/dessert.jpeg", IsActive = true, CreatedAt = DateTime.Now }
            };

            await _context.Categories.AddRangeAsync(categories);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Seeded {categories.Length} categories");
        }

        // =============================================
        // 4. SEED FOODS
        // =============================================
        private async Task SeedFoodsAsync()
        {
            if (await _context.Foods.AnyAsync())
            {
                _logger.LogInformation("Foods already seeded");
                return;
            }

            _logger.LogInformation("Seeding foods...");

            var foods = new[]
            {
            // ========= CATEGORY 1: Pizza =========
            new Food { FoodName = "Pizza Hải Sản", Description = "Tôm, mực, nghêu", Price = 159000, CategoryId = 1, ImageUrl = "/img/pizza-seafood.jpeg", IsAvailable = true, Rating = 4.5m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Pizza Pepperoni", Description = "Xúc xích cay", Price = 139000, CategoryId = 1, ImageUrl = "/img/pizza-pepperoni.jpeg", IsAvailable = true, Rating = 4.7m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Pizza Phô Mai", Description = "4 loại phô mai", Price = 149000, CategoryId = 1, ImageUrl = "/img/pizza-cheese.jpeg", IsAvailable = true, Rating = 4.3m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Pizza Gà BBQ", Description = "Gà sốt BBQ", Price = 145000, CategoryId = 1, ImageUrl = "/img/pizza-bbq.jpeg", IsAvailable = true, Rating = 4.6m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Pizza Hawaiian", Description = "Dứa + thịt nguội", Price = 139000, CategoryId = 1, ImageUrl = "/img/pizza-hawaiian.jpeg", IsAvailable = true, Rating = 4.2m, CreatedAt = DateTime.Now },

            // ========= CATEGORY 2: Drinks =========
            new Food { FoodName = "Coca Cola", Description = "Nước ngọt có gas", Price = 15000, CategoryId = 2, ImageUrl = "/img/coca.jpeg", IsAvailable = true, Rating = 4.8m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Trà Sữa Trân Châu", Description = "Trà sữa Đài Loan", Price = 35000, CategoryId = 2, ImageUrl = "/img/milktea.jpeg", IsAvailable = true, Rating = 4.6m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Nước Cam", Description = "Cam tươi", Price = 25000, CategoryId = 2, ImageUrl = "/img/orange-juice.jpeg", IsAvailable = true, Rating = 4.7m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Cà Phê Sữa Đá", Description = "Cà phê phin", Price = 20000, CategoryId = 2, ImageUrl = "/img/coffee.jpeg", IsAvailable = true, Rating = 4.5m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Trà Đào", Description = "Trà đào cam sả", Price = 30000, CategoryId = 2, ImageUrl = "/img/tra-dao.jpeg", IsAvailable = true, Rating = 4.7m, CreatedAt = DateTime.Now },

            // ========= CATEGORY 3: Rice =========
            new Food { FoodName = "Cơm Gà Xối Mỡ", Description = "Gà xối mỡ", Price = 45000, CategoryId = 3, ImageUrl = "/img/com-ga-xoi-mo.jpeg", IsAvailable = true, Rating = 4.4m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Cơm Tấm Sườn Bì", Description = "Cơm tấm Sài Gòn", Price = 50000, CategoryId = 3, ImageUrl = "/img/com-tam.jpeg", IsAvailable = true, Rating = 4.5m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Cơm Chiên Dương Châu", Description = "Cơm chiên", Price = 55000, CategoryId = 3, ImageUrl = "/img/fried-rice.jpeg", IsAvailable = true, Rating = 4.3m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Cơm Cá Kho", Description = "Cá kho tộ", Price = 50000, CategoryId = 3, ImageUrl = "/img/com-ca-kho.jpeg", IsAvailable = true, Rating = 4.6m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Cơm Gà Luộc", Description = "Gà luộc ta", Price = 45000, CategoryId = 3, ImageUrl = "/img/com-ga.jpeg", IsAvailable = true, Rating = 4.4m, CreatedAt = DateTime.Now },

            // ========= CATEGORY 4: Noodles =========
            new Food { FoodName = "Phở Bò", Description = "Phở bò tái", Price = 55000, CategoryId = 4, ImageUrl = "/img/pho-bo.jpeg", IsAvailable = true, Rating = 4.7m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Mì Ý Carbonara", Description = "Nước sốt kem", Price = 75000, CategoryId = 4, ImageUrl = "/img/pasta-carbonara.jpeg", IsAvailable = true, Rating = 4.4m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Bún Chả", Description = "Hà Nội truyền thống", Price = 50000, CategoryId = 4, ImageUrl = "/img/bun-cha.jpeg", IsAvailable = true, Rating = 4.6m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Hủ Tiếu Nam Vang", Description = "Hủ tiếu đặc sản", Price = 50000, CategoryId = 4, ImageUrl = "/img/hu-tieu.jpeg", IsAvailable = true, Rating = 4.5m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Bún Bò Huế", Description = "Đặc sản Huế", Price = 60000, CategoryId = 4, ImageUrl = "/img/bun-bo-hue.jpeg", IsAvailable = true, Rating = 4.8m, CreatedAt = DateTime.Now },

            // ========= CATEGORY 5: Fastfood =========
            new Food { FoodName = "Gà Rán", Description = "Gà rán kiểu Mỹ", Price = 85000, CategoryId = 5, ImageUrl = "/img/fried-chicken.jpeg", IsAvailable = true, Rating = 4.6m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Burger Bò", Description = "Burger bò phô mai", Price = 65000, CategoryId = 5, ImageUrl = "/img/burger.jpeg", IsAvailable = true, Rating = 4.5m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Khoai Tây Chiên", Description = "Khoai tây chiên giòn", Price = 30000, CategoryId = 5, ImageUrl = "/img/fries.jpeg", IsAvailable = true, Rating = 4.4m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Xúc Xích Rán", Description = "Xúc xích Đức", Price = 30000, CategoryId = 5, ImageUrl = "/img/sausage.jpeg", IsAvailable = true, Rating = 4.3m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Hotdog", Description = "Hotdog kiểu Mỹ", Price = 40000, CategoryId = 5, ImageUrl = "/img/hotdog.jpeg", IsAvailable = true, Rating = 4.4m, CreatedAt = DateTime.Now },

            // ========= CATEGORY 6: Snacks =========
            new Food { FoodName = "Bánh Tráng Trộn", Description = "Món ăn vặt nổi tiếng", Price = 25000, CategoryId = 6, ImageUrl = "/img/banh-trang.jpeg", IsAvailable = true, Rating = 4.3m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Phô Mai Que", Description = "Phô mai que chiên", Price = 25000, CategoryId = 6, ImageUrl = "/img/pho-mai-que.jpeg", IsAvailable = true, Rating = 4.5m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Tokbokki", Description = "Bánh gạo Hàn Quốc", Price = 35000, CategoryId = 6, ImageUrl = "/img/tokbokki.jpeg", IsAvailable = true, Rating = 4.6m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Bắp Xào", Description = "Bắp xào mỡ hành", Price = 20000, CategoryId = 6, ImageUrl = "/img/bap-xao.jpeg", IsAvailable = true, Rating = 4.4m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Xoài Lắc", Description = "Xoài lắc muối ớt", Price = 25000, CategoryId = 6, ImageUrl = "/img/xoai-lac.jpeg", IsAvailable = true, Rating = 4.5m, CreatedAt = DateTime.Now },

            // ========= CATEGORY 7: Seafood =========
            new Food { FoodName = "Tôm Nướng Muối Ớt", Description = "Tôm sú nướng", Price = 120000, CategoryId = 7, ImageUrl = "/img/tom-nuong.jpeg", IsAvailable = true, Rating = 4.7m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Mực Chiên Giòn", Description = "Mực chiên xù", Price = 90000, CategoryId = 7, ImageUrl = "/img/muc-chien.jpeg", IsAvailable = true, Rating = 4.5m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Nghêu Hấp Sả", Description = "Nghêu tươi hấp sả", Price = 80000, CategoryId = 7, ImageUrl = "/img/ngheu.jpeg", IsAvailable = true, Rating = 4.6m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Cua Rang Me", Description = "Cua rang me", Price = 150000, CategoryId = 7, ImageUrl = "/img/cua-rang-me.jpeg", IsAvailable = true, Rating = 4.8m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Hàu Nướng", Description = "Hàu phô mai", Price = 120000, CategoryId = 7, ImageUrl = "/img/hau-nuong.jpeg", IsAvailable = true, Rating = 4.7m, CreatedAt = DateTime.Now },

            // ========= CATEGORY 8: Hotpot =========
            new Food { FoodName = "Lẩu Thái", Description = "Lẩu Thái chua cay", Price = 180000, CategoryId = 8, ImageUrl = "/img/lau-thai.jpeg", IsAvailable = true, Rating = 4.7m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Lẩu Hải Sản", Description = "Lẩu hải sản tươi sống", Price = 200000, CategoryId = 8, ImageUrl = "/img/lau-hai-san.jpeg", IsAvailable = true, Rating = 4.8m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Lẩu Bò", Description = "Lẩu bò nhúng dấm", Price = 170000, CategoryId = 8, ImageUrl = "/img/lau-bo.jpeg", IsAvailable = true, Rating = 4.6m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Lẩu Gà Lá Giang", Description = "Đặc sản miền Tây", Price = 160000, CategoryId = 8, ImageUrl = "/img/lau-ga.jpeg", IsAvailable = true, Rating = 4.7m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Lẩu Nấm", Description = "Lẩu chay thanh đạm", Price = 150000, CategoryId = 8, ImageUrl = "/img/lau-nam.jpeg", IsAvailable = true, Rating = 4.5m, CreatedAt = DateTime.Now },

            // ========= CATEGORY 9: Vegan =========
            new Food { FoodName = "Đậu Hũ Sốt Cà", Description = "Đậu hũ sốt cà", Price = 35000, CategoryId = 9, ImageUrl = "/img/dau-hu.jpeg", IsAvailable = true, Rating = 4.5m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Cơm Chay Thập Cẩm", Description = "Cơm chay", Price = 45000, CategoryId = 9, ImageUrl = "/img/com-chay.jpeg", IsAvailable = true, Rating = 4.6m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Mì Xào Chay", Description = "Mì rau củ", Price = 40000, CategoryId = 9, ImageUrl = "/img/mi-chay.jpeg", IsAvailable = true, Rating = 4.4m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Gỏi Cuốn Chay", Description = "Gỏi cuốn chay", Price = 30000, CategoryId = 9, ImageUrl = "/img/goi-cuon-chay.jpeg", IsAvailable = true, Rating = 4.6m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Bún Riêu Chay", Description = "Bún riêu chay", Price = 45000, CategoryId = 9, ImageUrl = "/img/bun-rieu-chay.jpeg", IsAvailable = true, Rating = 4.5m, CreatedAt = DateTime.Now },

            // ========= CATEGORY 10: Dessert =========
            new Food { FoodName = "Kem Vani", Description = "Kem lạnh", Price = 20000, CategoryId = 10, ImageUrl = "/img/icecream.jpeg", IsAvailable = true, Rating = 4.7m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Chè Khúc Bạch", Description = "Chè thơm ngon", Price = 30000, CategoryId = 10, ImageUrl = "/img/che-khuc-bach.jpeg", IsAvailable = true, Rating = 4.8m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Bánh Flan", Description = "Flan caramel", Price = 15000, CategoryId = 10, ImageUrl = "/img/flan.jpeg", IsAvailable = true, Rating = 4.7m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Bánh Su Kem", Description = "Su kem mềm", Price = 25000, CategoryId = 10, ImageUrl = "/img/su-kem.jpeg", IsAvailable = true, Rating = 4.5m, CreatedAt = DateTime.Now },
            new Food { FoodName = "Sữa Chua Dẻo", Description = "Sữa chua dẻo", Price = 20000, CategoryId = 10, ImageUrl = "/img/sua-chua.jpeg", IsAvailable = true, Rating = 4.6m, CreatedAt = DateTime.Now }
            };

            await _context.Foods.AddRangeAsync(foods);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Seeded {foods.Length} foods");
        }
        // =============================================
        // 5. SEED CARTS & CART ITEMS
        // =============================================
        // =============================================
        // 6. SEED CARTS & CART ITEMS
        // =============================================
        private async Task SeedCartAsync()
        {
            if (await _context.Carts.AnyAsync())
            {
                _logger.LogInformation("Carts already seeded");
                return;
            }

            _logger.LogInformation("Seeding carts...");

            // Lấy danh sách user (chỉ Customer)
            // SỬA: Lấy tất cả user có role là Customer để đảm bảo tìm đúng ID.
            var customerUserIds = await _userManager.GetUsersInRoleAsync("Customer");

            // Đảm bảo lấy các user object từ DB để EF Core theo dõi mối quan hệ
            var customers = await _context.Users
                .Where(u => customerUserIds.Select(c => c.Id).Contains(u.Id))
                .ToListAsync();

            // Lấy danh sách món ăn (Foods) để chọn ngẫu nhiên
            var foods = await _context.Foods.ToListAsync();

            var carts = new List<Cart>();
            // BỎ cartItems list riêng biệt (chỉ dùng navigation property)

            var random = new Random();

            foreach (var customer in customers)
            {
                // Tạo 1 giỏ hàng cho mỗi customer
                var cart = new Cart
                {
                    // GÁN OBJECT USER, không cần dùng UserId
                    User = customer,
                    CreatedAt = DateTime.Now.AddDays(-random.Next(1, 5)),
                    UpdatedAt = DateTime.Now,
                    CartItems = new List<CartItem>() // Khởi tạo list chi tiết
                };

                // Thêm một số món ngẫu nhiên vào giỏ
                int numberOfItems = random.Next(2, 5);
                var selectedFoods = foods.OrderBy(x => Guid.NewGuid()).Take(numberOfItems).ToList();

                foreach (var food in selectedFoods)
                {
                    var item = new CartItem
                    {
                        // BỎ GÁN Cart = cart, (vì nó sẽ được gán tự động khi thêm vào list)
                        Food = food,
                        Quantity = random.Next(1, 4), // 1–3 món
                        Price = food.Price,
                        AddedAt = DateTime.Now
                    };

                    // THÊM CartItem VÀO NAVIGATION PROPERTY
                    cart.CartItems.Add(item);
                }

                carts.Add(cart);
            }

            // Chỉ cần thêm Carts, EF Core sẽ tự động chèn CartItems
            await _context.Carts.AddRangeAsync(carts);
            // BỎ DÒNG NÀY: await _context.CartItems.AddRangeAsync(cartItems);

            await _context.SaveChangesAsync();

            int totalCartItems = carts.Sum(c => c.CartItems.Count);
            _logger.LogInformation($"Seeded {carts.Count} carts and {totalCartItems} cart items");
        }
        // =============================================
        // 7. SEED ORDER
        // =============================================
        public async Task SeedOrderAsync()
        {
            if (await _context.Orders.AnyAsync()) return;

            // Lấy toàn bộ giỏ hàng có dữ liệu (quan trọng: Phải có Include)
            var carts = await _context.Carts
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Food)
                .Include(c => c.User) // Cần Include User để truy cập UserId chính xác
                .ToListAsync();

            if (!carts.Any())
            {
                _logger.LogWarning("⚠️ Không có giỏ hàng nào để tạo đơn hàng.");
                return;
            }

            int orderId = 1;
            int orderDetailId = 1;
            int paymentId = 1;
            int deliveryId = 1;

            foreach (var cart in carts)
            {
                if (cart.CartItems == null || !cart.CartItems.Any())
                    continue; // Bỏ qua giỏ hàng trống

                // Tính tổng tiền giỏ hàng
                decimal totalAmount = cart.CartItems.Sum(i => i.Quantity * i.Price);

                // === Tạo đơn hàng ===
                var order = new Order
                {
                    // OrderId = orderId++,
                    OrderCode = $"ORD{orderId:000}",
                    User = cart.User,
                    OrderDate = DateTime.Now,
                    TotalAmount = totalAmount,
                    Status = "Completed",
                    PaymentMethod = "Cash on Delivery",
                    ShippingAddress = "123 Đường Lê Lợi, Quận 1, TP.HCM",
                    PhoneNumber = "0909123456",
                    Notes = "Tạo đơn hàng mẫu từ giỏ hàng",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };
                orderId++;
                // === Tạo chi tiết đơn hàng ===
                var orderDetails = cart.CartItems.Select(ci => new OrderDetail
                {
                    //OrderDetailId = orderDetailId++,
                    Order = order,
                    Food = ci.Food,
                    FoodName = ci.Food?.FoodName ?? "Không rõ",
                    Quantity = ci.Quantity,
                    Price = ci.Price
                }).ToList();

                order.OrderDetails = orderDetails;

                // === Tạo thanh toán ===
                order.Payment = new Payment
                {
                    //PaymentId = paymentId++,
                    Order = order,
                    PaymentMethod = order.PaymentMethod,
                    Amount = order.TotalAmount,
                    Status = "Paid",
                    TransactionId = $"TRANS-{Guid.NewGuid().ToString().Substring(0, 8)}",
                    PaymentDate = DateTime.Now,
                    CreatedAt = DateTime.Now
                };

                // === Tạo giao hàng (Delivery) ===
                order.Delivery = new Delivery
                {
                    //DeliveryId = deliveryId++,
                    Order = order,
                    ShipperName = "Nguyễn Văn A",
                    ShipperPhone = "0909888777",
                    EstimatedTime = DateTime.Now.AddHours(2),
                    ActualDeliveryTime = DateTime.Now.AddHours(1),
                    Status = "Delivered",
                    Notes = "Giao hàng thành công cho khách",
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Thêm vào DbContext
                await _context.Orders.AddAsync(order);
                await _context.OrderDetails.AddRangeAsync(orderDetails);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("✅ Đã seed Order, OrderDetail, Payment, Delivery từ giỏ hàng thành công.");
        }

        public async Task EnsureDeletedAsync()
        {
            _logger.LogInformation("Attempting to delete database...");
            var deleted = await _context.Database.EnsureDeletedAsync();
            if (deleted)
            {
                _logger.LogInformation("Database deleted successfully.");
            }
            else
            {
                _logger.LogInformation("Database did not exist or could not be deleted.");
            }
        }
        public async Task ResetAndSeedAsync()
        {
            try
            {
                _logger.LogInformation("Starting database **RESET and** seeding...");

                // ⚠️ BƯỚC 1: XÓA DATABASE
                await EnsureDeletedAsync();

                // ⚠️ BƯỚC 2: TẠO LẠI DATABASE
                _logger.LogInformation("Ensuring database is created...");
                await _context.Database.EnsureCreatedAsync();

                // Seed in order
                await SeedRolesAsync();
                await SeedUsersAsync();
                await SeedCategoriesAsync();
                await SeedFoodsAsync();
                await SeedCartAsync();
                await SeedOrderAsync();

                _logger.LogInformation("Database RESET and seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while resetting and seeding the database");
                throw;
            }
        }
    }
}
