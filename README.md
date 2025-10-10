<pre> ```WebBanHang (Solution)
│
├── WebBanHang (Main MVC Project)
│   ├── Areas/
│   │   ├── Admin/
│   │   │   ├── Controllers/
│   │   │   │   ├── DashboardController.cs
│   │   │   │   ├── CategoriesController.cs
│   │   │   │   ├── FoodsController.cs
│   │   │   │   ├── OrdersController.cs
│   │   │   │   └── UsersController.cs
│   │   │   └── Views/
│   │   │       ├── Dashboard/
│   │   │       ├── Categories/
│   │   │       ├── Foods/
│   │   │       ├── Orders/
│   │   │       └── Users/
│   │   │
│   │   └── Customer/
│   │       ├── Controllers/
│   │       │   ├── HomeController.cs
│   │       │   ├── FoodsController.cs
│   │       │   ├── CartController.cs
│   │       │   └── OrdersController.cs
│   │       └── Views/
│   │           ├── Home/
│   │           ├── Foods/
│   │           ├── Cart/
│   │           └── Orders/
│   │
│   ├── Controllers/
│   │   └── AccountController.cs (Identity Login/Register)
│   │
│   ├── DTOs/
│   │   ├── CategoryDTO.cs
│   │   ├── FoodDTO.cs
│   │   ├── CartDTO.cs
│   │   └── OrderDTO.cs
│   │
│   ├── ViewComponents/
│   │   ├── CartWidgetViewComponent.cs
│   │   └── CategoryMenuViewComponent.cs
│   │
│   ├── wwwroot/
│   │   ├── css/
│   │   ├── js/
│   │   └── images/
│   │
│   ├── Views/
│   │   ├── Shared/
│   │   │   ├── _Layout.cshtml
│   │   │   ├── _AdminLayout.cshtml
│   │   │   └── _LoginPartial.cshtml
│   │   └── Account/
│   │       ├── Login.cshtml
│   │       └── Register.cshtml
│   │
│   ├── FileUpload/
│   │   └── (Uploaded images)
│   │
│   └── Startup.cs / Program.cs
│
├── WebBanHang.BLL (Business Logic Layer)
│   └── Services/
│       ├── ICategoryService.cs
│       ├── CategoryService.cs
│       ├── IFoodService.cs
│       ├── FoodService.cs
│       ├── ICartService.cs
│       ├── CartService.cs
│       ├── IOrderService.cs
│       └── OrderService.cs
│
├── WebBanHang.DAL (Data Access Layer)
│   ├── Data/
│   │   ├── ApplicationDbContext.cs
│   │   └── DbInitializer.cs
│   │
│   ├── Repository/
│   │   ├── IRepository.cs (Generic)
│   │   ├── Repository.cs (Generic)
│   │   ├── IUnitOfWork.cs
│   │   └── UnitOfWork.cs
│   │
│   └── Migrations/
│
└── WebBanHang.Models
    ├── ApplicationUser.cs
    ├── Category.cs
    ├── Food.cs
    ├── Cart.cs
    ├── CartItem.cs
    ├── Order.cs
    ├── OrderDetail.cs
    ├── Payment.cs
    ├── Review.cs
    ├── Voucher.cs
    └── ActivityLog.cs
