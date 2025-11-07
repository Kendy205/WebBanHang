
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
