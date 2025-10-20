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

            string[] roleNames = { "Admin", "Customer"};

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
                new Category
                {
                    
                    CategoryName = "Pizza",
                    Description = "Các loại Pizza truyền thống và hiện đại",
                    ImageUrl = "/images/categories/pizza.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Category
                {
                   
                    CategoryName = "Nước Uống",
                    Description = "Nước ngọt, nước ép, trà sữa, cà phê",
                    ImageUrl = "/images/categories/drinks.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Category
                {
                  
                    CategoryName = "Cơm",
                    Description = "Cơm truyền thống Việt Nam",
                    ImageUrl = "/images/categories/rice.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Category
                {
                   
                    CategoryName = "Mì & Phở",
                    Description = "Mì Ý, phở, bún, miến các loại",
                    ImageUrl = "/images/categories/noodles.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Category
                {
                   
                    CategoryName = "Đồ Ăn Nhanh",
                    Description = "Burger, gà rán, khoai tây chiên",
                    ImageUrl = "/images/categories/fastfood.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                },
                new Category
                {
                
                    CategoryName = "Món Ăn Vặt",
                    Description = "Các món ăn vặt phổ biến",
                    ImageUrl = "/images/categories/snacks.jpg",
                    IsActive = true,
                    CreatedAt = DateTime.Now
                }
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
                // Pizza
                new Food { FoodName = "Pizza Hải Sản", Description = "Pizza với tôm, mực, nghêu tươi ngon", Price = 159000, CategoryId = 1, ImageUrl = "/images/foods/pizza-seafood.jpg", IsAvailable = true, Rating = 4.5m, CreatedAt = DateTime.Now },
                new Food {FoodName = "Pizza Pepperoni", Description = "Pizza với xúc xích Pepperoni cay", Price = 139000, CategoryId = 1, ImageUrl = "/images/foods/pizza-pepperoni.jpg", IsAvailable = true, Rating = 4.7m, CreatedAt = DateTime.Now },
                new Food {FoodName = "Pizza Phô Mai", Description = "Pizza 4 loại phô mai đặc biệt", Price = 149000, CategoryId = 1, ImageUrl = "/images/foods/pizza-cheese.jpg", IsAvailable = true, Rating = 4.3m, CreatedAt = DateTime.Now },
                new Food { FoodName = "Pizza Gà BBQ", Description = "Pizza với gà nướng BBQ", Price = 145000, CategoryId = 1, ImageUrl = "/images/foods/pizza-bbq.jpg", IsAvailable = true, Rating = 4.6m, CreatedAt = DateTime.Now },

                // Nước Uống
                new Food {FoodName = "Coca Cola", Description = "Nước ngọt có gas", Price = 15000, CategoryId = 2, ImageUrl = "/images/foods/coca.jpg", IsAvailable = true, Rating = 4.8m, CreatedAt = DateTime.Now },
                new Food {FoodName = "Trà Sữa Trân Châu", Description = "Trà sữa Đài Loan với trân châu đen", Price = 35000, CategoryId = 2, ImageUrl = "/images/foods/milktea.jpg", IsAvailable = true, Rating = 4.6m, CreatedAt = DateTime.Now },
                new Food {FoodName = "Nước Cam Ép", Description = "Nước cam tươi nguyên chất", Price = 25000, CategoryId = 2, ImageUrl = "/images/foods/orange-juice.jpg", IsAvailable = true, Rating = 4.7m, CreatedAt = DateTime.Now },
                new Food {FoodName = "Cà Phê Sữa Đá", Description = "Cà phê phin truyền thống", Price = 20000, CategoryId = 2, ImageUrl = "/images/foods/coffee.jpg", IsAvailable = true, Rating = 4.5m, CreatedAt = DateTime.Now },

                // Cơm
                new Food {FoodName = "Cơm Gà Xối Mỡ", Description = "Cơm gà Hải Nam thơm ngon", Price = 45000, CategoryId = 3, ImageUrl = "/images/foods/chicken-rice.jpg", IsAvailable = true, Rating = 4.4m, CreatedAt = DateTime.Now },
                new Food {FoodName = "Cơm Tấm Sườn Bì", Description = "Cơm tấm Sài Gòn đặc trưng", Price = 50000, CategoryId = 3, ImageUrl = "/images/foods/com-tam.jpg", IsAvailable = true, Rating = 4.5m, CreatedAt = DateTime.Now },
                new Food {FoodName = "Cơm Chiên Dương Châu", Description = "Cơm chiên với tôm, xúc xích", Price = 55000, CategoryId = 3, ImageUrl = "/images/foods/fried-rice.jpg", IsAvailable = true, Rating = 4.3m, CreatedAt = DateTime.Now },

                // Mì & Phở
                new Food {FoodName = "Phở Bò Tái", Description = "Phở bò truyền thống Hà Nội", Price = 55000, CategoryId = 4, ImageUrl = "/images/foods/pho-bo.jpg", IsAvailable = true, Rating = 4.7m,  CreatedAt = DateTime.Now },
                new Food {FoodName = "Mì Ý Carbonara", Description = "Mì Ý sốt kem phô mai", Price = 75000, CategoryId = 4, ImageUrl = "/images/foods/pasta-carbonara.jpg", IsAvailable = true, Rating = 4.4m, CreatedAt = DateTime.Now },
                new Food {FoodName = "Bún Chả Hà Nội", Description = "Bún chả truyền thống", Price = 50000, CategoryId = 4, ImageUrl = "/images/foods/bun-cha.jpg", IsAvailable = true, Rating = 4.6m,  CreatedAt = DateTime.Now },

                // Đồ Ăn Nhanh
                new Food {FoodName = "Gà Rán KFC Style", Description = "Gà rán giòn tan kiểu Mỹ", Price = 85000, CategoryId = 5, ImageUrl = "/images/foods/fried-chicken.jpg", IsAvailable = true, Rating = 4.6m, CreatedAt = DateTime.Now },
                new Food {FoodName = "Burger Bò Phô Mai", Description = "Burger bò Úc với phô mai", Price = 65000, CategoryId = 5, ImageUrl = "/images/foods/burger.jpg", IsAvailable = true, Rating = 4.5m,  CreatedAt = DateTime.Now },
                new Food {FoodName = "Khoai Tây Chiên", Description = "Khoai tây chiên giòn", Price = 30000, CategoryId = 5, ImageUrl = "/images/foods/french-fries.jpg", IsAvailable = true, Rating = 4.4m, CreatedAt = DateTime.Now },

                // Món Ăn Vặt
                new Food {FoodName = "Bánh Tráng Trộn", Description = "Bánh tráng trộn Sài Gòn", Price = 25000, CategoryId = 6, ImageUrl = "/images/foods/banh-trang.jpg", IsAvailable = true, Rating = 4.3m,  CreatedAt = DateTime.Now },
                new Food {FoodName = "Chả Giò", Description = "Chả giò chiên giòn", Price = 35000, CategoryId = 6, ImageUrl = "/images/foods/cha-gio.jpg", IsAvailable = true, Rating = 4.5m,  CreatedAt = DateTime.Now },
                new Food {FoodName = "Nem Nướng", Description = "Nem nướng Nha Trang", Price = 40000, CategoryId = 6, ImageUrl = "/images/foods/nem-nuong.jpg", IsAvailable = true, Rating = 4.4m,  CreatedAt = DateTime.Now }
            };

            await _context.Foods.AddRangeAsync(foods);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Seeded {foods.Length} foods");
        }
        // =============================================
        // 5. SEED CARTS & CART ITEMS
        // =============================================
        // =============================================
        // 5. SEED CARTS & CART ITEMS
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
        // 5. SEED ORDER
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

    }
}
