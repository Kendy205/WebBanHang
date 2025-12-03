📁 Cấu Trúc Dự Án
```text WebBanHang (Solution)
├── WebBanHang (Main MVC Project)
│   ├── Areas/
│   │   ├── Admin/
│   │   │   ├── Controllers/
│   │   │   │   ├── CategoriesController.cs
│   │   │   │   ├── DashboardController.cs
│   │   │   │   ├── FoodsController.cs
│   │   │   │   ├── OrdersController.cs
│   │   │   │   └── UsersController.cs
│   │   │   └── Views/
│   │   │       ├── Categories/
│   │   │       ├── Dashboard/
│   │   │       ├── Foods/
│   │   │       ├── Orders/
│   │   │       └── Users/
│   │   │
│   │   └── Customer/
│   │       ├── Controllers/
│   │       │   ├── CartController.cs
│   │       │   ├── FoodController.cs
│   │       │   ├── HomeController.cs
│   │       │   └── OrderController.cs
│   │       └── Views/
│   │           ├── Cart/
│   │           ├── Food/
│   │           ├── Home/
│   │           └── Order/
│   │
│   ├── Controllers/
│   │   └── Api/
│   │       ├── CartApiController.cs
│   │       └── FoodsApiController.cs
│   │
│   ├── DTOs/
│   │   ├── AddToCartRequestDTO.cs
│   │   ├── DanhMucDTO.cs
│   │   ├── SanPhamDTO.cs
│   │   └── UpdateCartRequestDTO.cs
│   │
│   ├── FileUpload/
│   │   └── IFileUpload/
│   │       └── IBufferedFileUploadService.cs
│   │
│   ├── ViewComponents/
│   │   ├── RenderCaroselDanhMucViewComponent.cs
│   │   ├── RenderDanhMucViewComponent.cs
│   │   └── RenderFeaturedProductViewComponent.cs
│   │
│   ├── wwwroot/
│   │   ├── css/
│   │   ├── js/
│   │   └── images/ (Uploaded files)
│   │
│   └── Program.cs
│
├── WebBanHang.BLL (Business Logic Layer)
│   └── IServices/
│       ├── ICartService.cs
│       ├── ICategoryService.cs
│       ├── IFoodService.cs
│       └── IOrderService.cs
│
├── WebBanHang.DAL (Data Access Layer)
│   ├── Data/
│   │   └── ApplicationDbContext.cs
│   ├── Migrations/
│   ├── Repository/
│   │   ├── IRepository/
│   │   │   └── IRepository.cs (Generic)
│   │   └── Repository.cs (Generic)
│   └── DbSeeder.cs
│
└── WebBanHang.Models (Model Layer)
    └── Models/
        ├── ApplicationUser.cs
        ├── Cart.cs
        ├── CartItem.cs
        ├── Category.cs
        ├── Food.cs
        ├── Order.cs
        ├── OrderDetail.cs
        └── Payment.cs


📥 Cài Đặt
Yêu Cầu

Visual Studio 2019 hoặc cao hơn
SQL Server 2017 hoặc cao hơn
.NET Framework 8 trở lên

Bước 1: Clone Repository
git clone https://github.com/Kendy205/WebBanHang.git

Bước 2: Restore NuGet Packages
bash# Trong Package Manager Console (Visual Studio)
Update-Package

Bước 3: Cập nhật Connection String
Mở Web.config và cập nhật connection string trong app.setting

Bước 4: Tạo Database (nở package console)
Update-Database

Bước 5: Chạy Application
bash# Nhấn F5 hoặc Ctrl+F5 để debug
s
Bước 6: Tài Khoản Mặc Định
Admin:
Email: admin@foodorder.com
Password: Admin@123
Customer:
Email: customer1@gmail.com
Password: Customer@123
